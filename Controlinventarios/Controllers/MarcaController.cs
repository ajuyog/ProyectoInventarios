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
    public class MarcaController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public MarcaController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<MarcaDto>>> Get()
        {
            var marca = await _context.inv_marca.ToListAsync();
            var marcaDtos = _mapper.Map<List<MarcaDto>>(marca);

            return Ok(marcaDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<MarcaDto>> GetId(int id)
        {
            var marca = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == id);
            if (marca == null)
            {
                return BadRequest($"No existe el id: {id}");
            }
            var marcaDto = _mapper.Map<MarcaDto>(marca);
            return Ok(marcaDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(MarcaCreateDto createDto)
        {
            // el dto verifica la tabla
            var marca = _mapper.Map<Marca>(createDto);

            // añade la entidad al contexto
            _context.inv_marca.Add(marca);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = marca.id }, marca);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, MarcaCreateDto updateDto)
        {
            var marca = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == id);

            marca = _mapper.Map(updateDto, marca);

            _context.inv_marca.Update(marca);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { marca.id }, marca);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var marca = await _context.inv_marca.FindAsync(id);

            if (marca == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_marca.Remove(marca);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}