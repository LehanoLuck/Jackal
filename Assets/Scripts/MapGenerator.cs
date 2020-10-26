using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public VoxelTile[] TileTemplates;
    public Pirate PirateTemplate;
    private Transform Parent;
    public int Width;
    public int Length;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMatrix();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateMatrix()
    {
        float tileXSize = TileTemplates[0].transform.localScale.x * 3.2f;
        float tileYSize = TileTemplates[0].transform.localScale.y * 3.2f;

        List<VoxelTile> tiles = new List<VoxelTile>();
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Length; j++)
            {
                VoxelTile tile = Instantiate(TileTemplates[0], this.transform);
                tile.HorizontalIndex = i;
                tile.VerticalIndex = j;
                tile.transform.position = new Vector3(tileXSize * i, 0, tileYSize * j);
                tiles.Add(tile);
            }
        }

        int index = Random.Range(0, tiles.Count);

        Pirate pirate = Instantiate(PirateTemplate, this.transform);
        pirate.transform.position = tiles[index].transform.position;
        pirate.CurrentTile = tiles[index];
    }
}
