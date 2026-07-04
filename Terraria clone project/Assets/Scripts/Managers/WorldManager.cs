using Cinemachine;
using UnityEngine;
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    public WorldData worldData;

    public int chunkSize = 16;
    public Vector2Int worldChunkSize = new Vector2Int(20, 10);

    [SerializeField] Vector2 spawnPoint;
    [SerializeField] private PolygonCollider2D worldBoundsCollider;
    [SerializeField] private CinemachineConfiner2D cameraConfiner;
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
    private void Start()
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


        //worldData = await WorldGenerator.GenerateWorldAsync("Test", totalSizeX, totalSizeY);

        //SaveManager.SaveWorld(worldData);
        worldData = SaveManager.LoadWorld("Test");
    }
}
