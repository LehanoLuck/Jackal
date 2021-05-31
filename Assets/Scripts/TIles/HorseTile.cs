using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TIles
{
    public class HorseTile: ArrowTile
    {
        public override void EnterPirate(Pirate pirate)
        {
            base.EnterPirate(pirate);
            pirate.isActive = true;
        }

        public override bool IsPossibleForMove(Tile targetTile)
        {
            return ((Mathf.Abs(this.XPos - targetTile.XPos) == 2 && Mathf.Abs(this.YPos - targetTile.YPos) == 1) ||
                    (Mathf.Abs(this.XPos - targetTile.XPos) == 1 && Mathf.Abs(this.YPos - targetTile.YPos) == 2));
        }
    }
}
