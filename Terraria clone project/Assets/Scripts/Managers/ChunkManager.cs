using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] Transform watchedObject;

    [SerializeField] int renderDistance = 2;
    [SerializeField] int chunkSize;
    [SerializeField] float checkInterval = 1f;
    Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();


    void Start()
    {
        chunkSize = WorldManager.Instance.chunkSize;
        StartCoroutine(CheckChunksLoop());
    }
    IEnumerator CheckChunksLoop()
    {
        while (true)
        {
            if (WorldManager.Instance == null || WorldManager.Instance.worldData == null)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            Vector2Int currentChunkCoord = new Vector2Int(
                Mathf.FloorToInt(watchedObject.position.x / chunkSize),
                Mathf.FloorToInt(watchedObject.position.y / chunkSize)
            );

            HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

            for (int chunkX = currentChunkCoord.x - renderDistance; chunkX <= currentChunkCoord.x + renderDistance; chunkX++)
            {
                for (int chunkY = currentChunkCoord.y - renderDistance; chunkY <= currentChunkCoord.y + renderDistance; chunkY++)
                {
                    if (chunkX < 0 || chunkX >= WorldManager.Instance.worldChunkSize.x || chunkY < 0 || chunkY >= WorldManager.Instance.worldChunkSize.y)
                    {
                        continue;
                    }
                    neededChunks.Add(new Vector2Int(chunkX, chunkY));
                }
            }

            List<Vector2Int> chunksToRemove = new List<Vector2Int>();
            foreach (var pair in activeChunks)
            {
                if (!neededChunks.Contains(pair.Key))
                {
                    ChunkPoolManager.Instance.ReleaseChunk(pair.Value);
                    chunksToRemove.Add(pair.Key);
                }
            }

            foreach (Vector2Int coord in chunksToRemove)
            {
                activeChunks.Remove(coord);
            }

            foreach (Vector2Int coord in neededChunks)
            {
                if (!activeChunks.ContainsKey(coord))
                {
                    Chunk newChunk = ChunkPoolManager.Instance.AcquireChunk();
                    newChunk.SetUp(new Vector2(coord.x * chunkSize, coord.y * chunkSize));
                    newChunk.RenderWorld(WorldManager.Instance.worldData, coord.x, coord.y, chunkSize);

                    activeChunks.Add(coord, newChunk);
                }
            }


            yield return new WaitForSeconds(checkInterval);
        }
    }
}
