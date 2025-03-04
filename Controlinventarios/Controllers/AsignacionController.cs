using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Polygonize;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Threading.Tasks;

namespace Controlinventarios.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public AsignacionController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<AsignacionDto>>> Get()
        {
            var asignaciones = await _context.inv_asignacion.ToListAsync();

            if (asignaciones == null)
            {
                return BadRequest("No se encontraron elementos asignados.");
            }
            
            var asignacionDtos = new List<AsignacionDto>();

            foreach (var asignacion in asignaciones)
            {
                var identificacionPersona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == asignacion.IdPersona);
                if (identificacionPersona == null)
                {
                    return BadRequest("El nombre no se encontro");
                }

                var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacion.IdEnsamble);
                if (ensamble == null)
                {
                    return BadRequest("El ensamble no se encontro");
                }

                var asignacionDto = new AsignacionDto
                {
                    IdPersona = asignacion.IdPersona,
                    IdEnsamble = asignacion.IdEnsamble,
                    NombrePersona = identificacionPersona.Nombres,
                    Numeroserial = ensamble.NumeroSerial,
                    FechaRegistro = asignacion.FechaRegistro
                };

            asignacionDtos.Add(asignacionDto);
            }

            return Ok(asignacionDtos);
        }

        [HttpGet("Usuario con su lista de equipos")]
        public async Task<ActionResult<List<ListaAsignacionDto>>> Get5()
        {
            // Obtener todos los usuarios con sus asignaciones y detalles de ensambles
            var usuariosConEquipos = await (from ip in _context.inv_persona
                                            join a in _context.aspnetusers on ip.userId equals a.Id
                                            join ia in _context.inv_asignacion on a.Id equals ia.IdPersona
                                            join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                                            join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                                            select new ListaAsignacionDto
                                            {
                                                //Obtener los datos de la tabla inv_persona
                                                IdPersona = a.Id,
                                                ApellidoPersona = ip.Nombres,
                                                NombrePersona = ip.Apellidos,
                                                Email = a.Email,
                                                //Obtener los datos de la tabla inv_ensamble
                                                EquiposAsignados = (from ia2 in _context.inv_asignacion
                                                             join ie3 in _context.inv_ensamble on ia2.IdEnsamble equals ie3.Id
                                                             join im in _context.inv_marca on ie3.IdMarca equals im.id
                                                             join ie4 in _context.inv_elementType on ie3.IdElementType equals ie4.id
                                                             where ia2.IdPersona == a.Id
                                                             select new ListaEnsambleDto
                                                             {
                                                                 Id = ie3.Id,
                                                                 NumeroSerial = ie3.NumeroSerial,
                                                                 Estado = ie3.Estado,
                                                                 Renting = ie3.Renting,
                                                                 TipoElemento = ie4.Nombre,
                                                                 NombreMarca = im.Nombre,
                                                                 FechaRegistroEquipo = ia2.FechaRegistro
                                                             }).ToList()
                                            }).ToListAsync();
            // Verificar si se encontraron resultados
            if (usuariosConEquipos == null || !usuariosConEquipos.Any())
            {
                return BadRequest("No se encontraron usuarios con equipos asignados.");
            }

            return Ok(usuariosConEquipos);
        }

        [HttpGet("Consulta linq")]
        public async Task<ActionResult<List<AsignacionDto>>> Get3()
        {
            var query = from ia in _context.inv_asignacion
                        join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                        join a in _context.aspnetusers on ip.userId equals a.Id
                        join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                        select new
                        {
                            Nombre = a.UserName,
                            Serial = ie.NumeroSerial,
                            Elemento = ie2.Nombre
                        };

            var result = query.ToList();

            // Devuelve los resultados en formato JSON
            return Ok(result);
        }


        [HttpGet("Prueba")]
        public async Task<ActionResult<List<AsignacionDto>>> Get4()
        {
            // Obtener todos los ensambles de la base de datos
            var existeEnsamble = await _context.inv_ensamble.ToListAsync();
            if (existeEnsamble == null)
            {
                return BadRequest("No existen ensambles");
            }

            // Obtener los datos de la base de datos sin el Numeroserial
            var datosAsignacion = await (from ia in _context.inv_asignacion
                                         join a in _context.aspnetusers on ia.IdPersona equals a.Id
                                         join ip in _context.inv_persona on a.Id equals ip.userId
                                         select new
                                         {
                                             Email = a.Email,
                                             NombrePersona = ip.Nombres,
                                             IdEnsamble = ia.IdEnsamble,
                                             ApellidoPersona = ip.Apellidos,
                                             FechaRegistro = ia.FechaRegistro,
                                             IdPersona = ia.IdPersona
                                         }).ToListAsync();

            // Verificar si se encontraron resultados
            if (datosAsignacion == null || !datosAsignacion.Any())
            {
                return BadRequest("No se encontraron elementos asignados.");
            }

            // Combinar los datos con la lista existeEnsamble en memoria
            var resultado = datosAsignacion.Select(datos => new AsignacionDto
            {
                Email = datos.Email,
                NombrePersona = datos.NombrePersona,
                IdEnsamble = datos.IdEnsamble,
                ApellidoPersona = datos.ApellidoPersona,
                Numeroserial = existeEnsamble.FirstOrDefault(ie => ie.Id == datos.IdEnsamble)?.NumeroSerial ?? "No disponible",
                FechaRegistro = datos.FechaRegistro,
                IdPersona = datos.IdPersona
            }).ToList();

            return Ok(resultado);
        }


        [HttpGet("ConsultaLinq/{NumeroSerial}")]
        public async Task<ActionResult<AsignacionDto>> GetById(string NumeroSerial)
        {
            // Busca el ensamble que contenga el número de serial (coincidencia parcial)
            var ensambles = await _context.inv_ensamble
                .Where(x => x.NumeroSerial.Contains(NumeroSerial))
                .ToListAsync();

            if (ensambles == null || !ensambles.Any())
            {
                return BadRequest($"No se encontró ningún ensamble con el serial: {NumeroSerial}");
            }

            // Consulta LINQ para obtener los detalles asociados a las asignaciones de todos los ensambles encontrados
            var query = from ia in _context.inv_asignacion
                        join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                        join a in _context.aspnetusers on ip.userId equals a.Id
                        join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                        where ie.NumeroSerial.Contains(NumeroSerial)  // Filtro por coincidencia parcial en NumeroSerial
                        select new AsignacionDto
                        {
                            IdEnsamble = ia.IdEnsamble,
                            IdPersona = ia.IdPersona,
                            NombrePersona = a.UserName,
                            Numeroserial = ie.NumeroSerial,
                            FechaRegistro = ia.FechaRegistro
                        };

            var result = await query.ToListAsync(); // Recupera todos los resultados que coinciden    

            if (result == null || !result.Any())
            {
                return BadRequest($"No se encontró ningún resultado para el serial: {NumeroSerial}");
            }

            return Ok(result); // Devuelve la lista con todos los resultados encontrados
        }


        [HttpGet("{idEnsamble}")]
        public async Task<ActionResult<AsignacionDto>> GetId(int idEnsamble)
        {
            // Primero, buscamos la asignación correspondiente al idEnsamble
            var asignacion = await _context.inv_asignacion
                .Where(x => x.IdEnsamble == idEnsamble) // Filtramos por el IdEnsamble
                .ToListAsync();

            if (asignacion == null || !asignacion.Any())
            {
                return BadRequest($"No existe el id de ensamble: {idEnsamble}");
            }

            // Si encuentras la asignación, puedes buscar el nombre del ensamble
            var nombreEnsamble = await _context.inv_ensamble
                .Where(x => x.Id == idEnsamble)
                .FirstOrDefaultAsync();

            if (nombreEnsamble == null)
            {
                return BadRequest("No se encontraron ensambles con ese id.");
            }

            // Consulta LINQ para obtener los detalles asociados a las asignaciones
            var query = from ia in _context.inv_asignacion
                        join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                        join a in _context.aspnetusers on ip.userId equals a.Id
                        join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                        where ia.IdEnsamble == idEnsamble  // Filtro por el id recibido
                        select new AsignacionDto
                        {
                            IdEnsamble = ia.IdEnsamble,
                            IdPersona = ia.IdPersona,
                            NombrePersona = a.UserName,
                            Numeroserial = ie.NumeroSerial,
                            FechaRegistro = ia.FechaRegistro
                        };

            var result = await query.ToListAsync(); // Devuelve todos los resultados que coinciden

            if (result == null || !result.Any())
            {
                return BadRequest($"No se encontró ninguna asignación para el id de ensamble: {idEnsamble}");
            }

            return Ok(result); // Devuelve la lista con todos los resultados encontrados
        }


        [HttpPost]
        public async Task<ActionResult> Post(AsignacionCreateDto createDto)
        {
            // El DTO verifica la tabla
            var asignacion = _mapper.Map<Asignacion>(createDto);

            // Verificación del id de usuario
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == createDto.IdPersona);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {createDto.IdPersona} no fue encontrada.");
            }

            var usuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == usuarioExiste.userId);
            if (usuario == null)
            {
                return BadRequest("No se encontraron usuarios");
            }

            // Verificación del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
            }

            asignacion.FechaRegistro = DateOnly.FromDateTime(DateTime.Now);

            // Añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);

            // Guardar los datos en la base de datos
            await _context.SaveChangesAsync();

            // Retorna lo guardado, asegurándose de que el parámetro de la ruta coincida con lo esperado en GetId
            return CreatedAtAction(nameof(GetId), new { idEnsamble = asignacion.IdEnsamble }, asignacion);
        }

        [HttpPut("{idEnsamble}")]
        public async Task<ActionResult> Update(int idEnsamble, AsignacionUpdateDto updateDto)
        {
            var ensambleExiste = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == idEnsamble);

            if (ensambleExiste == null)
            {
                return BadRequest($"No se encontró un ensamble con el id: {idEnsamble}");
            }

            ensambleExiste = _mapper.Map(updateDto, ensambleExiste);

            _context.inv_asignacion.Update(ensambleExiste);
            await _context.SaveChangesAsync();

            // Se usa el nombre correcto del parámetro para la acción GetId
            return CreatedAtAction(nameof(GetId), new { idEnsamble = ensambleExiste.IdEnsamble }, ensambleExiste);
        }

        
        [HttpPatch("CambiarAsignado/{EnsambleId}")]
        public async Task<ActionResult> Patch(int EnsambleId, AsignacionPatchDto patchDto)
        {
            // Verificar si el ensamble existe
            var asignacionExistente = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == EnsambleId);
                
            if (asignacionExistente == null)
            {
                return BadRequest($"El ensamble {EnsambleId} no existe");
            }

            // Verificar si el nuevo usuario asignado existe
            var usuarioExiste = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == patchDto.IdPersona);
                
            if (usuarioExiste == null)
            {
                return BadRequest($"No existe el usuario: {patchDto.IdPersona}");
            }

            // Actualizar solo la propiedad IdPersona
            asignacionExistente.IdPersona = patchDto.IdPersona;

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Retornar la respuesta con CreatedAtAction
            return CreatedAtAction(nameof(GetId), new { idEnsamble = asignacionExistente.IdEnsamble }, asignacionExistente);
        }


        [HttpDelete("{IdEnsamble}")]
        public async Task<ActionResult> Delete(int IdEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == IdEnsamble);

            if (asignacion == null)
            {
                return BadRequest($"No existe el ensamble: {IdEnsamble}");
            }

            _context.inv_asignacion.Remove(asignacion);
            await _context.SaveChangesAsync();

            return Ok($"Se eliminó el ensamble: {IdEnsamble}");
        }
    }
}