using Newtonsoft.Json;

namespace Library.PrivacyModel
{
    public class Link
    {
        [JsonIgnore]
        public Node Source { get; set; }

        [JsonIgnore]
        public Node Target { get; set; }

    }

    public class OntologyLink : Link
    {
        public OntologyLink(string relationsName, string subjectAddress, string objectAddress)
        {
            RelationsName = relationsName;
            this.subjectAddress = subjectAddress;
            this.objectAddress = objectAddress;
        }

        [JsonProperty("predicate")]
        public string RelationsName { get; set; }

        [JsonProperty("subject")]
        public string subjectAddress { get; set; } 

        [JsonProperty("object")]
        public string objectAddress { get; set; }

    }
}