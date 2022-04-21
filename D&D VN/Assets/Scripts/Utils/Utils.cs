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

    void Awake()
    {
        instance = this;
    }

    public Coroutine TimedCallback(Callback callback, float delay, bool useScaledTime = true)
    {
        IEnumerator CallbackRoutine()
        {
            if(useScaledTime)
                yield return new WaitForSeconds(delay);
            else
                yield return new WaitForSecondsRealtime(delay);

            callback();
        }

        return instance.StartCoroutine(CallbackRoutine());
    }

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
}
