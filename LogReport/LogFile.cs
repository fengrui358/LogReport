using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogReport
{
    public class LogFile
    {
        #region 字段

        private readonly string _filePath;
        private IEnumerable<Regex> _matchRegexs;

        #endregion

        #region 构造

        public LogFile(string filePath, IEnumerable<Regex> matchRegexs)
        {
            _filePath = filePath;
            _matchRegexs = matchRegexs;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 读取并记录日志文件
        /// </summary>
        public void ReadAndRecord()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                
                
            }
        }

        /// <summary>
        /// 获取输出信息
        /// </summary>
        /// <returns></returns>
        public string GetOutput()
        {
            return string.Empty;
        }

        #endregion
    }
}