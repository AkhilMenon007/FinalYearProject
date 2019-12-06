namespace FYP 
{
    public static class RPCTags
    {
        public static ushort tagCount
        {
            get
            {
                return (ushort)typeof(RPCTags).GetFields().Length;
            }
        }
        public readonly static ushort colorTag = 0;
        public readonly static ushort attackTag = 1;
    }
}