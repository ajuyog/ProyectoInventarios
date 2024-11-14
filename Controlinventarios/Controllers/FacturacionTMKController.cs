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
    public class FacturacionTMKController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public FacturacionTMKController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<FacturacionTMKDto>>> Get()
        {
            var facturacion = await _context.inv_facturaciontmk.ToListAsync();
            var facturacionDtos = _mapper.Map<List<FacturacionTMKDto>>(facturacion);

            return Ok(facturacionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacturacionTMKDto>> GetId(int id)
        {
            var facturacion = await _context.inv_facturaciontmk.FirstOrDefaultAsync(x => x.Id == id);
            if (facturacion == null)
            {
                return BadRequest($"No existe el id: {id}");
            }
            var facturacionDto = _mapper.Map<FacturacionTMKDto>(facturacion);
            return Ok(facturacionDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(FacturacionTMKCreateDto createDto)
        {
            // el dto verifica la tabla
            var facturacion = _mapper.Map<FacturacionTMK>(createDto);

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FindAsync(createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return NotFound($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
            }

            // añade la entidad al contexto
            _context.inv_facturaciontmk.Add(facturacion);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = facturacion.Id }, facturacion);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, FacturacionTMKCreateDto updateDto)
        {
            var facturacion = await _context.inv_facturaciontmk.FirstOrDefaultAsync(x => x.Id == id);

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FindAsync(updateDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return NotFound($"El ensamble con ID {updateDto.IdEnsamble} no fue encontrado.");
            }

            facturacion = _mapper.Map(updateDto, facturacion);

            _context.inv_facturaciontmk.Update(facturacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { facturacion.Id }, facturacion);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var facturacion = await _context.inv_facturaciontmk.FindAsync(id);

            if (facturacion == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_facturaciontmk.Remove(facturacion);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
