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

    [SerializeField] private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);

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
        Debug.Log("Assigning player at " + firstPlayerId.Value + " for " + clientId);
        players.Add(firstPlayerId.Value, clientId);
        firstPlayerId.Value = (firstPlayerId.Value + 1)%4;
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
