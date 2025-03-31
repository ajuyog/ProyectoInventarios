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


        [HttpGet("Linqcombinado")]
        public async Task<ActionResult<List<CentroDeCostoDto>>> GetLinqCombinado()
        {
            // Consulta corregida para linqValorEquipo
            var linqValorEquipo = from ie in _context.inv_ensamble
                                  join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                                  join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
                                  join ip in _context.inv_persona on ia.IdPersona equals ip.userId
                                  join ia2 in _context.inv_area on ip.IdArea equals ia2.id
                                  join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
                                  join if2 in _context.inv_facturaciontmk on ie.Id equals if2.IdEnsamble
                                  join im in _context.inv_marca on ie.IdMarca equals im.id
                                  where ie.Renting == true && ie.Estado == true && (ie2.IdElementType == 0 || ie2.IdElementType == 4)
                                  group new { ie2, im, ie, ia2, ie3, if2 } by new
                                  {
                                      ie.Id,
                                      ie.NumeroSerial,
                                      ie.Renting,
                                      ia2.Nombre,
                                      ip.idEmpresa,
                                      if2.VlrNeto,
                                      if2.Descripcion,
                                  } into g
                                  orderby g.Key.Id ascending // Ordenar por Id ascendente aquí
                                  select new
                                  {
                                      g.Key.Id,
                                      g.Key.NumeroSerial,
                                      g.Key.Renting,
                                      Area = g.Key.Nombre,
                                      Empresa = g.Key.idEmpresa,
                                      VlrNeto = g.Key.VlrNeto,
                                      Factura = g.Key.Descripcion,
                                  };
            if (linqValorEquipo == null)
            {
                return BadRequest("No se encontraron valores por equipo");
            }

            // consulta para obtener los totales de equipos por area
            var ValorPorArea = from ensamble in _context.inv_ensamble
                               join elementType in _context.inv_elementType on ensamble.IdElementType equals elementType.id
                               join asignacion in _context.inv_asignacion on ensamble.Id equals asignacion.IdEnsamble
                               join persona in _context.inv_persona on asignacion.IdPersona equals persona.userId
                               join area in _context.inv_area on persona.IdArea equals area.id
                               join empresa in _context.inv_empresa on persona.idEmpresa equals empresa.id
                               join factura in _context.inv_facturaciontmk on ensamble.Id equals factura.IdEnsamble
                               where ensamble.Renting == true && ensamble.Estado == true && (elementType.IdElementType == 0 || elementType.IdElementType == 4)
                               group new { factura.VlrNeto, area.Nombre, factura.Descripcion, factura.Fecha }
                               by new { area.id, factura.Descripcion, factura.Fecha } into g
                               select new CentroDeCostoDto2
                               {
                                   AreaVlrNeto = g.Sum(x => x.VlrNeto), // suma de VlrNeto por area
                                   totalEquipos = g.Count(), // cuenta de equipos por area
                                   NombreArea = g.FirstOrDefault().Nombre, // nombre del area
                                   Factura = g.FirstOrDefault().Descripcion
                               };

            // sumar todos los VlrNeto y equipos de todas las areas
            var totalPorTodasLasAreas = await ValorPorArea
                                        .GroupBy(x => 1) // agrupa todo en un solo grupo
                                        .Select(s => new
                                        {
                                            TotalVlrNeto = s.Sum(x => x.AreaVlrNeto), // suma de todos los VlrNeto
                                            TotalEquipos = s.Sum(x => x.totalEquipos), // suma de todos los equipos
                                        })
                                        .FirstOrDefaultAsync();

            // si no se encuentra ningun resultado
            if (totalPorTodasLasAreas == null)
            {
                return BadRequest("No se encontraron resultados de totales por area");
            }

            // crear el dto para el total general de areas
            var resultado_Total_General = new List<CentroDeCostoDto3>
            {
                new CentroDeCostoDto3
                {
                    TotalVlrNeto = totalPorTodasLasAreas.TotalVlrNeto,
                    totalEquipos = totalPorTodasLasAreas.TotalEquipos,
                    TotalIva = totalPorTodasLasAreas.TotalVlrNeto * 19 / 100,
                    Retencion = totalPorTodasLasAreas.TotalVlrNeto * 40 / 1000,
                    Factura = "Total General", // descripcion general
                    Total_A_Pagar = totalPorTodasLasAreas.TotalVlrNeto + (totalPorTodasLasAreas.TotalVlrNeto * 19 / 100) - (totalPorTodasLasAreas.TotalVlrNeto * 40 / 1000),
                    Fecha = DateTime.Now
                }
            };

            if (resultado_Total_General == null)
            {
                return BadRequest("No se encontro total general");
            }

            // ejecuta ambas consultas
            var solo_equipos = await linqValorEquipo.ToListAsync();
            var Valor_Total_Area = await ValorPorArea.ToListAsync();

            // comprobamos si hay resultados
            if (solo_equipos == null || Valor_Total_Area == null)
            {
                return BadRequest("No se encontraron resultados");
            }

            // combina los resultados en un solo objeto
            var resultado = new
            {
                EquiposSolos = solo_equipos,
                ValorPorArea = Valor_Total_Area,
                TotalGeneral = resultado_Total_General
            };

            return Ok(resultado);
        }


    }

}
 