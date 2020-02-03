using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Door : Interactable {

    [SerializeField]
    private Sprite openedSprite;
    [SerializeField]
    private Sprite closedSprite;


    [SerializeField]
    protected bool isOpen = false;

    public bool IsOpen {
        get => isOpen;
        protected set
        {

            isOpen = value;
            UpdateSprite();
        }
    }

    public void OnValidate()
    {
        UpdateSprite();
    }

    public void Start()
    {
        UpdateSprite();
    }

    public override bool CanMoveTo(Player player)
    {
        return isOpen;
    }


    private void UpdateSprite()
    {

        if (isOpen)
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        else
            GetComponent<SpriteRenderer>().sprite = closedSprite;
    }


}
