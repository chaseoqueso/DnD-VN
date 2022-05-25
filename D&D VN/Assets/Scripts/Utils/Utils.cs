using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject temp = new GameObject("Utils");
                instance = temp.AddComponent<Utils>();
            }
            return instance;
        }

        private set
        {
            if(instance == null)
                instance = value;
            else if(instance != value)
                Destroy(value.gameObject);
        }
    }

    private static Utils instance;

    public delegate void Callback();
    public delegate bool Predicate();

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Performs a callback function after a specified delay.
    /// </summary>
    /// <param name="callback">The function to be called.</param>
    /// <param name="delay">The amount of time to wait.</param>
    /// <param name="useScaledTime">Whether the delay is affected by Time.timeScale.</param>
    /// <returns>Returns the Coroutine that manages the timed callback.</returns>
    public Coroutine TimedCallback(Callback callback, float delay, bool useScaledTime = true)
    {
        IEnumerator CallbackRoutine()
        {
            // If we want time to be scaled by Time.deltaTime, use WaitForSeconds, otherwise use WaitForSecondsRealtime.
            if(useScaledTime)
                yield return new WaitForSeconds(delay);
            else
                yield return new WaitForSecondsRealtime(delay);

            // After the delay, perform the callback.
            callback();
        }

        return instance.StartCoroutine(CallbackRoutine());
    }

    /// <summary>
    /// Performs a callback function repeatedly on a fixed interval.
    /// </summary>
    /// <param name="callback">The function to be called.</param>
    /// <param name="interval">The amount of time between callback calls.</param>
    /// <param name="repetitions">An optional number of times to repeat the callback before ending the routine. If a negative value is passed, the callback will be repeated indefinitely.</param>
    /// <param name="useScaledTime">Whether the interval is affected by Time.timeScale.</param>
    /// <returns>Returns the Coroutine that manages the repeated callback.</returns>
    public Coroutine RepeatCallback(Callback callback, float interval, int repetitions = -1, bool useScaledTime = true)
    {
        IEnumerator CallbackRoutine()
        {
            int count = repetitions;
            while(count != 0)
            {
                if(useScaledTime)
                    yield return new WaitForSeconds(interval);
                else
                    yield return new WaitForSecondsRealtime(interval);

                callback();

                if(count > 0)
                    --count;
            }
        }

        return instance.StartCoroutine(CallbackRoutine());
    }

    /// <summary>
    /// Performs a callback function once a predicate returns true, then ends the routine.
    /// </summary>
    /// <param name="callback">The function to be called.</param>
    /// <param name="predicate">The predicate to evaluate whether to call the callback or not.</param>
    /// <param name="duration">An optional duration to watch for the predicate to return true, after which the Coroutine ends.</param>
    /// <param name="useScaledTime">Whether the delay is affected by Time.timeScale (this value is only used if duration > 0).</param>
    /// <param name="performCallbackAfterDuration">Whether to call the callback once the duration ends (this value is only used if duration > 0).</param>
    /// <returns>Returns the Coroutine that watches for the predicate to evaluate to true.</returns>
    public Coroutine PerformOnceTrue(Callback callback, Predicate predicate, float duration = -1, bool useScaledTime = true, bool performCallbackAfterDuration = false)
    {
        bool limitedDuration = duration > 0;
        IEnumerator CallbackRoutine()
        {
            bool flag = predicate();
            while(!flag && (!limitedDuration || duration > 0))
            {
                yield return null;
                flag = predicate();
                duration -= useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }

            if(flag || (limitedDuration && performCallbackAfterDuration))
                callback();
        }

        return instance.StartCoroutine(CallbackRoutine());
    }
    /// <summary>
    /// Performs a callback function every frame that a predicate returns true, then ends the routine.
    /// </summary>
    /// <param name="callback">The function to be called.</param>
    /// <param name="predicate">The predicate to evaluate whether to call the callback or not.</param>
    /// <returns>Returns the Coroutine that watches for the predicate to evaluate to true.</returns>
    public Coroutine PerformWheneverTrue(Callback callback, Predicate predicate, float duration = -1, bool useScaledTime = true)
    {
        bool limitedDuration = duration > 0;
        IEnumerator CallbackRoutine()
        {
            while(!limitedDuration || duration > 0)
            {
                if(predicate())
                    callback();

                yield return null;
                duration -= useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }
        }

        return instance.StartCoroutine(CallbackRoutine());
    }

    #region UI Utility
        public static string TruncateDecimalForUIDisplay( float number )
        {
            float value = Mathf.Round( number * 100 );
            value = value / 100f;
            return value + "";
        }
    #endregion
}
