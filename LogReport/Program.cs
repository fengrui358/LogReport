using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace LogReport
{
    class Program
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private static string _logDirectory;
        private const string MatchRegexSeparate = ";;;";
        private static List<string> _matchRegexList = new List<string>();
        private static readonly List<Regex> Regexes = new List<Regex>();

        static void Main(string[] args)
        {
            #region 读取args参数

            //从参数中读取
            if (args != null && args.Any())
            {
                var argsList = args.ToList();

                var logDirectoryIndex = argsList.IndexOf("-LogDirectory");
                if (logDirectoryIndex >= 0 && argsList.Count >= logDirectoryIndex + 1)
                {
                    _logDirectory = argsList[logDirectoryIndex + 1];
                }

                var isAuto = argsList.IndexOf("-Auto");
                if (isAuto >= 0)
                {
                    OutPutHelper.IsAuto = true;
                }

                var matchRegexsIndex = argsList.IndexOf("-MatchRegexs");
                if (matchRegexsIndex >= 0 && argsList.Count >= matchRegexsIndex + 1)
                {
                    var tempSplit = argsList[matchRegexsIndex + 1].Split(new[] {MatchRegexSeparate},
                        StringSplitOptions.RemoveEmptyEntries);
                    _matchRegexList.AddRange(tempSplit.ToList());
                }
            }

            #endregion

            #region 读取配置文件

            //如果没有通过参数设置尝试读取配置文件
            if (string.IsNullOrEmpty(_logDirectory))
            {
                _logDirectory = ConfigurationManager.AppSettings["LogDirectory"];

                if (string.IsNullOrEmpty(_logDirectory))
                {
                    OutPutHelper.Write("LogDirectory不能为空。");
                    WaitExit();
                    return;
                }

                if (!Directory.Exists(_logDirectory))
                {
                    OutPutHelper.Write($"路径{_logDirectory}不存在。");
                    WaitExit();
                    return;
                }
            }

            var configMatchRegexs = ConfigurationManager.AppSettings["MatchRegexs"];
            if (configMatchRegexs != null)
            {
                var tempSplit = configMatchRegexs.Split(new[] { MatchRegexSeparate },
                    StringSplitOptions.RemoveEmptyEntries);
                _matchRegexList.AddRange(tempSplit.ToList());
            }

            //正则表达式去重
            _matchRegexList = _matchRegexList.Distinct().Where(s => !string.IsNullOrEmpty(s)).ToList();

            if (!_matchRegexList.Any())
            {
                //TODO:加入日志的默认匹配规则，Error和Fatal
                _matchRegexList.Add("[ERROR]");
                _matchRegexList.Add("[FATAL]");
            }

            foreach (var regex in _matchRegexList)
            {
                Regexes.Add(new Regex(regex, RegexOptions.Singleline));
            }

            #endregion

            var logFiles = new List<LogFile>();
            var logFilePaths = Directory.GetFiles(_logDirectory, "*.log", SearchOption.AllDirectories);
            foreach (var logFilePath in logFilePaths)
            {
                logFiles.Add(new LogFile(logFilePath, Regexes));
            }

            Parallel.ForEach(logFiles, file => file.ReadAndRecord());

            foreach (var logFile in logFiles)
            {
                OutPutHelper.Write(logFile.GetOutput());
            }

            OutPutHelper.RefreshLog();
            WaitExit();
        }

        private static void WaitExit()
        {
            if (!OutPutHelper.IsAuto)
            {
                Console.ReadKey();
            }
        }
    }
}
