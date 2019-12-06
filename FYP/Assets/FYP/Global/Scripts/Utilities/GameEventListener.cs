using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace FYP
{
    [System.Serializable]
    public class CallbackEvent : UnityEvent<object>
    {
    }
    public class GameEventListener : MonoBehaviour,IInvokable
    {
        [SerializeField] protected GameEvent gameEvent;

        [SerializeField] public CallbackEvent OnCallbackRecieved;



        private void OnEnable()
        {
            gameEvent.Subscribe(this);

        }

        private void OnDisable()
        {
            gameEvent.UnSubscribe(this);
        }


        public virtual void OnEventInvoked(object sender)
        {
            //Debug.Log(gameEvent.name);
            OnCallbackRecieved?.Invoke(sender);
        }
    }
}
