using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    internal const int ChunkWidth = 16;
    internal const int ChunkHeight = 256;
    internal const float BlockScale = 1.0f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    private void Start()
    {
        var chunkMesh = new Mesh();

        for (var y = 0; y < ChunkHeight; ++y)
        {
            for (var x = 0; x < ChunkWidth; ++x)
            {
                for (var z = 0; z < ChunkWidth; ++z)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.vertices = _vertices.ToArray();
        chunkMesh.triangles = _triangles.ToArray();
        
        chunkMesh.Optimize();
        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);

        if (GetBlockAtPosition(blockPosition) is 0)
        {
            return;
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) is 0)
        {
            GenerateRightSide(blockPosition);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.left) is 0)
        {
            GenerateLeftSide(blockPosition);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) is 0)
        {
            GenerateFrontSide(blockPosition);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.back) is 0)
        {
            GenerateBackSide(blockPosition);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.up) is 0)
        {
            GenerateTopSide(blockPosition);
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.down) is 0)
        {
            GenerateBottomSide(blockPosition);
        }
    }

    private void AddLastVerticesSquare()
    {
        _triangles.Add(_vertices.Count - 4);
        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 2);
        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 1);
        _triangles.Add(_vertices.Count - 2);
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);

        AddLastVerticesSquare();
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x is >= 0 and < ChunkWidth && blockPosition.y is >= 0 and < ChunkHeight &&
            blockPosition.z is >= 0 and < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }

        if (blockPosition.y is < 0 or >= ChunkHeight)
        {
            return BlockType.Air;
        }

        var adjacentChunkPosition = ChunkData.ChunkPosition;

        switch (blockPosition.x)
        {
            case < 0:
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
                break;
            case >= ChunkWidth:
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
                break;
        }

        switch (blockPosition.z)
        {
            case < 0:
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
                break;
            case >= ChunkWidth:
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
                break;
        }

        return ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out var adjacentChunk)
            ? adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z]
            : BlockType.Air;
    }
}
