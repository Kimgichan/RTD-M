using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region ����
    [SerializeField] private bool isDrag;
    [SerializeField] private Unit dragUnit;
    [SerializeField] private Unit targetUnit;
    [SerializeField] private Unit pointUnit;
    #endregion


    #region ������Ƽ
    #endregion


    #region �Լ�
    private void Start()
    {
        dragUnit.SetAlpha(0f);
    }


    #region �������̽�
    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = false;
        pointUnit = null;

        var ray = Camera.main.ScreenPointToRay(eventData.position);
        var result = Physics.RaycastAll(ray, 400f, 1 << LayerMask.NameToLayer("Unit"));

        for (int i = 0, icount = result.Length; i < icount; i++)
        {
            var target = result[i];
            var block = target.transform.GetComponent<Block>();

            if (block == null) continue;

            //Debug.Log(block.Unit.name);
            pointUnit = block.Unit;
            pointUnit.IsShowRange = true;
            break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDrag)
        {
            return;
        }

        if (pointUnit == null) return;

        if ((!pointUnit.IsDrag) && (!pointUnit.IsUI))
        {
            GameController.Instance.UI.ShowUnitInfo(pointUnit);
        }
        pointUnit.IsShowRange = false;
        pointUnit = null;

        //var ray = Camera.main.ScreenPointToRay(eventData.position);
        //var result = Physics.RaycastAll(ray, 400f, 1 << LayerMask.NameToLayer("Unit"));

        //for (int i = 0, icount = result.Length; i < icount; i++)
        //{
        //    var target = result[i];
        //    var block = target.transform.GetComponent<Block>();

        //    if (block == null) continue;

        //    //Debug.Log(block.Unit.name);
        //    break;
        //}

        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;

        if (pointUnit != null)
        {
            pointUnit.IsShowRange = false;
            pointUnit = null;
        }

        if (GameController.Instance.Turn == Enums.GameTurn.Fight)
        {
            return;
        }

        var gr = GameController.Instance.UI.MainCanvas.GetComponent<GraphicRaycaster>();
        
        var uiResults = new List<RaycastResult>();
        gr.Raycast(eventData, uiResults);

        for(int i = 0, icount = uiResults.Count; i<icount; i++)
        {
            var ui = uiResults[i];
            var unit = ui.gameObject.GetComponent<Unit>();
            if(unit != null)
            {
                dragUnit.Setting(unit);
                dragUnit.SetAlpha(0.5f);
                targetUnit = unit;
                return;
            }
        }


        var ray = Camera.main.ScreenPointToRay(eventData.position);
        var result = Physics.RaycastAll(ray, 400f, 1 << LayerMask.NameToLayer("Unit"));

        for (int i = 0, icount = result.Length; i < icount; i++)
        {
            var target = result[i];
            var block = target.transform.GetComponent<Block>();

            if (block == null) continue;
            if ((int)block.Unit.Team != (int)GameController.Instance.Turn) continue;
 
            dragUnit.Setting(block.Unit);
            dragUnit.SetAlpha(0.5f);
            targetUnit = block.Unit;
            return;
        }

        dragUnit.Setting(null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragUnit.UnitData == null) return;

        var pos = (Vector3)eventData.position;
        pos.z = 100f;
        var worldPos = Camera.main.ScreenToWorldPoint(pos);

        dragUnit.transform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragUnit.UnitData == null) return;


        var ray = Camera.main.ScreenPointToRay(eventData.position);
        var result = Physics.RaycastAll(ray, 400f, 1 << LayerMask.NameToLayer("Unit") | 1 << LayerMask.NameToLayer("Slot"), QueryTriggerInteraction.Collide);

        Block block = null;
        GameObject slot = null;

        bool blockCheck = false;
        for (int i = 0, icount = result.Length; i < icount; i++)
        {
            var target = result[i];
            block = target.transform.GetComponent<Block>();

            if (block != null)
            {
                if (block.Unit == targetUnit)
                {
                    block = null;
                }
                else
                    blockCheck = true;
            }


            if (block != null && (int)block.Unit.Team == (int)GameController.Instance.Turn)
            {
                break;
            }

            if(slot == null && target.transform.gameObject.layer == LayerMask.NameToLayer("Slot"))
            {
                slot = target.transform.gameObject;
            }
        }

        if (blockCheck)
        {
            slot = null;
        }

        if(block != null)
        {
            if (targetUnit.IsUI) // targetUnit�� UI��� ���(Swap, Upgrade �̺�Ʈ)
            {
                dragUnit.Setting(null);
                return;
            }
            else if (block.Unit.Upgrade(dragUnit)) // ���� ����, ���� Ÿ���� Unit�̶�� ���׷��̵�
            {
                Destroy(targetUnit.gameObject);
                dragUnit.Setting(null);
                return;
            }
            else // ���� ������ �ƴϰų� || ���� Ÿ���� Unit�� �ƴ϶�� ��ġ Swap
            {
                //if ((int)block.Unit.Team == (int)GameController.Instance.Turn)
                //{
                //    var pos = block.Unit.transform.position;
                //    block.Unit.transform.position = targetUnit.transform.position;
                //    targetUnit.transform.position = pos;

                //    block.Unit.Setting(block.Unit);
                //    targetUnit.Setting(targetUnit);
                //}
                dragUnit.Setting(null);
                return;
            }
        }
        
        if(slot != null)
        {
            if (targetUnit.IsUI) // UI��� �󽽷Կ� Unit �߰�
            {
                if(PlayerManager.Instance.GetPlayerInfo((int)GameController.Instance.Turn).gold < 100)
                {
                    dragUnit.Setting(null);
                    return;
                }

                PlayerManager.Instance.GetPlayerInfo((int)GameController.Instance.Turn).gold -= 100;
                GameController.Instance.UI.Gold = PlayerManager.Instance.GetPlayerInfo((int)GameController.Instance.Turn).gold;

                var unit = SpawnManager.Instance.SpawnObject(Enums.SpawnObject.Unit).GetComponent<Unit>();

                var pos = unit.transform.position;
                pos.x = slot.transform.position.x;
                pos.z = slot.transform.position.z;
                unit.transform.position = pos;
                unit.Setting(targetUnit);
                targetUnit.Setting(null);
            }
            else // UI�� �ƴ϶�� �󽽷Կ� targetUnit ��ġ�� ����
            {
                //var pos = targetUnit.transform.position;
                //pos.x = slot.transform.position.x;
                //pos.z = slot.transform.position.z;
                //targetUnit.transform.position = pos;
                //targetUnit.Setting(targetUnit);
            }
        }
        dragUnit.Setting(null);
    }
    #endregion
    #endregion
}
