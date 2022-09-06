using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object / Unit / Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    #region 변수
    private static List<UnityAction<Unit, Minion>> buffs = new List<UnityAction<Unit, Minion>>()
    {
        Buff_A,
        Buff_B,
        Buff_C,
    };

    [SerializeField] private Enums.Unit kind;
    [SerializeField] private Texture mainTex;
    [SerializeField] private int power;
    [SerializeField] private int upgradePower;
    [SerializeField] private int shootCount;
    [SerializeField] private int upgradeShootCount;
    [SerializeField] private int range;
    [SerializeField] private int upgradeRange;
    [TextArea] [SerializeField] private string content;
    #endregion


    #region 프로퍼티
    public Enums.Unit Kind => kind;
    public Texture MainTex => mainTex;
    public int Power => power;
    public int UpgradePower => upgradePower;
    public int ShootCount => shootCount;
    public int UpgradeShootCount => upgradeShootCount;
    public int Range => range;
    public int UpgradeRange => upgradeRange;
    public string Content => content;
    #endregion


    #region 함수
    public static void Buff(Unit unit, Minion minion)
    {
        buffs[(int)unit.UnitData.Kind](unit, minion);
    }
    private static void Buff_A(Unit unit, Minion minion)
    {
        if(unit.Team == minion.Team)
        {
            minion.AddHP(unit.Level);
        }
    }
    private static void Buff_B(Unit unit, Minion minion)
    {
        if (unit.Team == minion.Team)
        {
            minion.buffSpeed += (float)unit.Level*0.2f;
        }
    }
    private static void Buff_C(Unit unit, Minion minion)
    {
        if (unit.Team == minion.Team)
        {
            minion.Heal(unit.Level);
        }
    }
    #endregion
}
