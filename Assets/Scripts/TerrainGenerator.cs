using System;
using UnityEngine;

public static class TerrainGenerator
{
    public static BlockType[,,] Build(int xOffset, int yOffset)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkHeight];

        for (var x = 0; x < ChunkRenderer.ChunkWidth; ++x)
        {
            for (var z = 0; z < ChunkRenderer.ChunkWidth; ++z)
            {
                var height = Mathf.PerlinNoise((x + xOffset) * 0.2f, (z + yOffset) * 0.2f) * 5 + 10;
                
                for (var y = 0; y < height; ++y)
                {
                    result[x, y, z] = BlockType.Grass;
                }
            }
        }

        return result;
    }
}
