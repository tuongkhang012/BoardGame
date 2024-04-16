using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> token = new NetworkVariable<int>(25000, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public List<int> handTiles;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject playerHand;

    [SerializeField] private GameObject tileWallPrefab;
    [SerializeField] private GameObject tileWall;
    [SerializeField] private GameObject canvas;

    public override void OnNetworkSpawn()
    {
        token.OnValueChanged += (int previousValue, int newValue) =>{
            Debug.Log($"Token: {newValue}");
        };

        playerHand = GameObject.Find("PlayerHand");
        tileWall = GameObject.Find("Wall");

        if (IsOwner)
        {
            for (int i = 0; i < 13; i++)
            {
                DrawTileServerRpc();
            }
            handTiles.Sort();
            SpawnTileClientRpc();
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            token.Value += 1000;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DrawTileServerRpc()
    {
        int newTile = tileWall.GetComponent<TileWall>().wall[0];
        handTiles.Add(newTile);
        tileWall.GetComponent<TileWall>().wall.RemoveAt(0);
    }

    [ClientRpc]
    public void SpawnTileClientRpc()
    {
        for (int i = 0; i < handTiles.Count; i++)
        {
            GameObject tile = Instantiate(tilePrefab, new Vector2(0, 0), Quaternion.identity);
            tile.GetComponent<DisplayTile>().displayId = handTiles[i];
            NetworkObject networkTile = tile.GetComponent<NetworkObject>();
            networkTile.Spawn();
            networkTile.transform.SetParent(playerHand.transform);
        }
    }
}
