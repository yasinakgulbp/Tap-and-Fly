using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Variables
    // Game Settings
    private int currentLevel = 1; // Last played level
    private float secondsPerCube = 1.0f; // Time per cube (seconds)
    private int maxFailedMoves = 3; // Maximum number of failed moves

    // PlayerPrefs Keys
    private const string lastPlayedLevelKey = "LastPlayedLevel";
    private const string secondsPerCubeKey = "SecondsPerCube";
    private const string maxFailedMovesKey = "MaxFailedMoves";
    #endregion

    #region Unity Callbacks

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void OnDestroy()
    {
        SaveSettings();
    }
    #endregion

    #region Methods
    void Start()
    {
        // Load game settings from PlayerPrefs
        LoadSettings();
    }

    private void LoadSettings()
    {
        currentLevel = PlayerPrefs.GetInt(lastPlayedLevelKey, 1);
        secondsPerCube = PlayerPrefs.GetFloat(secondsPerCubeKey, 1.0f);
        maxFailedMoves = PlayerPrefs.GetInt(maxFailedMovesKey, 3);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(lastPlayedLevelKey, currentLevel);
        PlayerPrefs.SetFloat(secondsPerCubeKey, secondsPerCube);
        PlayerPrefs.SetInt(maxFailedMovesKey, maxFailedMoves);
        PlayerPrefs.Save(); // Save immediately
    }

    public void SetSecondsPerCube(float seconds)
    {
        secondsPerCube = seconds;
        SaveSettings();
    }

    public void SetMaxFailedMoves(int maxMoves)
    {
        maxFailedMoves = maxMoves;
        SaveSettings();
    }

    public void IncreaseMaxFailedMoves()
    {
        maxFailedMoves++;
        SaveSettings();
    }

    public void ResetMaxFailedMoves()
    {
        maxFailedMoves = 0;
        SaveSettings();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
        SaveSettings();
    }

    public float GetSecondsPerCube()
    {
        return secondsPerCube;
    }

    public int GetMaxFailedMoves()
    {
        return maxFailedMoves;
    }

    public void GetLevelSettings(int level, out int width, out int height, out int depth, out int removeCount, out float timePerCube)
    {
        // Zorluk eğrisi: her 12 seviyede bir grid boyutu 1 artar. (2x2x2'den başlar)
        int size = 2 + (level / 12); 
        if (size > 5) size = 5; // Maksimum 5x5x5
        
        if (level <= 1) size = 2; // Level 1 her zaman 2x2x2

        width = size;
        height = size;
        depth = size;

        removeCount = size * 2; // Kaldırılacak küp sayısı
        
        // Zaman hesaplaması: Seviye arttıkça her küp için ayrılan saniye azalır.
        timePerCube = Mathf.Max(0.5f, 2.0f - (level * 0.03f));
    }
    #endregion
}
