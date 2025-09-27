using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkData
{
    public Vector2Int chunkPos;
    public List<VoxelData> voxels;
}

[Serializable]
public class VoxelData
{
    public Vector3 position;
    public byte id;
}
