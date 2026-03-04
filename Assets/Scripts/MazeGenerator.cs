using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour        
{
    public GameObject[] cubePrefabs1;
    public GameObject[] cubePrefabs2;
    public GameObject targetObject; // Target objeyi buraya ata
    
    [Header("Block Dimensions")]
    public int mazeWidth = 3;
    public int mazeHeight = 3;
    public int mazeDepth = 3;

    [Header("Difficulty")]
    public int removeRandomObjects = 5;

    private GameObject[] selectedCubePrefabs;
    private List<GameObject> mazeObjects = new List<GameObject>(); // Tüm objeleri tutmak için bir dizi

    void Start()
    {
        // GameManager'dan aktif seviyenin ayarlarını çek
        if (GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.GetCurrentLevel();
            float unusedTime; // Counter.cs tarafından kullanılacak
            GameManager.Instance.GetLevelSettings(currentLevel, out mazeWidth, out mazeHeight, out mazeDepth, out removeRandomObjects, out unusedTime);
        }

        // Rastgele bir dizi seç
        if (Random.Range(0, 2) == 0)
        {
            selectedCubePrefabs = cubePrefabs1;
        }
        else
        {
            selectedCubePrefabs = cubePrefabs2;
        }

        GenerateSolvableBlock();
        GetObjectCount();
    }

    void GenerateSolvableBlock()
    {
        GameObject target = null;
        if(targetObject != null)
        {
            target = Instantiate(targetObject, Vector3.zero, Quaternion.identity);
        }

        // 1. Izgarayı oluştur ve rastgele küpleri eksilt
        bool[,,] grid = new bool[mazeWidth, mazeHeight, mazeDepth];
        for (int x = 0; x < mazeWidth; x++)
            for (int y = 0; y < mazeHeight; y++)
                for (int z = 0; z < mazeDepth; z++)
                    grid[x, y, z] = true;

        int totalPeeled = 0;
        int maxToRemove = Mathf.Min(removeRandomObjects, mazeWidth * mazeHeight * mazeDepth - 1);
        while (totalPeeled < maxToRemove)
        {
            int rx = Random.Range(0, mazeWidth);
            int ry = Random.Range(0, mazeHeight);
            int rz = Random.Range(0, mazeDepth);
            if (grid[rx, ry, rz])
            {
                grid[rx, ry, rz] = false;
                totalPeeled++;
            }
        }

        // 2. Geriye Doğru Soyma Algoritması
        Vector3[,,] escapeDirections = new Vector3[mazeWidth, mazeHeight, mazeDepth];
        int remainingCubes = (mazeWidth * mazeHeight * mazeDepth) - totalPeeled;
        int failSafe = 10000;

        while (remainingCubes > 0 && failSafe > 0)
        {
            failSafe--;
            List<Vector3Int> candidates = new List<Vector3Int>();
            List<List<Vector3>> candidateDirs = new List<List<Vector3>>();

            // Kaçabilecek tüm küpleri bul
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    for (int z = 0; z < mazeDepth; z++)
                    {
                        if (grid[x, y, z])
                        {
                            List<Vector3> freeDirs = GetFreeDirections(grid, x, y, z);
                            if (freeDirs.Count > 0)
                            {
                                candidates.Add(new Vector3Int(x, y, z));
                                candidateDirs.Add(freeDirs);
                            }
                        }
                    }
                }
            }

            if (candidates.Count == 0)
            {
                Debug.LogWarning("Generation Deadlock! Remaining: " + remainingCubes);
                break; 
            }

            // Rastgele bir küp seç ve onu soy
            int rndIndex = Random.Range(0, candidates.Count);
            Vector3Int chosenCell = candidates[rndIndex];
            List<Vector3> dirs = candidateDirs[rndIndex];
            Vector3 chosenDir = dirs[Random.Range(0, dirs.Count)];

            escapeDirections[chosenCell.x, chosenCell.y, chosenCell.z] = chosenDir;
            grid[chosenCell.x, chosenCell.y, chosenCell.z] = false;
            remainingCubes--;
        }

        // 3. Hesaplanan yönlerle küpleri sahnede oluştur
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                for (int z = 0; z < mazeDepth; z++)
                {
                    Vector3 escDir = escapeDirections[x, y, z];
                    if (escDir != Vector3.zero)
                    {
                        int randomIndex = Random.Range(0, selectedCubePrefabs.Length);
                        GameObject prefab = selectedCubePrefabs[randomIndex];
                        
                        ObjectMovement mov = prefab.GetComponent<ObjectMovement>();
                        Vector3 localMoveDir = mov != null ? mov.moveDirection : Vector3.forward;
                        if(localMoveDir == Vector3.zero) localMoveDir = Vector3.forward;

                        // Küpün lokal yönünü (ok işareti) hesaplanan kaçış yönüne çevir
                        Quaternion rotation = Quaternion.FromToRotation(localMoveDir, escDir);
                        
                        // İsteğe bağlı rastgele çevirme: Ok yönünü bozmadan küpü sağa sola çevirebiliriz
                        // Quaternion twist = Quaternion.AngleAxis(Random.Range(0, 4) * 90f, escDir);
                        // rotation = twist * rotation;

                        GameObject cube = Instantiate(prefab, new Vector3(x, y, z), rotation);
                        if (target != null) cube.transform.parent = target.transform;
                        else cube.transform.parent = this.transform;

                        mazeObjects.Add(cube);
                    }
                }
            }
        }
    }

    // Bir pozisyondan dışarı çıkabilen yönlerin (önünde engel olmayanların) listesini döner
    List<Vector3> GetFreeDirections(bool[,,] grid, int cx, int cy, int cz)
    {
        List<Vector3> freeDirs = new List<Vector3>();
        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.right, Vector3Int.left,
            Vector3Int.up, Vector3Int.down,
            new Vector3Int(0, 0, 1), // forward
            new Vector3Int(0, 0, -1) // back
        };

        foreach (var dir in directions)
        {
            bool isFree = true;
            int nx = cx + dir.x;
            int ny = cy + dir.y;
            int nz = cz + dir.z;
            
            while (nx >= 0 && nx < mazeWidth && ny >= 0 && ny < mazeHeight && nz >= 0 && nz < mazeDepth)
            {
                if (grid[nx, ny, nz])
                {
                    isFree = false; // Önünde başka bir küp var
                    break;
                }
                nx += dir.x;
                ny += dir.y;
                nz += dir.z;
            }
            
            if (isFree) freeDirs.Add(new Vector3((float)dir.x, (float)dir.y, (float)dir.z));
        }
        return freeDirs;
    }

    public int GetObjectCount()
    {
        Debug.Log("Block Object Count: " + mazeObjects.Count);
        return mazeObjects.Count;
    }

    public GameObject[] GetAllObjectsInMaze()
    {
        return mazeObjects.ToArray();
    }
}
