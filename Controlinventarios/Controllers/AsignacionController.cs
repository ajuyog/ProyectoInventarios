using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Polygonize;
using Newtonsoft.Json.Linq;
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


        //[HttpGet]
        //public async Task<ActionResult<List<AsignacionDto>>> Get()
        //{
        //    var asignacion = await _context.inv_asignacion.ToListAsync();
        //    var asignacionDtos = _mapper.Map<List<AsignacionDto>>(asignacion);

        //    return Ok(asignacionDtos);
        //}

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
                var identificacionPersona = await _context.inv_persona.FirstOrDefaultAsync(o => o.userId == asignacion.IdPersona);
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
                    Numeroserial = ensamble.NumeroSerial
                };

            asignacionDtos.Add(asignacionDto);
            }

            return Ok(asignacionDtos);
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

        [HttpGet("ConsultaLinq/{NumeroSerial}")]
        public async Task<ActionResult<AsignacionDto>> GetById(string NumeroSerial)
        {
            // Busca el ensamble que contenga el número de serial (coincidencia parcial)
            var ensamble = await _context.inv_ensamble
                .FirstOrDefaultAsync(x => x.NumeroSerial.Contains(NumeroSerial));

            if (ensamble == null)
            {
                return BadRequest($"No se encontró ningún ensamble con el serial: {NumeroSerial}");
            }

            // Verifica si existe la asignación con el IdEnsamble encontrado
            var asignacion = await _context.inv_asignacion
                .FirstOrDefaultAsync(x => x.IdEnsamble == ensamble.Id);

            if (asignacion == null)
            {
                return BadRequest($"No existe una asignación para el ensamble con Id: {ensamble.Id}");
            }

            // Consulta LINQ para obtener los detalles asociados a la asignación
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
                        };

            var result = await query.FirstOrDefaultAsync(); // Recupera el primer (y único) elemento

            if (result == null)
            {
                return BadRequest($"No se encontró ningún resultado para el serial: {NumeroSerial}");
            }

            return Ok(result); // Si existe, devuelve el resultado con un código 200
        }

        [HttpGet("{idEnsamble}")]
        public async Task<ActionResult<AsignacionDto>> GetId(int idEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == idEnsamble);
            if (asignacion == null)
            {
                return BadRequest($"No existe el id ensamble: {idEnsamble}");
            }

            var nombreEnsamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacion.IdEnsamble);
            if (nombreEnsamble == null)
            {
                return BadRequest("No se encontraron ensambles ");
            }

            var query = from ia in _context.inv_asignacion
                        join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                        join a in _context.aspnetusers on ip.userId equals a.Id
                        join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                        where ia.IdEnsamble == idEnsamble  // filtro por el id recibido
                        select new AsignacionDto
                        {
                            IdEnsamble = ia.IdEnsamble,
                            IdPersona = ia.IdPersona,
                            NombrePersona = a.UserName,
                            Numeroserial = ie.NumeroSerial,
                        };

            var result = await query.FirstOrDefaultAsync(); // recupera el primer (y unico) elemento

            if (result == null)
            {
                return BadRequest($"No se encontro el id: {idEnsamble}"); // si no se encuentra, devuelve error
            }

            //var asignacionDto = _mapper.Map<AsignacionDto>(asignacion);
            return Ok(result);
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

            // Verificación del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
            }

            asignacion.FechaDeRegistro = DateOnly.FromDateTime(DateTime.Now);

            // Añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);

            // Guardar los datos en la base de datos
            await _context.SaveChangesAsync();

            // Retorna lo guardado, asegurándose de que el parámetro de la ruta coincida con lo esperado en GetId
            return CreatedAtAction(nameof(GetId), new { idEnsamble = asignacion.IdEnsamble }, asignacion);
        }


        [HttpPut("{userId}")]
        public async Task<ActionResult> Update(string userId, AsignacionCreateDto updateDto)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdPersona == userId);
            if (asignacion == null)
            {
                return BadRequest($"No se encontraron asignaciones para el el usuario: {userId}");
            }

            // Verificación del id de usuario
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == updateDto.IdPersona);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {updateDto.IdPersona} no fue encontrado.");
            }

            // Verificación del id de Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacion.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {asignacion.IdEnsamble} no fue encontrado.");
            }

            asignacion = _mapper.Map(updateDto, asignacion);

            _context.inv_asignacion.Update(asignacion);
            await _context.SaveChangesAsync();

            // Cambiar el parámetro a 'IdEnsamble', que es el que se espera en GetId
            return CreatedAtAction(nameof(GetId), new { idpersona = asignacion.IdPersona }, asignacion);
        }


        [HttpDelete("{IdEnsamble}")]
        public async Task<ActionResult> Delete(int IdEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == IdEnsamble);

            if (asignacion == null)
            {
                return NotFound($"No existe el ensamble: {IdEnsamble}");
            }

            _context.inv_asignacion.Remove(asignacion);
            await _context.SaveChangesAsync();

            return Ok($"Se eliminó el ensamble: {IdEnsamble}");
        }

    }
}