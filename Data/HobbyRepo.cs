using HobbyService.Models;
using Microsoft.EntityFrameworkCore;

namespace HobbyService.Data;

public class HobbyRepo : IHobbyRepo
{
    private readonly AppDbContext _context;

    public HobbyRepo(AppDbContext context)
    {
        _context = context;
    }
    
    
    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }

    public IEnumerable<User> getAllUsers()
    {
        return _context.Users.ToList();
    }

    public void CreateUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _context.Users.Add(user);
    }

    public bool UserExists(int userId)
    {
        return _context.Users.Any(u => u.Id == userId);
    }

    public bool ExternalUserExists(int externalUserId)
    {
        return _context.Users.Any(u => u.ExternalId == externalUserId);
    }

    public IEnumerable<Hobby> getHobbiesForUser(int userId)
    {
        return _context.Hobbies.Where(h => h.UserId == userId)
            .OrderBy(h => h.User.Name);
    }

    public Hobby GetHobby(int userId, int hobbyId) =>
        _context.Hobbies.FirstOrDefault(h => h.UserId == userId && h.Id == hobbyId) ?? throw new InvalidOperationException();

    public void CreateHobby(int userId, Hobby hobby)
    {
        ArgumentNullException.ThrowIfNull(hobby);
        hobby.UserId = userId;
        _context.Hobbies.Add(hobby);
    }
}