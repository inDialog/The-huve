using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

// RULETAG [TAG] [[TYPE] [SUBTYPE]] [ARG..] [INDEX_ACTOR] [INDEX_METHOD]
struct RulePacket
{
    public Rule rule;
    public string tag;

    public static void Serialize(NetDataWriter writer, Rule rule, string tag)
    {
        writer.Put(tag);
        if (rule as RuleDistance != null)
        {
            RuleDistance ruleDistance = (RuleDistance)rule;
            writer.Put("DISTANCE");
            writer.Put(ruleDistance.distance);
        }
        else if (rule as RuleCollision != null)
        {
            writer.Put("COLLISION");
        }
        else if (rule as RuleCount != null)
        {
            RuleCount ruleCount = (RuleCount)rule;
            writer.Put("COUNT");
            writer.Put((int)ruleCount.type);
            writer.Put(ruleCount.count);
        }
        writer.Put(ActorManager.instance.listActors.IndexOf(rule.from));
        writer.Put(RuleManager.instance.listActions.IndexOf(rule.action));
    }

    public static RuleByTag Deserialize(NetDataReader reader)
    {
        RuleByTag ruleTag = new RuleByTag();

        ruleTag.tag = reader.GetString();
        string type = reader.GetString();
        if (type == "DISTANCE")
        {
            RuleDistance dist = new RuleDistance();
            dist.distance = reader.GetFloat();
            ruleTag.rule = dist;
        }
        else if (type == "COLLISION")
        {
            RuleCollision col = new RuleCollision();
            ruleTag.rule = col;
        }
        else if (type == "COUNT")
        {
            RuleCount count = new RuleCount();
            count.type = (RuleCountType)reader.GetInt();
            count.count = reader.GetFloat();
            ruleTag.rule = count;
        }
        ruleTag.rule.from = ActorManager.instance.listActors[reader.GetInt()];
        ruleTag.rule.action = RuleManager.instance.listActions[reader.GetInt()];
        return ruleTag;
    }
}

struct ActorPacket
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public int index;
    public MoveType moveType;
	public List<Vector3> waypoint;

    public static void Serialize(NetDataWriter writer, Actor actor, int index)
    {
        writer.Put("ACTOR");
        writer.Put(index);
        Vector3Packet.Serialize(writer, actor.transform.position);
        QuaternionPacket.Serialize(writer, actor.transform.rotation);
        Vector3Packet.Serialize(writer, actor.transform.localScale);
        writer.Put((int)actor.moveType);
        writer.Put(actor.waypoint.Count);
        for (int i = 0; i < actor.waypoint.Count; i++)
            Vector3Packet.Serialize(writer, actor.waypoint[i]);
    }

    public static ActorPacket Deserialize(NetDataReader reader)
    {
        ActorPacket packet = new ActorPacket();
        packet.index = reader.GetInt();
        packet.position = Vector3Packet.Deserialize(reader);
        packet.rotation = QuaternionPacket.Deserialize(reader);
        packet.localScale = Vector3Packet.Deserialize(reader);
        packet.moveType = (MoveType)reader.GetInt();
        int waypointCount = reader.GetInt();
        packet.waypoint = new List<Vector3>();
        for (int i = 0; i < waypointCount; i++)
            packet.waypoint.Add(Vector3Packet.Deserialize(reader));
        return packet;
    }
}

struct Vector3Packet
{
    public static void Serialize(NetDataWriter writer, Vector3 v)
    {
        writer.Put(v.x);
        writer.Put(v.y);
        writer.Put(v.z);
    }

    public static Vector3 Deserialize(NetDataReader reader)
    {
        Vector3 res = new Vector3();
        res.x = reader.GetFloat();
        res.y = reader.GetFloat();
        res.z = reader.GetFloat();
        return res;
    }
}

struct QuaternionPacket
{
    public static void Serialize(NetDataWriter writer, Quaternion quaternion)
    {
        writer.Put(quaternion.x);
        writer.Put(quaternion.y);
        writer.Put(quaternion.z);
        writer.Put(quaternion.w);
    }

    public static Quaternion Deserialize(NetDataReader reader)
    {
        return new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
    }
}


public class GameServer : MonoBehaviour, INetEventListener, INetLogger
{
    List<NetPeer> listPeer;
    NetManager netServer;
    ActorManager actorManager;
    public static GameServer instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        actorManager = ActorManager.instance;
        netServer = new NetManager(this);
        netServer.Start(5000);
        netServer.DiscoveryEnabled = true;
        netServer.UpdateTime = 15;
        netServer.MaxConnectAttempts = 25;
        listPeer = new List<NetPeer>();
    }

    void Update()
    {
        netServer.PollEvents();

        for (int i = 0; i < actorManager.listActors.Count; i++)
        {
            NetDataWriter writer = new NetDataWriter();
            ActorPacket.Serialize(writer, actorManager.listActors[i], i);
            foreach (NetPeer peer in listPeer)
            {
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
    }

    public bool Connected()
    {
        if (netServer == null)
            return false;
        return netServer.IsRunning;
    }

    void OnDestroy()
    {
        NetDebug.Logger = null;
        if (netServer != null)
            netServer.Stop();
    }

    public void SendRemoveActor(int index)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put("REMOVE");
        writer.Put(index);
        foreach (NetPeer peer in listPeer)
        {
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public void SendSpawnActor(int index)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put("SPAWN");
        writer.Put(index);
        foreach (NetPeer peer in listPeer)
        {
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        listPeer.Add(peer);
        NetDataWriter writer = new NetDataWriter();
        writer.Put("INIT");
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        for (int i = 0; i < actorManager.listActors.Count; i++)
        {
            writer = new NetDataWriter();
            writer.Put("SPAWN");
            int index = actorManager.listActorsOrigin.IndexOf(actorManager.listActors[i].origin);
            writer.Put(index);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            netServer.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("sample_app");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (listPeer.Contains(peer))
            listPeer.Remove(peer);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        string req = reader.GetString();
        if (req == "MOVETYPE")
        {
            Actor actor = actorManager.listActors[reader.GetInt()];
            MoveType moveType = (MoveType)reader.GetInt();
            actor.moveType = moveType;
            if (actor == actorManager.actorSelected)
            {
                Ui_Utilety.instance.ChargeActor(actor);
            }
        }
        else if (req == "WAYPOINT")
        {
            Actor actor = actorManager.listActors[reader.GetInt()];
            Vector3 pos = Vector3Packet.Deserialize(reader);
            actor.waypoint.Add(pos);
        }
        else if (req == "RULETAG_NEW")
        {
            RuleByTag ruleTag;
            ruleTag = RulePacket.Deserialize(reader);
            RuleManager.instance.AddRuleByTag(ruleTag.rule, ruleTag.tag);
        }
        else if (req == "RULETAG_DEL")
        {
            RuleByTag ruleTag;
            ruleTag = RulePacket.Deserialize(reader);
            RuleManager.instance.DeleteRule(ruleTag.rule, ruleTag.tag);
        }
        reader.Recycle();
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }
}
