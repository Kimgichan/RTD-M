using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Enums;
using Nodes;

public class GameController : MonoBehaviour
{
    #region º¯¼ö
    private static GameController instance;
    [SerializeField] private ControlUI ui;
    [SerializeField] private float spawnTimer;
    [SerializeField] private Texture minionTex;
    [SerializeField] private List<Color> teamCols;

    [SerializeField] private List<Minion> minions;
    [SerializeField] private List<Unit> units;

    private IEnumerator waveCor0;
    private IEnumerator waveCor1;

    private int turn;
    #endregion


    #region ÇÁ·ÎÆÛÆ¼
    public static GameController Instance => instance;
    public bool IsReady
    {
        get
        {
            return true;
        }
    }

    public ControlUI UI => ui;

    public GameTurn Turn
    {
        get
        {
            return (GameTurn)(turn % 3);
        }
    }
    public int WaveCount => UI.Wave;
    #endregion


    #region ÇÔ¼ö

    private IEnumerator Start()
    {
        UI.Wave = 0;
        turn = 0;
        UI.Turn = Turn;

        while (PlayerManager.Instance == null) yield return true;
        yield return null;
        CanvasController.NextUnitUpdate();

        for (int i = 0; i<2; i++)
        {
            PlayerManager.Instance.GetPlayerInfo(i).gold = 400;
            PlayerManager.Instance.GetPlayerInfo(i).hp = 10;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[0] = 10;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[1] = 0;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[2] = 4;

            for (int j = 0; j < 3; j++)
            {
                PlayerManager.Instance.GetPlayerInfo(i).upgradePays[j] = 100;
            }
        }
        UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
        //while (IsReady)
        //{
        //    yield return new WaitForSeconds(5f);

        //    RoundStart();
        //}
        UI.UpdateUI();
    }

    public void GameEnd()
    {
        StopAllCoroutines();
        // ·¹µåÆÀ ½Â¸®
        if(PlayerManager.Instance.GetPlayerInfo(0).hp > 0)
        {
            Debug.Log("·¹µåÆÀ ½Â¸®");
        }
        else // ºí·çÆÀ ½Â¸®
        {
            Debug.Log("ºí·çÆÀ ½Â¸®");
        }
        GameReset();
    }


    public void GameReset()
    {
        for(int i = 0, icount = units.Count; i<icount; i++)
        {
            if(units[i] != null)
            {
                Destroy(units[i].gameObject);
            }
        }

        units.Clear();

        for(int i = 0; i< minions.Count; i++)
        {
            if(minions[i] != null)
            {
                minions[i].Death();
            }
        }

        minions.Clear();

        UI.Wave = 0;
        turn = 0;
        CanvasController.NextUnitUpdate();
        UI.Turn = Turn;

        for (int i = 0; i < 2; i++)
        {
            PlayerManager.Instance.GetPlayerInfo(i).gold = 400;
            PlayerManager.Instance.GetPlayerInfo(i).hp = 10;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[0] = 10;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[1] = 0;
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[2] = 4;
            for(int j = 0; j<3; j++)
            {
                PlayerManager.Instance.GetPlayerInfo(i).upgradePays[j] = 100;
            }
        }
        UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
        UI.UpdateUI();
    }



    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    //private void Start()
    //{
    //    UI.Wave = 0;
    //}

    public int GetMinionCount() => minions.Count;
    public void AddUnit(Unit unit)
    {
        if (units.Find(f => unit == f)) return;
        units.Add(unit);
    }

    private void RoundStart()
    {
        UI.Wave += 1;

        waveCor0 = WaveCor_0();
        waveCor1 = WaveCor_1();

        StartCoroutine(waveCor0);
        StartCoroutine(waveCor1);

        StartCoroutine(RoundCor());
    }

