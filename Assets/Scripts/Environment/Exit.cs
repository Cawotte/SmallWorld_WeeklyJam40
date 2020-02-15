using Cawotte.Toolbox.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager = null;

    [SerializeField]
    private Sound endLevel = null;

    [SerializeField]
    private float timeBeforeStartNextLevel = 0.5f;

    private AudioSourcePlayer audioPlayer = null;
    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
    }

    //Disable those in builds
//#if UNITY_EDITOR
    private void Update()
    {

        //Cheat code previous/next levels
        if (Input.GetKeyDown(KeyCode.P))
        {
            NextLevel();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (SceneManager.GetActiveScene().buildIndex > 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1, LoadSceneMode.Single);
        }
    }
//#endif

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player)
        {

            Debug.Log("Sortie touché!");

            audioPlayer.PlaySound(endLevel);

            float totalWaitTime = endLevel.clip.length + timeBeforeStartNextLevel;

            player.AddCooldown(totalWaitTime);
            Invoke("NextLevel", totalWaitTime);
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
}
