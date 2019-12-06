using System.Collections;
using System.Collections.Generic;
using System;
using DarkRift;
using UnityEngine;
using DarkRift.Client;

namespace FYP.Client 
{
    public class NetworkEntity : MonoBehaviour
    {
        /// <summary>
        /// -1 for global , This value stores the clientID corresponding to the owner of the object
        /// </summary>
        public int clientID { get; internal set; }
        /// <summary>
        /// The id of the entity shared across all clients
        /// </summary>
        public ushort id { get;internal set; }

        public bool isMine 
        { 
            get 
            { 
                return clientID == ClientManager.client.ID; 
            } 
        }

        public Action OnRegisterCallback;
        public Action OnUnRegisterCallback;

        internal RPC[] rpcList;


        private void Awake()
        {
            rpcList = new RPC[RPCTags.tagCount];

        }

        public void ServeRPC(DarkRiftReader reader)
        {
            ushort rpcTag = reader.ReadUInt16();

        }
        internal void RegisterRPC(RPC rpc,ushort eventTag) 
        {
            rpcList[eventTag] = rpc;
        }
        internal void RemoveRPC(RPC rpc,ushort eventTag) 
        {
            rpcList[eventTag] = null;
        }

        public void RequestRPC(ushort eventTag, RPC parameter) 
        { 
            EntityRPCManager.RequestRPC(this, eventTag, parameter);
        }


        public virtual void Register() 
        {
            ClientManager.RegisterNetworkEntity(this);
            if(isMine)
                Debugger.Log("Registered this client with Entity ID : " + id.ToString());

            OnRegisterCallback?.Invoke();
        }
        public virtual void UnRegister() 
        {
            ClientManager.UnRegisterNetworkEntity(this);

            OnUnRegisterCallback?.Invoke();
        }
    }
}
