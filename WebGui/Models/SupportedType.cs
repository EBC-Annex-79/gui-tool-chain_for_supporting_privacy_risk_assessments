using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Library;
using Library.PrivacyModel;
using System.Diagnostics.CodeAnalysis;

namespace WebGui.Models
{
    public class SupportedType :IComparable<SupportedType>
    {
        public SupportedType(string name, string address, int? superTypeColorId, bool clickable)
        {
            subDataStrucktor = new SortedList<String, SupportedType>();
            Attributes = new LinkedList<DTO.Attribute>();
            Name = name;
            Address = address;
            SuperTypeColorId = superTypeColorId;
            Clickable =  clickable;
        }

        public SupportedType(string name, string address, int? superTypeColorId, bool clickable, string superAddress) : this(name,address,superTypeColorId, clickable)
        {
            SuperAddress = superAddress;
        }

        public int? SuperTypeColorId { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        public string SuperAddress { get; set; }

        private SortedList<string, SupportedType> subDataStrucktor;

        public LinkedList<SupportedType> GetSubDataStrucktor()
        {
            return new LinkedList<SupportedType> (subDataStrucktor.Values);
        }

        public LinkedList<DTO.Attribute> Attributes { get; }
        public bool Clickable { get; set; }

        public void AddSubSupportedType(SupportedType supported)
        {
            subDataStrucktor.Add(supported.Name,supported);
        }

        public void AddAttribute(DTO.Attribute attribute)
        {
            Attributes.AddLast(attribute);
        }

        public void AddAttribute(Library.PrivacyModel.Attribute attribute)
        {
            DTO.Attribute tempAttribute = new DTO.Attribute(attribute);
            Attributes.AddLast(tempAttribute);
        }

        public void AddAttributes(LinkedList<DTO.Attribute> attributes)
        {
            foreach (DTO.Attribute  item in attributes)
            {
                AddAttribute(item);
            }           

        }

        public void AddAttributes(LinkedList<Library.PrivacyModel.Attribute> attributes)
        {
            foreach (Library.PrivacyModel.Attribute  item in attributes)
            {
                AddAttribute(item);
            }           

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public int CompareTo([AllowNull] SupportedType other)
        {
             if (this == null) {
                return 1;
            }
            if (other == null) {
                return 0;
            }
            return String.Compare(this.Name, other.Name);
        }
    }
}
