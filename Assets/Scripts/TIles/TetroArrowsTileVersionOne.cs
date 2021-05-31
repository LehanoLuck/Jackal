using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TIles
{
    public class TetroArrowsTileVersionOne: ArrowTile
    {
        public List<Point> PossiblePositions = new List<Point>()
        {
            new Point(0,1),
            new Point(-1,0),
            new Point(1,0),
            new Point(0,-1)
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
