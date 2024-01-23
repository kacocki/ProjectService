using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using ProjectService.WebAPI;
using ProjectService.WebAPI.Data;
using ProjectService.WebAPI.SeedData;
using Xunit;

namespace ProjectService.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HttpClient Client { get; private set; }

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            SetUpClient();
        }

        private async Task SeedData()
        {
            var createForm0 = GenerateProjectCreateForm("Project Name 1");
            var response0 = await Client.PostAsync("/api/projects", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));

            var createForm1 = GenerateProjectCreateForm("Project Name 2");
            var response1 = await Client.PostAsync("/api/projects", new StringContent(JsonConvert.SerializeObject(createForm1), Encoding.UTF8, "application/json"));

            var createForm2 = GenerateProjectCreateForm("Project Name 3");
            var response2 = await Client.PostAsync("/api/projects", new StringContent(JsonConvert.SerializeObject(createForm2), Encoding.UTF8, "application/json"));

            var createForm3 = GenerateProjectCreateForm("Project Name 4");
            var response3 = await Client.PostAsync("/api/projects", new StringContent(JsonConvert.SerializeObject(createForm3), Encoding.UTF8, "application/json"));
        }

        public async Task SeedUser(string userName, int projectId)
        {
            var userForm = new UserForm
            {
                Name = userName,
                ProjectId = projectId
            };
            var response1 = await Client.PostAsync($"/api/projects/{projectId}/users",
                new StringContent(JsonConvert.SerializeObject(userForm), Encoding.UTF8, "application/json"));
        }

        private ProjectForm GenerateProjectCreateForm(string projectName)
        {
            return new ProjectForm
            {
                Name = projectName,
            };
        }

        // TEST NAME - addUserToProject
        // TEST DESCRIPTION - It adds user to a project
        [Fact]
        public async Task Test1()
        {
            await SeedData();

            var userForm = new UserForm
            {
                Name = "Test user 1",
                ProjectId = 1
            };

            var response1 = await Client.PostAsync($"/api/projects/1/users",
                new StringContent(JsonConvert.SerializeObject(userForm), Encoding.UTF8, "application/json"));

            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status201Created);

            userForm = new UserForm
            {
                Name = "Test user 2",
                ProjectId = 100
            };

            var response2 = await Client.PostAsync($"/api/projects/100/users",
                new StringContent(JsonConvert.SerializeObject(userForm), Encoding.UTF8, "application/json"));

            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - getUsersForAProject
        // TEST DESCRIPTION - It finds all users for project by ID
        [Fact]
        public async Task Test2()
        {
            await SeedData();

            await SeedUser("test user 1", 1);
            await SeedUser("test user 2", 1);

            var response1 = await Client.GetAsync($"/api/projects/2/users");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(response1.Content.ReadAsStringAsync().Result).ToList();
            users.Count.Should().Be(0);
            
            var response2 = await Client.GetAsync($"/api/projects/1/users");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var users2 = JsonConvert.DeserializeObject<IEnumerable<User>>(response2.Content.ReadAsStringAsync().Result).ToList();
            users2.Count.Should().Be(2);

            var response3 = await Client.GetAsync($"/api/projects/31232/users");
            response3.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - deleteProjectById
        // TEST DESCRIPTION - Check delete project web api end point
        [Fact]
        public async Task Test3()
        {
            await SeedData();

            var response0 = await Client.DeleteAsync("/api/projects/1");
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status204NoContent);

            // Verify that delete is successful
            var response1 = await Client.GetAsync("/api/projects/1/users");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);

            var response2 = await Client.DeleteAsync("/api/projects/1");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        private void SetUpClient()
        {
            Client = _factory.WithWebHostBuilder(builder =>
                builder.UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new TestProjectContext(new DbContextOptionsBuilder<TestProjectContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(TestProjectContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    context.SaveChanges();

                    // Clear local context cache
                    foreach (var entity in context.ChangeTracker.Entries().ToList())
                    {
                        entity.State = EntityState.Detached;
                    }
                })).CreateClient();
        }
    }
}
