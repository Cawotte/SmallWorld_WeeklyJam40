using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeveredDoor : MonoBehaviour {

    public bool isOpen;
    public Sprite openedDoor;
    public Sprite closedDoor;
	
    // Use this for initialization
	void Start () {
		
        //The door can start either opened or closed depending on the bool value we set in the inspector.
        if (isOpen)
            GetComponent<SpriteRenderer>().sprite = openedDoor;
        else
            GetComponent<SpriteRenderer>().sprite = closedDoor;

    }
	
	//Closed the door if it's open, open it if it's closed.
    public void openClose()
    {
        if ( isOpen )
        {
            GetComponent<SpriteRenderer>().sprite = closedDoor;
            isOpen = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = openedDoor;
            isOpen = true;
        }
    }

}
