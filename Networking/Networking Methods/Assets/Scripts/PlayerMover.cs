using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Client;

[RequireComponent(typeof(NetworkEntity))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField]
    private float speed=5f;
    [SerializeField]
    private float turning=30f;

    NetworkEntity entity;
    float moveH, moveV;
    private void Awake()
    {
        entity = GetComponent<NetworkEntity>();
        entity.OnRegisterCallback += Init;
    }


    private void Init()
    {
        if (!entity.isMine)
            Destroy(this);
    }

    void Update()
    {
        moveH = Input.GetAxis("Horizontal");
        moveV = Input.GetAxis("Vertical");
        transform.position = transform.position + (transform.forward * Time.deltaTime * speed * moveV);
    }
    private void LateUpdate()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0f, turning * moveH * Time.deltaTime, 0f);
    }

    private void OnDisable()
    {
        entity.OnRegisterCallback -= Init;
    }
}
