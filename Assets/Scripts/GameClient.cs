using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net.Sockets;
using System.Net;

public class GameClient : MonoBehaviour, INetEventListener
{
    public static GameClient instance;
    NetManager netClient;
    ActorManager actorManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        netClient = new NetManager(this);
        netClient.Start();
        netClient.UpdateTime = 15;
        actorManager = ActorManager.instance;
    }

    void Update()
    {
        if (netClient == null)
            return ;
        netClient.PollEvents();

        NetPeer peer = netClient.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
        }
        else
            netClient.SendDiscoveryRequest(new byte[] {1}, 5000);
    }

    public bool Connected()
    {
        if (netClient == null || netClient.FirstPeer == null)
            return false;
        return true;
    }

    public void SendRuleTag(Rule rule, string tag)
    {
        NetPeer peer = netClient.FirstPeer;
        if (peer == null)
            return ;
        NetDataWriter writer = new NetDataWriter();
        writer.Put("RULETAG_NEW");
        RulePacket.Serialize(writer, rule, tag);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void SendRuleTagDelete(Rule rule, string tag)
    {
        NetPeer peer = netClient.FirstPeer;
        if (peer == null)
            return ;
        NetDataWriter writer = new NetDataWriter();
        writer.Put("RULETAG_DEL");
        RulePacket.Serialize(writer, rule, tag);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void SendWayPoint(Actor actor, Vector3 pos)
    {
        NetPeer peer = netClient.FirstPeer;
        if (peer == null)
            return ;
        NetDataWriter writer = new NetDataWriter();
        writer.Put("WAYPOINT");
        writer.Put(actorManager.listActors.IndexOf(actor));
        Vector3Packet.Serialize(writer, pos);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void SendMoveType(Actor actor, MoveType type)
    {
        NetPeer peer = netClient.FirstPeer;
        if (peer == null)
            return ;
        NetDataWriter writer = new NetDataWriter();
        writer.Put("MOVETYPE");
        writer.Put(actorManager.listActors.IndexOf(actor));
        writer.Put((int)type);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    // public void SendMoveType(Actor actor, MoveType type)

    void OnDestroy()
    {
        if (netClient != null)
            netClient.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        string req = reader.GetString();
        if (req == "ACTOR")
        {
            ActorPacket packet = ActorPacket.Deserialize(reader);
            if (packet.index > actorManager.listActors.Count - 1)
                return ;
            Actor actor = actorManager.listActors[packet.index];
            actor.syncPos = packet.position;
            actor.syncRot = packet.rotation;
            actor.transform.localScale = packet.localScale;
            actor.waypoint = packet.waypoint;
            if (actor == actorManager.actorSelected && actor.moveType != packet.moveType && Ui_Utilety.instance != null)
            {
                actor.moveType = packet.moveType;
                Ui_Utilety.instance.ChargeActor(actor);
            }
        }
        else if (req == "REMOVE")
        {
            int index = reader.GetInt();
            actorManager.RemoveActor(index);
        }
        else if (req == "SPAWN")
        {
            int index = reader.GetInt();
            actorManager.SpawnActor(index);
        }
        else if (req == "INIT")
        {
            foreach (Actor actor in actorManager.listActors)
            {
                Destroy(actor.gameObject);
            }
            actorManager.listActors.Clear();
        }
        reader.Recycle();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryResponse && netClient.PeersCount == 0)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            netClient.Connect(remoteEndPoint, "sample_app");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
}