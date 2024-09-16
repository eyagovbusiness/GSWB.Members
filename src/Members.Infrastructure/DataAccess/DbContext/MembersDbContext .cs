using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TGF.CA.Infrastructure.DB.DbContext;

namespace Members.Infrastructure
{
    public class MembersDbContext(DbContextOptions<MembersDbContext> options) : EntitiesDbContext<MembersDbContext>(options)
    {
        public virtual DbSet<Guild> Guilds { get; set; }
        public virtual DbSet<GuildBooster> GuildBoosters { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<IncidentReport> IncidentReports { get; set; }
        public virtual DbSet<Sentence> Sentences { get; set; }
        public virtual DbSet<VerifyCode> VerifyCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder aModelBuilder)
        {
            base.OnModelCreating(aModelBuilder);

            aModelBuilder.Entity<Member>()
            .HasIndex(m => m.DiscordUserId)
            .IsUnique();

            aModelBuilder.Entity<Role>()
            .HasIndex(m => m.Id)
            .IsUnique();
        }

    }
}
