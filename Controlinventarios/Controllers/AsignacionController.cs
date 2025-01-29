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
                    id = asignacion.id,
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
                            Id = ia.id,
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
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.NumeroSerial == NumeroSerial);
            if  (ensamble == null)
            {
                return BadRequest($"No se encontro el serial: {NumeroSerial}");
            }

            // Verifica si existe la asignación con el id dado
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == ensamble.Id);

            if (asignacion == null)
            {
                return BadRequest($"No existe el id de asignación: {ensamble.Id}");
            }

            // consulta linq para obtener los detalles asociados a la asignacion
            var query = from ia in _context.inv_asignacion
                        join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                        join a in _context.aspnetusers on ip.userId equals a.Id
                        join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                        where ie.NumeroSerial == NumeroSerial  // filtro por el id recibido
                        select new AsignacionDto
                        {
                            id = ia.id,
                            IdEnsamble = ia.IdEnsamble,
                            IdPersona = ia.IdPersona,
                            NombrePersona = a.UserName,
                            Numeroserial = ie.NumeroSerial,
                        };

            var result = await query.FirstOrDefaultAsync(); // recupera el primer (y unico) elemento

            if (result == null)
            {
                return BadRequest($"No se encontro el id: {NumeroSerial}"); // si no se encuentra, devuelve error
            }

            return Ok(result); // si existe, devuelve el resultado con un codigo 200
        }




        [HttpGet("{idEnsamble}")]
        public async Task<ActionResult<AsignacionDto>> GetId(int idEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == idEnsamble);
            if (asignacion == null)
            {
                return BadRequest($"No existe el id ensamble: {idEnsamble}");
            }
            var asignacionDto = _mapper.Map<AsignacionDto>(asignacion);
            return Ok(asignacionDto);
        }



        [HttpPost]
        public async Task<ActionResult> Post(AsignacionCreateDto createDto)
        {
            // el dto verifica la tabla
            var asignacion = _mapper.Map<Asignacion>(createDto);

            //Verificacion del id de usuario
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == createDto.idPersona);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {createDto.idPersona} no fue encontrado.");
            }

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.idEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.idEnsamble} no fue encontrado.");
            }

            // añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = asignacion.IdPersona }, asignacion);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, AsignacionCreateDto updateDto)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdPersona == id);

            //Verificacion del id de usuario
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == updateDto.idPersona);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {updateDto.idPersona} no fue encontrado.");
            }

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacion.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {asignacion.IdEnsamble} no fue encontrado.");
            }

            asignacion = _mapper.Map(updateDto, asignacion);

            _context.inv_asignacion.Update(asignacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { asignacion.IdPersona }, asignacion);

        }


        [HttpDelete("{IdEnsamble}")]
        public async Task<ActionResult> Delete(int IdEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FindAsync(IdEnsamble);

            if (asignacion == null)
            {
                return BadRequest($"No existe el ensamble: {IdEnsamble}");
            }

            _context.inv_asignacion.Remove(asignacion);
            await _context.SaveChangesAsync();

            return Ok($"Se elimino el ensamble: {IdEnsamble}");
        }

    }
}