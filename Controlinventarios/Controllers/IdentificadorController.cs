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
    public class IdentificadorController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public IdentificadorController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<IdentificadorDto>>> Get()
        {
            var identificador = await _context.inv_identificador.ToListAsync();
            var identificadorDtos = _mapper.Map<List<IdentificadorDto>>(identificador);

            return Ok(identificadorDtos);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IdentificadorDto>> GetId(int id)
        {
            var identificador = await _context.inv_identificador.FirstOrDefaultAsync(x => x.id == id);
            if (identificador == null)
            {
                return BadRequest();
            }
            var identificadorDto = _mapper.Map<IdentificadorDto>(identificador);
            return Ok(identificadorDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(IdentificadorCreateDto createDto)
        {
            // el dto verifica la tabla
            var identificador = _mapper.Map<Identificador>(createDto);

            // añade la entidad al contexto
            _context.inv_identificador.Add(identificador);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = identificador.id }, identificador);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, IdentificadorCreateDto updateDto)
        {
            var identificador = await _context.inv_identificador.FirstOrDefaultAsync(x => x.id == id);

            identificador = _mapper.Map(updateDto, identificador);

            _context.inv_identificador.Update(identificador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { identificador.id }, identificador);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var identificador = await _context.inv_identificador.FindAsync(id);

            if (identificador == null)
            {
                return BadRequest();
            }

            _context.inv_identificador.Remove(identificador);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
