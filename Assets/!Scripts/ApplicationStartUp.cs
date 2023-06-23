using UnityEngine;

public class ApplicationStartUp : MonoBehaviour
{
    [SerializeField] private bool isDevelopMode;

    [Header("Configs")]
    public PlayerAccountConfig PlayerAccountConfig;
    public PlayerMoneyConfig PlayerMoneyConfig;
    public EnvironmentConfig EnvironmentConfig;
    public DebugConfig DebugConfig;
    public BlockSpawnConfig BlockSpawnConfig;
    public BuildingSpawnConfig BuildingSpawnConfig;
    
    private void Awake()
    {
        gameObject.AddComponent<Game>();
        gameObject.AddComponent<PlayerDataStorage>();
        gameObject.AddComponent<Serializer>();
        gameObject.AddComponent<DebugSettings>();
        gameObject.AddComponent<PlayerMoney>();
        gameObject.AddComponent<PlayerAccount>();
        gameObject.AddComponent<Environment>();
        gameObject.AddComponent<BlockStorageCreation>();
    }
}
