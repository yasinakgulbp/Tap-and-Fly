import codecs

filepath = r'd:\GitHub\Tap-and-Fly\Assets\Scripts\LevelButtonManager.cs'

content = """using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButtonManager : MonoBehaviour
{
    [Header("Level Generation Settings")]
    [Tooltip("Ornek bir Slot prefabini buraya surukle")]
    public GameObject buttonPrefab; 
    
    [Tooltip("Butonlarin eklenecegi UI Paneli")]
    public Transform contentPanel; 
    
    public int totalLevels = 50;

    [Header("Scene Target")]
    [Tooltip("Bolume tiklandiginda acilacak tek ana sahnenin adi")]
    public string gameSceneName = "1";

    void Start()
    {
        int lastPlayedLevel = 1;
        if (GameManager.Instance != null)
        {
            lastPlayedLevel = GameManager.Instance.GetCurrentLevel();
        }

        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, contentPanel);
            newButton.name = "Slot_Level_" + i;

            TMP_Text buttonTextTMP = newButton.GetComponentInChildren<TMP_Text>();
            if (buttonTextTMP != null)
            {
                buttonTextTMP.text = i.ToString();
            }
            else
            {
                Text buttonTextLegacy = newButton.GetComponentInChildren<Text>();
                if (buttonTextLegacy != null) buttonTextLegacy.text = i.ToString();
            }

            Button btn = newButton.GetComponent<Button>();
            if (btn != null)
            {
                if (i <= lastPlayedLevel)
                {
                    btn.interactable = true;
                    int levelIndex = i; // Lambda iin yerel kopyalama
                    btn.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
                }
                else
                {
                    btn.interactable = false;
                }
            }
        }
    }

    private void OnLevelButtonClicked(int levelIndex)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentLevel(levelIndex);
        }
        
        SceneManager.LoadScene(gameSceneName);
    }
}
"""

with codecs.open(filepath, 'w', 'utf-8-sig') as f:
    f.write(content)

print("SUCCESS")
