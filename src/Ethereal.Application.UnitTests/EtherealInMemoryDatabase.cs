using System;
using Ethereal.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ethereal.Application.UnitTests
{
    public class EtherealInMemoryDatabase : EtherealDbContext
    {
        public EtherealInMemoryDatabase() : base(null)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
}