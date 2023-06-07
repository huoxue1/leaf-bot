using leaf.eve;
using TouchSocket.Core;

namespace leaf.adapter.onebot_v11.eve
{
    public class MetaEvent : BaseEvent
    {
        public string meta_event_type{get;set;} = "";

         public override EventType getEventType()
        {
            return EventType.Meta;
        }

        public  override  string getName()
        {
            return "meta_event";
        }


       
    }

    public class HeartBeatMetaEvent : MetaEvent
    {
        public Int64 self_id {get;set;}

        public int interval {get;set;}

         public Status? status { get; set; }


         public  override  string getName()
        {
            return "heartbeat_meta_event";
        }
        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
    }


    public class Stat
{
    /// <summary>
    /// 
    /// </summary>
    public int packet_received { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int packet_sent { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int packet_lost { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int message_received { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int message_sent { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int disconnect_times { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int lost_times { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int last_message_time { get; set; }

    public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
}

public class Status
{
    /// <summary>
    /// 
    /// </summary>
    public string app_enabled { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public string app_good { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
     public string app_initialized { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public string good { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public string online { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public string plugins_good { get; set; } = "";
    /// <summary>
    /// 
    /// </summary>
    public Stat? stat { get; set; }

    public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
}


}