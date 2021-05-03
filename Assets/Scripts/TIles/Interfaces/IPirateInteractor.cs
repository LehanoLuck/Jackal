using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles.Interfaces
{
    interface IPirateInteractor
    {
        void EnterPirate(Pirate pirate);
        void LeavePirate(Pirate pirate);
        void LeaveAllPirates();
        void TryAttack(Pirate pirate);
        void AddPirate(Pirate pirate);

    }
}
