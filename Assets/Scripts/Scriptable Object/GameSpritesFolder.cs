using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CellImageFolder", menuName = "Scriptable Objects/CellImageFolder")]
public class GameSpritesFolder : ScriptableObject
{
    [SerializeField] private List<Sprite> _cellSprites;

    public List<Sprite> GetCellSpriteList()
    {
        List<Sprite> cellSprites = new List<Sprite>();
        cellSprites.AddRange(_cellSprites);
        return cellSprites;
    }
}
