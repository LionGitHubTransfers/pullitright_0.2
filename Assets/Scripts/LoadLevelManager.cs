using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class LoadLevelManager : ScriptableObject
{
    private const string LEVEL = "level";
    private const string VIBRATION = "vibration";
    public int CurrentAttempt = 0;
    
    [SerializeField] private int totalLevels;
    [SerializeField] private int tutorialLevels;

    public int CurrentLevelNumber
    {
        get => PlayerPrefs.GetInt(LEVEL, 1);
        set => PlayerPrefs.SetInt(LEVEL, value);
    }

    public bool IsVibration
    {
        get => PlayerPrefs.GetInt(VIBRATION, 1) == 1;
        set => PlayerPrefs.SetInt(VIBRATION, value ? 1 : 0);
    }

    private int GetLevelToLoad(int number)
    {
        if (number <= totalLevels)
        {
            return number;
        }

        return tutorialLevels + 1 + (number - totalLevels - 1) % 
            (totalLevels - tutorialLevels);
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene($"Level {GetLevelToLoad(CurrentLevelNumber)}");
    }

    public void LoadNextLevel()
    {
        CurrentLevelNumber++;
        SceneManager.LoadScene($"Level {GetLevelToLoad(CurrentLevelNumber)}");
    }
}
