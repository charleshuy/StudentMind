using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;

namespace StudentMind.Infrastructure.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<SurveyType> SurveyTypes { get; set; }
        public DbSet<HealthScoreRule> HealthScoreRules { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Role Relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            // Many-to-Many: User - Certificate Relationship
            modelBuilder.Entity<Certificate>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Certificates);

            // Appointment Relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Psychologist)
                .WithMany(u => u.PsychologistAppointments) // Explicit collection in User
                .HasForeignKey(a => a.PsychologistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.UserAppointments) // Explicit collection in User
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Survey - SurveyResponse Relationship
            modelBuilder.Entity<SurveyResponse>()
                .HasOne(sr => sr.Survey)
                .WithMany(s => s.SurveyResponses)
                .HasForeignKey(sr => sr.SurveyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SurveyResponse>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.SurveyResponses)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SurveyResponse>()
                .HasOne(sr => sr.Question)
                .WithMany(q => q.SurveyResponses)
                .HasForeignKey(sr => sr.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SurveyResponse>()
                .HasOne(sr => sr.Choice)
                .WithMany(c => c.SurveyResponses)
                .HasForeignKey(sr => sr.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Survey - SurveyQuestion Relationship
            modelBuilder.Entity<SurveyQuestion>()
                .HasKey(sq => new { sq.SurveyId, sq.QuestionId });

            modelBuilder.Entity<SurveyQuestion>()
                .HasOne(sq => sq.Survey)
                .WithMany(s => s.SurveyQuestions)
                .HasForeignKey(sq => sq.SurveyId);

            modelBuilder.Entity<SurveyQuestion>()
                .HasOne(sq => sq.Question)
                .WithMany(q => q.SurveyQuestions)
                .HasForeignKey(sq => sq.QuestionId);

            // HealthScore - Survey Relationship
            modelBuilder.Entity<HealthScoreRule>()
                .HasOne(hsr => hsr.Survey)
                .WithMany(s => s.HealthScoreRules) // Assuming Survey has a collection of HealthScoreRules
                .HasForeignKey(hsr => hsr.SurveyId)
                .OnDelete(DeleteBehavior.NoAction);

            // Question - Choice Relationship
            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId);

            // Event - UserEvent Relationship
            modelBuilder.Entity<UserEvent>()
                .HasKey(ue => new { ue.ProgramId, ue.UserId });

            // Event - UserEvent Relationship
            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.Event)
                .WithMany(e => e.UserEvents)
                .HasForeignKey(ue => ue.ProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Adjust delete behavior

            modelBuilder.Entity<UserEvent>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Adjust delete behavior


            // SurveyType - Survey Relationship
            modelBuilder.Entity<SurveyType>()
                .HasMany(st => st.Surveys)
                .WithOne(s => s.Type)
                .HasForeignKey(s => s.TypeId);

            // History - User Relationship
            modelBuilder.Entity<History>()
                .HasOne(h => h.User)
                .WithMany(u => u.Histories) // Explicit collection in User
                .HasForeignKey(h => h.UserId);
        }
    }
}
