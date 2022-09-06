using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UnityEditor;


[ExecuteInEditMode]
public class Board : MonoBehaviour
{
    #region 변수
    private Vector2 beforePos;
    [SerializeField] private bool enter = false;
    [SerializeField] private List<ObjectSlot> slots;
    #endregion

    #region 프로퍼티

    #endregion


    #region 함수
    public void OnEnter(Vector2 pos)
    {
        beforePos = pos;
        enter = true;
    }

    public void OnExit(int indx, Vector2 pos)
    {
        if (enter)
        {
            enter = false;
            if(beforePos == pos)
            {
                var spawnManager = SpawnManager.Instance;

                if (spawnManager == null) return;

                var spawnObj = spawnManager.SpawnObject(indx);
                spawnObj.transform.localPosition = pos;
                if(spawnObj == null)
                {
                    Debug.LogError("오브젝트를 생성하지 못함");
                }
            }
        }
    }

    #endregion
}
