using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteDB", menuName = "Scriptable Object / Sprite / DB", order = int.MaxValue)]
public class SpriteDB : ScriptableObject
{
    [SerializeField] private List<Texture> images;

    public Texture GetSprite(int indx)
    {
        return images[indx];
    }
    public Texture GetImage(string imageName)
    {
        for(int i = 0, icount = images.Count; i<icount; i++)
        {
            if (images[i].name.Equals(imageName))
            {
                return images[i];
            }
        }

        return null;
    }
}
