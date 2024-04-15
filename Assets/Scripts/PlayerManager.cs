using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Collections.LowLevel.Unsafe;

public class PlayerManager : NetworkBehaviour
{
    public GameObject tileWall;

    public GameObject playerHand;
    public GameObject enemyHandL;
    public GameObject enemyHandR;
    public GameObject enemyHandU;
    public GameObject discardZoneB;
    public GameObject discardZoneL;
    public GameObject discardZoneR;
    public GameObject discardZoneU;

    public NetworkIdentity networkIdentity;

    public GameObject tilePrefab;

    [SyncVar]
    private string playerDirection;

    [SyncVar]
    public int playerToken = 25000;

    private static int nextPlayerIndex = -1;
    private static string[] players = new string[] { "E", "S", "W", "N" };

    public List<Tile> handTiles = new List<Tile>();

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (nextPlayerIndex == -1)
        {
            nextPlayerIndex = Random.Range(0, 4);
        }

        playerDirection = players[nextPlayerIndex];
        nextPlayerIndex = (nextPlayerIndex + 1) % players.Length;

        networkIdentity.AssignClientAuthority(connectionToClient);

        StartCoroutine(DrawInitialTiles());
    }

    private IEnumerator DrawInitialTiles()
    {
        // Wait until the client is ready
        while (!NetworkClient.ready)
        {
            yield return null;
        }

        // Draw 13 tiles for the player
        for (int i = 0; i < 13; i++)
        {
            CmdDrawTile(tileWall.GetComponent<TileWall>());
        }
    }

    public string GetPlayerDirection()
    {
        return playerDirection;
    }

    [Command]
    public void CmdDrawTile(TileWall tileWall)
    {
        if (tileWall.wall.Count > 0)
        {
            Tile tile = tileWall.wall[0];
            tileWall.wall.RemoveAt(0);
            // Send the tile to the client
            TargetReceiveTile(tile);
        }
    }

    [TargetRpc]
    private void TargetReceiveTile(Tile tile)
    {
        handTiles.Add(tile);
        RenderTile(tile);
    }

    private void RenderTile(Tile tile)
    {
        GameObject temp = Instantiate(tilePrefab, new Vector2(0, 0), Quaternion.identity);
        NetworkServer.Spawn(temp, connectionToClient);
        temp.transform.SetParent(playerHand.transform);
        temp.GetComponent<DisplayTile>().displayId = tile.id;
    }
}
