using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using SupportBank.Banking;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            SetUpLogging();

            BankTeller bankTeller = new BankTeller();
            bankTeller.StartCommandLineInterface();
        }

        static void SetUpLogging()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"..\..\..\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            logger.Info("Program Started");
        }
    }
}
