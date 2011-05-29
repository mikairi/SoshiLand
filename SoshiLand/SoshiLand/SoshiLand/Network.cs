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
            config.Port = networkPort;                                                         // Sets the network port that the server will listen to
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

        public void clientDiscoverHost(IPEndPoint ip)
        {
            // Sends a discovery request to the IP
            client.DiscoverKnownPeer(ip);
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
                            break;
                    }
                }
            }
                
        }
    }
}
