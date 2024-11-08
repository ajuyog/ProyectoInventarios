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
    public class PersonaController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public PersonaController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<PersonaDto>>> Get()
        {
            var persona = await _context.inv_persona.ToListAsync();
            var personaDtos = _mapper.Map<List<PersonaDto>>(persona);

            return Ok(personaDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PersonaDto>> GetId(int id)
        {
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.id == id);
            if (persona == null)
            {
                return BadRequest($"No existe el id: {id}");
            }
            var areaDto = _mapper.Map<PersonaDto>(persona);
            return Ok(areaDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(PersonaCreateDto createDto)
        {
            // el dto verifica la tabla
            var persona = _mapper.Map<Persona>(createDto);

            // añade la entidad al contexto
            _context.inv_persona.Add(persona);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = persona.id }, persona);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, PersonaCreateDto updateDto)
        {
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.id == id);
            
            if(persona == null)
            {
                return BadRequest($"");
            }

            persona = _mapper.Map(updateDto, persona);

            _context.inv_persona.Update(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { persona.id }, persona);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var persona = await _context.inv_persona.FindAsync(id);

            if (persona == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_persona.Remove(persona);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}