using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebGui.Models.DTO
{
    public interface INode
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public string Address { get; set; }

        public string Type { get; }
        
    }

    public class Context : INode
    {
        public string Name { get => Name; set => Name = value; }
        public int ID { get => ID; set => ID = value; }
        public string Address { get => Address; set => Address = value; }

        public string Type { get => Type; set => Type = value; }
    }

}
