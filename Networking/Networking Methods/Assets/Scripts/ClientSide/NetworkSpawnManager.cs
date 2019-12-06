using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;

namespace FYP.Client
{

    public class NetworkSpawnManager : MonoBehaviour
    {
        [SerializeField]
        private NetworkSpawnPrefabs networkPrefabSpawner;
        [SerializeField]
        private GameObject localPlayerGameObject;
        DarkRift.Client.Unity.UnityClient client;

        public static NetworkSpawnManager instance = null;

        private void Awake()
        {
            if (instance == null) 
            {
                instance = this;
            }
            else 
            {
                Destroy(gameObject);
            }
        }



        private void OnEnable()
        {
            client = ClientManager.client;
            RegisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            ClientManager.messageHandlers[Tags.spawnPlayerTag] += SpawnPlayer;
            ClientManager.messageHandlers[Tags.spawnObjectTag] += SpawnNetworkObject;
            ClientManager.messageHandlers[Tags.spawnObjectWithRotTag] += SpawnNetworkObject;
            ClientManager.messageHandlers[Tags.destroyObjectTag] += DestroyNetworkObject;
            ClientManager.messageHandlers[Tags.destroyPlayerTag] += DestroyPlayer;
        }

        private void OnDisable()
        {
            UnRegisterMessageHandlers();
        }

        private void UnRegisterMessageHandlers()
        {
            ClientManager.messageHandlers[Tags.spawnPlayerTag] -= SpawnPlayer;
            ClientManager.messageHandlers[Tags.spawnObjectTag] -= SpawnNetworkObject;
            ClientManager.messageHandlers[Tags.spawnObjectWithRotTag] -= SpawnNetworkObject;
            ClientManager.messageHandlers[Tags.destroyObjectTag] -= DestroyNetworkObject;
            ClientManager.messageHandlers[Tags.destroyPlayerTag] -= DestroyPlayer;
        }

        #region Spawn Stuff

        private void SpawnPlayer(object sender, MessageReceivedEventArgs e)
        {
            using (Message m = e.GetMessage())
            {
                using (DarkRiftReader reader = m.GetReader())
                {
                    while (reader.Position != reader.Length)
                    {
                        ushort id = reader.ReadUInt16();
                        ushort teamID = reader.ReadUInt16();
                        //Do stuff that deals with above info
                        if (id == client.ID)
                        {
                            Vector3 pos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
                            //Set this as the local player
                            RequestNetworkSpawn(localPlayerGameObject, pos, Quaternion.identity);
                        }
                        else
                        {
                            //Register player to some player manager or something
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Requests the server to spawn the gameObject across all clients and register it as a network entity
        /// </summary>
        public void RequestNetworkSpawn(GameObject obj, Vector3 pos, Quaternion rot)
        {
            int index = networkPrefabSpawner.GetPrefabIndex(obj);
            if (index < 0)
            {
                Debug.LogError("Unregistered prefab on Network Spawn Prefabs");
                return;
            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write((ushort)index);
                writer.Write(pos.x);
                writer.Write(pos.y);
                writer.Write(pos.z);
                if (rot == Quaternion.identity)
                {
                    using (Message message = Message.Create(Tags.spawnObjectTag, writer))
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
                else
                {
                    Vector3 rot_ = rot.eulerAngles;
                    writer.Write(rot_.x);
                    writer.Write(rot_.y);
                    writer.Write(rot_.z);
                    using (Message message = Message.Create(Tags.spawnObjectWithRotTag, writer))
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }
        }
        private void SpawnNetworkObject(object sender, MessageReceivedEventArgs e)
        {
            using (Message m = e.GetMessage())
            {
                using (DarkRiftReader reader = m.GetReader())
                {
                    while (reader.Position != reader.Length)
                    {
                        Vector3 pos;
                        Quaternion rot = Quaternion.identity;

                        ushort client = reader.ReadUInt16();
                        ushort id = reader.ReadUInt16();
                        int index = reader.ReadUInt16();
                        pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        if (e.Tag == Tags.spawnObjectWithRotTag)
                        {
                            rot = Quaternion.Euler(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        }
                        GameObject spawnedObject;
                        if (client == ClientManager.client.ID)
                        {
                            spawnedObject = PoolManager.Instantiate(networkPrefabSpawner.networkPrefabs[index].localPrefab, pos, rot);
                        }
                        else
                        {
                            spawnedObject = PoolManager.Instantiate(networkPrefabSpawner.networkPrefabs[index].networkPrefab, pos, rot);
                        }
                        NetworkEntity entity = spawnedObject.GetComponent<NetworkEntity>();
                        entity.clientID = client;
                        entity.id = id;
                        entity.Register();
                    }
                }
            }
        }
        #endregion

        #region Destroy Stuff

        private void DestroyPlayer(object sender, MessageReceivedEventArgs e)
        {
            //Enter Code to kill player here..
        }

        private void DestroyNetworkObject(object sender, MessageReceivedEventArgs e)
        {
            using (Message mess = e.GetMessage())
            {
                using (DarkRiftReader reader = mess.GetReader())
                {
                    while (reader.Length != reader.Position)
                    {
                        ushort id = reader.ReadUInt16();
                        NetworkEntity entity;
                        if (ClientManager.networkEntities.TryGetValue(id, out entity))
                        {
                            entity.UnRegister();
                            PoolManager.Destroy(entity.gameObject);
                        }
                    }
                }
            }
        }

        #endregion
    }

}