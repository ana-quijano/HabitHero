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
        public DbSet<TquestHabit> TquestHabits { get; set; }
        public DbSet<ThabitOccurrence> ThabitOccurrences { get; set; }
        public DbSet<Tstatus> Tstatuses { get; set; }
        public DbSet<Tachievement> Tachievements { get; set; }
        public DbSet<TuserAchievement> TuserAchievements { get; set; }
        public DbSet<Titem> Titems { get; set; }
        public DbSet<TitemTier> TitemTiers { get; set; }
        public DbSet<TitemType> TitemTypes { get; set; }
        public DbSet<TappRestriction> TappRestrictions { get; set; }
        public DbSet<TavatarItem> TavatarItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------- TUsers ----------------
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

                // FK: Users -> AppRestrictions (optional)
                entity.HasOne(e => e.TappRestriction)
                      .WithMany(ar => ar.Tusers)
                      .HasForeignKey(e => e.IntAppRestrictionId)
                      .HasConstraintName("TUsers_TAppRestrictions_FK")
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: Users -> Avatars (optional)
                entity.HasOne(e => e.Tavatar)
                      .WithMany(a => a.Tusers)
                      .HasForeignKey(e => e.IntAvatarId)
                      .HasConstraintName("TUsers_TAvatars_FK")
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);

                // One User -> many Habits
                entity.HasMany(u => u.Thabits)
                      .WithOne(h => h.Tuser)
                      .HasForeignKey(h => h.IntUserId)
                      .HasConstraintName("THabits_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One User -> many UserQuests
                entity.HasMany(u => u.TuserQuests)
                      .WithOne(uq => uq.Tuser)
                      .HasForeignKey(uq => uq.IntUserId)
                      .HasConstraintName("TUserQuests_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One User -> many UserAchievements
                entity.HasMany(u => u.TuserAchievements)
                      .WithOne(ua => ua.Tuser)
                      .HasForeignKey(ua => ua.IntUserId)
                      .HasConstraintName("TUserAchievements_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TAvatars ----------------
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

            // ---------------- TSchedules ----------------
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

                // One Schedule -> many Habits
                entity.HasMany(s => s.Thabits)
                      .WithOne(h => h.Tschedule)
                      .HasForeignKey(h => h.IntScheduleId)
                      .HasConstraintName("THabits_TSchedules_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One Schedule -> many QuestHabits
                entity.HasMany(s => s.TquestHabits)
                      .WithOne(qh => qh.Tschedule)
                      .HasForeignKey(qh => qh.IntScheduleId)
                      .HasConstraintName("TQuestHabits_TSchedules_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- THabits ----------------
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

                // FK: Habits -> Users
                entity.HasOne(h => h.Tuser)
                      .WithMany(u => u.Thabits)
                      .HasForeignKey(h => h.IntUserId)
                      .HasConstraintName("THabits_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: Habits -> Schedules
                entity.HasOne(h => h.Tschedule)
                      .WithMany(s => s.Thabits)
                      .HasForeignKey(h => h.IntScheduleId)
                      .HasConstraintName("THabits_TSchedules_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One Habit -> many HabitOccurrences
                entity.HasMany(h => h.ThabitOccurrences)
                      .WithOne(ho => ho.Thabit)
                      .HasForeignKey(ho => ho.IntHabitId)
                      .HasConstraintName("THabitOccurrences_THabits_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TQuests ----------------
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

                // One Quest -> many UserQuests
                entity.HasMany(q => q.TuserQuests)
                      .WithOne(uq => uq.Tquest)
                      .HasForeignKey(uq => uq.IntQuestId)
                      .HasConstraintName("TUserQuests_TQuests_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One Quest -> many QuestHabits
                entity.HasMany(q => q.TquestHabits)
                      .WithOne(qh => qh.Tquest)
                      .HasForeignKey(qh => qh.IntQuestId)
                      .HasConstraintName("TQuestHabits_TQuests_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TUserQuests ----------------
            modelBuilder.Entity<TuserQuest>(entity =>
            {
                entity.ToTable("TUserQuests");
                entity.HasKey(e => e.IntUserQuestId).HasName("TUserQuests_PK");
                entity.Property(e => e.IntUserQuestId)
                      .HasColumnName("intUserQuestID")
                      .UseIdentityColumn();
                entity.Property(e => e.IntUserId).HasColumnName("intUserID");
                entity.Property(e => e.IntQuestId).HasColumnName("intQuestID");

                // FK: UserQuests -> Users
                entity.HasOne(uq => uq.Tuser)
                      .WithMany(u => u.TuserQuests)
                      .HasForeignKey(uq => uq.IntUserId)
                      .HasConstraintName("TUserQuests_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: UserQuests -> Quests
                entity.HasOne(uq => uq.Tquest)
                      .WithMany(q => q.TuserQuests)
                      .HasForeignKey(uq => uq.IntQuestId)
                      .HasConstraintName("TUserQuests_TQuests_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TStatuses ----------------
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

                // One Status -> many HabitOccurrences
                entity.HasMany(s => s.ThabitOccurrences)
                      .WithOne(ho => ho.Tstatus)
                      .HasForeignKey(ho => ho.IntStatusId)
                      .HasConstraintName("THabitOccurrences_TStatuses_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- THabitOccurrences ----------------
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
                entity.Property(e => e.DtmDate)
                      .HasColumnName("dtmDate")
                      .HasColumnType("datetime");

                // FK: HabitOccurrences -> Habits
                entity.HasOne(ho => ho.Thabit)
                      .WithMany(h => h.ThabitOccurrences)
                      .HasForeignKey(ho => ho.IntHabitId)
                      .HasConstraintName("THabitOccurrences_THabits_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: HabitOccurrences -> QuestHabits
                entity.HasOne(ho => ho.TquestHabit)
                      .WithMany(qh => qh.ThabitOccurrences)
                      .HasForeignKey(ho => ho.IntQuestHabitId)
                      .HasConstraintName("THabitOccurrences_TQuestHabits_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: HabitOccurrences -> Statuses
                entity.HasOne(ho => ho.Tstatus)
                      .WithMany(s => s.ThabitOccurrences)
                      .HasForeignKey(ho => ho.IntStatusId)
                      .HasConstraintName("THabitOccurrences_TStatuses_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TQuestHabits ----------------
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

                // FK: QuestHabits -> Quests
                entity.HasOne(qh => qh.Tquest)
                      .WithMany(q => q.TquestHabits)
                      .HasForeignKey(qh => qh.IntQuestId)
                      .HasConstraintName("TQuestHabits_TQuests_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: QuestHabits -> Schedules
                entity.HasOne(qh => qh.Tschedule)
                      .WithMany(s => s.TquestHabits)
                      .HasForeignKey(qh => qh.IntScheduleId)
                      .HasConstraintName("TQuestHabits_TSchedules_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // One QuestHabit -> many HabitOccurrences
                entity.HasMany(qh => qh.ThabitOccurrences)
                      .WithOne(ho => ho.TquestHabit)
                      .HasForeignKey(ho => ho.IntQuestHabitId)
                      .HasConstraintName("THabitOccurrences_TQuestHabits_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TAchievements ----------------
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

                // One Achievement -> many UserAchievements
                entity.HasMany(a => a.TuserAchievements)
                      .WithOne(ua => ua.Tachievement)
                      .HasForeignKey(ua => ua.IntAchievementId)
                      .HasConstraintName("TUserAchievements_TAchievements_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TUserAchievements ----------------
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

                // FK: UserAchievements -> Users
                entity.HasOne(ua => ua.Tuser)
                      .WithMany(u => u.TuserAchievements)
                      .HasForeignKey(ua => ua.IntUserId)
                      .HasConstraintName("TUserAchievements_TUsers_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: UserAchievements -> Achievements
                entity.HasOne(ua => ua.Tachievement)
                      .WithMany(a => a.TuserAchievements)
                      .HasForeignKey(ua => ua.IntAchievementId)
                      .HasConstraintName("TUserAchievements_TAchievements_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TItemTypes ----------------
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

                // One ItemType -> many Items
                entity.HasMany(it => it.Titems)
                      .WithOne(i => i.TitemType)
                      .HasForeignKey(i => i.IntItemTypeId)
                      .HasConstraintName("TItems_TItemTypes_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TAppRestrictions ----------------
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

            // ---------------- TItemTiers ----------------
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

                // One ItemTier -> many Items
                entity.HasMany(t => t.Titems)
                      .WithOne(i => i.TitemTier)
                      .HasForeignKey(i => i.IntItemTierId)
                      .HasConstraintName("TItems_TItemTiers_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TItems ----------------
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
                      .HasColumnType("VARCHAR(100)")
                      .HasMaxLength(100);
                entity.Property(e => e.StrSlug)
                      .HasColumnName("strSlug")
                      .HasColumnType("VARCHAR(255)")
                      .HasMaxLength(100);
                entity.Property(e => e.IntPrice)
                      .HasColumnName("intPrice")
                      .HasColumnType("INTEGER");
                entity.Property(e => e.IntItemTypeId)
                      .HasColumnName("intItemTypeID");
                entity.Property(e => e.IntItemTierId)
                      .HasColumnName("intItemTierID");

                // FK: Items -> ItemTypes
                entity.HasOne(i => i.TitemType)
                      .WithMany(it => it.Titems)
                      .HasForeignKey(i => i.IntItemTypeId)
                      .HasConstraintName("TItems_TItemTypes_FK")
                      .OnDelete(DeleteBehavior.Restrict);

                // FK: Items -> ItemTiers
                entity.HasOne(i => i.TitemTier)
                      .WithMany(t => t.Titems)
                      .HasForeignKey(i => i.IntItemTierId)
                      .HasConstraintName("TItems_TItemTiers_FK")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------- TAvatarItems ----------------
            modelBuilder.Entity<TavatarItem>(entity =>
            {
                entity.ToTable("TAvatarItems");
                entity.HasKey(e => e.IntAvatarItemId)
                      .HasName("TAvatarItems_PK");
                entity.Property(e => e.IntAvatarItemId)
                      .HasColumnName("intAvatarItemID")
                      .UseIdentityColumn();
                entity.Property(e => e.IntAvatarId)
                      .HasColumnName("intAvatarID")
                      .IsRequired();
                entity.Property(e => e.IntItemId)
                      .HasColumnName("intItemID")
                      .IsRequired();
                entity.Property(e => e.IntQuantity)
                      .HasColumnName("intQuantity")
                      .IsRequired();

                entity.HasOne(e => e.Tavatar)
                      .WithMany(a => a.TavatarItems) 
                      .HasForeignKey(e => e.IntAvatarId)
                      .HasConstraintName("TAvatarItems_TAvatars_FK")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Titem) 
                      .WithMany(i => i.TavatarItems) 
                      .HasForeignKey(e => e.IntItemId)
                      .HasConstraintName("TAvatarItems_TItems_FK")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}