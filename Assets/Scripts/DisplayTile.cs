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
    public Sprite frontImage;
    public Sprite backImage;
    public bool isFacedDown;
    public int angle = 0;

    public TextMeshProUGUI nameText;
    public Image tileImage;

    void Start()
    {
        displayTile = TileDatabase.tileList[displayId];

        id = displayTile.id;
        tileType = displayTile.tileType;
        tileName = displayTile.tileName;
        frontImage = displayTile.tileImage;

        nameText.text = tileName;
    }

    void Update()
    {
        if (isFacedDown)
        {
            tileImage.sprite = backImage;
            nameText.enabled = false;
        }
        else
        {
            tileImage.sprite = frontImage;
            nameText.enabled = true;
        }
    }
}
