using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DestroyToPoolOnTime : MonoBehaviour
{
    [SerializeField]
    private float delay=1f;
    [SerializeField]
    private bool callOnEnable = true;
    private void OnEnable()
    {
        if (callOnEnable)
            StartCoroutine(DestroyRoutine(delay));
    }

    public void Destroy(float delay)
    {
        StartCoroutine(DestroyRoutine(delay));
    }

    private IEnumerator DestroyRoutine(float t)
    {
        if(t>0f)
            yield return new WaitForSeconds(delay);
        PoolManager.Destroy(gameObject);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
