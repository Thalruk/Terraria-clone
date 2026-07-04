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
        });
        return worldData;
    }

    public static void GenerateTerrainBase(WorldData worldData, int[] heightMap)
    {
        // Parametry kszta³towania terenu
        int maxElevation = 120; // Maksymalna wysokoœæ gór w klockach
        int seaLevel = worldData.sizeY * 2 / 3; // Poziom bazowy powierzchni
        int octaves = 3; // Iloœæ warstw szumu
        float persistence = 0.4f; // Jak szybko malej¹ góry w kolejnych oktawach
        float lacunarity = 3f; // Jak szybko zagêszczaj¹ siê detale
        float baseScale = 600f; // G³ówny dzielnik rozci¹gaj¹cy kontynenty

        for (int x = 0; x < worldData.sizeX; x++)
        {
            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;
            float maxValue = 0f;

            // Nak³adanie warstw szumu (Fractal Brownian Motion)
            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x / baseScale) * frequency + (worldData.seed % 100000) + (i * 5000);

                // Szum od -1 do 1
                float perlinValue = Mathf.PerlinNoise(sampleX, 0f) * 2f - 1f;

                noiseHeight += perlinValue * amplitude;
                maxValue += amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            // Normalizacja wyniku do bezpiecznego przedzia³u 0.0 - 1.0
            float normalizedHeight = (noiseHeight + maxValue) / (2f * maxValue);

            // Redystrybucja - potêgowanie sp³aszcza doliny i wyostrza szczyty
            normalizedHeight = Mathf.Pow(normalizedHeight, 2f);

            // Ostateczna kalkulacja pozycji Y dla powierzchni
            int surfaceHeight = Mathf.RoundToInt(normalizedHeight * maxElevation) + seaLevel;
            heightMap[x] = surfaceHeight;

            // Wype³nianie kolumny w dó³ i w górê
            for (int y = 0; y < worldData.sizeY; y++)
            {
                if (y > surfaceHeight)
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Air };
                }
                else if (y == surfaceHeight)
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Grass };
                }
                else
                {
                    worldData.tiles[x, y] = new TileData { type = TileType.Dirt };
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
                        worldData.tiles[x, y].type = TileType.Stone;
                    }
                }
            }
        }
    }

    public static void GenerateCaves(WorldData worldData)
    {
        System.Random rng = new System.Random(worldData.seed);

        // 1. Inicjalizacja: Wype³niamy kamieñ/ziemiê "dziurami" (szum startowy)
        // 40% szansy na Air jest idealne dla jaskiñ, które siê ³¹cz¹, ale zostawiaj¹ œciany
        for (int x = 0; x < worldData.sizeX; x++)
        {
            for (int y = 0; y < worldData.sizeY; y++)
            {
                // Nie ruszamy powierzchni (np. top 10 klocków)
                if (y > worldData.sizeY - 10) continue;

                if (worldData.tiles[x, y].type == TileType.Dirt || worldData.tiles[x, y].type == TileType.Stone)
                {
                    if (rng.NextDouble() < 0.40f)
                        worldData.tiles[x, y].type = TileType.Air;
                }
            }
        }

        // 2. Iteracje Cellular Automata (4 przejœcia wystarcz¹ dla tej skali)
        for (int i = 0; i < 4; i++)
        {
            TileType[,] nextMap = new TileType[worldData.sizeX, worldData.sizeY];

            for (int x = 0; x < worldData.sizeX; x++)
            {
                for (int y = 0; y < worldData.sizeY; y++)
                {
                    int walls = CountAliveNeighbors(worldData, x, y);

                    // Regu³a: Jeœli ma mniej ni¿ 4 s¹siadów - staje siê powietrzem
                    // Jeœli ma 5 lub wiêcej - staje siê œcian¹
                    if (walls < 4) nextMap[x, y] = TileType.Air;
                    else nextMap[x, y] = worldData.tiles[x, y].type;
                }
            }

            // Kopiowanie zmian
            for (int x = 0; x < worldData.sizeX; x++)
                for (int y = 0; y < worldData.sizeY; y++)
                    worldData.tiles[x, y].type = nextMap[x, y];
        }
    }
    private static int CountAliveNeighbors(WorldData worldData, int x, int y)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Nie liczymy samego siebie

                int nx = x + i;
                int ny = y + j;

                // Sprawdzamy granice mapy
                if (nx >= 0 && nx < worldData.sizeX && ny >= 0 && ny < worldData.sizeY)
                {
                    // Liczymy jako s¹siada wszystko, co nie jest powietrzem
                    if (worldData.tiles[nx, ny].type != TileType.Air) count++;
                }
                else
                {
                    // Krawêdzie mapy traktujemy jako œciany, ¿eby jaskinie nie "wylewa³y siê" poza œwiat
                    count++;
                }
            }
        }
        return count;
    }
}