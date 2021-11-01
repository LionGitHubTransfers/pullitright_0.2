using System;
using Filo;
using Obi;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private Transform moveHookTransform;
    [SerializeField] private Rigidbody hookRigidbody;
    [SerializeField] private ObiRopeCursor cursor;
    [SerializeField] private float lenghtMofier;
    [SerializeField] private ObiRope obiRope;
    [SerializeField] private ObiColliderBase hookCollider;
    [SerializeField] private ObiColliderBase targetCollider;
    [SerializeField] private Cable cable;
    [SerializeField] private HingeJoint joint;

    public event Action<Cable> OnLocked;
    private Camera camera;
    private bool isLocked = false;
    private bool isLaunched = false;
    private float startRopeLenght;
    
    void Start()
    {
        var pinConstraints = obiRope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(obiRope.solverIndices[0], hookCollider, Vector3.zero, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(obiRope.solverIndices[obiRope.blueprint.activeParticleCount - 1], targetCollider, Vector3.zero, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);

        obiRope.SetConstraintsDirty(Oni.ConstraintType.Pin);
        
        camera = Camera.main;
        startRopeLenght = obiRope.restLength;
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                var point = hit.collider.GetComponent<HookTargetPoint>();
                if (hit.collider.gameObject == gameObject)
                {
                    isLaunched = true;
                }
                if (point != null && point.Cable == null && !isLocked && isLaunched)
                {
                    isLocked = true;
                    cable.links.Add(new Cable.Link {
                        body = hit.collider.GetComponent<CablePoint>(), 
                        type = Cable.Link.LinkType.Attachment
                    });
                    cable.Setup();
                    point.Cable = cable;
                    OnLocked?.Invoke(cable);
                    Destroy(hookRigidbody.gameObject);
                    obiRope.gameObject.SetActive(false);
                } 
                else if (!isLocked && isLaunched)
                {
                    moveHookTransform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    cursor.ChangeLength(Vector3.Distance(transform.position, hit.point) * lenghtMofier);
                    Debug.Log($"{gameObject.name} - {obiRope.restLength}");
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isLocked && isLaunched)
            {
                cursor.ChangeLength(startRopeLenght);
                moveHookTransform.position = transform.position;
            }
        }
    }

    public void SetNeedToPull()
    {
        joint.useMotor = true;
    }

    public void StopPull()
    {
        joint.useMotor = false;
    }
}
