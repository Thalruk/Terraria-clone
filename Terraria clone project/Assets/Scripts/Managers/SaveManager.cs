using System.IO;
using System.Text;
using UnityEngine;

public class SaveManager
{
    public static void SaveWorld(WorldData worldData)
    {
        using (Stream stream = File.Open(GetWorldFilePath(worldData.name), FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(worldData.name);
                writer.Write(worldData.sizeX);
                writer.Write(worldData.sizeY);
                writer.Write(worldData.seed);
                writer.Write(worldData.spawnPoint.x);
                writer.Write(worldData.spawnPoint.y);
                for (int x = 0; x < worldData.sizeX; x++)
                {
                    for (int y = 0; y < worldData.sizeY; y++)
                    {
                        writer.Write((byte)worldData.tiles[x, y].type);
                        writer.Write((byte)worldData.tiles[x, y].wall);
                    }
                }
            }
        }
        Debug.Log($"World succesfully saved in: {Application.persistentDataPath}");
    }

    public static WorldData LoadWorld(string worldName)
    {
        using (Stream stream = File.Open(GetWorldFilePath(worldName), FileMode.Open))
        {
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                string loadedName = reader.ReadString();
                int loadedSizeX = reader.ReadInt32();
                int loadedSizeY = reader.ReadInt32();
                int seed = reader.ReadInt32();
                Vector2 spawnPointString = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                TileData[,] tiles = new TileData[loadedSizeX, loadedSizeY];
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < tiles.GetLength(1); y++)
                    {
                        tiles[x, y] = new TileData { type = (TileType)reader.ReadByte(), wall = (WallType)reader.ReadByte() };
                    }
                }

                Debug.Log($"World succesfully loaded in: {Application.persistentDataPath}");
                return new WorldData(loadedName, loadedSizeX, loadedSizeY, seed, tiles);
            }
        }

    }

    private static string GetWorldFilePath(string worldName)
    {
        return Path.Combine(Application.persistentDataPath, worldName + ".map");
    }
}
