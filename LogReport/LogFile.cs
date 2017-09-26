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

        private readonly StringBuilder _records = new StringBuilder();
        private readonly string _filePath;
        private readonly IEnumerable<Regex> _matchRegexs;

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
            var tempMatchs = new Dictionary<int, string>();

            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                var lineNum = 0;

                foreach (var readLine in File.ReadLines(_filePath))
                {
                    lineNum++;

                    foreach (var regex in _matchRegexs)
                    {
                        if (regex.Match(readLine).Success)
                        {
                            tempMatchs.Add(lineNum, readLine);
                            break;
                        }
                    }
                }

                if (tempMatchs.Any())
                {
                    _records.AppendLine($"来源机器:{Environment.MachineName}  来源文件:{_filePath}  共{tempMatchs.Count}条记录");

                    foreach (var match in tempMatchs)
                    {
                        _records.AppendLine($"【行数:{match.Key}】  {match.Value}");
                    }
                }
            }
        }

        /// <summary>
        /// 获取输出信息
        /// </summary>
        /// <returns></returns>
        public string GetOutput()
        {
            return _records.ToString();
        }

        #endregion
    }
}