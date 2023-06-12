using System.Collections;
using System.Text;
using System.Text.Json.Nodes;
using TouchSocket.Core;

namespace leaf.core.message
{
    public class Message : List<MessageSegment>
    {
        public new void Add(MessageSegment item)
        {
            base.Add(item);
        }

        public static Message operator +(Message obj1, Message obj2)
        {
            foreach(MessageSegment segment in obj2)
            {
                obj1.Add(segment);
            }
            return obj1;
        }

        public static Message operator +(Message obj1, MessageSegment obj2)
        {
            obj1.Add(obj2);
            return obj1;
        }


        public string ExtractText()
        {
            var result = new StringBuilder();
            foreach (MessageSegment segment in this)
            {
                if (segment.type == "text")
                {
                    result.Append(segment.data!["text"]);
                }
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }

        public bool has(string @type){
            foreach(MessageSegment segment in this){
                if (segment.type == @type){
                    return true;
                }
            }
            return false;
        }

        public bool hasText(){
            return has("text");
        }

        public bool hasImage(){
            return has("image");
        }

        public List<MessageSegment> get(string @type){
            return this.FindAll((MessageSegment segment) => segment.type == @type);
        }



    }




    public class MessageSegment
    {
        public string @type { get; set; } = "";
        public Hashtable? data { get; set; }



        public static Message operator +(MessageSegment obj1, MessageSegment obj2)
        {
            var result = new Message();
            result.Add(obj1);
            result.Add(obj2);
            return result;
        }

        public MessageSegment() { }

        public MessageSegment(string @type,Hashtable data) {
            this.@type = @type;
            this.data = data;
        }

        public static MessageSegment Text(string text){
            var data = new Hashtable();
            data["text"] = text;
            return new MessageSegment("text",data);
        }




        public override string ToString()
        {
            return SerializeConvert.ToJson(this);
        }

    }
}