using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<Round> currentRound = new NetworkVariable<Round>(new Round
    {
        round = 0,
        time = 0
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> firstPlayerId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] public Dictionary<int, ulong> players = new Dictionary<int, ulong>();
    [SerializeField] private TileWall tileWall;

    [SerializeField] private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    [SerializeField] public NetworkVariable<int> turn = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] private GameObject onlineTilePrefab;

    public struct Round : INetworkSerializable
    {
        public int round;
        public int time;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref round);
            serializer.SerializeValue(ref time);
        }
    }

    public enum State
    {
        WaitingToStart,
        RoundStart,
        GamePlaying,
        RoundEnd,
        RoundEvaluation,
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("GameManager spawned");
        if (IsHost)
        {
            firstPlayerId.Value = Random.Range(0, 4);
        }

        tileWall = GameObject.Find("Wall").GetComponent<TileWall>();

    }

    [ServerRpc(RequireOwnership = false)]
    public void AssignPlayerServerRpc(ulong clientId)
    {
        Debug.Log("Assigning player to game manager");
        players.Add(firstPlayerId.Value, clientId);
        NetworkClient client = NetworkManager.Singleton.ConnectedClients[clientId];
        PlayerManager player = client.PlayerObject.GetComponent<PlayerManager>();

        Debug.Log("First player value is: " + firstPlayerId.Value);
        player.SetClientIdClientRpc(firstPlayerId.Value);
        firstPlayerId.Value = (firstPlayerId.Value + 1)%4;
    }

    public int GetTurn()
    {
        return turn.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeTurnServerRpc(ulong clientId)
    {
        turn.Value = (turn.Value + 1) % 4;

        NetworkClient client = NetworkManager.Singleton.ConnectedClients[clientId];
        PlayerManager player = client.PlayerObject.GetComponent<PlayerManager>();

        player.GetComponent<PlayerManager>().SetActiveStatusClientRpc(false);
    }

    public void Update()
    {
        if (!IsServer) return;

        if (state.Value == State.WaitingToStart)
        {
            if (players.Count == 4)
            {
                Debug.Log("Starting Round");
                StartRoundDrawTile();
                state.Value = State.RoundStart;
            }
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.RoundStart:
                {
                    Debug.Log("Round Start");
                    for (int i = 0; i < 4; i++)
                    {
                        NetworkClient client = NetworkManager.Singleton.ConnectedClients[players[i]];
                        PlayerManager player = client.PlayerObject.GetComponent<PlayerManager>();
                        player.GetComponent<PlayerManager>().SpawnTileLocallyClientRpc();

                        for (int j = 0; j < 13; j++)
                        {
                            player.GetComponent<PlayerManager>().SpawnTileRClientRpc(
                                NetworkManager.Singleton.ConnectedClients[players[(i + 1) % 4]].PlayerObject.GetComponent<PlayerManager>().handTiles[j],
                                new ClientRpcParams
                            {
                                Send = new ClientRpcSendParams
                                {
                                    TargetClientIds = new List<ulong> { players[(i + 1)%4] }
                                }
                            });

                            player.GetComponent<PlayerManager>().SpawnTileUClientRpc(
                                NetworkManager.Singleton.ConnectedClients[players[(i + 2) % 4]].PlayerObject.GetComponent<PlayerManager>().handTiles[j],
                                new ClientRpcParams
                            {
                                Send = new ClientRpcSendParams
                                {
                                    TargetClientIds = new List<ulong> { players[(i + 2) % 4] }
                                }
                            });

                            player.GetComponent<PlayerManager>().SpawnTileLClientRpc(
                                NetworkManager.Singleton.ConnectedClients[players[(i + 3) % 4]].PlayerObject.GetComponent<PlayerManager>().handTiles[j],
                                new ClientRpcParams
                            {
                                Send = new ClientRpcSendParams
                                {
                                    TargetClientIds = new List<ulong> { players[(i + 3) % 4] }
                                }
                            });
                        }
                    }
                    state.Value = State.GamePlaying;
                    break;
                }
            case State.GamePlaying:
                {
                    Debug.Log("This turn of player: " + players[turn.Value]);
                    NetworkClient client = NetworkManager.Singleton.ConnectedClients[players[turn.Value]];
                    PlayerManager player = client.PlayerObject.GetComponent<PlayerManager>();
                    player.GetComponent<PlayerManager>().SetActiveStatusClientRpc(true);
                    break;
                }
                
        }
    }

    void StartRoundDrawTile()
    {
        for (int k = 0; k < 4; k++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (k == 3) 
                { 
                    for (int j = 0; j < 1; j++)
                    {
                        tileWall.DrawTileServerRpc(players[i]);
                    }
                } else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        tileWall.DrawTileServerRpc(players[i]);
                    }
                }
            }
        }
    }
}
