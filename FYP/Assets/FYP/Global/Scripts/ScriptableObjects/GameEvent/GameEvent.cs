using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameEvent", order = 1)]
    public class GameEvent : ScriptableObject
    {
        public List<IInvokable> listeners = new List<IInvokable>();

        public void Subscribe(IInvokable listener)
        {
            listeners.Add(listener);
        }

        public void UnSubscribe(IInvokable listener)
        {
            listeners.Remove(listener);
        }
        
        public void InvokeEvent(object sender)
        {
            foreach (IInvokable listener in listeners.ToArray())
                listener.OnEventInvoked(sender);
        }
    }
}
