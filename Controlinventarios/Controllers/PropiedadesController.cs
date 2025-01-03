﻿using AutoMapper;
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

            if (propiedades == null)
            {
                return BadRequest("No se encontraron propiedades.");
            }

            var propiedadesDtos = new List<PropiedadesDto>();

            foreach (var propiedad in propiedades)
            {
               
                var ensambleName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == propiedad.IdEnsamble);

                if (ensambleName == null)
                {
                    return BadRequest($"No se encontró el ensamble para la propiedad: {ensambleName}");
                }

                var propiedadDto = new PropiedadesDto
                {
                    id = propiedad.id,
                    Propiedad = propiedad.Propiedad,
                    IdEnsamble = propiedad.IdEnsamble,
                    EnsambleName = ensambleName.NumeroSerial
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
                EnsambleName = ensambleName.NumeroSerial
            };

            return Ok(propiedadDto);
        }



        [HttpPost]
        public async Task<ActionResult> Post(PropiedadesCreateDto createDto)
        {
            // el dto verifica la tabla
            var propiedad = _mapper.Map<Propiedades>(createDto);

            // Verificacion de que existe el ensasmble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
            }

            // añade la entidad al contexto
            _context.inv_propiedades.Add(propiedad);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = propiedad.id }, propiedad);
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
    }
}
