using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace GameEngine
{
    class StorageWriter
    {
        XmlWriter writer;
        private static Dictionary<string, Type> PrimitivesTypes;

        static StorageWriter()
        {
            PrimitivesTypes = new Dictionary<string, Type>();
            PrimitivesTypes.Add("Int", typeof(int));
            PrimitivesTypes.Add("Float", typeof(float));
            PrimitivesTypes.Add("Double", typeof(double));
            PrimitivesTypes.Add("Bool", typeof(bool));
            PrimitivesTypes.Add("String", typeof(string));
            PrimitivesTypes.Add("Char", typeof(char));
            PrimitivesTypes.Add("Decimal", typeof(decimal));
            PrimitivesTypes.Add("Long", typeof(long));
            PrimitivesTypes.Add("Short", typeof(short));
        }


        public StorageWriter(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            writer = XmlWriter.Create(filename, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Storage");
        }

        public void WriteStorage(Storage storage)
        {
            foreach (KeyValuePair<string, object> keyValuePair in storage.Values)
            {
                if (keyValuePair.Value.GetType() == typeof(string))
                {
                    WriteString(keyValuePair.Key, (string)keyValuePair.Value);
                }
                else
                {
                    WriteNoStringValue(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public void WriteInt(string name, int value)
        {
            WriteNoStringValue(name, value);
        }
        public void WriteShort(string name, short value)
        {
            WriteNoStringValue(name, value);
        }
        public void WriteLong(string name, long value)
        {
            WriteNoStringValue(name, value);
        }

        public void WriteBool(string name, bool value)
        {
            WriteNoStringValue(name, value);
        }

        public void WriteChar(string name, char value)
        {
            WriteNoStringValue(name, value);
        }
        public void WriteString(string name, string value)
        {
            writer.WriteStartElement("String");
            writer.WriteAttributeString("name", name);
            writer.WriteString(value);
            writer.WriteEndElement();
        }

        public void WriteFloat(string name, float value)
        {
            WriteNoStringValue(name, value);
        }
        public void WriteDouble(string name, double value)
        {
            WriteNoStringValue(name, value);
        }
        public void WriteDecimal(string name, decimal value)
        {
            WriteNoStringValue(name, value);
        }

        private void WriteNoStringValue<T>(string name, T value)
        {
            writer.WriteStartElement(PrimitivesTypes.First(x => x.Value == value.GetType()).Key);
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("value", value.ToString());
            writer.WriteEndElement();

        }

        public void Save()
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
    }
}