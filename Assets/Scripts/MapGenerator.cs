using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GroundTile GroundTileTemplate;
    public WaterTile WaterTileTemplate;
    public ShipTile ShipTileTemplate;
    public GroundTile[] HiddenTemplates;
    public Pirate PirateTemplate;
    public int countPirates = 10;
    public int Width;
    public int Length;
    public int shipCount = 3;

    public BaseTile[][] Map;

    private bool isLeftSideCreatedShip = false;
    private bool isRightSideCreatedShip = false;
    private bool isBottomSideCreatedShip = false;
    private bool isTopSideCreatedShip = false;


    // Start is called before the first frame update
    void Start()
    {
        GenerateMatrix();
        for (int i = 0; i < countPirates; i++)
        {
            AddPirateOnMap();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateMatrix()
    {
        Map = new BaseTile[Width][];

        for (int i = 0; i < this.Width; i++)
        {
            Map[i] = new BaseTile[Length];
            for (int j = 0; j < this.Length; j++)
            {
                if (i == 0 || i == Width - 1 || j == 0 || j == Length - 1)
                {
                    var isTrue = Random.Range(0f, 1f) > 0.7;
                    if (i == 0 && j != 0 && !isLeftSideCreatedShip && isTrue)
                    {
                        CreateShipTile(i, j);
                        isLeftSideCreatedShip = true;
                    }
                    else if (i == Width - 1 && j != Length - 1 && !isRightSideCreatedShip && isTrue)
                    {
                        CreateShipTile(i, j);
                        isRightSideCreatedShip = true;
                    }
                    else if (j == 0 && i != 0 && !isBottomSideCreatedShip && isTrue)
                    {
                        CreateShipTile(i, j);
                        isBottomSideCreatedShip = true;

                    }
                    else if (j == Length - 1 && i != Width - 1 && !isTopSideCreatedShip && isTrue)
                    {
                        CreateShipTile(i, j);
                        isTopSideCreatedShip = true;
                    }
                    else
                    {
                        CreateWaterTile(i, j);
                    }
                }
                else
                {
                    CreateGroundTile(i, j);
                }
            }
        }
    }

    private void PlaceTile(BaseTile tile, int i, int j)
    {
        float tileXSize = GroundTileTemplate.transform.localScale.x * 3.2f;
        float tileYSize = GroundTileTemplate.transform.localScale.y * 3.2f;

        tile.HorizontalIndex = i;
        tile.VerticalIndex = j;
        tile.SetTransformPosition(new Vector3(tileXSize * i, 0, tileYSize * j));
        Map[i][j] = tile;
        tile.Map = this.Map;
    }

    private void CreateGroundTile(int i, int j)
    {
        GroundTile tile = Instantiate(GroundTileTemplate, this.transform);

        PlaceTile(tile, i, j);
        tile.HiddenTile = HiddenTemplates[Random.Range(0, this.HiddenTemplates.Length)];
        tile.isHidden = false;
    }

    private void CreateWaterTile(int i, int j)
    {
        WaterTile tile = Instantiate(WaterTileTemplate, this.transform);
        PlaceTile(tile, i, j);
    }

    public void CreateShipTile(int i, int j)
    {
        ShipTile tile = Instantiate(ShipTileTemplate, this.transform);
        PlaceTile(tile, i, j);

        for(int k = 0; k < shipCount; k++)
        {
            AddPirateOnTile(i, j);
        }
    }

    private void AddPirateOnTile(int i, int j)
    {
        Pirate pirate = Instantiate(PirateTemplate, this.transform);

        Map[i][j].EnterPirate(pirate);
    }
    private void AddPirateOnMap()
    {
        int i = Random.Range(1, Width-1);
        int j = Random.Range(1, Length-1);

        this.AddPirateOnTile(i, j);
    }
}
