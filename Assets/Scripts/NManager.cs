using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class NManager : MonoBehaviour
{
    public bool isServer;
    public string host;
    public int port;
    public static NManager instance;
    NetManager server;
    NetManager client;
    List<NetPeer> listPeer;
    ActorManager actorManager;

    void Awake()
    {
        instance = this;
        listPeer = new List<NetPeer>();
    }

    void Start()
    {
        if (isServer)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.Start(port);

            server.MaxConnectAttempts = 25;
            listener.ConnectionRequestEvent += request =>
            {
                if (server.PeersCount < server.MaxConnectAttempts)
                    request.AcceptIfKey("passkey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                listPeer.Add(peer);
                Debug.Log(string.Format("We got connection: {0}", peer.EndPoint));
            };
            listener.NetworkReceiveEvent += NetworkReceive;
        }
        else
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            client.Connect(host, port, "passkey");

            listener.NetworkReceiveEvent += NetworkReceive;
        }
        actorManager = ActorManager.instance;
    }

    void NetworkReceive(NetPeer fromPeer, NetPacketReader dataReader, DeliveryMethod deliveryMethod)
    {
        string req = dataReader.GetString();
        if (req == "ACTOR")
        {
            ActorPacket packet = ActorPacket.Deserialize(dataReader);
            actorManager.listActors[packet.index].transform.position = packet.position;
            actorManager.listActors[packet.index].transform.rotation = packet.rotation;
            actorManager.listActors[packet.index].moveType = packet.moveType;
            dataReader.Recycle();
        }
    }

    void Update()
    {
        if (isServer)
        {
            for (int i = 0; i < actorManager.listActors.Count; i++)
            {
                NetDataWriter writer = new NetDataWriter();
                ActorPacket.Serialize(writer, actorManager.listActors[i], i);
                foreach (NetPeer peer in listPeer)
                {
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }
            server.PollEvents();
        }
        else
        {
            client.PollEvents();
        }
    }
}
