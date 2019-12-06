using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using System;

namespace FYP.Client
{
    public class EntityRPCManager : MonoBehaviour
    {
        public static EntityRPCManager instance = null;

        private void Awake()
        {
            if (instance == null) 
            {
                instance = this;
            }
            else 
            {
                Destroy(this);
            }
            ClientManager.messageHandlers[Tags.rpcTag] += ServeRPC;
        }
        private static void ServeRPC(object sender, MessageReceivedEventArgs e)
        {
            using(Message message = e.GetMessage()) 
            {
                using(DarkRiftReader reader = message.GetReader()) 
                {
                    ushort entID = reader.ReadUInt16();
                    Debugger.Log("Recieved Message from Entity ID : " + entID.ToString());
                    ushort rpcTag = reader.ReadUInt16();
                    int pos = reader.Position;

                    ClientManager.networkEntities[entID].rpcList[rpcTag].ReadFromReaderAndInvoke(reader);
                }
            }
        }
        public static void RequestRPC(NetworkEntity entity,ushort eventTag,RPC parameters)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(entity.id);
                writer.Write(eventTag);
                writer.Write(parameters);
                using (Message message = Message.Create(Tags.rpcTag, writer))
                    ClientManager.client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
}