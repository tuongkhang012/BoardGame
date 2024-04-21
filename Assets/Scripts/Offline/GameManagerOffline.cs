using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameManagerOffline : MonoBehaviour
{
    [SerializeField]
    private Round currentRound = new Round {
        round = 1,
        time = 1,
    };

    [SerializeField] private TileWallOffline tileWall;

    [SerializeField] private State state = State.WaitingToStart;
    [SerializeField] public int turn = 0;
    [SerializeField] private int playerId = 0;
    [SerializeField] private int botRId = 0;
    [SerializeField] private int botUId = 0;
    [SerializeField] private int botLId = 0;
    [SerializeField] private bool hasDrawn = false;

    [SerializeField] private GameObject playerTilePrefab;
    [SerializeField] private GameObject botTilePrefab;

    [SerializeField] private RoundDisplay roundDisplay;

    public struct Round
    {
        public int round;
        public int time;
    }

    public enum State
    {
        WaitingToStart,
        RoundStart,
        GamePlaying,
        GamePlayingDiscard,
        Paused,
        GamePlayingCall,
        RoundEnd,
        RoundEvaluation,
    }

    [SerializeField] private GameObject playerHand;
    [SerializeField] private GameObject enemyHandL;
    [SerializeField] private GameObject enemyHandR;
    [SerializeField] private GameObject enemyHandU;
    [SerializeField] private GameObject discardZoneU;
    [SerializeField] private GameObject discardZoneL;
    [SerializeField] private GameObject discardZoneR;
    [SerializeField] private GameObject discardZoneB;
    [SerializeField] private GameObject seatU;
    [SerializeField] private GameObject seatL;
    [SerializeField] private GameObject seatR;
    [SerializeField] private GameObject seatB;
    [SerializeField] private GameObject tokenU;
    [SerializeField] private GameObject tokenL;
    [SerializeField] private GameObject tokenR;
    [SerializeField] private GameObject tokenB;

    [SerializeField] private BotManager botU;
    [SerializeField] private BotManager botL;
    [SerializeField] private BotManager botR;

    [SerializeField] private PlayerManagerOffline player;

    private void Awake()
    {
        Debug.Log("GameManagerOffline Awake()");
        tileWall = GameObject.Find("Wall").GetComponent<TileWallOffline>();
        playerHand = GameObject.Find("PlayerHand");
        enemyHandL = GameObject.Find("EnemyHandL");
        enemyHandR = GameObject.Find("EnemyHandR");
        enemyHandU = GameObject.Find("EnemyHandU");
        discardZoneU = GameObject.Find("DiscardZoneU");
        discardZoneL = GameObject.Find("DiscardZoneL");
        discardZoneR = GameObject.Find("DiscardZoneR");
        discardZoneB = GameObject.Find("DiscardZoneB");
        seatU = GameObject.Find("SeatU");
        seatL = GameObject.Find("SeatL");
        seatR = GameObject.Find("SeatR");
        seatB = GameObject.Find("SeatB");
        tokenU = GameObject.Find("TokenU");
        tokenB = GameObject.Find("TokenB");
        tokenL = GameObject.Find("TokenL");
        tokenR = GameObject.Find("TokenR");

        playerId = Random.Range(0, 4);
        player = GameObject.Find("Player").GetComponent<PlayerManagerOffline>();
        botR = GameObject.Find("BotR").GetComponent<BotManager>();
        botU = GameObject.Find("BotU").GetComponent<BotManager>();
        botL = GameObject.Find("BotL").GetComponent<BotManager>();
        botRId = (playerId + 1) % 4;
        botUId = (playerId + 2) % 4;
        botLId = (playerId + 3) % 4;
    }

    public void Start()
    {
        string[] directions = { "E", "S", "W", "N" };
        seatU.GetComponent<TextMeshProUGUI>().text = directions[botUId];
        seatL.GetComponent<TextMeshProUGUI>().text = directions[botLId];
        seatR.GetComponent<TextMeshProUGUI>().text = directions[botRId];
        seatB.GetComponent<TextMeshProUGUI>().text = directions[playerId];
    }

    public int GetTurn()
    {
        return turn;
    }

    public void ChangeTurn()
    {
        turn = (turn + 1) % 4;
        hasDrawn = false;
    }

    public void ChangeToEndGame()
    {
        state = State.RoundEnd;
    }

    public void EndPlayerTurn()
    {
        player.SetActive(false);
        ChangeTurn();
    }

    public void Update()
    {
        if (state == State.WaitingToStart)
        {
            if (player.IsReady())
            {
                roundDisplay.ShowRound(currentRound.round, () =>
                {
                    state = State.RoundStart;
                });
            }
        }

        switch (state)
        {
            case State.RoundStart:
                {
                    Debug.Log("Round Start");
                    StartRoundDrawTile();
                    SpawnTile();
                    state = State.GamePlaying;
                    break;
                }
            case State.GamePlaying:
                {
                    if (turn == playerId)
                    {
                        Debug.Log("Player's turn");
                        player.SetActive(true);
                        if (!hasDrawn)
                        {
                            player.handTiles.Add(tileWall.DrawTile());
                            Debug.Log("Player drawn card");
                            GameObject tile = Instantiate(playerTilePrefab, playerHand.transform);
                            tile.GetComponent<DisplayTile>().displayId = player.handTiles[player.handTiles.Count-1];
                            tile.transform.SetParent(playerHand.transform, false);

                            player.playerTiles.Add(tile);
                            hasDrawn = true;
                        }
                    }
                    else if (turn == botRId)
                    {
                        Debug.Log("Bot R's turn");
                        botR.SetActive(true);
                        if (!hasDrawn)
                        {
                            botR.handTiles.Add(tileWall.DrawTile());
                            Debug.Log("Bot R drawn card");
                            GameObject tile = Instantiate(botTilePrefab, enemyHandR.transform);
                            tile.GetComponent<DisplayTile>().displayId = botR.handTiles[botR.handTiles.Count - 1];
                            tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
                            tile.transform.SetParent(enemyHandR.transform, false);

                            botR.botTiles.Add(tile);
                            hasDrawn = true;
                        }
                    }
                    else if (turn == botUId)
                    {
                        Debug.Log("Bot U's turn");
                        botU.SetActive(true);
                        if (!hasDrawn)
                        {
                            botU.handTiles.Add(tileWall.DrawTile());
                            Debug.Log("Bot U drawn card");
                            GameObject tile = Instantiate(botTilePrefab, enemyHandU.transform);
                            tile.GetComponent<DisplayTile>().displayId = botU.handTiles[botU.handTiles.Count - 1];
                            tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
                            tile.transform.SetParent(enemyHandU.transform, false);

                            botU.botTiles.Add(tile);
                            hasDrawn = true;
                        }
                    }
                    else if (turn == botLId)
                    {
                        Debug.Log("Bot L's turn");
                        botL.SetActive(true);
                        if (!hasDrawn)
                        {
                            botL.handTiles.Add(tileWall.DrawTile());
                            Debug.Log("Bot L drawn card");
                            GameObject tile = Instantiate(botTilePrefab, enemyHandL.transform);
                            tile.GetComponent<DisplayTile>().displayId = botL.handTiles[botL.handTiles.Count - 1];
                            tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 270);
                            tile.transform.SetParent(enemyHandL.transform, false);

                            botL.botTiles.Add(tile);
                            hasDrawn = true;
                        }
                    }
                    break;
                }
        }
    }

    void StartRoundDrawTile()
    {
        for (int k = 0; k < 4; k++)
        {
            if (k == 3)
            {
                for (int j = 0; j < 1; j++)
                {
                    player.handTiles.Add(tileWall.DrawTile());
                    botR.handTiles.Add(tileWall.DrawTile());
                    botU.handTiles.Add(tileWall.DrawTile());
                    botL.handTiles.Add(tileWall.DrawTile());
                }
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    player.handTiles.Add(tileWall.DrawTile());
                    botR.handTiles.Add(tileWall.DrawTile());
                    botU.handTiles.Add(tileWall.DrawTile());
                    botL.handTiles.Add(tileWall.DrawTile());
                }
            }
        }

        player.handTiles.Sort();
        botL.handTiles.Sort();
        botU.handTiles.Sort();
        botR.handTiles.Sort();
    }

    private void SpawnTile()
    {
        for(int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(playerTilePrefab, playerHand.transform);
            tile.GetComponent<DisplayTile>().displayId = player.handTiles[i];
            tile.transform.SetParent(playerHand.transform, false);

            player.playerTiles.Add(tile);
        }

        for (int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(botTilePrefab, enemyHandR.transform);
            tile.GetComponent<DisplayTile>().displayId = botR.handTiles[i];
            tile.GetComponent<DisplayTile>().isFacedDown = true;
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
            tile.transform.SetParent(enemyHandR.transform, false);

            botR.botTiles.Add(tile);
        }

        for (int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(botTilePrefab, enemyHandU.transform);
            tile.GetComponent<DisplayTile>().displayId = botU.handTiles[i];
            tile.GetComponent<DisplayTile>().isFacedDown = true;
            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
            tile.transform.SetParent(enemyHandU.transform, false);

            botU.botTiles.Add(tile);
        }

        for (int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(botTilePrefab, enemyHandL.transform);
            tile.GetComponent<DisplayTile>().displayId = botL.handTiles[i];
            tile.GetComponent<DisplayTile>().isFacedDown = true;
            tile.transform.rotation = Quaternion.Euler(0, 0, 270);
            tile.transform.SetParent(enemyHandL.transform, false);

            botL.botTiles.Add(tile);
        }
    }

    public void BotDiscard(GameObject tile)
    {
        if (turn == botRId)
        {
            tile.GetComponent<DisplayTile>().isFacedDown = false;
            tile.transform.SetParent(discardZoneR.transform, false);
        }
        else if (turn == botUId)
        {
            tile.GetComponent<DisplayTile>().isFacedDown = false;
            tile.transform.SetParent(discardZoneU.transform, false);
        }
        else if (turn == botLId)
        {
            tile.GetComponent<DisplayTile>().isFacedDown = false;
            tile.transform.SetParent(discardZoneL.transform, false);
        }
    }
}
