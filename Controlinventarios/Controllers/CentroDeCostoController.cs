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

                var nombreArea = await _context.inv_area.FirstOrDefaultAsync();
                if (nombreArea == null) 
                {
                    return BadRequest("No se encontraron areas");
                }

                var facturaDto = new CentroDeCostoDto
                {
                    Descripcion = facturas.Descripcion,
                    VlrNeto = facturas.VlrNeto,
                    //IdEnsamble = facturas.IdEnsamble,
                    //IdArea = nombreArea.id,
                    NumeroSerial = numeroSerial.NumeroSerial,
                    NombreArea = nombreArea.Nombre
                };
                facturaDtos.Add(facturaDto);
            }
            return Ok(facturaDtos);
        }

        [HttpGet("linq4")]
        public async Task<ActionResult<List<CentroDeCostoDto>>> Get5()
        {
            var resultado = from ie in _context.inv_ensamble
                            join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                            join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
                            join ip in _context.inv_persona on ia.IdPersona equals ip.id
                            join ia2 in _context.inv_area on ip.IdArea equals ia2.id
                            join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
                            join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
                            join im in _context.inv_marca on ie.IdMarca equals im.id
                            where ie.Renting == true
                            group new { ie2, im, ie, ia2, ie3, if2 } by new
                            {
                                ia2.id,
                                ie.IdMarca,
                                ie.NumeroSerial,
                                ie.Renting,
                                ia2.Nombre,
                                ip.idEmpresa,
                                if2.VlrNeto,
                                if2.Descripcion,
                            } into g
                            select new
                            {
                                g.Key.id,                            // Nombre del tipo de elemento
                                Marca = g.Key.IdMarca,              // Marca
                                g.Key.NumeroSerial,                // Número de serie
                                g.Key.Renting,                    // Renting
                                Area = g.Key.Nombre,             // Nombre del área
                                Empresa = g.Key.idEmpresa,      // Nombre de la empresa
                                VlrNeto = g.Key.VlrNeto,       // Valor neto
                                Iva19 = g.Key.VlrNeto * 0.19, // Cálculo del IVA (19%)
                                Factura = g.Key.Descripcion  // Descripción de la factura
                            };

            var listaResultados = await resultado.ToListAsync();

            if (listaResultados == null)
            {
                return BadRequest("No se encontraron resultados.");
            }

            return Ok(listaResultados);
        }



        [HttpGet("linq de total de equipos")]
        public async Task<ActionResult<List<CentroDeCostoDto2>>> Get6()
        {
            // Realizar la consulta LINQ
            var result = await (from ensamble in _context.inv_ensamble
                                join elementType in _context.inv_elementType on ensamble.IdElementType equals elementType.id
                                join asignacion in _context.inv_asignacion on ensamble.Id equals asignacion.IdEnsamble
                                join persona in _context.inv_persona on asignacion.IdPersona equals persona.id
                                join area in _context.inv_area on persona.IdArea equals area.id
                                join empresa in _context.inv_empresa on persona.idEmpresa equals empresa.id
                                join factura in _context.inv_facturaciontmk on ensamble.Id equals factura.IdEnsamble
                                where ensamble.Renting == true
                                group new { factura.VlrNeto, area.Nombre, factura.Descripcion, factura.Fecha }
                                by new { area.id, factura.Descripcion, factura.Fecha } into g
                                select new CentroDeCostoDto2
                                {
                                    VlrNeto = g.Sum(x => x.VlrNeto), // Suma de VlrNeto
                                    totalEquipos = g.Count(), // Cuenta de equipos
                                    NombreArea = g.FirstOrDefault().Nombre, // Nombre del área
                                    Factura = g.FirstOrDefault().Descripcion, // Descripción de la factura
                                    //Fecha = g.FirstOrDefault().Fecha // Fecha de la factura
                                }).ToListAsync();

            if (result == null)
            {
                return BadRequest("No se encontraron resultados.");
            }

            return Ok(result);
        }


        [HttpGet("linq combinado")]
        public async Task<ActionResult<List<CentroDeCostoDto>>> GetLinqCombinado()
        {
            // consulta para obtener los detalles del ensamble
            var linqValorEquipo = from ie in _context.inv_ensamble
                                  join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
                                  join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
                                  join ip in _context.inv_persona on ia.IdPersona equals ip.id
                                  join ia2 in _context.inv_area on ip.IdArea equals ia2.id
                                  join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
                                  join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
                                  join im in _context.inv_marca on ie.IdMarca equals im.id
                                  where ie.Renting == true
                                  group new { ie2, im, ie, ia2, ie3, if2 } by new
                                  {
                                      ia2.id,
                                      ie.IdMarca,
                                      ie.NumeroSerial,
                                      ie.Renting,
                                      ia2.Nombre,
                                      ip.idEmpresa,
                                      if2.VlrNeto,
                                      if2.Descripcion,
                                  } into g
                                  select new                                   
                                  {                               
                                      g.Key.id,                            ////// nombre del tipo de elemento
                                      Marca = g.Key.IdMarca,              ////// marca
                                      g.Key.NumeroSerial,                ////// numero de serie
                                      g.Key.Renting,                    ////// renting
                                      Area = g.Key.Nombre,             ////// nombre del area
                                      Empresa = g.Key.idEmpresa,      ////// nombre de la empresa
                                      VlrNeto = g.Key.VlrNeto,       ////// valor neto
                                      Iva19 = g.Key.VlrNeto * 0.19, ////// calculo del IVA (19%)
                                      Factura = g.Key.Descripcion  ////// descripcion de la factura
                                  };

            // consulta para obtener los totales de equipos por area
            var ValorPorArea = from ensamble in _context.inv_ensamble
                               join elementType in _context.inv_elementType on ensamble.IdElementType equals elementType.id
                               join asignacion in _context.inv_asignacion on ensamble.Id equals asignacion.IdEnsamble
                               join persona in _context.inv_persona on asignacion.IdPersona equals persona.id
                               join area in _context.inv_area on persona.IdArea equals area.id
                               join empresa in _context.inv_empresa on persona.idEmpresa equals empresa.id
                               join factura in _context.inv_facturaciontmk on ensamble.Id equals factura.IdEnsamble
                               where ensamble.Renting == true
                               group new { factura.VlrNeto, area.Nombre, factura.Descripcion, factura.Fecha }
                               by new { area.id, factura.Descripcion, factura.Fecha } into g
                               select new CentroDeCostoDto2
                               {
                                   VlrNeto = g.Sum(x => x.VlrNeto), // suma de VlrNeto por area
                                   totalEquipos = g.Count(), // cuenta de equipos por área
                                   NombreArea = g.FirstOrDefault().Nombre, // nombre del area
                                   Factura = g.FirstOrDefault().Descripcion, // descripción de la factura
                               };

            // sumar todos los VlrNeto y equipos de todas las áreas
            var totalPorTodasLasAreas = await ValorPorArea
                                        .GroupBy(x => 1) // agrupa todo en un solo grupo
                                        .Select(s => new
                                        {
                                            TotalVlrNeto = s.Sum(x => x.VlrNeto), // suma de todos los VlrNeto
                                            TotalEquipos = s.Sum(x => x.totalEquipos), // suma de todos los equipos
                                        })
                                        .FirstOrDefaultAsync();

            // si no se encuentra ningun resultado
            if (totalPorTodasLasAreas == null)
            {
                return BadRequest("No se encontraron resultados de totales");
            }

            // crear el dto para el total general de areas
            var resultado_Total_General = new CentroDeCostoDto2
            {
                VlrNeto = totalPorTodasLasAreas.TotalVlrNeto,
                totalEquipos = totalPorTodasLasAreas.TotalEquipos,
                NombreArea = "Total General", // el nombre sería algo indicativo de "Total"
                Factura = "Total General" //descripcion general
            };

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



        //[HttpGet("linq2")]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> Get3()
        //{
        //    // Realizar la consulta LINQ
        //    var result = await (from ie in _context.inv_ensamble
        //                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
        //                        join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
        //                        join ip in _context.inv_persona on ia.IdPersona equals ip.id
        //                        join ia2 in _context.inv_area on ip.IdArea equals ia2.id
        //                        join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
        //                        join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
        //                        where ie.Renting == true  // Filtrar por Renting true
        //                        group new { ie, ie2.Nombre, ia2.Nombre, ie3.Nombre, if2.VlrNeto }
        //                            by new { ia2.id, ie2.id, ie.NumeroSerial, ie.Renting, ia2.Nombre, ie3.Nombre, if2.VlrNeto } into g
        //                        select new
        //                        {
        //                            Elemento = g.FirstOrDefault().Nombre,
        //                            NumeroSerial = g.FirstOrDefault().ie.NumeroSerial,
        //                            Renting = g.FirstOrDefault().ie.Renting,
        //                            Area = g.FirstOrDefault().Nombre,
        //                            Empresa = g.FirstOrDefault().ie3.Nombre,
        //                            VlrNeto = g.Sum(x => x.VlrNeto)  // Sumamos VlrNeto
        //                        }).ToListAsync();

        //    // Si no hay resultados, retornar un BadRequest
        //    if (result == null)
        //    {
        //        return BadRequest("No se encontraron resultados.");
        //    }

        //    // Devolver los resultados exitosos
        //    return Ok(result);
        //}


    }

}
