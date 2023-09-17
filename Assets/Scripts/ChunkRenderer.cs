using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    internal const int ChunkWidth = 16;
    internal const int ChunkHeight = 256;

    private int[,,] _blocks = new int[ChunkWidth, ChunkHeight, ChunkWidth];

    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();

    private void Start()
    {
        var chunkMesh = new Mesh();
        var position = transform.position;
        
        _blocks = TerrainGenerator.Build((int)position.x, (int)position.z);

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

        chunkMesh.RecalculateBounds();
        chunkMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = chunkMesh;
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
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);

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

    private int GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x is >= 0 and < ChunkWidth && blockPosition.y is >= 0 and < ChunkHeight &&
            blockPosition.z is >= 0 and < ChunkWidth)
        {
            return _blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }

        return 0;
    }
}
