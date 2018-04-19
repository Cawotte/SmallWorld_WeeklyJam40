using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    public bool isPulled;
    public List<LeveredDoor> doorList;

    public Sprite leverOn;
    public Sprite leverOff;

    // Use this for initialization
    void Start () {

        //The lever can start either pulled or not depending on the bool value we set in the inspector.
        renderLever();

    }

    private void renderLever()
    {
        if (isPulled)
            GetComponent<SpriteRenderer>().sprite = leverOn;
        else
            GetComponent<SpriteRenderer>().sprite = leverOff;
    }
	
    //Change the state of each door the lever operate ones.
	public void operate()
    {
        isPulled = !isPulled;
        renderLever();
        

        foreach ( LeveredDoor door in doorList )
        {
            door.openClose();
        }

    }
}
