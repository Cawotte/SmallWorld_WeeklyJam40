using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class toVanille : MonoBehaviour {

    public RectTransform vanillePanel;
    public TextMeshProUGUI vanilleText;

    private void Start()
    {
        vanillePanel.gameObject.SetActive(false);
        vanilleText.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        vanillePanel.gameObject.SetActive(true);
        vanilleText.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        vanillePanel.gameObject.SetActive(false);
        vanilleText.enabled = false;
    }
}
