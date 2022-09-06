using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enums;

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "Scriptable Object / Unit / Database", order = int.MaxValue)]
public class UnitDatabase : ScriptableObject
{
    #region 변수
    [SerializeField] private List<UnitData> datas;
    #endregion


    #region 프로퍼티
    #endregion


    #region 함수

    public int GetCount() => datas.Count;
    public UnitData GetData(Enums.Unit unit)
    {
        for(int i = 0, icount = datas.Count; i<icount; i++)
        {
            var data = datas[i];

            if(data.Kind == unit)
            {
                return data;
            }
        }

        return null;
    }

    #endregion
}
