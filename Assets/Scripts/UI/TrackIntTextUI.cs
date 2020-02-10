using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TrackIntTextUI : GameEventListener
{
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

    }

    private void Start()
    {
        UpdateTextValue();
    }

    public void UpdateTextValue()
    {
        textMeshPro.text = ((IntVariable)Event).Value.ToString();
    }

    //When the attached IntVariable is modified, call this function
    public override void OnEventRaised()
    {
        base.OnEventRaised();

        UpdateTextValue();
    }

}
