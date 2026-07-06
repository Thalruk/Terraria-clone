using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class WorldManager : MonoBehaviour
{
    public Dictionary<Vector2Int, byte> activeDamage = new Dictionary<Vector2Int, byte>();
    public static WorldManager Instance { get; private set; }
    public WorldData worldData;

    public int chunkSize = 16;
    public int seed = 0;
    public Vector2Int worldChunkSize = new Vector2Int(20, 10);

    public PolygonCollider2D worldBoundsCollider;
    [SerializeField] private CinemachineConfiner2D cameraConfiner;
    [SerializeField] WorldPreview preview;

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
    public async Task Generate()
    {
        int totalSizeX = worldChunkSize.x * chunkSize;
        int totalSizeY = worldChunkSize.y * chunkSize;
        if (worldBoundsCollider != null)
        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0, 0);
            points[1] = new Vector2(0, totalSizeY);
            points[2] = new Vector2(totalSizeX, totalSizeY);
            points[3] = new Vector2(totalSizeX, 0);

            worldBoundsCollider.points = points;
            if (cameraConfiner != null)
            {
                cameraConfiner.InvalidateCache();
            }
        }


        worldData = await WorldGenerator.GenerateWorldAsync("Test", totalSizeX, totalSizeY, seed);
        preview.GeneratePreview(worldData);

        //SaveManager.SaveWorld(worldData);

        //worldData = SaveManager.LoadWorld("Test");
    }

    public void SetTile(int x, int y, TileType newType)
    {
        if (x < 0 || x >= worldData.sizeX || y < 0 || y >= worldData.sizeY) return;

        worldData.tiles[x, y].type = newType;

        int targetChunkX = Mathf.FloorToInt((float)x / chunkSize);
        int targetChunkY = Mathf.FloorToInt((float)y / chunkSize);
        Vector2Int chunkCoord = new Vector2Int(targetChunkX, targetChunkY);

        ChunkManager.Instance.RefreshChunk(chunkCoord);
    }
    public void DamageTile(int x, int y, byte damageAmount)
    {
        if (x < 0 || x >= worldData.sizeX || y < 0 || y >= worldData.sizeY) return;

        TileType currentType = worldData.tiles[x, y].type;

        if (currentType == TileType.Air) return;

        Vector2Int pos = new Vector2Int(x, y);
        byte currentDamage = 0;

        if (activeDamage.ContainsKey(pos))
        {
            currentDamage = activeDamage[pos];
        }

        currentDamage += damageAmount;
        byte maxHp = currentType.GetMaxHP();

        if (currentDamage >= maxHp)
        {
            SetTile(x, y, TileType.Air);
            activeDamage.Remove(pos);

            ChunkManager.Instance.UpdateDamageVisual(pos, false);
        }
        else
        {
            activeDamage[pos] = currentDamage;

            ChunkManager.Instance.UpdateDamageVisual(pos, true);
        }
    }
}
