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
        A, // 세모
        B, // 동그라미
        C, // 마름모
    }

    public enum GameTurn 
    {
        Red, // 레드 셋팅
        Blue, // 블루 셋팅
        Fight, // 전투 시작
    }
}
