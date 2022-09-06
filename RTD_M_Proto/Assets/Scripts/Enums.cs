using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum MinionUpgrade
    {
        Amount,
        Speed,  
        HP,
    }

    public enum SpawnObject
    {
        Slot,
        Minion,
        LevelTxt,
        Unit,
    }

    public enum Unit
    {
        A, // ����
        B, // ���׶��
        C, // ������
    }

    public enum GameTurn 
    {
        Red, // ���� ����
        Blue, // ��� ����
        Fight, // ���� ����
    }
}
