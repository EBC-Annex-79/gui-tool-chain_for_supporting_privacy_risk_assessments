

namespace Library.PrivacyModel
{
    public class Relation
    {
        public Relation(string relationName, string domain, string range)
        {
            RelationName = relationName;
            Domain = domain;
            Range = range;
        }

        public string RelationName { get; set; }

        public string Domain { get; set; }

        public string Range { get; set; }

        
    }
}