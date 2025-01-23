﻿using AutoMapper;
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
                var areaName = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
                if (areaName == null)
                {
                    return BadRequest("El area no se encontro");
                }

                var user = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);
                if (user == null)
                {
                    return BadRequest("El usuario no existe");
                }

                var nombreEmpresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
                if (nombreEmpresa == null)
                {
                    return BadRequest("No se encontraron empresas");
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
                    NombreEmpresa = nombreEmpresa.Nombre
                };

                personaDtos.Add(personaDto);
            }

            return Ok(personaDtos);
        }

        [HttpGet("{identificacion},{correo}")]
        public async Task<ActionResult<PersonaDto>> GetId(string identificacion, string correo)
        {
            // Verifica si la persona existe
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.identificacion == identificacion);

            if (persona == null)
            {
                return BadRequest($"No existe la identificacion: {identificacion}");
            }

            // Se verifica el area del usuario
            var areaName = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);

            if (areaName == null)
            {
                return BadRequest("El área no se encontró");
            }

            // Se verifica el nombre del usuario
            var user = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);

            if (user == null)
            {
                return BadRequest("El usuario no existe");
            }

            var nombreEmpresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);

            if (nombreEmpresa == null)
            {
                return BadRequest($"No existe la empresa: {nombreEmpresa.Nombre}");
            }

            var personaDto = new PersonaDto
            {
                id = persona.id,
                userId = persona.userId,
                idEmpresa = persona.idEmpresa,
                IdArea = persona.IdArea,
                identificacion = persona.identificacion,
                Estado = persona.Estado,
                UserName = user.UserName,
                AreaName = areaName.Nombre,
                NombreEmpresa = nombreEmpresa.Nombre
            };

            return Ok(personaDto);
        }

        [HttpGet("{Busqueda}")]
        public async Task<ActionResult<List<PersonaDto>>> Get(string Busqueda)
        {
            // busca todas las personas que contengan la letra o numero en identificacion o correo
            var personas = await _context.inv_persona.Where(x => x.identificacion.Contains(Busqueda) || x.userId.Contains(Busqueda)).ToListAsync();
                
            if (!personas.Any())
            {
                return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
            }

            // lista para almacenar los resultados en formato DTO
            var personasDto = new List<PersonaDto>();

            foreach (var persona in personas)
            {
                // busca el area asociada
                var areaName = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);

                if (areaName == null)
                {
                    return BadRequest("El área no se encontró");
                }

                // busca el usuario asociado
                var user = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);

                if (user == null)
                {
                    return BadRequest("El usuario no existe");
                }

                // busca la empresa asociada
                var nombreEmpresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);

                if (nombreEmpresa == null)
                {
                    return BadRequest("La empresa no se encontró");
                }

                // agrega el DTO a la lista
                personasDto.Add(new PersonaDto
                {
                    id = persona.id,
                    userId = persona.userId,
                    idEmpresa = persona.idEmpresa,
                    IdArea = persona.IdArea,
                    identificacion = persona.identificacion,
                    Estado = persona.Estado,
                    UserName = user.UserName,
                    AreaName = areaName.Nombre,
                    NombreEmpresa = nombreEmpresa.Nombre
                });
            }

            return Ok(personasDto);
        }    

            


        [HttpPost]
        public async Task<ActionResult> Post(PersonaCreateDto createDto)
        {
            // el dto verifica la tabla
            var persona = _mapper.Map<Persona>(createDto);

            //Verificacion si existe el area
            var areaExiste = await _context.inv_area.FirstOrDefaultAsync(x => x.id == createDto.IdArea);
            if (areaExiste == null)
            {
                return BadRequest($"El area con el ID {createDto.IdArea} no fue encontrado.");
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
            var areaExiste = await _context.inv_area.FirstOrDefaultAsync(x => x.id == updateDto.IdArea);
            if (areaExiste == null)
            {
                return BadRequest($"El area con el ID {updateDto.IdArea} no fue encontrado.");
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