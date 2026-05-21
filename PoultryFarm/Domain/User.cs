using System;

namespace PoultryFarm.Domain;

public class User
{
    public string Username { get; }
    public UserRole Role { get; }
    
    public User(string username, UserRole role) 
    { 
        Username = username; 
        Role = role; 
    }
}