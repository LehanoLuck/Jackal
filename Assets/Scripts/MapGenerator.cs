using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public VoxelTile[] TileTemplates;
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
        for (int i = 0; i < this.Width; i++)
        {
            for (int j = 0; j < this.Length; j++)
            {
                VoxelTile tile = Instantiate(TileTemplates[Random.Range(0, TileTemplates.Length)], this.transform);
                tile.transform.position = new Vector3(tile.transform.localScale.x * 3.2f * i, 0, tile.transform.localScale.z * 3.2f * j);
            }
        }
    }
}
