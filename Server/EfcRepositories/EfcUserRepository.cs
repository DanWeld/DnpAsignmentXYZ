using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace EfcRepositories
{
    public class EfcUserRepository : IUserRepository
    {
        private readonly AppContext ctx;

        public EfcUserRepository(AppContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<User> AddAsync(User user)
        {
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            if (!(await ctx.Users.AnyAsync(u => u.Id == user.Id)))
            {
                throw new NotFoundException($"User with id {user.Id} not found");
            }
            ctx.Users.Update(user);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            User? existing = await ctx.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (existing == null)
            {
                throw new NotFoundException($"User with id {id} not found");
            }
            ctx.Users.Remove(existing);
            await ctx.SaveChangesAsync();
        }

        public async Task<User?> GetSingleAsync(int id)
        {
            return await ctx.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public IQueryable<User> GetMany()
        {
            return ctx.Users.AsQueryable();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await ctx.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<object> GetAllAsync()
        {
            var users = await ctx.Users.ToListAsync();
            return users;
        }
    }
}
