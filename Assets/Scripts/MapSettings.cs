using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Assets.Scripts
{
    //public enum TilesCategory
    //{
    //    EmptyGround,
    //    Coin,
    //    Water
    //}

    public class MapSettings
    {
        public MapSettings()
        {
        }

        public Hashtable TilesCategoryTable = new Hashtable();

        private int[] TilesCategory = { 0, 1, 2 };

        public int LeftWaterSide = 2;
        public int RightWaterSide = 2;
        public int TopWaterSide = 2;
        public int BottomWaterSide = 2;

        public int GroundWidth = 10;
        public int GroundLength = 10;

        public int MinEmptyGroundTiles = 10;

        public int MapWidth => GroundWidth + LeftWaterSide + RightWaterSide;
        public int MapLength => GroundLength + TopWaterSide + BottomWaterSide;
        public int CountGroundTiles => GroundLength * GroundWidth;


        private static readonly MapSettings defaultSettings = new MapSettings
        {
            LeftWaterSide = 1,
            RightWaterSide = 1,
            TopWaterSide = 1,
            BottomWaterSide = 1,
            GroundWidth = 4,
            GroundLength = 4,
            MinEmptyGroundTiles = 1,
        };

        public static MapSettings Default
        {
            get
            {
                //TODO: Придумать как не добавлять каждый раз новую HashTable
                var defaultMapSettings = defaultSettings;
                defaultSettings.TilesCategoryTable = new Hashtable();
                defaultSettings.TilesCategoryTable.Add(1, 7);
                defaultSettings.TilesCategoryTable.Add(2, 9);

                return defaultMapSettings;
            }
            
        }

        public void AddTilesCategory(int count, int categoryIndex)
        {
            this.TilesCategoryTable.Add(categoryIndex, count);
        }
    }
}
