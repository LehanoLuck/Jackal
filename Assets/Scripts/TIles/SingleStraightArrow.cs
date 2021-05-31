using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TIles
{
    public class SingleStraightArrow: ArrowTile
    {
        private int XPossible = 0;
        private int YPossible = -1;

        public override bool IsPossibleForMove(Tile targetTile)
        {
            return (targetTile.XPos == this.XPos + XPossible && targetTile.YPos == this.YPos + YPossible);
        }

        public override void DoAction()
        {
            var turple = RotateTile(rotationAngle, XPossible, YPossible);
            XPossible = turple.Item1;
            YPossible = turple.Item2;

            transform.Rotate(Vector3.up, rotationAngle * 90f);
        }
    }
}
