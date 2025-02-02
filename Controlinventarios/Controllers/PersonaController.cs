﻿using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Algorithm;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
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
            var personas = await _context.inv_persona.OrderByDescending(x => x.userId).ToListAsync();

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
                    userId = persona.userId,
                    IdArea = persona.IdArea,
                    idEmpresa = nombreEmpresa.id,
                    identificacion = persona.identificacion,
                    Estado = persona.Estado,
                    UserName = user.UserName,
                    AreaName = areaName.Nombre,
                    NombreEmpresa = nombreEmpresa.Nombre,
                    Nombres = persona.Nombres,
                    Apellidos = persona.Apellidos
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
                userId = persona.userId,
                idEmpresa = nombreEmpresa.id,
                IdArea = persona.IdArea,
                identificacion = persona.identificacion,
                Estado = persona.Estado,
                UserName = user.UserName,
                AreaName = areaName.Nombre,
                NombreEmpresa = nombreEmpresa.Nombre,
                Nombres = persona.Nombres,
                Apellidos = persona.Apellidos
            };

            return Ok(personaDto);
        }

        //[HttpGet("{Busqueda}")]
        //public async Task<ActionResult<List<PersonaDto>>> Get(string Busqueda)
        //{
        //    // busca todas las personas que contengan la letra o numero en identificacion o correo
        //    var personas = await _context.inv_persona.Where(x => x.identificacion.Contains(Busqueda)).ToListAsync();

        //    if (!personas.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
        //    }

        //    var nombreUsuraio = await _context.aspnetusers.Where(x => x.UserName.Contains(Busqueda)).ToListAsync();

        //    if (!nombreUsuraio.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
        //    }

        //    // lista para almacenar los resultados en formato DTO
        //    var personasDto = new List<PersonaDto>();

        //    foreach (var persona in personas)
        //    {
        //        // busca el area asociada
        //        var areaName = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);

        //        if (areaName == null)
        //        {
        //            return BadRequest("El área no se encontró");
        //        }

        //        // busca el usuario asociado
        //        var user = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);

        //        if (user == null)
        //        {
        //            return BadRequest("El usuario no existe");
        //        }

        //        // busca la empresa asociada
        //        var nombreEmpresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);

        //        if (nombreEmpresa == null)
        //        {
        //            return BadRequest("La empresa no se encontró");
        //        }

        //        // agrega el DTO a la lista
        //        personasDto.Add(new PersonaDto
        //        {
        //            id = persona.id,
        //            userId = persona.userId,
        //            idEmpresa = persona.idEmpresa,
        //            IdArea = persona.IdArea,
        //            identificacion = persona.identificacion,
        //            Estado = persona.Estado,
        //            UserName = user.UserName,
        //            AreaName = areaName.Nombre,
        //            NombreEmpresa = nombreEmpresa.Nombre
        //        });
        //    }

        //    return Ok(personasDto);
        //}

        //[HttpGet("{Busqueda}")]
        //public async Task<ActionResult<List<PersonaDto>>> Get(string Busqueda)
        //{
        //    // Busca todas las personas que contengan la búsqueda en identificación
        //    var personas = await _context.inv_persona.Where(x => x.identificacion.Contains(Busqueda)).ToListAsync();

        //    if (!personas.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con la identificación que contiene: {Busqueda}");
        //    }

        //    // Busca los usuarios cuyo UserName contenga la búsqueda
        //    var usuarios = await _context.aspnetusers.Where(x => x.UserName.Contains(Busqueda)).ToListAsync();

        //    if (!usuarios.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con el nombre de usuario que contiene: {Busqueda}");
        //    }

        //    // Lista para almacenar los resultados en formato DTO
        //    var personasDto = new List<PersonaDto>();

        //    foreach (var persona in personas)
        //    {
        //        // Busca el área asociada
        //        var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
        //        if (area == null)
        //        {
        //            return BadRequest($"No se encontró el área asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        // Busca el usuario asociado
        //        var user = usuarios.FirstOrDefault(x => x.Id == persona.userId);
        //        if (user == null)
        //        {
        //            return BadRequest($"No se encontró el usuario asociado para la persona con identificación: {persona.identificacion}");
        //        }

        //        // Busca la empresa asociada
        //        var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
        //        if (empresa == null)
        //        {
        //            return BadRequest($"No se encontró la empresa asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        // Agrega el DTO a la lista
        //        personasDto.Add(new PersonaDto
        //        {
        //            id = persona.id,
        //            userId = persona.userId,
        //            idEmpresa = persona.idEmpresa,
        //            IdArea = persona.IdArea,
        //            identificacion = persona.identificacion,
        //            Estado = persona.Estado,
        //            UserName = user.UserName,
        //            AreaName = area.Nombre,
        //            NombreEmpresa = empresa.Nombre
        //        });
        //    }

        //    return Ok(personasDto);
        //}


        //[HttpGet("{Busqueda}")]
        //public async Task<ActionResult<List<PersonaDto>>> Get(string Busqueda)
        //{
        //    // Busca todas las personas que contengan la búsqueda en identificación
        //    var personas = await _context.inv_persona.Where(x => x.identificacion.Contains(Busqueda)).ToListAsync();

        //    // Busca todos los usuarios cuyo UserName contenga la búsqueda
        //    var usuarios = await _context.aspnetusers.Where(x => x.UserName.Contains(Busqueda)).ToListAsync();    

        //    // Verifica si no hay resultados en ambas consultas
        //    if (!personas.Any() && !usuarios.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
        //    }

        //    // Lista para almacenar los resultados en formato DTO
        //    var personasDto = new List<PersonaDto>();

        //    // Procesa las personas encontradas
        //    foreach (var persona in personas)
        //    {
        //        // Busca el área asociada
        //        var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
        //        if (area == null)
        //        {
        //            return BadRequest($"No se encontró el área asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        var nombreUsuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);
        //        if (nombreUsuario == null)
        //        {
        //            return BadRequest($"No se encontró el usuario asociado para la persona con identificación: {persona.identificacion}");
        //        }

        //        // Busca la empresa asociada
        //        var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
        //        if (empresa == null)
        //        {
        //            return BadRequest($"No se encontró la empresa asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        // Agrega el DTO a la lista
        //        personasDto.Add(new PersonaDto
        //        {
        //            id = persona.id,
        //            userId = persona.userId,
        //            idEmpresa = persona.idEmpresa,
        //            IdArea = persona.IdArea,
        //            identificacion = persona.identificacion,
        //            Estado = persona.Estado,
        //            UserName = nombreUsuario.UserName,
        //            AreaName = area.Nombre,
        //            NombreEmpresa = empresa.Nombre
        //        });
        //    }

        //    return Ok(personasDto);
        //}

        //[HttpGet("{Busqueda}")]
        //public async Task<ActionResult<List<PersonaDto>>> Get2(string Busqueda)
        //{
        //    // busca todas las personas que contengan la búsqueda en identificacion
        //    var personas = await _context.inv_persona.Where(x => x.identificacion.Contains(Busqueda)).ToListAsync();

        //    // busca todos los usuarios cuyo UserName contenga la busqueda
        //    var usuarios = await _context.aspnetusers.Where(x => x.UserName.Contains(Busqueda)).ToListAsync();

        //    // verifica si no hay resultados en ambas consultas
        //    if (!personas.Any() && !usuarios.Any())
        //    {
        //        return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
        //    }

        //    // lista para almacenar los resultados en formato DTO
        //    var personasDto = new List<PersonaDto>();

        //    // procesa las personas encontradas
        //    foreach (var persona in personas)
        //    {
        //        // busca el area asociada
        //        var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
        //        if (area == null)
        //        {
        //            return BadRequest($"No se encontró el área asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        // busca el usuario asociado a la persona
        //        var nombreUsuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);
        //        if (nombreUsuario == null)
        //        {
        //            return BadRequest($"No se encontró el usuario asociado para la persona con identificación: {persona.identificacion}");
        //        }

        //        // busca la empresa asociada
        //        var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
        //        if (empresa == null)
        //        {
        //            return BadRequest($"No se encontró la empresa asociada para la persona con identificación: {persona.identificacion}");
        //        }

        //        // agrega el DTO para la persona
        //        personasDto.Add(new PersonaDto
        //        {
        //            id = persona.id,
        //            userId = persona.userId,
        //            idEmpresa = persona.idEmpresa,
        //            IdArea = persona.IdArea,
        //            identificacion = persona.identificacion,
        //            Estado = persona.Estado,
        //            UserName = nombreUsuario.UserName, // muestra el UserName de la persona asociada
        //            AreaName = area.Nombre,
        //            NombreEmpresa = empresa.Nombre
        //        });
        //    }

        //    // agrega los usuarios que no estan asociados a personas
        //    foreach (var usuario in usuarios)
        //    {
        //        // verifica si el usuario ya esta asociado a una persona
        //        var personaExistente = personas.FirstOrDefault(x => x.userId == usuario.Id);
        //        if (personaExistente == null)
        //        {
        //            // si el usuario ya fue asociado a una persona, se omite
        //            continue;
        //        }

        //        // agrega el DTO solo con la informacion del usuario
        //        personasDto.Add(new PersonaDto
        //        {
        //            userId = usuario.Id,
        //            idEmpresa = personas.idEmpresa,
        //            IdArea = personas.IdArea,
        //            identificacion = personas.identificacion,
        //            Estado = personas.Estado,
        //            UserName = usuario.UserName,
        //            AreaName = area.Nombre,
        //            NombreEmpresa = empresa.Nombre
        //        });
        //    }

        //    return Ok(personasDto);
        //}

        [HttpGet("{Busqueda}")]
        public async Task<ActionResult<List<PersonaDto>>> Get2(string Busqueda)
        {
            var personas = await _context.inv_persona.Where(x => x.identificacion != null && x.identificacion.Contains(Busqueda)).OrderByDescending(x => x.userId).ToListAsync();

            var usuarios = await _context.aspnetusers.Where(x => x.UserName != null && x.UserName.Contains(Busqueda)).ToListAsync();

            if (!personas.Any() && !usuarios.Any())
            {
                return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
            }

            var personasDto = new List<PersonaDto>();

            foreach (var persona in personas)
            {
                var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
                if (area == null)
                {
                    return BadRequest($"No se encontró el área asociada para la persona con identificación: {persona.identificacion}");
                }

                var nombreUsuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);
                if (nombreUsuario == null)
                {
                    return BadRequest($"No se encontró el usuario asociado para la persona con identificación: {persona.identificacion}");
                }

                var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
                if (empresa == null)
                {
                    return BadRequest($"No se encontró la empresa asociada para la persona con identificación: {persona.identificacion}");
                }

                personasDto.Add(new PersonaDto
                {
                    userId = persona.userId,
                    idEmpresa = empresa.id,
                    IdArea = persona.IdArea,
                    identificacion = persona.identificacion,
                    Estado = persona.Estado,
                    UserName = nombreUsuario.UserName,
                    AreaName = area.Nombre,
                    NombreEmpresa = empresa.Nombre,
                    Nombres = persona.Nombres,
                    Apellidos = persona.Apellidos
                });
            }

            foreach (var usuario in usuarios)
            {
                var nombre = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == usuario.Id);
                if (nombre == null)
                {
                    continue; // Omite usuarios sin persona asociada
                }

                var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == nombre.IdArea);
                var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == nombre.idEmpresa);

                if (area == null || empresa == null)
                {
                    return BadRequest($"No se encontró la empresa o área asociada para el usuario: {usuario.UserName}");
                }

                personasDto.Add(new PersonaDto
                {
                    userId = usuario.Id,
                    idEmpresa = empresa.id,
                    IdArea = area.id,
                    identificacion = nombre.identificacion,
                    Estado = nombre.Estado,
                    UserName = usuario.UserName,
                    AreaName = area.Nombre,
                    NombreEmpresa = empresa.Nombre,
                    Nombres = nombre.Nombres,
                    Apellidos = nombre.Apellidos
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
            return CreatedAtAction(nameof(GetId), new { id = persona.userId }, persona);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, PersonaCreateDto updateDto)
        {
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == id);

            //verificacion si existe el area
            var areaExiste = await _context.inv_area.FirstOrDefaultAsync(x => x.id == updateDto.IdArea);
            if (areaExiste == null)
            {
                return BadRequest($"El area con el ID {updateDto.IdArea} no fue encontrado.");
            }

            persona = _mapper.Map(updateDto, persona);

            _context.inv_persona.Update(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { persona.userId }, persona);

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