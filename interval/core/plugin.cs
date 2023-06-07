using leaf.bot;
using leaf.driver;

namespace leaf.core
{
    public interface Plugin
    {
        string GetName();

        string GetHelp();

        Matcher[] RegistrMatcher();

        void OnCreated(Driver driver);

        void OnBotConnect(Bot bot);

        void OnBotDisconnect(Bot bot);
    }


    public abstract class BasePlugin : Plugin
    {
        public abstract string GetHelp();

        public abstract string GetName();


        public abstract Matcher[] RegistrMatcher();

        public void OnBotConnect(Bot bot)
        {
            
        }

        public void OnBotDisconnect(Bot bot)
        {
            
        }

        public void OnCreated(Driver driver)
        {
            
        }

        
    }
}