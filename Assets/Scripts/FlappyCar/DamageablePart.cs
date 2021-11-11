using System;
using UnityEngine;

namespace FlappyCar
{
    public class DamageablePart : MonoBehaviour
    {
    
        private SkinnedMeshRenderer smr;
        private FixedJoint fj;
        private HingeJoint hj;
        private int hp;

        private void Awake()
        {
            smr = GetComponent<SkinnedMeshRenderer>();
            fj = GetComponent<FixedJoint>();
            hj = GetComponent<HingeJoint>();
            hp = 0;
        }

        public void SetDamage(int value)
        {
            if (hp >= 100) return;
            hp += value;
            smr.SetBlendShapeWeight(0, hp);
            if (hp >= 100)
            {
                transform.SetParent(null);
                GetComponent<Collider>().isTrigger = false;        
                var rig = gameObject.GetOrAddComponent<Rigidbody>();
                rig.useGravity=true;
                rig.isKinematic = false;
            }
            if(fj&&hp>=50) Destroy(fj);
            if(hj&&hp>=100) Destroy(hj);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Ground"))
            {
                SetDamage(100);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                SetDamage(5);
            }
        }
    }
}
