using System;
using System.Collections.Generic;
using System.Text;

namespace FYPServer
{
    /// <summary>
    /// An object in the network which has a position which needs to be tracked across all the players in the room along with an id.
    /// </summary>
    class NetworkEntity
    {
        public ushort id { get; set; }
        public ushort owner { get; set; }
        public ushort index { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        public float rotX { get; set; }
        public float rotY { get; set; }
        public float rotZ { get; set; }

    }
}
