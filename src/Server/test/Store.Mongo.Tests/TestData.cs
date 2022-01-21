using System;
using System.Collections.Generic;
using IdOps.Model;

namespace IdOps.Server.Store.Mongo.Tests
{
    public static class TestData
    {
        public static List<Client> GetTestClients() =>
            new()
            {
                new()
                {
                    Id = Guid.Parse("00000000-0001-0000-0000-000000000000"),
                    ClientId = "Id1",
                    AllowedScopes = new List<ClientScope>
                    {
                        new() { Id =  Guid.Parse("00000000-0001-0000-0000-000000000000") },
                        new() { Id =  Guid.Parse("00000000-0002-0000-0000-000000000000") }
                    }
                },
                new()
                {
                    Id = Guid.Parse("00000000-0002-0000-0000-000000000000"),
                    ClientId = "Id2",
                    AllowedScopes = new List<ClientScope>
                    {
                        new() { Id =  Guid.Parse("00000000-0001-0000-0000-000000000000") }
                    }
                },
                new()
                {
                    Id = Guid.Parse("00000000-0003-0000-0000-000000000000"),
                    ClientId = "Id3",
                    AllowedScopes = new List<ClientScope>
                    {
                        new() { Id =  Guid.Parse("00000000-0002-0000-0000-000000000000") }
                    }
                },
                new()
                {
                    Id = Guid.Parse("00000000-0004-0000-0000-000000000000"),
                    ClientId = "Id4",
                }
            };
    }
}
