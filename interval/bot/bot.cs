using leaf.eve;
using System.Collections;

namespace leaf.bot
{
    public interface Bot{
        public Task<Hashtable> call_api(string api,Hashtable data);

        public string getId();
    }
}