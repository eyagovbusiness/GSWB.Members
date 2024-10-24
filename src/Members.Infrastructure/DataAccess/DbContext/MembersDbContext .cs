using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TGF.CA.Infrastructure.DB.DbContext;

namespace Members.Infrastructure
{
    public class MembersDbContext(DbContextOptions<MembersDbContext> options) : EntitiesDbContext<MembersDbContext>(options)
    {
        public virtual DbSet<Guild> Guilds { get; set; }
        protected virtual DbSet<GuildBooster> GuildBoosters { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        protected virtual DbSet<Role> Roles { get; set; }
        protected virtual DbSet<IncidentReport> IncidentReports { get; set; }
        protected virtual DbSet<Sentence> Sentences { get; set; }
        protected virtual DbSet<VerifyCode> VerifyCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the ValueConverter to convert ulong to decimal and back
            var ulongToDecimalConverter = new ValueConverter<ulong, decimal>(
                v => Convert.ToDecimal(v),    // Convert ulong to decimal for storage
                v => Convert.ToUInt64(v)      // Convert decimal back to ulong
            );

            modelBuilder.Entity<Role>(entity =>
            {
                // Map Id property to PostgreSQL numeric(20,0) and apply the converter because ulong is not directly supported in postgres
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnType("numeric(20,0)")           // Store as numeric in PostgreSQL
                    .HasConversion(ulongToDecimalConverter);  // Use the ValueConverter
            });

            modelBuilder.Entity<Guild>(entity =>
            {
                // Map Id property to PostgreSQL numeric(20,0) and apply the converter because ulong is not directly supported in postgres
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnType("numeric(20,0)")           // Store as numeric in PostgreSQL
                    .HasConversion(ulongToDecimalConverter);  // Use the ValueConverter
            });

            modelBuilder.Entity<Member>()
            .HasOne<Guild>()                  // No navigation property in Member
            .WithMany()                       // No navigation property in Guild
            .HasForeignKey(m => m.GuildId)    // GuildId is the foreign key
            .IsRequired();                    // GuildId is required
        }

    }
}
