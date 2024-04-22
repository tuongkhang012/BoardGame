using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Linq;

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
    [SerializeField] private EndDisplay endDisplay;

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
        Paused,
        PlayerWin,
        BotLWin,
        BotRWin,
        BotUWin,
        Draw,
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
    [SerializeField] private GameObject dora;

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
        dora = GameObject.Find("Dora");

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
        state = State.Draw;
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
                    string[] directions = { "E", "S", "W", "N" };
                    seatU.GetComponent<TextMeshProUGUI>().text = directions[botUId];
                    seatL.GetComponent<TextMeshProUGUI>().text = directions[botLId];
                    seatR.GetComponent<TextMeshProUGUI>().text = directions[botRId];
                    seatB.GetComponent<TextMeshProUGUI>().text = directions[playerId];
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
                            if (CheckWinningHand(player.handTiles))
                            {
                                state = State.PlayerWin;
                            }
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
                            //tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
                            tile.transform.SetParent(enemyHandR.transform, false);

                            botR.botTiles.Add(tile);
                            if (CheckWinningHand(botR.handTiles))
                            {
                                state = State.PlayerWin;
                            }
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
                            //tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
                            tile.transform.SetParent(enemyHandU.transform, false);

                            botU.botTiles.Add(tile);
                            if (CheckWinningHand(botU.handTiles))
                            {
                                state = State.PlayerWin;
                            }
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
                            //tile.GetComponent<DisplayTile>().isFacedDown = true;
                            tile.transform.rotation = Quaternion.Euler(0, 0, 270);
                            tile.transform.SetParent(enemyHandL.transform, false);

                            botL.botTiles.Add(tile);
                            if (CheckWinningHand(botL.handTiles))
                            {
                                state = State.PlayerWin;
                            }
                            hasDrawn = true;
                        }
                    }
                    break;
                }
            case State.PlayerWin:
                {
                    Debug.Log("Player Win");
                    break;
                }
            case State.BotRWin:
                {
                    Debug.Log("Bot R Win");
                    break;
                }
            case State.BotUWin:
                {
                    Debug.Log("Bot U Win");
                    break;
                }
            case State.BotLWin:
                {
                    Debug.Log("Bot L Win");
                    break;
                }
            case State.Draw:
                {
                    Debug.Log("Draw");
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
            //tile.GetComponent<DisplayTile>().isFacedDown = true;
            tile.transform.rotation = Quaternion.Euler(0, 0, 90);
            tile.transform.SetParent(enemyHandR.transform, false);

            botR.botTiles.Add(tile);
        }

        for (int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(botTilePrefab, enemyHandU.transform);
            tile.GetComponent<DisplayTile>().displayId = botU.handTiles[i];
            //tile.GetComponent<DisplayTile>().isFacedDown = true;
            tile.transform.rotation = Quaternion.Euler(0, 0, 180);
            tile.transform.SetParent(enemyHandU.transform, false);

            botU.botTiles.Add(tile);
        }

        for (int i = 0; i < 13; i++)
        {
            GameObject tile = Instantiate(botTilePrefab, enemyHandL.transform);
            tile.GetComponent<DisplayTile>().displayId = botL.handTiles[i];
            //tile.GetComponent<DisplayTile>().isFacedDown = true;
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
            discardZoneR.GetComponent<DiscardZoneManager>().discardedTiles.Add(tile.GetComponent<DisplayTile>().displayId);
            botR.SortTiles();
        }
        else if (turn == botUId)
        {
            tile.GetComponent<DisplayTile>().isFacedDown = false;
            tile.transform.SetParent(discardZoneU.transform, false);
            discardZoneU.GetComponent<DiscardZoneManager>().discardedTiles.Add(tile.GetComponent<DisplayTile>().displayId);
            botU.SortTiles();
        }
        else if (turn == botLId)
        {
            tile.GetComponent<DisplayTile>().isFacedDown = false;
            tile.transform.SetParent(discardZoneL.transform, false);
            discardZoneL.GetComponent<DiscardZoneManager>().discardedTiles.Add(tile.GetComponent<DisplayTile>().displayId);
            botL.SortTiles();
        }
    }

    public void AddDiscard(GameObject tile)
    {
        discardZoneB.GetComponent<DiscardZoneManager>().discardedTiles.Add(tile.GetComponent<DisplayTile>().displayId);
    }

    public bool CheckWinningHand(List<int> handTiles)
    {
        List<int> tileCount = new List<int>(Enumerable.Repeat(0, 39));
        foreach (int tile in handTiles)
        {
            tileCount[tile] = 1;
        }
        return CheckWinningRecur(tileCount, false);
    }

    private bool CheckWinningRecur(List<int> c, bool got_pair)
    {
        int sum = 0;
        for (int i = 0; i < 39; ++i)
        {
            sum += c[i];
        }
        if (sum == 0)
        {
            return true;
        }

        for (int i = 0; i < 37; ++i)
        {
            if (c[i] >= 3)
            {
                c[i] -= 3;
                if (CheckWinningRecur(c, got_pair))
                {
                    return true;
                }
                c[i] += 3;
            }

            if (c[i] == 2 && !got_pair)
            {
                c[i] -= 2;
                if (CheckWinningRecur(c, true))
                {
                    return true;
                }
                c[i] += 2;
            }

            if (i < 27)
            {
                if (c[i] > 0)
                {
                    if (c[i+1] > 0 && c[i+2] > 0)
                    {
                        c[i]--;
                        c[i+1]--;
                        c[i+2]--;
                        if (CheckWinningRecur(c, got_pair))
                        {
                            return true;
                        }
                        c[i]++;
                        c[i+1]++;
                        c[i+2]++;
                    }
                }
            }
        }

        return false;
    }

    public GameState getCurrentGameState()
    {
        return new GameState(new List<int>(tileWall.wall),
        new List<int>(player.handTiles), new List<int>(botL.handTiles), new List<int>(botR.handTiles), new List<int>(botU.handTiles),
                                  new List<int>(), new List<int>(), new List<int>(), new List<int>(),
                                             turn, playerId);
    }
}

