using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.TIles.Interfaces
{
    interface ICoinInteractor
    {
        void AddCoin(Coin coin);

        Coin PopCoin();

        bool IsHaveCoins();
    }
}
