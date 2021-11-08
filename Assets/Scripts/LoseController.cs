using DG.Tweening;
using UnityEngine;

public class LoseController : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [SerializeField] private string tagToFind;
    [SerializeField] private bool isNeedParticle = false;
    [SerializeField] private ParticleSystem loseParticlePrefab;
    [SerializeField] private float loseParticleSize;
    [SerializeField] private Transform bounceTransform;
    [SerializeField] private Vector3 bounceVector;
    [SerializeField] private float bounceTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToFind))
        {
            if (isNeedParticle)
            {
                var point = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                var particle = Instantiate(loseParticlePrefab, point, Quaternion.Euler(-90, 0, 0));
                particle.transform.localScale = Vector3.one * loseParticleSize;
                bounceTransform.DOPunchScale(bounceVector, bounceTime, 10, 10);
            }
            levelController.LoseLevel();
        }
    }
}
