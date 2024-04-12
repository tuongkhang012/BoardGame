using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDatabase : MonoBehaviour
{
    public static List<Tile> tileList = new List<Tile>();
    public static List<Sprite> tileImages = new List<Sprite>();

    void Awake()
    {
        tileImages = new List<Sprite>(Resources.LoadAll<Sprite>(""));

        tileList.Add(new Tile(0, "0p", "5", tileImages[0]));
        tileList.Add(new Tile(1, "1p", "1", tileImages[1]));
        tileList.Add(new Tile(2, "2p", "2", tileImages[2]));
        tileList.Add(new Tile(3, "3p", "3", tileImages[3]));
        tileList.Add(new Tile(4, "4p", "4", tileImages[4]));
        tileList.Add(new Tile(5, "5p", "5", tileImages[5]));
        tileList.Add(new Tile(6, "6p", "6", tileImages[6]));
        tileList.Add(new Tile(7, "7p", "7", tileImages[7]));
        tileList.Add(new Tile(8, "8p", "8", tileImages[8]));
        tileList.Add(new Tile(9, "9p", "9", tileImages[9]));

    }
}
