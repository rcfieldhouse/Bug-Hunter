﻿using System;
using KaymakNetwork.Network;
using UnityEngine;

public class ServerNetworkConfig
{
    private static Server _socket;
    public static Server socket
    {
        get { return _socket;}
        set {
            if (_socket != null)
            {
                _socket.ConnectionReceived -= Socket_ConnectionReceived;
                _socket.ConnectionLost -= Socket_ConnectionLost;
            }

            _socket = value;
            if(_socket != null)
            {
                _socket.ConnectionReceived += Socket_ConnectionReceived;
                _socket.ConnectionLost += Socket_ConnectionLost;
            }
        }
    }

    public static void InitNetwork()
    {
        if (!(socket == null))
            return;

        socket = new Server(100)
        {
            BufferLimit = 2048000,
            PacketAcceptLimit = 100,
            PacketDisconnectCount = 150
        };

        ServerNetworkReceive.PacketRouter();
    }

    internal static void Socket_ConnectionReceived(int connectionID)
    {
        //Debug.Log(_socket.ClientIp(connectionID));
        Debug.Log("Connection received on index ["+connectionID+"]");

        ServerNetworkSend.WelcomeMsg(connectionID, "You Are Connected to DeepSpace! Operative: '"+connectionID+"'");

        for(int i = 0; i<ServerNetworkManager.enemyCount; i++)
        {
            Debug.Log("Updating Enemy Positions.......");
           // EnemyType type = ServerNetworkManager.enemyObjectList[i].GetComponentInChildren<AI>().type;
            

           // ServerNetworkSend.InstantiateNetworkEnemy(i, type);
        }
        Debug.Log("Updating Enemy Positions.......Done!");
    }

    internal static void Socket_ConnectionLost(int connectionID)
    {
        //Debug.Log(_socket.ClientIp(connectionID));
        Debug.Log("Connection lost on index ["+connectionID+"]");

    }
}
