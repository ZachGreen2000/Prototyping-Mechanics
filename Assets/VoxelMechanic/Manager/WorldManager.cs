using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
