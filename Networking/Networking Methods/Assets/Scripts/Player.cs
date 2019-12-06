using System;
using System.Collections;
using System.Collections.Generic;
using FYP.Client;
using UnityEngine;

using Random=UnityEngine.Random;

[RequireComponent(typeof(NetworkEntity))]
public class Player : MonoBehaviour
{
    NetworkEntity entity;
    private void Awake()
    {
        entity = GetComponent<NetworkEntity>();
        entity.OnRegisterCallback += Init;
        entity.OnUnRegisterCallback += UnInit;
    }

    private void UnInit()
    {
        if(entity.isMine)
            PlayerManager.OnClick -= ChangeColor;
    }

    private void ChangeColor()
    {
        GetComponent<SpawnTest>()?.TestFunction();
    }

    private void Init()
    {
        if (entity.isMine) 
        {
            PlayerManager.OnClick += ChangeColor;
            PlayerManager.OnClickNoRPC += () => GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
        }
    }


}
