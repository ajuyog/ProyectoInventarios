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
            var asignacion = await _context.inv_asignacion.ToListAsync();
            var asignacionDtos = _mapper.Map<List<AsignacionDto>>(asignacion);

            return Ok(asignacionDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AsignacionDto>> GetId(int id)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdPersona == id);
            if (asignacion == null)
            {
                return BadRequest($"No existe el id: {id}");
            }
            var asignacionDto = _mapper.Map<AsignacionDto>(asignacion);
            return Ok(asignacionDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(AsignacionCreateDto createDto)
        {
            // el dto verifica la tabla
            var asignacion = _mapper.Map<Asignacion>(createDto);

            // añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = asignacion.IdPersona }, asignacion);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, AsignacionCreateDto updateDto)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdPersona == id);

            asignacion = _mapper.Map(updateDto, asignacion);

            _context.inv_asignacion.Update(asignacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { asignacion.IdPersona }, asignacion);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var asignacion = await _context.inv_asignacion.FindAsync(id);

            if (asignacion == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_asignacion.Remove(asignacion);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}