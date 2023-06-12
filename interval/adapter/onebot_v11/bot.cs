using System.Collections;
using TouchSocket.Core;
using leaf.adapter.onebot_v11.eve;
using leaf.core.message;
using Newtonsoft.Json;
using leaf.eve;
using leaf.core;

namespace leaf.adapter.onebot_v11{
    public class Bot : leaf.bot.Bot
    {
        private string user_id;
        private driver.Driver driver;
        private string conn_id;

        private Adapter adapter;

        private Hashtable state;


        public Bot(ref driver.Driver driver, Adapter adapter,string user_id,string conn_id,Hashtable? state){
            this.driver = driver;
            this.adapter = adapter;
            this.user_id = user_id;
            this.conn_id = conn_id;
            if (state == null){
                this.state = new Hashtable();
            }
            this.state = state!;
        }

        public  async Task<Hashtable>  call_api(string api, Hashtable data)
        {
            log.L.Debug($"call api {api} {JsonConvert.SerializeObject(data)}");
            var call_data = new Hashtable();
            call_data["action"] = api;
            call_data["params"] = data;
            var resp = await this.driver.sendData(this.conn_id,call_data);
            var api_resp =  await SerializeConvert.FromJsonAsync<ApiResp>(resp);
            log.L.Debug($"api resp {JsonConvert.SerializeObject(api_resp.data)}");
            return api_resp.data!;
        }

        public async Task<List<Hashtable>> call_api_return_list(string api, Hashtable data)
        {
            log.L.Debug($"call api {api} {JsonConvert.SerializeObject(data)}");
            var call_data = new Hashtable();
            call_data["action"] = api;
            call_data["params"] = data;
            var resp = await this.driver.sendData(this.conn_id, call_data);
            var api_resp = await SerializeConvert.FromJsonAsync<ApiRespList>(resp);
            log.L.Debug($"api resp {JsonConvert.SerializeObject(api_resp.data)}");
            return api_resp.data!;
        }

        public async Task<Hashtable> call_api(string api)
        {
            return await call_api(api, new Hashtable());
        }

        string bot.Bot.getId()
        {
            return this.user_id;
        }

        public async Task<Hashtable> send_group_msg(Int64 group_id,Message message)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["message"] = message;
            return await call_api("send_group_msg",data);
        }

        public async Task<Hashtable> send_private_msg(Int64 user_id,Message message)
        {
            var data = new Hashtable();
            data["user_id"] = user_id;
            data["message"] = message;
            return await call_api("send_group_msg",data);
        }

        public async Task<Event?> get_event()
        {
            return await this.adapter.GetFutureEvent();
        }


        public async Task<Event?> get_event(params Delegate[] rules)
        {
            return await this.adapter.GetFutureEvent(rules);
        }

        public async Task<Event?> get_event(params Func<Event, bool>[] rules)
        {
            return await this.adapter.GetFutureEvent(rules);
        }

        public async Task<T?> GetSessionEvent<T>(Event e) where T: Event
        {

            var group_id = e.getRawField("group_id");
            var user_id = e.getRawField("user_id");
           return (T?)await this.adapter.GetFutureEvent((Event e1) => {
                if (e1.getEventType() == EventType.Message)
                {
                    var messageEvent = (MessageEvent)e1;
                    if (messageEvent.message_type == "group")
                    {
                        if (group_id != null && group_id.ToString() == messageEvent.getRawField("group_id")!.ToString())
                        {
                            return true;
                        }
                    }
                    else if (messageEvent.message_type == "private")
                    {
                        if (user_id != null && user_id.ToString() == messageEvent.user_id.ToString())
                        {
                            return true;
                        }
                    }
                }
                return false;
           });

        }




        /// <summary>
        /// ɾ����Ϣ
        /// </summary>
        /// <param name="message_id">��Ϣ ID</param>
        /// <returns></returns>
        public async Task DeleteMsg(int message_id)
        {
            var data = new Hashtable();
            data["message_id"] = message_id;
            await call_api("delete_msg", data);
        }


        public async Task<Hashtable> GetMsg(int message_id)
        {
            var data = new Hashtable();
            data["message_id"] = message_id;
            return await call_api("get_msg", data);
        }

        public async Task<List<Hashtable>?> GetForwardMsg(string id)
        {
            var data = new Hashtable();
            data["id"] = id;
            return await call_api_return_list("get_forward_msg", data);
        }

        public async Task SendLike(long user_id, int times = 1)
        {
            var data = new Hashtable();
            data["user_id"] = user_id;
            data["times"] = times;
            await call_api("send_like", data);
        }

