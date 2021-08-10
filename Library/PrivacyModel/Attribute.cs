using System;
using Newtonsoft.Json;

namespace Library.PrivacyModel
{
    public class Attribute
    {
        public Attribute(string name)
        {
            DataTypeName = name;
        }

        public Attribute(string dataTypeName, string dataType)
        {
            DataTypeName = dataTypeName;
            SetDataType(dataType);
        }

        public Attribute(string dataTypeName, string dataType, string value)
        {
            DataTypeName = dataTypeName;
            Value = value;
            SetDataType(dataType);
        }

        [JsonProperty("name")]
        public string DataTypeName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("dataType")]
        public string DataType;

        public string GetDataType()
        {
            return DataType;
        }

        private void SetDataType(SuppertedDataType xsd_datatype)
        {
            switch (xsd_datatype)
            {
                case SuppertedDataType.Int:
                    DataType = "int";
                    break;
                case SuppertedDataType.Double:
                    DataType = "double";
                    break;
                case SuppertedDataType.String:
                    DataType = "string";
                    break;
                default:
                    break;
            }
        }

        public void SetDataType(string xsd_datatype)
        {
            switch (xsd_datatype)
            {
                case "int":
                    SetDataType(SuppertedDataType.Int);
                    break;
                case "double":
                case "number":
                    SetDataType(SuppertedDataType.Double);
                    break;
                case "string":
                    SetDataType(SuppertedDataType.String);
                    break;
                default:
                    break;
            }
        }
        public string GetRDF()
        {
            return "";
        }
    }

    public enum SuppertedDataType
    {
        String, 
        Int, 
        Double
    }
}
