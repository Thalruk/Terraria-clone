using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera mainCam;
    [Header("Settings")]
    [SerializeField] private float mineCooldown = 0.2f;
    [SerializeField] private float buildCooldown = 0.2f;

    private float nextMineTime = 0f;
    private float nextBuildTime = 0f;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextMineTime)
        {
            MineBlock();
            nextMineTime = Time.time + mineCooldown;
        }

        if (Input.GetMouseButton(1) && Time.time >= nextBuildTime)
        {
            BuildBlock();
            nextBuildTime = Time.time + buildCooldown;
        }
    }
    private void MineBlock()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        int gridX = Mathf.FloorToInt(mouseWorldPos.x);
        int gridY = Mathf.FloorToInt(mouseWorldPos.y);

        WorldManager.Instance.DamageTile(gridX, gridY, 1);
    }

    private void BuildBlock()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        int gridX = Mathf.FloorToInt(mouseWorldPos.x);
        int gridY = Mathf.FloorToInt(mouseWorldPos.y);

        if (gridX < 0 || gridX >= WorldManager.Instance.worldData.sizeX ||
            gridY < 0 || gridY >= WorldManager.Instance.worldData.sizeY)
            return;

        TileType currentType = WorldManager.Instance.worldData.tiles[gridX, gridY].type;

        if (currentType == TileType.Air)
        {
            WorldManager.Instance.SetTile(gridX, gridY, TileType.Dirt);
        }
    }
}
