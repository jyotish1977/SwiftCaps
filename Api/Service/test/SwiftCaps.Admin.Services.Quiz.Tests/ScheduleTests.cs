using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ScenarioTests;
using Shouldly;
using SwiftCaps.Models.Enums;
using SCModels = SwiftCaps.Models.Models;
using SwiftCaps.Services.Quiz;
using System.Collections.Generic;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public partial class ScheduleTests : CRUDTestBase
    {
        [Scenario]
        public async Task ScheduleService(ScenarioContext scenario)
        {
            using var context = GetDbContext();
            var sut = new ScheduleService(context);

            await scenario.Fact("Get list should return 0 schedules when database empty", async () =>
            {
                var schedules = await sut.GetSchedulesAsync();
                schedules.Count.ShouldBe(0);
            });
            await InvalidCreatePayloadScenarios(scenario, sut);

            var expectedScheduleId = Guid.NewGuid();
            var quizId = Guid.NewGuid();
            await Seed(new List<SCModels.Quiz>{
                        new SCModels.Quiz{
                            Id = quizId,
                            Name = "Quiz 1",
                            Description = "Quiz 1 Description"
                        }
                    });
            await context.SaveChangesAsync();
            var newId = await sut.CreateScheduleAsync(new SCModels.Schedule()
            {
                Id = expectedScheduleId,
                QuizId = quizId,
                Recurrence = Recurrence.Monthly,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(30)
            });

            scenario.Fact("Create should create schedule when payload is valid", () =>
            {
                newId.HasValue.ShouldBeTrue();
                newId.ShouldBeOfType<Guid>();
            });

            await scenario.Fact("Get list should return schedules when database is not empty", async () =>
            {
                var schedules = await sut.GetSchedulesAsync();
                schedules.Count.ShouldBe(1);
            });

            await scenario.Fact("Get should error when invalid payload", async () =>
            {
                _ = await Should.ThrowAsync<ArgumentException>(async () =>
                                        await sut.GetScheduleAsync(Guid.Empty));

                //Non existing schedule id
                _ = await Should.ThrowAsync<NotFoundException>(async () =>
                                        await sut.GetScheduleAsync(Guid.NewGuid()));
            });

            await scenario.Fact("Get should retrieve schedule when valid payload", async () =>
            {
                var schedule = await sut.GetScheduleAsync(newId.Value);
                schedule.Id.ShouldBe(newId.Value);
            });
        }

        private static async Task InvalidCreatePayloadScenarios(ScenarioContext scenario, ScheduleService sut)
        {
            await scenario.Fact("Create should error for invalid payloads", async () =>
            {
                _ = await Should.ThrowAsync<ArgumentException>(async () =>
                    await sut.CreateScheduleAsync(null));

                _ = await Should.ThrowAsync<ArgumentException>(async () =>
                          await sut.CreateScheduleAsync(new SCModels.Schedule()));

                _ = await Should.ThrowAsync<ArgumentException>(async () =>
                      await sut.CreateScheduleAsync(new SCModels.Schedule()
                      {
                          QuizId = Guid.NewGuid(),
                          Recurrence = Recurrence.Monthly,
                          StartTime = DateTime.UtcNow.AddDays(-1),
                          EndTime = DateTime.UtcNow
                      }));

                _ = await Should.ThrowAsync<ArgumentException>(async () =>
                      await sut.CreateScheduleAsync(new SCModels.Schedule()
                      {
                          QuizId = Guid.NewGuid(),
                          Recurrence = Recurrence.Monthly,
                          StartTime = DateTime.UtcNow,
                          EndTime = DateTime.UtcNow.AddDays(-1)
                      }));

                //Non existing quiz id
                _ = await Should.ThrowAsync<NotFoundException>(async () =>
                     await sut.CreateScheduleAsync(new SCModels.Schedule()
                     {
                         QuizId = Guid.NewGuid(),
                         Recurrence = Recurrence.Monthly,
                         StartTime = DateTime.UtcNow,
                         EndTime = DateTime.UtcNow.AddDays(30)
                     }));
            });
        }
    }
}
