using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSlot : MonoBehaviour
{
    #region ����
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] BoxCollider colider;
    #endregion


    #region ������Ƽ
    public bool IsShow
    {
        get => _renderer.enabled;
        set => _renderer.enabled = value;
    }

    public bool isColider
    {
        get => colider.enabled;
        set => colider.enabled = value;
    }
    #endregion


    #region �Լ�
    #endregion

}
