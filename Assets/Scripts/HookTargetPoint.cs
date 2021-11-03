using Filo;
using UnityEngine;

public class HookTargetPoint : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;
    
    public Cable Cable { private set; get; } = null;

    public void Lock(Cable cable, Color lockedColor)
    {
        renderer.material.color = lockedColor;
        Cable = cable;
    }
}
