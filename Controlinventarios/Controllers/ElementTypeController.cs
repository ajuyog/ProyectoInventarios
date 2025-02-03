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
    public class ElementTypeController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public ElementTypeController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ElementTypeDto>>> Get()
        {
            // Obtén la lista de elementos de la tabla inv_elementType
            var elementos = await _context.inv_elementType.ToListAsync();

            if (elementos == null)
            {
                return BadRequest("No se encontraron tipos de elementos");
            }

            // Inicializamos la lista para almacenar los DTOs
            var elementoDtos = elementos.Select(e => new ElementTypeDto
            {
                // Mapea las propiedades necesarias del objeto 'e' al DTO
                id = e.id, // Suponiendo que 'Id' sea una propiedad de 'inv_elementType'
                Nombre = e.Nombre // Suponiendo que 'nombre' sea una propiedad de 'inv_elementType'
                              // Puedes agregar más propiedades según el modelo que tengas en 'inv_elementType'
            }).ToList();

            return Ok(elementoDtos);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ElementTypeDto>> GetId(int id)
        {
            var elemento = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == id);
            if (elemento == null)
            {
                return BadRequest($"No existe el id: {id}");
            }
            var elementoDto = _mapper.Map<ElementTypeDto>(elemento);
            return Ok(elementoDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(ElementTypeCreateDto createDto)
        {
            // el dto verifica la tabla
            var elemento = _mapper.Map<ElementType>(createDto);
            // añade la entidad al contexto
            _context.inv_elementType.Add(elemento);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = elemento.id }, elemento);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ElementTypeCreateDto updateDto)
        {
            var elemento = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == id);

            elemento = _mapper.Map(updateDto, elemento);

            _context.inv_elementType.Update(elemento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { elemento.id }, elemento);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var elemento = await _context.inv_elementType.FindAsync(id);

            if (elemento == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_elementType.Remove(elemento);
            await _context.SaveChangesAsync();

            return Ok($"Se elimino el elemento: {elemento}");
        }
    }
}