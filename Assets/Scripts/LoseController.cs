using UnityEngine;

public class LoseController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [SerializeField] private string tagToFind;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToFind))
        {
            levelController.LoseLevel();
        }
    }
}
