using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TGF.CA.Infrastructure.DB.DbContext;

namespace Members.Infrastructure.DataAccess.DbContext
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
                // Map RoleId property to PostgreSQL numeric(20,0) and apply the converter because ulong is not directly supported in postgres
                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnType("numeric(20,0)")           // Store as numeric in PostgreSQL
                    .HasConversion(ulongToDecimalConverter);  // Use the ValueConverter

                // Configure GuildId property similarly
                entity.Property(e => e.GuildId)
                    .ValueGeneratedNever()
                    .HasColumnType("numeric(20,0)")
                    .HasConversion(ulongToDecimalConverter);

                // Define composite key based on RoleId and GuildId
                entity.HasKey(e => new { e.RoleId, e.GuildId });
            });

            modelBuilder.Entity<Guild>(entity =>
            {
                // Map Id property to PostgreSQL numeric(20,0) and apply the converter because ulong is not directly supported in postgres
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnType("numeric(20,0)")           // Store as numeric in PostgreSQL
                    .HasConversion(ulongToDecimalConverter);  // Use the ValueConverter
            });

            modelBuilder.Entity<Member>(entity =>
            {
                // Explicitly define GuildId and UserId as the composite key
                entity.HasKey(e => new { e.GuildId, e.UserId });

                // Configure GuildId and UserId with the required conversions
                entity.Property(e => e.GuildId)
                    .HasColumnType("numeric(20,0)")
                    .HasConversion(ulongToDecimalConverter)
                    .IsRequired(); // Ensure it's marked as required

                entity.Property(e => e.UserId)
                    .HasColumnType("numeric(20,0)")
                    .HasConversion(ulongToDecimalConverter)
                    .IsRequired(); // Ensure it's marked as required

                // Configure the foreign key relationship with Guild using GuildId
                entity.HasOne<Guild>()
                    .WithMany()                       // No navigation property in Guild
                    .HasForeignKey(m => m.GuildId)    // Use GuildId as foreign key
                    .IsRequired();
            });
        }

    }
}
