using AutoMapper;
using Azure;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Controlinventarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropiedadesController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public PropiedadesController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PropiedadesDto>>> Get()
        {
            // obtiene todas las propiedades de la base de datos y las convierte en una lista.
            var propiedades = await _context.inv_propiedades.ToListAsync();

            // verifica si la lista de propiedades está vacía.
            if (!propiedades.Any())
            {
                // si no hay propiedades, devuelve un error 400 (Bad Request) con un mensaje.
                return BadRequest("No se encontraron propiedades.");
            }

            // crea una lista vacía para almacenar los DTOs de las propiedades.
            var propiedadesDtos = new List<PropiedadesDto>();

            // itera sobre cada propiedad en la lista de propiedades.
            foreach (var propiedad in propiedades)
            {
                // busca el ensamble asociado a la propiedad en la base de datos usando el IdEnsamble.
                var ensambleName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == propiedad.IdEnsamble);

                // verifica si el ensamble existe.
                if (ensambleName == null)
                {
                    // si no se encuentra el ensamble, devuelve un error 400 (Bad Request) con un mensaje.
                    return BadRequest($"No se encontró el ensamble para la propiedad con ID {propiedad.id}");
                }

                // crea un DTO para la propiedad actual y asigna los valores correspondientes.
                var propiedadDto = new PropiedadesDto
                {
                    id = propiedad.id, // asigna el ID de la propiedad.
                    Propiedad = propiedad.Propiedad, // asigna el nombre de la propiedad.
                    IdEnsamble = propiedad.IdEnsamble, // asigna el ID del ensamble asociado.
                    EnsambleName = ensambleName.NumeroSerial, // asigna el número de serie del ensamble.
                };

                // agrega el DTO de la propiedad a la lista de DTOs.
                propiedadesDtos.Add(propiedadDto);
            }

            // devuelve una respuesta 200 (OK) con la lista de DTOs de las propiedades.
            return Ok(propiedadesDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropiedadesDto>> GetId(int id)
        {
            // Buscar la propiedad con el id dado
            var propiedad = await _context.inv_propiedades.FirstOrDefaultAsync(x => x.id == id);

            if (propiedad == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            // Buscar el ensamble correspondiente
            var ensambleName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == propiedad.IdEnsamble);

            if (ensambleName == null)
            {
                return BadRequest($"No se encontró el ensamble para la propiedad: {id}");
            }

            // Crear el DTO de la propiedad
            var propiedadDto = new PropiedadesDto
            {
                id = propiedad.id,
                Propiedad = propiedad.Propiedad,
                IdEnsamble = propiedad.IdEnsamble,
                EnsambleName = ensambleName.NumeroSerial
            };

            return Ok(propiedadDto);
        }

        //[HttpPost]
        //public async Task<ActionResult<Propiedades>> Post(PropiedadesCreateDto createDto)
        //{
        //    // Mapeo DTO a entidad
        //    var propiedad = _mapper.Map<Propiedades>(createDto);

        //    // validar existencia del ensamble
        //    var ensambleExiste = await _context.inv_ensamble
        //        .FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);

        //    if (ensambleExiste == null)
        //    {
        //        return BadRequest($"Ensamble con ID {createDto.IdEnsamble} no existe.");
        //    }

        //    // Persistir en base de datos
        //    _context.inv_propiedades.Add(propiedad);
        //    await _context.SaveChangesAsync();

        //    // Retornar resultado
        //    return CreatedAtAction(nameof(GetId), new { id = propiedad.id }, propiedad);
        //}

        [HttpPost("Recibir listado de propiedades")]
        public async Task<ActionResult> RecibirPropiedades(PropiedadesCreateDto createDto)
        {
            // validar que el DTO no sea nulo y que la cadena de propiedades no este vacía
            if (createDto == null || string.IsNullOrEmpty(createDto.Propiedad))
            {
                return BadRequest("Debe ingresar al menos una propiedad.");
            }

            // verificar si el ensamble existe en la base de datos
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);

            // si el ensamble no existe, devolver un error
            if (ensamble == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no existe.");
            }

            // dividir la cadena de propiedades en un arreglo usando la coma como separador
            var propiedadesArray = createDto.Propiedad
                .Split(',') // divide la cadena por comas
                .Select(p => p.Trim()) // elimina espacios en blanco de cada propiedad
                .Where(p => !string.IsNullOrEmpty(p)) // filtra propiedades vacías
                .ToList();

            // crear una lista para almacenar las propiedades que se van a crear
            var propiedadesCreadas = new List<Propiedades>();

            // iterar sobre cada propiedad en el arreglo
            foreach (var prop in propiedadesArray)
            {
                // crear un nuevo objeto Propiedades para cada propiedad
                var propiedad = new Propiedades
                {
                    Propiedad = prop, // asignar el valor de la propiedad (ya se aplicó Trim)
                    IdEnsamble = ensamble.Id // asignar el ID del ensamble existente
                };

                // agregar la propiedad creada a la lista de propiedadesCreadas
                propiedadesCreadas.Add(propiedad);
            }

            // agregar todas las propiedades creadas al contexto de la base de datos
            _context.inv_propiedades.AddRange(propiedadesCreadas);

            // guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // retornar una respuesta exitosa con un mensaje y los datos creados
            return Ok(new
            {
                Mensaje = "Propiedades registradas con éxito", // mensaje de éxito
                EnsambleId = ensamble.Id, // ID del ensamble al que se asociaron las propiedades
                PropiedadesCreadas = propiedadesCreadas.Select(p => p.Propiedad).ToList() // lista de propiedades creadas
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, PropiedadesUpdateDto updateDto)
        {
            // busca la propiedad en la base de datos usando el ID proporcionado en la ruta.
            var propiedad = await _context.inv_propiedades.FirstOrDefaultAsync(x => x.id == id);

            // verifica si el ensamble asociado a la propiedad existe en la base de datos.
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == updateDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                // si el ensamble no existe, devuelve un error 400 (Bad Request) con un mensaje.
                return BadRequest($"El ensamble con ID {updateDto.IdEnsamble} no fue encontrado.");
            }

            // mapea los datos del DTO (updateDto) a la entidad existente (propiedad).
            // esto actualiza los campos de la entidad con los valores proporcionados en el DTO.
            propiedad = _mapper.Map(updateDto, propiedad);

            // marca la entidad como modificada en el contexto de la base de datos.
            _context.inv_propiedades.Update(propiedad);

            // guarda los cambios en la base de datos de manera asíncrona.
            await _context.SaveChangesAsync();

            // devuelve una respuesta 201 (Created) con la ubicación del recurso actualizado.
            // createdAtAction redirige a la acción "GetId" para obtener los detalles de la propiedad actualizada.
            return CreatedAtAction(nameof(GetId), new { propiedad.id }, propiedad);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // busca la propiedad en la base de datos usando el ID proporcionado en la ruta.
            var propiedad = await _context.inv_propiedades.FirstOrDefaultAsync(x => x.id == id);

            // verifica si la propiedad existe.
            if (propiedad == null)
            {
                // si la propiedad no existe, devuelve un error 400 (Bad Request) con un mensaje.
                return BadRequest($"No existe el id: {id}");
            }

            // marca la propiedad para ser eliminada del contexto de la base de datos.
            _context.inv_propiedades.Remove(propiedad);

            // guarda los cambios en la base de datos de manera asíncrona.
            await _context.SaveChangesAsync();

            // devuelve una respuesta 200 (OK) indicando que la operación se completó con éxito.
            return Ok();
        }


    }
}
