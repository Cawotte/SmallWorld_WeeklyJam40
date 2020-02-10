
using Cawotte.Toolbox.Audio;
using UnityEngine;

public class Key : Pickup
{
    protected override void onPickup(Player player)
    {
        player.KeyCount++;
    }
}
