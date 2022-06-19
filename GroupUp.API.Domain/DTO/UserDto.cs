using System.Collections.Generic;
using GroupUp.API.Domain.Models;

namespace GroupUp.API.Domain.DTO
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        
        public string Email { get; set; }
        
        public IEnumerable<Group> Groups { get; set; }

        public IEnumerable<string> WishList { get; set; }
        
        public string Token { get; set; }

        public string Uid { get; set; }
    }
}