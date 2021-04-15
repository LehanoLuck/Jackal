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

        public bool IsHidden;

        public virtual GroundTile OpenTile()
        {
            var hidden = Instantiate(HiddenTile, this.transform.parent);
            hidden.SetTransformPosition(this.fixedPosition);
            hidden.XPos = this.XPos;
            hidden.YPos = this.YPos;
            hidden.Map = this.Map;
            Map[XPos][YPos] = hidden;
            hidden.Pirates = this.Pirates;

            Destroy(gameObject);

            hidden.DoActionAfterOpenning();
            return hidden;
        }

        protected override void SetCurrentPirateTile(Pirate pirate)
        {
            if (!IsHidden)
            {
                var hidden = this.OpenTile();
                hidden.SetCurrentPirateTile(pirate);
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
