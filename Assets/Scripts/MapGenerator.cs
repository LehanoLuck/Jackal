using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public VoxelTile TileTemplate;
    public VoxelTile[] HiddenTemplates;
    public Pirate PirateTemplate;
    public int countPirates = 10;
    public int Width;
    public int Length;

    public VoxelTile[][] Map;

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
        float tileXSize = TileTemplate.transform.localScale.x * 3.2f;
        float tileYSize = TileTemplate.transform.localScale.y * 3.2f;

        Map = new VoxelTile[Width][];

        for (int i = 0; i < this.Width; i++)
        {
            Map[i] = new VoxelTile[Length];
            for (int j = 0; j < this.Length; j++)
            {
                VoxelTile tile = Instantiate(TileTemplate, this.transform);
                tile.HorizontalIndex = i;
                tile.VerticalIndex = j;
                tile.transform.position = new Vector3(tileXSize * i, 0, tileYSize * j);
                Map[i][j] = tile;
            }
        }
    }

    private void AddPirateOnMap()
    {
        int i = Random.Range(0, Width);
        int j = Random.Range(0, Length);

        Pirate pirate = Instantiate(PirateTemplate, this.transform);

        Map[i][j].AddPirate(pirate);
    }
}
