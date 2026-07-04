using System.Threading.Tasks;
using UnityEngine;

public static class WorldGenerator
{
    public static async Task<WorldData> GenerateWorldAsync(string worldName, int worldSizeX, int worldSizeY)
    {
        WorldData worldData = new WorldData(worldName, worldSizeX, worldSizeY);

        await Task.Run(() =>
        {
            for (int x = 0; x < worldSizeX; x++)
            {
                for (int y = 0; y < worldSizeY; y++)
                {
                    if (Mathf.PerlinNoise(x * 0.1f, y * 0.1f) > 0.5f)
                    {
                        worldData.tiles[x, y] = new TileData { type = TileType.Dirt };
                    }
                    else
                    {
                        worldData.tiles[x, y] = new TileData { type = TileType.Grass };
                    }
                }
            }
        });
        return worldData;
    }
}