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
            var areas = await _context.Persona.ToListAsync();

            return Ok(areas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PersonaDto>> GetId(int id)
        {
            var area = await _context.Persona.FirstOrDefaultAsync(x => x.id == id);
            if (area == null)
            {
                return BadRequest();
            }
            var areaDto = _mapper.Map<PersonaDto>(area);
            return Ok(areaDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(PersonaCreateDto createDto)
        {
            // el dto verifica la tabla
            var persona = _mapper.Map<Persona>(createDto);

            // añade la entidad al contexto
            _context.Persona.Add(persona);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = persona.id }, persona);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, PersonaCreateDto updateDto)
        {
            var persona = await _context.Persona.FirstOrDefaultAsync(x => x.id == id);

            persona = _mapper.Map(updateDto, persona);

            _context.Persona.Update(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { persona.id }, persona);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var persona = await _context.Persona.FindAsync(id);

            if (persona == null)
            {
                return BadRequest();
            }

            _context.Persona.Remove(persona);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}