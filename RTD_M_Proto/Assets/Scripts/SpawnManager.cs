using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Enums;

[ExecuteInEditMode]
public class SpawnManager : MonoBehaviour
{
    private static SpawnManager instance;

    #region 변수

#if UNITY_EDITOR
    [SerializeField] private List<GameObject> slots;
    [SerializeField] private bool slotUpdate;
    [SerializeField] private Vector2 size;
    [SerializeField] private int row;
    [SerializeField] private int column;
#endif

    [SerializeField] GameObject world;
    /// <summary>
    /// 프리팹
    /// </summary>
    [SerializeField] List<GameObject> spawnList;

    #endregion

    #region 프로퍼티
    public static SpawnManager Instance => instance;
    #endregion

    #region 함수

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }


            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }


    public void Init(GameObject world)
    {
        this.world = world;
    }

    public GameObject SpawnObject(int indx)
    {
        if (world == null) return null;

        GameObject obj = null;
        if (indx != (int)Enums.SpawnObject.LevelTxt)
            obj = Instantiate(spawnList[indx], world.transform);
        else obj = Instantiate(spawnList[indx], GameController.Instance.UI.MainCanvas.transform);
        return obj;
    }

    public GameObject SpawnObject(SpawnObject spawnKind)
    {
        return SpawnObject((int)spawnKind);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (slotUpdate)
        {
            slotUpdate = false;
            var unitX = size.x / (float)row;
            var unitY = size.y / (float)column;

            var destCount = row * column;


            for(int i = 0, icount = destCount - slots.Count; i<icount; i++)
            {
                var obj = Instantiate(slots[0], slots[0].transform.parent);
                //var obj = PrefabUtility.InstantiatePrefab(slots[0], slots[0].transform.parent);
                slots.Add(obj as GameObject);
            }

            for(int i = slots.Count - 1; i>=destCount && i > 0; i--)
            {
                Destroy(slots[i].gameObject);
                slots.RemoveAt(i);
            }

            for(int y = 0; y < column; y++)
            {
                for(int x = 0; x < row; x++)
                {
                    //var obj = Instantiate()
                    var slot = slots[y * row + x];
                    var posX = x * unitX + unitX * 0.5f;
                    var posY = y * unitY + unitY * 0.5f;
                    slot.transform.localPosition = new Vector3(
                        x * unitX + unitX * 0.5f, 0,
                        y * unitY + unitY * 0.5f);
                }
            }
        }
    }
#endif

    #endregion
}
