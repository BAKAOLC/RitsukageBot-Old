﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.Model;
using NLua;

namespace Native.Csharp.App.LuaEnv
{
    class LuaApi
    {
        /// <summary>
        /// 获取图片对象
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="length">高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap GetBitmap(int width, int length)
        {
            Bitmap bmp = new Bitmap(width, length);
            return bmp;
        }

        /// <summary>
        /// 摆放文字
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="text">文字内容</param>
        /// <param name="type">字体名称</param>
        /// <param name="size">字体大小</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutText(Bitmap bmp, int x, int y, string text, string type = "宋体", int size = 9,
            int r = 0, int g = 0, int b = 0)
        {
            Graphics pic = Graphics.FromImage(bmp);
            Font font = new Font(type, size);
            Color myColor = Color.FromArgb(r, g, b);
            SolidBrush myBrush = new SolidBrush(myColor);
            pic.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            pic.DrawString(text, font, myBrush, new PointF { X = x, Y = y });
            return bmp;
        }

        /// <summary>
        /// 填充矩形
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="xx">结束x坐标</param>
        /// <param name="yy">结束y坐标</param>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        /// <returns>图片对象</returns>
        public static Bitmap PutBlock(Bitmap bmp, int x, int y, int xx, int yy,
            int r = 0, int g = 0, int b = 0)
        {
            Color myColor = Color.FromArgb(r, g, b);
            //遍历矩形框内的各象素点
            for (int i = x; i <= xx; i++)
            {
                for (int j = y; j <= yy; j++)
                {
                    bmp.SetPixel(i, j, myColor);//设置当前象素点的颜色
                }
            }
            return bmp;
        }

        /// <summary>
        /// 摆放图片
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <param name="x">起始x坐标</param>
        /// <param name="y">起始y坐标</param>
        /// <param name="path">图片路径</param>
        /// <param name="xx">摆放图片宽度</param>
        /// <param name="yy">摆放图片高度</param>
        /// <returns>图片对象</returns>
        public static Bitmap SetImage(Bitmap bmp, int x, int y, string path, int xx = 0, int yy = 0)
        {
            if (!File.Exists(path))
                return bmp;
            Bitmap b = new Bitmap(path);
            Graphics pic = Graphics.FromImage(bmp);
            if (xx != 0 && yy != 0)
                pic.DrawImage(b, x, y, xx, yy);
            else
                pic.DrawImage(b, x, y);
            return bmp;
        }

        /// <summary>
        /// 保存并获取图片路径
        /// </summary>
        /// <param name="bmp">图片对象</param>
        /// <returns>图片路径</returns>
        public static string GetDir(Bitmap bmp)
        {
            string result = Tools.GetRandomString(32, true, false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            bmp.Save(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/image/" + result + ".luatemp", ImageFormat.Jpeg);
            return result + ".luatemp";
        }


        /// <summary>
        /// 获取程序运行目录
        /// </summary>
        /// <returns>主程序运行目录</returns>
        public static string GetPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 获取qq消息中图片的路径
        /// </summary>
        /// <param name="image">图片字符串，如“[CQ:image,file=123123]”</param>
        /// <returns>网址</returns>
        public static string GetImagePath(string image)
        {
            string fileName = Tools.Reg_get(image, "\\[CQ:image,file=(?<name>.*?)\\]", "name");//获取文件
            if (fileName == "")
                return "";
            return Common.CqApi.ReceiveImage(fileName);
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Url">文件网址</param>
        /// <param name="fileName">路径</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="referer">发起页面</param>
        /// <returns>下载结果</returns>
        public static bool HttpDownload(string Url, string fileName, int timeout = 5000, string referer = "")
        {
            //fileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data/" + fileName;
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = timeout;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

                if (referer != "")
                    request.Referer = referer;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.ContentLength < 1024 * 1024 * 20)//超过20M的文件不下载
                {
                    return Tools.SaveBinaryFile(response, fileName);
                }
                return false;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "下载文件错误", e.ToString());
            }
            return false;
        }

