using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Controlinventarios.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class AspnetUsersController : ControllerBase
        {
            private readonly InventoryTIContext _context;
            private readonly IMapper _mapper;

            public AspnetUsersController(InventoryTIContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            [HttpGet]
            public async Task<ActionResult<List<AspnetUsersDto>>> Get()
            {
                var user = await _context.aspnetusers.ToListAsync();
                var userDtos = _mapper.Map<List<AspnetUsersDto>>(user);

                return Ok(userDtos);

            }
        }
}
