using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region 변수
    [SerializeField] private Unit unit;
    #endregion


    #region 프로퍼티
    public Unit Unit => unit;
    #endregion


    #region 함수

    private void OnTriggerEnter(Collider other)
    {
        var minion = other.GetComponent<Minion>();

        if (minion == null) return;
        if (minion.Team == unit.Team) return;

        var buffSpeed = minion.buffSpeed;
        buffSpeed -= 1f / ((float)Unit.Level * 10f);

        if(buffSpeed < -1f)
        {
            buffSpeed = -1f;
        }

        minion.buffSpeed = buffSpeed;
    }

    private void OnTriggerExit(Collider other)
    {
        var minion = other.GetComponent<Minion>();

        if (minion == null) return;
        if (minion.Team == unit.Team) return;

        minion.buffSpeed += 1f / ((float)Unit.Level * 10f);
    }

    #endregion
}
