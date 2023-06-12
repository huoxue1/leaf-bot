using System.Collections;
using leaf.driver;
using leaf.eve;

namespace leaf.adapter
{
    public interface Adapter
    {
        string Name { get; }
        void handleData(string data);

        void setDriver(Driver driver);

        bot.Bot createBot(string user_id,string conn_id,Hashtable state);

        public  Task<Event?> GetFutureEvent(params Delegate[] rules);
    }
}