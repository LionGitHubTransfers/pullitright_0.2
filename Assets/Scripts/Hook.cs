using System;
using System.Linq;
using Filo;
using Obi;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float magnetPointDistance;
    [SerializeField] private Transform ropeStartTransform;
    [SerializeField] private ParticleSystem pullParticleSystem;
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color startColor;
    [SerializeField] private MeshRenderer[] cableRenderers;

    public event Action<Cable> OnLocked;
    public event Action<Cable> OnUnlocked;
    private HookTargetPoint[] targetPoints;
    private HookTargetPoint lockedPoint;
    private ObiRopeExtrudedRenderer obiRenderer;
    private Camera camera;
    private bool isLaunched = false;
    private float startRopeLenght;
    public bool IsCanLaunch = true;
    private LayerMask dragMask;

    void Start()
    {
        targetPoints = FindObjectsOfType<HookTargetPoint>();
        var pinConstraints = obiRope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(obiRope.solverIndices[0], hookCollider, ropeStartTransform.localPosition , Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(obiRope.solverIndices[obiRope.blueprint.activeParticleCount - 1], targetCollider, Vector3.zero, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);
        obiRenderer = obiRope.GetComponent<ObiRopeExtrudedRenderer>();

        obiRope.SetConstraintsDirty(Oni.ConstraintType.Pin);
        IsCanLaunch = true;
        camera = Camera.main;
        startRopeLenght = obiRope.restLength;
        SetColors(startColor);
        dragMask = LayerMask.GetMask("Drag", "UI");
    }

    private void SetColors(Color color)
    {
        foreach (var cableRenderer in cableRenderers)
        {
            cableRenderer.material.color = color;    
        }
    }
    
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0) && !obiRenderer.enabled)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == gameObject && lockedPoint != null)
                {
                    cable.links.RemoveAt(cable.links.Count - 1);
                    obiRenderer.enabled = true;
                    lockedPoint.Unlock(startColor);
                    SetColors(startColor);
                    lockedPoint = null;
                    cable.Setup();
                    OnUnlocked?.Invoke(cable);
                }
            }
        }
        
        if (!obiRenderer.enabled) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsCanLaunch) return;
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == gameObject && lockedPoint == null)
                {
                    isLaunched = true;
                }
            }
        }
        
        if (!isLaunched) return;
        
        if (Input.GetMouseButton(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, dragMask))
            {
                var point = targetPoints.FirstOrDefault(targetPoint => targetPoint.Cable == null && 
                                                                       Vector3.Distance(targetPoint.transform.position, hit.point) < magnetPointDistance);
                if (point != null && point.Cable == null && lockedPoint == null)
                {
                    lockedPoint = point;
                    moveHookTransform.position = lockedPoint.transform.position;
                    cursor.ChangeLength(Vector3.Distance(transform.position, lockedPoint.transform.position) * lenghtMofier);
                    Vibration.VibratePop();
                    SetColors(lockedColor);
                } 
                else if (point == null)
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        moveHookTransform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);    
                    }
                    else
                    {
                        moveHookTransform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    }
                    
                    cursor.ChangeLength(Vector3.Distance(transform.position, hit.point) * lenghtMofier);
                    lockedPoint = null;
                    SetColors(startColor);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (lockedPoint != null)
            {
                cable.links.Add(new Cable.Link {
                    body = lockedPoint.gameObject.GetComponent<CablePoint>(), 
                    type = Cable.Link.LinkType.Attachment
                });
                cable.Setup();
                lockedPoint.Lock(cable, lockedColor);
                OnLocked?.Invoke(cable);
                obiRenderer.enabled = false;
                isLaunched = false;
            }
            else if (isLaunched)
            {
                cursor.ChangeLength(startRopeLenght);
                moveHookTransform.position = transform.position;
                isLaunched = false;
            }
        }
    }

    public void SetNeedToPull()
    {
        joint.useMotor = true;
        pullParticleSystem.Play();
    }

    public void StopPull()
    {
        joint.useMotor = false;
        pullParticleSystem.Stop();
    }
}
