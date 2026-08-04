using Codx.Temple.Application.Abstractions;
using Codx.Temple.Domain.Entities;
using Codx.Temple.Domain.Enums;
using Codx.Temple.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codx.Temple.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminEmail = configuration["Admin:Email"];
        var adminPassword = configuration["Admin:Password"];

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            return;

        var exists = await db.Users.AnyAsync(u => u.Email == adminEmail.ToLowerInvariant());
        if (exists)
            return;

        var adminUser = User.CreateWithPassword(adminEmail, passwordHasher.Hash(adminPassword), "Admin");
        db.Users.Add(adminUser);

        var adminRole = RoleAssignment.Create(adminUser.Id, Role.Admin, adminUser.Id);
        db.RoleAssignments.Add(adminRole);

        await db.SaveChangesAsync();

        await SeedLessonsAsync(db);
    }

    private static async Task SeedLessonsAsync(AppDbContext db)
    {
        if (await db.Lessons.AnyAsync())
            return;

        var lesson1 = Lesson.Create(1, "God's Eternal Purpose");
        db.Lessons.Add(lesson1);

        var version1 = LessonVersion.Create(lesson1.Id, 1);
        db.LessonVersions.Add(version1);

        var section1 = LessonNode.Create(version1.Id, null, 1, 0, "Before the Foundation of the World", "Understanding that God's plan for His people existed before time began — covering Ephesians 1:4 and related passages.");
        db.LessonNodes.Add(section1);

        var q1 = Question.Create(section1.Id, 0, QuestionType.Essay, "What does it mean that God chose His people \"before the foundation of the world\"?");
        db.Questions.Add(q1);

        var q2 = Question.Create(section1.Id, 1, QuestionType.Essay, "How does knowing you were chosen before time began affect your daily life?");
        db.Questions.Add(q2);

        var section2 = LessonNode.Create(version1.Id, null, 1, 1, "Manifested in Christ", "Exploring how God's purpose was revealed through Jesus Christ's life, death, and resurrection — covering Ephesians 1:7-10.");
        db.LessonNodes.Add(section2);

        var sub1 = LessonNode.Create(version1.Id, section2.Id, 2, 0, "Redemption Through His Blood", "The price of our salvation — examining the meaning of redemption and forgiveness.");
        db.LessonNodes.Add(sub1);

        var q3 = Question.Create(sub1.Id, 0, QuestionType.Essay, "What is redemption, and why was blood required?");
        db.Questions.Add(q3);

        var q4 = Question.Create(sub1.Id, 1, QuestionType.Essay, "What does forgiveness mean in light of Christ's sacrifice?");
        db.Questions.Add(q4);

        var sub2 = LessonNode.Create(version1.Id, section2.Id, 2, 1, "The Mystery of His Will", "God's plan to unite all things in Christ — a mystery now revealed.");
        db.LessonNodes.Add(sub2);

        var q5 = Question.Create(sub2.Id, 0, QuestionType.Essay, "What is the \"mystery\" Paul refers to in Ephesians 1:9-10?");
        db.Questions.Add(q5);

        var q6 = Question.Create(sub2.Id, 1, QuestionType.Essay, "Why do you think God chose to reveal His will gradually throughout history?");
        db.Questions.Add(q6);

        version1.Publish();
        lesson1.SetCurrentPublishedVersion(version1.Id);

        var lessonTitles = new[]
        {
            "The Creation Account", "The Fall and Promise", "The Flood and Covenant",
            "The Call of Abram", "The Covenant with Abraham", "Isaac and Ishmael",
            "Jacob Becomes Israel", "Joseph — God's Sovereignty", "Moses and the Exodus",
            "The Law and the Tabernacle", "The Wilderness Journey", "Entering the Promised Land",
            "The Judges", "David — A Man After God's Heart", "Wisdom from Proverbs"
        };

        for (int i = 0; i < lessonTitles.Length; i++)
        {
            var lesson = Lesson.Create(i + 2, lessonTitles[i]);
            db.Lessons.Add(lesson);

            var version = LessonVersion.Create(lesson.Id, 1);
            db.LessonVersions.Add(version);

            var placeholderNode = LessonNode.Create(version.Id, null, 1, 0,
                lessonTitles[i],
                $"Content for {lessonTitles[i]} will be developed in a future phase. This placeholder node meets the minimum-structure requirement for publishing.");
            db.LessonNodes.Add(placeholderNode);

            version.Publish();
            lesson.SetCurrentPublishedVersion(version.Id);
        }

        await db.SaveChangesAsync();
    }
}
