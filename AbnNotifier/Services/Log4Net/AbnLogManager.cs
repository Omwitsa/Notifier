using System;
using log4net;
using System.IO;
using System.Reflection;
using log4net.Config;

namespace AbnNotifier.Services.Log4Net
{
    public class AbnLogManager
    {
        private ILog _logger;
        public ILog Logger(Type type)
        {
            var cDirectory = Directory.GetCurrentDirectory();
            var log4NetConfigFilePath = Path.Combine(cDirectory, "Log4Net", "log4net.config");
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(logRepository, new FileInfo(log4NetConfigFilePath));
            _logger = LogManager.GetLogger(type);

            return _logger;
        }
    }



}