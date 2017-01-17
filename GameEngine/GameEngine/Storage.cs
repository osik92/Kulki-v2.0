using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEngine
{
    public class Storage
    {
        private Dictionary<string, object> values;

        public Storage()
        {
            values = new Dictionary<string, object>();
        }
        public T Get<T> (string key)
        {
            return (T)values[key];
        }

        public void Set<T>(string key, T value)
        {
            if (values.ContainsKey(key))
                values[key] = value;
            else
                values.Add(key, value);
        }

        public void Save(string filename)
        {
            StorageWriter sw = new StorageWriter(filename);
            sw.WriteStorage(this);
            sw.Save();
        }
        
        private object GetValueByType(string value, string type)
        {
            switch (type)
            {
                case "System.Int32":
                    return Int32.Parse(value);
                case "System.Single":
                    return Single.Parse(value);
                case "System.Boolean":
                    return Boolean.Parse(value);
                case "System.Char":
                    return Char.Parse(value);
                default:
                    return type;
            }
        }

        public Dictionary<string, object> Values
        { get { return values; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> keyValuePair in values)
            {
                sb.AppendLine(string.Format("<{0}>[{1}]:{2} ", keyValuePair.Value.GetType().ToString(), keyValuePair.Key,
                    keyValuePair.Value));
            }
            return sb.ToString();
        }
    }
}
