using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class TileWall : NetworkBehaviour
{
    public NetworkList<int> wall;
    public NetworkList<int> dora;

    public int fullSize = 136;
    public int doraSize = 14;
    public int handSize = 13;
    public int wallSize = 70;

    public TextMeshProUGUI wallSizeText;

    private void Awake()
    {
        wall = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        dora = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    public void Start()
    {
        // Creating the deck with 4 of each tiles (with the exception of the 5, it would have one 0)
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < TileDatabase.tileList.Count; j++)
            {
                if (i == 0)
                {
                    if (TileDatabase.tileList[j].tileType == "5m" || TileDatabase.tileList[j].tileType == "5s" 
                        || TileDatabase.tileList[j].tileType == "5p")
                    {
                        continue;
                    }
                }
                else
                {
                    if (TileDatabase.tileList[j].tileType == "0m" || TileDatabase.tileList[j].tileType == "0s"
                        || TileDatabase.tileList[j].tileType == "0p")
                    {
                        continue;
                    }
                }

                wall.Add(TileDatabase.tileList[j].id);
            }
        }
        Shuffle();

        // Creating the dora stack by getting the bottom 14 tiles from the wall
        for (int i = 0; i < doraSize; i++)
        {
            dora.Add(wall[wall.Count - i - 1]);
            wall.RemoveAt(wall.Count - i - 1);
        }

        wallSizeText.text = "x" + wall.Count.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        wallSizeText.text = "x" + wall.Count.ToString();
    }

    void Shuffle()
    {
        for (int i = 0; i < fullSize; i++)
        {
            int randomIndex = Random.Range(i, fullSize);
            int temp = wall[i];
            wall[i] = wall[randomIndex];
            wall[randomIndex] = temp;
        }
    }
}
