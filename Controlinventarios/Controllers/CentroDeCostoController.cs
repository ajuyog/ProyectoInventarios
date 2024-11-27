using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controlinventarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CentroDeCostoController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public CentroDeCostoController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CentroDeCostoDto>>> Get()
        {
            var factura = await _context.inv_facturaciontmk.ToListAsync();

            if (factura == null)
            {
                return BadRequest();
            }

            var facturaDtos = new List<CentroDeCostoDto>();

            foreach (var facturas in factura)
            {
                var numeroSerial = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == facturas.IdEnsamble);
                if (numeroSerial == null)
                {
                    return BadRequest("No se encontraron ensambles");
                }

                var nombreArea = await _context.inv_area.FirstOrDefaultAsync(x => x.id == facturas.Id);
                if (nombreArea == null) 
                {
                    return BadRequest("No se encontraron areas");
                }

                var facturaDto = new CentroDeCostoDto
                {
                    Descripcion = facturas.Descripcion,
                    VlrNeto = facturas.VlrNeto,
                    IdEnsamble = facturas.IdEnsamble,
                    NumeroSerial = numeroSerial.NumeroSerial,
                    NombreArea = nombreArea.Nombre
                };
                facturaDtos.Add(facturaDto);
            }
            return Ok(facturaDtos);
        }

        //[HttpGet("Factura")]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> Get()
        //{
        //    var factura = await _context.inv_facturaciontmk.ToListAsync();
        //    if (factura == null)
        //    {
        //        return BadRequest("No se encontraron facturas");
        //    }

        //    var facturaDtos = _mapper.Map<List<CentroDeCostoDto>>(factura);

        //    return Ok(facturaDtos);
        //}

        //[HttpGet("FacturacionTMK")]
        //public async Task<ActionResult<List<FacturacionTMKDto>>> Get1()
        //{
        //    var facturacion = await _context.inv_facturaciontmk.ToListAsync();

        //    if (facturacion == null)
        //    {
        //        return BadRequest("No se encontro la factura.");
        //    }

        //    var facturacionDtos = new List<FacturacionTMKDto>();

        //    foreach (var factura in facturacion)
        //    {

        //        var facturaName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == factura.IdEnsamble);

        //        if (facturaName == null)
        //        {
        //            return BadRequest($"No se encontró el ensamble para la propiedad: {facturaName}");
        //        }

        //        var facturaDto = new FacturacionTMKDto
        //        {
        //            Id = factura.Id,
        //            VlrNeto = factura.VlrNeto,
        //            IdEnsamble = factura.IdEnsamble,
        //            Descripcion = factura.Descripcion,
        //            Fecha = factura.Fecha,
        //            EnsambleName = facturaName.NumeroSerial
        //        };

        //        facturacionDtos.Add(facturaDto);
        //    }

        //    return Ok(facturacionDtos);
        //}

        //[HttpGet("Ensamble")]
        //public async Task<ActionResult<List<EnsambleDto>>> Get2()
        //{
        //    var ensamble = await _context.inv_ensamble.ToListAsync();

        //    if (ensamble == null)
        //    {
        //        return BadRequest("No existe ningun ensamble");
        //    }

        //    var EnsambleDtos = new List<EnsambleDto>();

        //    foreach (var ensambles in ensamble)
        //    {
        //        var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensambles.Id);
        //        if (elementype == null)
        //        {
        //            return BadRequest("No tiene ningun tipo de elemento");
        //        }

        //        var ensambleDto = new EnsambleDto
        //        {
        //            Id = ensambles.Id,
        //            IdElementType = ensambles.IdElementType,
        //            IdMarca = ensambles.IdMarca,
        //            NumeroSerial = ensambles.NumeroSerial,
        //            Estado = ensambles.Estado,
        //            Descripcion = ensambles.Descripcion,
        //            Renting = ensambles.Renting,
        //            TipoElemento = elementype.Nombre,
        //        };

        //        EnsambleDtos.Add(ensambleDto);
        //    }

        //    return Ok(EnsambleDtos);
        //}
        //[HttpGet]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> Get()
        //{
        //    var facturacion = await _context.inv_facturaciontmk.ToListAsync();

        //    if (facturacion == null)
        //    {
        //        return BadRequest("No se encontro la factura.");
        //    }

        //    var ensamble = await _context.inv_ensamble.ToListAsync();
        //    if (ensamble == null)
        //    {
        //        return BadRequest("No encontraron ensambles");
        //    }

        //    var facturacionDtos = new List<CentroDeCostoDto>();

        //    foreach (var factura in facturacion)
        //    {

        //        var facturaName = await _context.inv_ensamble.FirstOrDefaultAsync(o => o.Id == factura.IdEnsamble);

        //        if (facturaName == null)
        //        {
        //            return BadRequest($"No se encontró el ensamble para la propiedad: {facturaName}");
        //        }

        //        var facturaDto = new CentroDeCostoDto
        //        {
        //            Id = factura.Id,
        //            VlrNeto = factura.VlrNeto,
        //            IdEnsamble = factura.IdEnsamble,
        //            Descripcion = factura.Descripcion,
        //            Fecha = factura.Fecha,
        //            EnsambleName = facturaName.NumeroSerial
        //        };

        //        facturacionDtos.Add(facturaDto);
        //    }

        //    return Ok(facturacionDtos);
        //}

    }

}
