using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace GroupUp.API.Domain.Models
{
    [FirestoreData]
    public class User : Base
    {
        [FirestoreProperty]
        public string DisplayName { get; set; }

        [FirestoreProperty]
        public IEnumerable<string> WishList { get; set; }
        
        [FirestoreProperty]
        public IEnumerable<string> GroupIds { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        public string Token { get; set; }
    }
}
