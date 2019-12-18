using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using Native.Csharp.Sdk.Cqp.Enum;
using NLua;
using System.Net;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv {
    class LuaEnv {
        private static volatile Lua luaField;
        private static readonly object luaLock = new object();

        public static Lua Lua {
            get {
                if (luaField == null) {
                    lock (luaLock) {
                        if (luaField == null) {
                            luaField = new Lua();
                            luaField.State.Encoding = Encoding.UTF8;
                            luaField.LoadCLRPackage();
                            luaField["handled"] = false; //处理标志
                            Initial(luaField);
                        }
                    }
                }
                return luaField;
            }
        }

        [LuaAPIFunction("cqSetGroupSpecialTitle")]
        public static int SetGroupSpecialTitle(long groupId, long qqId, string specialTitle, int time) {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupSpecialTitle(groupId, qqId, specialTitle, span);
        }

        [LuaAPIFunction("cqSetGroupAnonymousBanSpeak")]
        public static int SetGroupAnonymousBanSpeak(long groupId, string anonymous, int time) {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupAnonymousBanSpeak(groupId, anonymous, span);
        }

        [LuaAPIFunction("cqSetGroupBanSpeak")]
        public static int SetGroupBanSpeak(long groupId, long qqId, int time) {
            TimeSpan span = new TimeSpan(time / 60 / 60 / 24, time / 60 / 60 % 60, time / 60 % 60, time % 60);
            return Common.CqApi.SetGroupBanSpeak(groupId, qqId, span);
        }

        /// <summary>
        /// 初始化lua对象
        /// </summary>
        /// <param name="lua"></param>
        /// <returns></returns>
        public static void Initial(Lua lua) {
            //API查询遍历的类型列表
            //（可改写为从程序集检索，暂不修改）
            List<Type> searchTypes = new List<Type>
            {
                typeof(LuaApi), 
                typeof(LuaEnv), 
                typeof(XmlApi), 
                typeof(TcpServer), 
                typeof(Tools) 
            };
            foreach(Type t in searchTypes)
            {
                //类型中查找所有静态方法
                MethodInfo[] mis = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                foreach(MethodInfo mi in mis)
                {
                    //获取特性
                    LuaAPIFunctionAttribute lapiattr = mi.GetCustomAttribute<LuaAPIFunctionAttribute>();
                    if (lapiattr != null)
                    {
                        //获取自定义名称
                        string s = lapiattr.Name;
                        if (!string.IsNullOrEmpty(s))
                        {
                            lua.RegisterFunction(s, null, mi);
                        }
                        else
                        {
                            lua.RegisterFunction(mi.Name, null, mi);
                        }
                    }
                }
            }

            lua.DoFile(Common.AppDirectory + "lua/require/head.lua");
        }


        /// <summary>
        /// 运行lua文件
        /// </summary>
        /// <param name="code">提前运行的代码</param>
        /// <param name="file">文件路径（app/xxx.xxx.xx/lua/开头）</param>
        public static bool RunLua(string code, string file, ArrayList args = null) {
            //还没下载lua脚本，先不响应消息
            if (!File.Exists(Common.AppDirectory + "lua/require/head.lua")) return false;

            lock (luaLock) {
                var lua = Lua;
                try {
                    //maybe.... only once? by yys
//                    lua.State.Encoding = Encoding.UTF8;
//                    lua.LoadCLRPackage();
//                    lua["handled"] = false;//处理标志
//                    Initial(lua);
                    if (args != null)
                        for (int i = 0; i < args.Count; i += 2) {
                            lua[(string) args[i]] = args[i + 1];
                        }
                    lua.DoString(code);
                    if (file != "") lua.DoFile(Common.AppDirectory + "lua/" + file);
                    return (bool) lua["handled"];
                } catch (Exception e) {
                    Common.CqApi.AddLoger(LogerLevel.Error, "lua脚本错误", e.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// 在沙盒中运行代码，仅允许安全地运行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [LuaAPIFunction("apiSandBox")]
        public static string RunSandBox(string code) {
            using (var lua = new Lua()) {
                lock (luaLock) {
                    try {
                        //函数干脆一起注册了 部分也在new的时候操作了 by:yys canceled by:yys
                        lua.State.Encoding = Encoding.UTF8;
                        lua.RegisterFunction("apiGetPath", null, typeof(LuaApi).GetMethod("GetPath"));
                        //获取程序运行目录
                        lua.RegisterFunction("apiGetAsciiHex", null, typeof(LuaApi).GetMethod("GetAsciiHex"));
                        //获取字符串ascii编码的hex串
                        lua["lua_run_result_var"] = ""; //返回值所在的变量
                        lua.DoFile(Common.AppDirectory + "lua/require/sandbox/head.lua");
                        lua.DoString(code);
                        return lua["lua_run_result_var"].ToString();
                    } catch (Exception e) {
                        Common.CqApi.AddLoger(LogerLevel.Error, "沙盒lua脚本错误", e.ToString());
                        return "运行错误：" + e;
                    }
                }
            }
        }

        private static void UploadAsyncResult(int taskID, string retString = "")
        {
            MySQLHelper.Disconnect();
            try
            {
                MySQLHelper.Connect();
                var msg = MySQLHelper.DoSQLCommand("INSERT INTO 异步任务池 ( taskID, finish, result ) VALUES ( "
                    + taskID + ", 1, '" + retString + "'"
                    + " ) ON DUPLICATE KEY UPDATE finish = VALUES(finish), result = VALUES(result);");
                if (msg != "success")
                {
                    throw new Exception("MySQL " + msg);
                }
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "MySQL错误", e.ToString());
            }
            MySQLHelper.Disconnect();
        }
        
        [LuaAPIFunction]
        public static bool AsyncHttpGet(int taskID, string Url,
            string postDataStr = "", int timeout = 5000,
            string cookie = "", string referer = "")
        {
            
            void Response(IAsyncResult asynchronousResult)
            {
                var request = (HttpWebRequest)asynchronousResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));

                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                UploadAsyncResult(taskID, retString);
            }

            try
            {
                //请求前设置一下使用的安全协议类型 System.Net
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                    {
                        return true; //总是接受
                    };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                if (referer != "")
                    request.Referer = referer;

                request.BeginGetResponse(new AsyncCallback(Response), request);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "get错误", e.ToString());
                return false;
            }

            return true;
        }

    }
}