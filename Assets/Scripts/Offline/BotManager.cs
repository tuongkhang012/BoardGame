using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    [SerializeField] private int token = 25000;
    [SerializeField] public List<int> handTiles;
    [SerializeField] public List<GameObject> botTiles;
    [SerializeField] private GameManagerOffline gameManager;
    [SerializeField] private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManagerOffline").GetComponent<GameManagerOffline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Debug.Log("Bot is active :)");
            isActive = false;
            handTiles.Sort();
            RemoveTile();
            gameManager.ChangeTurn();
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetActive(bool x)
    {
        isActive = x;
    }

    public void RemoveTile()
    {
        GameObject choice = botTiles[Random.Range(0, botTiles.Count)];
        handTiles.Remove(choice.GetComponent<DisplayTile>().displayId);
        gameManager.BotDiscard(choice);
        botTiles.Remove(choice);
    }
}
