using DG.Tweening;
using UnityEngine;

public class PullButton : MonoBehaviour
{
    [SerializeField] private Transform movablePart;
    [SerializeField] private Vector3 pushVector;
    [SerializeField] private float pushTime;

    public void Push()
    {
        movablePart.DOLocalMove(pushVector, pushTime);
    }

    public void Release()
    {
        movablePart.DOLocalMove(Vector3.zero, pushTime);
    }
}
