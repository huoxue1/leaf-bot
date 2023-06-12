using leaf.core.message;

namespace leaf.eve
{
    public interface Event{
        string getName();

        EventType getEventType();

        public void setDriver(driver.Driver driver);

        public string GetSelfId();
        Message GetMessage();

        object? getRawField(string key);
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