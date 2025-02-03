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
        //    if (ensamble == null)
        //    {
        //        return BadRequest($"No se encontraron datos en la tabla ensamble");
        //    }

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
                var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensambles.IdElementType);
                if (elementype == null)
                {
                    return BadRequest("No tiene ningun tipo de elemento");
                }

                var marca = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == ensambles.IdMarca);
                if (marca == null)
                {
                    return BadRequest("No se encontraron marcas");
                }

                var factura = await _context.inv_facturaciontmk.FirstOrDefaultAsync(x => x.IdEnsamble == ensambles.Id);
                if (factura == null)
                {
                    return BadRequest("No se encontraron numeros de faturas");
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
                    NumeroFactura = factura.Descripcion,
                    NombreMarca = marca.Nombre,
                    FechaRegistroEquipo = ensambles.FechaRegistroEquipo
                };

                EnsambleDtos.Add(ensambleDto);
            }

            return Ok(EnsambleDtos);
        }

        [HttpGet("Propiedades concatenadas")]
        public async Task<ActionResult<List<EnsambleDto2>>> GetPropiedadesConcatenadas()
        {
            var result = await _context.inv_ensamble
                .Join(_context.inv_propiedades,
                      ie => ie.Id,
                      ip => ip.IdEnsamble,
                      (ie, ip) => new { ie.Id, ie.NumeroSerial, ip.Propiedad })
                .OrderByDescending(x => x.Id) // Aquí ordenas primero por Id
                .GroupBy(x => x.NumeroSerial)
                .Select(g => new EnsambleDto2
                {
                    id = g.First().Id,
                    NumeroSerial = g.Key,
                    // ordenas dentro del grupo por id y concatenas las propiedades
                    PropiedadesConcatenadas = string.Join("; ", g.OrderBy(x => x.Id).Select(x => x.Propiedad)),
                })
                .ToListAsync();

            if (result == null )
            {
                return BadRequest("No se encontraron datos de propiedades concatenadas");
            }

            return Ok(result);
        }





        [HttpGet("{NumeroSerial}")]
        public async Task<ActionResult<EnsambleDto>> GetId(string NumeroSerial)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.NumeroSerial == NumeroSerial);

            if (ensamble == null)
            {
                return BadRequest($"No existe el id: {NumeroSerial}");
            }

            var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensamble.IdElementType);

            if (elementype == null)
            {
                return BadRequest("No tiene ningun tipo de elemento");
            }

            var marca = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == ensamble.IdMarca);
            if (marca == null)
            {
                return BadRequest("No se encontraron marcas");
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
                NombreMarca = marca.Nombre
            };

            return Ok(ensambleDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(EnsambleCreateDto createDto)
        {
            // El DTO verifica la tabla
            var ensamble = _mapper.Map<Ensamble>(createDto);

            // Verificación de ElementType
            var elementoExiste = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == createDto.IdElementType);
            if (elementoExiste == null)
            {
                return BadRequest($"El tipo de elemento con el ID {createDto.IdElementType} no fue encontrado.");
            }

            // Verificación sobre la marca
            var marcaExiste = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == createDto.IdMarca);
            if (marcaExiste == null)
            {
                return BadRequest($"La marca con ID {createDto.IdMarca} no fue encontrada.");
            }

            // asigna la fecha actual al campo FechaRegistroEquipo
            ensamble.FechaRegistroEquipo = DateOnly.FromDateTime(DateTime.Now);

            // añade la entidad al contexto
            _context.inv_ensamble.Add(ensamble);

            // guarda los datos en la base de datos
            await _context.SaveChangesAsync();

            // retorna lo guardar
            return CreatedAtAction(nameof(GetId), new { id = ensamble.Id }, ensamble);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Ensamble>> GetId(int id)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);
            if (ensamble == null)
            {
                return BadRequest($"No sencontraron ensambles con el id {id}");
            }

            return Ok(ensamble);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EnsambleCreateDto updateDto)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);
            if (ensamble == null) 
            {
                return BadRequest($"No se encontro el {id}.");
            }

            //verificacion de ElemenType
            var elementoExiste = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == updateDto.IdElementType);
            if (elementoExiste == null)
            {
                return BadRequest($"El tipo de elemento con el ID {updateDto.IdElementType} no fue encontrado.");
            }

            //verificacion sobre la marca
            var marcaExiste = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == updateDto.IdMarca);
            if (marcaExiste == null)
            {
                return BadRequest($"La marca con el ID {updateDto.IdMarca} no fue encontrado.");
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