        /// <summary>
        /// GET 请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr = "", int timeout = 5000,
            string cookie = "", string referer = "")
        {
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

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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

                return retString;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "get错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// POST请求与获取结果
        /// </summary>
        public static string HttpPost(string Url, string postDataStr, int timeout = 5000,
            string cookie = "", string contentType = "application/x-www-form-urlencoded", string referer = "")
        {
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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = timeout;
                request.ContentType = contentType + "; charset=UTF-8";
                byte[] byteResquest = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = byteResquest.Length;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";
                if (cookie != "")
                    request.Headers.Add("cookie", cookie);

                if (referer != "")
                    request.Referer = referer;

                Stream stream = request.GetRequestStream();
                stream.Write(byteResquest, 0, byteResquest.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "post错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// 网络请求
        /// </summary>
        public static HttpWebRequest OHTTPCreateRequest(string Url, string postDataStr = "")
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
            request.Timeout = 5000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 Vivaldi/2.2.1388.37";

            return request;
        }

        public static bool OHTTPSetMethod(HttpWebRequest request, string method = "")
        {
            if (method != "")
            {
                request.Method = method;
                return true;
            }
            return false;
        }

        public static bool OHTTPSetTimeout(HttpWebRequest request, int timeout = 5000)
        {
            if (timeout > 0)
            {
                request.Timeout = timeout;
                return true;
            }
            return false;
        }

        public static bool OHTTPContentType(HttpWebRequest request, string contentType = "")
        {
            if (contentType != "")
            {
                request.ContentType = contentType;
                return true;
            }
            return false;
        }

        public static bool OHTTPUserAgent(HttpWebRequest request, string userAgent = "")
        {
            if (userAgent != "")
            {
                request.UserAgent = userAgent;
                return true;
            }
            return false;
        }

        public static bool OHTTPSetCookie(HttpWebRequest request, string cookie = "")
        {
            if (cookie != "")
            {
                request.Headers.Add("cookie", cookie);
                return true;
            }
            return false;
        }

        public static bool OHTTPSetReferer(HttpWebRequest request, string referer = "")
        {
            if (referer != "")
            {
                request.Referer = referer;
                return true;
            }
            return false;
        }

        public static bool OHTTPSetPOSTData(HttpWebRequest request, string postDataStr = "")
        {
            if (request.Method != "POST")
            {
                return false;
            }
            byte[] byteResquest = Encoding.UTF8.GetBytes(postDataStr);
            Stream stream = request.GetRequestStream();
            stream.Write(byteResquest, 0, byteResquest.Length);
            stream.Close();
            return true;
        }

        public static string OHTTPGetResponse(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));
                string retString = myStreamReader.ReadToEnd();
                if (request.Method == "GET")
                {
                    myStreamReader.Close();
                    myResponseStream.Close();
                }
                return retString;
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "web错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// 获取本地图片的base64结果，会转成jpeg
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Base64File(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception e)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "base64错误", e.ToString());
            }
            return "";
        }

        /// <summary>
        /// 获取图片宽度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetPictureWidth(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                return bmp.Width;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取图片高度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int GetPictureHeight(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                return bmp.Height;
            }
            catch
            {
                return 0;
            }
        }

        private static Dictionary<string, string> luaTemp = new Dictionary<string, string>();
        /// <summary>
        /// 把值存入ram
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        public static void SetVar(string n, string d)
        {
            luaTemp[n] = d;
        }
        /// <summary>
        /// 取出某值
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string GetVar(string n)
        {
            if (luaTemp.ContainsKey(n))
                return luaTemp[n];
            return "";
        }

        /// <summary>
        /// 获取字符串ascii编码的hex串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetAsciiHex(string str)
        {
            return BitConverter.ToString(Encoding.Default.GetBytes(str)).Replace("-", "");
        }

        /// <summary>
        /// 设置定时脚本运行间隔时间
        /// </summary>
        /// <param name="wait"></param>
        public static void SetTimerScriptWait(int wait) => TimerRun.luaWait = wait;

        public static string Execute(string shell)
        {
            string rText = "";

            void receive(object sender, DataReceivedEventArgs e)
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    rText += e.Data + "\n";
                }
            }

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.OutputDataReceived += receive;
                p.Start();
                StreamWriter w = p.StandardInput;
                p.BeginOutputReadLine();
                if (!String.IsNullOrEmpty(shell))
                {
                    w.WriteLine(shell);
                }
                w.Close();
                p.WaitForExit();
                p.Close();
                return rText;
            }
            catch (Exception ex)
            {
                Common.CqApi.AddLoger(LogerLevel.Error, "shell错误", ex.ToString());
            }

            return "";
        }

        /// <summary>
        /// MySQL
        /// </summary>
        public static bool MySQLSet(string host, int post, string username, string password)
        {
            return MySQLHelper.Set(host, post, username, password);
        }

        public static string CqCode_At(long qq) => Common.CqApi.CqCode_At(qq);
        //获取酷Q "At某人" 代码
        public static string CqCode_Emoji(int id) => Common.CqApi.CqCode_Emoji(id);
        //获取酷Q "emoji表情" 代码
        public static string CqCode_Face(int id) => Common.CqApi.CqCode_Face((Face)id);
        //获取酷Q "表情" 代码
        public static string CqCode_Shake() => Common.CqApi.CqCode_Shake();
        //获取酷Q "窗口抖动" 代码
        public static string CqCode_Trope(string str) => Common.CqApi.CqCode_Trope(str);
        //获取字符串的转义形式
        public static string CqCode_UnTrope(string str) => Common.CqApi.CqCode_UnTrope(str);
        //获取字符串的非转义形式
        public static string CqCode_ShareLink(string url, string title, string content, string imgUrl) => Common.CqApi.CqCode_ShareLink(url, title, content, imgUrl);
        //获取酷Q "链接分享" 代码
        public static string CqCode_ShareCard(string cardType, long id) => Common.CqApi.CqCode_ShareCard(cardType, id);
        //获取酷Q "名片分享" 代码
        public static string CqCode_ShareGPS(string site, string detail, double lat, double lon, int zoom) => Common.CqApi.CqCode_ShareGPS(site, detail, lat, lon, zoom);
        //获取酷Q "位置分享" 代码
        public static string CqCode_Anonymous(bool forced) => Common.CqApi.CqCode_Anonymous(forced);
        //获取酷Q "匿名" 代码
        public static string CqCode_Image(string path) => Common.CqApi.CqCode_Image(path);
        //获取酷Q "图片" 代码
        public static string CqCode_Music(long id, string type, bool newStyle) => Common.CqApi.CqCode_Music(id, type, newStyle);
        //获取酷Q "音乐" 代码
        public static string CqCode_MusciDIY(string url, string musicUrl, string title, string content, string imgUrl) => Common.CqApi.CqCode_MusciDIY(url, musicUrl, title, content, imgUrl);
        //获取酷Q "音乐自定义" 代码
        public static string CqCode_Record(string path) => Common.CqApi.CqCode_Record(path);
        //获取酷Q "语音" 代码
        public static int SendGroupMessage(long groupId, string message) => Common.CqApi.SendGroupMessage(groupId, message);
        //发送群消息
        public static int SendPrivateMessage(long qqId, string message) => Common.CqApi.SendPrivateMessage(qqId, message);
        //发送私聊消息
        public static int SendDiscussMessage(long discussId, string message) => Common.CqApi.SendDiscussMessage(discussId, message);
        //发送讨论组消息
        public static int SendPraise(long qqId, int count) => Common.CqApi.SendPraise(qqId, count);
        //发送赞
        public static int RepealMessage(int id) => Common.CqApi.RepealMessage(id);
        //撤回消息
        public static long GetLoginQQ() => Common.CqApi.GetLoginQQ();
        //取登录QQ
        public static string GetLoginNick() => Common.CqApi.GetLoginNick();
        //获取当前登录QQ的昵称
        public static string GetAppDirectory() => Common.AppDirectory;
        //取应用目录
        public static LuaTable GetQQInfo(LuaTable t, long q, bool a)
        {
            // 当地时区
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            QQ m;
            Common.CqApi.GetQQInfo(q, out m, a);
            t["Id"] = m.Id;
            t["Nick"] = m.Nick;
            t["Sex"] = (int)m.Sex;
            t["Age"] = m.Age;
            return t;
        }
        //获取用户信息
        public static LuaTable GetMemberInfo(LuaTable t, long g, long q, bool a)
        {
            // 当地时区
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            GroupMember m;
            Common.CqApi.GetMemberInfo(g, q, out m, a);
            t["Age"] = m.Age;
            t["Area"] = m.Area;
            t["BadRecord"] = m.BadRecord;
            t["Card"] = m.Card;
            t["JoiningTime"] = (long)(m.JoiningTime - startTime).TotalSeconds;
            t["LastDateTime"] = (long)(m.LastDateTime - startTime).TotalSeconds;
            t["Level"] = m.Level;
            t["Nick"] = m.Nick;
            t["PermitType"] = (int)m.PermitType;
            t["Sex"] = (int)m.Sex;
            t["SpecialTitle"] = m.SpecialTitle;
            t["SpecialTitleDurationTime"] = (long)(m.SpecialTitleDurationTime - startTime).TotalSeconds;
            return t;
        }
        //获取群成员信息
        public static LuaTable GetGroupList(LuaTable t)
        {
            List<Group> g;
            Common.CqApi.GetGroupList(out g);
            long index = 1;
            foreach (var group in g)
            {
                t[index] = group.Id;
                t[index + 1] = group.Name;
                index += 2;
            }
            return t;
        }
        //获取群列表
        public static LuaTable GetMemberList(LuaTable t, long g)
        {
            // 当地时区
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            List<GroupMember> m;
            Common.CqApi.GetMemberList(g, out m);
            long index = 1;
            foreach (var member in m)
            {
                t[index] = member.GroupId;
                t[index + 1] = member.QQId;
                t[index + 2] = member.Nick;
                t[index + 3] = member.Card;
                t[index + 4] = (int)member.Sex;
                t[index + 5] = member.Age;
                t[index + 6] = member.Area;
                t[index + 7] = (long)(member.JoiningTime - startTime).TotalSeconds;
                t[index + 8] = (long)(member.LastDateTime - startTime).TotalSeconds;
                t[index + 9] = member.Level;
                t[index + 10] = (int)member.PermitType;
                t[index + 11] = member.SpecialTitle;
                t[index + 12] = (long)(member.SpecialTitleDurationTime - startTime).TotalSeconds;
                t[index + 13] = member.BadRecord;
                t[index + 14] = member.CanModifiedCard;
                index += 15;
            }
            return t;
        }
        //获取群成员列表
        public static int AddLoger(int level, string type, string content) => Common.CqApi.AddLoger((LogerLevel)level, type, content);
        //添加日志
        public static int AddFatalError(string msg) => Common.CqApi.AddFatalError(msg);
        //添加致命错误提示
        public static int SetGroupWholeBanSpeak(long groupId, bool isOpen) => Common.CqApi.SetGroupWholeBanSpeak(groupId, isOpen);
        //置全群禁言
        public static int SetFriendAddRequest(string tag, int respond, string msg) => Common.CqApi.SetFriendAddRequest(tag, (ResponseType)respond, msg);
        //置好友添加请求
        public static int SetGroupAddRequest(string tag, int request, int respond, string msg) => Common.CqApi.SetGroupAddRequest(tag, (RequestType)request, (ResponseType)respond, msg);
        //置群添加请求
        public static int SetGroupMemberNewCard(long groupId, long qqId, string newNick) => Common.CqApi.SetGroupMemberNewCard(groupId, qqId, newNick);
        //置群成员名片
        public static int SetGroupManager(long groupId, long qqId, bool isCalcel) => Common.CqApi.SetGroupManager(groupId, qqId, isCalcel);
        //置群管理员
        public static int SetAnonymousStatus(long groupId, bool isOpen) => Common.CqApi.SetAnonymousStatus(groupId, isOpen);
        //置群匿名设置
        public static int SetGroupMemberRemove(long groupId, long qqId, bool notAccept) => Common.CqApi.SetGroupMemberRemove(groupId, qqId, notAccept);
        //置群员移除
        public static int SetDiscussExit(long discussId) => Common.CqApi.SetDiscussExit(discussId);
        //置讨论组退出
    }
}
