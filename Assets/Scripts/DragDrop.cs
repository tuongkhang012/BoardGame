using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private GameObject startParent;
    private Vector2 startPosition;
    private GameObject dropZone;
    private bool isOverDropZone;

    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DropZone")
        {
            dropZone = collision.gameObject;
            isOverDropZone = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DropZone")
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
