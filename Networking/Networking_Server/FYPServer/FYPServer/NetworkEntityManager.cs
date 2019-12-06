using System;
using System.Linq;
using System.Collections.Generic;
using DarkRift.Server;
using DarkRift;
using FYP;
namespace FYPServer
{
    /// <summary>
    /// This plugin tracks the transform of all entities along with its entity id and stuff
    /// </summary>
    class NetworkEntityManager : Plugin,ISchedulable
    {
        #region Constructors and stuff

        public override bool ThreadSafe => false;
        public override Version Version => new Version(1,0,0);
        public NetworkEntityManager(PluginLoadData pluginLoadData) : base(pluginLoadData) 
        {

        }
        #endregion

        #region Entity Lookup Stuff
        public ushort entityCount { get; private set; }
        private Dictionary<ushort, NetworkEntity> networkEntities=new Dictionary<ushort, NetworkEntity>();
        private Queue<ushort> freeID=new Queue<ushort>();
        #endregion

        /// <summary>
        /// The dictionary tracks the clients along with a set corresponding to the entities they own, it is designed to destroy
        /// all the entities owned by a client when they disconnect
        /// </summary>
        private Dictionary<ushort, HashSet<ushort>> clientEntitySetLookup=new Dictionary<ushort, HashSet<ushort>>();
        public void Init() 
        {
            entityCount = 0;
            ClientManager.ClientConnected += ClientConnected;
            ClientManager.ClientDisconnected += ClientDisconnected;
        }

        public NetworkEntity GetNetworkEntity(ushort id)
        {
            NetworkEntity entity = null;
            networkEntities.TryGetValue(id, out entity);
            return entity;
        }

        #region Client Connected and Disconnected Methods

        private void ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= SpawnObject;
            e.Client.MessageReceived -= SpawnObjectWithRot;
            e.Client.MessageReceived -= Movement;

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                foreach (ushort entID in clientEntitySetLookup[e.Client.ID])
                {
                    writer.Write(entID);
                    entityCount--;
                    freeID.Enqueue(entID);
                    networkEntities.Remove(entID);
                }
                using (Message message = Message.Create(Tags.destroyObjectTag, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }
            }


            clientEntitySetLookup.Remove(e.Client.ID);
        }


