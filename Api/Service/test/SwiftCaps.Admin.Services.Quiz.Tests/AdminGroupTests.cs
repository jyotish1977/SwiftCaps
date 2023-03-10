using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScenarioTests;
using Shouldly;
using SwiftCaps.Models.Models;

namespace SwiftCaps.Admin.Services.Quiz.Tests
{
    public partial class AdminGroupTests : CRUDTestBase
    {
        [Scenario]
        public async Task AdminGroupService(ScenarioContext scenario)
        {
            using var context = GetDbContext();
                var sut = new AdminGroupService(context);
                var groups = await sut.GetGroupsAsync();
                scenario.Fact("Get Groups should return 0 groups when database empty", () =>
                {
                    groups.Length.ShouldBe(0);
                });
                
                await Seed(new List<Group>
                {
                    new Group { Id = Guid.NewGuid(), Name = "grp-sqn-foo", Created = DateTime.Now, Updated=DateTime.Now}
                });
                await context.SaveChangesAsync();
                groups = await sut.GetGroupsAsync();
                scenario.Fact("Get Groups should return groups when database contains groups", () =>
                {
                    groups.Length.ShouldBe(1);
                    groups[0].Name.ShouldBe("grp-sqn-foo");
                });
        }
        
    }
}
