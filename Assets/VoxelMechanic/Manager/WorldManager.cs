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
    [SerializeField] private int chunkX = 4;
    [SerializeField] private int chunkY = 8;
    [SerializeField] private int chunkZ = 4;
    public int chunkSize = 16;

    //perlin noise variables
    private float noiseScale = 0.1f;
    private float heightMultiplier = 6f;
    private float noiseOffset = 100f;

    public bool boxWorld; // toggle between box world and smooth world

    public Dictionary<Vector2Int, Container> chunks; // Dictionary to hold chunks with their positions as keys

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_EDITOR
    if (!Application.isPlaying)
        return; // safe default during editor reload
#endif
        chunks = new Dictionary<Vector2Int, Container>();
        // generate chunks
        for (int cx = 0; cx < chunkX; cx++)
        {
            for (int cz = 0; cz < chunkZ; cz++)
            {
                Vector2Int chunkPos = new Vector2Int(cx, cz);
                GameObject cont = new GameObject("Chunk_" + cx + "_" + cz);
                cont.transform.parent = transform;
                cont.transform.position = new Vector3(cx * chunkSize, 0, cz * chunkSize);
                Container container = cont.AddComponent<Container>();
                container.Initialise(worldMaterial, cont.transform.position);

                // Add some voxels to the chunk
                for (int x = 0; x < chunkSize; x++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        // Calculate Perlin noise-based height
                        float xCoord = (cx * chunkSize + x) * noiseScale + noiseOffset;
                        float zCoord = (cz * chunkSize + z) * noiseScale + noiseOffset;
                        float flatness = 0.4f;
                        float perlinValue = Mathf.PerlinNoise(xCoord, zCoord);
                        if (perlinValue < flatness) perlinValue = flatness; // Apply flatness threshold
                        int height = Mathf.FloorToInt(perlinValue * heightMultiplier);

                        // Clamp height to chunkY
                        height = Mathf.Clamp(height, 1, chunkY);
                        for (int y = 0; y < height; y++)
                        {
                            container[new Vector3(x, y, z)] = new Voxel() { ID = 1 };
                        }
                    }
                }
                if (boxWorld)
                {
                    container.GenerateMesh();
                    container.UploadMesh();
                }
                chunks.Add(chunkPos, container);// Add chunk to dictionary
            }
        }
        if (!boxWorld)
        {
            // Generate and upload meshes for all chunks
            foreach (var chunk in chunks.Values)
            {
                chunk.GenerateMeshMarchingCubes(chunkSize, chunkY, chunkSize);// Generate mesh for each chunk
                chunk.UploadMesh();
            }
        }
    }

    public static WorldManager Instance { get; private set; }// Singleton instance that can be accessed from other scripts but not modified
    
    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;  
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
