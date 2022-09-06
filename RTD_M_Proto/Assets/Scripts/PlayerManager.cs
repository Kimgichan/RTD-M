using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nodes;

public class PlayerManager : MonoBehaviour
{
    #region ����
    private static PlayerManager instance;
    [SerializeField] private List<Player> players;
    #endregion

    #region ������Ƽ
    public static PlayerManager Instance => instance;
    #endregion

    #region �Լ�
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Player GetPlayerInfo(int indx) => players[indx];
    public void SetPlayerInfo(int indx, Player info)
    {
        players[indx] = info;
    }
    public int PlayerCount() => players.Count;


    public void UpgradeStat(Enums.MinionUpgrade upgradeStat)
    {
        if (GameController.Instance.Turn == Enums.GameTurn.Fight) return;

        var player = players[(int)GameController.Instance.Turn];
        player.upgradePays[(int)upgradeStat] += 20;
        if (upgradeStat == Enums.MinionUpgrade.Amount)
            player.upgradeStats[(int)upgradeStat] += 2;
        else if (upgradeStat == Enums.MinionUpgrade.HP)
            player.upgradeStats[(int)upgradeStat] += 4;
        else
            player.upgradeStats[(int)upgradeStat] += 1;
    }

    #endregion
}
