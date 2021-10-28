using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float changeRopeSpeed;

    private Camera camera;
    private bool isLocked = false;
    private bool isNeedToPull = false;
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
                if (hit.collider.gameObject == gameObject)
                {
                    isLaunched = true;
                }
                if (hit.collider.CompareTag("Player") && !isLocked && isLaunched)
                {
                    var pinConstraints = obiRope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
                    pinConstraints.Clear();
                    var batch = new ObiPinConstraintsBatch();
                    batch.AddConstraint(obiRope.solverIndices[0], hookCollider, Vector3.zero, Quaternion.identity, 0, 0, float.PositiveInfinity);
                    var obiColliderBase = hit.collider.GetComponent<ObiColliderBase>();
                    batch.AddConstraint(obiRope.solverIndices[obiRope.blueprint.activeParticleCount - 1], obiColliderBase, Vector3.zero, Quaternion.identity, 0, 0, float.PositiveInfinity);
                    batch.activeConstraintCount = 2;
                    pinConstraints.AddBatch(batch);

                    obiRope.SetConstraintsDirty(Oni.ConstraintType.Pin);
                    isLocked = true;
                    cursor.ChangeLength(Vector3.Distance(transform.position, obiColliderBase.transform.position) * lenghtMofier);
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

    private void FixedUpdate()
    {
        if (isNeedToPull)
        {
            cursor.ChangeLength(obiRope.restLength - changeRopeSpeed * Time.deltaTime);
            Debug.Log($"{gameObject.name} - {obiRope.restLength}");
        }
    }

    public void SetNeedToPull()
    {
        isNeedToPull = true;
        hookRigidbody.isKinematic = false;
    }
}
