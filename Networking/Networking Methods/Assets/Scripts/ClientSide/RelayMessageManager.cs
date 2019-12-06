using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP;
using DarkRift.Client;
using System;
using DarkRift;

namespace FYP.Client
{
    /*
        Relay Messages are messages which are sent from an entity on 
        one client to the instances of the entity across all the clients
    */
    public class RelayMessageManager : MonoBehaviour
    {
        public static RelayMessageManager instance;
        public delegate void RelayDelegate(DarkRiftReader reader);

        private static Dictionary<ushort, RelayDelegate> networkEntities;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else 
            {
                Destroy(gameObject);
            }
            networkEntities = new Dictionary<ushort, RelayDelegate>();
        }
        private void OnEnable()
        {
            ClientManager.messageHandlers[Tags.relayTag] += HandleRelayMessage;
            ClientManager.OnEntityRegistered += RegisterEntity;
            ClientManager.OnEntityUnRegistered += UnRegisterEntity;
        }

        public void RegisterRelayFunction(NetworkEntity entity,RelayDelegate function) 
        {
            RegisterEntity(entity);
            networkEntities[entity.id] = function;
        }
        public void RemoveRelayFunction(NetworkEntity entity, RelayDelegate function)
        {
            RegisterEntity(entity);
            networkEntities[entity.id] = function;
        }



        private void UnRegisterEntity(NetworkEntity entity)
        {
            if (!networkEntities.ContainsKey(entity.id))
                return;
            networkEntities.Remove(entity.id);
        }

        private void RegisterEntity(NetworkEntity entity)
        {
            if (networkEntities.ContainsKey(entity.id))
                return;
            networkEntities.Add(entity.id,null);
        }

        private void HandleRelayMessage(object sender, MessageReceivedEventArgs e)
        {
            using(Message mess = e.GetMessage()) 
            {
                using(DarkRiftReader reader = mess.GetReader()) 
                {
                    ushort entID = reader.ReadUInt16();
                    int pos = reader.Position;
                    networkEntities[entID]?.Invoke(reader);
                }
            }
        }
        public void SendRelayMessage(NetworkEntity entity,SendMode sendMode,Action<DarkRiftWriter> callback) 
        {
            using(DarkRiftWriter writer = DarkRiftWriter.Create()) 
            {
                writer.Write(entity.id);
                callback?.Invoke(writer);
                using (Message message = Message.Create(Tags.relayTag, writer))
                {
                    ClientManager.client.SendMessage(message, sendMode);
                }
            }
        }
    }
}