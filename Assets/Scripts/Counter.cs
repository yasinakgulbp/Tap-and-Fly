using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Counter : MonoBehaviour
{
    private int failedMoves = 0;
    private int remainingObjects = 0;
    public int maxFailedMoves = 3; // Hata yapma limiti
    public float secondsPerCube = 1f; // Her küp için 1 saniye

    // TMPro text nesneleri için referanslar
    public TMP_Text failedMovesText;
    public TMP_Text remainingObjectsText;
    public TMP_Text timerText;
    public TMP_Text levelText; // Oyun ekranında kaçıncı levelde olduğumuzu gösterecek metin

    private float currentTime;

    private void Start()
    {
        currentTime = GetInitialTime();
        // Başlangıçta bir gecikme ekleyerek obje sayısını al
        Invoke("GetRemainingObjects", 0.2f);

        // UI'da level yazısını güncelle
        if (levelText != null && GameManager.Instance != null)
        {
            levelText.text = "Level: " + GameManager.Instance.GetCurrentLevel().ToString();
        }
    }

    private float GetInitialTime()
    {
        MazeGenerator mazeGenerator = Object.FindFirstObjectByType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            remainingObjects = mazeGenerator.GetObjectCount();
            
            if (GameManager.Instance != null)
            {
                int level = GameManager.Instance.GetCurrentLevel();
                int dummy; 
                GameManager.Instance.GetLevelSettings(level, out dummy, out dummy, out dummy, out dummy, out secondsPerCube);
            }

            return remainingObjects * secondsPerCube;
        }
        else
        {
            Debug.LogError("MazeGenerator not found in the scene!");
            return 0f;
        }
    }

    private void GetRemainingObjects()
    {
        // MazeGenerator scriptine erişimi al
        MazeGenerator mazeGenerator = Object.FindFirstObjectByType<MazeGenerator>();
        if (mazeGenerator != null)
        {
            // MazeGenerator'dan obje sayısını al ve kalan obje sayısını güncelle
            remainingObjects = mazeGenerator.GetObjectCount();
            // Başlangıçta metinleri güncelle
            UpdateTexts();
        }
        else
        {
            Debug.LogError("MazeGenerator not found in the scene!");
        }
    }

    public void IncrementSuccessfulMoves()
    {
        // Kalan obje sayısını azalt
        remainingObjects--;
        // Başarılı hareket sayısını güncelle ve metni yenile
        UpdateTexts();

        // Oyun bitti mi kontrol et
        CheckGameEnd();
    }

    public void IncrementFailedMoves()
    {
        failedMoves++;
        // Başarısız hareket sayısını güncelle ve metni yenile
        UpdateTexts();

        // Hata yapma limitini kontrol et
        CheckFailureLimit();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        timerText.text = currentTime.ToString("F0");

        if (currentTime <= 0)
        {
            // Süre bittiğinde oyunu yeniden başlat
            RestartScene();
        }

        // Geri sayımın son 5 saniyesinde uyarı mesajı göster
        if (currentTime <= 5f)
        {
             timerText.color = Color.red;
        }
    }

    // TextMesh Pro nesnelerini güncelleyen yardımcı fonksiyon
    private void UpdateTexts()
    {
        if (failedMovesText != null)
        {
            failedMovesText.text = (maxFailedMoves - failedMoves).ToString(); // Kalan hak sayısını göster
        }

        if (remainingObjectsText != null)
        {
            remainingObjectsText.text = remainingObjects.ToString();
        }
    }

    // Oyunun bitip bitmediğini kontrol eden fonksiyon
    private void CheckGameEnd()
    {
        if (remainingObjects <= 0)
        {
            Debug.Log("Zafer!"); // Zafer durumunu konsola yazdır
            // Bir sonraki seviyeye geç
            GoToNextLevel();
        }
    }

    // Hata yapma limitini kontrol eden fonksiyon
    private void CheckFailureLimit()
    {
        if (maxFailedMoves - failedMoves == 0) // Kalan hak sıfıra eşit ise
        {
            Debug.Log("Malubiyet!"); // Malubiyet durumunu konsola yazdır
                                      // Sahneyi yeniden başlat
            RestartScene();
        }
    }

    // Sahneyi yeniden başlatan fonksiyon
    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Bir sonraki seviyeye geçen fonksiyon
    private void GoToNextLevel()
    {
        if (GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.GetCurrentLevel();
            GameManager.Instance.SetCurrentLevel(currentLevel + 1);
            Debug.Log("Yeni Seviyeye Geciliyor: " + (currentLevel + 1));
        }

        RestartScene();
    }
}
