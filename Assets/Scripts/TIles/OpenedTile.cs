using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles
{
    public abstract class OpenedTile: Tile, IPirateEnter
    {
        public abstract void DoActionAfterOpenning();

        public abstract void EnterPirate(Pirate pirate);
    }
}
