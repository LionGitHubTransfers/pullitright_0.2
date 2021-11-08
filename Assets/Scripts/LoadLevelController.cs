using UnityEngine;

public class LoadLevelController : MonoBehaviour
{
    [SerializeField] private LoadLevelManager levelManager;
    
    private void Awake()
    {
        levelManager.LoadCurrentLevel();
    }
}