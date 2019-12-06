using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP {
    public class InvokeAfterDelay : MonoBehaviour
    {
        [SerializeField]
        private float delay = 1f;
        [SerializeField]
        private CallbackEvent functionsToInvoke = null;
        [SerializeField] private List<GameEvent> gameEvents = new List<GameEvent>();

        [SerializeField]
        private bool invokeOnEnable = true;

        private void OnEnable()
        {
            if (invokeOnEnable)
                InvokeEvents();
        }

        public void InvokeEvents()
        {
            StartCoroutine(DelayRoutine());
        }

        public void StopInvocations()
        {
            StopAllCoroutines();
        }

        private IEnumerator DelayRoutine()
        {
            yield return new WaitForSeconds(delay);
            functionsToInvoke?.Invoke(this);


            foreach (GameEvent gameEvent in gameEvents)
            {
                gameEvent.InvokeEvent(this);
            }

        }
        private void OnDisable()
        {
            StopInvocations();
        }
    }
}