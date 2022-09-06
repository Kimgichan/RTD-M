using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    #region 변수
    [SerializeField] private UnitData unitData;
    /// <summary>
    /// 값이 같으면 팀, 다르면 적
    /// </summary>
    [SerializeField] private int team;
    [SerializeField] private int level;

    [SerializeField] private Text levelTxt = null;

    /// <summary>
    /// 초당 발사 수
    /// </summary>
    //[SerializeField] private int shootCount;

    //[SerializeField] private float range;

    //[SerializeField] private int power;

    private HashSet<Minion> targets;
    [SerializeField] private List<Minion> targetView;

    private IEnumerator attackCor;

    [SerializeField] private bool isDrag;
    [SerializeField] private bool isUI;

    [SerializeField] private MeshRenderer sphere;
    [SerializeField] private float colliderRadius;
    [SerializeField] private float sphereRadius;

    [SerializeField] private LineRenderer lineRenderer;
    private IEnumerator shootTimerCor;
    private WaitForSeconds wait;

    [SerializeField] private MeshRenderer plane;
    #endregion


    #region 프로퍼티
    public UnitData UnitData 
    {
        get => unitData;
        private set
        {
            unitData = value;

            if (!IsDrag && IsUI)
            {
                var rawImage = GetComponent<RawImage>();
                rawImage.texture = unitData.MainTex;
                return;
            }

            var mat = GetComponent<MeshRenderer>().material;
            mat.SetTexture("_MainTex", unitData.MainTex);
        }
    }
    public int Level 
    {
        get => level;
        private set
        {
            level = value;

            if (IsDrag || IsUI)
            {
                if(levelTxt != null)
                {
                    levelTxt = null;
                    Destroy(levelTxt);
                }
                return;
            }

            if (levelTxt == null)
            {
                levelTxt = SpawnManager.Instance.SpawnObject(Enums.SpawnObject.LevelTxt).GetComponent<Text>();
            }

            if (levelTxt != null)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                var canvas = GameController.Instance.UI.MainCanvas;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera, out Vector2 outLocalPos))
                {
                    (levelTxt.transform as RectTransform).localPosition = outLocalPos;
                }

                levelTxt.text = $"{level}";
            }
        }
    }
    public int Team
    {
        get => team;
        private set
        {
            team = value;

            if (!IsDrag && IsUI) return;

            var mat = GetComponent<MeshRenderer>().material;
            mat.SetColor("_Color", GameController.Instance.GetTeamColor(team));
        }
    }

    public bool IsUI => isUI;
    public bool IsDrag => isDrag;

    public bool IsShowRange
    {
        get => sphere.enabled;
        set => sphere.enabled = value;
    }
    #endregion


    #region 함수
    private void Start()
    {
        targets = new HashSet<Minion>();
        if (!IsDrag && IsUI)
        {
            var db = GameManager.Instance.UnitDatabase;
            UnitData = db.GetData((Enums.Unit)Random.Range(0, db.GetCount()));
            Team = Team;
            Level = Level;
        }

        if (IsDrag || IsUI) return;

        GameController.Instance.AddUnit(this);
        sphere.enabled = false;

        lineRenderer.enabled = false;
        wait = new WaitForSeconds(0.25f);

        StartCoroutine(ClearTargetMissing());
    }


    public void Setting(Unit unit)
    {
        if(unit == null)
        {
            if (!IsDrag && IsUI)
            {
                var db = GameManager.Instance.UnitDatabase;
                UnitData = db.GetData((Enums.Unit)Random.Range(0, db.GetCount()));
                Team = (int)GameController.Instance.Turn;
                Level = Level;
                return;
            }

            unitData = null;
            SetAlpha(0f);
            return;
        }

        UnitData = unit.unitData;
        Team = (int)GameController.Instance.Turn;
        Level = unit.level;

        if (IsDrag) return;

        AttackStop();
        AttackRun();

        RangeUpdate();
    }

    public bool Upgrade(Unit unit)
    {
        if(unit.UnitData != UnitData || unit.Level != Level || unit.Team != Team)
        {
            return false;
        }

        if (unit.Level >= 5 || Level >= 5) return false;

        Level += 1;

        if (IsDrag) return false;

        AttackStop();
        AttackRun();

        RangeUpdate();
        return true;
    }

    private void RangeUpdate()
    { 
        var col = GameController.Instance.GetTeamColor(Team);
        col.a *= 0.5f;
        sphere.material.SetColor("_Color", col);

        float scale = (float)Level;
        var collider = GetComponent<SphereCollider>();
        collider.radius = scale * colliderRadius;
        var scale3 = sphere.transform.localScale;
        var scaleXZ = scale * sphereRadius;
        scale3.x = scaleXZ;
        scale3.z = scaleXZ;
        sphere.transform.localScale = scale3;

        lineRenderer.material.SetColor("_Color", col);

        col.a = 1f;
        plane.material.SetColor("_Color", Color.white);
    }

    public void AttackRun()
    {
        if (UnitData == null) return;
        if (attackCor == null)
        {
            attackCor = AttackCor();
        }
        StartCoroutine(attackCor);
    }
    public void AttackStop()
    {
        if (UnitData == null) return;
        if (attackCor != null)
        {
            StopCoroutine(attackCor);
            attackCor = null;
        }
    }
    private IEnumerator AttackCor()
    {
        while (targets == null) yield return null;

        var attackTimer = 1f / (float)(UnitData.ShootCount + (Level - 1) * UnitData.UpgradeShootCount);
        var cooltime = new WaitForSeconds(attackTimer);

        while (true)
        {
            if (targetView.Count > 0)
            {
                var indx = Random.Range(0, targetView.Count);
                var attackMinion = targetView[indx];

                if (attackMinion != null && attackMinion.gameObject.activeSelf)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, attackMinion.transform.position);
                    attackMinion.Hit(UnitData.Power + (Level - 1) * UnitData.UpgradePower);

                    if(shootTimerCor != null)
                    {
                        StopCoroutine(shootTimerCor);
                    }
                    shootTimerCor = ShootTimerCor();
                    StartCoroutine(ShootTimerCor());
                }
                else
                {
                    targetView.RemoveAt(indx);
                }
            }
            yield return cooltime;
        }
    }

    private IEnumerator ClearTargetMissing()
    {
        int i = 0;
        while(targetView.Count > 0)
        {
            if (targetView[i] == null)
            {
                targetView.RemoveAt(i);
                targets.RemoveWhere(f => f == null);
            }
            ++i;
            if(i >= targetView.Count)
            {
                i = 0;
            }
            yield return null;
        }
    }

    private IEnumerator ShootTimerCor()
    {
        yield return wait;

        if (lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
        }

        shootTimerCor = null;
    }



    private void OnTriggerEnter(Collider other)
    {
        var minion = other.gameObject.GetComponent<Minion>();

        if (minion == null) return;
        if (minion.Team == team)
        {
            UnitData.Buff(this, minion);
            return;
        }

        if (!targets.Contains(minion))
        {
            targets.Add(minion);
            targetView.Add(minion);

            var gameobject = gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var minion = other.gameObject.GetComponent<Minion>();

        if (minion == null) return;
        if (minion.Team == team) return;

        TargetRemove(minion);
    }

    private void TargetRemove(Minion minion)
    {
        targetView.Remove(minion);
        targets.Remove(minion);
    }


    public void SetAlpha(float a)
    {
        if (!IsDrag && IsUI) return;

        var mat = GetComponent<MeshRenderer>().material;

        var col = mat.GetColor("_Color");
        col.a = a;
        mat.SetColor("_Color", col);
    }

    private void OnDestroy()
    {
        if(levelTxt != null)
        {
            Destroy(levelTxt.gameObject);
        }
    }

    #endregion
}
