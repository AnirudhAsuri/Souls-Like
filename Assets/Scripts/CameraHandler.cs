using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CameraHandler : MonoBehaviour
    {
        public InputHandler inputHandler;
        public PlayerManager playerManager;

        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private LayerMask environmentLayer;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.3f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffSet = 0.2f;
        public float minimumCollisionOffSet = 0.2f;
        public float lockedPivotPosition = 1.3f;
        public float unlockedPivotPosition = 1f;

        public CharacterManager currentLockOnTarget;

        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public float maximumLockOnDistance = 30;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if(inputHandler.lockOnFlag == false && currentLockOnTarget == null)
            {
                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYInput * lookSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                Vector3 dir = currentLockOnTarget.transform.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.transform.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffSet);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
            {
                targetPosition = -minimumCollisionOffSet;
            }

            // Directly adjust camera position
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y, Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f));
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            for(int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();

                if(character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;

                    if(character.transform.root != targetTransform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if(Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                            if(hit.transform.gameObject.layer == environmentLayer)
                            {
                                //Cannot Lock On
                            }

                        }
                        availableTargets.Add(character);
                    }
                }
            }

            for(int k = 0; k < availableTargets.Count; k++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[k];
                }
            }
        }

        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if(currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}