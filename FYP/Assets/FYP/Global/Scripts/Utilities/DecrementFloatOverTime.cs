using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class DecrementFloatOverTime : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private FloatVariable variable = null;

        
        private void Update()
        {
            if (variable !=null)
            {
                variable.currentValue -= Time.deltaTime * speed;
            }
        }

    }
}