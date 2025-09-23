using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;

    public VoxelColour[] worldColors;
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

        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();
        
        container.Initialise(worldMaterial, Vector3.zero);
        // Add some voxels to the container
        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                int randomHeight = UnityEngine.Random.Range(1, 16);
                for (int y = 0; y < randomHeight; y++)
                {
                    container[new Vector3(x, y, z)] = new Voxel() { ID = 1 };
                }
            }
        }

        container.GenerateMesh();
        container.UploadMesh();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
