using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class InstantiateOnTransform : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab = null;
        public void InstantiatePrefab()
        {
            PoolManager.Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}