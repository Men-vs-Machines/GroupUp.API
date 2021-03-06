using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using GroupUp.API.Domain.DTO;
using GroupUp.API.Domain.Models;

namespace GroupUp.API.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<UserDto> Create(User user);
        public Task<WriteResult> Update(User record);
        public Task<bool> Delete(User record);
        public Task<User> Get(User record);
        public Task<IEnumerable<User>> GetAll();
        public Task<List<User>> QueryRecords(Query query);
        public Task BatchUpdate(IEnumerable<User> users, Group group);
    }
}
