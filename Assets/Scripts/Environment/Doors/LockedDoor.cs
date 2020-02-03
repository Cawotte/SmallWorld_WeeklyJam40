using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door
{

    public override bool CanMoveTo(Player player)
    {

        if (isOpen)
        {
            return true;
        }
        else
        {
            if (player.KeyCount > 0)
            {
                Unlock();
                player.KeyCount--;
                return true;
            }

            return false;
        }
    }

    private void Unlock()
    {
        IsOpen = true;

        if (AudioManager.getInstance() != null)
            AudioManager.getInstance().Find("unlockDoor").source.Play();
    }
}
