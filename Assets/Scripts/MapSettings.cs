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

        public int GroundWidth = 6;
        public int GroundLength = 6;

        public int MinEmptyGroundTiles = 10;

        public int MapWidth => GroundWidth + LeftWaterSide + RightWaterSide;
        public int MapLength => GroundLength + TopWaterSide + BottomWaterSide;
        public int CountGroundTiles => GroundLength * GroundWidth;


        private static readonly MapSettings defaultSettings = new MapSettings();

        public static MapSettings Default
        {
            get
            {
                //TODO: Придумать как не добавлять каждый раз новую HashTable
                var defaultMapSettings = defaultSettings;
                defaultSettings.TilesCategoryTable = new Hashtable();
                defaultSettings.TilesCategoryTable.Add(1, 10);
                defaultSettings.TilesCategoryTable.Add(2, 26);

                return defaultMapSettings;
            }
            
        }

        public void AddTilesCategory(int count, int categoryIndex)
        {
            this.TilesCategoryTable.Add(categoryIndex, count);
        }
    }
}
