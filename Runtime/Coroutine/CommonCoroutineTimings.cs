using UnityEngine;

using Planet.Coroutines{

    public static class CommonCoroutineTimings
    {
        public static WaitForEndOfFrame WaitFrame = new WaitForEndOfFrame();
        public static WaitForSeconds WaitHalfSecond = new WaitForSeconds(0.5f);
    }
}

