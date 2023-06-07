using System.Collections;
using TouchSocket.Core;
using leaf.adapter.onebot_v11.eve;

namespace leaf.adapter.onebot_v11{
    public class Bot : leaf.bot.Bot
    {


        private string user_id;
        private driver.Driver driver;
        private string conn_id;

        private Adapter adapter;

        private Hashtable state;


        public Bot(ref driver.Driver driver, Adapter adapter,string user_id,string conn_id,Hashtable? state){
            this.driver = driver;
            this.adapter = adapter;
            this.user_id = user_id;
            this.conn_id = conn_id;
            if (state == null){
                this.state = new Hashtable();
            }
            this.state = state!;
        }

        async Task<Hashtable>   bot.Bot.call_api(string api, Hashtable data)
        {
            var call_data = new Hashtable();
            call_data["action"] = api;
            call_data["params"] = data;
            var resp = await this.driver.sendData(this.conn_id,call_data);
            var api_resp =  await SerializeConvert.FromJsonAsync<ApiResp>(resp);
            return api_resp.data!;
        }

        string bot.Bot.getId()
        {
            return this.user_id;
        }
    }
}