using System.Collections.Generic;
using UnityEngine;

public class ChunkPoolManager : MonoBehaviour
{
    public static ChunkPoolManager Instance { get; private set; }
    [SerializeField] private Chunk chunkPrefab;

    private readonly List<Chunk> activeChunks = new List<Chunk>();
    private readonly List<Chunk> inactiveChunks = new List<Chunk>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Chunk AcquireChunk()
    {
        if (inactiveChunks.Count > 0)
        {
            Chunk chunk = inactiveChunks[0];
            inactiveChunks.RemoveAt(0);
            activeChunks.Add(chunk);
            chunk.gameObject.SetActive(true);
            chunk.ClearTilemaps();
            return chunk;
        }
        else
        {
            Chunk newChunk = Instantiate(chunkPrefab, Vector2.zero, Quaternion.identity, transform);
            activeChunks.Add(newChunk);
            return newChunk;
        }
    }

    public void ReleaseChunk(Chunk chunk)
    {
        activeChunks.Remove(chunk);
        inactiveChunks.Add(chunk);
        chunk.gameObject.SetActive(false);
    }
}
