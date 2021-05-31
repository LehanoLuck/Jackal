using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles
{
    public class IceTile: GroundTile
    {
        private int xOffset;
        private int yOffset;

        public override void EnterPirate(Pirate pirate)
        {
            xOffset = this.XPos - pirate.CurrentTile.XPos;
            yOffset = this.YPos - pirate.CurrentTile.YPos;

            base.EnterPirate(pirate);
            pirate.isActive = true;
        }

        public override bool IsPossibleForMove(Tile targetTile)
        {
            return (this.XPos + xOffset == targetTile.XPos && this.YPos + yOffset == targetTile.YPos);
        }
    }
}
