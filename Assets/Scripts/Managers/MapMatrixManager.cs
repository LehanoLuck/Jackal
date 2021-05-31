using System;
using System.Linq;

namespace Assets.Scripts
{
    public class MapMatrixManager
    {
        public static int CoinsCount;
        public static int ArrowsCount;
        public static int StartCoinsCount;

        public static int[][] GenerationMapMatrix;

        public static void SetGenerationMapMatrix(int[][] mapMatrix)
        {
            GenerationMapMatrix = mapMatrix;
            CoinsCount = mapMatrix.SelectMany(t => t).Where(t => t == 2).ToArray().Length;
            StartCoinsCount = CoinsCount;
        }

        public static int[][] CreateRandomGenerationMapMatrix(MapSettings mapSettings)
        {
            Random random = new Random();
            var mapMatrix = new int[mapSettings.MapWidth][];

            for (int i = 0; i < mapSettings.MapWidth; i++)
            {
                mapMatrix[i] = new int[mapSettings.MapLength];
            }

            int emptyTilesCount = mapSettings.CountGroundTiles;

            for (int i = 0; i < mapMatrix.Length; i++)
            {
                for (int j = 0; j < mapMatrix[i].Length; j++)
                {
                    if (i < mapSettings.LeftWaterSide ||
                        i >= mapSettings.MapWidth - mapSettings.RightWaterSide ||
                        j < mapSettings.TopWaterSide ||
                        j >= mapSettings.MapLength - mapSettings.BottomWaterSide)
                    {
                        mapMatrix[i][j] = (int)TilesCategory.Water;
                    }
                    else
                    {
                        //Генерируем случайное число в диапазоне от 0 до 1
                        var factor = random.NextDouble();
                        //Барьер, необходимый для того чтобы определить какой тайл мы поставим на выбранную позицию
                        float categoryBarrier = 0;

                        foreach (TilesCategory category in mapSettings.TilesCategoryTable.Keys)
                        {
                            //Вероятностое распределние в зависимости от количества оставшихся свободных тайлов и кол-ва тайлов данной категории
                            categoryBarrier += (int)mapSettings.TilesCategoryTable[category] / (float)emptyTilesCount;

                            if (factor <= categoryBarrier)
                            {
                                mapMatrix[i][j] = (int)category;
                                mapSettings.TilesCategoryTable[category] = (int)mapSettings.TilesCategoryTable[category] - 1;
                                emptyTilesCount--;
                                break;
                            }
                        }
                    }
                }
            }

            return mapMatrix;
        }
    }
}
