using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.LuaEnv
{
    /// <summary>
    /// 标注该方法为一个Lua API函数。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LuaAPIFunctionAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置方法在Lua API中的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标注一个方法为LuaAPI。
        /// </summary>
        /// <param name="name">方法在Lua API中的名称，若保持默认值或设定为 <see cref="null"/> 则以函数名作为Lua API函数名称。</param>
        public LuaAPIFunctionAttribute(string name = null)
        {
            Name = name;
        }
    }
}
