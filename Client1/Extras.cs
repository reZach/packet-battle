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

        public static string ListenForString(this TcpListener _)
        {
            string retVal = string.Empty;

            using (TcpClient client = _.AcceptTcpClient())
            using (NetworkStream ns = client.GetStream())
            {
                byte[] response = new byte[1024];
                int bytesRead = ns.Read(response, 0, response.Length);

                retVal = Encoding.ASCII.GetString(response, 0, bytesRead);
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

        public static void SendString(string IP, int port, string String)
        {
            using (TcpClient client = new TcpClient(IP, port))
            using (NetworkStream ns = client.GetStream())
            {
                byte[] byteMessage = Encoding.ASCII.GetBytes(String);

                ns.Write(byteMessage, 0, byteMessage.Length);
            }
        }
    }

    public static class GameHelpers
    {
        public static char[] CompileDecision(char opponentsChoice)
        {
            Random random = new Random();
            char[] retVal = new char[5] { opponentsChoice, ' ', ' ', ' ', ' ' };

            for (int i = 0; i < retVal.Length; i++)
            {
                if ((i == 0 && ((int)opponentsChoice < 96 || (int)opponentsChoice > 122))
                    || i > 0)
                {
                    retVal[i] = (char)random.Next(97, 123); // a-z
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