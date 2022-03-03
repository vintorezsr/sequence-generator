namespace SequenceGenerator
{
    public class Payload : Dictionary<string, object>
    {
        public void Update(string key, object value)
        {
            if (!ContainsKey(key))
            {
                Add(key, value);
            }
            else
            {
                this[key] = value;
            }
        }
    }
}