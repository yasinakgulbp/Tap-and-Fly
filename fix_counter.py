import codecs
import re

filepath = r'd:\GitHub\Tap-and-Fly\Assets\Scripts\Counter.cs'

with open(filepath, 'rb') as f:
    raw_bytes = f.read()
    
# Decode ignoring errors to skip the broken Turkish chars
text = raw_bytes.decode('utf-8-sig', errors='ignore')

# 1. Update GetInitialTime
pattern1 = re.compile(r'private float GetInitialTime\(\).*?return 0f;\s*\}', re.DOTALL)
replacement1 = """private float GetInitialTime()
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
    }"""
text = pattern1.sub(replacement1, text)

# 2. Update GoToNextLevel
pattern2 = re.compile(r'private void GoToNextLevel\(\).*?\}\s*\}', re.DOTALL)
replacement2 = """private void GoToNextLevel()
    {
        if (GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.GetCurrentLevel();
            GameManager.Instance.SetCurrentLevel(currentLevel + 1);
            Debug.Log("Yeni Seviyeye Geciliyor: " + (currentLevel + 1));
        }

        RestartScene();
    }
}"""
text = pattern2.sub(replacement2, text)

with codecs.open(filepath, 'w', 'utf-8') as f:
    f.write(text)

print("SUCCESS")
