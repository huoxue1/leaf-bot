using System.Collections;

namespace leaf.adapter.onebot_v11.eve
{

    class  ApiResp
    {
        public string status {get;set;} = "";
        public int retcode  {get;set;}

        public string message {get;set;} = "";

        public string echo {get;set;} = "";

        public Hashtable? data {get;set;}
    }

    class ApiRespList
    {
        public string status { get; set; } = "";
        public int retcode { get; set; }

        public string message { get; set; } = "";

        public string echo { get; set; } = "";

        public List<Hashtable>? data { get; set; }
    }

}