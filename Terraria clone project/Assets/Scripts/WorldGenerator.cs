using System.Threading.Tasks;
using UnityEngine;

public static class WorldGenerator
{
    public static async Task<WorldData> GenerateWorldAsync(string worldName, int worldSizeX, int worldSizeY, int seed)
    {
        WorldData worldData = new WorldData(worldName, worldSizeX, worldSizeY, seed, null);
        int[] heightMap = new int[worldSizeX];

        await Task.Run(() =>
        {
            GenerateTerrainBase(worldData, heightMap);
            GenerateStone(worldData, heightMap);
            GenerateCaves(worldData);
            worldData.spawnPoint = GetSpawnPoint(worldData);
        });
        return worldData;
    }

    public static void GenerateTerrainBase(WorldData worldData, int[] heightMap)
    {
        int maxElevation = 120;
        int seaLevel = worldData.sizeY * 1 / 2;
        int octaves = 3;
        float persistence = 0.4f;
        float lacunarity = 3f;
        float baseScale = 600f;

        for (int x = 0; x < worldData.sizeX; x++)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;
            float maxValue = 0f;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x / baseScale) * frequency + (worldData.seed % 100000) + (i * 5000);

                float perlinValue = Mathf.PerlinNoise(sampleX, 0f) * 2f - 1f;

                noiseHeight += perlinValue * amplitude;
                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            float normalizedHeight = (noiseHeight + maxValue) / (2f * maxValue);

            normalizedHeight = Mathf.Pow(normalizedHeight, 2f);

            int surfaceHeight = Mathf.RoundToInt(normalizedHeight * maxElevation) + seaLevel;
            heightMap[x] = surfaceHeight;

            for (int y = 0; y < worldData.sizeY; y++)
            {
                if (y > surfaceHeight)
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Air, wall = WallType.Air };
                }
                else if (y == surfaceHeight)
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Grass, wall = WallType.DirtWall };
                }
                else
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Dirt, wall = WallType.DirtWall };
                }
            }
        }
    }
    public static void GenerateStone(WorldData worldData, int[] heightMap)
    {
        float offsetX = worldData.seed % 100000;
        float offsetY = (worldData.seed / 10f) % 100000;

        for (int x = 0; x < worldData.sizeX; x++)
        {
            int surfaceHeight = heightMap[x];
            int maxStoneY = surfaceHeight - 5;

            for (int y = 0; y < maxStoneY; y++)
            {
                if (worldData.tiles[x, y].type == TileType.Dirt)
                {
                    float currentDepthRatio = 1f - ((float)y / maxStoneY);

                    float dynamicThreshold = Mathf.Lerp(0.6f, 0.15f, currentDepthRatio);

                    float noise = Mathf.PerlinNoise((x + offsetX) * 0.1f, (y + offsetY) * 0.1f);

                    if (noise > dynamicThreshold)
                    {
                        worldData.tiles[x, y] = new TileData(TileType.Stone, WallType.StoneWall);
                    }
                }
            }
        }
    }

    public static void GenerateCaves(WorldData worldData)
    {
        System.Random rng = new System.Random(worldData.seed);

        int tunnelCount = 20;

        for (int i = 0; i < tunnelCount; i++)
        {
            float currentX = rng.Next(20, worldData.sizeX - 20);
            float currentY = rng.Next(20, worldData.sizeY - 40);

            float angle = (float)(rng.NextDouble() * Mathf.PI * 2);

            int lifeTime = rng.Next(60, 150);

            float radius = rng.Next(3, 7);

            for (int step = 0; step < lifeTime; step++)
            {
                int cx = Mathf.RoundToInt(currentX);
                int cy = Mathf.RoundToInt(currentY);

                for (int tx = cx - Mathf.CeilToInt(radius); tx <= cx + Mathf.CeilToInt(radius); tx++)
                {
                    for (int ty = cy - Mathf.CeilToInt(radius); ty <= cy + Mathf.CeilToInt(radius); ty++)
                    {
                        if (tx >= 0 && tx < worldData.sizeX && ty >= 0 && ty < worldData.sizeY)
                        {
                            if (ty < worldData.sizeY - 30)
                            {
                                if (Vector2.Distance(new Vector2(cx, cy), new Vector2(tx, ty)) <= radius)
                                {
                                    worldData.tiles[tx, ty].type = TileType.Air;
                                }
                            }
                        }
                    }
                }

                currentX += Mathf.Cos(angle) * 1.5f;
                currentY += Mathf.Sin(angle) * 1.2f;

                angle += (float)(rng.NextDouble() * 0.6f - 0.3f);

                if (Mathf.Sin(angle) > 0.5f)
                {
                    angle -= 0.2f;
                }

                radius += (float)(rng.NextDouble() * 0.8f - 0.4f);
                radius = Mathf.Clamp(radius, 2f, 8f);

                if (rng.NextDouble() < 0.02f && lifeTime > 30)
                {
                    currentX += rng.Next(-3, 4);
                    angle += Mathf.PI / 2f;
                }
            }
        }
    }

    public static Vector2 GetSpawnPoint(WorldData worldData)
    {
        int spawnX = worldData.sizeX / 2;

        for (int y = worldData.sizeY - 1; y >= 0; y--)
        {
            if (worldData.tiles[spawnX, y].type != TileType.Air)
            {
                return new Vector2(spawnX, y + 2f);
            }
        }

        return new Vector2(spawnX, worldData.sizeY / 2f);
    }
}