
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentMind.Core.Entity;
using StudentMind.Core.Utils;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Infracstructure.Seeds
{
    public class DataSeeds
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<DatabaseContext>();
            await context.Database.MigrateAsync(); // Ensure database is up to date

            await SeedRoles(context);
            await SeedUsers(context);
            await SeedSurveyTypes(context);
            await SeedSurveys(context);
            await SeedQuestions(context);
            await SeedChoices(context);
        }

        private static async Task SeedRoles(DatabaseContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "User" },
                    new Role { RoleName = "Psychologist" }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUsers(DatabaseContext context)
        {
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                var userRole = context.Roles.FirstOrDefault(r => r.RoleName == "User");

                if (adminRole != null && userRole != null)
                {
                    context.Users.AddRange(
                        new User { FullName = "Admin User", RoleId = adminRole.Id },
                        new User { FullName = "Regular User", RoleId = userRole.Id }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedSurveyTypes(DatabaseContext context)
        {
            if (!context.SurveyTypes.Any())
            {
                context.SurveyTypes.AddRange(
                    new SurveyType {Name = "Mental Health Assessment" },
                    new SurveyType {Name = "Personality Test" }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSurveys(DatabaseContext context)
        {
            if (!await context.Surveys.AnyAsync())
            {
                var mentalHealthType = await context.SurveyTypes.FirstOrDefaultAsync(s => s.Name == "Mental Health Assessment");
                var personalityType = await context.SurveyTypes.FirstOrDefaultAsync(s => s.Name == "Personality Test");

                if (mentalHealthType != null && personalityType != null)
                {
                    context.Surveys.AddRange(
                        new Survey { Name = "Stress Level Survey", Description = "N/A", TypeId = mentalHealthType.Id },
                        new Survey { Name = "Introvert or Extrovert?", Description = "N/A", TypeId = personalityType.Id }
                    );
                    await context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("SurveyTypes not found. Make sure SeedSurveyTypes is executed first.");
                }
            }
        }


        private static async Task SeedQuestions(DatabaseContext context)
        {
            if (!context.Questions.Any())
            {
                var stressSurvey = context.Surveys.FirstOrDefault(s => s.Name == "Stress Level Survey");
                var personalitySurvey = context.Surveys.FirstOrDefault(s => s.Name == "Introvert or Extrovert?");

                if (stressSurvey != null && personalitySurvey != null)
                {
                    context.Questions.AddRange(
                        new Question { Content = "How often do you feel overwhelmed?" },
                        new Question {                  Content = "Do you prefer working alone or in groups?" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedChoices(DatabaseContext context)
        {
            if (!context.Choices.Any())
            {
                var stressQuestion = context.Questions.FirstOrDefault(q => q.Content == "How often do you feel overwhelmed?");
                var personalityQuestion = context.Questions.FirstOrDefault(q => q.Content == "Do you prefer working alone or in groups?");

                if (stressQuestion != null && personalityQuestion != null)
                {
                    context.Choices.AddRange(
                        new Choice { QuestionId = stressQuestion.Id, Content = "Very Often" },
                        new Choice { QuestionId = stressQuestion.Id, Content = "Sometimes" },
                        new Choice { QuestionId = stressQuestion.Id, Content = "Rarely" },
                        new Choice { QuestionId = personalityQuestion.Id, Content = "Alone" },
                        new Choice { QuestionId = personalityQuestion.Id, Content = "In Groups" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
