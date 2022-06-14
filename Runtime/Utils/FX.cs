
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planet.Effects
{
    /*
     * After modding for so many years Call of duty I got used to they FX system so I decided to make a wrapper around Unity's FX system
     * to use similar calls and reduce the work needed to actually play FXs
     */
    public static class FX
    {
        private static Dictionary<string, GameObject> precachedFX = new Dictionary<string, GameObject>();
        public static GameObject PlayFx(string name, Vector3 position, Vector3 angles, GameObject parent = null) => PlayFx(name, position, Quaternion.Euler(angles.x, angles.y, angles.z), parent);
        public static GameObject PlayFx(string name, Vector3 position, Quaternion angles, GameObject parent = null)
        {
            GameObject fx = FetchFXGameObject(name);
            GameObject instance = null;
            if (fx is null)
            {
                return null;
            }

            Debug.Log(parent == null);
            if (parent == null)
            {
                instance = GameObject.Instantiate(fx, position, angles);
                return instance;

            }

            instance = GameObject.Instantiate(fx, position, angles, parent.transform);
            return instance;
        }

        public static void StopFX(GameObject fxRef)
        {
            if (fxRef is null)
            {
                return;
            }

            GameObject.Destroy(fxRef);
        }

        /*
         * Adds a gameObject
         */
        public static bool PrecacheFX(GameObject particleFX)
        {
            if (precachedFX.ContainsKey(particleFX.name))
            {
                Debug.LogWarning("Attempted to cache an already cached FX");
                return false;
            }
            precachedFX.Add(particleFX.name, particleFX);
            return true;
        }

        private static GameObject FetchFXGameObject(string name)
        {
            GameObject prefab = null;
            precachedFX.TryGetValue(name, out prefab);
            if (prefab is null)
            {
                Debug.LogWarning("Attempted to fetch un-cached FX");
                return null;
            }

            return prefab;
        }
    }
}