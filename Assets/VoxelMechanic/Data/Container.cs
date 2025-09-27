using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// following code adds componenets every time you add this file to an instantiated object
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
    public Vector3 containerPosition;

    private Dictionary<Vector3, Voxel> data;
    private MeshData meshData = new MeshData();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public void Initialise(Material mat, Vector3 pos) // called to set up the container
    {
        ConfigComponents();
        data = new Dictionary<Vector3, Voxel>();
        meshRenderer.sharedMaterial = mat;
        containerPosition = pos;
    }

    public void ClearData() // called to clear the voxel data
    {
        data.Clear();
    }   

    public void ConfigComponents() // called to get the components
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void GenerateMesh() // called to generate the mesh data from the voxel data
    {
        meshData.ClearData();

        Vector3 blockPos = new Vector3(0,0,0);
        Voxel block = new Voxel() { ID = 1 };

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUvs = new Vector2[4];

        VoxelColour voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        foreach (KeyValuePair<Vector3, Voxel> item in data) // iterate through each voxel in the container
        {
            if (item.Value.ID == 0) { continue; } // skip empty voxels
            blockPos = item.Key;
            block = item.Value;

            voxelColor = WorldManager.Instance.worldColors[block.ID - 1];
            voxelColorAlpha = voxelColor.color;
            voxelColorAlpha.a = 1;
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

            //iterate through each face of the voxel
            for (int f = 0; f < 6; f++)
            {
                if (this[blockPos + voxelFaceChecks[f]].isSolid) { continue; } // skip faces that are solid as dont need to draw it

                //iterate through each vertex of the face
                for (int v = 0; v < 4; v++)
                {
                    faceVertices[v] = voxelVertices[voxelVertexIndex[f, v]] + blockPos;
                    faceUvs[v] = voxelUVs[v];
                }
                //add the face vertices and uvs to the mesh data
                int vertIndex = meshData.vertices.Count;
                for (int i = 0; i < 4; i++)
                {
                    meshData.vertices.Add(faceVertices[i]);
                    meshData.uvs.Add(faceUvs[i]);

                    meshData.colors.Add(voxelColorAlpha);
                    meshData.UVs2.Add(voxelSmoothness);
                }
                //add the face triangles to the mesh data
                for (int t = 0; t < 6; t++)
                {
                    meshData.triangles.Add(vertIndex + voxelTris[f, t]);
                }
                counter++;
            }
        }
    }

    public void UploadMesh() // called to upload the mesh data to the mesh
    {
        meshData.UploadMesh();

        if (meshRenderer == null) { ConfigComponents(); } // in case this is called before config components

        meshFilter.mesh = meshData.mesh;
        if (meshData.vertices.Count > 3) { meshCollider.sharedMesh = meshData.mesh; } // only set the collider if there are enough vertices
    }

    public Voxel this[Vector3 index] // indexer to get or set voxel data
    {
        get
        {
            if (data.ContainsKey(index))
            {
                return data[index];
            }
            else
            {
                return emptyVoxel;
            }
        }
        set
        {
            if (data.ContainsKey(index))
            {
                data[index] = value;
            }
            else
            {
                data.Add(index, value);
            }
        }
    }

    public static Voxel emptyVoxel = new Voxel() { ID = 0 }; // static empty voxel for easy access

    public ChunkData GetChunkData(Vector2Int chunkPos)
    {
        var data = new ChunkData { chunkPos = chunkPos, voxels = new List<VoxelData>() };
        foreach (var kvp in this.data) // Assuming you store voxels in a dictionary
        {
            data.voxels.Add(new VoxelData { position = kvp.Key, id = kvp.Value.ID });
        }
        return data;
    }

    public void LoadChunkData(ChunkData data)
    {
        foreach (var v in data.voxels)
        {
            this[v.position] = new Voxel { ID = v.id };
        }
    }

    // this section is for storing mesh data
    #region Mesh Data
    public struct MeshData
    {
        public Mesh mesh;
        // using arrays for better performance over lists
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;
        public List<Vector2> UVs2;
        public List<Color> colors;

        public bool Initialised;

        public void ClearData() // called to clear or set up the arrays
        {
            if (!Initialised)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                uvs = new List<Vector2>();
                UVs2 = new List<Vector2>();
                colors = new List<Color>();

                Initialised = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                uvs.Clear();
                UVs2.Clear();
                colors.Clear();
                mesh.Clear();
            }
        }

        public void UploadMesh(bool sharedVertices = false) // called to upload the mesh data to the mesh
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);
            
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(2, UVs2);

            mesh.Optimize();

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
    #endregion
    // this section is for voxel statics, readonly stops them from being changed
    #region Voxel Statics
    static readonly Vector3[] voxelVertices = new Vector3[8] // 8 vertices in a cube
    {
        new Vector3(0.0f, 0.0f, 0.0f), // 0
        new Vector3(1.0f, 0.0f, 0.0f), // 1
        new Vector3(1.0f, 1.0f, 0.0f), // 2
        new Vector3(0.0f, 1.0f, 0.0f), // 3

        new Vector3(0.0f, 1.0f, 1.0f), // 4
        new Vector3(1.0f, 1.0f, 1.0f), // 5
        new Vector3(1.0f, 0.0f, 1.0f), // 6
        new Vector3(0.0f, 0.0f, 1.0f)  // 7
    };

    static readonly Vector3[] voxelFaceChecks = new Vector3[6] // 6 faces in a cube
    {
        new Vector3(0.0f, 0.0f, -1.0f), // Front
        new Vector3(0.0f, 0.0f, 1.0f),  // Back
        new Vector3(0.0f, 1.0f, 0.0f),  // Top
        new Vector3(0.0f, -1.0f, 0.0f), // Bottom
        new Vector3(-1.0f, 0.0f, 0.0f), // Left
        new Vector3(1.0f, 0.0f, 0.0f)   // Right  
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4] // 6 faces, 4 vertices per face
    {
        {0, 1, 2, 3}, // Front
        {7, 6, 5, 4}, // Back
        {3, 2, 5, 4}, // Top
        {0, 1, 6, 7}, // Bottom
        {0, 3, 4, 7}, // Left
        {1, 2, 5, 6}  // Right  
    };

    static readonly Vector2[] voxelUVs = new Vector2[4] // 4 UVs per face
    {
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(1.0f, 1.0f), 
        new Vector2(0.0f, 1.0f)  
    };

    static readonly int[,] voxelTris = new int[6, 6] // 6 faces, 2 triangles per face, 3 vertices per triangle
    {
        {0, 2, 1, 0, 3, 2}, // Front  
        {0, 1, 2, 0, 2, 3}, // Back   
        {0, 2, 1, 0, 3, 2}, // Top    
        {0, 1, 2, 0, 2, 3}, // Bottom 
        {0, 2, 1, 0, 3, 2}, // Left   
        {0, 1, 2, 0, 2, 3}  // Right  
    };
    #endregion
}

