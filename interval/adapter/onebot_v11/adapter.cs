using leaf.driver;

using leaf.eve;
using leaf.adapter.onebot_v11.eve;
using leaf.log;
using Newtonsoft.Json.Linq;
using leaf.core;
using System.Reflection;
using System.Collections;
using TouchSocket.Core;

namespace leaf.adapter.onebot_v11
{
    class OnebotV11Adapter : Adapter
    {

        private Driver? d = null;



        private void handleMessage(MessageEvent e)
        {
            Bot bot = (Bot)d!.GetBot(e.self_id.ToString());
            var matcherList = d!.getMatcherList(EventType.Message);
            foreach (var matcher in matcherList)
            {
                L.Debug($"start handle the matcher {matcher.MetaData["plugin_name"]} --> {matcher.MetaData["method_name"]}");
                var state = new Hashtable();
                if ((string)matcher.MetaData["message_type"]! != "" && (string)matcher.MetaData["message_type"]! != e.message_type)
                {
                    L.Debug($"the matcher {matcher.MetaData["method_name"]} message_type mismatch");
                    continue;
                }
                bool ruleSuccess = true;
                foreach (var rule in matcher.rules)
                {
                    MethodInfo method = rule.GetMethodInfo();
                    ruleSuccess = (bool)method.Invoke(rule.Target, Utils.parseParam(e, bot, state, method.GetParameters()))!;
                    if (!ruleSuccess)
                    {
                        break;
                    }
                }
                if (!ruleSuccess)
                {
                    break;
                }

                var handleMethod = matcher.MetaData["method"] as MethodInfo;
                L.Debug($"start handle the matcher {handleMethod!.Name}");
                handleMethod!.Invoke(matcher.MetaData["matcher"], Utils.parseParam(e, bot, state, handleMethod!.GetParameters()));
                if (matcher.isBlock)
                {
                    break;
                }
            }
        }

        private void handleMeta(MetaEvent e)
        {

        }


        List<Hashtable> taskList = new List<Hashtable>();

        public static string AdapterName = "OneBot_V11";

        public string Name => AdapterName;

        public bool handleFuture(Event e){
            bot.Bot bot = (Bot)d!.GetBot(e.GetSelfId());
            var state = new Hashtable();
            foreach (var task in taskList)
            {
                var rules = task["rules"] as Delegate[];
                var taskEvent = task["task"] as TaskCompletionSource<Event>;
                bool ruleSuccess = true;
                foreach (var rule in rules!)
                {
                    MethodInfo method = rule.GetMethodInfo();
                    ruleSuccess = (bool)method.Invoke(rule.Target, Utils.parseParam(e, bot, state, method.GetParameters()))!;
                    if (!ruleSuccess)
                    {
                        break;
                    }
                }
                if (!ruleSuccess)
                {
                    continue;
                }
                taskEvent!.SetResult(e);
                return true;
            }
            return false;
        }

        public async Task<Event?> GetFutureEvent(params Delegate[] rules){
            var tsc = new TaskCompletionSource<Event>();
            var data = new Hashtable();
            data["task"] = tsc;
            data["rules"] = rules;
            taskList.Add(data);
            tsc.Task.GetAwaiter().OnCompleted(() => {
                taskList.Remove(data);
            });
            try
            {
                await tsc.Task.WaitAsync(TimeSpan.FromSeconds(60));
            }catch (TimeoutException e)
            {
                return null;
            }
            
            return tsc.Task.Result;
        }





        void Adapter.handleData(string data)
        {

            var e = AdapterHandler.fromJson(data);
            if (e == null)
            {
                return;
            }
            e.setDriver(d!);
            if (handleFuture(e))
            {
                return;
            }
            switch (e.getEventType())
            {
                case EventType.Message:
                    {
                        handleMessage((MessageEvent)e);
                        break;
                    }
                case EventType.Meta:
                    {
                        handleMeta((MetaEvent)e);
                        break;
                    }

            }
        }

        void Adapter.setDriver(Driver driver)
        {
            this.d = driver;
        }

        bot.Bot Adapter.createBot(string user_id, string conn_id, Hashtable state)
        {
            return new Bot(ref this.d!, this, user_id, conn_id, state);
        }
    }



    public class AdapterHandler
    {
        public static Event? fromJson(string data)
        {
            var table = JObject.Parse(data);
            string post_type = (string)table["post_type"]!;

            switch (post_type)
            {
                case "message":
                    {
                        MessageEvent? e = null;
                        string message_type = (string)table["message_type"]!;
                        if (message_type == "private")
                        {
                            e =  SerializeConvert.FromJson<PrivateMessageEvent>(data);
                        }
                        else if (message_type == "group")
                        {
                            e =  SerializeConvert.FromJson<GroupMessageEvent>(data);
                        }
                        if (e!= null)
                        {
                            e.raw_event = table;
                            return e;
                        }
                        return null;
                    }
                case "meta_event":
                    {
                        MetaEvent? e = null;
                        string meta_event_type = (string)table["meta_event_type"]!;
                        if (meta_event_type == "heartbeat")
                        {
                            e =  SerializeConvert.FromJson<HeartBeatMetaEvent>(data);
                        }
                        if (e != null)
                        {
                            e.raw_event = table;
                            return e;
                        }
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}