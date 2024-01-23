using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectService.WebAPI.Data;

namespace ProjectService.WebAPI.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly TestProjectContext _testProjectContext;

        public ProjectsService(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }

        public async Task<IEnumerable<Project>> Get(int[] ids)
        {
            var projects = _testProjectContext.Projects.AsQueryable();

            if (ids != null && ids.Any())
                projects = projects.Where(x => ids.Contains(x.Id));

            return await projects.ToListAsync();
        }

        public async Task<Project> Add(Project project)
        {
            await _testProjectContext.Projects.AddAsync(project);
            project.AddedDate = DateTime.UtcNow;

            await _testProjectContext.SaveChangesAsync();
            return project;
        }

        public async Task<IEnumerable<Project>> AddRange(IEnumerable<Project> projects)
        {
            await _testProjectContext.Projects.AddRangeAsync(projects);
            await _testProjectContext.SaveChangesAsync();
            return projects;
        }

        public async Task<Project> Update(Project project)
        {
            var projectForChanges = await _testProjectContext.Projects.SingleAsync(x => x.Id == project.Id);
            projectForChanges.IsAvailable = project.IsAvailable;
            projectForChanges.Name = project.Name;

            _testProjectContext.Projects.Update(projectForChanges);
            await _testProjectContext.SaveChangesAsync();
            return project;
        }

        public async Task<bool> Delete(Project project)
        {
            _testProjectContext.Projects.Remove(project);
            await _testProjectContext.SaveChangesAsync();

            return true;
        }
    }

    public interface IProjectsService
    {
        Task<IEnumerable<Project>> Get(int[] ids);

        Task<Project> Add(Project project);

        Task<Project> Update(Project project);

        Task<bool> Delete(Project project);
    }
}
