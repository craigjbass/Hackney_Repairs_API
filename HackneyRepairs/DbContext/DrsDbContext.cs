using System;
using Microsoft.EntityFrameworkCore;

namespace HackneyRepairs.DbContext
{
    public class DrsDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DrsDbContext(DbContextOptions<DrsDbContext> options) : base(options)
        {
        }
    }
}
