using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace GameEngine
{
    public class StorageReader
    {
        XmlReader reader;
        private static Dictionary<string, Type> PrimitivesTypes;

        public StorageReader(string filename)
        {
            reader = XmlReader.Create(filename);
        }

        public Storage Load()
        {
            Storage storage = new Storage();
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    string nodeType = reader.Name;
                    if (nodeType == "Storage")
                        continue;
                    switch (nodeType.ToLower())
                    {
                        case "string":
                            string atrName = reader["name"];
                            reader.Read();
                            storage.Set(atrName, reader.Value.Trim());
                            break;
                        case "int": storage.Set(reader["name"], int.Parse(reader["value"])); break;
                        case "float": storage.Set(reader["name"], float.Parse(reader["value"])); break;
                        case "double": storage.Set(reader["name"], double.Parse(reader["value"])); break;
                        case "decimal": storage.Set(reader["name"], decimal.Parse(reader["value"])); break;
                        case "bool": storage.Set(reader["name"], bool.Parse(reader["value"])); break;
                        case "char": storage.Set(reader["name"], char.Parse(reader["value"])); break;
                        case "short": storage.Set(reader["name"], short.Parse(reader["value"])); break;
                        case "long": storage.Set(reader["name"], long.Parse(reader["value"])); break;
                    }

                }
            }
            reader.Close();
            return storage;
        }
    }
}