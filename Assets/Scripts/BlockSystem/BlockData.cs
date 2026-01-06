using UnityEngine;

[System.Serializable]
public class BlockData
{
    public string id;
    public GameObject prefab;
    [Range(0.1f, 10f)]
    public float spawnWeight = 1f;
    
    public BlockData(string blockId, GameObject blockPrefab, float weight = 1f)
    {
        id = blockId;
        prefab = blockPrefab;
        spawnWeight = weight;
    }
}
