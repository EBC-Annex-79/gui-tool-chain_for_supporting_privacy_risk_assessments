using System.Collections.Generic;

namespace WebGui.Models.DTO
{
    public class DataAttribute
    {
        public int ID { get; set; }

        public string Type { get; set; }

        public int superType { get; set; }

        public string superAddress { get; set; }

        public List<Attribute> Attribute { get; set; }
    }

    public class Attribute
    {
        public Attribute(){

        }

         public Attribute(Library.PrivacyModel.Attribute attribute){
             DataType = attribute.DataType;
             DataTypeName = attribute.DataTypeName;
             Value = attribute.Value;
        }

        public string DataType { get; set; }

        public string DataTypeName { get; set; }

        public string Value { get; set; }
    }
}