using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public Sprite openedSprite;
    private bool isClosed = true;

    public void open()
    {
        GetComponent<SpriteRenderer>().sprite = openedSprite;
        isClosed = false;
    }

    public bool IsClosed()
    {
        return isClosed;
    }

}
