using UnityEngine;
using FYP.Client;
using RPC = FYP.Client.RPC;
using DarkRift;
using FYP;

[RequireComponent(typeof(NetworkEntity))]
public class SpawnTest : MonoBehaviour
{
    NetworkEntity networkEntity;


    private class ShootRPC : RPC
    {
        public NetworkEntity target;
        SpawnTest outer;
        public override void ReadFromReaderAndInvoke(DarkRiftReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override void Serialize(SerializeEvent e)
        {

        }
    }

    private class ColorRPC : RPC
    {
        public Color32 color;
        SpawnTest outer;
        public ColorRPC(SpawnTest outer) 
        {
            this.outer = outer;
        }
        public ColorRPC(Color32 color) 
        {
            this.color = color;
        }


        public override void ReadFromReaderAndInvoke(DarkRiftReader reader)
        {
            Color c=new Color(
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte(),
                reader.ReadByte()
                );
            c = c / 255f;

            outer.GetComponent<MeshRenderer>().material.color = c;
        }
        public override void Serialize(SerializeEvent e)
        {
            e.Writer.Write(color.r);
            e.Writer.Write(color.g);
            e.Writer.Write(color.b);
            e.Writer.Write(color.a);
        }
    }

    private void Shoot(NetworkEntity target) 
    {
        target.SendMessage("Shot", this, SendMessageOptions.DontRequireReceiver);
    }

    private void Awake()
    {
        networkEntity = GetComponent<NetworkEntity>();
    }

    public void TestFunction() 
    {
        EntityRPCManager.RequestRPC(networkEntity, RPCTags.colorTag, new ColorRPC(Random.ColorHSV()));
    }
    private void Start()
    {
        networkEntity.RegisterRPC(new ColorRPC(this),RPCTags.colorTag);
    }
    private void Update()
    {
        if (networkEntity.clientID==ClientManager.client.ID && Input.GetKeyDown(KeyCode.K))
        {
            EntityRPCManager.RequestRPC(networkEntity, RPCTags.colorTag, new ColorRPC(Random.ColorHSV()));
        }
    }
}
