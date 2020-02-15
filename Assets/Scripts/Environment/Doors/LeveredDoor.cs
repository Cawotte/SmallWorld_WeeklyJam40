using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeveredDoor : Door {

    public void SwitchOpenClose()
    {
        IsOpen = !IsOpen; //Property IsOpen will edit sprite.
    }

}
