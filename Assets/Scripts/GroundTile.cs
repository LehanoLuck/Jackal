using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts
{

    public class GroundTile : BaseTile
    {
        public GroundTile HiddenTile;

        public bool isHidden;

        public virtual GroundTile OpenTile()
        {
            var hidden = Instantiate(HiddenTile, this.transform.parent);
            hidden.SetTransformPosition(this.fixedPosition);
            hidden.HorizontalIndex = this.HorizontalIndex;
            hidden.VerticalIndex = this.VerticalIndex;
            Map[HorizontalIndex][VerticalIndex] = hidden;
            hidden.Pirates = this.Pirates;

            Destroy(gameObject);

            hidden.DoActionAfterOpenning();
            return hidden;
        }

        protected override void SetCurrentPirateTile(Pirate pirate)
        {
            if (!isHidden)
            {
                var hidden = this.OpenTile();
                hidden.SetCurrentPirateTile(pirate);

                //if(pirate.isMoveWithCoin)
                //{
                //    hidden.AddCoin(pirate.SelfCoin);
                //    pirate.SelfCoin.transform.position = hidden.transform.position;
                //}
            }
            else
            {
                base.SetCurrentPirateTile(pirate);
            }
        }

        public virtual void DoActionAfterOpenning()
        {
        }
    }
}
