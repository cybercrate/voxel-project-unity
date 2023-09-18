using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    
    [FormerlySerializedAs("ChunkPrefab")]
    public ChunkRenderer chunkPrefab;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
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

                var chunk = Instantiate(chunkPrefab, new Vector3(xPosition, 0, yPosition), Quaternion.identity, transform);
                chunk.ChunkData = chunkData;
                chunk.parentWorld = this;
                
                chunkData.Renderer = chunk;
            }
        }
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) is false)
        {
            return;
        }

        var isDestroying = Input.GetMouseButtonDown(0);
        var ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out var hitInfo) is false)
        {
            return;
        }

        Vector3 blockCenter;

        if (isDestroying)
        {
            blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2;
        }
        else
        {
            blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
        }
        
        var blockWorldPosition = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
        var chunkPosition = GetChunkContainingBlock(blockWorldPosition);

        if (ChunkDatas.TryGetValue(chunkPosition, out var chunkData) is false)
        {
            return;
        }
        
        var chunkOrigin = new Vector3Int(chunkPosition.x, 0, chunkPosition.y) * ChunkRenderer.ChunkWidth;

        if (isDestroying)
        {
            chunkData.Renderer.DestroyBlock(blockWorldPosition - chunkOrigin);
        }
        else
        {
            chunkData.Renderer.SpawnBlock(blockWorldPosition - chunkOrigin, BlockType.Grass);
        }
    }

    private static Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPosition)
    {
        return new Vector2Int(
            blockWorldPosition.x / ChunkRenderer.ChunkWidth,
            blockWorldPosition.z / ChunkRenderer.ChunkWidth);
    }
}