        public async Task SetGroupKick(long group_id, long user_id, bool reject_add_request = false)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["reject_add_request"] = reject_add_request;
            await call_api("set_group_kick", data);
        }

        public async Task SetGroupBan(long group_id, long user_id, int duration = 30 * 60)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["duration"] = duration;
            await call_api("set_group_ban", data);
        }

        public async Task SetGroupAnonymousBan(long group_id, string anonymous_flag, int duration = 30 * 60)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["anonymous_flag"] = anonymous_flag;
            data["duration"] = duration;
            await call_api("set_group_anonymous_ban", data);
        }

        public async Task SetGroupWholeBan(long group_id, bool enable = true)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["enable"] = enable;
            await call_api("set_group_whole_ban", data);
        }

        public async Task SetGroupAdmin(long group_id, long user_id, bool enable = true)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["enable"] = enable;
            await call_api("set_group_admin", data);
        }

        public async Task SetGroupAnonymous(long group_id, bool enable = true)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["enable"] = enable;
            await call_api("set_group_anonymous", data);
        }

        public async Task SetGroupCard(long group_id, long user_id, string card)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["card"] = card;
            await call_api("set_group_card", data);
        }

        /// <summary>
        /// ����Ⱥ��
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <param name="group_name">��Ⱥ��</param>
        /// <returns></returns>
        public async Task SetGroupName(long group_id, string group_name)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["group_name"] = group_name;
            await call_api("set_group_name", data);
        }

        /// <summary>
        /// ����Ⱥ��ר��ͷ��
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <param name="user_id">Ҫ���õ� QQ ��</param>
        /// <param name="special_title">ר��ͷ�Σ��������ַ�����ʾɾ��ר��ͷ��</param>
        /// <param name="duration">ר��ͷ����Ч�ڣ���λ�룬-1 ��ʾ���ã����������ƺ�û��Ч����������ֻ��ĳЩ�����ʱ�䳤����Ч���д�����</param>
        /// <returns></returns>
        public async Task SetGroupSpecialTitle(long group_id, long user_id, string special_title = "", int duration = -1)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["special_title"] = special_title;
            data["duration"] = duration;
            await call_api("set_group_special_title", data);
        }

        /// <summary>
        /// ����Ӻ�������
        /// </summary>
        /// <param name="flag">�Ӻ�������� flag������ϱ��������л�ã�</param>
        /// <param name="approve">�Ƿ�ͬ������</param>
        /// <param name="remark">��Ӻ�ĺ��ѱ�ע������ͬ��ʱ��Ч��</param>
        /// <returns></returns>
        public async Task SetFriendAddRequest(string flag, bool approve = true, string remark = "")
        {
            var data = new Hashtable();
            data["flag"] = flag;
            data["approve"] = approve;
            data["remark"] = remark;
            await call_api("set_friend_add_request", data);
        }

        /// <summary>
        /// �����Ⱥ��������
        /// </summary>
        /// <param name="flag">��Ⱥ����� flag������ϱ��������л�ã�</param>
        /// <param name="sub_type">add �� invite���������ͣ���Ҫ���ϱ���Ϣ�е� sub_type �ֶ������</param>
        /// <param name="approve">�Ƿ�ͬ����������</param>
        /// <param name="reason">�ܾ����ɣ����ھܾ�ʱ��Ч��</param>
        /// <returns></returns>
        public async Task SetGroupAddRequest(string flag, string sub_type, bool approve = true, string reason = "")
        {
            var data = new Hashtable();
            data["flag"] = flag;
            data["sub_type"] = sub_type;
            data["approve"] = approve;
            data["reason"] = reason;
            await call_api("set_group_add_request", data);
        }


        /// <summary>
        /// ��ȡ��¼����Ϣ
        /// </summary>
        /// <returns>��¼����Ϣ</returns>
        public async Task<LoginInfo> GetLoginInfo()
        {
            var response = await call_api("get_login_info", new Hashtable());
            // ������Ӧ���ݣ����ص�¼����Ϣ����
            // ʾ�����룬�����ʵ����������޸�
            var loginInfo = response.ToObject<LoginInfo>();
            return loginInfo;
        }


        /// <summary>
        /// ��ȡȺ��Ϣ
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <param name="no_cache">�Ƿ�ʹ�û���</param>
        /// <returns>Ⱥ��Ϣ</returns>
        public async Task<GroupInfo> GetGroupInfo(long group_id, bool no_cache = false)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["no_cache"] = no_cache;
            var response = await call_api("get_group_info", data);
            return response.ToObject<GroupInfo>();
        }

        /// <summary>
        /// ��ȡȺ�б�
        /// </summary>
        /// <returns>Ⱥ�б�</returns>
        public async Task<List<GroupInfo>> GetGroupList()
        {
            var response = await call_api_return_list("get_group_list",new Hashtable());
            var result = new List<GroupInfo>();
            foreach (var item in response)
            {
                result.Add(item.ToObject<GroupInfo>());
            }
            return result;
        }

        /// <summary>
        /// ��ȡȺ��Ա��Ϣ
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <param name="user_id">QQ ��</param>
        /// <param name="no_cache">�Ƿ�ʹ�û���</param>
        /// <returns>Ⱥ��Ա��Ϣ</returns>
        public async Task<GroupMemberInfo> GetGroupMemberInfo(long group_id, long user_id, bool no_cache = false)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["user_id"] = user_id;
            data["no_cache"] = no_cache;
            var response = await call_api("get_group_member_info", data);
            return response.ToObject<GroupMemberInfo>();
        }

        /// <summary>
        /// ��ȡȺ��Ա�б�
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <returns>Ⱥ��Ա�б�</returns>
        public async Task<List<GroupMemberInfo>> GetGroupMemberList(long group_id)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            var response = await call_api_return_list("get_group_member_list", data);
            var result = new List<GroupMemberInfo>();
            foreach (var item in response)
            {
                result.Add(item.ToObject<GroupMemberInfo>());
            }
            return result;
        }

        /// <summary>
        /// ��ȡȺ������Ϣ
        /// </summary>
        /// <param name="group_id">Ⱥ��</param>
        /// <param name="type">Ҫ��ȡ��Ⱥ��������</param>
        /// <returns>Ⱥ������Ϣ</returns>
        public async Task<GroupHonorInfo> GetGroupHonorInfo(long group_id, string type)
        {
            var data = new Hashtable();
            data["group_id"] = group_id;
            data["type"] = type;
            var response = await call_api("get_group_honor_info", data);
            return response.ToObject<GroupHonorInfo>();
        }


        /// <summary>
        /// ��ȡ Cookies
        /// </summary>
        /// <param name="domain">��Ҫ��ȡ cookies ������</param>
        /// <returns>Cookies</returns>
        public async Task<string?> GetCookies(string domain)
        {
            var data = new Hashtable();
            data["domain"] = domain;
            var response = await call_api("get_cookies", data);
            return response["cookies"] as string;
        }

        /// <summary>
        /// ��ȡ CSRF Token
        /// </summary>
        /// <returns>CSRF Token</returns>
        public async Task<int> GetCSRFToken()
        {
            var response = await call_api("get_csrf_token");
            return Convert.ToInt32(response["token"]);
        }

        /// <summary>
        /// ��ȡ QQ ��ؽӿ�ƾ֤
        /// </summary>
        /// <param name="domain">��Ҫ��ȡ cookies ������</param>
        /// <returns>ƾ֤��Ϣ</returns>
        public async Task<Credentials> GetCredentials(string domain)
        {
            var data = new Hashtable();
            data["domain"] = domain;
            var response = await call_api("get_credentials", data);
            return response.ToObject<Credentials>();
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="file">�յ��������ļ���</param>
        /// <param name="out_format">Ҫת�����ĸ�ʽ</param>
        /// <returns>ת����������ļ�·��</returns>
        public async Task<string?> GetRecord(string file, string out_format)
        {
            var data = new Hashtable();
            data["file"] = file;
            data["out_format"] = out_format;
            var response = await call_api("get_record", data);
            return response["file"] as string;
        }

        /// <summary>
        /// ��ȡͼƬ
        /// </summary>
        /// <param name="file">�յ���ͼƬ�ļ���</param>
        /// <returns>���غ��ͼƬ�ļ�·��</returns>
        public async Task<string?> GetImage(string file)
        {
            var data = new Hashtable();
            data["file"] = file;
            var response = await call_api("get_image", data);
            return response["file"]! as string;
        }

        /// <summary>
        /// ����Ƿ���Է���ͼƬ
        /// </summary>
        /// <returns>�Ƿ���Է���ͼƬ</returns>
        public async Task<bool> CanSendImage()
        {
            var response = await call_api("can_send_image");
            return (bool)response["yes"]!;
        }

        /// <summary>
        /// ����Ƿ���Է�������
        /// </summary>
        /// <returns>�Ƿ���Է�������</returns>
        public async Task<bool> CanSendRecord()
        {
            var response = await call_api("can_send_record");
            return (bool)response["yes"]!;
        }

        /// <summary>
        /// ��ȡ����״̬
        /// </summary>
        /// <returns>����״̬��Ϣ</returns>
        public async Task<StatusInfo> GetStatus()
        {
            var response = await call_api("get_status");
            return new StatusInfo
            {
                Online = response["online"] as bool?,
                Good = response["good"] as bool?,
                // �����ֶ�...
            };
        }

        /// <summary>
        /// ��ȡ�汾��Ϣ
        /// </summary>
        /// <returns>�汾��Ϣ</returns>
        public async Task<VersionInfo> GetVersionInfo()
        {
            var response = await call_api("get_version_info");
            return response.ToObject<VersionInfo>();
        }

        /// <summary>
        /// ���� OneBot ʵ��
        /// </summary>
        /// <param name="delay">Ҫ�ӳٵĺ�����</param>
        /// <returns>����״̬</returns>
        public async Task<string?> RestartOneBot(int delay)
        {
            var data = new Hashtable();
            data["delay"] = delay;
            var response = await call_api("set_restart", data);
            return response["status"] as string;
        }

        /// <summary>
        /// ������
        /// </summary>
        public async Task CleanCache()
        {
            await call_api("clean_cache",new Hashtable());
        }


    }
}