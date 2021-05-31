using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles
{
    public class TetroArrowsTileVersionTwo: ArrowTile
    {
        public List<Point> PossiblePositions = new List<Point>()
        {
            new Point(1,1),
            new Point(-1,1),
            new Point(-1,-1),
            new Point(1,-1)
        };

        public override bool IsPossibleForMove(Tile targetTile)
        {
            foreach (var point in PossiblePositions)
            {
                if (this.XPos + point.X == targetTile.XPos && this.YPos + point.Y == targetTile.YPos)
                    return true;
            }

            return false;
        }

        public override void DoAction()
        {
        }
    }
}
