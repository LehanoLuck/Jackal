﻿using System;
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

        public override void EnterPirate(Pirate pirate)
        {
            base.EnterPirate(pirate);
        }

        public GroundTile OpenTile()
        {
            var hidden = Instantiate(HiddenTile, this.transform.parent);
            hidden.SetTransformPosition(this.fixedPosition);
            hidden.HorizontalIndex = this.HorizontalIndex;
            hidden.VerticalIndex = this.VerticalIndex;
            Map[HorizontalIndex][VerticalIndex] = hidden;
            hidden.Pirates = this.Pirates;

            Destroy(gameObject);

            return hidden;
        }

        protected override void SetCurrentPirateTile(Pirate pirate)
        {
            if (!isHidden)
            {
                pirate.CurrentTile = OpenTile();
            }
            else
            {
                base.SetCurrentPirateTile(pirate);
            }
        }
    }
}