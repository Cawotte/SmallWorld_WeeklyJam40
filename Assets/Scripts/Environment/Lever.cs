using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable {

    [SerializeField]
    private bool isSwitchedOn = false;

    [SerializeField]
    private List<LeveredDoor> doorList;

    [SerializeField]
    private Sprite switchedOnSprite;

    [SerializeField]
    private Sprite switchedOffSprite;


    private void OnValidate()
    {
        UpdateSprite();
    }
    // Use this for initialization
    void Start () {

        UpdateSprite();

    }

    public override bool CanMoveTo(Player player)
    {
        SwitchLever(); //Switch the lever, then return false because a lever is an obstacle.
        return false;
    }

    public void SwitchLever()
    {
        isSwitchedOn = !isSwitchedOn;

        UpdateSprite();

        //Switch levers
        foreach (LeveredDoor door in doorList)
        {
            door.SwitchOpenClose();
        }


        //Click sound !
        if (AudioManager.getInstance() != null)
        {
            AudioManager.getInstance().Find("leverClick").source.Play();
            AudioManager.getInstance().Find("blocked").source.mute = true;
        }

    }

    private void UpdateSprite()
    {

        if (isSwitchedOn)
            GetComponent<SpriteRenderer>().sprite = switchedOnSprite;
        else
            GetComponent<SpriteRenderer>().sprite = switchedOffSprite;
    }

}
