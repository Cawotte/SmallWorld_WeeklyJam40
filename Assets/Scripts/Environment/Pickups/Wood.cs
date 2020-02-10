
using UnityEngine;

public class Wood : Pickup
{
    protected override void onPickup(Player player)
    {
        player.WoodCount++;
    }
}
