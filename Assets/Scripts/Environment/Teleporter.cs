using Cawotte.Toolbox.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    [SerializeField]
    private Teleporter exitPassage;

    [HideInInspector] public bool isTeleporting = false;

    private float teleportDuration = 0.2f;


    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager = null;

    [SerializeField]
    private Sound staircaseSound = null;

    private AudioSourcePlayer audioPlayer = null;

    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
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
        audioPlayer.PlaySound(staircaseSound);

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
        player.transform.position = exitPassage.transform.position;

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
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, exitPassage.transform.position);
        }
    }
}
