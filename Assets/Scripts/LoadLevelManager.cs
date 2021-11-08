using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class LoadLevelManager : ScriptableObject
{
    private const string LEVEL = "level"; 
    
    [SerializeField] private int totalLevels;

    public int CurrentLevelNumber
    {
        get => PlayerPrefs.GetInt(LEVEL, 1);
        set => PlayerPrefs.SetInt(LEVEL, value);
    }

    public void LoadCurrentLevel()
    {
        SceneManager.LoadScene($"Level {CurrentLevelNumber}");
    }

    public void LoadNextLevel()
    {
        CurrentLevelNumber = (CurrentLevelNumber + 1) % totalLevels;
        SceneManager.LoadScene($"Level {CurrentLevelNumber}");
    }
}
