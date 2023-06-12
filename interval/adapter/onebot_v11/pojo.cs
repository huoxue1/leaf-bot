using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leaf.adapter.onebot_v11
{
    public class LoginInfo
    {
        /// <summary>
        /// QQ 号
        /// </summary>
        public long user_id { get; set; }

        /// <summary>
        /// QQ 昵称
        /// </summary>
        public string nickname { get; set; } = "";
    }

    public class StrangerInfo
    {
        /// <summary>
        /// QQ 号
        /// </summary>
        public long user_id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; } = ""; 

        /// <summary>
        /// 性别，male 或 female 或 unknown
        /// </summary>
        public string sex { get; set; } = ""; 

        /// <summary>
        /// 年龄
        /// </summary>
        public int age { get; set; }
    }

    public class FriendInfo
    {
        /// <summary>
        /// QQ 号
        /// </summary>
        public long user_id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; } = "";

        /// <summary>
        /// 备注名
        /// </summary>
        public string remark { get; set; } = "";
    }

    public class GroupInfo
    {
        public long group_id { get; set; }
        public string group_name { get; set; } = "";
        public int member_count { get; set; }
        public int max_member_count { get; set; }
    }

    public class GroupMemberInfo
    {
        public long group_id { get; set; }
        public long user_id { get; set; }
        public string nickname { get; set; } = "";
        public string card { get; set; } = "";
        public string sex { get; set; } = "";
        public int age { get; set; }
        public string area { get; set; } = "";
        public int join_time { get; set; }
        public int last_sent_time { get; set; }
        public string level { get; set; } = "";
        public string role { get; set; } = "";
        public bool unfriendly { get; set; }
        public string title { get; set; } = "";
        public int title_expire_time { get; set; }
        public bool card_changeable { get; set; }
    }

    public class GroupHonorInfo
    {
        public long group_id { get; set; }
        public GroupHonorData? current_talkative { get; set; }
        public List<GroupHonorData>? talkative_list { get; set; }
        public List<GroupHonorData>? performer_list { get; set; }
        public List<GroupHonorData>? legend_list { get; set; }
        public List<GroupHonorData>? strong_newbie_list { get; set; }
        public List<GroupHonorData>? emotion_list { get; set; }
    }

    public class GroupHonorData
    {
        public long user_id { get; set; }
        public string nickname { get; set; } = "";
        public string avatar { get; set; } = "";
        public int day_count { get; set; }
    }

    public class Credentials
    {
        public string Cookies { get; set; } = "";
        public int CsrfToken { get; set; }
    }

    public class StatusInfo
    {
        public bool? Online { get; set; }
        public bool? Good { get; set; }
        // 其他字段...
    }

    public class VersionInfo
    {
        public string AppName { get; set; } = "";
        public string AppVersion { get; set; } = "";
        public string ProtocolVersion { get; set; } = ""; 
        // 其他字段...
    }
}
