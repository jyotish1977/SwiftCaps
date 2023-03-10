using System.Collections.Generic;
using SwiftCaps.Fake.Infrastructure;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Fake.Data
{
    public class FakeUserData
    {
        static FakeUserData()
        {
            Init();
        }

        public static List<User> Data { get; set; }

        public static void Init()
        {
            Data = null;
            Data = new List<User>
            {

                new User
                {
                    Id = GenericIdentifiers._1ID,
                    FirstName = "Alan",
                    MiddleName = "S",
                    LastName = "User",
                    Username = "98765432",
                    Password = "Donkey11",
                    Email = @"testuser@benxamariners.onmicrosoft.com",
                    GroupId = GenericIdentifiers._101ID,
                },
                new User
                {
                    Id = GenericIdentifiers._2ID,
                    FirstName = "John",
                    MiddleName = "Bill",
                    LastName = "Kirk",
                    Username = "98560620",
                    Password = "P@ssw0rd",
                    Email = @"kirk@kirkgmail.com",
                    GroupId = GenericIdentifiers._101ID,
                },
                new User
                {
                    Id = GenericIdentifiers._3ID,
                    FirstName = "Mike",
                    MiddleName = "Rhodes",
                    LastName = "Birk",
                    Username = "98765431",
                    Password = "P@ssw0rd",
                    Email = @"mike@mikegmail.com",
                    GroupId = GenericIdentifiers._101ID,
                },
                new User
                {
                    Id = GenericIdentifiers._4ID,
                    FirstName = "Gal",
                    MiddleName = "Varsano",
                    LastName = "Gadot",
                    Username = "65412345",
                    Password = "P@ssw0rd",
                    Email = @"galgadot@hollywood.com",
                    GroupId = GenericIdentifiers._101ID,
                },
                new User
                {
                    Id = GenericIdentifiers._5ID,
                    FirstName = "Jake",
                    MiddleName = "Bobbins",
                    LastName = "Minestaki",
                    Username = "98776654",
                    Password = "P@ssw0rd",
                    Email = @"jake@jakegmail.com",
                    GroupId = GenericIdentifiers._102ID,
                },
                new User
                {
                    Id = GenericIdentifiers._6ID,
                    FirstName = "Robin",
                    MiddleName = "Charles",
                    LastName = "Scherbatsky",
                    Username = "98776655",
                    Password = "P@ssw0rd",
                    Email = @"robin@robingmail.com",
                    GroupId = GenericIdentifiers._102ID,
                }
            };
        }
    }
}