    private IEnumerator RoundCor()
    {
        yield return null;

        while(minions.Count > 0 || waveCor0 != null || waveCor1 != null)
        {
            if(minions.Count > 0 && minions[minions.Count - 1] == null)
            {
                minions.RemoveAt(minions.Count - 1);
            }
            yield return null;
        }

        turn += 1;
        CanvasController.NextUnitUpdate();
        UI.Turn = Turn;

        for(int i = 0; i<2; i++)
        {
            PlayerManager.Instance.GetPlayerInfo(i).upgradeStats[(int)MinionUpgrade.HP] += 2;
        }

        UI.UpdateUI();

        RewardAddGold();

        UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
    }
    private IEnumerator WaveCor_0()
    {
        yield return null;

        var timer = new WaitForSeconds(spawnTimer/(1f +(float)PlayerManager.Instance.GetPlayerInfo(0).upgradeStats[(int)MinionUpgrade.Speed]*0.2f));
        var spawnCount = PlayerManager.Instance.GetPlayerInfo(0).upgradeStats[(int)MinionUpgrade.Amount];

        var minion0Spr = minionTex;

        while(spawnCount > 0)
        {
            spawnCount -= 1;
            var minion0 = SpawnManager.Instance.SpawnObject(Enums.SpawnObject.Minion);
            minion0.transform.localPosition = new Vector3(600f, minion0.transform.localPosition.y, 600f);
            minion0.SetActive(false);
            minion0.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", minion0Spr);
            var minion = minion0.GetComponent<Minion>();
            minion.Run(0, true, 0);
            minions.Add(minion);
            minion0.GetComponent<MeshRenderer>().material.SetColor("_Color", teamCols[0]);
            yield return timer;
        }

        waveCor0 = null;
    }

    private IEnumerator WaveCor_1()
    {
        yield return null;

        var timer = new WaitForSeconds(spawnTimer/ (1f + (float)PlayerManager.Instance.GetPlayerInfo(1).upgradeStats[(int)MinionUpgrade.Speed]*0.2f));
        var spawnCount = PlayerManager.Instance.GetPlayerInfo(1).upgradeStats[(int)Enums.MinionUpgrade.Amount];

        var minion1Spr = minionTex;

        while (spawnCount > 0)
        {
            spawnCount -= 1;
            var minion1 = SpawnManager.Instance.SpawnObject(Enums.SpawnObject.Minion);
            minion1.transform.localPosition = new Vector3(600f, minion1.transform.localPosition.y, 600f);
            minion1.SetActive(false);
            minion1.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", minion1Spr);
            var minion = minion1.GetComponent<Minion>();
            minion.Run(1, false, 1);
            minions.Add(minion);
            minion1.GetComponent<MeshRenderer>().material.SetColor("_Color", teamCols[1]);
            yield return timer;
        }

        waveCor1 = null;
    }

    public Color GetTeamColor(int team) => teamCols[team];

    public void TurnOut()
    {
        if (Turn == GameTurn.Fight) return;

        turn += 1;
        CanvasController.NextUnitUpdate();
        UI.Turn = Turn;
        if (Turn == GameTurn.Fight)
        {
            UI.Gold = 0;
            RoundStart();
            UI.UpdateUI();
        }
        else
        {
            UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
            UI.UpdateUI();
        }
    }

    public void Upgrade(int stat)
    {
        if (Turn == GameTurn.Fight) return;

        var gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
        //var pay = UI.GetUpgradePay((int)stat);
        var pay = PlayerManager.Instance.GetPlayerInfo((int)Turn).upgradePays[stat];
        if (gold >= pay)
        {
            PlayerManager.Instance.GetPlayerInfo((int)Turn).gold = gold - pay;
            PlayerManager.Instance.UpgradeStat((MinionUpgrade)stat);
            UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)Turn).gold;
            UI.UpdateUI();
        }
    }

    private void RewardAddGold()
    {
        List<int> reward = new List<int>() { 0, 0 };

        Stack<int> removeList = new Stack<int>();

        for(int i = 0, icount = units.Count; i<icount; i++)
        {
            var unit = units[i];
            if(unit == null)
            {
                removeList.Push(i);
                continue;
            }

            reward[unit.Team] += GameManager.Instance.GetUnitReward(unit.Level);
        }

        while(removeList.Count > 0)
        {
            units.RemoveAt(removeList.Pop());
        }

        for(int i = 0; i<2; i++)
        {
            PlayerManager.Instance.GetPlayerInfo(i).gold += reward[i];
        }
    }
    #endregion
}
