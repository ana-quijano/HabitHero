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
                entity.HasOne(e => e.IntAvatar)
                      .WithMany(a => a.Tusers)
                      .HasForeignKey(e => e.IntAvatarId)
                      .HasConstraintName("TUsers_TAvatars_FK");
                entity.HasOne(e => e.IntAppRestriction)
                      .WithMany(r => r.Tusers)
                      .HasForeignKey(e => e.IntAppRestrictionId)
                      .HasConstraintName("TUsers_TAppRestrictions_FK");
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
                entity.HasMany(e => e.ThabitOccurrences)
                      .WithOne(o => o.IntStatus)
                      .HasForeignKey(o => o.IntStatusId)
                      .HasConstraintName("THabitOccurrences_TStatuses_FK");
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
                entity.HasOne(e => e.IntHabit)
                      .WithMany(h => h.ThabitOccurrences)
                      .HasForeignKey(e => e.IntHabitId)
                      .HasConstraintName("THabitOccurrences_THabits_FK");
                entity.HasOne(e => e.IntQuestHabit)
                      .WithMany(qh => qh.ThabitOccurrences)
                      .HasForeignKey(e => e.IntQuestHabitId)
                      .HasConstraintName("THabitOccurrences_TQuestHabits_FK");
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
                entity.HasOne(e => e.IntQuest)
                      .WithMany(q => q.TquestHabits)
                      .HasForeignKey(e => e.IntQuestId)
                      .HasConstraintName("TQuestHabits_TQuests_FK");
                entity.HasOne(e => e.IntSchedule)
                      .WithMany(s => s.TquestHabits)
                      .HasForeignKey(e => e.IntScheduleId)
                      .HasConstraintName("TQuestHabits_TSchedules_FK");
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
                entity.HasMany(e => e.TuserAchievements)
                      .WithOne(ua => ua.IntAchievement)
                      .HasForeignKey(ua => ua.IntAchievementId)
                      .HasConstraintName("TUserAchievements_TAchievements_FK");
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
                entity.HasOne(e => e.IntUser)
                      .WithMany(u => u.TuserAchievements)
                      .HasForeignKey(e => e.IntUserId)
                      .HasConstraintName("TUserAchievements_TUsers_FK");
                entity.HasOne(e => e.IntAchievement)
                      .WithMany(a => a.TuserAchievements)
                      .HasForeignKey(e => e.IntAchievementId)
                      .HasConstraintName("TUserAchievements_TAchievements_FK");
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
                entity.HasMany(e => e.Titems)
                      .WithOne(i => i.IntItemType)
                      .HasForeignKey(i => i.IntItemTypeId)
                      .HasConstraintName("TItems_TItemTypes_FK");
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
                entity.HasMany(e => e.Tusers)
                      .WithOne(u => u.IntAppRestriction)
                      .HasForeignKey(u => u.IntAppRestrictionId)
                      .HasConstraintName("TUsers_TAppRestrictions_FK");
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
                entity.HasMany(e => e.Titems)
                      .WithOne(i => i.IntItemTier)
                      .HasForeignKey(i => i.IntItemTierId)
                      .HasConstraintName("TItems_TItemTiers_FK");
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
                entity.HasOne(e => e.IntItemType)
                      .WithMany(t => t.Titems)
                      .HasForeignKey(e => e.IntItemTypeId)
                      .HasConstraintName("TItems_TItemTypes_FK");
                entity.HasOne(e => e.IntItemTier)
                      .WithMany(t => t.Titems)
                      .HasForeignKey(e => e.IntItemTierId)
                      .HasConstraintName("TItems_TItemTiers_FK");
            });

        }
    }
}