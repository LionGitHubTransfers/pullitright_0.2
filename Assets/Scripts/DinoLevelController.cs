using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoLevelController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] boneRb;
    
    private LevelController levelController;
    
    void Start()
    {
        levelController = FindObjectOfType<LevelController>();
        levelController.Fsm.Changed += state =>
        {
            if (state == LevelController.GameState.Lose)
            {
                foreach (var bone in boneRb)
                {
                    bone.isKinematic = false;
                    bone.GetComponent<Collider>().enabled = true;
                }
            }
        };
    }
}
