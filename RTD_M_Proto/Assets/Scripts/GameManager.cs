using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 변수
    private static GameManager instance;
    [SerializeField] private SpriteDB spriteDB;
    [SerializeField] private UnitDatabase unitDatabase;

    /// <summary>
    /// 게임이 끝나고, 정산할 때 쓰일 변수, Unit 레벨이 1이면 rewardGolds[0]값의 보상을 추가로 받음
    /// </summary>
    [SerializeField] private List<int> rewardGolds;
    [SerializeField] private float minionMaxHP;

    #endregion


    #region 프로퍼티
    public static GameManager Instance => instance;
    public UnitDatabase UnitDatabase => unitDatabase;

    public float MinionMaxHP => minionMaxHP;
    #endregion


    #region 함수
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

    public Texture GetTexture(string sprName)
    {
        return spriteDB.GetImage(sprName);
    }

    /// <summary>
    /// 보유 유닛에 따른 추가 보상 값
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetUnitReward(int level)
    {
        return rewardGolds[level - 1];
    }

    #endregion
}
