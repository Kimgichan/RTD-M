using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSlot : MonoBehaviour
{
    #region 변수
    [SerializeField] MeshRenderer _renderer;
    [SerializeField] BoxCollider colider;
    #endregion


    #region 프로퍼티
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


    #region 함수
    #endregion

}
