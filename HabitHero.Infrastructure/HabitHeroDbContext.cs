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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Tuser>(entity =>
            {
                entity.ToTable("TUsers");
                entity.HasKey(e => e.IntUserId).HasName("TUsers_PK");
                entity.Property(e => e.IntUserId)
                      .HasColumnName("intUserID")
                      .UseIdentityColumn();
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
                entity.Property(e => e.IntAvatarId)
                      .HasColumnName("intAvatarID");
                entity.Property(e => e.IntAppRestrictionId)
                      .HasColumnName("intAppRestrictionID");

                entity.HasOne(e => e.TappRestriction)
                      .WithMany()
                      .HasForeignKey(e => e.IntAppRestrictionId)
                      .HasConstraintName("TUsers_TAppRestrictions_FK")
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Tavatar)
                      .WithMany()
                      .HasForeignKey(e => e.IntAvatarId)
                      .HasConstraintName("TUsers_TAvatars_FK")
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
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

                entity.HasMany(a => a.Tusers)                  
                     .WithOne(u => u.Tavatar)
                     .HasForeignKey(u => u.IntAvatarId)
                     .HasConstraintName("TUsers_TAvatars_FK")
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.Restrict);
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
            });

            modelBuilder.Entity<Tstatus>(entity =>
            {
                entity.ToTable("TStatuses");
                entity.HasKey(e => e.IntStatusId).HasName("TStatuses_PK");
                entity.Property(e => e.IntStatusId)
                      .HasColumnName("intStatusID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrStatus)
                      .HasColumnName("strStatus")
                      .IsRequired()
                      .HasColumnType("varchar(30)")
                      .HasMaxLength(30);
            });

            modelBuilder.Entity<ThabitOccurrence>(entity =>
            {
                entity.ToTable("THabitOccurrences");
                entity.HasKey(e => e.IntHabitOccurrenceId).HasName("THabitOccurrences_PK");
                entity.Property(e => e.IntHabitOccurrenceId)
                      .HasColumnName("intHabitOccurrenceID")
                      .UseIdentityColumn();
                entity.Property(e => e.IntHabitId)
                      .HasColumnName("intHabitID");
                entity.Property(e => e.IntQuestHabitId)
                      .HasColumnName("intQuestHabitID");
                entity.Property(e => e.IntStatusId)
                      .HasColumnName("intStatusID");
                entity.Property(e => e.DtmCompleted)
                      .HasColumnName("dtmCompleted")
                      .HasColumnType("datetime");
            });

            modelBuilder.Entity<TquestHabit>(entity =>
            {
                entity.ToTable("TQuestHabits");
                entity.HasKey(e => e.IntQuestHabitId).HasName("TQuestHabits_PK");
                entity.Property(e => e.IntQuestHabitId)
                      .HasColumnName("intQuestHabitID")
                      .UseIdentityColumn();
                entity.Property(e => e.IntQuestId)
                      .HasColumnName("intQuestID");
                entity.Property(e => e.IntScheduleId)
                      .HasColumnName("intScheduleID");
                entity.Property(e => e.StrHabitName)
                      .HasColumnName("strHabitName")
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasMaxLength(100);
                entity.Property(e => e.DtmReminderTime)
                      .HasColumnName("dtmReminderTime")
                      .HasColumnType("time");
                entity.Property(e => e.DtmStartDate)
                      .HasColumnName("dtmStartDate")
                      .HasColumnType("date");
                entity.Property(e => e.DtmEndDate)
                      .HasColumnName("dtmEndDate")
                      .HasColumnType("date");
                entity.Property(e => e.StrDescription)
                      .HasColumnName("strDescription")
                      .HasColumnType("varchar(500)")
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<Tachievement>(entity =>
            {
                entity.ToTable("TAchievements");
                entity.HasKey(e => e.IntAchievementId).HasName("TAchievements_PK");
                entity.Property(e => e.IntAchievementId)
                      .HasColumnName("intAchievementID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrAchievement)
                      .HasColumnName("strAchievement")
                      .IsRequired()
                      .HasColumnType("varchar(150)")
                      .HasMaxLength(150);
            });

            modelBuilder.Entity<TuserAchievement>(entity =>
            {
                entity.ToTable("TUserAchievements");
                entity.HasKey(e => e.IntUserAchievementId).HasName("TUserAchievements_PK");
                entity.Property(e => e.IntUserAchievementId)
                      .HasColumnName("intUserAchievementID")
                      .UseIdentityColumn();
                entity.Property(e => e.IntUserId)
                      .HasColumnName("intUserID");
                entity.Property(e => e.IntAchievementId)
                      .HasColumnName("intAchievementID");
            });

            modelBuilder.Entity<TitemType>(entity =>
            {
                entity.ToTable("TItemTypes");
                entity.HasKey(e => e.IntItemTypeId).HasName("TItemTypes_PK");
                entity.Property(e => e.IntItemTypeId)
                      .HasColumnName("intItemTypeID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrItemType)
                      .HasColumnName("strItemType")
                      .IsRequired()
                      .HasColumnType("varchar(50)")
                      .HasMaxLength(50);
            });

            modelBuilder.Entity<TappRestriction>(entity =>
            {
                entity.ToTable("TAppRestrictions");
                entity.HasKey(e => e.IntAppRestrictionId).HasName("TAppRestrictions_PK");
                entity.Property(e => e.IntAppRestrictionId)
                      .HasColumnName("intAppRestrictionID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrAppName)
                      .HasColumnName("strAppName")
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasMaxLength(100);
                entity.Property(e => e.BlnRestricted)
                      .HasColumnName("blnRestricted")
                      .HasColumnType("bit")
                      .HasDefaultValue(true);

                entity.HasMany(ar => ar.Tusers)               
                     .WithOne(u => u.TappRestriction)
                     .HasForeignKey(u => u.IntAppRestrictionId)
                     .HasConstraintName("TUsers_TAppRestrictions_FK")
                     .IsRequired(false)
                     .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TitemTier>(entity =>
            {
                entity.ToTable("TItemTiers");
                entity.HasKey(e => e.IntItemTierId).HasName("TItemTiers_PK");
                entity.Property(e => e.IntItemTierId)
                      .HasColumnName("intItemTierID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrItemTier)
                      .HasColumnName("strItemTier")
                      .IsRequired()
                      .HasColumnType("varchar(50)")
                      .HasMaxLength(50);
            });

            modelBuilder.Entity<Titem>(entity =>
            {
                entity.ToTable("TItems");
                entity.HasKey(e => e.IntItemId).HasName("TItems_PK");
                entity.Property(e => e.IntItemId)
                      .HasColumnName("intItemID")
                      .UseIdentityColumn();
                entity.Property(e => e.StrItem)
                      .HasColumnName("strItem")
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasMaxLength(100);
                entity.Property(e => e.IntItemTypeId)
                      .HasColumnName("intItemTypeID");
                entity.Property(e => e.IntItemTierId)
                      .HasColumnName("intItemTierID");
            });

        }
    }
}