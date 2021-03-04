using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Assets.Scripts
{
    public class MapSettings
    {
        public MapSettings()
        {
            TilesCategoryTable = new Hashtable();
            TilesCategoryTable.Add(1, 39);
            TilesCategoryTable.Add(2, 10);
        }

        public MapSettings(int width, int length, int coins, int ground)
        {
            int amount = width * length;
            int percents = coins + ground;
            double factor = (double)percents / (double)amount;
            int coinsCount = (int)((double)coins / factor);
            int groundCount = amount - coinsCount;
            TilesCategoryTable.Add(1, groundCount);
            TilesCategoryTable.Add(2, coinsCount);

            GroundWidth = width;
            GroundLength = length;
        }

        public Hashtable TilesCategoryTable = new Hashtable();

        public int LeftWaterSide = 2;
        public int RightWaterSide = 2;
        public int TopWaterSide = 2;
        public int BottomWaterSide = 2;

        public int GroundWidth = 7;
        public int GroundLength = 7;

        public int MinEmptyGroundTiles = 10;

        public int MapWidth => GroundWidth + LeftWaterSide + RightWaterSide;
        public int MapLength => GroundLength + TopWaterSide + BottomWaterSide;
        public int CountGroundTiles => GroundLength * GroundWidth;

        public static MapSettings Default { get; private set; } = new MapSettings();

        public void AddTilesCategory(int count, int categoryIndex)
        {
            this.TilesCategoryTable.Add(categoryIndex, count);
        }
    }
}
