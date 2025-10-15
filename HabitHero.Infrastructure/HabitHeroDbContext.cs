using Microsoft.EntityFrameworkCore;

namespace HabitHero.Infrastructure
{
    // ---------- ENTITIES ----------
    public class Schedule
    {
        public int Id { get; set; }                // TSchedules.intScheduleID
        public string Name { get; set; } = "";     // TSchedules.strSchedule

        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    }

    public class Avatar
    {
        public int Id { get; set; }                // TAvatars.intAvatarID
        public string Name { get; set; } = "";     // TAvatars.strAvatar

        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public class User
    {
        public int Id { get; set; }                        // TUsers.intUserID
        public string UserName { get; set; } = "";         // TUsers.strUserName
        public string Email { get; set; } = "";            // TUsers.strEmail
        public string Password { get; set; } = "";         // TUsers.strPassword
        public decimal Points { get; set; }                // TUsers.decPoints (decimal(10,2))
        public decimal Cash { get; set; }                  // TUsers.monCash (money)
        public bool AppRestriction { get; set; }           // TUsers.blnAppRestriction
        public int? AvatarId { get; set; }                 // TUsers.intAvatarID (nullable FK)

        public Avatar? Avatar { get; set; }
        public ICollection<Habit> Habits { get; set; } = new List<Habit>();
        public ICollection<UserQuest> UserQuests { get; set; } = new List<UserQuest>();
    }

    public class Habit
    {
        public int Id { get; set; }                        // THabits.intHabitID
        public int UserId { get; set; }                    // THabits.intUserID
        public int ScheduleId { get; set; }                // THabits.intScheduleID
        public string Name { get; set; } = "";             // THabits.strHabit
        public string? Description { get; set; }           // THabits.strDescription
        public DateTime? StartDate { get; set; }           // THabits.dtmStartDate (DATE)
        public DateTime? EndDate { get; set; }             // THabits.dtmEndDate (DATE)
        public TimeSpan? ReminderTime { get; set; }        // THabits.dtmReminderTime (TIME)

        public User? User { get; set; }
        public Schedule? Schedule { get; set; }
    }

    public class Quest
    {
        public int Id { get; set; }                        // TQuests.intQuestID
        public string Name { get; set; } = "";             // TQuests.strQuestName
        public decimal? MoneyPot { get; set; }             // TQuests.monMoneyPot (money)
        public decimal? PointsPot { get; set; }            // TQuests.decPointsPot (decimal(10,2))
        public DateTime? StartDate { get; set; }           // TQuests.dtmStartDate (DATE)
        public DateTime? EndDate { get; set; }             // TQuests.dtmEndDate (DATE)

        public ICollection<UserQuest> UserQuests { get; set; } = new List<UserQuest>();
    }

    public class UserQuest
    {
        public int Id { get; set; }                        // TUserQuests.intUserQuestID
        public int UserId { get; set; }                    // TUserQuests.intUserID
        public int QuestId { get; set; }                   // TUserQuests.intQuestID

        public User? User { get; set; }
        public Quest? Quest { get; set; }
    }

    // ---------- DB CONTEXT ----------
    public class HabitHeroDbContext : DbContext
    {
        public HabitHeroDbContext(DbContextOptions<HabitHeroDbContext> options) : base(options) { }

        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Habit> Habits => Set<Habit>();
        public DbSet<Quest> Quests => Set<Quest>();
        public DbSet<UserQuest> UserQuests => Set<UserQuest>();

