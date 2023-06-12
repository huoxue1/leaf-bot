using leaf.adapter.onebot_v11.eve;


namespace leaf.adapter.onebot_v11
{
    public class Rule
    {
        public delegate bool MyDelegate(MessageEvent e);

        public static bool OnlyToMe(MessageEvent e)
        {
            if (e.message_type == "private")
            {
                return true;
            }
            if (e.message!.has("at"))
            {
                var messages = e.message!.get("at");
                foreach (var m in messages)
                {
                    if ((string)m.data!["qq"]! == e.self_id.ToString())
                    {
                        return true;
                    }
                }
            }


            return false;
        }
    }
}
