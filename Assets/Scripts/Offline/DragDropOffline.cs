using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropOffline : MonoBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone;

    private Vector2 startPosition;

    private GameObject startParent;
    private GameObject dropZone;
    private PlayerManagerOffline playerManager;

    void Awake()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManagerOffline>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "DiscardZoneB")
        {
            dropZone = collision.gameObject;
            isOverDropZone = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "DiscardZoneB")
        {
            isOverDropZone = false;
            dropZone = null;
        }
    }

    public void StartDrag()
    {
        if (transform.parent.gameObject.tag == "DropZone")
        {
            return;
        }

        isDragging = true;
        startPosition = transform.position;
        startParent = transform.parent.gameObject;
    }

    public void EndDrag()
    {
        if (transform.parent.gameObject.tag == "DropZone")
        {
            return;
        }

        isDragging = false;

        if (isOverDropZone)
        {
            if (!playerManager.IsActive())
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, false);
                return;
            }

            transform.SetParent(dropZone.transform, false);
            playerManager.handTiles.Remove(gameObject.GetComponent<DisplayTile>().displayId);
            playerManager.playerTiles.Remove(gameObject);
            playerManager.EndTurn();

            //Check for prompt
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }
}
