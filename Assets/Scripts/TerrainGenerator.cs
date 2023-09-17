using System;
using UnityEngine;

public static class TerrainGenerator
{
    public static int[,,] Build(int xOffset, int yOffset)
    {
        var result = new int[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkHeight];

        for (var x = 0; x < ChunkRenderer.ChunkWidth; ++x)
        {
            for (var z = 0; z < ChunkRenderer.ChunkWidth; ++z)
            {
                var height = Mathf.PerlinNoise((x + xOffset) * 0.2f, (z + yOffset) * 0.2f) * 5 + 10;
                
                for (var y = 0; y < height; ++y)
                {
                    result[x, y, z] = 1;
                }
            }
        }

        return result;
    }
}
