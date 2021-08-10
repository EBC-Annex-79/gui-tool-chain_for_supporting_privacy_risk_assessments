using System.Collections.Generic;
using Newtonsoft.Json;

namespace Library.PrivacyModel
{
    public class Node
    {

        [JsonIgnore]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string AddressType { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("superType")]
        public string SuperType { get; set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; }

        public Node(int id, string name, string addressType)
        {
            Attributes = new List<Attribute>();
            ID = id;
            Name = name;
            AddressType = addressType;
        }

        public Node(int id, string name, string addressType, string superType) : this(id, name, addressType)
        {
            SuperType = superType;
        }

        public void AddAttribute(Attribute attribute)
        {
            Attributes.Add(attribute);
        }
    }
}