using UnityEngine;

public class WinController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [Range(0f, 1f)][SerializeField] private float finishCoefficient;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            var percentage = BoundsContainedPercentage(other.bounds, collider.bounds);
            if (percentage > finishCoefficient)
            {
                levelController.WinLevel();
            }
        }
    }

    private float BoundsContainedPercentage(Bounds obj, Bounds region)
    {
        var total = 1f;
 
        for ( var i = 0; i < 3; i++ )
        {
            var dist = obj.min[i] > region.center[i] ?
                obj.max[i] - region.max[i] :
                region.min[i] - obj.min[i];
 
            total *= Mathf.Clamp01(1f - dist / obj.size[i]);
        }
 
        return total;
    }
}
