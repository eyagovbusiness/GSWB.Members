using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Members.Infrastructure
{
    public class MembersDbContext : DbContext
    {
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<IncidentReport> IncidentReports { get; set; }
        public virtual DbSet<Sentence> Sentences { get; set; }
        public virtual DbSet<VerifyCode> VerifyCodes { get; set; }

        public MembersDbContext(DbContextOptions<MembersDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder aModelBuilder)
        {
            base.OnModelCreating(aModelBuilder);

            aModelBuilder.Entity<Member>()
            .HasIndex(m => m.DiscordUserId)
            .IsUnique();

            aModelBuilder.Entity<Role>()
            .HasIndex(m => m.DiscordRoleId)
            .IsUnique();
        }


    }
}
