using System.Collections;
using System.Collections.Generic;
using leaf.adapter;
using leaf.bot;
using leaf.config;
using leaf.core;
using leaf.eve;
using Newtonsoft.Json.Linq;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace leaf.driver.reverse_ws
{
    class Reverse_Websocker : WebSocketPluginBase<HttpSocketClient>, Driver
    {


        private string address;

        private Adapter? adapter1;

        public List<BaseMatcher> matchers = new List<BaseMatcher>();

        private HttpService? service = null;

        private Hashtable botList = new Hashtable();


        private string token;

        public Reverse_Websocker()
        {
            this.address = config.Config.get("driver:reverse_ws_address", "127.0.0.1:8080");
            this.token = config.Config.get("driver:token", "");
        }





        public static Driver newDriver()
        {
            return new Reverse_Websocker();
        }

        public string getName()
        {
            return "reverse_ws";
        }

        public void initDriver(Adapter adapter, List<BaseMatcher> matcherList)
        {
            this.adapter1 = adapter;
            adapter1.setDriver(this);
            this.matchers = matcherList;
        }


        public void run()
        {
            string name = this.getName();
            service = new HttpService();
            service.Setup(new TouchSocketConfig()//加载配置
                    .UsePlugin()
                    .SetListenIPHosts(new IPHost[] { new IPHost(this.address) })
                    .ConfigureContainer(a =>
                        {
                            a.AddConsoleLogger();
                        })
                    .ConfigurePlugins(a =>
                        {
                            a.UseWebSocket()//添加WebSocket功能
                            .SetWSUrl($"/{this.getName()}/ws");
                            a.Add(this);
                        }))
                    .Start();
            

            log.L.get().Info($"the reverse_websocket is listing in {this.address}/{this.getName()}/ws");

        }




        protected override void OnHandshaked(HttpSocketClient client, HttpContextEventArgs e)
        {
            log.L.get().Info($"bot {e.Context.Request.Headers.Get("x-self-id")} 连接成功！");
            var bot = this.adapter1!.createBot(e.Context.Request.Headers.Get("x-self-id")!,client.ID,new Hashtable());
            botList.Add(bot.getId(),bot);
        }

        protected override void OnClosing(ITcpClientBase client, MsgEventArgs e){
        
            // log.L.get().Info($"bot {e.Context.Request.Headers.Get("x-self-id")} 断开连接！");
        }

        protected override void OnHandshaking(HttpSocketClient client, HttpContextEventArgs e)
        {

            if (token != ""){
                var authToken = e.Context.Request.Headers.Get("authorization")!.Split(' ')[1];
                if ( authToken != token){
                    e.Context.Response
                    .SetStatus("401", "auth failed!")
                    .Answer();
                    log.L.get().Info($"已拒绝客户端{e.Context.Request.Headers.Get("x-self-id")}的连接,原因：token错误!");
                     base.OnHandshaking(client, e);
                }
                
            }
            
           
        }


        protected override Task OnHandleWSDataFrameAsync(HttpSocketClient client, WSDataFrameEventArgs e)
        {
            string data = e.DataFrame.ToText();
            var echo = JObject.Parse(data)["echo"];
            if (echo == null || (string)echo! == ""){
                return Task.Run(() => this.adapter1!.handleData(data));
            }else{
                if (echoResp.ContainsKey((string)echo!)){
                    TaskCompletionSource<string> echoHandle = (TaskCompletionSource<string>)echoResp[(string)echo!]!;
                    echoHandle.TrySetResult(data);
                }
                return Task.Run(()=>{});
            }
        }

        public static string GuidTo16String()
         {
             long i = 1;
             foreach (byte b in Guid.NewGuid().ToByteArray())
                 i *= ((int)b + 1);
             return string.Format("{0:x}", i - DateTime.Now.Ticks);
         }


         public Hashtable echoResp = new Hashtable();

 

        public async Task<string> sendData(string connId,Hashtable data)
        {
            HttpSocketClient? client;
            this.service!.TryGetSocketClient(connId,out client);
            string echo_id = GuidTo16String();
            data.Add("echo",echo_id);
            client.SendWithWSAsync(SerializeConvert.ToJson(data));
            var tasksource = new TaskCompletionSource<string>();
            tasksource.Task.GetAwaiter().OnCompleted(()=>{
                echoResp.Remove(echo_id);
            });
            echoResp.Add(echo_id,tasksource);
            return await tasksource.Task.WaitAsync(new TimeSpan(0,0,30));
        }

        public Bot GetBot(string bot_id)
        {
            return (Bot)botList[bot_id]!;
        }

        List<BaseMatcher> Driver.getMatcherList(EventType @type)
        {
            return this.matchers.FindAll((matcher) => matcher.MatcherType == @type);
        }
    }
}