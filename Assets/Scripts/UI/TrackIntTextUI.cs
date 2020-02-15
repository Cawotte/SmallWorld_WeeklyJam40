using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrackIntTextUI : GameEventListener
{

    [SerializeField]
    private string preNumberText = "";

    [SerializeField]
    private bool hideIfZero = true;

    [SerializeField]
    private Image parentPanel = null;

    private bool gameIsRunning = false;

    private void Start()
    {
        UpdateTextValue();
        gameIsRunning = true;
    }

    private void OnValidate()
    {
        UpdateTextValue();
    }

    public void UpdateTextValue()
    {
        TextMeshProUGUI textMeshPro = GetComponent<TextMeshProUGUI>();
        int value = ((IntVariable)Event).Value;

        if (hideIfZero && value == 0)
        {
            textMeshPro.text = "";
            parentPanel.enabled = false;
        }
        else
        {
            textMeshPro.text = preNumberText + value.ToString();
            parentPanel.enabled = true;

            //When the game runs, we only hide the text at the beginning.
            if (gameIsRunning)
                hideIfZero = false;
        }
    }

    //When the attached IntVariable is modified, call this function
    public override void OnEventRaised()
    {
        base.OnEventRaised();

        UpdateTextValue();
    }

}
