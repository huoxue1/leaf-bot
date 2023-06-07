using Microsoft.Extensions.Configuration;
using Tomlyn.Extensions.Configuration;
using System.Collections;
namespace leaf.config
{
    class Config
    {

        private static Config? conf;

        public static void loadConfig(){
            conf = new Config();

        }





        IConfigurationRoot data;

        private Config(){
            data = new ConfigurationBuilder().AddTomlFile("config.toml",true,true).AddJsonFile("config.json",true,true).AddEnvironmentVariables().Build();
            
        }

        private T? getValue<T>(string key){
             return data.GetValue<T>(key);
        }

         private T getValue<T>(string key,T defaultValue){
             return data.GetValue<T>(key,defaultValue)!;
        }

        public static T? get<T>(string key){
            if (conf == null){
                loadConfig();
            }
            return conf!.getValue<T>(key);
        }

        public static T get<T>(string key,T defaultValue){
            if (conf == null){
                loadConfig();
            }
            return conf!.getValue<T>(key,defaultValue);
        }

        public static string? get(string key){
            if (conf == null){
                loadConfig();
            }
            return conf!.getValue<string>(key);
        }

        public static string get(string key,string defaultValue){
            if (conf == null){
                loadConfig();
            }
            return conf!.getValue<string>(key,defaultValue);
        }

    }
}