using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GroupUp.API.Domain.Interfaces;
using GroupUp.API.Domain.Models;
using GroupUp.API.Firestore.Workflows;

namespace GroupUp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupCreationWorkflow _groupCreationWorkflow;

        public GroupsController(IGroupRepository groupRepository, IGroupCreationWorkflow groupCreationWorkflow)
        {
            _groupRepository = groupRepository;
            _groupCreationWorkflow = groupCreationWorkflow;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var result = await _groupRepository.GetAll();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup(string id)
        {
            try
            {
                var group = new Group { Id = id };
                var result = await _groupRepository.Get(group);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddGroup([FromBody] Group group)
        {
            try
            {
                var result = await _groupRepository.Add(group);
                await _groupCreationWorkflow.AddGroupsToUser(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
