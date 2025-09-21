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

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public void Initialise(Material mat, Vector3 pos) // called to set up the container
    {
        ConfigComponents();
        meshRenderer.sharedMaterial = mat;
        containerPosition = pos;
    }
    
    public void ConfigComponents() // called to get the components
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void GenerateMesh()
    {

    }

    public void UploadMesh()
    {

    }

    // this section is for storing mesh data
    #region Mesh Data
    public struct MeshData
    {
        public Mesh mesh;
        // using arrays for better performance over lists
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        public bool Initialised;

        public void ClearData() // called to clear or set up the arrays
        {
            if (!Initialised)
            {
                vertices = new Vector3[24];
                triangles = new int[36];
                uvs = new Vector2[24];

                Initialised = true;
                mesh = new Mesh();
            }
            else
            {
                Array.Clear(vertices, 0, vertices.Length);
                Array.Clear(triangles, 0, triangles.Length);
                Array.Clear(uvs, 0, uvs.Length);
                mesh.Clear();
            }
        }

        public void UploadMesh(bool sharedVertices = false) // called to upload the mesh data to the mesh
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetUVs(0, uvs);

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

    static readonly int[,] voxelVertexIndex = new int[6, 4] // 6 faces, 4 vertices per face
    {
        {0, 1, 2, 3}, 
        {4, 5, 6, 7}, 
        {4, 0, 6, 2}, 
        {5, 1, 7, 3}, 
        {0, 1, 4, 5}, 
        {2, 3, 6, 7}  
    };

    static readonly Vector2[] voxelUVs = new Vector2[4] // 4 UVs per face
    {
        new Vector2(0.0f, 0.0f), 
        new Vector2(0.0f, 0.1f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(1.0f, 1.0f)  
    };

    static readonly int[,] voxelTris = new int[6, 6] // 6 faces, 2 triangles per face, 3 vertices per triangle
    {
        {0, 2, 3, 0, 3, 1}, 
        {0, 1, 2, 1, 3, 2}, 
        {0, 2, 3, 0, 3, 1},
        {0, 1, 2, 1, 3, 2},
        {0, 1, 2, 1, 3, 2},
        {0, 2, 3, 0, 3, 1}
    };
    #endregion
}

