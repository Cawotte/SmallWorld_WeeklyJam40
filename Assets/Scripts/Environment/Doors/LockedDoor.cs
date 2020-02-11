using Cawotte.Toolbox.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door
{

    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager = null;

    [SerializeField]
    private Sound unlockingDoor = null;

    private AudioSourcePlayer audioPlayer = null;

    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
    }


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

        audioPlayer.PlaySound(unlockingDoor);
    }
}
