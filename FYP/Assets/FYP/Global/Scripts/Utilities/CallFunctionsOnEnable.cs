using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CallFunctionsOnEnable : MonoBehaviour
    {
        [SerializeField]
        private CallbackEvent functionsToCall = null;

        private void OnEnable()
        {
            functionsToCall?.Invoke(this);
        }
    }
}