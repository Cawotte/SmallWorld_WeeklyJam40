using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {

            Debug.Log("Sortie touché!");
            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("victory").source.Play();

            player.onExit = true; //Prevent the player from moving.
            Invoke("NextLevel", 1f);
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
}
