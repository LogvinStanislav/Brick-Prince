using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlockPool blockPool;
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float spawnHeightAbovePlayer = 8f;
    public AudioClip destroySound;

    [Header("Controls")]
    [SerializeField] private KeyCode spawnKey = KeyCode.Q;
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode fastFallKey = KeyCode.DownArrow;
    [SerializeField] private KeyCode rotateKey = KeyCode.UpArrow;



    private FallingBlock currentBlock;
    private bool hasActiveBlock = false;

    void Start()
    {
        if (blockPool == null)
        {
            blockPool = GetComponent<BlockPool>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(spawnKey) && !hasActiveBlock)
        {
            SpawnBlock();
        }

        if (hasActiveBlock && currentBlock != null)
        {
            float horizontal = 0f;

            if (Input.GetKey(moveLeftKey))
                horizontal = -1f;
            else if (Input.GetKey(moveRightKey))
                horizontal = 1f;

            bool fastFall = Input.GetKey(fastFallKey);

            currentBlock.UpdateMovement(horizontal, fastFall);

            if (Input.GetKeyDown(rotateKey))
            {
                currentBlock.Rotate90();
            }
        }
    }


    void SpawnBlock()
    {
        GameObject prefab = blockPool.GetRandomBlock();
        if (prefab == null)
        {
            Debug.LogWarning("No blocks available in pool!");
            return;
        }

        // Спавн над игроком
        Vector3 spawnPos = player != null 
            ? new Vector3(player.position.x, player.position.y + spawnHeightAbovePlayer, 0)
            : new Vector3(0, spawnHeightAbovePlayer, 0);

        GameObject blockObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        currentBlock = blockObj.GetComponent<FallingBlock>();

        if (currentBlock == null)
        {
            currentBlock = blockObj.AddComponent<FallingBlock>();
        }

        currentBlock.destroySound = destroySound;

        currentBlock.Initialize(this, fallSpeed, moveSpeed);
        hasActiveBlock = true;
    }

    public void OnBlockLanded()
    {
        hasActiveBlock = false;
        currentBlock = null;
    }

    // Методы для настройки параметров (для будущих артефактов)
    public void SetFallSpeed(float speed) => fallSpeed = speed;
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
}