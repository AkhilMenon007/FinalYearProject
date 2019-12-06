using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FloatVariable", order = 1)]
    public class FloatVariable : ScriptableObject
    {
        public float defaultValue;

        public float maxValue = 100f;
        public float minValue = 0f;

        /// <summary>
        /// Event fired when the variable reaches maxValue
        /// </summary>
        public GameEvent maxEvent;
        /// <summary>
        /// Event fired when variable reaches minValue
        /// </summary>
        public GameEvent minEvent;


        public GameEvent onChangeEvent;

        protected float _currentValue;

        /// <summary>
        /// How much the value changed since last onChangeEvent
        /// </summary>
        [HideInInspector]
        public float delta = 0f;

        private static FloatVariable _identity;

        public void SetFloat(FloatVariable other)
        {
            currentValue = other.currentValue;
        }

        public static FloatVariable identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = CreateInstance<FloatVariable>();
                    _identity.defaultValue = 1f;
                    _identity.OnEnable();
                }
                return _identity;
            }
        }


        public float currentValue
        {
            get { return _currentValue; }
            set
            {

                float oldValue = _currentValue;
                _currentValue = Mathf.Clamp(value, minValue, maxValue);

                if (oldValue == _currentValue)
                    return;

                delta = _currentValue - oldValue;
                onChangeEvent?.InvokeEvent(this);

                if (_currentValue == maxValue)
                    maxEvent?.InvokeEvent(this);
                if (_currentValue == minValue)
                    minEvent?.InvokeEvent(this);
            }
        }
        public static implicit operator float(FloatVariable f)
        {
            return f.currentValue;
        }
        private void OnEnable()
        {
            _currentValue = defaultValue;
        }

        public void SetToMax()
        {
            currentValue = maxValue;
        }
        public void SetToMin()
        {
            currentValue = minValue;
        }
        public void Reset()
        {
            currentValue = defaultValue;
        }

    }
}
