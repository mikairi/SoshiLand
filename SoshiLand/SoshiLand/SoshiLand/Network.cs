using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Timers;

using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;

// Network Library
using Lidgren.Network;

namespace SoshiLand
{
    class Network
    {
        // Configuration settings
        private NetPeerConfiguration config;                // Network Configuration
        private NetClient client = null;                    // Client Object
        private NetServer server = null;                    // Host object
        private bool isServer = false;                      // boolean to distinguish between Host or Client
        private int networkPort = 14242;                    // Port to listen to (host) or to connect to (client). Default is 14242

        private bool connectionEstablished = false;         // boolean for whether a connection is established


        private IPEndPoint connectToIP;                     // IP that client is attempting to connect to

        // Polling Variables
        private int numberOfPolls = 100;
        private Timer pollTimer = new Timer(2000);


        string periodsLoading = ".";

        private string networkSystemMessage = "TEST";       // For network messages, mostly informative for the user

        // The message that the network wants to communicate to the system / user.
        // This will contain useful messages about the status of the connection
        public string NetworkMessage
        {
            get { return networkSystemMessage; }
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

        // This simply starts the server (Host or Client)
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
            pollDiscoverHost(ip);
        }

        // Same as clientDiscoverHost, but on LAN (so no need to specify an IP)
        public void clientDiscoverLAN()
        {
            client.DiscoverLocalPeers(14242);
        }

        // For creating the "animation" that something is loading in the form of periods
        private string textPeriodLoading()
        {
            if (periodsLoading.Length > 4)
                periodsLoading = ".";
            else
                periodsLoading = periodsLoading + ".";

            return periodsLoading;
        }

        // The polling interval for connecting to a host
        private void PollHostInterval(object source, ElapsedEventArgs e)
        {
            // Debug message
            Console.WriteLine("POLLING IP: " + connectToIP.Address.ToString(), e.SignalTime);

            // If there are still polls left, attempt to connect to IP
            if (numberOfPolls > 0)
            {
                networkSystemMessage = "Attempting to connect to: " + connectToIP.Address.ToString() + " " + textPeriodLoading();
                client.DiscoverKnownPeer(connectToIP);
            }
            // Otherwise, disable timer and send failure message
            else
            {
                // Debug message
                Console.WriteLine("Failed to establish connection to: " + connectToIP.Address.ToString());

                networkSystemMessage = "Failed to establish connection to: " + connectToIP.Address.ToString();

                // Disable timer
                pollTimer.Enabled = false;
            }

            // If a connection was established, send success message and disable timer
            if (connectionEstablished)
            {
                // Debug message
                Console.WriteLine("Connection Established to: " + connectToIP.Address.ToString());

                networkSystemMessage = "Connection Established to: " + connectToIP.Address.ToString();

                // Disable timer
                pollTimer.Enabled = false;
            }

            // Decrement number of polls
            numberOfPolls -= 1;

        }

        private void pollDiscoverHost(IPEndPoint ip)
        {
            if (ip != null)
            {
                connectToIP = ip;
                numberOfPolls = 10;                // Arbitrary value for number of Polls.

                // Start the Polling Timer
                pollTimer.Elapsed += new ElapsedEventHandler(PollHostInterval);
                pollTimer.Enabled = true;
            }
        }

        // Takes an IP string and returns it in a converted IPEndPoint type
        // Converts an IP String (ie. 192.168.1.2) into IPEndPoint 
        // Later, I'll probably extend this to include the port too
        public IPEndPoint convertIP(string ip)
        {
            // Ensure that the IP given is valid
            string pattern = "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}$";
            Regex regex = new Regex(pattern, RegexOptions.Singleline);

            if (ip.Length > 15)
            {
                Console.WriteLine("WARNING! IP ENTERED IS INVALID! GREATER THAN 15 CHARACTERS!");
                networkSystemMessage = "Warning! Invalid IP entered!";
                return null;
            }

            Match match = regex.Match(ip, 0, ip.Length);

            if (!match.Success)
            {
                Console.WriteLine("WARNING! IP ENTERED IS INVALID!");

                networkSystemMessage = "Warning! Invalid IP entered!";

                return null;
            }

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
            
            // Convert strings into bytes
            byte part1Byte = 0;
            byte part2Byte = 0;
            byte part3Byte = 0;
            byte part4Byte = 0;

            // Try to convert the IP into bytes. The try/catch statement ensures that the values are valid (ie. between 0-255)
            try{
                part1Byte = System.Convert.ToByte(part1);
                part2Byte = System.Convert.ToByte(part2);
                part3Byte = System.Convert.ToByte(part3);
                part4Byte = System.Convert.ToByte(part4);
            } catch (Exception e) {
                Console.WriteLine("WARNING! Could not convert IP into bytes. Was there a segment greater than 255?");
                Console.WriteLine(e.Message.ToString());
            }

            // Build the IP Byte array
            byte[] ipByteArray = new byte[4];
            ipByteArray[0] = part1Byte;
            ipByteArray[1] = part2Byte;
            ipByteArray[2] = part3Byte;
            ipByteArray[3] = part4Byte;

            IPAddress tempIPAddress = new IPAddress(ipByteArray);

            // Create the IPEndPoint
            // Temporarily hardcoded with port
            IPEndPoint tempIPEndPoint = new IPEndPoint(tempIPAddress, networkPort);

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
                            networkSystemMessage = "Congratulations! Connection Established! Client: " + incomingMessage.SenderEndpoint;

                            connectionEstablished = true;

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
                            networkSystemMessage = "Congratulations! Connection Established! Host: " + incomingMessage.SenderEndpoint;

                            connectionEstablished = true;
                            break;
                    }
                }
            }
                
        }
    }
}
