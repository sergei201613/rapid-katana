using System.Collections;
using UnityEngine;

namespace TeaGames.PlatformerEngine.Utilities
{
    public static class TimeUtility
    {
        public static System.Action Delay(this MonoBehaviour mono, float delayInSeconds, System.Action action)
        {
            IEnumerator Del()
            {
                yield return new WaitForSeconds(delayInSeconds);
                action?.Invoke();
            }

            mono.StartCoroutine(Del());
            return action;
        }
    }
}
