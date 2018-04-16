using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageWay : MonoBehaviour {

    public PassageWay exitPassage;

    public Vector2 exitPos()
    {
        return exitPassage.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(name + "collider disabled!");
        GetComponent<BoxCollider2D>().enabled = false;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(name + "collider enabled!");
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
