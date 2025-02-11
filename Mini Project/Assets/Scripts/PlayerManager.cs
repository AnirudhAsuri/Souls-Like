using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
    public class PlayerManager : MonoBehaviour
    {
        public InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        AnimatorHandler animatorHandler;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;


        void Start()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            Cursor.lockState = CursorLockMode.Locked;
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        // Update is called once per frame
        private void Update()
        {
            float  delta = Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            inputHandler.sprintFlag = false;

            inputHandler.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            if (animatorHandler.canRotate)
            {
                playerLocomotion.HandleRotation(delta);
            }

        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
        }

    }
}