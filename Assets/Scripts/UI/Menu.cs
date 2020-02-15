using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    [SerializeField]
    private GameObject quitPanel;

#if UNITY_WEBGL
    private void Start()
    {
        //No quit button in the OpenGL build. The game can't be closed in a web page.
        quitPanel.SetActive(false);
    }
#endif

    public void PlayGame()
    {
		SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quitted !");
        Application.Quit();
    }

}
