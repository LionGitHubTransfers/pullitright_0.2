using UnityEngine;

public class LoseController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            levelController.LoseLevel();
        }
    }
}
