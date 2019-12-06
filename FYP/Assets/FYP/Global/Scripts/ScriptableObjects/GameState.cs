using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{


    [CreateAssetMenu(menuName ="ScriptableObjects/GameState")]
    public class GameState : ScriptableObject
    {
        private GameStates _currentState;
        [SerializeField]
        private GameEvent onStateChangeEvent=null;

        [SerializeField]
        private GameStates defaultState = GameStates.Waiting;

        public GameStates previousState;
        public GameStates currentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                if (value != currentState)
                {
                    previousState = _currentState;
                    _currentState = value;
                    onStateChangeEvent?.InvokeEvent(this);
                }
            }
        }

        private void OnEnable()
        {
            _currentState = defaultState;
            previousState = defaultState;
        }

    }
    [System.Flags]
    public enum GameStates
    {
        Waiting = 1,
        Started = 2,
        Paused = 4,
        Over = 8,
    }
}