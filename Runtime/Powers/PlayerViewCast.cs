using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Planet.Powers
{
    // TODO Unify this into the telekinesis script, theres no need ot keep it separate as both need each other
    public class PlayerViewCast : MonoBehaviour
    {

        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameObject crossHair;
        [SerializeField] private Material OutlineMaterial;
        [SerializeField] private Telekinesis playerTelekinesis;

        [SerializeField] private float CameraRayStartScalar = 1;
        [SerializeField] private float capsuleCastLenght = 100f;
        [SerializeField] private float capsuleCastRadius = 50f;
        private GameObject outlineGO = null;
        private GameObject outlinedObject = null;

        private Vector2 lastCameraInput = default;
        private bool CameraInPlace = false;


        public void ClearOutlineObject()
        {
            if (outlineGO == null) return;
            Destroy(outlineGO);
            outlineGO = null;
        }

        //[SerializeField] private int maxRayCastHits = 8;
        [SerializeField] private LayerMask hittableLayers = default;
        private RaycastHit[] viewHits;
        public RaycastHit[] ViewHits
        {
            get { return viewHits; }
        }

        private void FixedUpdate()
        {
            //viewHits = new RaycastHit[maxRayCastHits];
            if (!playerTelekinesis.IsHoldingObject && !CameraInPlace)
            {
                CylinderCast();
            }
        }

        public void OnLook(InputValue value)
        {
            if (value.Get<Vector2>() == lastCameraInput)
            {
                CameraInPlace = true;
            }
            lastCameraInput = value.Get<Vector2>();
            CameraInPlace = false;
        }
        private void CylinderCast()
        {
            // Not consistent, it can also grab objects behind the player
            Ray ray = playerCamera.ScreenPointToRay(crossHair.transform.position);
            viewHits = Physics.SphereCastAll((ray.origin + ray.direction) * CameraRayStartScalar, capsuleCastRadius, ray.direction, capsuleCastLenght, hittableLayers);
            var orderedByProximity = viewHits.OrderBy(c => (ray.origin - c.transform.position).sqrMagnitude).ToArray();
            viewHits = orderedByProximity;

            // After sort the first object will always be used to enable outline on it
            if (viewHits.Length > 0 && outlinedObject != viewHits[0].transform.gameObject)
            {
                //CreateOutline(viewHits[0].transform.gameObject, 1.5f, Color.white);
                outlinedObject = viewHits[0].transform.gameObject;
            }


            Debug.DrawLine(ray.origin, ray.direction * capsuleCastLenght, Color.green, 0.5f);
            foreach (var hit in viewHits)
            {
                Debug.DrawLine((ray.origin + ray.direction) * CameraRayStartScalar, hit.transform.position, Color.yellow, 0.5f);
            }
            //Debug.Log("num Hit objects: " + numHits);
        }

        private void CreateOutline(GameObject o, float scaleFactor, Color color)
        {
            ClearOutlineObject();
            Rigidbody p_rb = o.GetComponent<Rigidbody>();
            p_rb.isKinematic = true;

            GameObject outlineObject = Instantiate(o, o.transform.position, o.transform.rotation, o.transform);
            outlineObject.transform.localPosition = Vector3.zero;

            Renderer rend = outlineObject.GetComponent<Renderer>();

            outlineObject.layer = LayerMask.NameToLayer("Default");
            rend.material = OutlineMaterial;
            //rend.material.SetColor("_OutlineColor", color);
            //rend.material.SetFloat("_Scale", scaleFactor);

            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Destroy(outlineObject.GetComponent<Collider>());
            Destroy(outlineObject.GetComponent<Rigidbody>());
            outlineGO = outlineObject;
            p_rb.isKinematic = false;
        }
    }
}