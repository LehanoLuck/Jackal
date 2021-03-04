using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;

namespace Assets.Scripts
{
    public enum VictoryCondition
    {
        CollectMinimumCoins,
        CollectAllCoins
    }

    public static class EndGameManager
    {

        public static bool IsWin(VictoryCondition condition, GamePlayer player)
        {
            switch (condition)
            {
                case VictoryCondition.CollectMinimumCoins:
                    return IsWinWithCollectMinimumCoins(player);
                //case VictoryCondition.CollectAllCoins:
                default:
                    return false;
            }
        }

        private static bool IsWinWithCollectMinimumCoins(GamePlayer player)
        {
            return player.PlayerUI.CoinsCount >= (double)MapMatrixManager.StartCoinsCount / (double)PhotonNetwork.CurrentRoom.PlayerCount;
        }

        //private static bool IsWinWithCollectAllCoins(GamePlayer player)
        //{
        //    if(MapMatrixManager.CoinsCount == 0)
        //    {
        //        var winner = PhotonNetwork.CurrentRoom.Players.S
        //    }
        //}
    }
}
