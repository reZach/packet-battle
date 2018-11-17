using System;
using System.Net;
using System.Net.Sockets;
using static Extras.TcpListenerExtensions;
using static Extras.NetworkHelpers;
using static Extras.GameHelpers;
using static Extras.GlobalHelpers;
using Extras;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup-specific
            char input = ' ';
            int port = 6000;
            int theirPort = 6001;
            bool running = true;
            string myIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            string theirIP = string.Empty;
            TcpListener listener = new TcpListener(IPAddress.Parse(myIP), port);

            // Game-specific
            char[] response = new char[0];
            char key;            
            int turn = 1;
            int points = 0;
            int theirPoints = 0;                        
            Message message;            

            try
            {                
                while (running)
                {
                    PrintGameHeader(myIP);
                    Print("Please select an option:");
                    Print("1. Initiate battle (client).");
                    Print("2. Invite challengers (server).");
                    Print("3. View rules.");
                    Print("4. Exit.");

                    input = GetInput(singleKey: true, intercept: true)[0];

                    switch (input)
                    {
                        // The 'client';
                        // attempts to connect
                        case '1':
                            {
                                listener.Start();

                                Console.Clear();
                                theirIP = GetInput("Enter in the IP (v6) of your opponent:");

                                Print($"Attempting to connect to [{theirIP}]...");
                                SendMessage(theirIP, theirPort, Message.CLIENT_CONNECT);

                                message = listener.ListenForMessage();
                                if (message == Message.SERVER_CONNECT)
                                {
                                    Console.Clear();
                                    Print("New game begins! Good luck!");

                                    while (points < 3 || theirPoints < 3)
                                    {
                                        Print($"Turn: {turn}");
                                        Print($"Your score: {points}");
                                        Print($"Their score: {theirPoints}");
                                        Print("");

                                        // Show previous points scored here
                                        if (response.Length == 1 && response[0] == '1')
                                        {
                                            points++;
                                            Print("You guessed correctly, you got 1 point!");
                                        }
                                        else if (response.Length == 1 && response[0] == '0')
                                        {
                                            Print("You guessed wrong.");
                                        }

                                        // Pick a character
                                        key = GetInput("Choose a letter key on your keyboard:", true, true)[0];
                                        Print($"You chose '{key}'.");
                                        SendCharArray(theirIP, theirPort, CompileChoices(key));
                                        Print("Waiting for opponent to guess...");

                                        // Validate opponent's choice
                                        response = listener.ListenForCharArray();
                                        bool pointToOppponent = response[0] == key;
                                        Print($"Your opponent chose '{response[0]}'. " +
                                            (pointToOppponent ? "They get 1 point." : "They guessed wrong!"));

                                        if (pointToOppponent)
                                        {
                                            theirPoints++;
                                        }

                                        Print("Opponent is picking a character...");
                                        SendCharArray(theirIP, theirPort, new char[1] { pointToOppponent ? '1' : '0' });

                                        // Wait for opponent's choice
                                        response = listener.ListenForCharArray();
                                        input = GetInput($"Choose one of these characters: [{string.Join(',', response)}]", true, true)[0];
                                        Print($"You chose '{input}'.");
                                        SendCharArray(theirIP, theirPort, new char[1] { input });

                                        // Receive points?
                                        response = listener.ListenForCharArray();

                                        Console.Clear();
                                        turn++;
                                    }

                                    Console.Clear();

                                    Print("Good game!");
                                    Print("");

                                    if (points > theirPoints)
                                    {
                                        Print("You won!");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                    else if (theirPoints > points)
                                    {
                                        Print("You lost.");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                    else
                                    {
                                        Print("You tied.");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                }
                            }
                            
                            break;

                        // The 'server';
                        // waits for a connection
                        case '2':
                            {
                                listener.Start();

                                Console.Clear();
                                Print($"Your IP is: [{myIP}]");
                                Print("");
                                Print("Waiting for challengers...");
                                Print("(alt + f4 to abort)");

                                message = listener.ListenForMessage();
                                
                                if (message == Message.CLIENT_CONNECT)
                                {
                                    Console.Clear();
                                    Print("New game begins! Good luck!");

                                    SendMessage(theirIP, theirPort, Message.SERVER_CONNECT);

                                    while (points < 3 || theirPoints < 3)
                                    {
                                        Print($"Turn: {turn}");
                                        Print($"Your score: {points}");
                                        Print($"Their score: {theirPoints}");
                                        Print("");
                                        Print("Opponent is going first...");                                        

                                        // Guess opponent's character
                                        response = listener.ListenForCharArray();
                                        input = GetInput($"Choose one of these characters: [{string.Join(',', response)}]", true, true)[0];
                                        Print($"You chose '{input}'.");
                                        SendCharArray(theirIP, theirPort, new char[1] { input });

                                        // Receive points?
                                        response = listener.ListenForCharArray();
                                        if (response.Length == 1 && response[0] == '1')
                                        {
                                            points++;
                                            Print("You guessed correctly, you got 1 point!");
                                        }
                                        else if (response.Length == 1 && response[0] == '0')
                                        {
                                            Print("You guessed wrong.");
                                        }

                                        // Pick a character
                                        key = GetInput("Choose a letter key on your keyboard:", true, true)[0];
                                        Print($"You chose '{key}'.");
                                        SendCharArray(theirIP, theirPort, CompileChoices(key));
                                        Print("Waiting for opponent to guess...");

                                        // Validate opponent's choice
                                        response = listener.ListenForCharArray();
                                        bool pointToOppponent = response[0] == key;
                                        Print($"The opponent chose '{response[0]}'. " +
                                            (pointToOppponent ? "They get 1 point." : " They guessed wrong!"));

                                        if (pointToOppponent)
                                        {
                                            theirPoints++;
                                        }
                                        SendCharArray(theirIP, theirPort, new char[1] { pointToOppponent ? '1' : '0' });

                                        Console.Clear();
                                        turn++;
                                    }

                                    Console.Clear();

                                    Print("Good game!");
                                    Print("");

                                    if (points > theirPoints)
                                    {
                                        Print("You won!");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                    else if (theirPoints > points)
                                    {
                                        Print("You lost.");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                    else
                                    {
                                        Print("You tied.");
                                        GetInput("(Press any key to continue)", true, true);
                                    }
                                }
                            }
                            
                            break;

                        // Display the rules
                        case '3':
                            PrintRules();

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
