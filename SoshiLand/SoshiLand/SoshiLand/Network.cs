using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Microsoft.Xna.Framework;

// Network Library
using Lidgren.Network;

namespace SoshiLand
{
    class Network
    {
        // Configuration settings
        private NetPeerConfiguration config;
        private NetClient client = null;
        private NetServer server = null;
        private bool isServer = false;
        private int networkPort;
        
        // Temporary
        private string sendMessage = "TEST";

        public string NetworkMessage
        {
            get { return sendMessage; }
        }

        // Constructor for Client
        public Network()
        {
            config = new NetPeerConfiguration("clientConfig");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);         // Enables client to detect server discovery response message
        }
        // Constructor for Host
        public Network(int port)
        {
            networkPort = port;

            config = new NetPeerConfiguration("serverConfig");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);          // Enables Server to detect client discovery request messages
            config.Port = networkPort;                                                  // Sets the network port that the server will listen to
            isServer = true;                                                            // Sets the server flag
        }

        public void startNetwork()
        {
            if (isServer)
            {
                server = new NetServer(config);                                         // Load Host Configuration
                server.Start();                                                         // Starts the host server
            }
            else
            {
                client = new NetClient(config);                                         // Load client configuration
                client.Start();                                                         // Starts the client server
            }
        }

        // This function doesn't work yet
        public string getThisIP()
        {
            if (isServer)
                return server.Socket.LocalEndPoint.ToString();
            //return server.UniqueIdentifier.ToString();
            else
                return client.UniqueIdentifier.ToString(); 
        }

        public void clientDiscoverHost(IPEndPoint ip)
        {
            // Sends a discovery request to the IP
            client.DiscoverKnownPeer(ip);
        }

        public void clientDiscoverLAN()
        {
            client.DiscoverLocalPeers(14242);
        }

        // Takes an IP string and returns it in a converted IPEndPoint type
        // Converts an IP String (ie. 192.168.1.2) into IPEndPoint 
        // Later, I'll probably extend this to include the port too
        public IPEndPoint convertIP(string ip)
        {
            // Variables to hold temporary data
            string part1 = "", part2 = "", part3 = "", part4 = "";
            string tempIP = ip;
            for (int i = 0; i < 4; i++)
            {
                int index = tempIP.IndexOf(".");
                if (i == 0)
                    part1 = tempIP.Substring(0, index);
                if (i == 1)
                    part2 = tempIP.Substring(0, index);
                if (i == 2)
                    part3 = tempIP.Substring(0, index);
                if (i == 3)
                    part4 = tempIP;
                if (i != 3)
                    tempIP = tempIP.Substring(index + 1);
            }
            
            byte part1Byte = System.Convert.ToByte(part1);
            byte part2Byte = System.Convert.ToByte(part2);
            byte part3Byte = System.Convert.ToByte(part3);
            byte part4Byte = System.Convert.ToByte(part4);

            // Build the IP Byte array
            byte[] ipByteArray = new byte[4];
            ipByteArray[0] = part1Byte;
            ipByteArray[1] = part2Byte;
            ipByteArray[2] = part3Byte;
            ipByteArray[3] = part4Byte;

            IPAddress tempIPAddress = new IPAddress(ipByteArray);

            // Create the IPEndPoint
            // Temporarily hardcoded with port
            IPEndPoint tempIPEndPoint = new IPEndPoint(tempIPAddress, 14242);

            return tempIPEndPoint;
        }

        public void Update(GameTime gameTime)
        {
            
            NetIncomingMessage incomingMessage;

            // Standard network message polling

            // For host
            if (isServer)
            {
                while ((incomingMessage = server.ReadMessage()) != null)
                {
                    switch (incomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            // Create a response and write some example data to it
                            NetOutgoingMessage response = server.CreateMessage();
                            response.Write("My Server Name");

                            // Send the response to the sender of the request
                            server.SendDiscoveryResponse(response, incomingMessage.SenderEndpoint);
                            
                            Console.WriteLine("CLIENT DISCOVERED: " + incomingMessage.SenderEndpoint);
                            sendMessage = "Congratulations! Connection Established! Client: " + incomingMessage.SenderEndpoint;

                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            Console.WriteLine(incomingMessage.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine("STATUS CHANGED: " + (NetConnectionStatus)incomingMessage.ReadByte());
                            break;
                        default:
                            Console.WriteLine("Unhandled type: " + incomingMessage.MessageType);
                            break;
                    }
                    server.Recycle(incomingMessage);
                }

            }
                // For Client
            else
            {
                while ((incomingMessage = client.ReadMessage()) != null)
                {
                    switch (incomingMessage.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            Console.WriteLine("Found server at " + incomingMessage.SenderEndpoint + " name: " + incomingMessage.ReadString());
                            sendMessage = "Congratulations! Connection Established! Host: " + incomingMessage.SenderEndpoint;
                            break;
                    }
                }
            }
                
        }
    }
}
