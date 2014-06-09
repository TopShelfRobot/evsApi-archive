using System;
//using System.ComponentModel.DataAnnotations;
namespace evs.Model
{
    public class Client
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public Int32 OwnerId { get; set; }
    }
}
