using TouchSocket.Core;

namespace leaf.log
{

    class L
    {
        private LoggerGroup enginer;

        private static L? l { get; set; }

        public static LoggerGroup get()
        {
            if (l == null)
            {
                l = new L();
            }
            return l.enginer;
        }


        private L()
        {
            enginer = new LoggerGroup();
            if (config.Config.get<bool>("log:console:enable"))
            {
                var l1 = new ConsoleLogger();
                //l1.LogType = GetLogType(config.Config.get<string>("log:console:level","info"));
                enginer.AddLogger(l1);

            }
            if (config.Config.get<bool>("log.file.enable"))
            {
                var l2 = new FileLogger(config.Config.get<string>("log:file:dir", AppContext.BaseDirectory + "/data/"));
                //l2.LogType = GetLogType(config.Config.get<string>("log.file:level","info"));

                enginer.AddLogger(l2);
            }
        }

        public static void Info(string msg)
        {
            get().Info(msg);
        }

        public static void Error(string msg)
        {
            get().Error(msg);
        }

        public static void Warning(string msg)
        {
            get().Warning(msg);
        }

        public static void Debug(string msg)
        {
            get().Debug(msg);
        }


        private LogType GetLogType(string level)
        {
            switch (level)
            {
                case "none":
                    {
                        return LogType.None;
                    }
                case "trace":
                    {
                        return LogType.Trace;
                    }
                case "debug":
                    {
                        return LogType.Debug;
                    }
                case "info":
                    {
                        return LogType.Info;
                    }
                case "warning":
                    {
                        return LogType.Warning;
                    }
                case "error":
                    {
                        return LogType.Error;
                    }
                case "critical":
                    {
                        return LogType.Critical;
                    }
                default:
                    {
                        return LogType.None;
                    }
            }
        }

    }
}