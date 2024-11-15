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
            var personas = await _context.inv_persona.ToListAsync();

            if (personas == null)
            {
                return BadRequest("No se encontraron registros de personas.");
            }

            var personaDtos = new List<PersonaDto>();

            foreach (var persona in personas)
            {
                var areaName = await _context.inv_area.FirstOrDefaultAsync(o => o.id == persona.IdArea);
                if (areaName == null)
                {
                    return BadRequest("El area no se encontro");
                }

                var user = await _context.aspnetusers.FirstOrDefaultAsync(u => u.Id == persona.userId);
                if (user == null)
                {
                    return BadRequest("El usuario no existe");
                }

                var personaDto = new PersonaDto
                {
                    id = persona.id,
                    userId = persona.userId,
                    IdArea = persona.IdArea,
                    identificacion = persona.identificacion,
                    Estado = persona.Estado,
                    UserName = user.UserName,
                    AreaName = areaName.Nombre,
                };

                personaDtos.Add(personaDto);
            }

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

            //Verificacion si existe el area
            var areaExiste = await _context.inv_area.FindAsync(createDto.IdArea);
            if (areaExiste == null)
            {
                return NotFound($"El area con el ID {createDto.IdArea} no fue encontrado.");
            }

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

            //Verificacion si existe el area
            var areaExiste = await _context.inv_area.FindAsync(updateDto.IdArea);
            if (areaExiste == null)
            {
                return NotFound($"El area con el ID {updateDto.IdArea} no fue encontrado.");
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