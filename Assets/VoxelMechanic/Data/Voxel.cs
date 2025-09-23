using UnityEngine;

public struct Voxel
{
    public byte ID;

    public bool isSolid
    {
        get
        {
            return ID != 0; // assuming ID 0 is air/empty
        }
    }
}
