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
    public class EnsambleController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public EnsambleController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<EnsambleDto>>> Get()
        {
            var ensamble = await _context.inv_ensamble.ToListAsync();
            var ensambleDtos = _mapper.Map<List<EnsambleDto>>(ensamble);

            return Ok(ensambleDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EnsambleDto>> GetId(int id)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.id == id);
            if (ensamble == null)
            {
                return BadRequest();
            }
            var ensambleDto = _mapper.Map<EnsambleDto>(ensamble);
            return Ok(ensambleDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(EnsambleCreateDto createDto)
        {
            // el dto verifica la tabla
            var ensamble = _mapper.Map<Ensamble>(createDto);

            // añade la entidad al contexto
            _context.inv_ensamble.Add(ensamble);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = ensamble.id }, ensamble);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EnsambleCreateDto updateDto)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.id == id);

            ensamble = _mapper.Map(updateDto, ensamble);

            _context.inv_ensamble.Update(ensamble);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { ensamble.id }, ensamble);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ensamble = await _context.inv_ensamble.FindAsync(id);

            if (ensamble == null)
            {
                return BadRequest();
            }

            _context.inv_ensamble.Remove(ensamble);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}