        /// <summary>
        /// Here all the previously spawned entities are sent to the newly connected client and the IClient reference is added
        /// to the clients dictionary
        /// </summary>
        private void ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                foreach (ushort client in clientEntitySetLookup.Keys)
                {
                    foreach (ushort entID in clientEntitySetLookup[client])
                    {
                        NetworkEntity entity = GetNetworkEntity(entID);
                        writer.Write(entity.owner);
                        writer.Write(entity.id);
                        writer.Write(entity.index);
                        writer.Write(entity.posX);
                        writer.Write(entity.posY);
                        writer.Write(entity.posZ);
                        writer.Write(entity.rotX);
                        writer.Write(entity.rotY);
                        writer.Write(entity.rotZ);
                    }
                }
                if(writer.Length > 0) 
                {
                    using (Message m = Message.Create(Tags.spawnObjectWithRotTag, writer))
                    {
                        e.Client.SendMessage(m, SendMode.Reliable);
                    }
                }
            }
            clientEntitySetLookup.Add(e.Client.ID, new HashSet<ushort>());
            e.Client.MessageReceived += SpawnObject;
            e.Client.MessageReceived += SpawnObjectWithRot;
            e.Client.MessageReceived += Movement;
        }

        #endregion

        #region Movement Method

        private void Movement(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag==Tags.movementRotTag || e.Tag == Tags.movementTag) 
            {
                using (Message mess = e.GetMessage())
                {
                    using (DarkRiftReader reader = mess.GetReader())
                    {
                        ushort id = reader.ReadUInt16();
                        NetworkEntity entity = GetNetworkEntity(id);
                        if (entity != null)
                        {
                            entity.posX = reader.ReadSingle();
                            entity.posY = reader.ReadSingle();
                            entity.posZ = reader.ReadSingle();

                            if (e.Tag == Tags.movementRotTag)
                            {
                                entity.rotX = reader.ReadSingle();
                                entity.rotY = reader.ReadSingle();
                                entity.rotZ = reader.ReadSingle();

                                foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
                                {
                                    client.SendMessage(mess, SendMode.Unreliable);
                                }
                            }
                            else 
                            {
                                foreach (IClient client in ClientManager.GetAllClients().Where(x => x != e.Client))
                                {
                                    client.SendMessage(mess, SendMode.Unreliable);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid entity");
                        }
                    }
                    
                }
            }
        }

        #endregion

        #region Helpers to Spawn and Destroy Entities on Server
        public NetworkEntity SpawnEntity(ushort clientID)
        {
            ushort id;
            if (freeID.Count==0) 
            {
                id = entityCount;
            }
            else 
            {
                id = freeID.Dequeue();
            }
            NetworkEntity entity = new NetworkEntity() { id = id, owner = clientID };
            networkEntities.Add(id,entity);
            
            //Add the entity to the entity set of the client
            clientEntitySetLookup[clientID].Add(id);
            
            Console.WriteLine("Spawning object");
            entityCount++;
            return entity;
        }
        public void DestroyEntity(ushort entityID) 
        {
            if (!networkEntities.ContainsKey(entityID))
                return;

            entityCount--;
            ushort clientID = networkEntities[entityID].owner;
            clientEntitySetLookup[clientID].Remove(entityID);
            freeID.Enqueue(entityID);
            networkEntities.Remove(entityID);
        }
        #endregion

        #region Functions to Spawn Object across all Clients

        /// <summary>
        /// Spawn a prefab on all the clients with index,position and rotation sent from the client
        /// </summary>
        private void SpawnObjectWithRot(object sender, MessageReceivedEventArgs e)
        {
            if (e.Tag != Tags.spawnObjectWithRotTag)
                return;
            ushort index;
            float posX, posY, posZ, rotX, rotY, rotZ;
            using (Message m = e.GetMessage())
            {
                using (DarkRiftReader reader = m.GetReader())
                {
                    index = reader.ReadUInt16();
                    posX = reader.ReadSingle();
                    posY = reader.ReadSingle();
                    posZ = reader.ReadSingle();
                    rotX = reader.ReadSingle();
                    rotY = reader.ReadSingle();
                    rotZ = reader.ReadSingle();
                }
                Console.WriteLine("Recieved Command to spawn object at (" + posX + "," + posY + "," + posZ + ")");

            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                NetworkEntity entity = SpawnEntity(e.Client.ID);
                entity.posX = posX;
                entity.posY = posY;
                entity.posZ = posZ;
                entity.rotX = rotX;
                entity.rotY = rotY;
                entity.rotZ = rotZ;
                entity.index = index;

                writer.Write(e.Client.ID);
                writer.Write(entity.id);
                writer.Write(index);
                writer.Write(posX);
                writer.Write(posY);
                writer.Write(posZ);
                writer.Write(rotX);
                writer.Write(rotY);
                writer.Write(rotZ);

                using (Message m = Message.Create(Tags.spawnObjectWithRotTag, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(m, SendMode.Reliable);
                    }
                }
            }
        }
        /// <summary>
        /// Spawn a prefab on all clients with index and position given from client
        /// </summary>
        private void SpawnObject(object sender, MessageReceivedEventArgs e)
        {
            if (e.Tag != Tags.spawnObjectTag)
                return;
            ushort index;
            float posX, posY, posZ;
            using (Message m = e.GetMessage())
            {
                using (DarkRiftReader reader = m.GetReader())
                {
                    index = reader.ReadUInt16();
                    posX = reader.ReadSingle();
                    posY = reader.ReadSingle();
                    posZ = reader.ReadSingle();
                }
                Console.WriteLine("Recieved Command to spawn object at (" + posX + "," + posY + "," + posZ + ")");

            }

            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                NetworkEntity entity = SpawnEntity(e.Client.ID);
                entity.posX = posX;
                entity.posY = posY;
                entity.posZ = posZ;
                entity.index = index;

                writer.Write(e.Client.ID);
                writer.Write(entity.id);
                writer.Write(index);
                writer.Write(posX);
                writer.Write(posY);
                writer.Write(posZ);
                using (Message m = Message.Create(Tags.spawnObjectTag, writer))
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(m, SendMode.Reliable);
                    }
                }
            }
        }
        #endregion

        #region Function to Destroy Object across all Clients

        /// <summary>
        /// Destroys the object across all clients
        /// </summary>
        private void DestroyObject(ushort entityID) 
        {
            using(DarkRiftWriter writer = DarkRiftWriter.Create()) 
            {
                writer.Write(entityID);
                DestroyEntity(entityID);
                using (Message message = Message.Create(Tags.destroyObjectTag, writer)) 
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(message, SendMode.Reliable);
                    }
                }

            }
        }
        #endregion

    }
}
