using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    [SerializeField] private List<BlockData> blocks = new List<BlockData>();
    
    public GameObject GetRandomBlock()
    {
        if (blocks.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var block in blocks)
        {
            totalWeight += block.spawnWeight;
        }

        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var block in blocks)
        {
            currentWeight += block.spawnWeight;
            if (random <= currentWeight)
            {
                return block.prefab;
            }
        }

        return blocks[0].prefab;
    }

    
    public void AddBlock(BlockData newBlock)
    {
        blocks.Add(newBlock);
    }

    public void RemoveBlock(string blockId)
    {
        blocks.RemoveAll(b => b.id == blockId);
    }

    public void SetBlockWeight(string blockId, float newWeight)
    {
        BlockData block = blocks.Find(b => b.id == blockId);
        if (block != null)
        {
            block.spawnWeight = Mathf.Max(0.1f, newWeight);
        }
    }

    public BlockData GetBlock(string blockId)
    {
        return blocks.Find(b => b.id == blockId);
    }

    public List<BlockData> GetAllBlocks()
    {
        return new List<BlockData>(blocks);
    }
}
