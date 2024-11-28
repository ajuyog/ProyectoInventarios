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


        //[HttpGet]
        //public async Task<ActionResult<List<FacturacionTMKDto>>> Get()
        //{
        //    var facturacion = await _context.inv_facturaciontmk.ToListAsync();
        //    var facturacionDtos = _mapper.Map<List<FacturacionTMKDto>>(facturacion);

        //    return Ok(facturacionDtos);
        //}

        [HttpGet]
        public async Task<ActionResult<List<FacturacionTMKDto>>> Get()
        {
            var facturacion = await _context.inv_facturaciontmk.ToListAsync();

            if (facturacion == null)
            {
                return BadRequest("No se encontro la factura.");
            }

            var facturacionDtos = new List<FacturacionTMKDto>();

            foreach (var factura in facturacion)
            {

                var facturaName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == factura.IdEnsamble);

                if (facturaName == null)
                {
                    return BadRequest($"No se encontró el ensamble para la propiedad: {facturaName}");
                }

                var facturaDto = new FacturacionTMKDto
                {
                    Id = factura.Id,
                    VlrNeto = factura.VlrNeto,
                    IdEnsamble = factura.IdEnsamble,
                    Descripcion = factura.Descripcion,
                    Fecha = factura.Fecha,
                    EnsambleName = facturaName.NumeroSerial
                };

                facturacionDtos.Add(facturaDto);
            }

            return Ok(facturacionDtos);
        }

        [HttpGet("Objetos renting true")]
        public async Task<ActionResult<List<EnsambleDto>>> GetTrue()
        {
            var facturacion = await _context.inv_ensamble.ToListAsync();

            if (facturacion == null)
            {
                return BadRequest("No se encontraron ensambles");
            }

            var facturacionDtosTrue = new List<EnsambleDto>();

            foreach (var factura in facturacion)
            {
                if (factura.Renting == true)
                {
                    var facturaName = await _context.inv_facturaciontmk.FirstOrDefaultAsync(x => x.Id == factura.IdElementType);
                    if (facturaName == null)
                    {
                        return BadRequest($"No se encontró la factura para el ensamble");
                    }

                    var facturaDto = new EnsambleDto
                    {
                        Id = factura.Id,
                        IdElementType = factura.IdElementType,
                        IdMarca = factura.IdMarca,
                        NumeroSerial = factura.NumeroSerial,
                        Estado = factura.Estado,
                        Descripcion = factura.Descripcion,
                        Renting = factura.Renting,
                        NumeroFactura = facturaName.Descripcion
                    };

                    facturacionDtosTrue.Add(facturaDto);
                }
            }

            return Ok(facturacionDtosTrue);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<FacturacionTMKDto>> GetId(int id)
        {

            var facturacion = await _context.inv_facturaciontmk.FirstOrDefaultAsync(x => x.Id == id);
            if (facturacion == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            var facturaName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == facturacion.IdEnsamble);
            if (facturaName == null)
            {
                return BadRequest($"No se encontró el ensamble para el id de facturación: {id}");
            }

            var facturacionDto = new FacturacionTMKDto
            {
                Id = facturacion.Id,
                VlrNeto = facturacion.VlrNeto,
                IdEnsamble = facturacion.IdEnsamble,
                Descripcion = facturacion.Descripcion,
                Fecha = facturacion.Fecha,
                EnsambleName = facturaName.NumeroSerial
            };

            return Ok(facturacionDto);
        }


        [HttpPost]
        public async Task<ActionResult> Post(FacturacionTMKCreateDto createDto)
        {
            // el dto verifica la tabla
            var facturacion = _mapper.Map<FacturacionTMK>(createDto);

            //Verificacion del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
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
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == updateDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {updateDto.IdEnsamble} no fue encontrado.");
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
