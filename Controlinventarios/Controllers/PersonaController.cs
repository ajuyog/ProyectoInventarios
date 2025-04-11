using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Controlinventarios.Utildad;
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
        public async Task<ActionResult<object>> Get(string searchUsuarios, [FromQuery] PaginacionDTO paginacionDTO)
        {
            if (!string.IsNullOrEmpty(searchUsuarios))
            {
                var personasConfi = _context.inv_persona
                .Join(_context.inv_area, persona => persona.IdArea, idarea => idarea.id, (persona, idarea) => new
                {
                    persona,
                    idarea
                })
                .Join(_context.inv_empresa, persona => persona.persona.idEmpresa, empresa => empresa.id, (persona, empresa) => new
                {
                    persona,
                    empresa
                })
                .Join(_context.aspnetusers, userName => userName.persona.persona.userId, user => user.Id, (userName, user) => new
                {
                    userName.persona,
                    userName.persona.idarea,
                    userName.empresa,
                    user
                })
                .Where(x => x.user.UserName.Contains(searchUsuarios)
                    || x.idarea.Nombre.Contains(searchUsuarios)
                    || x.empresa.Nombre.Contains(searchUsuarios)
                    || x.persona.persona.identificacion.Contains(searchUsuarios)
                    || x.persona.persona.Nombres.Contains(searchUsuarios)
                    || x.persona.persona.Apellidos.Contains(searchUsuarios)
                    || x.persona.persona.Email.Contains(searchUsuarios)
                    || x.persona.persona.TelefonoMovil.Contains(searchUsuarios)
                )
                .Select(x => new PersonaDto
                {
                    UserId = x.persona.persona.userId,
                    Identificacion = x.persona.persona.identificacion,
                    Nombres = x.persona.persona.Nombres,
                    Apellidos = x.persona.persona.Apellidos,
                    FechaCumpleaños = x.persona.persona.FechaCumpleaños,
                    Email = x.persona.persona.Email,
                    TelefonoMovil = x.persona.persona.TelefonoMovil,
                    Cargo =x.persona.persona.Cargo,
                    AreaName = x.idarea.Nombre ?? "No tiene area",
                    NombreEmpresa = x.empresa.Nombre ?? "No tiene empresa",
                    IdArea = x.persona.idarea.id,
                    IdEmpresa = x.empresa.id,
                    Estado = x.persona.persona.Estado,
                })
                .OrderBy(e => e.UserId).AsQueryable();

                var personas = await personasConfi.Paginar(paginacionDTO).ToListAsync();
                await HttpContext.InsertarParametrosPaginacionEnCabecera(personasConfi);
                await HttpContext.TInsertarParametrosPaginacion(personasConfi, paginacionDTO.RegistrosPorPagina);
                return Ok(personas);
            }
            else
            {
                var usuariosConfival = _context.inv_persona
               .Join(_context.inv_area, persona => persona.IdArea, idarea => idarea.id, (persona, idarea) => new
               {
                   persona,
                   idarea
               })
                .Join(_context.inv_empresa, persona => persona.persona.idEmpresa, empresa => empresa.id, (persona, empresa) => new
                {
                    persona,
                    empresa
                })
                .Join(_context.aspnetusers, userName => userName.persona.persona.userId, user => user.Id, (userName, user) => new
                {
                    userName.persona,
                    userName.persona.idarea,
                    userName.empresa,
                    user
                })
                 .Select(x => new PersonaDto
                 {
                     UserId = x.persona.persona.userId,
                     Identificacion = x.persona.persona.identificacion,
                     Nombres = x.persona.persona.Nombres,
                     Apellidos = x.persona.persona.Apellidos,
                     FechaCumpleaños = x.persona.persona.FechaCumpleaños,
                     Email = x.persona.persona.Email,
                     TelefonoMovil = x.persona.persona.TelefonoMovil,
                     Cargo = x.persona.persona.Cargo,
                     AreaName = x.idarea.Nombre ?? "No tiene area",
                     NombreEmpresa = x.empresa.Nombre ?? "No tiene empresa",
                     IdArea = x.persona.idarea.id,
                     IdEmpresa = x.empresa.id,
                     Estado = x.persona.persona.Estado,
                 })
                .OrderBy(e => e.UserId).AsQueryable();

                var personas = await usuariosConfival.Paginar(paginacionDTO).ToListAsync();
                await HttpContext.InsertarParametrosPaginacionEnCabecera(usuariosConfival);
                await HttpContext.TInsertarParametrosPaginacion(usuariosConfival, paginacionDTO.RegistrosPorPagina);
                return Ok(personas);
            }
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
                UserId = persona.userId,
                IdEmpresa = nombreEmpresa.id,
                IdArea = persona.IdArea,
                Identificacion = persona.identificacion,
                Estado = persona.Estado,
                UserName = user.UserName,
                AreaName = areaName.Nombre,
                NombreEmpresa = nombreEmpresa.Nombre,
                Nombres = persona.Nombres,
                Apellidos = persona.Apellidos
            };

            return Ok(personaDto);
        }

        [HttpGet("{Busqueda}")]
        public async Task<ActionResult<List<PersonaDto>>> Get2(string Busqueda)
        {
            // Busca personas por identificación que contenga el valor de 'Busqueda'
            var personas = await _context.inv_persona.Where(x => x.identificacion != null && x.identificacion.Contains(Busqueda)).OrderByDescending(x => x.userId).ToListAsync();

            // Busca usuarios por nombre de usuario que contenga el valor de 'Busqueda'
            var usuarios = await _context.aspnetusers.Where(x => x.UserName != null && x.UserName.Contains(Busqueda)).ToListAsync();

            // Busca personas por nombres que contengan el valor de 'Busqueda'
            var nombres = await _context.inv_persona.Where(x => x.Nombres != null && x.Nombres.Contains(Busqueda)).ToListAsync();

            // Si no hay coincidencias en personas, usuarios ni nombres, devuelve un error
            if (!personas.Any() && !usuarios.Any() && !nombres.Any())
            {
                return BadRequest($"No se encontraron coincidencias con el criterio: {Busqueda}");
            }

            // Lista para almacenar los resultados transformados a DTO
            var personasDto = new List<PersonaDto>();

            // Procesa las personas encontradas
            foreach (var persona in personas)
            {
                // Busca el área asociada a la persona
                var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == persona.IdArea);
                if (area == null)
                {
                    return BadRequest($"No se encontró el área asociada para la persona con identificación: {persona.identificacion}");
                }

                // Busca el nombre de usuario asociado a la persona
                var nombreUsuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == persona.userId);
                if (nombreUsuario == null)
                {
                    return BadRequest($"No se encontró el usuario asociado para la persona con identificación: {persona.identificacion}");
                }

                // Busca la empresa asociada a la persona
                var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == persona.idEmpresa);
                if (empresa == null)
                {
                    return BadRequest($"No se encontró la empresa asociada para la persona con identificación: {persona.identificacion}");
                }

                // Agrega la persona transformada a DTO a la lista
                personasDto.Add(new PersonaDto
                {
                    UserId = persona.userId,
                    IdEmpresa = empresa.id,
                    IdArea = persona.IdArea,
                    Identificacion = persona.identificacion,
                    Estado = persona.Estado,
                    UserName = nombreUsuario.UserName,
                    AreaName = area.Nombre,
                    NombreEmpresa = empresa.Nombre,
                    Nombres = persona.Nombres,
                    Apellidos = persona.Apellidos
                });
            }

            // Procesa los usuarios encontrados
            foreach (var usuario in usuarios)
            {
                // Busca la persona asociada al usuario
                var nombre = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == usuario.Id);
                if (nombre == null)
                {
                    continue; // Omite usuarios sin persona asociada
                }

                // Busca el área y la empresa asociadas a la persona
                var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == nombre.IdArea);
                var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == nombre.idEmpresa);

                // Si no se encuentra el área o la empresa, devuelve un error
                if (area == null || empresa == null)
                {
                    return BadRequest($"No se encontró la empresa o área asociada para el usuario: {usuario.UserName}");
                }

                // Agrega el usuario transformado a DTO a la lista
                personasDto.Add(new PersonaDto
                {
                    UserId = usuario.Id,
                    IdEmpresa = empresa.id,
                    IdArea = area.id,
                    Identificacion = nombre.identificacion,
                    Estado = nombre.Estado,
                    UserName = usuario.UserName,
                    AreaName = area.Nombre,
                    NombreEmpresa = empresa.Nombre,
                    Nombres = nombre.Nombres,
                    Apellidos = nombre.Apellidos
                });
            }

            // Devuelve la lista de resultados
            return Ok(personasDto);
        }

        [HttpGet("BusquedaPorId/{userId}")]
        public async Task<ActionResult<PersonaDto>> GetById(string userId)
        {
            // Verifica si la persona existe
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == userId);

            if (persona == null)
            {
                return BadRequest($"No existe la identificacion: {userId}");
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
                UserId = persona.userId,
                IdEmpresa = nombreEmpresa.id,
                IdArea = persona.IdArea,
                Identificacion = persona.identificacion,
                Estado = persona.Estado,
                UserName = user.UserName,
                AreaName = areaName.Nombre,
                NombreEmpresa = nombreEmpresa.Nombre,
                Nombres = persona.Nombres,
                Apellidos = persona.Apellidos
            };

            return Ok(personaDto);
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
            var userId = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == createDto.userId);
            if (userId != null)
            {
                return BadRequest($"Ya existe una persona con el id: {createDto.userId}");
            }

            // añade la entidad al contexto
            _context.inv_persona.Add(persona);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return Created("", persona);  // Devuelve 201 pero sin una URL específica.
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> Update(string userId, PersonaUpdateDto updateDto)
        {
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == userId);
            if (persona == null)
            {
                return BadRequest($"No ecnontraron personas con el id: {userId}");
            }
            //verificacion si existe el area
            var areaExiste = await _context.inv_area.FirstOrDefaultAsync(x => x.id == updateDto.IdArea);
            if (areaExiste == null)
            {
                return BadRequest($"El area con el ID {updateDto.IdArea} no fue encontrado.");
            }

            persona = _mapper.Map(updateDto, persona);

            _context.inv_persona.Update(persona);
            await _context.SaveChangesAsync();

            return Created("", persona);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var persona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == id);

            if (persona == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_persona.Remove(persona);
            await _context.SaveChangesAsync();

            return Ok($"Se ellimino correctamente la persona con el id {id}");
        }
    }
}