using leaf.core.message;
using leaf.eve;

namespace leaf.adapter.onebot_v11.eve{
    public  class BaseEvent : Event
    {
        public virtual    EventType getEventType(){
            return EventType.Unknow;
        }
    

        public  virtual  string getName()
        {
            return "base_event";
        }

        public virtual Message  GetMessage()
        {
            throw new NotImplementedException();
        }

        public Int64 time {get;set;} = 0;

    public string post_type {get;set;} = "";

    public string sub_type {get;set;} = "";

    }


}