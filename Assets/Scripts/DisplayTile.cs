using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTile : MonoBehaviour
{
    public Tile displayTile;
    public int displayId;

    public int id;
    public string tileType;
    public string tileName;
    public Sprite tileImage;

    public TextMeshProUGUI nameText;
    public Image frontImage;

    void Start()
    {
        displayTile = TileDatabase.tileList[displayId];

        id = displayTile.id;
        tileType = displayTile.tileType;
        tileName = displayTile.tileName;
        tileImage = displayTile.tileImage;

        nameText.text = tileName;
        frontImage.sprite = tileImage;
    }
    void Update()
    {
        
    }
}
