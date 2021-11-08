using UnityEngine;

public class WinController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [Range(0f, 1f)][SerializeField] private float finishCoefficient;
    [SerializeField] private float distanceToWin;
    [SerializeField] private ParticleSystem[] winParticles;
    private Collider collider;

    private bool isWin = false;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Target") && !isWin)
        {
            var percentage = BoundsContainedPercentage(other.bounds, collider.bounds) > finishCoefficient;
            var bounds = ContainBounds(collider.bounds, other.bounds);
            var distance = Vector3.Distance(collider.transform.position, other.transform.position) < distanceToWin;
            Debug.Log($"Perce: {percentage}, Bounds: {bounds}, Distance: {distance}");
            if (percentage && bounds && distance)
            {
                levelController.WinLevel();
                isWin = true;
                foreach (var particle in winParticles)
                {
                    particle.Play();
                }
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
    
    private bool ContainBounds(Bounds bounds, Bounds target)
    {
        return bounds.Contains(target.min) && bounds.Contains(target.max);
    }
}
