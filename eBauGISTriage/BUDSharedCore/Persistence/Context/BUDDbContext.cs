using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BUDSharedCore.Persistence.Model.Context
{
    public partial class BUDDbContext : BaseDbContext
    {
        public BUDDbContext(DbContextOptions<BUDDbContext> options, DbContextConfig<BUDDbContext> config) : base(options, config)
        {
        }

        public virtual DbSet<Amt> Amt { get; set; }
        public virtual DbSet<Mitarbeiter> Mitarbeiter { get; set; }
        public virtual DbSet<GemeindenBL> GemeindenBL { get; set; }
        public virtual DbSet<VMaOrganisation> MaOrganisationen { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "BUD");

            modelBuilder.Entity<Amt>(entity =>
            {
                entity.Property(e => e.Kuerzel).IsUnicode(false);
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.Bereich).IsUnicode(false);
            });

            modelBuilder.Entity<Mitarbeiter>(entity =>
            {
                entity.HasAlternateKey(e => e.ADUsername);

                entity.Property(e => e.Vorname).IsUnicode(false);
                entity.Property(e => e.Nachname).IsUnicode(false);
                entity.Property(e => e.Position).IsUnicode(false);
                entity.Property(e => e.Firma).IsUnicode(false);
                entity.Property(e => e.Abteilung).IsUnicode(false);
                entity.Property(e => e.Buero).IsUnicode(false);
                entity.Property(e => e.Postfach).IsUnicode(false);
                entity.Property(e => e.Strasse).IsUnicode(false);
                entity.Property(e => e.Ort).IsUnicode(false);
                entity.Property(e => e.PLZ).IsUnicode(false);
                entity.Property(e => e.TelefonNr).IsUnicode(false);
                entity.Property(e => e.MobiltelefonNr).IsUnicode(false);
                entity.Property(e => e.Pager).IsUnicode(false);
                entity.Property(e => e.ADUsername).IsUnicode(false);
                entity.Property(e => e.Email).IsUnicode(false);
                entity.Property(e => e.Hostname).IsUnicode(false);
                entity.Property(e => e.IP).IsUnicode(false);
                entity.Property(e => e.Logon).IsUnicode(false);
                entity.Property(e => e.Funktion).IsUnicode(false);
                entity.Property(e => e.MobileVPN).IsUnicode(false);

                entity.HasOne(d => d.Dienststelle)
                    .WithMany(p => p.Mitarbeiter)
                    .HasForeignKey(d => d.DstId);
            });

            modelBuilder.Entity<GemeindenBL>(entity =>
            {
                entity.Property(e => e.Plz).IsUnicode(false);
                entity.Property(e => e.Name).IsUnicode(false);
                entity.Property(e => e.Bezirk).IsUnicode(false);
                entity.Property(e => e.Kanton).IsUnicode(false);
                entity.Property(e => e.Land).IsUnicode(false);
            });

            modelBuilder.Entity<VMaOrganisation>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.GruppenId });

                entity.Property(e => e.Vorname).IsUnicode(false);
                entity.Property(e => e.Nachname).IsUnicode(false);
                entity.Property(e => e.Position).IsUnicode(false);
                entity.Property(e => e.Firma).IsUnicode(false);
                entity.Property(e => e.Abteilung).IsUnicode(false);
                entity.Property(e => e.Buero).IsUnicode(false);
                entity.Property(e => e.Postfach).IsUnicode(false);
                entity.Property(e => e.Strasse).IsUnicode(false);
                entity.Property(e => e.Ort).IsUnicode(false);
                entity.Property(e => e.PLZ).IsUnicode(false);
                entity.Property(e => e.TelefonNr).IsUnicode(false);
                entity.Property(e => e.MobiltelefonNr).IsUnicode(false);
                entity.Property(e => e.Pager).IsUnicode(false);
                entity.Property(e => e.ADUsername).IsUnicode(false);
                entity.Property(e => e.Email).IsUnicode(false);
                entity.Property(e => e.GruppenKuerzel).IsUnicode(false);
                entity.Property(e => e.Gruppe).IsUnicode(false);

            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public static DbContextInfo GetDbContextInfo()
        {
            return new DbContextInfo("BUD", typeof(BUDDbContext));
        }
    }
}