using HabitHero.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HabitHero.Infrastructure.Data
{
    public class HabitHeroDbContext : DbContext
    {
        public HabitHeroDbContext(DbContextOptions<HabitHeroDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tuser> Tusers { get; set; }
        public DbSet<Tavatar> Tavatars { get; set; }
        public DbSet<Tschedule> Tschedules { get; set; }
        public DbSet<Thabit> Thabits { get; set; }
        public DbSet<Tquest> Tquests { get; set; }
        public DbSet<TuserQuest> TuserQuests { get; set; }
        public DbSet<ThabitGroup> ThabitGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tuser>(entity =>
            {
                entity.ToTable("TUsers");

                entity.HasKey(e => e.IntUserId).HasName("TUsers_PK");
                entity.Property(e => e.IntUserId)
                      .HasColumnName("intUserID")
                      .UseIdentityColumn(); // INTEGER IDENTITY

                entity.Property(e => e.StrUsername)
                      .HasColumnName("strUserName")
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasMaxLength(100);

                entity.Property(e => e.StrEmail)
                      .HasColumnName("strEmail")
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasMaxLength(255);

                entity.Property(e => e.StrPassword)
                      .HasColumnName("strPassword")
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasMaxLength(255);

                entity.Property(e => e.DecPoints)
                      .HasColumnName("decPoints")
                      .HasColumnType("decimal(10,2)")
                      .HasDefaultValue(0m);

                entity.Property(e => e.MonCash)
                      .HasColumnName("monCash")
                      .HasColumnType("money")
                      .HasDefaultValue(0m);

                entity.Property(e => e.BlnAppRestriction)
                      .HasColumnName("blnAppRestriction")
                      .HasColumnType("bit")
                      .HasDefaultValue(false);

                entity.Property(e => e.IntAvatarId)
                      .HasColumnName("intAvatarID");

                entity.HasOne(e => e.IntAvatar)
                      .WithMany(a => a.Tusers)
                      .HasForeignKey(e => e.IntAvatarId)
                      .HasConstraintName("TUsers_TAvatars_FK");
            });

            modelBuilder.Entity<Tavatar>(entity =>
            {
                entity.ToTable("TAvatars");
                entity.HasKey(e => e.IntAvatarId).HasName("TAvatars_PK");
                entity.Property(e => e.IntAvatarId)
                      .HasColumnName("intAvatarID")
                      .UseIdentityColumn();

                entity.Property(e => e.StrAvatar)
                      .HasColumnName("strAvatar")
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasMaxLength(255);
            });

            modelBuilder.Entity<Tschedule>(entity =>
            {
                entity.ToTable("TSchedules");
                entity.HasKey(e => e.IntScheduleId).HasName("TSchedules_PK");
                entity.Property(e => e.IntScheduleId)
                      .HasColumnName("intScheduleID")
                      .UseIdentityColumn();

                entity.Property(e => e.StrSchedule)
                      .HasColumnName("strSchedule")
                      .IsRequired()
                      .HasColumnType("varchar(10)")
                      .HasMaxLength(10);
            });

            modelBuilder.Entity<Thabit>(entity =>
            {
                entity.ToTable("THabits");
                entity.HasKey(e => e.IntHabitId).HasName("THabits_PK");
                entity.Property(e => e.IntHabitId)
                      .HasColumnName("intHabitID")
                      .UseIdentityColumn();

                entity.Property(e => e.IntUserId).HasColumnName("intUserID");
                entity.Property(e => e.IntScheduleId).HasColumnName("intScheduleID");

                entity.Property(e => e.StrHabit)
                      .HasColumnName("strHabit")
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasMaxLength(255);

                entity.Property(e => e.StrDescription)
                      .HasColumnName("strDescription")
                      .HasColumnType("varchar(500)")
                      .HasMaxLength(500);

                entity.Property(e => e.DtmStartDate)
                      .HasColumnName("dtmStartDate")
                      .HasColumnType("date");

                entity.Property(e => e.DtmEndDate)
                      .HasColumnName("dtmEndDate")
                      .HasColumnType("date");

                entity.Property(e => e.DtmReminderTime)
                      .HasColumnName("dtmReminderTime")
                      .HasColumnType("time");

                entity.HasOne(e => e.IntUser)
                      .WithMany(u => u.Thabits)
                      .HasForeignKey(e => e.IntUserId)
                      .HasConstraintName("THabits_TUsers_FK");

                entity.HasOne(e => e.IntSchedule)
                      .WithMany(s => s.Thabits)
                      .HasForeignKey(e => e.IntScheduleId)
                      .HasConstraintName("THabits_TSchedules_FK");
            });

            modelBuilder.Entity<Tquest>(entity =>
            {
                entity.ToTable("TQuests");
                entity.HasKey(e => e.IntQuestId).HasName("TQuests_PK");
                entity.Property(e => e.IntQuestId)
                      .HasColumnName("intQuestID")
                      .UseIdentityColumn();

                entity.Property(e => e.StrQuestName)
                      .HasColumnName("strQuestName")
                      .IsRequired()
                      .HasColumnType("varchar(150)")
                      .HasMaxLength(150);

                entity.Property(e => e.MonMoneyPot)
                      .HasColumnName("monMoneyPot")
                      .HasColumnType("money");

                entity.Property(e => e.DecPointsPot)
                      .HasColumnName("decPointsPot")
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.DtmStartDate)
                      .HasColumnName("dtmStartDate")
                      .HasColumnType("date");

                entity.Property(e => e.DtmEndDate)
                      .HasColumnName("dtmEndDate")
                      .HasColumnType("date");
            });

            modelBuilder.Entity<TuserQuest>(entity =>
            {
                entity.ToTable("TUserQuests");
                entity.HasKey(e => e.IntUserQuestId).HasName("TUserQuests_PK");
                entity.Property(e => e.IntUserQuestId)
                      .HasColumnName("intUserQuestID")
                      .UseIdentityColumn();

                entity.Property(e => e.IntUserId).HasColumnName("intUserID");
                entity.Property(e => e.IntQuestId).HasColumnName("intQuestID");

                entity.HasOne(e => e.IntUser)
                      .WithMany(u => u.TuserQuests)
                      .HasForeignKey(e => e.IntUserId)
                      .HasConstraintName("TUserQuests_TUsers_FK");

                entity.HasOne(e => e.IntQuest)
                      .WithMany(q => q.TuserQuests)
                      .HasForeignKey(e => e.IntQuestId)
                      .HasConstraintName("TUserQuests_TQuests_FK");
            });
        }
    }
}