﻿using System;
using System.Net;
using System.Net.Sockets;
using static Extras.TcpListenerExtensions;
using static Extras.NetworkHelpers;
using static Extras.GlobalHelpers;
using Extras;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            char input = 'a';
            int port = 6000;
            int theirPort = 6001;
            bool running = true;
            string myIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            string theirIP = string.Empty;
            Message message;
            TcpListener listener = new TcpListener(IPAddress.Parse(myIP), port);

            try
            {                
                while (running)
                {
                    PrintGameHeader(myIP, true);
                    Print("Please select an option:");
                    Print("1. Initiate battle (client).");
                    Print("2. Invite challengers (server).");
                    Print("3. Exit.");

                    input = GetInput(singleKey: true, intercept: true)[0];

                    switch (input)
                    {
                        // The 'client';
                        // attempts to connect
                        case '1':
                            {
                                listener.Start();

                                theirIP = GetInput("Enter in the IP (v6) of your opponent:");

                                Print($"Attempting to connect to [{theirIP}]...");
                                SendMessage(theirIP, theirPort, Message.CLIENT_CONNECT);

                                message = listener.ListenForMessage();
                                if (message == Message.SERVER_CONNECT)
                                {
                                    Print("Opponent found.");

                                    string key = GetInput("Choose a letter key on your keyboard:", true);
                                    SendString(theirIP, theirPort, key);
                                }
                            }
                            
                            break;

                        // The 'server';
                        // waits for a connection
                        case '2':
                            {
                                listener.Start();

                                Print("Waiting for challengers...");
                                message = listener.ListenForMessage();
                                
                                if (message == Message.CLIENT_CONNECT)
                                {
                                    Print("Challenger found.");

                                    SendMessage(theirIP, theirPort, Message.SERVER_CONNECT);

                                    string response = listener.ListenForString();
                                }
                            }
                            
                            break;
                        default:
                            running = false;
                            break;
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                listener.Stop();
            }

            Print("done");
            Console.ReadKey();
        }
    }
}
