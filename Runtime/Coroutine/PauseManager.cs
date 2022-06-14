using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Planet.Common.PauseSystem
{
    /*
     *  This class should handle pause for everything in the game:
     *  Objectives:
     *      1- Provide a wait function for tasks and Coroutines to pause on
     *      2- Easy to use method to pause/unpause the game
     *      3- OnPause Event to extend functionality where needed
     */
    public static class PauseManager
    {
        //Pause event for external scripts
        public delegate void ActionGamePause(bool Performed);
        public static event ActionGamePause OnPause;

        public static bool IsPaused { get; private set; } = false;

        /// <summary>
        /// If the game is paused stops the coroutine that called this
        /// </summary>
        public static IEnumerator WaitIfPaused()
        {
            while (IsPaused)
                yield return CommonCoroutineTimings.WaitFrame;
        }

        /// <summary>
        /// If the game is paused stops the task that called this
        /// </summary>
        /// <returns> Task Object </returns>        
        public static async Task WaitIfPausedAsync()
        {
            while (IsPaused)
                await Task.Yield();
        }

        public static void ToggleGamePause(bool bToggle)
        {
            IsPaused = bToggle;
            // Pause Physics system and AudioSystem
            Time.timeScale = IsPaused ? 0f : 1f;
            AudioListener.pause = IsPaused;


            //Notify all listeners
            OnPause?.Invoke(IsPaused);
        }

        /// <summary>
        /// This helper function provides coroutines wait functionality which takes into account if the game is paused
        /// </summary>
        /// <param name="WaitAmount"></param> Amount of time to wait
        public static IEnumerator WaitOrPause(float WaitAmount)
        {
            while (WaitAmount > 0)
            {
                yield return WaitIfPaused();
                WaitAmount -= Time.deltaTime;
                yield return CommonCoroutineTimings.WaitFrame;
            }
        }


        /// <summary>
        /// This helper function for async methods provides wait functionality which takes into account if the game is paused
        /// </summary>
        /// <param name="WaitAmount"></param> Amount of time to wait
        public static async Task WaitOrPauseAsync(float WaitAmount)
        {
            while (WaitAmount > 0)
            {
                await WaitIfPausedAsync();
                WaitAmount -= Time.deltaTime;
                await Task.Yield();
            }
        }

        public static async Task FrameOrPauseAsync()
        {
            await WaitIfPausedAsync();
            await Task.Yield();
        }

        public static IEnumerator FrameOrPause()
        {
            yield return WaitIfPaused();
            yield return CommonCoroutineTimings.WaitFrame;
        }


    }
}
