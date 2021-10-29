using Filo;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private Transform moveHookTransform;
    [SerializeField] private Cable cable;
    [SerializeField] private HingeJoint joint;

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
                if (hit.collider.gameObject == gameObject)
                {
                    isLaunched = true;
                }
                if (hit.collider.CompareTag("Player") && !isLocked && isLaunched)
                {
                    isLocked = true;
                    cable.links.RemoveAll(link => link.type == Cable.Link.LinkType.Attachment);
                    cable.links.Add(new Cable.Link() {body = hit.collider.GetComponent<CablePoint>(), type = Cable.Link.LinkType.Attachment});
                    cable.Setup();
                } 
                else if (!isLocked && isLaunched)
                {
                    moveHookTransform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                }
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
