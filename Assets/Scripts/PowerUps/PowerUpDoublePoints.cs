using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDoublePoints : BasePowerUp
{
        protected override bool ApplyToPlayer(Player player)
    {
        player.ScoreNetVar.Value *= 2;
        return true;
    }

}
