using leaf.driver;
using TouchSocket.Core;
using leaf.eve;
using System.Collections;
using leaf.adapter.onebot_v11.eve;
using leaf.log;
using Newtonsoft.Json.Linq;
using leaf.bot;
using leaf.core;
using System.Reflection;

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
                foreach(var rule in matcher.rules){
                    MethodInfo method = rule.GetMethodInfo();
                    ruleSuccess = (bool)method.Invoke(rule.Target,Utils.parseParam(e,bot,state,method.GetParameters()))!;
                    if (!ruleSuccess){
                        break;
                    }
                }
                if (!ruleSuccess){
                    break;
                }
               
                var handleMethod = matcher.MetaData["method"] as MethodInfo;
                L.Debug($"start handle the matcher {handleMethod!.Name}");
                handleMethod!.Invoke(matcher.MetaData["matcher"],Utils.parseParam(e,bot,state,handleMethod!.GetParameters()));
                if (matcher.isBlock){
                    break;
                }
            }
        }

        private void handleMeta(MetaEvent e)
        {

        }


        private Event? fromJson(string data)
        {
            var table = JObject.Parse(data);
            string post_type = (string)table["post_type"]!;

            switch (post_type)
            {
                case "message":
                    {

                        string message_type = (string)table["message_type"]!;
                        if (message_type == "private")
                        {
                            return SerializeConvert.FromJson<PrivateMessageEvent>(data);
                        }
                        else if (message_type == "group")
                        {
                            return SerializeConvert.FromJson<GroupMessageEvent>(data);
                        }
                        return null;
                    }
                case "meta_event":
                    {
                        string meta_event_type = (string)table["meta_event_type"]!;
                        if (meta_event_type == "heartbeat")
                        {
                            return SerializeConvert.FromJson<HeartBeatMetaEvent>(data);
                        }
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        void Adapter.handleData(string data)
        {

            var e = fromJson(data);
            if (e == null)
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

        void Adapter.setDriver( Driver driver)
        {
            this.d = driver;
        }

        bot.Bot Adapter.createBot(string user_id, string conn_id, Hashtable state)
        {
           return new Bot(ref this.d!,this,user_id,conn_id,state);
        }
    }
}