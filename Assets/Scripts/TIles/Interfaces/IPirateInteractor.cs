using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles.Interfaces
{
    interface IPirateInteractor: IPirateEnter
    {
        void LeavePirate(Pirate pirate);
        void LeaveAllPirates();
        Pirate GetPirate();
    }
}
