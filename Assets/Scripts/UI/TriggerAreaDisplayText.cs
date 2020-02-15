using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerAreaDisplayText : MonoBehaviour {

    public RectTransform textPanel;
    public TextMeshProUGUI text;

    private void Start()
    {
        textPanel.gameObject.SetActive(false);
        text.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        textPanel.gameObject.SetActive(true);
        text.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        textPanel.gameObject.SetActive(false);
        text.enabled = false;
    }
}
