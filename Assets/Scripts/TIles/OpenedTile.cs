using Assets.Scripts.TIles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.Scripts.TIles
{
    public abstract class OpenedTile: BasicTile
    {
        public virtual void DoAction()
        {
        }

        public virtual void Open(Pirate pirate)
        {
            EnterPirate(pirate);
            DoAction();
        }
    }
}