        protected override void OnModelCreating(ModelBuilder model)
        {
            // TSchedules
            model.Entity<Schedule>(e =>
            {
                e.ToTable("TSchedules");
                e.HasKey(x => x.Id).HasName("TSchedules_PK");
                e.Property(x => x.Id).HasColumnName("intScheduleID").ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasColumnName("strSchedule").HasMaxLength(10).IsRequired();
            });

            // TAvatars
            model.Entity<Avatar>(e =>
            {
                e.ToTable("TAvatars");
                e.HasKey(x => x.Id).HasName("TAvatars_PK");
                e.Property(x => x.Id).HasColumnName("intAvatarID").ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasColumnName("strAvatar").HasMaxLength(255).IsRequired();
            });

            // TUsers
            model.Entity<User>(e =>
            {
                e.ToTable("TUsers");
                e.HasKey(x => x.Id).HasName("TUsers_PK");
                e.Property(x => x.Id).HasColumnName("intUserID").ValueGeneratedOnAdd();
                e.Property(x => x.UserName).HasColumnName("strUserName").HasMaxLength(100).IsRequired();
                e.Property(x => x.Email).HasColumnName("strEmail").HasMaxLength(255).IsRequired();
                e.Property(x => x.Password).HasColumnName("strPassword").HasMaxLength(255).IsRequired();
                e.Property(x => x.Points).HasColumnName("decPoints").HasColumnType("decimal(10,2)");
                e.Property(x => x.Cash).HasColumnName("monCash").HasColumnType("money");
                e.Property(x => x.AppRestriction).HasColumnName("blnAppRestriction");
                e.Property(x => x.AvatarId).HasColumnName("intAvatarID");

                // FK: TUsers -> TAvatars (nullable)
                e.HasOne(x => x.Avatar)
                 .WithMany(a => a.Users)
                 .HasForeignKey(x => x.AvatarId)
                 .HasConstraintName("TUsers_TAvatars_FK");
            });

            // THabits
            model.Entity<Habit>(e =>
            {
                e.ToTable("THabits");
                e.HasKey(x => x.Id).HasName("THabits_PK");
                e.Property(x => x.Id).HasColumnName("intHabitID").ValueGeneratedOnAdd();
                e.Property(x => x.UserId).HasColumnName("intUserID");
                e.Property(x => x.ScheduleId).HasColumnName("intScheduleID");
                e.Property(x => x.Name).HasColumnName("strHabit").HasMaxLength(255).IsRequired();
                e.Property(x => x.Description).HasColumnName("strDescription").HasMaxLength(500);
                e.Property(x => x.StartDate).HasColumnName("dtmStartDate");     // DATE -> DateTime?
                e.Property(x => x.EndDate).HasColumnName("dtmEndDate");         // DATE -> DateTime?
                e.Property(x => x.ReminderTime).HasColumnName("dtmReminderTime"); // TIME -> TimeSpan?

                // FK: THabits -> TUsers
                e.HasOne(x => x.User)
                 .WithMany(u => u.Habits)
                 .HasForeignKey(x => x.UserId)
                 .HasConstraintName("THabits_TUsers_FK");

                // FK: THabits -> TSchedules
                e.HasOne(x => x.Schedule)
                 .WithMany(s => s.Habits)
                 .HasForeignKey(x => x.ScheduleId)
                 .HasConstraintName("THabits_TSchedules_FK");
            });

            // TQuests
            model.Entity<Quest>(e =>
            {
                e.ToTable("TQuests");
                e.HasKey(x => x.Id).HasName("TQuests_PK");
                e.Property(x => x.Id).HasColumnName("intQuestID").ValueGeneratedOnAdd();
                e.Property(x => x.Name).HasColumnName("strQuestName").HasMaxLength(150).IsRequired();
                e.Property(x => x.MoneyPot).HasColumnName("monMoneyPot").HasColumnType("money");
                e.Property(x => x.PointsPot).HasColumnName("decPointsPot").HasColumnType("decimal(10,2)");
                e.Property(x => x.StartDate).HasColumnName("dtmStartDate");
                e.Property(x => x.EndDate).HasColumnName("dtmEndDate");
            });

            // TUserQuests
            model.Entity<UserQuest>(e =>
            {
                e.ToTable("TUserQuests");
                e.HasKey(x => x.Id).HasName("TUserQuests_PK"); // matches your script's constraint case
                e.Property(x => x.Id).HasColumnName("intUserQuestID").ValueGeneratedOnAdd();
                e.Property(x => x.UserId).HasColumnName("intUserID");
                e.Property(x => x.QuestId).HasColumnName("intQuestID");

                // FK: TUserQuests -> TUsers
                e.HasOne(x => x.User)
                 .WithMany(u => u.UserQuests)
                 .HasForeignKey(x => x.UserId)
                 .HasConstraintName("TUserQuests_TUsers_FK");

                // FK: TUserQuests -> TQuests
                e.HasOne(x => x.Quest)
                 .WithMany(q => q.UserQuests)
                 .HasForeignKey(x => x.QuestId)
                 .HasConstraintName("TUserQuests_TQuests_FK");
            });
        }
    }
}
