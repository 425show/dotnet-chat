using System;

namespace chat
{
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name)
        {
            this.Name = name;

        }
        public string Name { get; set; }
    }
}