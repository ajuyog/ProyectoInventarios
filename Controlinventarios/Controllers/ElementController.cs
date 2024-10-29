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
    public class ElementController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public ElementController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ElementDto>>> Get()
        {
            var elemento = await _context.inv_element.ToListAsync();
            var elementoDto = _mapper.Map<List<ElementDto>>(elemento);

            return Ok(elementoDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ElementDto>> GetId(int id)
        {
            var elemento = await _context.inv_element.FirstOrDefaultAsync(x => x.id == id);
            if (elemento == null)
            {
                return BadRequest();
            }
            var elementoDto = _mapper.Map<ElementDto>(elemento);
            return Ok(elementoDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ElementCreateDto createDto)
        {
            // el dto verifica la tabla
            var elemento = _mapper.Map<Element>(createDto);
            // añade la entidad al contexto
            _context.inv_element.Add(elemento);
            // guardar los datos en la basee de datoss
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = elemento.id }, elemento);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ElementCreateDto updateDto)
        {
            var elemento = await _context.inv_element.FirstOrDefaultAsync(x => x.id == id);

            elemento = _mapper.Map(updateDto, elemento);

            _context.inv_element.Update(elemento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { elemento.id }, elemento);

        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var elemento = await _context.inv_element.FindAsync(id);

            if (elemento == null)
            {
                return BadRequest();
            }

            _context.inv_element.Remove(elemento);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}