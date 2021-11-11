using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [Range(0f, 1f)][SerializeField] private float finishCoefficient;
    [SerializeField] private float distanceToWin;
    [SerializeField] private ParticleSystem[] winParticles;
    private Collider collider;

    public event Action<bool> OnChangedWin;
    public List<Collider> winColliders = new List<Collider>();


    private void Awake()
    {
        collider = GetComponent<Collider>();
        winColliders.Clear();
    }

    public void PlayWinParticle()
    {
        foreach (var particle in winParticles)
        {
            particle.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            var percentage = BoundsContainedPercentage(other.bounds, collider.bounds) > finishCoefficient;
            var bounds = ContainBounds(collider.bounds, other.bounds);
            var distance = Vector3.Distance(collider.transform.position, other.transform.position) < distanceToWin;
            var isWin = percentage && bounds && distance;
            
            if (isWin && !winColliders.Contains(other))
            {
                OnChangedWin?.Invoke(true);
                winColliders.Add(other);
                Debug.Log($"{other.gameObject.name} enter win trigger");
            }

            if (!isWin && winColliders.Contains(other))
            {
                OnChangedWin?.Invoke(false);
                winColliders.Remove(other);
                Debug.Log($"{other.gameObject.name} leave win trigger");
            }
            else
            {
                Debug.Log($"Perce: {percentage}, Bounds: {bounds}, Distance: {distance}");
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
