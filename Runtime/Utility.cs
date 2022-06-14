using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Planet.Generic
{
    public static class Utility
    {
        public static bool GreaterThan(this Vector3 Source, Vector3 other)
        {
            return Source.x > other.x && Source.y > other.y && Source.z > other.z;
        }

        public static void ExecOnChildren(this GameObject Parent, Action<Transform> Func)
        {
            var Children = Parent.GetComponentsInChildren<Transform>();
            foreach (var Obj in Children)
            {
                Func.Invoke(Obj);
            }
        }

        public static float GetRandomInInterval(Vector2 interval)
        {
            return UnityEngine.Random.Range(interval.x, interval.y);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }

        public static bool IsLookingAt(this GameObject Looker, GameObject Obj, float maxLookawayPercentage = 0.25f)
        {
            maxLookawayPercentage = maxLookawayPercentage > 1f ? 1f : maxLookawayPercentage;

            // This function calculates the dot product and the "% of alignment" (ie if dot product between two angles is 40° the % will be 50%)
            //Get two vectors starting from the same origin, one the camera forward, another one the vector connecting camera to bject
            Vector3 LookerToObject = Obj.transform.position - Looker.transform.position;
            Vector3 LookerForward = Looker.transform.forward;

#if UNITY_EDITOR
            Debug.DrawRay(Looker.transform.position, LookerForward, Color.green, 1f);
            Debug.DrawLine(Looker.transform.position, Obj.transform.position, Color.red, 1f);
#endif

            float dot = Vector3.Dot(LookerToObject, LookerForward);

            // If the two vectors are opposite don't even bother checking the %
            if (dot < 0)
            {
                return false;
            }

            // Player is facing object, is it below the threshold?
            float angleBetweenVectors = Vector3.Angle(LookerToObject, LookerForward);
            float alignmentPercentage = angleBetweenVectors / 90; // Of Theta / 90° is the percentage.
                                                                  // Dot is negative when the two vectors are facing each other so in a way the true % needs to be "inverted" for a "true" result
            return alignmentPercentage <= maxLookawayPercentage;
        }

        public static float ScaleValuesBetweenRanges(float min, float max, float current, float newMin, float newMax)
        {
            if (min > max || newMin > newMax)
            {
                return current;
            }

            float temp = (current - min) / (max - min);
            temp = temp * (newMax - newMin) + newMin;
            return temp;
        }
    }
}