using System;
using System.Threading.Tasks;
using AutoMapper;
using FirebaseAdmin.Auth;
using SecretSanta.API.Domain.DTO;
using SecretSanta.API.Domain.Interfaces;
using SecretSanta.API.Domain.Models;
using SecretSanta.API.Firestore.Utility;

namespace SecretSanta.API.Firestore.Workflows
{
    public class UserWorkflow : IUserWorkflow
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserWorkflow(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> HandleSignIn(User user)
        {
            var freshBakedUser = UserMapper.AppendEmailToUsername(user);
            
            if (string.IsNullOrEmpty(freshBakedUser.Email) || string.IsNullOrEmpty(freshBakedUser.Password))
            {
                throw new Exception("Invalid login credentials");
            }

            var userRecord = await _userRepository.FindUserByEmail(freshBakedUser);

            var mappedUser = _mapper.Map<UserDto>(userRecord);
            return await AttachToken(mappedUser);
        }

        private async Task<UserDto> AttachToken(UserDto user)
        {
            var token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Email);
            user.Token = token;
            return user;
        }
    }
}