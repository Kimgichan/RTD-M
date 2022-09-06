using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Enums;

namespace Nodes
{
    [Serializable] public struct Root
    {
        public List<GameObject> point;
    }

    [Serializable] public class Player
    {
        public int gold;
        public int hp;
        [NonReorderable] public List<int> upgradeStats;
        [NonReorderable] public List<int> upgradePays;
        public List<Unit> myUnits;
    }

    [Serializable] public class ControlUI
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text wave;
        [SerializeField] private Text gold;
        [SerializeField] private List<Text> upgradeList;
        [SerializeField] private List<Text> upgradePayTxtList;

        [SerializeField] private int waveCount;
        [SerializeField] private int goldCount;

        [SerializeField] private Text turnTitle;
        [SerializeField] private Text redHP;
        [SerializeField] private Text blueHP;

        [SerializeField] private GameObject unitInfoPanel;
        [SerializeField] private Text unitName;
        [SerializeField] private Text unitContent;
        [SerializeField] private Text unitDamage;
        [SerializeField] private Text unitTeam;

        public int Wave
        {
            get
            {
                return waveCount;
            }
            set
            {
                if(value < 0)
                {
                    value = 0;
                }

                waveCount = value;
                wave.text = $"Wave : {waveCount}";
            }
        }

        public Enums.GameTurn Turn
        {
            set
            {
                turnTitle.transform.parent.gameObject.SetActive(true);
                if (value == GameTurn.Red) turnTitle.text = "레드 턴";
                else if (value == GameTurn.Blue) turnTitle.text = "블루 턴";
                else turnTitle.transform.parent.gameObject.SetActive(false);
            }
        }
        public int Gold
        {
            get
            {
                return goldCount;
            }
            set
            {
                if (value < 0) value = 0;

                goldCount = value;
                gold.text = $"Gold : {goldCount}";
            }
        }

        public Canvas MainCanvas => canvas;

        public void UpdateUI()
        {
            if (GameController.Instance.Turn != GameTurn.Fight)
            {
                for (int i = 0, icount = upgradeList.Count; i < icount; i++)
                {
                    upgradeList[i].transform.parent.parent.gameObject.SetActive(true);
                }

                var list = PlayerManager.Instance.GetPlayerInfo((int)GameController.Instance.Turn);

                upgradeList[0].text = $"숫자 : {list.upgradeStats[0]}";
                upgradeList[1].text = $"스피드 : {Mathf.Floor((1f + (float)list.upgradeStats[1] * 0.2f) * 100f) / 100f}";
                upgradeList[2].text = $"체력 : {list.upgradeStats[2]}";

                for(int j = 0; j<3; j++)
                {
                    upgradePayTxtList[j].text = $"{list.upgradePays[j]}";
                }
            }
            else
            {
                for(int i = 0, icount =upgradeList.Count; i<icount; i++)
                {
                    upgradeList[i].transform.parent.parent.gameObject.SetActive(false);
                }
            }
            redHP.text = $"Red : {PlayerManager.Instance.GetPlayerInfo(0).hp}";
            blueHP.text = $"Blue : {PlayerManager.Instance.GetPlayerInfo(1).hp}";
        }

        public void ShowUnitInfo(Unit unit)
        {
            GameController.Instance.StopCoroutine(UnitInfoFadeCor());
            GameController.Instance.StartCoroutine(UnitInfoFadeCor());

            unitName.text = unit.UnitData.name;
            unitTeam.text = $"{(Enums.GameTurn)unit.Team} 팀";
            unitContent.text = unit.UnitData.Content;
        }

        private IEnumerator UnitInfoFadeCor()
        {
            yield return null;
            var group = unitInfoPanel.GetComponent<CanvasGroup>();
            group.alpha = 1f;

            while(group.alpha > 0f)
            {
                group.alpha -= Time.deltaTime*0.5f;
                yield return null;
            }
        }
    }

    [Serializable] public struct MinionBaseInfo
    {
        public int hp;
        public int speed;
    }
}
