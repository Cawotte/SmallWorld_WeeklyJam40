using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    public Teleporter exitPassage;

    [HideInInspector] public bool isTeleporting = false;

    private float teleportDuration = 0.2f;
    public Vector2 exitPos()
    {
        return exitPassage.transform.position;
    }

    public void setTeleportersAvailability(bool boolean)
    {
        isTeleporting = boolean;
        exitPassage.isTeleporting = boolean;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {
            StartCoroutine(Teleport(player, teleportDuration));
        }
    }


    public IEnumerator Teleport(Player player, float aTime)
    {

        //If the teleporter is in use, abort
        if (isTeleporting) yield break;

        //We wait for any other movement coroutines to finish before starting this one.
        while (player.isMoving) yield return null;

        //we prevent the player from moving while teleporting
        player.isMoving= true;

        //Staircase sound!
        if (AudioManager.getInstance() != null)
            AudioManager.getInstance().Find("staircase").source.Play();

        Debug.Log("Teleporting from " + name);

        //We set both teleporters as "In Use"
        isTeleporting = true; exitPassage.isTeleporting = true;

        float alpha = player.GetComponent<Renderer>().material.color.a;

        //The character disappear
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
            player.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
        
        //Now me teleport the player
        player.transform.position = exitPos();

        //The character fades back to reality 
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(0f, alpha, t));
            player.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }

        //We allow the player to move again
        player.isMoving = false;
        //We set both teleporter as "Available"
        isTeleporting = false; exitPassage.isTeleporting = false;
    }

    private void OnDrawGizmos()
    {
        if (exitPassage != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, exitPassage.transform.position);
        }
    }
}
