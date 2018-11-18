using System;
using System.Net.Sockets;
using System.Text;

namespace Extras
{
    /// <summary>
    ///     An enumeration of the type of <see cref="Message"/>.
    /// </summary>
    public enum Message
    {
        FAILURE,
        CLIENT_CONNECT,
        SERVER_CONNECT
    }

    /// <summary>
    ///     Extension methods for <see cref="TcpListener"/>.
    /// </summary>
    public static class TcpListenerExtensions
    {
        /// <summary>
        ///     Wait, and return the response of a client's TcpClient
        ///     message that is of type <see cref="Message"/>.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static Message ListenForMessage(this TcpListener _)
        {
            Message retVal;
            bool success = false;

            // Accept a new TCP request and
            // reference it's network stream
            using (TcpClient client = _.AcceptTcpClient())
            using (NetworkStream ns = client.GetStream())
            {
                // Read bytes from the network stream
                byte[] response = new byte[1024];
                int bytesRead = ns.Read(response, 0, response.Length);

                // The use of the "out" keyword passes retVal by reference
                // which assigns it to the Message type the response parses out to
                // if the parse is successful
                success = Enum.TryParse<Message>(Encoding.ASCII.GetString(response, 0, bytesRead), out retVal);
            }

            if (success)
                return retVal;
            else
                return Message.FAILURE;
        }

        /// <summary>
        ///     Wait, and return the response of a client's TcpClient
        ///     message that is of type <see cref="char[]"/>.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static char[] ListenForCharArray(this TcpListener _)
        {
            char[] retVal = new char[0];

            // Accept a new TCP request and
            // reference it's network stream
            using (TcpClient client = _.AcceptTcpClient())
            using (NetworkStream ns = client.GetStream())
            {
                // Read bytes from the network stream
                byte[] response = new byte[1024];
                int bytesRead = ns.Read(response, 0, response.Length);

                // Return the value from the network stream
                retVal = Encoding.ASCII.GetString(response, 0, bytesRead).ToCharArray();
            }

            return retVal;
        }
    }

    /// <summary>
    ///     A collection of methods that are used to send
    ///     messages through a <see cref="TcpClient"/>.
    /// </summary>
    public static class NetworkHelpers
    {
        /// <summary>
        ///     Send a given <see cref="Message"/> to an IP/port.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        public static void SendMessage(string IP, int port, Message message)
        {
            using (TcpClient client = new TcpClient(IP, port))
            using (NetworkStream ns = client.GetStream())
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(message.ToString());
                ns.Write(byteMessage, 0, byteMessage.Length);
            }
        }

        /// <summary>
        ///     Send a given <see cref="char[]"/> to an IP/port.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        public static void SendCharArray(string IP, int port, char[] charArray)
        {
            using (TcpClient client = new TcpClient(IP, port))
            using (NetworkStream ns = client.GetStream())
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(charArray);
                ns.Write(byteMessage, 0, byteMessage.Length);
            }
        }
    }

    /// <summary>
    ///     Helper methods used within the logic
    ///     of the game (Packet Battle)
    /// </summary>
    public static class GameHelpers
    {
        /// <summary>
        ///     Returns a random list of 5 characters,
        ///     including the character that is passed into
        ///     this function.
        /// </summary>
        /// <param name="myChoice"></param>
        /// <returns></returns>
        public static char[] CompileChoices(char myChoice)
        {
            Random random = new Random();
            char[] retVal = new char[5] { myChoice, ' ', ' ', ' ', ' ' };
            char temp = ' ';

            for (int i = 0; i < retVal.Length; i++)
            {
                // If our character choice is not a letter
                if ((i == 0 && ((int)myChoice < 96 || (int)myChoice > 122))
                    || i > 0)
                {
                    // Assign temp to a character that doesn't
                    // already exist in retVal
                    while (retVal.Exists(temp))
                    {
                        temp = (char)random.Next(97, 123); // a-z
                    }

                    retVal[i] = temp;
                    temp = ' ';
                }
            }

            // Shuffle our array
            retVal.Shuffle();

            return retVal;
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="array"></param>
        private static void Shuffle(this char[] array)
        {
            // A C# implementation of the
            // Fisher-Yates shuffling algorithm
            Random rng = new Random();

            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                char value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        /// <summary>
        ///     An extension method that returns true if <paramref name="valueExists"/>
        ///     exists in <paramref name="array"/>.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="valueExists"></param>
        /// <returns></returns>
        public static bool Exists(this char[] array, char valueExists)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == valueExists)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Global functions that abstract low-level functions
    ///     that print text and/or take in input from the user.
    /// </summary>
    public static class GlobalHelpers
    {
        /// <summary>
        ///     Print information to the console.
        /// </summary>
        /// <param name="text"></param>
        public static void Print(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        ///     Print the game header.
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="cls"></param>
        public static void PrintGameHeader(string IP, bool cls = false)
        {
            if (cls)
                Console.Clear();

            Print("Welcome to Packet Battle");
            Print("------------------------");
            Print($"Your IP is: [{IP}]");
            Print("");
            Print("");
        }

        /// <summary>
        ///     Print the rules for the game.
        /// </summary>
        public static void PrintRules()
        {
            Console.Clear();

            Print("Rules");
            Print("-----");
            Print("The game of Packet Battle is a game of wits and luck.");
            Print("In this game, players take turns picking characters on their keyboard.");
            Print("The opposing player must choose the character the opponent picked.");
            Print("If the player chose the character the opponent picked, the player gets 1 point.");
            Print("The first player to 3 points is the winner.");
            Print("");
            Print("(Type any key to return)");

            GetInput(singleKey: true, intercept: true);
            Console.Clear();
        }

        /// <summary>
        ///     Get input from the user.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="singleKey"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public static string GetInput(string prompt = null,
            bool singleKey = false,
            bool intercept = false)
        {
            if (!string.IsNullOrEmpty(prompt))
                Print(prompt);

            // Support getting input for a single key
            // or an entire line of text
            if (singleKey)
                return Console.ReadKey(intercept).KeyChar.ToString();
            else
                return Console.ReadLine();
        }
    }
}