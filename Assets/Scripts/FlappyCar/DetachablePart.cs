using UnityEngine;

namespace FlappyCar
{
    public class DetachablePart : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                transform.SetParent(null);
                GetComponent<Collider>().isTrigger = false;
                var rig = gameObject.GetOrAddComponent<Rigidbody>();
                rig.useGravity = true;
                rig.isKinematic = false;
            }
        }
    }
}

public static class NSHelpers
{

    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
        var done = self.TryGetComponent<T>(out var result);
        if (!done) result = self.AddComponent<T>();
        return result;
    }
}
