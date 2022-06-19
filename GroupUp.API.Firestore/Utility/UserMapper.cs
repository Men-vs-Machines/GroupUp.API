using FirebaseAdmin.Auth;
using GroupUp.API.Domain.Models;

namespace GroupUp.API.Firestore.Utility
{
    public static class UserMapper
    {
        public static UserRecordArgs AppendEmailToUsername(User user)
        {
            return new UserRecordArgs
            {
                DisplayName = user.DisplayName,
                Email = user.DisplayName + "@example.com",
                Password = user.Password
            };
        }
    }
}