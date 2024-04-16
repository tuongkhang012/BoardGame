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

        tileList.Add(new Tile(0, "1p", "1", getSpriteByName("1p")));
        tileList.Add(new Tile(1, "2p", "2", getSpriteByName("2p")));
        tileList.Add(new Tile(2, "3p", "3", getSpriteByName("3p")));
        tileList.Add(new Tile(3, "4p", "4", getSpriteByName("4p")));
        tileList.Add(new Tile(4, "5p", "5", getSpriteByName("5p")));
        tileList.Add(new Tile(5, "0p", "5", getSpriteByName("0p")));
        tileList.Add(new Tile(6, "6p", "6", getSpriteByName("6p")));
        tileList.Add(new Tile(7, "7p", "7", getSpriteByName("7p")));
        tileList.Add(new Tile(8, "8p", "8", getSpriteByName("8p")));
        tileList.Add(new Tile(9, "9p", "9", getSpriteByName("9p")));
        tileList.Add(new Tile(10, "1s", "1", getSpriteByName("1s")));
        tileList.Add(new Tile(11, "2s", "2", getSpriteByName("2s")));
        tileList.Add(new Tile(12, "3s", "3", getSpriteByName("3s")));
        tileList.Add(new Tile(13, "4s", "4", getSpriteByName("4s")));
        tileList.Add(new Tile(14, "5s", "5", getSpriteByName("5s")));
        tileList.Add(new Tile(15, "0s", "5", getSpriteByName("0s")));
        tileList.Add(new Tile(16, "6s", "6", getSpriteByName("6s")));
        tileList.Add(new Tile(17, "7s", "7", getSpriteByName("7s")));
        tileList.Add(new Tile(18, "8s", "8", getSpriteByName("8s")));
        tileList.Add(new Tile(19, "9s", "9", getSpriteByName("9s")));
        tileList.Add(new Tile(20, "1m", "1", getSpriteByName("1m")));
        tileList.Add(new Tile(21, "2m", "2", getSpriteByName("2m")));
        tileList.Add(new Tile(22, "3m", "3", getSpriteByName("3m")));
        tileList.Add(new Tile(23, "4m", "4", getSpriteByName("4m")));
        tileList.Add(new Tile(24, "5m", "5", getSpriteByName("5m")));
        tileList.Add(new Tile(25, "0m", "5", getSpriteByName("0m")));
        tileList.Add(new Tile(26, "6m", "6", getSpriteByName("6m")));
        tileList.Add(new Tile(27, "7m", "7", getSpriteByName("7m")));
        tileList.Add(new Tile(28, "8m", "8", getSpriteByName("8m")));
        tileList.Add(new Tile(29, "9m", "9", getSpriteByName("9m")));
        tileList.Add(new Tile(30, "1z", "E", getSpriteByName("1z")));
        tileList.Add(new Tile(31, "2z", "S", getSpriteByName("2z")));
        tileList.Add(new Tile(32, "3z", "W", getSpriteByName("3z")));
        tileList.Add(new Tile(33, "4z", "N", getSpriteByName("4z")));
        tileList.Add(new Tile(34, "5z", "Wh", getSpriteByName("5z")));
        tileList.Add(new Tile(35, "6z", "G", getSpriteByName("6z")));
        tileList.Add(new Tile(36, "7z", "R", getSpriteByName("7z")));

    }

    Sprite getSpriteByName(string name)
    {
        
        for (int i = 0; i < tileImages.Count; i++)
        {
            if (tileImages[i].ToString() == (name + " (UnityEngine.Sprite)"))
            {
                return tileImages[i];
            }
        }
        return null;
    }
}
