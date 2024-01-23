using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectService.WebAPI.Data;

namespace ProjectService.WebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly TestProjectContext _testProjectContext;

        public UsersService(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }

        public async Task<IEnumerable<User>> Get(int projectId, int[] ids)
        {
            var users = _testProjectContext.Users.Where(x => x.ProjectId == projectId).AsQueryable();

            if (ids != null && ids.Any())
                users = users.Where(x => ids.Contains(x.Id));

            return await users.ToListAsync();
        }

        public async Task<User> Add(User user)
        {

            await _testProjectContext.Users.AddAsync(user);
            user.AddedDate = DateTime.UtcNow;

            await _testProjectContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            var userForChanges = await _testProjectContext.Users.SingleAsync(x => x.Id == user.Id);

            userForChanges.Name = user.Name;

            _testProjectContext.Users.Update(userForChanges);
            await _testProjectContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(User user)
        {
            _testProjectContext.Users.Remove(user);
            await _testProjectContext.SaveChangesAsync();

            return true;
        }
    }

    public interface IUsersService
    {
        Task<IEnumerable<User>> Get(int projectId, int[] ids);

        Task<User> Add(User user);

        Task<User> Update(User user);

        Task<bool> Delete(User user);
    }
}
