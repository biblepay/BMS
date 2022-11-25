using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiblePay.BMS.Extensions;
using BiblePay.BMS.Models;

namespace BiblePay.BMS.EndPoints
{
    [ApiController]
    [Route("api/users")]
    public class UsersEndpoint : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _manager;
        private readonly SmartSettings _settings;
      
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> Get()
        {
            var users = await _manager.Users.AsNoTracking().ToListAsync();

            return Ok(new { data = users, recordsTotal = users.Count, recordsFiltered = users.Count });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IdentityUser>> Get([FromRoute]string id) => Ok(await _manager.FindByIdAsync(id));

       

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromForm]IdentityUser model)
        {
           
                return NoContent();
            
        }
    }
}
