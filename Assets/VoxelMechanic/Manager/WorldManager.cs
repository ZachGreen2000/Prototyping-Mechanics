using UnityEngine;
using System.IO;
//using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;

    public VoxelColour[] worldColors;

    // chunk variables
    private int chunkX = 4;
    private int chunkY = 8;
    private int chunkZ = 4;

    // dictionairy to hold chunks
    private Dictionary<Vector2Int, Container> chunks = new Dictionary<Vector2Int, Container>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else 
        {
            _instance = this;
        }
        // generate chunks
        for (int cx = 0; cx < chunkX; cx++)
        {
            for (int cz = 0; cz < chunkZ; cz++)
            {
                Vector2Int chunkPos = new Vector2Int(cx, cz);
                GameObject cont = new GameObject("Chunk_" + cx + "_" + cz);
                cont.transform.parent = transform;
                Container container = cont.AddComponent<Container>();
                container.Initialise(worldMaterial, cont.transform.position);

                // Add some voxels to the chunk
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int randomHeight = UnityEngine.Random.Range(1, chunkY);
                        for (int y = 0; y < randomHeight; y++)
                        {
                            container[new Vector3(x, y, z)] = new Voxel() { ID = 1 };
                        }
                    }
                }

                container.GenerateMesh();
                container.UploadMesh();

                chunks.Add(chunkPos, container);

            }
        }    
    }

    private static WorldManager _instance; // Singleton instance
    public static WorldManager Instance // Public accessor
    { 
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WorldManager>();// Find existing instance in the scene 
            }
            return _instance; 
        }
    }
    /*
    // Save all chunks to JSON
    public void SaveWorld(string path)
    {
        var saveData = new List<ChunkData>();
        foreach (var kvp in chunks)
        {
            saveData.Add(kvp.Value.GetChunkData(kvp.Key));
        }
        //string json = JsonSerializer.Serialize(saveData);
        File.WriteAllText(path, json);
    }

    // Load all chunks from JSON
    public void LoadWorld(string path)
    {
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        //var saveData = JsonSerializer.Deserialize<List<ChunkData>>(json);

        // Clear existing chunks
        foreach (var chunk in chunks.Values)
            Destroy(chunk.gameObject);
        chunks.Clear();

        // Recreate chunks
        foreach (var chunkData in saveData)
        {
            GameObject cont = new GameObject("Chunk_" + chunkData.chunkPos.x + "_" + chunkData.chunkPos.y);
            cont.transform.parent = transform;
            Container newChunk = cont.AddComponent<Container>();
            newChunk.Initialise(worldMaterial, cont.transform.position);
            newChunk.LoadChunkData(chunkData);
            newChunk.GenerateMesh();
            newChunk.UploadMesh();
            chunks.Add(chunkData.chunkPos, newChunk);
        }
    }*/
}
