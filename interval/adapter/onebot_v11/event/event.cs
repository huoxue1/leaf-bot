using leaf.core.message;
using leaf.driver;
using leaf.eve;
using TouchSocket.Core;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace leaf.adapter.onebot_v11.eve
{
    public class BaseEvent : Event
    {

        public driver.Driver? driver {get;set;}

        public JObject? raw_event { get; set; }
        

        public virtual EventType getEventType()
        {
            return EventType.Unknow;
        }


        public virtual string getName()
        {
            return "base_event";
        }

        public virtual Message GetMessage()
        {
            throw new NotImplementedException();
        }

        public Int64 time { get; set; } = 0;

        public string post_type { get; set; } = "";

        public string sub_type { get; set; } = "";

        public Int64 self_id { get; set; }

        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }

        public void setDriver(Driver driver)
        {
            this.driver = driver;
        }

        public string GetSelfId()
        {
            return self_id.ToString();
        }

        public object? getRawField(string key)
        {
            return raw_event?[key];
        }
    }


}