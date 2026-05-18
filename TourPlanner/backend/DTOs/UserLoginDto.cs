using System;

namespace TourPlanner.backend.DTOs{
    public class UserLoginDto
    {
        public required string Email {get;set;}
        public required string Password {get;set;} 
    }
}