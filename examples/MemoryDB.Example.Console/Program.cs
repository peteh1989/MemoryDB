using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryDB.Example.Console.Models;

namespace MemoryDB.Example.Console
{
    class Program
    {
        private static readonly Database Database = new Database();

        static void Main(string[] args)
        {
            Database.AddressList.Add(new Address
            {
                AddressLine1 = "6 ",
                AddressLine2 = "Some Avenue",
                PostCode = "FU0 7SW"
            });

            foreach (var address in Database.AddressList)
            {
                System.Console.WriteLine(address.AddressId);
                System.Console.WriteLine(address.AddressLine1);
                System.Console.WriteLine(address.AddressLine2);
                System.Console.WriteLine(address.PostCode);
            };

        }
    }
}
