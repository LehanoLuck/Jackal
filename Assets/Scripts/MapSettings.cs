using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Assets.Scripts
{
    enum TilesCategory
    {
        Water,
        Ground,
        Coin,
        Arrow,
        Ice,
        Cave
    }
    public class MapSettings
    {
        public MapSettings()
        {
            TilesCategoryTable = new Hashtable();
            TilesCategoryTable.Add(TilesCategory.Ground, 10);
            TilesCategoryTable.Add(TilesCategory.Coin, 15);
            TilesCategoryTable.Add(TilesCategory.Arrow, 25);
            TilesCategoryTable.Add(TilesCategory.Ice,12);
            TilesCategoryTable.Add(TilesCategory.Cave,8);
        }

        public MapSettings(int width, int length, int coins, int ground)
        {
            int amount = width * length;
            int percents = coins + ground;
            double factor = (double)percents / (double)amount;
            int coinsCount = (int)((double)coins / factor);
            int groundCount = amount - coinsCount;
            TilesCategoryTable.Add(TilesCategory.Ground, groundCount);
            TilesCategoryTable.Add(TilesCategory.Coin, coinsCount);

            GroundWidth = width;
            GroundLength = length;
        }

        public Hashtable TilesCategoryTable = new Hashtable();

        public int LeftWaterSide = 2;
        public int RightWaterSide = 2;
        public int TopWaterSide = 2;
        public int BottomWaterSide = 2;

        public int GroundWidth = 8;
        public int GroundLength = 8;

        public int MapWidth => GroundWidth + LeftWaterSide + RightWaterSide;
        public int MapLength => GroundLength + TopWaterSide + BottomWaterSide;
        public int CountGroundTiles => GroundLength * GroundWidth;

        public static MapSettings Default { get; private set; } = new MapSettings();
    }
}
