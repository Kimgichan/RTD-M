using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nodes;

public class RootManager : MonoBehaviour
{
    #region ���� 
    /// <summary>
    /// ��� : DontDestroyOnLoad ��ü�� �ƴ�
    /// </summary>
    private static RootManager instance;

    /// <summary>
    /// ��Ʈ�� ������ �� �¿��� �� �������� ������ ��
    /// </summary>
    [SerializeField] private List<Root> roots;
    #endregion


    #region ������Ƽ
    public static RootManager Instance => instance;
    #endregion


    #region �Լ�
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    public Vector3 RootPos(int root, int indx)
    {
        return roots[root].point[indx].transform.position;
    }


    /// <summary>
    /// ��Ʈ ������ ����� True
    /// �׷��� �ʴٸ� False ��ȯ
    /// </summary>
    /// <param name="root"></param>
    /// <param name="indx"></param>
    /// <returns></returns>
    public bool OutOfRoot(int root, int indx)
    {
        if(root >= 0 && root < roots.Count)
        {
            if(indx >= 0 && indx < roots[root].point.Count)
            {
                return false;
            }
        }
        return true;
    }

    public int GetPointCount(int root)
    {
        return roots[root].point.Count;
    }
    #endregion
}
