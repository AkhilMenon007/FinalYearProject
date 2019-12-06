using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client;
using DarkRift;
using System;
using FYP;
using FYP.Client;
using DarkRift.Client.Unity;
/// <summary>
/// This class is the singleton which handles communication with server.
/// Use this to handle object spawning and destruction along with as a mean to access the UnityClient for Darkrift
/// </summary>
[RequireComponent(typeof(UnityClient))]
public class ClientManager : MonoBehaviour
{
    [HideInInspector]
    public static UnityClient client = null;
    public static ClientManager instance = null;

    public static Dictionary<ushort,NetworkEntity> networkEntities = null;
    public static Action<NetworkEntity> OnEntityRegistered;
    public static Action<NetworkEntity> OnEntityUnRegistered;

    public static EventHandler<MessageReceivedEventArgs>[] messageHandlers;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            client = GetComponent<UnityClient>();
            networkEntities = new Dictionary<ushort, NetworkEntity>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        messageHandlers = new System.EventHandler<MessageReceivedEventArgs>[Tags.tagCount];

        client.MessageReceived += HandleMessage;
    }



    private void HandleMessage(object sender, MessageReceivedEventArgs e)
    {
        messageHandlers[e.Tag]?.Invoke(sender, e);
    }


    public static void RegisterNetworkEntity(NetworkEntity entity)
    {
        if (!networkEntities.ContainsKey(entity.id))
            networkEntities.Add(entity.id, entity);
        OnEntityRegistered?.Invoke(entity);
    }
    public static void UnRegisterNetworkEntity(NetworkEntity entity)
    {
        if (networkEntities.ContainsKey(entity.id))
            networkEntities.Remove(entity.id);
        OnEntityUnRegistered?.Invoke(entity);
    }

}
