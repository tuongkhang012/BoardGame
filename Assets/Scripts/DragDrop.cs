using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private bool isOverDropZone;

    private Vector2 startPosition;

    private GameObject startParent;
    private GameObject dropZone;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            if (gameManager.turn.Value != 0)
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, false);
                return;
            }

            transform.SetParent(dropZone.transform, false);
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
