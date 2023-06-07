using leaf.driver.reverse_ws;
using System.Collections;
using leaf.adapter.onebot_v11;
using leaf.config;
using leaf.core;
using leaf.driver;
using leaf.log;
using leaf.adapter.onebot_v11.eve;

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

        [OnMessage("group")]
        public async void OnMessage(Bot bot, MessageEvent e)
        {
           L.Info($"echo OnMessage success,{e.raw_message}");
        }

        [OnCommand("echo")]
        public async void _(Bot bot, MessageEvent e)
        {
           L.Info($"echo success,{e.raw_message}");
        }
    }
}
