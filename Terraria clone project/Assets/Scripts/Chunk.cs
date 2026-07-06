using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    [SerializeField] Tilemap wallMap;
    [SerializeField] Tilemap baseMap;
    [SerializeField] Tilemap damageMap;

    [Header("Tiles")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase stoneTile;

    [Header("Walls")]
    [SerializeField] private TileBase dirtWall;
    [SerializeField] private TileBase stoneWall;

    [Header("Damage")]
    [SerializeField] private TileBase singleDamageTile;
    public void SetUp(Vector2 posiition)
    {
        transform.position = posiition;
    }
    public void RenderWorld(WorldData worldData, int chunkX, int chunkY, int chunkSize)
    {
        TileBase[] tileArray = new TileBase[chunkSize * chunkSize];
        TileBase[] wallArray = new TileBase[chunkSize * chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int globalX = (chunkX * chunkSize) + x;
                int globalY = (chunkY * chunkSize) + y;

                int index = x + (y * chunkSize);
                tileArray[index] = worldData.tiles[globalX, globalY].type switch
                {
                    TileType.Air => null,
                    TileType.Grass => grassTile,
                    TileType.Dirt => dirtTile,
                    TileType.Stone => stoneTile,
                    _ => grassTile,
                };

                wallArray[index] = worldData.tiles[globalX, globalY].wall switch
                {
                    WallType.Air => null,
                    WallType.DirtWall => dirtWall,
                    WallType.StoneWall => stoneWall,
                    _ => null,
                };
            }
        }

        BoundsInt bounds = new BoundsInt(0, 0, 0, chunkSize, chunkSize, 1);
        baseMap.ClearAllTiles();
        baseMap.SetTilesBlock(bounds, tileArray);

        wallMap.ClearAllTiles();
        wallMap.SetTilesBlock(bounds, wallArray);
    }


    public void UpdateDamageVisual(int localX, int localY, bool isDamaged)
    {
        TileBase tileToSet = isDamaged ? singleDamageTile : null;
        damageMap.SetTile(new Vector3Int(localX, localY, 0), tileToSet);
    }
    public void SetDamage(int localX, int localY, TileBase tile)
    {
        damageMap.SetTile(new Vector3Int(localX, localY, 0), tile);
    }

    internal void ClearTilemaps()
    {
        baseMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        damageMap.ClearAllTiles();
    }
}