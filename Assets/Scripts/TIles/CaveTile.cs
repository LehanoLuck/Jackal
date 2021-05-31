using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum CaveStatus
    {
        canEnter,
        canLeave
    }

    public class CaveTile: GroundTile
    {
        public ParticleSystem CaveActiveLight;

        public CaveStatus CaveStatus = CaveStatus.canEnter;

        private void SetActive(bool canEnter)
        {
            if (canEnter)
            {
                CaveStatus = CaveStatus.canEnter;
                CaveActiveLight.Play();
            }
            else
            {
                CaveStatus = CaveStatus.canLeave;
                CaveActiveLight.Stop();
            }
        }

        public override void EnterPirate(Pirate pirate)
        {
            base.EnterPirate(pirate);

            if (CanMoveAfterEntering() && CaveStatus == CaveStatus.canEnter)
            {
                pirate.isActive = true;
            }
        }

        private bool CanMoveAfterEntering()
        {
            var otherCaves = GetOtherCaves();

            if (otherCaves.Any())
            {
                foreach (CaveTile cave in otherCaves)
                {
                    if (cave.CaveStatus == CaveStatus.canEnter)
                    {
                        if (!(cave.isHavePirates) || otherCaves.Count() == 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        public override void LeavePirate(Pirate pirate)
        {
            base.LeavePirate(pirate);

            if (CaveStatus == CaveStatus.canEnter)
            {
                RelocateToAnotherCave(pirate);
            }
            else
            {
                SetActive(true);
            }
        }

        private void RelocatePirate(Pirate pirate)
        {
            base.LeavePirate(pirate);
        }

        public IEnumerable<Tile> GetOtherCaves()
        {
            IEnumerable<Tile> caves = Map.SelectMany(t => t).Where(t => t is CaveTile && t != this);
            return caves;
        }

        public void RelocateToAnotherCave(Pirate pirate)
        {
            var targetTile = (CaveTile)pirate.TargetTile;
            
            var other = targetTile.Pirates.FirstOrDefault();

            targetTile.SetActive(false);

            if (other)
            {
                SetActive(false);
                other.TargetTile = this;
                targetTile.RelocatePirate(other);
                this.EnterPirate(other);
            }
        }

        public override bool IsPossibleForMove(Tile targetTile)
        {
            if (CaveStatus == CaveStatus.canEnter)
            {
                var caves = GetOtherCaves();
                if (caves.Any())
                {
                    foreach (var cave in caves)
                    {
                        if (cave.XPos == targetTile.XPos && cave.YPos == targetTile.YPos && CanEnterInCave())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                return base.IsPossibleForMove(targetTile);
            }

            bool CanEnterInCave()
            {
                var caveTile = ((CaveTile) targetTile);

                return (caveTile.CaveStatus == CaveStatus.canEnter);
            }
        }

    }
}
