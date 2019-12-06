using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FYP
{
    [RequireComponent(typeof(TrailRenderer))]
    public class ClearTrailImmediately : MonoBehaviour
    {
        TrailRenderer trailRenderer=null;
        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        private void OnEnable()
        {
            ClearTrail();
        }

        private void ClearTrail()
        {
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer?.ClearTrailImmediate();
        }
    }
}