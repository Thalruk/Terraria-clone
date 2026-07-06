
using UnityEngine;

public class WorldData
{
    public string name;
    public int seed;
    public int sizeX;
    public int sizeY;
    public Vector2 spawnPoint;
    public TileData[,] tiles;

    public WorldData(string worldName, int worldSizeX, int worldSizeY, int worldSeed, TileData[,] worldTiles = null)
    {
        name = worldName;
        sizeX = worldSizeX;
        sizeY = worldSizeY;
        seed = worldSeed;
        if (tiles != null)
        {
            tiles = worldTiles;
        }
        else
        {
            tiles = new TileData[worldSizeX, worldSizeY];
        }
    }
}
