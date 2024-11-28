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
    public class FacturacionConfivalController : ControllerBase
    {
        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public FacturacionConfivalController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Objetos renting false")]
        public async Task<ActionResult<List<EnsambleDto>>> Get()
        {
            var facturacion = await _context.inv_ensamble.ToListAsync();

            if (facturacion == null)
            {
                return BadRequest("No se encontraron ensambles");
            }

            var facturacionDtosFalse = new List<EnsambleDto>();

            foreach (var factura in facturacion)
            {
                if (factura.Renting == false)
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

                    facturacionDtosFalse.Add(facturaDto);
                }
            }

            return Ok(facturacionDtosFalse);
        }


    }
}
