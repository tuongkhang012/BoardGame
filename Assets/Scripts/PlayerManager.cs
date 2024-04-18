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

    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool isAssigned = false;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player spawned " + OwnerClientId);
        token.OnValueChanged += (int previousValue, int newValue) =>{
            Debug.Log($"Token: {newValue}");
        };

        playerHand = GameObject.Find("PlayerHand");
        tileWall = GameObject.Find("Wall");

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!isAssigned)
        {
            gameManager.AssignPlayerServerRpc(OwnerClientId);
            isAssigned = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            token.Value += 1000;
        }
    }

    [ClientRpc]
    public void ReceiveTileClientRpc(int tile)
    {
        if (!IsOwner) return;

        handTiles.Add(tile);
    }
}
