using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private static CanvasController instance;
    [SerializeField] private List<Unit> nextUnits;

    private void Start()
    {
        instance = this;
    }

    public static void NextUnitUpdate()
    {
        if (instance == null) return;

        for (int i = 0; i < 2; i++)
            instance.nextUnits[i].gameObject.SetActive(false);
        if (GameController.Instance.Turn != Enums.GameTurn.Fight)
        {
            instance.nextUnits[(int)GameController.Instance.Turn].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < 2; i++)
                instance.nextUnits[i].Setting(null);
        }
    }
}
