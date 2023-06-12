using leaf.bot;
using leaf.core.errors;
using leaf.eve;
using System;
using System.Collections;
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

        public static void LoadPlugin(Plugin plugin)
        {
            plugins.Add(plugin.GetName(), plugin);
            Matcher[] matchers = plugin.RegistrMatcher();

            foreach (var matcher in matchers)
            {
                var baseMatchers = Utils.parseMatcher(plugin.GetName(), matcher);

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


        public static void Run(driver.Driver driver, adapter.Adapter adapter)
        {
            driver.initDriver(adapter, matcherList);
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


        public static LinkedList<BaseMatcher> parseMatcher(string pluginName, Matcher matcher)
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
                        IEnumerable<R> ruleAttributes = method.GetCustomAttributes<R>();
                        OnMessage onMessage = (OnMessage)attribute;
                        int priority = onMessage.priority;
                        bool isBlock = onMessage.isBlock;
                        Hashtable metaData = new Hashtable
                        {
                            { "method", method },
                            { "matcher", matcher },
                            { "message_type", onMessage.messageType },
                            { "plugin_name", pluginName },
                            { "method_name", method.Name }
                        };
                        var rules = new LinkedList<Delegate>();
                        foreach (var item in ruleAttributes)
                        {
                            rules.AddLast(item.Delegate);
                        }
                        BaseMatcher baseMatcher = new BaseMatcher(EventType.Message, rules, priority, isBlock, metaData);
                        result.AddLast(baseMatcher);
                        break;
                    }
                    if (attribute is OnCommand)
                    {
                        OnCommand onCommand = (OnCommand)attribute;

                        int priority = onCommand.priority;
                        bool isBlock = onCommand.isBlock;
                        Hashtable metaData = new Hashtable
                        {
                            { "method", method },
                            { "matcher", matcher },
                            { "command", onCommand.command },
                            { "alias", onCommand.alias },
                            { "message_type", onCommand.messageType },
                            { "plugin_name", pluginName },
                            { "method_name", method.Name }
                        };
                        IEnumerable<R> ruleAttributes = method.GetCustomAttributes<R>();
                        var rules = new LinkedList<Delegate>();
                        foreach (var item in ruleAttributes)
                        {
                            rules.AddLast(item.Delegate);
                        }
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

        public OnMessage(string messageType, int priority, bool isBlock)
        {
            this.messageType = messageType;

            this.priority = priority;
            this.isBlock = isBlock;
        }

        public OnMessage(string messageType)
        {
            this.messageType = messageType;
        }

        public string messageType { get; set; } = "";





        public int priority { get; set; } = 0;

        public bool isBlock { get; set; } = false;



    }


    [AttributeUsage(AttributeTargets.Method)]
    public class R : Attribute
    {
        Type Type { get; set; }

        string method { get; set; }


        public Delegate Delegate { get; }
        public R(Type classType, Type delegateType,string method)
            {
            this.Type = classType;
            this.method = method;
            this.Delegate = Delegate.CreateDelegate(delegateType,classType.GetMethod(method)!);
            }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class OnCommand : OnMessage
    {


        public string command { get; set; } = "";

        public string[] alias { get; set; }



        public OnCommand(string command, string messageType = "", int priority = 0, bool isBlock = false, string[]? alias = null)
        {
            this.command = command;

            this.messageType = messageType;
            this.priority = priority;
            this.isBlock = isBlock;
            this.alias = alias == null ? new string[0] : alias!;
        }


        public bool commandHandle(Event @event)
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


        }
    }



    public static class TableExtensions
    {

        public static Hashtable ToTable<TKey, TValue>(this (object Key, object Value)[] items) where TKey : notnull
        {
            var dictionary = new Hashtable();
            foreach (var item in items)
            {
                dictionary[item.Key] = item.Value;
            }
            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this (TKey Key, TValue Value)[] items)
        where TKey : notnull
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in items)
            {
                dictionary[item.Key] = item.Value;
            }
            return dictionary;
        }

        /// <summary>
        /// 将 Hashtable 转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="hashtable">要转换的 Hashtable</param>
        /// <returns>转换后的对象</returns>
        public static T ToObject<T>(this Hashtable hashtable) where T : new()
        {
            if (hashtable == null)
            {
                throw new ArgumentNullException(nameof(hashtable));
            }

            var obj = new T();
            var properties = typeof(T).GetProperties();

            foreach (DictionaryEntry entry in hashtable)
            {
                var key = entry.Key.ToString();
                var value = entry.Value;

                var property = Array.Find(properties, prop => prop.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (property != null && property.CanWrite && value != null)
                {
                    var propertyType = property.PropertyType;

                    if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = propertyType.GetGenericArguments()[0];
                    }

                    try
                    {
                        var convertedValue = Convert.ChangeType(value, propertyType);
                        property.SetValue(obj, convertedValue);
                    }
                    catch (InvalidCastException)
                    {
                        // Ignore invalid cast exception and skip setting the property
                    }
                }
            }

            return obj;
        }

    }
}
