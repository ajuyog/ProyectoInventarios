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

        //[HttpGet]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> Get()
        //{
        //    var factura = await _context.inv_facturaciontmk.ToListAsync();

        //    if (factura == null)
        //    {
        //        return BadRequest();
        //    }

        //    var facturaDtos = new List<CentroDeCostoDto>();

        //    foreach (var facturas in factura)
        //    {
        //        var numeroSerial = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == facturas.IdEnsamble);
        //        if (numeroSerial == null)
        //        {
        //            return BadRequest("No se encontraron ensambles");
        //        }

        //        var nombreArea = await _context.inv_area.FirstOrDefaultAsync();
        //        if (nombreArea == null) 
        //        {
        //            return BadRequest("No se encontraron areas");
        //        }

        //        var facturaDto = new CentroDeCostoDto
        //        {
        //            Descripcion = facturas.Descripcion,
        //            VlrNeto = facturas.VlrNeto,
        //            //IdEnsamble = facturas.IdEnsamble,
        //            //IdArea = nombreArea.id,
        //            NumeroSerial = numeroSerial.NumeroSerial,
        //            NombreArea = nombreArea.Nombre
        //        };
        //        facturaDtos.Add(facturaDto);
        //    }
        //    return Ok(facturaDtos);
        //}

        //[HttpGet("linq4")]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> Get5()
        //{
        //    var resultado = from ie in _context.inv_ensamble
        //                    join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
        //                    join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
        //                    join ip in _context.inv_persona on ia.IdPersona equals ip.id
        //                    join ia2 in _context.inv_area on ip.IdArea equals ia2.id
        //                    join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
        //                    join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
        //                    join im in _context.inv_marca on ie.IdMarca equals im.id
        //                    where ie.Renting == true
        //                    group new { ie2, im, ie, ia2, ie3, if2 } by new
        //                    {
        //                        ia2.id,
        //                        ie.IdMarca,
        //                        ie.NumeroSerial,
        //                        ie.Renting,
        //                        ia2.Nombre,
        //                        ip.idEmpresa,
        //                        if2.VlrNeto,
        //                        if2.Descripcion,
        //                    } into g
        //                    select new
        //                    {
        //                        g.Key.id,                            // Nombre del tipo de elemento
        //                        Marca = g.Key.IdMarca,              // Marca
        //                        g.Key.NumeroSerial,                // Número de serie
        //                        g.Key.Renting,                    // Renting
        //                        Area = g.Key.Nombre,             // Nombre del área
        //                        Empresa = g.Key.idEmpresa,      // Nombre de la empresa
        //                        VlrNeto = g.Key.VlrNeto,       // Valor neto
        //                        Iva19 = g.Key.VlrNeto * 0.19, // Cálculo del IVA (19%)
        //                        Factura = g.Key.Descripcion  // Descripción de la factura
        //                    };

        //    var listaResultados = await resultado.ToListAsync();

        //    if (listaResultados == null)
        //    {
        //        return BadRequest("No se encontraron resultados.");
        //    }

        //    return Ok(listaResultados);
        //}



        //[HttpGet("linq de total de equipos")]
        //public async Task<ActionResult<List<CentroDeCostoDto2>>> Get6()
        //{
        //    // Realizar la consulta LINQ
        //    var result = await (from ensamble in _context.inv_ensamble
        //                        join elementType in _context.inv_elementType on ensamble.IdElementType equals elementType.id
        //                        join asignacion in _context.inv_asignacion on ensamble.Id equals asignacion.IdEnsamble
        //                        join persona in _context.inv_persona on asignacion.IdPersona equals persona.id
        //                        join area in _context.inv_area on persona.IdArea equals area.id
        //                        join empresa in _context.inv_empresa on persona.idEmpresa equals empresa.id
        //                        join factura in _context.inv_facturaciontmk on ensamble.Id equals factura.IdEnsamble
        //                        where ensamble.Renting == true
        //                        group new { factura.VlrNeto, area.Nombre, factura.Descripcion, factura.Fecha }
        //                        by new { area.id, factura.Descripcion, factura.Fecha } into g
        //                        select new CentroDeCostoDto2
        //                        {
        //                            VlrNeto = g.Sum(x => x.VlrNeto), // Suma de VlrNeto
        //                            totalEquipos = g.Count(), // Cuenta de equipos
        //                            NombreArea = g.FirstOrDefault().Nombre, // Nombre del área
        //                            Factura = g.FirstOrDefault().Descripcion, // Descripción de la factura
        //                            //Fecha = g.FirstOrDefault().Fecha // Fecha de la factura
        //                        }).ToListAsync();

        //    if (result == null)
        //    {
        //        return BadRequest("No se encontraron resultados.");
        //    }

        //    return Ok(result);
        //}


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
                                      Iva19 = g.Key.VlrNeto * 19 / 100, ////// calculo del iva
                                      Factura = g.Key.Descripcion,     ////// descripcion de la factura
                                      VlrBrutoEquipo = g.Key.VlrNeto + (g.Key.VlrNeto * 19 / 100)     ///// Valor con iva
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
                                   AreaVlrNeto = g.Sum(x => x.VlrNeto), // suma de VlrNeto por area
                                   totalEquipos = g.Count(), // cuenta de equipos por area
                                   NombreArea = g.FirstOrDefault().Nombre, // nombre del area
                                   Factura = g.FirstOrDefault().Descripcion, // descripcion de la factura
                                   VlrBrutoArea = g.Sum(x => x.VlrNeto) + (g.Sum(x => x.VlrNeto) * 19 / 100)
                               };

            // sumar todos los VlrNeto y equipos de todas las áreas
            var totalPorTodasLasAreas = await ValorPorArea
                                        .GroupBy(x => 1) // agrupa todo en un solo grupo
                                        .Select(s => new
                                        {
                                            TotalVlrNeto = s.Sum(x => x.VlrBrutoArea), // suma de todos los VlrNeto
                                            TotalEquipos = s.Sum(x => x.totalEquipos), // suma de todos los equipos
                                        })
                                        .FirstOrDefaultAsync();

            // si no se encuentra ningun resultado
            if (totalPorTodasLasAreas == null)
            {
                return BadRequest("No se encontraron resultados de totales");
            }

            // crear el dto para el total general de areas
            var resultado_Total_General = new List<CentroDeCostoDto3>
            {
                new CentroDeCostoDto3
                {
                    TotalVlrNeto = totalPorTodasLasAreas.TotalVlrNeto,
                    totalEquipos = totalPorTodasLasAreas.TotalEquipos,
                    NombreArea = "Total General", // el nombre sería algo indicativo de "Total"
                    Factura = "Total General" // descripcion general
                }
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

        //[HttpGet("linq combinado 2")]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> GetLinqCombinado2()
        //{
        //    var linqCombinado = from ie in _context.inv_ensamble
        //                        join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
        //                        join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
        //                        join ip in _context.inv_persona on ia.IdPersona equals ip.id
        //                        join ia2 in _context.inv_area on ip.IdArea equals ia2.id
        //                        join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
        //                        join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
        //                        join im in _context.inv_marca on ie.IdMarca equals im.id
        //                        where ie.Renting == true
        //                        group new { ie2, im, ie, ia2, ie3, if2, ip } by new
        //                        {
        //                            ia2.id,
        //                            ie.IdMarca,
        //                            ie.NumeroSerial,
        //                            ie.Renting,
        //                            ia2.Nombre,
        //                            ip.idEmpresa,
        //                            if2.VlrNeto,
        //                            if2.Descripcion,
        //                            if2.Fecha
        //                        } into g
        //                        select new
        //                        {
        //                            g.Key.id,                            // nombre del tipo de elemento
        //                            Marca = g.Key.IdMarca,               // marca
        //                            g.Key.NumeroSerial,                  // numero de serie
        //                            g.Key.Renting,                       // renting
        //                            Area = g.Key.Nombre,                 // nombre del área
        //                            Empresa = g.Key.idEmpresa,           // nombre de la empresa
        //                            VlrNeto = g.Key.VlrNeto,             // valor neto
        //                            Iva19 = g.Key.VlrNeto * 0.19,        // cálculo del IVA
        //                            Factura = g.Key.Descripcion,         // descripción de la factura
        //                            FechaFactura = g.Key.Fecha,          // fecha de la factura
        //                            TotalEquiposPorArea = g.Count(),     // cantidad de equipos por área
        //                            TotalVlrNetoPorArea = g.Sum(x => x.if2.VlrNeto) // suma total de VlrNeto por área
        //                        };

        //    // Ejecutar la consulta combinada
        //    var resultado = await linqCombinado.ToListAsync();

        //    // Si no hay resultados, devolver error
        //    if (resultado == null || !resultado.Any())
        //    {
        //        return BadRequest("No se encontraron resultados");
        //    }

        //    // Agrupar los resultados por área para el total general
        //    var totalGeneral = resultado
        //                        .GroupBy(x => 1) // Agrupar todos los resultados
        //                        .Select(g => new CentroDeCostoDto2
        //                        {
        //                            VlrNeto = g.Sum(x => x.TotalVlrNetoPorArea),
        //                            totalEquipos = g.Sum(x => x.TotalEquiposPorArea),
        //                            NombreArea = "Total General", // Nombre del total general
        //                            Factura = "Total General" // Descripción general
        //                        })
        //                        .FirstOrDefault();

        //    // Si no se encuentra el total general, devolver error
        //    if (totalGeneral == null)
        //    {
        //        return BadRequest("No se encontraron resultados de totales");
        //    }

        //    // Combinar los resultados en un solo objeto
        //    var resultadoFinal = new
        //    {
        //        EquiposSolos = resultado, // Detalles de cada equipo
        //        TotalGeneral = totalGeneral // Total general de todas las áreas
        //    };

        //    return Ok(resultadoFinal);
        //}

        //[HttpGet("linq combinado 3")]
        //public async Task<ActionResult<List<CentroDeCostoDto>>> GetLinqCombinado3()
        //{
        //    // Obtener detalles de equipos por área
        //    var linqValorEquipoPorArea = from ie in _context.inv_ensamble
        //                                 join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
        //                                 join ia in _context.inv_asignacion on ie.Id equals ia.IdEnsamble
        //                                 join ip in _context.inv_persona on ia.IdPersona equals ip.id
        //                                 join ia2 in _context.inv_area on ip.IdArea equals ia2.id
        //                                 join ie3 in _context.inv_empresa on ip.idEmpresa equals ie3.id
        //                                 join if2 in _context.inv_facturaciontmk on ie.Id equals if2.Id
        //                                 join im in _context.inv_marca on ie.IdMarca equals im.id
        //                                 where ie.Renting == true
        //                                 group new { ie2, im, ie, ia2, ie3, if2, ip } by new
        //                                 {
        //                                     ia2.id,
        //                                     ia2.Nombre,
        //                                     ie.IdMarca,
        //                                     ie.NumeroSerial,
        //                                     ie.Renting,
        //                                     ip.idEmpresa,
        //                                     if2.VlrNeto,
        //                                     if2.Descripcion,
        //                                     if2.Fecha
        //                                 } into g
        //                                 select new
        //                                 {
        //                                     g.Key.id,                            // ID del área
        //                                     Area = g.Key.Nombre,                 // Nombre del área
        //                                     Marca = g.Key.IdMarca,               // Marca
        //                                     g.Key.NumeroSerial,                  // Número de serie
        //                                     g.Key.Renting,                       // Renting
        //                                     Empresa = g.Key.idEmpresa,           // Empresa
        //                                     VlrNeto = g.Key.VlrNeto,             // Valor neto
        //                                     Iva19 = g.Key.VlrNeto * 0.19,        // IVA
        //                                     Factura = g.Key.Descripcion,         // Descripción de la factura
        //                                     FechaFactura = g.Key.Fecha,          // Fecha de la factura
        //                                 };

        //    // Ejecutar la consulta para equipos por área
        //    var resultadoPorArea = await linqValorEquipoPorArea.ToListAsync();

        //    // Si no hay resultados, devolver error
        //    if (resultadoPorArea == null || !resultadoPorArea.Any())
        //    {
        //        return BadRequest("No se encontraron resultados");
        //    }

        //    // Obtener los totales por área (suma de VlrNeto y equipos por área)
        //    var totalesPorArea = resultadoPorArea
        //                         .GroupBy(x => new { x.id, x.Area }) // Agrupar por área
        //                         .Select(g => new CentroDeCostoDto2
        //                         {
        //                             NombreArea = g.Key.Area,
        //                             VlrNeto = g.Sum(x => x.VlrNeto),     // Sumar VlrNeto por área
        //                             totalEquipos = g.Count(),             // Contar equipos por área
        //                             Factura = g.FirstOrDefault()?.Factura // Usar la primera factura asociada
        //                         })
        //                         .ToList();

        //    // Obtener el total general (todos los equipos y áreas)
        //    var totalGeneral = new CentroDeCostoDto2
        //    {
        //        NombreArea = "Total General",
        //        VlrNeto = resultadoPorArea.Sum(x => x.VlrNeto),
        //        totalEquipos = resultadoPorArea.Count(),
        //        Factura = "Total General"
        //    };

        //    // Resultado final con dos partes: Equipos por área y totales por área
        //    var resultadoFinal = new
        //    {
        //        EquiposPorArea = resultadoPorArea,   // Detalles de los equipos por área
        //        TotalesPorArea = totalesPorArea,     // Totales de VlrNeto y equipos por área
        //        TotalGeneral = totalGeneral          // Total general de todos los equipos y áreas
        //    };

        //    // Retornar solo los datos de equipos por área (sin los totales por área ni total general)
        //    var resultadoEquipos = new
        //    {
        //        EquiposPorArea = resultadoPorArea   // Detalles de los equipos por área
        //    };

        //    // Retornar solo los totales por área y el total general
        //    var resultadoTotales = new
        //    {
        //        TotalesPorArea = totalesPorArea,     // Totales de VlrNeto y equipos por área
        //        TotalGeneral = totalGeneral          // Total general de todos los equipos y áreas
        //    };

        //    // Devolver ambos JSONs separados
        //    return Ok(new { EquiposPorArea = resultadoEquipos, TotalesPorArea = resultadoTotales });
        //}

    }

}
