using System.Collections.Generic;

namespace MemoryDB.Example.Console.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public ICollection<Address> Addresses { get; set; } 
    }
}
