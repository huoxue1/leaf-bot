using leaf.eve;
using TouchSocket.Core;
using leaf.core.message;

namespace leaf.adapter.onebot_v11.eve
{
    public class MessageEvent : BaseEvent
    {


        public override EventType getEventType()
        {
            return EventType.Message;
        }


        public override Message GetMessage()
        {
            return message!;
        }

        public Int64 self_id { get; set; }


        public Int64 user_id {get;set;}
        public string message_type { get; set; } = "";
        public Int32 message_id { get; set; }

        public Message? @message { get; set; }
        public string raw_message { set; get; } = "";

        public int front { get; set; }

        public Sender? sender { get; set; }

    }



    class PrivateMessageEvent : MessageEvent
    {


        public override string getName()
        {
            return "private_message_event";
        }

        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
    }


    class GroupMessageEvent : MessageEvent
    {


        public override string getName()
        {
            return "group_message_event";
        }


        public Int64 group_id { get; set; }

        public Anonymous? anonymous { get; set; }


        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
    }


    class Anonymous
    {
        public Int64 id { get; set; }
        public string name { get; set; } = "";

        public string flag { get; set; } = "";


        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
    }




    public class Sender
    {
        public Int64 user_id { get; set; }
        public string nickname { get; set; } = "";

        public string sex { get; set; } = "";

        public int age { get; set; }




        public string? card { get; set; }

        public string? area { get; set; }

        public string? level { get; set; }

        public string? role { get; set; }

        public string? title { get; set; }


        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }
    }

}