using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CommandListener : MonoBehaviour, IInvokable
    {
        [SerializeField] protected GameEvent gameEvent;
        [SerializeField] protected Object source;

        [SerializeField] public CallbackEvent OnCallbackRecieved;



        private void OnEnable()
        {
            gameEvent.Subscribe(this);

        }

        private void OnDisable()
        {
            gameEvent.UnSubscribe(this);

        }


        public void OnEventInvoked(object sender)
        {
            if(sender as Object == source)
                OnCallbackRecieved?.Invoke(sender);
            else if(sender is MonoBehaviour && source is MonoBehaviour)
            {
                if (((MonoBehaviour)sender).gameObject==((MonoBehaviour) source).gameObject)
                    OnCallbackRecieved?.Invoke(sender);
            }
            else if(sender is MonoBehaviour && source is GameObject)
            {
                if (((MonoBehaviour)sender).gameObject == source)
                    OnCallbackRecieved?.Invoke(sender);
            }
        }
    }
}