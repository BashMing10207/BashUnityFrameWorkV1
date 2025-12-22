using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Unit
{
    public class UnitPawn : ActorNode
    {

    
        protected override void Awake()
        {
            base.Awake();

            
        }

        // [New] 데이터 초기화
        public virtual void Initialize()
        {


        }

        public void LookAt(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}