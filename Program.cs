using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft;
using client;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;

namespace client
{
    class Program
    {
        private static UdpClient udpServer = new UdpClient(Helper.UdpPort);

        public static void SendData(string userMessage)
        {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action", ServerActions.SendToLeader);
            var message = new Message(){
                id = Guid.NewGuid().ToString(),
                value = userMessage
                
            };
            dataToSend.Add("data", message);
            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
            byte[] data = Encoding.UTF8.GetBytes(serializedMessage);
            udpServer.Send(data, data.Length, Helper.ServerIP, Helper.ServerPort);
        }

        public static void SendData(ServerActions action)
        {
            var dataToSend = new Dictionary<string, object>();
            dataToSend.Add("action", action);
            dataToSend.Add("address", new{
                    Ip = Helper.GetLocalIPAddress(),
                    Port = ((IPEndPoint)udpServer.Client.LocalEndPoint).Port.ToString()
            });

            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(dataToSend);
            byte[] data = Encoding.UTF8.GetBytes(serializedMessage);
            udpServer.Send(data, data.Length, Helper.ServerIP, Helper.ServerPort);
        }

        public static void ListenUDP()
        {
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(
                    IPAddress.Any,
                    Helper.UdpPort
                    );
                Byte[] receiveBytes = udpServer.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                JObject data = new JObject();
                data = JObject.Parse(returnData);
                // Console.WriteLine(data);
                Enum.TryParse(data["action"].ToString(), out ServerActions action);
                // Console.WriteLine(data);
                switch(action)
                {
                    case ServerActions.GetDataFromLeader:
                        var dataFromServer = data["data"];
                        Console.WriteLine(dataFromServer);
                        break;

                    
                }
            }
        }
        static void Main(string[] args)
        {
            var listenUDP = new Thread(() => ListenUDP());
            listenUDP.Start();

            while(true)
            {
                int action = 0;
                Console.WriteLine("Select action:\n0 - Send Data;\n1 - Get Data;\n");
                // SendData(ServerActions.GetDataFromLeader);
                action = int.Parse( Console.ReadLine() );
                switch(action)
                {
                    case 0:
                        Console.WriteLine("Input message:");
                        SendData(Console.ReadLine());
                        break;
                    case 1:
                        SendData(ServerActions.GetDataFromLeader);
                        break;
                }

            }
        }
    }
}
