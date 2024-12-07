namespace JSONRising.Models
{
    public class MessagePair
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public MessagePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}