public class GameState
{
    List<int> wall;
    List<int> playerHand;
    List<int> botLHand;
    List<int> botRHand;
    List<int> botUHand;
    List<int> disZoneB;
    List<int> disZoneL;
    List<int> disZoneR;
    List<int> disZoneU;
    int turn; 
    List<int> last_played;
    int id;

    public GameState(List<int> _wall,
        List<int> _playerHand, List<int> _botLHand, List<int> _botRHand, List<int> _botUHand,
        List<int> _disZoneB, List<int> _disZoneL, List<int> _disZoneR, List<int> _disZoneU,
        int _turn, int _id)
    {
        wall = _wall;
        playerHand = _playerHand;
        botLHand = _botLHand;
        botRHand = _botRHand;
        botUHand = _botUHand;
        disZoneB = _disZoneB;
        disZoneL = _disZoneL;
        disZoneR = _disZoneR;
        disZoneU = _disZoneU;
        turn = _turn; //player, botR, botU, botL
        last_played = new List<int>();
        id = _id; // 0 for player, 1 for R, 2 for U, 3 for L
    }

    public void move(int tileIndex)
    {
        if (id == 0)
        {
            disZoneU.Add(playerHand[tileIndex]);
            playerHand.RemoveAt(tileIndex);
        }
        else if (id == 1)
        {
            disZoneR.Add(botRHand[tileIndex]);
            botRHand.RemoveAt(tileIndex);
        }
        else if (id == 2)
        {
            disZoneB.Add(botUHand[tileIndex]);
            botUHand.RemoveAt(tileIndex);
        }
        else if (id == 3)
        {
            disZoneL.Add(botLHand[tileIndex]);
            botLHand.RemoveAt(tileIndex);
        }

        turn = (turn + 1) % 4;
        last_played.Add(tileIndex);
    }

    public GameState copy()
    {
        return new GameState(new List<int>(wall),
            new List<int>(playerHand), new List<int>(botLHand), new List<int>(botRHand), new List<int>(botUHand),
            new List<int>(disZoneB), new List<int>(disZoneL), new List<int>(disZoneR), new List<int>(disZoneU),
            turn, id);
    }

    public bool CheckWinningHand(List<int> handTiles)
    {
        List<int> tileCount = new List<int>(Enumerable.Repeat(0, 39));
        foreach (int tile in handTiles)
        {
            tileCount[tile] = 1;
        }
        return CheckWinningRecur(tileCount, false);
    }

    private bool CheckWinningRecur(List<int> c, bool got_pair)
    {
        int sum = 0;
        for (int i = 0; i < 39; ++i)
        {
            sum += c[i];
        }
        if (sum == 0)
        {
            return true;
        }

        for (int i = 0; i < 37; ++i)
        {
            if (c[i] >= 3)
            {
                c[i] -= 3;
                if (CheckWinningRecur(c, got_pair))
                {
                    return true;
                }
                c[i] += 3;
            }

            if (c[i] == 2 && !got_pair)
            {
                c[i] -= 2;
                if (CheckWinningRecur(c, true))
                {
                    return true;
                }
                c[i] += 2;
            }

            if (i < 27)
            {
                if (c[i] > 0)
                {
                    if (c[i + 1] > 0 && c[i + 2] > 0)
                    {
                        c[i]--;
                        c[i + 1]--;
                        c[i + 2]--;
                        if (CheckWinningRecur(c, got_pair))
                        {
                            return true;
                        }
                        c[i]++;
                        c[i + 1]++;
                        c[i + 2]++;
                    }
                }
            }
        }

        return false;
    }

    public bool isWinning()
    {
        if (id == 0)
        {
            return CheckWinningHand(playerHand);
        }
        else if (id == 1)
        {
            return CheckWinningHand(botRHand);
        }
        else if (id == 2)
        {
            return CheckWinningHand(botUHand);
        }
        else if (id == 3)
        {
            return CheckWinningHand(botLHand);
        }
        else return false;
    }

    public bool game_over()
    {
        return isWinning() || wall.Count == 0;
    }

    public string get_winner()
    {
        if (isWinning())
        {
            if (id == 0)
            {
                return "p";
            }
            else if (id == 1)
            {
                return "r";
            }
            else if (id == 2)
            {
                return "u";
            }
            else if (id == 3)
            {
                return "l";
            }
            else
            {
                return "error";
            }
        }
        else if (wall.Count == 0)
        {
            return "draw";
        }
        else return "notover";
    }
}