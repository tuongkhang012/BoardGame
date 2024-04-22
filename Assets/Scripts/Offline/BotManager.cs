using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BotManager : MonoBehaviour
{
    [SerializeField] private int token = 25000;
    [SerializeField] public List<int> handTiles;
    [SerializeField] public List<int> winningTiles;
    [SerializeField] public List<GameObject> botTiles;
    [SerializeField] private GameManagerOffline gameManager;
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool isPrompted = false;

    private Node root;

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
        GameObject choice = botTiles[UnityEngine.Random.Range(0, botTiles.Count)];
        handTiles.Remove(choice.GetComponent<DisplayTile>().displayId);
        gameManager.BotDiscard(choice);
        botTiles.Remove(choice);

    }

    public void SortTiles()
    {
        botTiles.Sort((x, y) => x.GetComponent<DisplayTile>().displayId.CompareTo(y.GetComponent<DisplayTile>().displayId));

        for (int i = 0; i < botTiles.Count; i++)
        {
            botTiles[i].transform.SetSiblingIndex(i);
        }
    }
}

public class Node
{
    int move;
    Node parent;
    float N = 0;
    float Q = 0;
    List<Node> children;
    string outcome;

    public Node(int move, Node parent)
    {
        this.move = move;
        this.parent = parent;
        children = new List<Node>();
    }

    public float value(float exploration = 1.41f)
    {
        if (N == 0)
        {
            if (exploration == 0)
            {
                return 0;
            }
            else
            {
                return float.PositiveInfinity;
            }
        }
        return Q / N + exploration * Mathf.Sqrt(Mathf.Log(parent.N) / N);
    }
}
