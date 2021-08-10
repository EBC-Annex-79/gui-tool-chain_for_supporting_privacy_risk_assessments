using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Library.PrivacyModel
{
    public class ModelItem : IComparer<ModelItem>
    {
        public ModelItem()
        {
            SubDataStrucktor = new ArrayList();
            Attributes = new LinkedList<Attribute>();
        }

        public ModelItem(string name, string address)
        {
            SubDataStrucktor = new ArrayList();
            Attributes = new LinkedList<Attribute>();
            Name = name;
            Address = address;
        }

        public string Name { get; set; }
        public string Address { get; set; }

        [JsonIgnore]
        public string RootSuperClass { get; set; }

        public string SuperClass { get; set; }

        public ArrayList    SubDataStrucktor { get; }
        public LinkedList<Attribute> Attributes { get; }

        public void AddSubModelItem(ModelItem modelItem)
        {
            SubDataStrucktor.Add(modelItem);
        }

        public void AddAttributes(Attribute attribute)
        {
            Attributes.AddLast(attribute);
        }

        public void AddAttributes(LinkedList<Attribute> attributes)
        {
            foreach (var item in attributes)
            {
                Attributes.AddLast(item);
            }
        }

        public int Compare([AllowNull] ModelItem item1, [AllowNull] ModelItem item2)
        {
            if (item1 == null) {
                return 1;
            }
            if (item2 == null) {
                return 0;
            }
            return String.Compare(item1.Name, item2.Name);
        }
    }
}
