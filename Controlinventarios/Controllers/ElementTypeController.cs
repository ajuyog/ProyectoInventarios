using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Controlinventarios.Utildad;
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

        [HttpGet("Paginado")]
        public async Task<ActionResult<object>> Get1(string search, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var query = _context.inv_elementType
            .Join(_context.inv_elementType,idelemento => idelemento.id,nombreelemento => nombreelemento.id,(idelemento, nombreelemento) => new
            {
                idelemento,
                nombreelemento
            })
            .Select(x => new ElementTypeDto
            {
                id = x.idelemento.id,
                Nombre = x.idelemento.Nombre,
                IdElementType = x.idelemento.IdElementType,
                NombreElemento = x.nombreelemento.Nombre
            })
            .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.Nombre.Contains(search) ||
                    x.NombreElemento.Contains(search)
                );
            }
            var elementos = await query.Paginar(paginacionDTO).ToListAsync();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(query);
            await HttpContext.TInsertarParametrosPaginacion(query, paginacionDTO.RegistrosPorPagina);

            return Ok(elementos);
        }
        [HttpGet]
        public async Task<ActionResult<List<ElementTypeDto>>> Get()
        {
            var elementos = await _context.inv_elementType.ToListAsync();

            if (elementos == null ) // Mejor validación
            {
                return BadRequest("No se encontraron tipos de elementos");
            }
            var elementoDtos = new List<ElementTypeDto>();

            foreach (var e in elementos)
            {
                // Obtener el nombre correspondiente al IdElementType de cada elemento
                var nombreElemento = await _context.inv_elementType
                    .FirstOrDefaultAsync(x => x.id == e.IdElementType); // Usar e.IdElementType

                elementoDtos.Add(new ElementTypeDto
                {
                    id = e.id,
                    Nombre = e.Nombre,
                    IdElementType = e.IdElementType,
                    NombreElemento = nombreElemento?.Nombre ?? "Sin referencia" // si es 0 que ponga Sin referencia en el campo de nombreElemento
                });
            }
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
            var elemento = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == id);

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