using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleCharacterMotor : MonoBehaviour, IMotionAnimated
    {
        public float moveX { get; set; }
        public float moveY { get; set; }
        public float turning { get; set; }
        public Semaphore moveAllowed { get; set; } = new Semaphore();


        public bool freeLook = false;

        public float speed = 10.0f;
        public float walkSpeed = 1f;
        public float reverseSpeed = 1f;
        public float sideSpeed = 1f;
        public float sensitivity = 60.0f;
        CharacterController character;
        [SerializeField]
        private AnimationResolver animationResolver = null;


        [SerializeField]
        private Transform cam = null;
        [SerializeField]
        private Transform upwardOverride = null;

        private Vector3 upwardVector
        {
            get
            {
                if (upwardOverride != null)
                    return upwardOverride.up;
                else
                    return Vector3.up;
            }
        }


        float moveFB, moveLR;
        float rotHorizontal, rotVertical;

        float gravity = -9.8f;


        private void Awake()
        {
            character = GetComponent<CharacterController>();
            if (animationResolver != null)
                animationResolver.RegisterMovementScript(this);
        }

        private void OnDisable()
        {
            if (animationResolver != null)
                animationResolver.UnRegisterMovementScript(this);
        }
        private void Update()
        {
            if (moveAllowed)
            {
                moveX = Input.GetAxisRaw("Horizontal");
                moveY = Input.GetAxisRaw("Vertical");
            }
            else
            {
                moveX = 0f;
                moveY = 0f;
            }


            if(Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(1)) 
            {
                freeLook = true;
            }
            else 
            {
                freeLook = false;
            }

            turning = 0f;
        }
        void FixedUpdate()
        {
            Vector3 movement = new Vector3(moveX * sideSpeed, 0f, moveY * (moveY > 0 ? speed : reverseSpeed));
            if (!Input.GetKey(KeyCode.LeftShift) && moveY > 0)
            {
                movement.z = movement.z * walkSpeed / speed;
                moveY = moveY * walkSpeed / speed;
            }

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, upwardVector);
            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, upwardVector);

            movement.y = gravity;
            if(!freeLook) 
            {
                turning = Quaternion.Angle(Quaternion.LookRotation(forward, upwardVector), Quaternion.LookRotation(camForward, upwardVector)) / 180f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(camForward, upwardVector), Time.fixedDeltaTime*5f);
            }
            movement = transform.rotation * movement;
            character.Move(movement * Time.fixedDeltaTime);
        }
    }
}