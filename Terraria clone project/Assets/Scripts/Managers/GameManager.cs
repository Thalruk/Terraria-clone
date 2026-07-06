using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerObject;
    [SerializeField] Cinemachine.CinemachineVirtualCamera CinemachineVirtualCamera;

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

    private async void Start()
    {
        await WorldManager.Instance.Generate();
        playerObject = Instantiate(playerPrefab, WorldManager.Instance.worldData.spawnPoint, Quaternion.identity, transform);
        CinemachineVirtualCamera.Follow = playerObject.transform;
        CinemachineVirtualCamera.LookAt = playerObject.transform;
        ChunkManager.Instance.SetWatchedObject(playerObject.transform);
    }

}
