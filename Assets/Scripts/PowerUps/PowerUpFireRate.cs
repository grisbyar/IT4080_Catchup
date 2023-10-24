using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFireRate : BasePowerUp
{
    public float timeBetweenBullets = .3f;
    protected override bool ApplyToPlayer(Player player) {  
        if(player.bulletSpawner.timeBetweenBullets <= timeBetweenBullets)
        {
            return false;
        }
        else
        {
            player.bulletSpawner.timeBetweenBullets = timeBetweenBullets;
            return true;
        }
        
      }
}
