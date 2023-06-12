using leaf.driver.reverse_ws;
using System.Collections;
using leaf.adapter.onebot_v11;
using leaf.config;
using leaf.core;
using leaf.driver;
using leaf.log;
using leaf.adapter.onebot_v11.eve;
using leaf.core.message;
using static leaf.adapter.onebot_v11.Rule;

namespace main
{
    class Program
    {
        public static void Main(){
            Core.LoadPlugin(new Echo() );
            leaf.driver.Driver d = new Reverse_Websocker();
            Core.Run(d,new OnebotV11Adapter());
            Console.ReadLine();
        }
    }

    public class Echo : BasePlugin, Matcher
    {
        public override string GetHelp()
        {
           return "echo";
        }

        public override string GetName()
        {
            return "echo";
        }

        public override Matcher[] RegistrMatcher()
        {
           return new Matcher[]{this};
        }


        [OnCommand("echo",priority = 1)]
        [R(typeof(Rule), typeof(MyDelegate),"OnlyToMe")]
        public async void _(Bot bot, MessageEvent e)
        {
           L.Info($"echo success,{e.raw_message}");
           
           await e.send(MessageSegment.Text($"echo {e.raw_message}"));
           var l =  await  bot.GetGroupList();
            l.ForEach(item => { L.Debug(item.group_name); });
        }

        
    }
}
