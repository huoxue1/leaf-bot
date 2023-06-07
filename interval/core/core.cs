using leaf.bot;
using leaf.core.errors;
using leaf.eve;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using leaf.log;

namespace leaf.core
{


    public class Core
    {

        private static Hashtable plugins = new Hashtable();

        public static List<BaseMatcher> matcherList = new List<BaseMatcher>();

        public static void LoadPlugin( Plugin plugin)
        {
            plugins.Add(plugin.GetName(), plugin);
            Matcher[] matchers = plugin.RegistrMatcher();

            foreach (var matcher in matchers)
            {
                var baseMatchers = Utils.parseMatcher(plugin.GetName(),matcher);
                
                L.Info($"success load plugin {plugin.GetName()},find {baseMatchers.Count} matcher");
                matcherList.AddRange(baseMatchers);
            }
        }

        public static void LoadPlugin(Plugin[] plugins)
        {
            foreach (var plugin in plugins)
            {
                LoadPlugin(plugin);
            }
        }


        public static void Run(driver.Driver driver,adapter.Adapter adapter)
        {
            driver.initDriver(adapter,matcherList);
            driver.run();
        }
    }

    public class BaseMatcher
    {
        public EventType MatcherType { get; set; }
        public LinkedList<Delegate> rules { get; set; }

        public bool isBlock { get; set; }

        public int priority { get; set; }

        public Hashtable MetaData { get; set; }

        public BaseMatcher(EventType MatcherType, LinkedList<Delegate> rules, int priority, bool isBlock, Hashtable MetaData)
        {
            this.MatcherType = MatcherType;
            this.rules = rules;
            this.priority = priority;
            this.isBlock = isBlock;
            this.MetaData = MetaData;
        }


    }


    public class Utils
    {


        public static LinkedList<BaseMatcher> parseMatcher(string pluginName,Matcher matcher)
        {
            var result = new LinkedList<BaseMatcher>();
            MethodInfo[] methods = matcher.GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is OnMessage)
                    {
                        OnMessage onMessage = (OnMessage)attribute;
                        LinkedList<Delegate> rules = onMessage.rules;
                        int priority = onMessage.priority;
                        bool isBlock = onMessage.isBlock;
                        Hashtable metaData = new Hashtable();
                        metaData.Add("method", method);
                        metaData.Add("matcher", matcher);
                        metaData.Add("message_type", onMessage.messageType);
                        metaData.Add("plugin_name", pluginName);
                        metaData.Add("method_name",method.Name);
                        BaseMatcher baseMatcher = new BaseMatcher(EventType.Message, rules, priority, isBlock, metaData);
                        result.AddLast(baseMatcher);
                        break;
                    }
                    if (attribute is OnCommand)
                    {
                        OnCommand onCommand = (OnCommand)attribute;
                        LinkedList<Delegate> rules = onCommand.rules;
                        int priority = onCommand.priority;
                        bool isBlock = onCommand.isBlock;
                        Hashtable metaData = new Hashtable();
                        metaData.Add("method", method);
                        metaData.Add("matcher", matcher);
                        metaData.Add("command", onCommand.command);
                        metaData.Add("alias", onCommand.alias);
                        metaData.Add("message_type", onCommand.messageType);
                        metaData.Add("plugin_name", pluginName);
                        metaData.Add("method_name",method.Name);
                        BaseMatcher baseMatcher = new BaseMatcher(EventType.Message, rules, priority, isBlock, metaData);
                        result.AddLast(baseMatcher);
                        break;
                    }
                }
            }
            foreach (var item in result)
                {
                    L.Debug($"success load matcher  {((MethodInfo)item.MetaData["method"]!).Name}");
                }


            return result;
        }


        public static object[] parseParam(Event e, Bot bot, Hashtable state, ParameterInfo[] parameterInfos)
        {
            object[] objects = new object[parameterInfos.Length];
            int i = 0;
            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                if (parameterInfo.ParameterType == typeof(Event) || parameterInfo.ParameterType.GetInterface(nameof(Event)) != null)
                {
                  
                        objects[i] = e;
                    
                }

                if (parameterInfo.ParameterType == typeof(Bot) || parameterInfo.ParameterType.GetInterface(nameof(Bot)) != null)
                {
                    
                        objects[i] = bot;
                   
                }

                if (parameterInfo.ParameterType == typeof(Hashtable))
                {
                    // 判断State类型相同
                   
                        objects[i] = state;
                   
                }
                i++;
            }
            return objects;
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class OnMessage : Attribute
    {
        public OnMessage() { }

        public OnMessage(string messageType, LinkedList<Delegate> rules, int priority, bool isBlock)
        {
            this.messageType = messageType;
            this.rules = rules;
            this.priority = priority;
            this.isBlock = isBlock;
        }

        public OnMessage(string messageType)
        {
            this.messageType = messageType;
        }

        public string messageType { get; set; } = "";

        public LinkedList<Delegate> rules { get; set; } = new LinkedList<Delegate>();

        public int priority { get; set; } = 0;

        public bool isBlock { get; set; } = false;



    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnCommand : OnMessage
    {


        public string command { get; set; } = "";

        public string[] alias { get; set; }



        public OnCommand(string command, string messageType = "", LinkedList<Delegate>? rules = null, int priority = 0, bool isBlock = false, string[]? alias = null)
        {
            this.command = command;
            this.rules = rules == null ? new LinkedList<Delegate>() : rules!;
            this.messageType = messageType;
            this.priority = priority;
            this.isBlock = isBlock;
            this.alias = alias == null ? new string[0] : alias!;
        }


        public bool commandHandle( Event @event)
        {
            if (@event.getEventType() != EventType.Message)
            {
                return false;
            }
            string start = config.Config.get("command_start", "");
            string[] messages = @event.GetMessage().ExtractText().Split(' ');
            if (messages.Length == 0)
            {
                return false;
            }
            if (start + command == messages[0])
            {
                return true;
            }

            foreach (string c in alias)
            {
                if (start + c == messages[0])
                {
                    return true;
                }
            }
            return false;
        }


        public OnCommand(string command)
        {
            this.command = command;
            this.alias = new string[0];

            this.rules.AddFirst(new LinkedListNode<Delegate>(this.commandHandle));
        }
    }
}
