using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ProjectService.WebAPI.Data
{
    public class TestProjectContext : DbContext
    {
        public TestProjectContext(DbContextOptions<TestProjectContext> options)
            : base(options)
        { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime AddedDate { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }

    public class Project
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
