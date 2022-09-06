using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region ����
    private static GameManager instance;
    [SerializeField] private SpriteDB spriteDB;
    [SerializeField] private UnitDatabase unitDatabase;

    /// <summary>
    /// ������ ������, ������ �� ���� ����, Unit ������ 1�̸� rewardGolds[0]���� ������ �߰��� ����
    /// </summary>
    [SerializeField] private List<int> rewardGolds;
    [SerializeField] private float minionMaxHP;

    #endregion


    #region ������Ƽ
    public static GameManager Instance => instance;
    public UnitDatabase UnitDatabase => unitDatabase;

    public float MinionMaxHP => minionMaxHP;
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

    public Texture GetTexture(string sprName)
    {
        return spriteDB.GetImage(sprName);
    }

    /// <summary>
    /// ���� ���ֿ� ���� �߰� ���� ��
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int GetUnitReward(int level)
    {
        return rewardGolds[level - 1];
    }

    #endregion
}
