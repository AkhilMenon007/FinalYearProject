using System;

namespace FYP
{
    static class Tags
    {
        public static ushort tagCount
        {
            get
            {
                return (ushort)typeof(Tags).GetFields().Length;
            }
        }
        public static readonly ushort spawnPlayerTag = 0;
        public static readonly ushort destroyPlayerTag = 1;
        public static readonly ushort relayTag = 2;
        public static readonly ushort spawnObjectTag = 3;
        public static readonly ushort spawnObjectWithRotTag = 4;
        public static readonly ushort movementTag = 5;
        public static readonly ushort movementRotTag = 6;
        public static readonly ushort destroyObjectTag = 7;
        public static readonly ushort rpcTag = 8;
    }
}