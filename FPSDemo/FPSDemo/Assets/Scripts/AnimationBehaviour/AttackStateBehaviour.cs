using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP
{
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        static int attackBoolID;
        [SerializeField]
        private bool rootPlayer = false;
        private AnimationResolver animationResolver = null;
        private void Awake()
        {
            attackBoolID = Animator.StringToHash("AttackTrigger");
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, animatorStateInfo, layerIndex);
            if (rootPlayer)
            {
                if (animationResolver == null)
                {
                    animationResolver = animator.GetComponent<AnimationResolver>();
                }
                animationResolver.SetPlayerRooted(true, this);
            }
            animator.SetBool(attackBoolID, false);
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (rootPlayer)
                animationResolver.SetPlayerRooted(false, this);
        }
    }
}