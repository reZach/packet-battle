using System;
using System.Net.Sockets;
using System.Text;

namespace Extras
{
    public enum Message
    {
        FAILURE,
        CLIENT_CONNECT,
        SERVER_CONNECT
    }

    public static class TcpListenerExtensions
    {
        public static Message ListenForMessage(this TcpListener _)
        {
            Message retVal;
            bool success = false;

            using (TcpClient client = _.AcceptTcpClient())
            using (NetworkStream ns = client.GetStream())
            {
                byte[] response = new byte[1024];
                int bytesRead = ns.Read(response, 0, response.Length);

                success = Enum.TryParse<Message>(Encoding.ASCII.GetString(response, 0, bytesRead), out retVal);
            }

            if (success)
                return retVal;
            else
                return Message.FAILURE;
        }

        public static char[] ListenForCharArray(this TcpListener _)
        {
            char[] retVal = new char[0];

            using (TcpClient client = _.AcceptTcpClient())
            using (NetworkStream ns = client.GetStream())
            {
                byte[] response = new byte[1024];
                int bytesRead = ns.Read(response, 0, response.Length);

                retVal = Encoding.ASCII.GetString(response, 0, bytesRead).ToCharArray();
            }

            return retVal;
        }
    }

    public static class NetworkHelpers
    {
        public static void SendMessage(string IP, int port, Message message)
        {
            using (TcpClient client = new TcpClient(IP, port))
            using (NetworkStream ns = client.GetStream())
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(message.ToString());
                ns.Write(byteMessage, 0, byteMessage.Length);
            }
        }

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

    public static class GameHelpers
    {
        public static char[] CompileChoices(char myChoice)
        {
            Random random = new Random();
            char[] retVal = new char[5] { myChoice, ' ', ' ', ' ', ' ' };
            char temp = ' ';

            for (int i = 0; i < retVal.Length; i++)
            {
                if ((i == 0 && ((int)myChoice < 96 || (int)myChoice > 122))
                    || i > 0)
                {
                    while (retVal.Exists(temp))
                    {
                        temp = (char)random.Next(97, 123); // a-z
                    }

                    retVal[i] = temp;
                    temp = ' ';
                }
            }

            retVal.Shuffle();

            return retVal;
        }

        private static void Shuffle(this char[] array)
        {
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

    public static class GlobalHelpers
    {
        public static void Print(string text)
        {
            Console.WriteLine(text);
        }

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

        public static string GetInput(string prompt = null,
            bool singleKey = false,
            bool intercept = false)
        {
            if (!string.IsNullOrEmpty(prompt))
                Print(prompt);

            if (singleKey)
                return Console.ReadKey(intercept).KeyChar.ToString();
            else
                return Console.ReadLine();
        }
    }
}