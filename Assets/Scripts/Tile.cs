using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class Tile
{
    public int id;
    public string tileType;
    public string tileName;
    public Sprite tileImage;

    public Tile()
    {

    }

    public Tile(int Id, string TileType, string TileName, Sprite TileImage)
    {
        this.id = Id;
        this.tileType = TileType;
        this.tileName = TileName;
        this.tileImage = TileImage;
    }
}
