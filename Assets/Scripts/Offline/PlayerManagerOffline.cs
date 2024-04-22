using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerOffline : MonoBehaviour
{
    [SerializeField] private int token = 25000;
    [SerializeField] public List<int> handTiles;
    [SerializeField] public List<int> winningTiles;
    [SerializeField] public List<GameObject> playerTiles;
    [SerializeField] private GameManagerOffline gameManager;
    [SerializeField] private bool isReady = false;
    [SerializeField] private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManagerOffline").GetComponent<GameManagerOffline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isReady)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isReady = true;
            }
        }

        if (isActive)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                EndTurn();
            }
        }
    }

    public bool IsReady()
    {
        return isReady;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetActive(bool x)
    {
        isActive = x;
    }

    public void EndTurn()
    {
        isActive = false;
        handTiles.Sort();
        SortTiles();

        gameManager.ChangeTurn();
    }

    public void RemoveTile(GameObject tile)
    {
        gameManager.AddDiscard(tile);
        handTiles.Remove(tile.GetComponent<DisplayTile>().displayId);
        playerTiles.Remove(tile);
    }

    public void SortTiles()
    {
        playerTiles.Sort((x, y) => x.GetComponent<DisplayTile>().displayId.CompareTo(y.GetComponent<DisplayTile>().displayId));

        for (int i = 0; i < playerTiles.Count; i++)
        {
            playerTiles[i].transform.SetSiblingIndex(i);
        }
    }
}
