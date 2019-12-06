using System;
using System.Collections.Generic;
using System.Linq;
using DarkRift;
using DarkRift.Server;
using FYP;

namespace FYPServer
{
    /// <summary>
    /// This Plugin handles event occuring based on a session involving player connect,disconnect,rpc etc.
    /// </summary>
    public class SessionManager : Plugin,ISchedulable
    {
        public override bool ThreadSafe => false;
        public override Version Version => new Version(1, 0, 0);

        private Dictionary<IClient, Player> players = null;

        public SessionManager(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
        }

        public void Init() 
        {
            players = new Dictionary<IClient, Player>();
            ClientManager.ClientDisconnected += RemovePlayer;
            ClientManager.ClientConnected += SpawnPlayer;
            
        }
        private void InitializeClient(IClient client)
        {
            client.MessageReceived += RelayMessage;
        }

        private void RelayMessage(object sender, MessageReceivedEventArgs e)
        {
            if(e.Tag==Tags.rpcTag || e.Tag == Tags.relayTag) 
            {
                using(Message mess = e.GetMessage()) 
                {
                    foreach (IClient client in ClientManager.GetAllClients())
                    {
                        client.SendMessage(mess,SendMode.Reliable);
                    }
                }
            }
        }

        #region Player Spawn and Removal

        private void SpawnPlayer(object sender, ClientConnectedEventArgs e)
        {
            Random r = new Random();
            Player newPlayer = new Player(e.Client.ID, 0, (float)r.NextDouble()*10f - 5f, (float)r.NextDouble() * 10f - 5f, (float)r.NextDouble() * 10f - 5f);

            using (DarkRiftWriter writer = DarkRiftWriter.Create()) 
            {
                writer.Write(newPlayer.id);
                writer.Write(newPlayer.owner);
                using(Message m= Message.Create(Tags.spawnPlayerTag, writer)) 
                {
                    foreach (IClient client in players.Keys)
                        client.SendMessage(m, SendMode.Reliable);
                }
            }


            players.Add(e.Client, newPlayer);
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                foreach (Player player in players.Values)
                {
                    writer.Write(player.id);
                    writer.Write(player.owner);
                }
                using (Message m = Message.Create(Tags.spawnPlayerTag, writer))
                {
                    e.Client.SendMessage(m, SendMode.Reliable);
                }
            }
            InitializeClient(e.Client);
        }
        private void RemovePlayer(object sender, ClientDisconnectedEventArgs e)
        {
            players.Remove(e.Client);
            RemoveClient(e.Client);
        }



        private void RemoveClient(IClient client)
        {

        }

        #endregion
    }
}
