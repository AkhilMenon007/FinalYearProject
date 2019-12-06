using DarkRift;

namespace FYP.Client
{
    public abstract class RPC : IDarkRiftSerializable
    {
        public abstract void ReadFromReaderAndInvoke(DarkRiftReader reader);
        public void Deserialize(DeserializeEvent e) { ReadFromReaderAndInvoke(e.Reader); }
        public abstract void Serialize(SerializeEvent e);

    }
}