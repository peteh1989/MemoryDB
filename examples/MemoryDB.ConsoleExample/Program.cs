using System;
using MemoryDB.ConsoleExample.Models;
using MemoryDB.Core;

namespace MemoryDB.ConsoleExample
{
    class Program
    {

        private static readonly Database Database = new Database();

        private static void Main(string[] args)
        {
            Database.AddressList.Clear();
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
                }

            System.Console.ReadLine();

        }
    
     

        }
    }

