using GiftOfTheGivers.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Data;

/// <summary>
/// Creates the database (if needed) and seeds it with initial relief projects.
/// </summary>
public static class DbSeeder
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        // Creates the database if it does not exist yet.
        // (Switch to MigrateAsync once EF migrations are added.)
        await db.Database.EnsureCreatedAsync();

        if (await db.Projects.AnyAsync())
        {
            return; // Already seeded.
        }

        var projects = new List<Project>
        {
            new()
            {
                Title = "Gauteng Flood Relief",
                Location = "Gauteng, South Africa",
                Category = "Emergency Relief",
                Status = "Ongoing",
                Summary = "Providing emergency food, clean water and essential supplies to communities affected by flooding.",
                Description = "The Gauteng Flood Relief project provides emergency assistance to communities affected by severe flooding. " +
                              "The project focuses on providing essential resources to families who have been displaced or affected. " +
                              "Relief teams work with volunteers and community organisations to distribute food, clean water, hygiene supplies " +
                              "and other essential resources.",
                ImageUrl = "https://images.unsplash.com/photo-1547683905-f686c993aae5?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 70,
                VolunteerCount = 45,
                ReliefPackagesDelivered = 850,
                StartDate = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Provide emergency food supplies.",
                    "Provide clean drinking water.",
                    "Support displaced families.",
                    "Coordinate community volunteers."
                },
                Updates =
                {
                    new ProjectUpdate
                    {
                        Title = "Relief distribution completed",
                        Body = "Our relief team successfully distributed food parcels and clean water to affected families. " +
                               "Additional supplies are being prepared for the next distribution.",
                        PostedBy = "Employee",
                        CreatedAt = new DateTime(2026, 8, 18, 10, 30, 0, DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Title = "Limpopo Food Relief",
                Location = "Limpopo, South Africa",
                Category = "Food Security",
                Status = "Ongoing",
                Summary = "Delivering food parcels and essential supplies to vulnerable households.",
                Description = "The Limpopo Food Relief project supports vulnerable households with regular food parcels and essential supplies. " +
                              "Working with local community leaders, the project identifies families in greatest need and ensures they receive " +
                              "nutritious food support on an ongoing basis.",
                ImageUrl = "https://images.unsplash.com/photo-1594708767771-a7502209ff51?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 55,
                VolunteerCount = 32,
                ReliefPackagesDelivered = 1240,
                StartDate = new DateTime(2026, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Distribute weekly food parcels.",
                    "Support child-headed households.",
                    "Partner with local farmers.",
                    "Establish community food gardens."
                },
                Updates =
                {
                    new ProjectUpdate
                    {
                        Title = "1,200th food parcel delivered",
                        Body = "This week we reached a major milestone: 1,200 food parcels delivered to families across the region. " +
                               "Thank you to every volunteer and donor who made this possible.",
                        PostedBy = "Employee",
                        CreatedAt = new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Title = "Medical Assistance",
                Location = "KwaZulu-Natal, South Africa",
                Category = "Healthcare",
                Status = "Ongoing",
                Summary = "Supporting communities with medical supplies and emergency healthcare.",
                Description = "The Medical Assistance project supports communities in KwaZulu-Natal with essential medical supplies, " +
                              "mobile clinics and emergency healthcare services. Medical volunteers provide basic health screenings, " +
                              "chronic medication support and referrals to hospitals where needed.",
                ImageUrl = "https://images.unsplash.com/photo-1603398938378-e54eab446dde?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 40,
                VolunteerCount = 28,
                ReliefPackagesDelivered = 460,
                StartDate = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Operate mobile medical clinics.",
                    "Supply essential medication.",
                    "Train community health workers.",
                    "Refer critical cases to hospitals."
                },
                Updates =
                {
                    new ProjectUpdate
                    {
                        Title = "New mobile clinic vehicle commissioned",
                        Body = "A second mobile clinic has been added to the project, allowing our medical teams to reach two " +
                               "additional rural communities every week.",
                        PostedBy = "Employee",
                        CreatedAt = new DateTime(2026, 8, 28, 14, 15, 0, DateTimeKind.Utc)
                    }
                }
            },
            new()
            {
                Title = "Community Support",
                Location = "Eastern Cape, South Africa",
                Category = "Community Development",
                Status = "Ongoing",
                Summary = "Supporting families with essential household resources and food.",
                Description = "The Community Support project assists families in the Eastern Cape with essential household resources, " +
                              "food support and social development programmes. The project works closely with schools and community centres " +
                              "to reach households caring for orphans and vulnerable children.",
                ImageUrl = "https://images.unsplash.com/photo-1488521787991-ed7bbaae773c?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 80,
                VolunteerCount = 51,
                ReliefPackagesDelivered = 2100,
                StartDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Support child-headed households.",
                    "Provide school uniforms and supplies.",
                    "Run community care programmes.",
                    "Distribute winter warmth items."
                }
            },
            new()
            {
                Title = "Education Support",
                Location = "Mpumalanga, South Africa",
                Category = "Education",
                Status = "Ongoing",
                Summary = "Providing educational resources to children in underserved communities.",
                Description = "The Education Support project provides learning materials, stationary, books and school infrastructure support " +
                              "to children in underserved communities across Mpumalanga. Education changes lives, and this project aims to give " +
                              "every child the tools they need to succeed.",
                ImageUrl = "https://images.unsplash.com/photo-1532629345422-7515f3d16bb6?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 35,
                VolunteerCount = 19,
                ReliefPackagesDelivered = 380,
                StartDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Distribute school stationary packs.",
                    "Build and equip school libraries.",
                    "Support matric study programmes.",
                    "Provide learner transport assistance."
                }
            },
            new()
            {
                Title = "Emergency Relief",
                Location = "North West, South Africa",
                Category = "Emergency Relief",
                Status = "Ongoing",
                Summary = "Coordinating emergency resources for communities affected by disasters.",
                Description = "The Emergency Relief project maintains a rapid-response capability for communities affected by disasters such as " +
                              "fires, floods and storms in North West. Pre-positioned relief supplies and trained volunteer teams allow the project " +
                              "to respond within hours of a disaster occurring.",
                ImageUrl = "https://images.unsplash.com/photo-1559027615-cd4628902d4a?auto=format&fit=crop&w=900&q=80",
                ProgressPercent = 60,
                VolunteerCount = 37,
                ReliefPackagesDelivered = 690,
                StartDate = new DateTime(2026, 4, 5, 0, 0, 0, DateTimeKind.Utc),
                Objectives =
                {
                    "Maintain emergency supply stock.",
                    "Train rapid-response volunteers.",
                    "Respond within 24 hours of disasters.",
                    "Coordinate with local disaster management."
                },
                Updates =
                {
                    new ProjectUpdate
                    {
                        Title = "Rapid-response drill completed",
                        Body = "Volunteer teams completed a full emergency response drill this weekend, cutting average deployment " +
                               "preparation time down to under three hours.",
                        PostedBy = "Employee",
                        CreatedAt = new DateTime(2026, 9, 12, 9, 45, 0, DateTimeKind.Utc)
                    }
                }
            }
        };

        db.Projects.AddRange(projects);
        await db.SaveChangesAsync();
    }
}
