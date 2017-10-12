using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace LogReport
{
    public class OutPutHelper
    {
        private static readonly StringBuilder Logs = new StringBuilder();

        /// <summary>
        /// 是否自动，自动模式不输出到屏幕，挑选重点内容发送Email
        /// </summary>
        public static bool IsAuto { get; set; }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">信息</param>
        public static void Write(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                if (!IsAuto)
                {
                    Console.WriteLine(msg);
                }

                Logs.AppendLine(msg);
            }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        public static void RefreshLog()
        {
            if (Logs.Length > 0)
            {
                LogManager.GetCurrentClassLogger().Info(Logs.ToString);
                Logs.Clear();
            }
        }
    }
}
