using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        //[HttpGet]
        //public async Task<ActionResult<List<EnsambleDto>>> Get()
        //{

        //    var ensamble = await _context.inv_ensamble.ToListAsync();
        //    var ensambleDtos = _mapper.Map<List<EnsambleDto>>(ensamble);

        //    return Ok(ensambleDtos);
        //}


        [HttpGet]
        public async Task<ActionResult<List<EnsambleDto>>> Get()
        {
            var ensamble = await _context.inv_ensamble.ToListAsync();

            if (ensamble == null)
            {
                return BadRequest("No existe ningun ensamble");
            }

            var EnsambleDtos = new List<EnsambleDto>();

            foreach (var ensambles in ensamble)
            {
                var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensambles.Id);
                if (elementype == null)
                {
                    return BadRequest("No tiene ningun tipo de elemento");
                }

                var ensambleDto = new EnsambleDto
                {
                    Id = ensambles.Id,
                    IdElementType = ensambles.IdElementType,
                    IdMarca = ensambles.IdMarca,
                    NumeroSerial = ensambles.NumeroSerial,
                    Estado = ensambles.Estado,
                    Descripcion = ensambles.Descripcion,
                    Renting = ensambles.Renting,
                    TipoElemento = elementype.Nombre,
                };

                EnsambleDtos.Add(ensambleDto);
            }

            return Ok(EnsambleDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EnsambleDto>> GetId(int id)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);

            if (ensamble == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensamble.IdElementType);

            if (elementype == null)
            {
                return BadRequest("No tiene ningun tipo de elemento");
            }

            var ensambleDto = new EnsambleDto
            {
                Id = ensamble.Id,
                IdElementType = ensamble.IdElementType,
                IdMarca = ensamble.IdMarca,
                NumeroSerial = ensamble.NumeroSerial,
                Estado = ensamble.Estado,
                Descripcion = ensamble.Descripcion,
                Renting = ensamble.Renting,
                TipoElemento = elementype.Nombre,  
            };

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
                return NotFound($"La marca con el ID {updateDto.IdMarca} no fue encontrado.");
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