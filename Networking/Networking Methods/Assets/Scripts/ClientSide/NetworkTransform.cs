using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FYP.Client;
using FYP;
using DarkRift;
using DarkRift.Client;

[RequireComponent(typeof(NetworkEntity))]
public class NetworkTransform : MonoBehaviour
{

    [SerializeField]
    private bool syncPosition = true;
    [SerializeField]
    private bool syncRotation = false;

    [Tooltip("When this is set the object is controlled by local motions and other clients will follow the object based on this client")]
    /// <summary>
    /// When this is set to true the updates are obtained locally and not from network
    /// </summary>
    public bool localUpdate = false;
    [Tooltip("How much motion should be present for the position to be synched")]
    [SerializeField]
    private float positionSyncThreshold = 0.1f;
    [Tooltip("How much motion should be present for the rotation to be synched in degrees")]
    [SerializeField]
    private float rotationSyncThreshold = 10f;

    [Header("Applicable for Objects moved with network only : ")]

    [Tooltip("How much should the translation be lerped , higher value makes it snappier")]
    [SerializeField]
    private float movementLerpSpeed = 2f;
    [Tooltip("Same thing as above but with rotation")]
    [SerializeField]
    private float rotationLerpSpeed = 10f;

    [Tooltip("How much should the object be moved to snap the object to position instead of lerping them")]
    [SerializeField]
    private float movementSnapThreshold = 1f;
    [Tooltip("Same as above but with rotation")]
    [SerializeField]
    private float rotationSnapThreshold = 1f;


    DarkRift.Client.Unity.UnityClient client;
    NetworkEntity networkEntity;
    private Vector3 lastNetworkSyncPosition;
    private Vector3 lastNetworkSyncRotation;


    private void Awake()
    {
        client = ClientManager.client;
        networkEntity = GetComponent<NetworkEntity>();
        networkEntity.OnRegisterCallback += Initialize;
    }

    private void Initialize()
    {
        lastNetworkSyncPosition = transform.position;
        lastNetworkSyncRotation = transform.eulerAngles;
        if (networkEntity.clientID == client.ID)
        {
            localUpdate = true;
        }
        else 
        {
            localUpdate = false;
            ClientManager.messageHandlers[Tags.movementRotTag] += NetworkUpdate;
            ClientManager.messageHandlers[Tags.movementTag] += NetworkUpdate;
        }
    }
    private void OnDisable()
    {

        ClientManager.messageHandlers[Tags.movementRotTag] -= NetworkUpdate;
        ClientManager.messageHandlers[Tags.movementTag] -= NetworkUpdate;
    }

    private void NetworkUpdate(object sender, MessageReceivedEventArgs e)
    {
        using(Message mess = e.GetMessage()) 
        {
            using(DarkRiftReader reader = mess.GetReader()) 
            {
                ushort entityID = reader.ReadUInt16();
                if (entityID != networkEntity.id) 
                    return;
                lastNetworkSyncPosition.x = reader.ReadSingle();
                lastNetworkSyncPosition.y = reader.ReadSingle();
                lastNetworkSyncPosition.z = reader.ReadSingle();
                if (e.Tag == Tags.movementRotTag) 
                {
                    lastNetworkSyncRotation.x = reader.ReadSingle();
                    lastNetworkSyncRotation.y = reader.ReadSingle();
                    lastNetworkSyncRotation.z = reader.ReadSingle();
                }
            }
        }
    }

    private void Update()
    {
        CheckTransform();//Checks if threshold has crossed on local player and sends data, does nothing on remote
        UpdateTransform();//Lerps the position and rotation to last received values, does nothing on local
    }


    private void UpdateTransform() 
    {
        if (localUpdate)
            return;
        if (syncPosition) 
        {
            if((transform.position- lastNetworkSyncPosition).magnitude > movementSnapThreshold || (transform.position - lastNetworkSyncPosition).magnitude < 0.01f) 
            {
                transform.position = lastNetworkSyncPosition;
            }
            else 
            {
                transform.position = Vector3.Lerp(transform.position, lastNetworkSyncPosition, Time.deltaTime * movementLerpSpeed);
            }
        }
        if (syncRotation) 
        {
            if ((transform.eulerAngles - lastNetworkSyncRotation).magnitude > rotationSnapThreshold || (transform.eulerAngles - lastNetworkSyncRotation).magnitude< 0.01f)
            {
                transform.rotation = Quaternion.Euler(lastNetworkSyncRotation);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(lastNetworkSyncRotation), Time.deltaTime * rotationLerpSpeed);
            }

        }
    }
    private void CheckTransform() 
    {
        if (!localUpdate)
            return;
        if (syncPosition)
        {
            if ((transform.position - lastNetworkSyncPosition).magnitude > positionSyncThreshold)
            {
                SendTransform();
                return;
            }
        }
        if (syncRotation)
        {
            if ((transform.rotation.eulerAngles - lastNetworkSyncRotation).magnitude > rotationSyncThreshold)
            {
                SendTransform();
                return;
            }
        }
    }

    private void SendTransform() 
    {
        lastNetworkSyncPosition = transform.position;
        lastNetworkSyncRotation = transform.rotation.eulerAngles;

        using(DarkRiftWriter writer = DarkRiftWriter.Create()) 
        {
            writer.Write(networkEntity.id);
            writer.Write(lastNetworkSyncPosition.x);
            writer.Write(lastNetworkSyncPosition.y);
            writer.Write(lastNetworkSyncPosition.z);
            if (syncRotation) 
            {
                writer.Write(lastNetworkSyncRotation.x);
                writer.Write(lastNetworkSyncRotation.y);
                writer.Write(lastNetworkSyncRotation.z);
                using (Message message = Message.Create(Tags.movementRotTag, writer))
                {
                    client.SendMessage(message, SendMode.Unreliable);
                }
            }
            else 
            {
                using (Message message = Message.Create(Tags.movementTag, writer))
                {
                    client.SendMessage(message, SendMode.Unreliable);
                }
            }
        }
    }
}
