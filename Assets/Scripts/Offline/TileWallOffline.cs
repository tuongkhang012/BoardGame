using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileWallOffline : MonoBehaviour
{
    public List<int> wall = new List<int>();
    public List<int> dora = new List<int>();

    public int fullSize = 136;
    public int doraSize = 14;
    public int handSize = 13;
    public int wallSize = 70;

    public TextMeshProUGUI wallSizeText;
    public GameManagerOffline gameManager;

    private void Awake()
    {
        Debug.Log("TileWallOffline Awake()");
        gameManager = GameObject.Find("GameManagerOffline").GetComponent<GameManagerOffline>();
    }

    private void Start()
    {
        Create();
        wallSizeText.text = "x" + wall.Count.ToString();
    }

    void Update()
    {
        wallSizeText.text = "x" + wall.Count.ToString();

        if (wall.Count == 0)
        {
            gameManager.ChangeToEndGame();
        }
    }

    void Create()
    {
        // Creating the deck with 4 of each tiles (with the exception of the 5, it would have one 0)
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < TileDatabase.tileList.Count; j++)
            {
                if(j == 9 || j == 19 || j == 29)
                {
                    continue;
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

    public int DrawTile()
    {
        if (wall.Count == 0)
        {
            gameManager.ChangeToEndGame();
            return -1;
        }

        int tile = wall[wall.Count - 1];
        wall.RemoveAt(wall.Count - 1);

        return tile;
    }
}
