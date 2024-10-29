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
            var areas = await _context.Area.ToListAsync();
            //var areaDtos = _mapper.Map<List<AreaDto>>(areas);

            return Ok(areas);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDto>> GetId(int id)
        {
            var area = await _context.Area.FirstOrDefaultAsync(x => x.id == id);
            if (area == null)
            {
                return BadRequest();
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
            _context.Area.Add(area);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = area.id }, area);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, AreaCreateDto updateDto)
        {
            var area = await _context.Area.FirstOrDefaultAsync(x => x.id == id);

            area = _mapper.Map(updateDto, area);

            _context.Area.Update(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { area.id }, area);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var area = await _context.Area.FindAsync(id);

            if (area == null)
            {
                return BadRequest();
            }

            _context.Area.Remove(area);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}