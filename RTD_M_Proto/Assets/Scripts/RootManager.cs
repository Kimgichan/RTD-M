using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nodes;

public class RootManager : MonoBehaviour
{
    #region 변수 
    /// <summary>
    /// 경고 : DontDestroyOnLoad 객체가 아님
    /// </summary>
    private static RootManager instance;

    /// <summary>
    /// 루트를 셋팅할 때 좌에서 우 방향으로 셋팅할 것
    /// </summary>
    [SerializeField] private List<Root> roots;
    #endregion


    #region 프로퍼티
    public static RootManager Instance => instance;
    #endregion


    #region 함수
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
    /// 루트 범위를 벗어나면 True
    /// 그렇지 않다면 False 반환
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
