using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    #region 변수
    /// <summary>
    /// 숫자가 같으면 팀, 다르면 적
    /// </summary>
    [SerializeField] private int team;

    /// <summary> 
    /// 미니언 이동 방향       <br/>
    /// true    : 좌에서 우로  <br/>
    /// false   : 우에서 좌로  <br/>
    /// </summary>
    [SerializeField] private bool dir;

    /// <summary>
    /// RootManager의 루트를 참조할 Indx값
    /// </summary>
    [SerializeField] private int progress;

    [SerializeField] private int root;


    [SerializeField] private int currentHP;
    public int buffHP;
    public float buffSpeed = 0;

    public UnityAction successEvent;

    private IEnumerator moveCor;

    [SerializeField] private Image hpBar;
    #endregion


    #region 프로퍼티
    public int Team => team;
    #endregion


    #region 함수



    public void Run(int team, bool dir, int root)
    {
        currentHP = PlayerManager.Instance.GetPlayerInfo(team).upgradeStats[(int)Enums.MinionUpgrade.HP];

        gameObject.SetActive(true);
        hpBar.gameObject.SetActive(true);
        hpBar.color = GameController.Instance.GetTeamColor(team);
        Hit(0);
        if (dir)
        {
            progress = 0;
        }
        else
        {
            progress = RootManager.Instance.GetPointCount(root) - 1;
        }
        this.team = team;
        this.dir = dir;
        this.root = root;

        if(moveCor != null)
        {
            StopCoroutine(moveCor);
        }
        moveCor = MoveCor();
        StartCoroutine(moveCor);
    }
    public void MoveStop()
    {
        if (moveCor != null)
        {
            StopCoroutine(moveCor);
        }
    }

    public bool MoveRestart()
    {
        if(moveCor != null)
        {
            StartCoroutine(moveCor);
            return true;
        }

        return false;
    }

    private IEnumerator MoveCor()
    {
        while (!PlayerManager.Instance) yield return null;
        yield return null;

        {
            var pos = transform.position;
            pos = RootManager.Instance.RootPos(root, progress);
            pos.y = transform.position.y;
            transform.position = pos;
        }

        while (true)
        {
            yield return null;

            int nextIndx;
            if (dir)
            {
                nextIndx = progress + 1;
            }
            else
            {
                nextIndx = progress - 1;
            }

            if(RootManager.Instance.OutOfRoot(root, nextIndx))
            {
                Success();
                yield break;
            }

            bool destTouch = false;
            var destPos = RootManager.Instance.RootPos(root, nextIndx);
            destPos.y = 0;
            var startPos = transform.position;
            startPos.y = 0;
            var destLength = (destPos - startPos).sqrMagnitude;
            while (!destTouch)
            {
                var currentPos = transform.position;
                currentPos.y = 0;
                currentPos += (destPos - currentPos).normalized * Time.deltaTime *(1+buffSpeed)*(float)(1+PlayerManager.Instance.GetPlayerInfo(team).upgradeStats[(int)Enums.MinionUpgrade.Speed]*0.2f);
                currentPos.y = transform.position.y;
                transform.position = currentPos;
                
                if(destLength <= (transform.position - startPos).sqrMagnitude)
                {
                    progress = nextIndx;
                    destTouch = true;
                }
                yield return null;
            }
        }
    }

    private void Success()
    {
        var rival = PlayerManager.Instance.GetPlayerInfo(((int)Team + 1) % 2);
        rival.hp -= 1;
        GameController.Instance.UI.UpdateUI();
        if(rival.hp <= 0)
        {
            GameController.Instance.GameEnd();
        }

        StopAllCoroutines();
        Death();
    }

    public void Hit(int damage)
    {
        currentHP -= damage;
        if(currentHP <= 0)
        {
            Death();
            return;
        }
        var scale = Vector3.one;
        scale.x = (float)currentHP / GameManager.Instance.MinionMaxHP;

        if (hpBar != null)
            hpBar.transform.localScale = scale;
    }

    public void Death()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // Max값을 넘지 않음
    public void Heal(int addHP)
    {
        var nextHP = (currentHP - buffHP) + addHP;
        var maxHP = PlayerManager.Instance.GetPlayerInfo(team).upgradeStats[(int)Enums.MinionUpgrade.HP];

        if(nextHP > maxHP)
        {
            nextHP = maxHP;
        }

        currentHP = nextHP + buffHP;
        Hit(0);
    }

    // Max값을 넘어갈 수 있음
    public void AddHP(int addHP)
    {
        buffHP += addHP;
        currentHP += addHP;
        Hit(0);
    }

    #endregion
}
