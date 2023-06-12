using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using leaf.adapter;
using leaf.core;
using leaf.eve;

namespace leaf.driver
{
    public interface Driver {
         void initDriver(Adapter adapter,List<BaseMatcher> matcherList);

        string getName();
        void run();
        Task<string> sendData(string conn_id,Hashtable data);

        bot.Bot GetBot(string bot_id);

        List<BaseMatcher> getMatcherList(EventType @type);
    }
}