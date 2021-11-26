using UnityEngine;

public class ChangeVibrationButton : MonoBehaviour
{
    [SerializeField] private GameObject vibrationOnObject;
    [SerializeField] private GameObject vibrationOffObject;
    [SerializeField] private LoadLevelManager levelManager;
    private void Start()
    {
        if (levelManager.IsVibration)
        {
            vibrationOnObject.SetActive(true);
            vibrationOffObject.SetActive(false);
        }
        else
        {
            vibrationOnObject.SetActive(false);
            vibrationOffObject.SetActive(true);
        }
    }

    public void ChangeVibration()
    {
        if (levelManager.IsVibration)
        {
            levelManager.IsVibration = false;
            levelManager.IsVibration = false;
            vibrationOnObject.SetActive(false);
            vibrationOffObject.SetActive(true);
        }
        else
        {
            levelManager.IsVibration = true;
            levelManager.IsVibration = true;
            vibrationOnObject.SetActive(true);
            vibrationOffObject.SetActive(false);
        }
    }
}
