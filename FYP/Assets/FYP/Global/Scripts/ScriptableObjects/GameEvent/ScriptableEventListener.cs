using UnityEngine;

namespace FYP
{
    [CreateAssetMenu(fileName = "ScriptableEventListener", menuName = "ScriptableObjects/EventListener")]
    public class ScriptableEventListener : MonoScriptableobject, IInvokable
    {
        [SerializeField] protected GameEvent gameEvent;

        public CallbackEvent callbackEvents;


        public void OnEventInvoked(object sender)
        {
            callbackEvents?.Invoke(sender);
        }

        protected override void OnBegin()
        {
            gameEvent?.Subscribe(this);
        }

        protected override void OnEnd()
        {
            gameEvent?.UnSubscribe(this);
        }
    }
}