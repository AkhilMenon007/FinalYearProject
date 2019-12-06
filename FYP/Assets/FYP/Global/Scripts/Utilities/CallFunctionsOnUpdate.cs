using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CallFunctionsOnUpdate : MonoBehaviour
    {
        [SerializeField]
        private CallbackEvent functionsToCall = null;

        private void Update()
        {
            functionsToCall?.Invoke(this);
        }
    }
}