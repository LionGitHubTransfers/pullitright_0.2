using System;
using Filo;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private Transform moveHookTransform;
    [SerializeField] private Cable cable;
    [SerializeField] private HingeJoint joint;

    public event Action OnLocked;
    
    private Camera camera;
    private bool isLocked = false;
    private bool isLaunched = false;
    private float startRopeLenght;

    void Start()
    {
        camera = Camera.main;
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
                    cable.links.RemoveAll(link => link.type == Cable.Link.LinkType.Attachment);
                    var cablePoint = hit.collider.GetComponent<CablePoint>();
                    cable.links.Add(new Cable.Link() {body = cablePoint, type = Cable.Link.LinkType.Attachment});
                    cable.Setup();
                    point.Cable = cable;
                    OnLocked?.Invoke();
                } 
                else if (!isLocked && isLaunched)
                {
                    moveHookTransform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            moveHookTransform.position = transform.position;
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
