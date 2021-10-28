using UnityEngine;

public class LevelController : MonoBehaviour
{
    public void Pull()
    {
        foreach (var hook in FindObjectsOfType<Hook>())
        {
            hook.SetNeedToPull();
        }
    }
}
