using NTWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    class Program {
        static void Main(string[] args) {
            DevMode.SetDevMode();

            //CpuTest();
            WebSocketTest();

            Console.ReadKey();
        }
        
        static void CpuTest() {
            foreach (var cpu in HardwareProviders.CPU.Cpu.Discover()) {
                Console.WriteLine(cpu.PackageTemperature?.ToString());
                foreach (var item in cpu.CoreTemperatures) {
                    Console.WriteLine(item.Value?.ToString());
                }
            }
        }

        static void WebSocketTest() {
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:8181");
            server.Start(socket => {
                socket.OnOpen = () => {
                    Console.WriteLine("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () => {
                    Console.WriteLine("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message => {
                    Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                };
            });


            var input = Console.ReadLine();
            while (input != "exit") {
                foreach (var socket in allSockets.ToList()) {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }
        }
    }
}
