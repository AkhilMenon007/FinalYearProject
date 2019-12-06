using System;
using System.Collections.Generic;
using System.Text;

namespace FYPServer
{
    /// <summary>
    /// Player inherits from NetworkEntity 
    /// </summary>
    class Player : IDamageable
    {
        const ushort MAX_HEALTH = 100;
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }
        ushort health;
        public ushort id;
        public ushort owner;

        public Player(ushort id, ushort owner, float posX, float posY, float posZ)
        {
            this.id = id;
            this.owner = owner;
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
        }
        public void TakeDamage(ushort damageAmount)
        {
            health -= damageAmount;
        }
    }
}
