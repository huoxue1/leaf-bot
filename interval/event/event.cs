using leaf.core.message;

namespace leaf.eve
{
    public interface Event{
        string getName();

        EventType getEventType();


        Message GetMessage();
    }

    public enum EventType
    {
        Request,
        Meta,
        Message,
        Notice,

        Unknow
    }
}