
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Planet.Utils;

namespace Planet.Powers
{
    public class Telekinesis : DefaultPower
    {
        private GameObject levitatingObject = null;
        private bool objectTransitioning = false;
        private Rigidbody objectRigidBody = null;

        [SerializeField] private Transform levitatingPoint;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject crossHair;
        [SerializeField] private Animator charAnimator;
        [SerializeField] private GameObject concreteFloatFX;
        [SerializeField] private float initialImpulseForce = 100f;

        private PlayerViewCast playerViewCast;
        private GameObject fxInstance = null;

        public bool IsHoldingObject
        {
            get { return levitatingObject != null; }
        }

        private void Start()
        {
            if (levitatingPoint is null)
            {
                Debug.LogError("Missing Levitating Reference");
                levitatingPoint = this.transform;
            }
            playerViewCast = GetComponent<PlayerViewCast>();
            FX.PrecacheFX(concreteFloatFX);
        }

        public void OnPowerRight(InputValue value)
        {
            Activate(value.isPressed);
        }

        public bool Activate(bool input_value)
        {
            // TODO :: This isn't enough, objects can get stuck mid-travel, keep a list of floating items and reset them?
            StopCoroutine("TravelToHand");

            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(crossHair.transform.position);
            if (levitatingObject != null && objectTransitioning == false && input_value)
            {
                Debug.Log("ThrowingObject to crosshair");
                UnbindObjectFromSelf();
                ThrowProp(ray);
                return true;
            }

            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            if (/*Physics.Raycast(ray.origin, ray.direction, out hit, 100, LayerMask.GetMask("KineticGrabbable"))*/playerViewCast.ViewHits.Length > 0)
            {
                //foreach(RaycastHit rhit in playerViewCast.ViewHits)
                if (playerViewCast.ViewHits.Length > 0)
                {
                    RaycastHit rhit = playerViewCast.ViewHits[0];
                    if (rhit.transform.gameObject.layer == LayerMask.NameToLayer("KineticGrabbable"))
                    {
                        levitatingObject = rhit.transform.gameObject;
                        //break;
                    }
                }

                if (levitatingObject is null)
                {
                    Debug.Log("No object found in raycasthits");
                    return false;
                }
                //levitatingObject = hit.transform.gameObject;
                BindObjectToSelf();
                playerViewCast.ClearOutlineObject();

                StartCoroutine("TravelToHand", levitatingObject);
                Debug.Log("Obejct grabbed");
            }

            Debug.Log("No object grabbed");
            return false;
        }

        private IEnumerator TravelToHand(GameObject prop)
        {
            Debug.Log("Object transitioning...");

            objectTransitioning = true;
            float lerpDelta = 0.0f;
            float lerpIncrement = 0.1f;
            float lerpInterval = 0.01f;
            Vector3 originalPos = prop.transform.position;
            while (lerpDelta < 1f)
            {
                prop.transform.position = Vector3.Lerp(originalPos, levitatingPoint.position, lerpDelta);
                lerpDelta += lerpIncrement;

                yield return new WaitForSeconds(lerpInterval);
            }
            levitatingObject.transform.position = levitatingPoint.position;
            levitatingObject.transform.parent = levitatingPoint;

            FX.StopFX(fxInstance);
            fxInstance = FX.PlayFx(concreteFloatFX.name, levitatingPoint.transform.position, new Vector3Int(90, 0, 0), levitatingPoint.gameObject);

            objectTransitioning = false;

            Debug.Log("Object ready!");
        }

        private void ThrowProp(Ray screenRay)
        {
            RaycastHit hit;
            Vector3 force = screenRay.direction * initialImpulseForce;
            if (Physics.Raycast(screenRay.origin, screenRay.direction, out hit, Mathf.Infinity))
            {
                Debug.Log("Found target for kinetic throw!");
                force = (hit.point - screenRay.origin).normalized * initialImpulseForce;
            }

            objectRigidBody.AddForce(force, ForceMode.Impulse);

            levitatingObject = null;
            FX.StopFX(fxInstance);
        }

        private void UnbindObjectFromSelf()
        {
            levitatingObject.transform.parent = null;
            objectRigidBody.detectCollisions = true;
            objectRigidBody.constraints = RigidbodyConstraints.None;
        }

        private void BindObjectToSelf()
        {
            objectRigidBody = levitatingObject.GetComponent<Rigidbody>();
            objectRigidBody.isKinematic = false;
            objectRigidBody.detectCollisions = false;
            objectRigidBody.constraints = RigidbodyConstraints.FreezePosition;
            objectRigidBody.AddTorque(new Vector3(45, 0, 45));
        }

        [SerializeField] private float telekHandWeight = 0.5f;
        [SerializeField] private Transform telekHandIKTarget = null;
        private void OnAnimatorIK(int layerIndex)
        {
            if (charAnimator)
            {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (levitatingObject != null)
                {

                    // Set the look target position, if one has been assigned
                    if (telekHandIKTarget != null)
                    {
                        charAnimator.SetLookAtWeight(0.1f);
                        charAnimator.SetLookAtPosition(telekHandIKTarget.position);
                        charAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, telekHandWeight);
                        charAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, telekHandWeight);
                        charAnimator.SetIKPosition(AvatarIKGoal.RightHand, telekHandIKTarget.position);
                        charAnimator.SetIKRotation(AvatarIKGoal.RightHand, telekHandIKTarget.rotation);
                    }

                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    charAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    charAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    charAnimator.SetLookAtWeight(0);
                }
            }
        }
    }
}
