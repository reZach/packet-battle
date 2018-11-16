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

                success = Enum.TryParse(Encoding.ASCII.GetString(response, 0, bytesRead), out retVal);
            }

            if (success)
                return retVal;
            else
                return Message.FAILURE;
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

        public static string GetInput(string prompt = null, bool singleKey = false)
        {
            if (!string.IsNullOrEmpty(prompt))
                Print(prompt);

            if (singleKey)
                return Console.ReadKey().KeyChar.ToString();
            else
                return Console.ReadLine();
        }
    }
}