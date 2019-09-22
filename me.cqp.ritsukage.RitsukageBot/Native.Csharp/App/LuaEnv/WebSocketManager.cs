using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Native.Csharp.Sdk.Cqp.Enum;

namespace Native.Csharp.App.LuaEnv {
    internal static class WebSocketManager {
        private static readonly Dictionary<string, ClientWebSocket> sockets = new Dictionary<string, ClientWebSocket>();

        /// <summary>
        /// 创建一个WebSocket 其中name为socket的name
        /// </summary>
        /// <param name="name"></param> socket识别名字
        /// <param name="ip"></param> socket指向ip
        /// <param name="port"></param> socket指向端口
        /// <returns>socket识别名字 如果为空则已有改名字</returns>
        internal static string CreateSocket(string name, string ip, int port) {
            if (sockets.ContainsKey(name)) {
                return "";
            }
            var socket = new ClientWebSocket();
            socket.Options.Proxy = new WebProxy(ip, port);
            new Thread(
                () => {
                    while (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted) {
                        var seg = new ArraySegment<byte>();
                        var result = socket.ReceiveAsync(seg, CancellationToken.None).Result;
                        if (result.MessageType == WebSocketMessageType.Text) {
                            LuaEnv.RunLua(
                                $"message=[[{Encoding.UTF8.GetString(seg.Array ?? new byte[0]).Replace("]", "] ")}]]",
                                "event/ReceiveWebSocket.lua"
                            );
                        } else {
                            Common.CqApi.AddLoger(LogerLevel.Info, "WebSocket接受未知类型数据", $"WebSocket接受到{result.MessageType.ToString()}");
                        }
                    }
                }
            ).Start();
            sockets[name] = socket;
            return name;
        }

        internal static async void SendSocketMessage(string name, string msg) {
            await sockets[name].SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        internal static async void CloseSocket(string name, string desc = "default desc by yys from function invoked") {
            if (sockets.ContainsKey(name)) {
                await sockets[name].CloseAsync(WebSocketCloseStatus.Empty, desc, CancellationToken.None);
                sockets.Remove(name);
            }
        }
    }
}