using HobbyService.DTO;
using HobbyService.Models;

namespace HobbyService.Data;

public interface IHobbyRepo
{
    bool SaveChanges();
    
    // //Users 
    // IEnumerable<User> getAllUsers();
    // void CreateUser(User user);
    // bool UserExists(int userId);
    // bool ExternalUserExists(int externalUserId);
    //
    // //Hobbies
    // IEnumerable<Hobby> getHobbiesForUser(int userId);
    Hobby GetHobby(int userId, int hobbyId);
    void CreateHobby(Hobby hobby);
    
    Hobby GetHobby(int id);
    
    IEnumerable<Hobby> getAllHobbies();

    void DeleteHobby(int questionId);
    bool HobbyExists(int hobbyId);

    void UpdateHobby(Hobby hobby);





}