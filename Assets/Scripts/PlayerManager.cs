using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> token = new NetworkVariable<int>(25000,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public List<int> handTiles;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private NetworkVariable<bool> isAssigned = new NetworkVariable<bool>(false,
               NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private NetworkVariable<bool> isActive = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private GameObject offlineTilePrefab;
    [SerializeField] private GameObject onlineTilePrefab;
    [SerializeField] public NetworkVariable<int> id = new NetworkVariable<int>(0,
               NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private GameObject canvas;
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

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player spawned " + OwnerClientId);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        canvas = Instantiate(canvas, canvas.transform.position, canvas.transform.rotation);
        playerHand = Instantiate(playerHand, playerHand.transform.position, playerHand.transform.rotation);
        playerHand.transform.SetParent(canvas.transform, false);
        enemyHandL = Instantiate(enemyHandL, enemyHandL.transform.position, enemyHandL.transform.rotation);
        enemyHandL.transform.SetParent(canvas.transform, false);
        enemyHandR = Instantiate(enemyHandR, enemyHandR.transform.position, enemyHandR.transform.rotation);
        enemyHandR.transform.SetParent(canvas.transform, false);
        enemyHandU = Instantiate(enemyHandU, enemyHandU.transform.position, enemyHandU.transform.rotation);
        enemyHandU.transform.SetParent(canvas.transform, false);
        discardZoneU = Instantiate(discardZoneU, discardZoneU.transform.position, discardZoneU.transform.rotation);
        discardZoneU.transform.SetParent(canvas.transform, false);
        discardZoneL = Instantiate(discardZoneL, discardZoneL.transform.position, discardZoneL.transform.rotation);
        discardZoneL.transform.SetParent(canvas.transform, false);
        discardZoneR = Instantiate(discardZoneR, discardZoneR.transform.position, discardZoneR.transform.rotation);
        discardZoneR.transform.SetParent(canvas.transform, false);
        discardZoneB = Instantiate(discardZoneB, discardZoneB.transform.position, discardZoneB.transform.rotation);
        discardZoneB.transform.SetParent(canvas.transform, false);
        seatU = Instantiate(seatU, seatU.transform.position, seatU.transform.rotation);
        seatU.transform.SetParent(canvas.transform, false);
        seatL = Instantiate(seatL, seatL.transform.position, seatL.transform.rotation);
        seatL.transform.SetParent(canvas.transform, false);
        seatR = Instantiate(seatR, seatR.transform.position, seatR.transform.rotation);
        seatR.transform.SetParent(canvas.transform, false);
        seatB = Instantiate(seatB, seatB.transform.position, seatB.transform.rotation);
        seatB.transform.SetParent(canvas.transform, false);
        tokenU = Instantiate(tokenU, tokenU.transform.position, tokenU.transform.rotation);
        tokenU.transform.SetParent(canvas.transform, false);
        tokenL = Instantiate(tokenL, tokenL.transform.position, tokenL.transform.rotation);
        tokenL.transform.SetParent(canvas.transform, false);
        tokenR = Instantiate(tokenR, tokenR.transform.position, tokenR.transform.rotation);
        tokenR.transform.SetParent(canvas.transform, false);
        tokenB = Instantiate(tokenB, tokenB.transform.position, tokenB.transform.rotation);
        tokenB.transform.SetParent(canvas.transform, false);

        if (IsOwner)
        {
            Debug.Log("Player " + OwnerClientId + " is owner");

            canvas.SetActive(true);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!isAssigned.Value)
        {
            gameManager.AssignPlayerServerRpc(OwnerClientId);
            isAssigned.Value = true;
        }

        if (isActive.Value)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                token.Value += 1000;
            }
        }
    }

    [ClientRpc]
    public void ReceiveTileClientRpc(int tile)
    {
        handTiles.Add(tile);
    }

    [ClientRpc]
    public void SetActiveStatusClientRpc(bool active)
    {
        if (!IsOwner) return;
        isActive.Value = active;
    }

    [ClientRpc]
    public void SetClientIdClientRpc(int clientId)
    {
        Debug.Log("Running on client " + OwnerClientId);
        if (!IsOwner) return;
        id.Value = clientId;
        string[] direction = { "E", "S", "W", "N" };
        TextMeshProUGUI text;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    text = seatB.GetComponent<TextMeshProUGUI>();
                    text.text = direction[id.Value];
                    break;
                case 1:
                    text = seatR.GetComponent<TextMeshProUGUI>();
                    text.text = direction[(id.Value + 1) % 4];
                    break;
                case 2:
                    text = seatU.GetComponent<TextMeshProUGUI>();
                    text.text = direction[(id.Value + 2) % 4];
                    break;
                case 3:
                    text = seatL.GetComponent<TextMeshProUGUI>();
                    text.text = direction[(id.Value + 3) % 4];
                    break;
            }
        }
    }

    [ClientRpc]
    public void SpawnTileLocallyClientRpc()
    {
        if (!IsOwner) return;
        Debug.Log("Spawning tiles locally");
        handTiles.Sort();

        for (int i = 0; i < handTiles.Count; i++)
        {
            GameObject tile = Instantiate(offlineTilePrefab, playerHand.transform);
            tile.GetComponent<DisplayTile>().displayId = handTiles[i];
            tile.transform.SetParent(playerHand.transform, false);
        }
    }

    [ClientRpc]
    public void SpawnTileRClientRpc(int tileId, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Spawning tile for enemy R " + tileId + " on id: " + OwnerClientId);
        GameObject tile = Instantiate(offlineTilePrefab, enemyHandR.transform);
        tile.transform.rotation = Quaternion.Euler(0, 0, 90);
        tile.GetComponent<DisplayTile>().displayId = tileId;
        tile.transform.SetParent(enemyHandR.transform, false);
    }

    [ClientRpc]
    public void SpawnTileUClientRpc(int tileId, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Spawning tile for enemy U " + tileId + " on id: " + OwnerClientId);
        GameObject tile = Instantiate(offlineTilePrefab, enemyHandU.transform);
        tile.transform.rotation = Quaternion.Euler(0, 0, 180);
        tile.GetComponent<DisplayTile>().displayId = tileId;
        tile.transform.SetParent(enemyHandU.transform, false);
    }

    [ClientRpc]
    public void SpawnTileLClientRpc(int tileId, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Spawning tile for enemy L " + tileId + " on id: " + OwnerClientId);
        GameObject tile = Instantiate(offlineTilePrefab, enemyHandL.transform);
        tile.transform.rotation = Quaternion.Euler(0, 0, 270);
        tile.GetComponent<DisplayTile>().displayId = tileId;
        tile.transform.SetParent(enemyHandL.transform, false);
    }
}
