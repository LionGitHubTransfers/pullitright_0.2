using UnityEngine;

public class SwitchToRagdoll : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Animator animator;
    [SerializeField] private float deadImpulse = 5f;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Target"))
        {
            foreach (var collider in colliders)
            {
                collider.enabled = true;
                collider.attachedRigidbody.isKinematic = false;
                collider.attachedRigidbody.AddForce(Random.onUnitSphere * deadImpulse, ForceMode.VelocityChange);
            }
            animator.enabled = false;
            FindObjectOfType<LevelController>().LoseLevel();
        }
    }
}
