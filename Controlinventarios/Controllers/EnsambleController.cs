using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Http.HttpResults;
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

            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);
            if (ensamble == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            var ensambleDto = _mapper.Map<EnsambleDto>(ensamble);
            return Ok(ensambleDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(EnsambleCreateDto createDto)
        {
            // el dto verifica la tabla
            var ensamble = _mapper.Map<Ensamble>(createDto);

            //Verificacion de ElemenType
            var elementoExiste = await _context.inv_elementType.FindAsync(createDto.IdElementType);
            if (elementoExiste == null)
            {
                return NotFound($"El tipo de elemento con el ID {createDto.IdElementType} no fue encontrado.");
            }

            //Verificacion sobre la marca
            var marcaExiste = await _context.inv_persona.FindAsync(createDto.IdMarca);
            if (marcaExiste == null)
            {
                return NotFound($"La persona con el ID {createDto.IdMarca} no fue encontrado.");
            }

            // añade la entidad al contexto
            _context.inv_ensamble.Add(ensamble);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = ensamble.Id }, ensamble);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EnsambleCreateDto updateDto)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);

            //Verificacion de ElemenType
            var elementoExiste = await _context.inv_elementType.FindAsync(updateDto.IdElementType);
            if (elementoExiste == null)
            {
                return NotFound($"El tipo de elemento con el ID {updateDto.IdElementType} no fue encontrado.");
            }

            //Verificacion sobre la marca
            var marcaExiste = await _context.inv_persona.FindAsync(updateDto.IdMarca);
            if (marcaExiste == null)
            {
                return NotFound($"La persona con el ID {updateDto.IdMarca} no fue encontrado.");
            }

            ensamble = _mapper.Map(updateDto, ensamble);

            _context.inv_ensamble.Update(ensamble);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { ensamble.Id }, ensamble);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ensamble = await _context.inv_ensamble.FindAsync(id);

            if (ensamble == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_ensamble.Remove(ensamble);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}