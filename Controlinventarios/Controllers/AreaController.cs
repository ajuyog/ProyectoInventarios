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
    public class AreaController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public AreaController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<List<AreaDto>>> Get()
        {
            var areas = await _context.inv_area.ToListAsync();
            var areaDtos = _mapper.Map<List<AreaDto>>(areas);

            return Ok(areaDtos);
        }


        [HttpGet("{nombre}")]
        public async Task<ActionResult<AreaDto>> GetId(string nombre)
        {
            var area = await _context.inv_area.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (area == null)
            {
                return BadRequest($"No existe el nombre: {nombre}");
            }
            var areaDto = _mapper.Map<AreaDto>(area);
            return Ok(areaDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(AreaCreateDto createDto)
        {
            // el dto verifica la tabla
            var area = _mapper.Map<Area>(createDto);
            
            // añade la entidad al contexto
            _context.inv_area.Add(area);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = area.id }, area);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, AreaCreateDto updateDto)
        {
            var area = await _context.inv_area.FirstOrDefaultAsync(x => x.id == id);

            area = _mapper.Map(updateDto, area);

            _context.inv_area.Update(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { area.id }, area);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var area = await _context.inv_area.FindAsync(id);

            if (area == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_area.Remove(area);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}