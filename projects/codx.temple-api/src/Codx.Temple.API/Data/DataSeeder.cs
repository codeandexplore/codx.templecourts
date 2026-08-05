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

        var studentEmail = configuration["Student:Email"];
        var studentPassword = configuration["Student:Password"];

        if (!string.IsNullOrEmpty(studentEmail) && !string.IsNullOrEmpty(studentPassword))
        {
            var studentExists = await db.Users.AnyAsync(u => u.Email == studentEmail.ToLowerInvariant());
            if (!studentExists)
            {
                var studentUser = User.CreateWithPassword(studentEmail, passwordHasher.Hash(studentPassword), "Student");
                db.Users.Add(studentUser);
                var studentRole = RoleAssignment.Create(studentUser.Id, Role.Student, adminUser.Id);
                db.RoleAssignments.Add(studentRole);
            }
        }

        await db.SaveChangesAsync();

        await SeedLessonsAsync(db);
    }

    private static async Task SeedLessonsAsync(AppDbContext db)
    {
        if (await db.Lessons.AnyAsync())
            return;

        var lesson1 = Lesson.Create(1, "God's Eternal Purpose");
        db.Lessons.Add(lesson1);
        var v1 = LessonVersion.Create(lesson1.Id, 1);
        db.LessonVersions.Add(v1);

        var n1 = LessonNode.Create(v1.Id, null, 1, 0, "Before the Foundation of the World", "Understanding that God's plan for His people existed before time began — covering Ephesians 1:4 and related passages.");
        db.LessonNodes.Add(n1);
        db.Questions.Add(Question.Create(n1.Id, 0, QuestionType.TrueFalse, "God chose His people before the foundation of the world.", System.Text.Json.JsonSerializer.SerializeToDocument(new { correct = true })));
        db.Questions.Add(Question.Create(n1.Id, 1, QuestionType.Essay, "What does it mean that God chose His people \"before the foundation of the world\"?"));

        var n2 = LessonNode.Create(v1.Id, null, 1, 1, "Manifested in Christ", "Exploring how God's purpose was revealed through Jesus Christ — covering Ephesians 1:7-10.", requiresPriorSiblingAnswered: true);
        db.LessonNodes.Add(n2);
        var n2a = LessonNode.Create(v1.Id, n2.Id, 2, 0, "Redemption Through His Blood", "The price of our salvation.");
        db.LessonNodes.Add(n2a);
        db.Questions.Add(Question.Create(n2a.Id, 0, QuestionType.FillBlank, "In Him we have ___ through His blood.", System.Text.Json.JsonSerializer.SerializeToDocument(new { answer = "redemption" })));
        db.Questions.Add(Question.Create(n2a.Id, 1, QuestionType.Essay, "What does forgiveness mean in light of Christ's sacrifice?"));
        var n2b = LessonNode.Create(v1.Id, n2.Id, 2, 1, "The Mystery of His Will", "God's plan to unite all things in Christ.");
        db.LessonNodes.Add(n2b);
        db.Questions.Add(Question.Create(n2b.Id, 0, QuestionType.YesNo, "Did God keep His will hidden until Christ came?", System.Text.Json.JsonSerializer.SerializeToDocument(new { correct = false })));
        db.Questions.Add(Question.Create(n2b.Id, 1, QuestionType.Essay, "Why do you think God chose to reveal His will gradually throughout history?"));
        v1.Publish();
        lesson1.SetCurrentPublishedVersion(v1.Id);

        AddLesson(db, 2, "The Creation Account", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Creation Account", "Genesis 1–2");
            Q(db, n, 0, QuestionType.Essay, "What do the opening words \"In the beginning, God created\" reveal about God's nature?");
            Q(db, n, 1, QuestionType.TrueFalse, "God saw that creation was \"good.\"", Serialize(new { correct = true }));
            Q(db, n, 2, QuestionType.Essay, "What does it mean that humanity was made in the image of God?");
        });

        AddLesson(db, 3, "The Fall and Promise", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Fall and Promise", "Genesis 3");
            Q(db, n, 0, QuestionType.Essay, "What was the serpent's first tactic in questioning God's command?");
            Q(db, n, 1, QuestionType.FillBlank, "\"I will put enmity between you and the woman, and between your offspring and ___ offspring.\"", Serialize(new { answer = "her" }));
            Q(db, n, 2, QuestionType.Essay, "God asked Adam \"Where are you?\" — why does an all-knowing God ask questions?");
        });

        AddLesson(db, 4, "The Flood and Covenant", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Flood and Covenant", "Genesis 6–9");
            Q(db, n, 0, QuestionType.YesNo, "Did Noah earn God's favor through his own righteousness?", Serialize(new { correct = false }));
            Q(db, n, 1, QuestionType.Essay, "What does Genesis 6:5-6 tell us about God's response to human wickedness?");
            Q(db, n, 2, QuestionType.Essay, "What is the significance of the rainbow as a covenant sign?");
        });

        AddLesson(db, 5, "The Call of Abram", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Call of Abram", "Genesis 12");
            Q(db, n, 0, QuestionType.Essay, "What did God ask Abram to leave behind, and why is this significant?");
            Q(db, n, 1, QuestionType.FillBlank, "\"In you all the ___ of the earth will be blessed.\"", Serialize(new { answer = "families" }));
            Q(db, n, 2, QuestionType.Essay, "What did God promise Abram beyond land?");
        });

        AddLesson(db, 6, "The Covenant with Abraham", (db, v1s) => {
            var n1d = Node(db, v1s, null, 1, 0, "The Promise Confirmed", "Genesis 15–17");
            Q(db, n1d, 0, QuestionType.Essay, "Why did God use a covenant ceremony with animals in Genesis 15?");
            var n2d = Node(db, v1s, null, 1, 1, "The Name Change", "Genesis 15–17");
            Q(db, n2d, 0, QuestionType.Essay, "Why did God change Abram's name to Abraham, and what does it mean?");
            Q(db, n2d, 1, QuestionType.TrueFalse, "God's covenant with Abraham was conditional on Abraham's obedience.", Serialize(new { correct = false }));
        });

        AddLesson(db, 7, "Isaac and Ishmael", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "Isaac and Ishmael", "Genesis 21");
            Q(db, n, 0, QuestionType.Essay, "Why is Isaac called the \"son of promise\"?");
            Q(db, n, 1, QuestionType.Essay, "How did God show mercy to Hagar and Ishmael even as they were sent away?");
        });

        AddLesson(db, 8, "Jacob Becomes Israel", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "Jacob Becomes Israel", "Genesis 28, 32");
            Q(db, n, 0, QuestionType.Essay, "What was God's promise to Jacob in the dream at Bethel?");
            Q(db, n, 1, QuestionType.FillBlank, "\"Your name will no longer be Jacob, but ___.\"", Serialize(new { answer = "Israel" }));
        });

        AddLesson(db, 9, "Joseph — God's Sovereignty", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "Joseph — God's Sovereignty", "Genesis 37, 45, 50");
            Q(db, n, 0, QuestionType.Essay, "What enabled Joseph to endure years of suffering without bitterness?");
            Q(db, n, 1, QuestionType.Essay, "What does Joseph mean when he says \"You meant it for evil, but God meant it for good\"?");
        });

        AddLesson(db, 10, "Moses and the Exodus", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "Moses and the Exodus", "Exodus 3, 12, 14");
            Q(db, n, 0, QuestionType.Essay, "What did God reveal about Himself when He said \"I AM WHO I AM\"?");
            Q(db, n, 1, QuestionType.TrueFalse, "The Passover lamb could have any blemish.", Serialize(new { correct = false }));
            Q(db, n, 2, QuestionType.Essay, "What does the Red Sea crossing teach about God's power to deliver?");
        });

        AddLesson(db, 11, "The Law and the Tabernacle", (db, v1s) => {
            var t1 = Node(db, v1s, null, 1, 0, "The Ten Commandments", "Exodus 20");
            var s1a = Node(db, v1s, t1, 2, 0, "Loving God", "Exodus 20:1-11");
            Q(db, s1a, 0, QuestionType.Essay, "Why do the first four commandments focus on our relationship with God?");
            var s1b = Node(db, v1s, t1, 2, 1, "Loving Neighbor", "Exodus 20:12-17");
            Q(db, s1b, 0, QuestionType.YesNo, "The commandments were given so Israel could earn salvation.", Serialize(new { correct = false }));
            Q(db, s1b, 1, QuestionType.Essay, "How do the last six commandments guide our treatment of others?");
            var t2 = Node(db, v1s, null, 1, 1, "The Tabernacle", "Exodus 25–31");
            Q(db, t2, 0, QuestionType.Essay, "What was the significance of the Most Holy Place?");
        });

        AddLesson(db, 12, "The Wilderness Journey", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Wilderness Journey", "Numbers 13–14, 21");
            Q(db, n, 0, QuestionType.Essay, "Why did the Israelites refuse to enter the Promised Land after the spies' report?");
            Q(db, n, 1, QuestionType.FillBlank, "\"As Moses lifted up the serpent in the wilderness, so must the ___ be lifted up.\"", Serialize(new { answer = "Son of Man" }));
        });

        AddLesson(db, 13, "Entering the Promised Land", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "Entering the Promised Land", "Joshua 1, 3–4, 6");
            Q(db, n, 0, QuestionType.Essay, "What command did God repeat to Joshua multiple times, and why?");
            Q(db, n, 1, QuestionType.Essay, "Why did God instruct Israel to march around Jericho rather than attack directly?");
        });

        AddLesson(db, 14, "The Judges", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "The Judges", "Judges 2, 4, 6–7");
            Q(db, n, 0, QuestionType.Essay, "What pattern of behavior repeats throughout the book of Judges?");
            Q(db, n, 1, QuestionType.Essay, "Why did God reduce Gideon's army from 32,000 to 300?");
            Q(db, n, 2, QuestionType.TrueFalse, "Gideon's victory came through superior military strategy.", Serialize(new { correct = false }));
        });

        AddLesson(db, 15, "David — A Man After God's Heart", (db, v1s) => {
            var n = Node(db, v1s, null, 1, 0, "David — A Man After God's Heart", "1 Samuel 16–17, 2 Samuel 7, 11–12");
            Q(db, n, 0, QuestionType.Essay, "Why did God choose David — the youngest, a shepherd — over his brothers?");
            Q(db, n, 1, QuestionType.FillBlank, "David told Goliath \"the battle is the ___'s.\"", Serialize(new { answer = "Lord" }));
            Q(db, n, 2, QuestionType.Essay, "How does David's response when confronted by Nathan differ from Saul's pattern?");
        });

        AddLesson(db, 16, "Wisdom from Proverbs", (db, v1s) => {
            var n1d = Node(db, v1s, null, 1, 0, "The Foundation of Wisdom", "Proverbs 1:7, 3:5-6");
            Q(db, n1d, 0, QuestionType.Essay, "What does it mean that \"the fear of the Lord is the beginning of wisdom\"?");
            Q(db, n1d, 1, QuestionType.TrueFalse, "Wisdom in Proverbs is primarily about intellectual knowledge.", Serialize(new { correct = false }));
            var n2d = Node(db, v1s, null, 1, 1, "Trusting God's Guidance", "Proverbs 3:5-6", requiresPriorSiblingAnswered: true);
            var n2a = Node(db, v1s, n2d, 2, 0, "The Call to Trust", "Proverbs 3:5-6");
            Q(db, n2a, 0, QuestionType.Essay, "What does Proverbs 3:5 command us to do, and what does it command us NOT to do?");
            var n2b = Node(db, v1s, n2d, 2, 1, "The Promise for the Trusting", "Proverbs 3:5-6");
            Q(db, n2b, 0, QuestionType.Essay, "What promise does God give to those who trust in Him?");
        });

        await db.SaveChangesAsync();
    }

    private static void AddLesson(AppDbContext db, int number, string title, Action<AppDbContext, LessonVersion> build)
    {
        var lesson = Lesson.Create(number, title);
        db.Lessons.Add(lesson);
        var version = LessonVersion.Create(lesson.Id, 1);
        db.LessonVersions.Add(version);
        build(db, version);
        version.Publish();
        lesson.SetCurrentPublishedVersion(version.Id);
    }

    private static LessonNode Node(AppDbContext db, LessonVersion version, LessonNode? parent, int depth, int order, string title, string description, bool requiresPriorSiblingAnswered = false)
    {
        var node = LessonNode.Create(version.Id, parent?.Id, depth, order, title, description, requiresPriorSiblingAnswered);
        db.LessonNodes.Add(node);
        return node;
    }

    private static void Q(AppDbContext db, LessonNode node, int order, QuestionType type, string prompt, System.Text.Json.JsonDocument? metadata = null)
    {
        db.Questions.Add(Question.Create(node.Id, order, type, prompt, metadata));
    }

    private static System.Text.Json.JsonDocument Serialize(object obj)
    {
        return System.Text.Json.JsonSerializer.SerializeToDocument(obj);
    }

}
