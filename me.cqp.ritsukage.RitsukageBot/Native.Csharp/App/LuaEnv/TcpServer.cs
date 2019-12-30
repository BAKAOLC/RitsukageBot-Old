using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Native.Csharp.Sdk.Cqp.Enum;
using SimpleTCP;

namespace Native.Csharp.App.LuaEnv {
    internal class TcpServer {
        //需要发送的包列表
        private static readonly ConcurrentQueue<string> toSend = new ConcurrentQueue<string>();

        //每个包发送间隔时间（可以自己改）
        private const int packTime = 1000;

        private static readonly SimpleTcpServer server = new SimpleTcpServer();

        public static void Start() {
            try {
                server.StringEncoder = Encoding.UTF8;
                server.Start(23333);
                Common.CqApi.AddLoger(LogerLevel.Info, "tcp server", "tcp server started!");
                server.DataReceived += (sender, msg) => {
                    LuaEnv.RunLua($"message=[[{msg.MessageString.Replace("]", "] ")}]] ", "envent/ReceiveTcp.lua");
                };
            } catch (Exception e) {
                Common.CqApi.AddLoger(LogerLevel.Error, "tcp server", "tcp server start failed!\r\n" + e);
            }

            //消息发送队列
            Task.Run(
                () => {
                    while (true) {
                        while (toSend.TryDequeue(out string dataResult)) {
                            server.Broadcast(dataResult);
                            Task.Delay(packTime).Wait();
                        }
                        Task.Delay(200).Wait(); //等等，防止卡死
                    }
                }
            );
        }

        [LuaAPIFunction("apiTcpSend")]
        public static void Send(string msg) {
            try {
                //server.Broadcast(msg);
                toSend.Enqueue(msg);
            } catch (Exception e) {
                Common.CqApi.AddLoger(LogerLevel.Error, "tcp server", "tcp server send failed!\r\n" + e);
            }
        }
    }
}