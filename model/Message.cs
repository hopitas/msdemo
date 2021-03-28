using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace MS.model
{

    public class Message
    {
        [Computed]
        [Key]
        public int id { get; set; }
        public string Key { get; set; }
        public string Email { get; set; }
        public List<string> Attributes { get; set; }
    }

    public class MessageString
    {
        [Computed]
        [Key]
        public int id { get; set; }
        public string Key { get; set; }
        public string Email { get; set; }
        public string Attributes { get; set; }
    }

    public class MessageTable
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Atributes { get; set; }
    }
}