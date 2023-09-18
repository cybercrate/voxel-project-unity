using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;

    private void Start()
    {
        const int size = 10;

        for (var x = 0; x < size; ++x)
        {
            for (var y = 0; y < size; ++y)
            {
                var xPosition = x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                var yPosition = y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                
                var chunkData = new ChunkData
                {
                    Blocks = TerrainGenerator.Build((int)xPosition, (int)yPosition),
                    ChunkPosition = new Vector2Int(x, y)
                };

                ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(ChunkPrefab, new Vector3(xPosition, 0, yPosition), Quaternion.identity, transform);
                chunk.ChunkData = chunkData;
                chunk.ParentWorld = this;
            }
        }
    }
}
