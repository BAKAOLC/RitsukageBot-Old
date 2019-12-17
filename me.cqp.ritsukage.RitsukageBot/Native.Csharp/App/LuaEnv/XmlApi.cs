﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Native.Csharp.App.LuaEnv
{
    class XmlApi
    {
        public const string rootNode = "Categories";
        public const string keyNode = "msginfo";
        public const string keyName = "msg";
        public const string valueName = "ans";

        public static string path = Common.AppDirectory + "xml/";

        [LuaAPIFunction("apiXmlSet")]
        public static void set(string group, string msg, string str)
        {
            del(group, msg);
            insert(group, msg, str);
        }

        /// <summary>
        /// 随机获取一条结果
        /// </summary>
        /// <param name="group"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [LuaAPIFunction("apiXmlReplayGet")]
        public static string replay_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            Random ran = new Random(DateTime.Now.Millisecond);
            int RandKey;
            string ansall = "";
            var element = from ee in root.Elements()
                          where msg.IndexOf(ee.Element(keyName).Value) != -1
                          select ee;
            XElement[] result = element.ToArray();
            if (result.Count() > 0)
            {
                RandKey = ran.Next(0, result.Count());
                ansall = result[RandKey].Element(valueName).Value;
            }
            return ansall;
        }

        [LuaAPIFunction("apiXmlGet")]
        public static string xml_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(valueName).Value;
            return ansall;
        }

        [LuaAPIFunction("apiXmlRow")]
        public static string xml_row(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(valueName).Value == msg
                          select ee;
            if (element.Count() > 0)
                ansall = element.First().Element(keyName).Value;
            return ansall;
        }

        [LuaAPIFunction("apiXmlListGet")]
        public static string list_get(string group, string msg)
        {
            dircheck(group);
            XElement root = XElement.Load(path + group + ".xml");
            string ansall = "";
            var element = from ee in root.Elements()
                          where ee.Element(keyName).Value == msg
                          select ee;
            XElement[] result = element.ToArray();
            foreach (XElement mm in result)
                ansall = ansall + mm.Element(valueName).Value + "\r\n";
            ansall = ansall + "一共有" + element.Count() + "条回复";
            return ansall;
        }

        [LuaAPIFunction("apiXmlDelete")]
        public static void del(string group, string msg)
        {
            dircheck(group);
            string gg = group;
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg
                          select ee;
            if (element.Count() > 0)
                element.Remove();
            root.Save(path + group + ".xml");
        }

        [LuaAPIFunction("apiXmlRemove")]
        public static void remove(string group, string msg, string ans)
        {
            dircheck(group);
            string gg = @group;
            XElement root = XElement.Load(path + group + ".xml");

            var element = from ee in root.Elements()
                          where (string)ee.Element(keyName) == msg && (string)ee.Element(valueName) == ans
                          select ee;
            if (element.Count() > 0)
                element.First().Remove();
            root.Save(path + group + ".xml");
        }


        [LuaAPIFunction("apiXmlInsert")]
        public static void insert(string group, string msg, string ans)
        {
            if (msg.IndexOf("\r\n") < 0 & msg != "")
            {
                dircheck(group);
                XElement root = XElement.Load(path + group + ".xml");

                XElement read = root.Element(keyNode);

                if (read == null)
                {
                    root.Add(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                else
                {
                    read.AddBeforeSelf(new XElement(keyNode,
                      new XElement(keyName, msg),
                      new XElement(valueName, ans)
                      ));
                }
                root.Save(path + group + ".xml");
            }
        }

        public static void dircheck(string group)
        {
            if (!File.Exists(path + group + ".xml"))
            {
                XElement root = new XElement(rootNode,
                    new XElement(keyNode,
                        new XElement(keyName, "初始问题"),
                        new XElement(valueName, "初始回答")
                        )
                   );
                root.Save(path + group + ".xml");
            }
        }
    }
}
