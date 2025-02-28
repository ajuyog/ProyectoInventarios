﻿using AutoMapper;
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


        //[HttpGet]
        //public async Task<ActionResult<List<PropiedadesDto>>> Get()
        //{
        //    var propiedad = await _context.inv_propiedades.ToListAsync();
        //    var propiedadDtos = _mapper.Map<List<PropiedadesDto>>(propiedad);

        //    return Ok(propiedadDtos);
        //}

        [HttpGet]
        public async Task<ActionResult<List<PropiedadesDto>>> Get()
        {
            var propiedades = await _context.inv_propiedades.ToListAsync();

            if (!propiedades.Any())
            {
                return BadRequest("No se encontraron propiedades.");
            }

            var propiedadesDtos = new List<PropiedadesDto>();

            foreach (var propiedad in propiedades)
            {
                var ensambleName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == propiedad.IdEnsamble);

                if (ensambleName == null)
                {
                    return BadRequest($"No se encontró el ensamble para la propiedad con ID {propiedad.id}");
                }

                var propiedadDto = new PropiedadesDto
                {
                    id = propiedad.id,
                    Propiedad = propiedad.Propiedad,
                    IdEnsamble = propiedad.IdEnsamble,
                    EnsambleName = ensambleName.NumeroSerial,
                    //Propiedades = propiedad.Propiedadess
                };

                propiedadesDtos.Add(propiedadDto);
            }

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
                EnsambleName = ensambleName.NumeroSerial,
                //Propiedades = propiedad.Propiedadess
            };

            return Ok(propiedadDto);
        }



        //[HttpPost]
        //public async Task<ActionResult> Post2(PropiedadesCreateDto createDto)
        //{
        //    // el dto verifica la tabla
        //    var propiedad = _mapper.Map<Propiedades>(createDto);

        //    // Verificacion de que existe el ensasmble
        //    var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
        //    if (ensambleExiste == null)
        //    {
        //        return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
        //    }

        //    // añade la entidad al contexto
        //    _context.inv_propiedades.Add(propiedad);
        //    // guardar los datos en la basee de datos
        //    await _context.SaveChangesAsync();
        //    //retorna lo guardado
        //    return CreatedAtAction(nameof(GetId), new { id = propiedad.id }, propiedad);
        //}

        [HttpPost]
        public async Task<ActionResult<Propiedades>> Post(PropiedadesCreateDto createDto)
        {
            // Mapeo DTO a entidad
            var propiedad = _mapper.Map<Propiedades>(createDto);

            // validar existencia del ensamble
            var ensambleExiste = await _context.inv_ensamble
                .FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);

            if (ensambleExiste == null)
            {
                return BadRequest($"Ensamble con ID {createDto.IdEnsamble} no existe.");
            }

            // Persistir en base de datos
            _context.inv_propiedades.Add(propiedad);
            await _context.SaveChangesAsync();

            // Retornar resultado
            return CreatedAtAction(nameof(GetId), new { id = propiedad.id }, propiedad);
        }

        [HttpPost("Recibir listado de propiedades")]
        public async Task<ActionResult> RecibirPropiedades(PropiedadesCreateDto createDto)
        {
            // validar que el DTO no sea nulo y que la cadena de propiedades no este vacía
            if (createDto == null || string.IsNullOrEmpty(createDto.Propiedad))
            {
                return BadRequest("Debe ingresar al menos una propiedad.");
            }

            // verificar si el ensamble existe en la base de datos
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(e => e.Id == createDto.IdEnsamble);
                
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
        public async Task<ActionResult> Update(int id, PropiedadesCreateDto updateDto)
        {
            var propiedad = await _context.inv_propiedades.FirstOrDefaultAsync(x => x.id == id);

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == updateDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {updateDto.IdEnsamble} no fue encontrado.");
            }

            propiedad = _mapper.Map(updateDto, propiedad);

            _context.inv_propiedades.Update(propiedad);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { propiedad.id }, propiedad);

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var propiedad = await _context.inv_propiedades.FindAsync(id);

            if (propiedad == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_propiedades.Remove(propiedad);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //metodos de prueba 

        //[HttpPost("Post de prueba")]
        //public async Task<ActionResult<IEnumerable<Propiedades>>> Post2(PropiedadesCreateDto createDto)
        //{
        //    // Validar existencia del ensamble
        //    var ensambleExiste = await _context.inv_ensamble
        //        .FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);

        //    if (ensambleExiste == null)
        //    {
        //        return BadRequest($"Ensamble con ID {createDto.IdEnsamble} no existe.");
        //    }

        //    // Validar que la lista de propiedades no esté vacía
        //    if (createDto.Propiedades == null || !createDto.Propiedades.Any())
        //    {
        //        return BadRequest("Debe proporcionar al menos una propiedad.");
        //    }

        //    // Crear una entidad por cada propiedad en la lista
        //    var propiedades = createDto.Propiedades.Select(tag => new PropiedadesDto
        //    {
        //        Propiedad = tag.Trim(), // Campo donde se guarda cada tag
        //        IdEnsamble = createDto.IdEnsamble,
        //        NumeroSerial = createDto.NumeroSerial
        //    }).ToList();

        //    _context.inv_propiedades.AddRange((IEnumerable<Propiedades>)propiedades);
        //    await _context.SaveChangesAsync();

        //    // Retornar todas las entidades creadas
        //    return CreatedAtAction(nameof(GetId), new { id = propiedades.First().id }, propiedades);
        //}

        //[HttpPut("Put de prueba/{id}")]
        //public async Task<ActionResult> Update2(int id, PropiedadesCreateDto updateDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var propiedad = await _context.inv_propiedades
        //        .FirstOrDefaultAsync(x => x.id == id);

        //    if (propiedad == null)
        //    {
        //        return BadRequest("Propiedad no encontrada.");
        //    }

        //    var ensambleExiste = await _context.inv_ensamble
        //        .FirstOrDefaultAsync(x => x.Id == updateDto.IdEnsamble);

        //    if (ensambleExiste == null)
        //    {
        //        return BadRequest($"El ensamble con ID {updateDto.IdEnsamble} no fue encontrado.");
        //    }

        //    _mapper.Map(updateDto, propiedad);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.inv_propiedades.Any(p => p.id == id))
        //        {
        //            return NotFound("Propiedad no encontrada.");
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

     
    }
}
