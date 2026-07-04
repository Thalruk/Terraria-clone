using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    [Header("Grafiki kafelków")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase dirtTile;

    public void SetUp(Vector2 posiition)
    {
        transform.position = posiition;
    }
    public void RenderWorld(WorldData worldData, int chunkX, int chunkY, int chunkSize)
    {
        TileBase[] tileArray = new TileBase[chunkSize * chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int globalX = (chunkX * chunkSize) + x;
                int globalY = (chunkY * chunkSize) + y;

                int index = x + (y * chunkSize);
                tileArray[index] = worldData.tiles[globalX, globalY].type switch
                {
                    TileType.Air => dirtTile,
                    TileType.Grass => grassTile,
                    TileType.Dirt => dirtTile,
                    TileType.Stone => grassTile,
                    _ => grassTile,
                };
            }
        }

        BoundsInt bounds = new BoundsInt(0, 0, 0, chunkSize, chunkSize, 1);
        tilemap.ClearAllTiles();
        tilemap.SetTilesBlock(bounds, tileArray);
    }

    internal void ClearTilemap()
    {
        tilemap.ClearAllTiles();
    }
}