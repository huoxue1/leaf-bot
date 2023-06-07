using System.Collections;
using leaf.driver;

namespace leaf.adapter
{
    public interface Adapter
    {
        void handleData(string data);

        void setDriver(Driver driver);

        bot.Bot createBot(string user_id,string conn_id,Hashtable state);
    }
}