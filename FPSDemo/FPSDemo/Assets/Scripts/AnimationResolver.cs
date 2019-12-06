using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP
{

    [RequireComponent(typeof(Animator))]
    public class AnimationResolver : MonoBehaviour
    {
        private Animator animator = null;

        public Semaphore moveLocked = null;
        public Semaphore rotLocked = new Semaphore();


        private float xMovement = 0f;
        private float yMovement = 0f;

        [SerializeField]
        private float animationSmoothing = 3f;
        [SerializeField]
        private float _animationSpeed = 1f;
        public float animationSpeed
        {
            get
            {
                return _animationSpeed;
            }
            set
            {
                _animationSpeed = value;
                if (animator != null)
                    animator.SetFloat(animatorSpeedID, _animationSpeed);
            }
        }

        #region Animator parameter ID
        private int xMoveFloatID;
        private int yMoveFloatID;
        private int attackSpeedID;
        private int turningFloatID;
        private int animatorSpeedID;
        private int isMovingBoolID;
        #endregion

        private IMotionAnimated currentMotion = null;
        public void RegisterMovementScript(IMotionAnimated motion)
        {
            currentMotion = motion;
            moveLocked = currentMotion.moveAllowed;
        }
        public void UnRegisterMovementScript(IMotionAnimated motion)
        {
            if (currentMotion == motion) 
            {
                currentMotion = null;
                moveLocked = null;
            }
        }
        private void Awake()
        {
            animator = GetComponent<Animator>();
            animatorSpeedID = Animator.StringToHash("moveSpeed");
            xMoveFloatID = Animator.StringToHash("moveX");
            yMoveFloatID = Animator.StringToHash("moveY");
            turningFloatID = Animator.StringToHash("turning");
            isMovingBoolID = Animator.StringToHash("isMoving");
            attackSpeedID = Animator.StringToHash("attackSpeed");
            animationSpeed = _animationSpeed;
            animator.fireEvents = false;
        }
        private void Update()
        {
            if (currentMotion != null)
            {
                SetMovement();
            }
            if (Input.GetMouseButton(0))
            {
                animator.SetBool("Attacking", true);
            }
            else
            {
                animator.SetBool("Attacking", false);
            }
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("AttackTrigger", true);
            }
        }

        public void SetPlayerRooted(bool val, object obj)
        {
            if (val)
                currentMotion?.moveAllowed?.Set(obj);
            else
                currentMotion?.moveAllowed?.Reset(obj);
        }
        private void SetMovement()
        {
            //if (currentMotion.moveX * currentMotion.moveX + currentMotion.moveY * currentMotion.moveX < 0.01f)
            //    animator.SetBool(isMovingBoolID, false);
            //else
            //    animator.SetBool(isMovingBoolID, true);

            xMovement = Mathf.Lerp(xMovement, currentMotion.moveX, Time.deltaTime * animationSmoothing);
            yMovement = Mathf.Lerp(yMovement, currentMotion.moveY, Time.deltaTime * animationSmoothing);
            animator.SetFloat(xMoveFloatID, xMovement);
            animator.SetFloat(yMoveFloatID, yMovement);
        }
    }
}