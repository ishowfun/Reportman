using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Config;

namespace Reportman.Utils
{
    public static class Log
    {
        private static ILog _logger;
        public static event EventHandler<string> LogError;
        public static event EventHandler<string> LogInfo;
        public static event EventHandler<string> LogDebug;
        
        private static ILog Logger
        {
            get
            {
                if(_logger == null)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load("log4net.config");
                    var docElement = xmlDoc.DocumentElement;
                    XmlConfigurator.Configure(docElement);
                    _logger = LogManager.GetLogger("Reportman");
                    return _logger;
                }
                return _logger;
            }
        }
        public static void Error(string log)
        {
            Logger.Error(log);
            LogError?.Invoke(null, log);
        }

        public static void Info(string log)
        {
            Logger.Info(log);
            LogInfo?.Invoke(null, log);
        }

        public static void Debug(string log)
        {
            Logger.Debug(log);
            LogDebug?.Invoke(null, log);
        }
    }
}
