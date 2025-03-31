using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Polygonize;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Threading.Tasks;

namespace Controlinventarios.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public AsignacionController(InventoryTIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<AsignacionDto>>> Get()
        {
            var asignaciones = await _context.inv_asignacion.ToListAsync();

            if (asignaciones == null)
            {
                return BadRequest("No se encontraron elementos asignados.");
            }
            
            var asignacionDtos = new List<AsignacionDto>();

            foreach (var asignacion in asignaciones)
            {
                var identificacionPersona = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == asignacion.IdPersona);
                if (identificacionPersona == null)
                {
                    return BadRequest("El nombre no se encontro");
                }

                var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacion.IdEnsamble);
                if (ensamble == null)
                {
                    return BadRequest("El ensamble no se encontro");
                }

                var asignacionDto = new AsignacionDto
                {
                    IdPersona = asignacion.IdPersona,
                    IdEnsamble = asignacion.IdEnsamble,
                    NombrePersona = identificacionPersona.Nombres,
                    Numeroserial = ensamble.NumeroSerial,
                    FechaRegistro = asignacion.FechaRegistro
                };

            asignacionDtos.Add(asignacionDto);
            }

            return Ok(asignacionDtos);
        }

        //[HttpGet("UsuarioConSuListaEquipos")]
        //public async Task<ActionResult<List<ListaAsignacionDto>>> Get5()
        //{
        //    // Obtener todos los usuarios con sus asignaciones y detalles de ensambles
        //    var usuariosConEquipos = await (from ip in _context.inv_persona
        //                                    join a in _context.aspnetusers on ip.userId equals a.Id
        //                                    join ia in _context.inv_asignacion on a.Id equals ia.IdPersona
        //                                    join ie in _context.inv_ensamble on ia.IdEnsamble equals ie.Id
        //                                    join ie2 in _context.inv_elementType on ie.IdElementType equals ie2.id
        //                                    select new ListaAsignacionDto
        //                                    {
        //                                        //Obtener los datos de la tabla inv_persona
        //                                        IdPersona = a.Id,
        //                                        ApellidoPersona = ip.Nombres,
        //                                        NombrePersona = ip.Apellidos,
        //                                        Email = a.Email,
        //                                        //Obtener los datos de la tabla inv_ensamble
        //                                        EquiposAsignados = (from ia2 in _context.inv_asignacion
        //                                                     join ie3 in _context.inv_ensamble on ia2.IdEnsamble equals ie3.Id
        //                                                     join im in _context.inv_marca on ie3.IdMarca equals im.id
        //                                                     join ie4 in _context.inv_elementType on ie3.IdElementType equals ie4.id
        //                                                     where ia2.IdPersona == a.Id
        //                                                     select new ListaEnsambleDto
        //                                                     {
        //                                                         Id = ie3.Id,
        //                                                         NumeroSerial = ie3.NumeroSerial,
        //                                                         Estado = ie3.Estado,
        //                                                         Renting = ie3.Renting,
        //                                                         TipoElemento = ie4.Nombre,
        //                                                         NombreMarca = im.Nombre,
        //                                                         FechaRegistroEquipo = ia2.FechaRegistro
        //                                                     }).ToList()
        //                                    }).ToListAsync();
        //    // Verificar si se encontraron resultados
        //    if (usuariosConEquipos == null || !usuariosConEquipos.Any())
        //    {
        //        return BadRequest("No se encontraron usuarios con equipos asignados.");
        //    }

        //    return Ok(usuariosConEquipos);
        //}

        [HttpGet("UsuarioConSuListaEquipos")]
        public async Task<ActionResult<List<ListaAsignacionDto>>> Get5()
        {
            var usuariosConEquipos = _context.inv_persona
            .Join(_context.aspnetusers, ip => ip.userId, a => a.Id, (ip, a) => new { ip, a })
            .Join(_context.inv_area, ia2 => ia2.ip.IdArea, ia => ia.id, (temp, ia) => new { temp.ip, temp.a, ia })
            .Join(_context.inv_empresa, ie => ie.ip.idEmpresa, ie => ie.id, (temp, ie) => new { temp.ip, temp.a, temp.ia, ie })
            .GroupJoin(_context.inv_asignacion, ua => ua.a.Id, ia => ia.IdPersona, (ua, asignaciones) => new { ua, asignaciones })
            .AsEnumerable() // Se ejecuta en memoria antes de aplicar Any()
            .Where(result => result.asignaciones.Any()) // Filtra usuarios con equipos asignados
            .ToList(); // Se usa ToList() en lugar de ToListAsync()


            var listaUsuarios = new List<ListaAsignacionDto>();

            foreach (var result in usuariosConEquipos)
            {
                listaUsuarios.Add(new ListaAsignacionDto
                {
                    IdPersona = result.ua.a.Id,
                    ApellidoPersona = result.ua.ip.Apellidos,
                    NombrePersona = result.ua.ip.Nombres,
                    CCPersonas = result.ua.ip.identificacion,
                    AreaPersona = result.ua.ia.Nombre, // Manteniendo el ID del área
                    EmpresaPersona = result.ua.ie.Nombre, // Manteniendo el ID de la empresa
                    Email = result.ua.a.Email,
                    CantidadEquiposAsignados = result.asignaciones.Count(), // Contador de equipos asignados
                    EquiposAsignados = result.asignaciones
                        .Join(_context.inv_ensamble, ia => ia.IdEnsamble, ie => ie.Id, (ia, ie) => new { ia, ie })
                        .Join(_context.inv_marca, ieResult => ieResult.ie.IdMarca, im => im.id, (ieResult, im) => new { ieResult, im })
                        .Join(_context.inv_elementType, ieResult => ieResult.ieResult.ie.IdElementType, ie4 => ie4.id, (ieResult, ie4) => new
                        ListaEnsambleDto
                        {
                            Id = ieResult.ieResult.ie.Id,
                            NumeroSerial = ieResult.ieResult.ie.NumeroSerial,
                            Estado = ieResult.ieResult.ie.Estado,
                            Renting = ieResult.ieResult.ie.Renting,
                            TipoElemento = ie4.Nombre,
                            NombreMarca = ieResult.im.Nombre,
                            FechaRegistroEquipo = ieResult.ieResult.ia.FechaRegistro
                        }).ToList()
                });
            }

            if (!listaUsuarios.Any())
            {
                return BadRequest("No se encontraron usuarios con equipos asignados.");
            }

            return Ok(listaUsuarios);
        }



        [HttpGet("{Idpersona}")]
        public async Task<ActionResult<List<ListaAsignacionDto>>> GetId(string Idpersona) // Recibir el parámetro
        {
            var usuariosConEquipos = await _context.inv_persona
                .Join(_context.aspnetusers, ip => ip.userId, a => a.Id, (ip, a) => new { ip, a })
                .Join(_context.inv_area, ia2 => ia2.ip.IdArea, ia => ia.id, (temp, ia) => new { temp.ip, temp.a, ia })
                .Join(_context.inv_empresa, ie => ie.ip.idEmpresa, ie => ie.id, (temp, ie) => new { temp.ip, temp.a, temp.ia, ie })
                .GroupJoin(_context.inv_asignacion, ua => ua.a.Id, ia => ia.IdPersona, (ua, asignaciones) => new { ua, asignaciones })
                .Where(result => result.asignaciones.Any() && result.ua.a.Id == Idpersona) // Filtrar por IDPersona
                .ToListAsync();

            if (!usuariosConEquipos.Any())
            {
                return NotFound($"No se encontraron asignaciones para el usuario con ID: {Idpersona}");
            }

            var listaUsuarios = usuariosConEquipos.Select(result => new ListaAsignacionDto
            {
                IdPersona = result.ua.a.Id,
                ApellidoPersona = result.ua.ip.Nombres,
                NombrePersona = result.ua.ip.Apellidos,
                CCPersonas = result.ua.ip.identificacion,
                AreaPersona = result.ua.ia.Nombre,
                EmpresaPersona = result.ua.ie.Nombre,
                Email = result.ua.a.Email,
                CantidadEquiposAsignados = result.asignaciones.Count(),
                EquiposAsignados = result.asignaciones
                    .Join(_context.inv_ensamble, ia => ia.IdEnsamble, ie => ie.Id, (ia, ie) => new { ia, ie })
                    .Join(_context.inv_marca, ieResult => ieResult.ie.IdMarca, im => im.id, (ieResult, im) => new { ieResult, im })
                    .Join(_context.inv_elementType, ieResult => ieResult.ieResult.ie.IdElementType, ie4 => ie4.id, (ieResult, ie4) => new ListaEnsambleDto
                    {
                        Id = ieResult.ieResult.ie.Id,
                        NumeroSerial = ieResult.ieResult.ie.NumeroSerial,
                        Estado = ieResult.ieResult.ie.Estado,
                        Renting = ieResult.ieResult.ie.Renting,
                        TipoElemento = ie4.Nombre,
                        NombreMarca = ieResult.im.Nombre,
                        FechaRegistroEquipo = ieResult.ieResult.ia.FechaRegistro
                    }).ToList()
            }).ToList();

            return Ok(listaUsuarios);
        }



        [HttpPost]
        public async Task<ActionResult> Post(AsignacionCreateDto createDto)
        {
            // El DTO verifica la tabla
            var asignacion = _mapper.Map<Asignacion>(createDto);

            // Verificación del id de usuario
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == createDto.IdPersona);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {createDto.IdPersona} no fue encontrada.");
            }

            var usuario = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == usuarioExiste.userId);
            if (usuario == null)
            {
                return BadRequest("No se encontraron usuarios");
            }

            // Verificación del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == createDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {createDto.IdEnsamble} no fue encontrado.");
            }

            asignacion.FechaRegistro = DateOnly.FromDateTime(DateTime.Now);

            // Añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);

            // Guardar los datos en la base de datos
            await _context.SaveChangesAsync();

            // Retorna lo guardado, asegurándose de que el parámetro de la ruta coincida con lo esperado en GetId
            return CreatedAtAction(nameof(GetId), new { idEnsamble = asignacion.IdEnsamble }, asignacion);
        }

        [HttpPut("{idEnsamble}")]
        public async Task<ActionResult> Update(int idEnsamble, AsignacionUpdateDto updateDto)
        {
            var ensambleExiste = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == idEnsamble);

            if (ensambleExiste == null)
            {
                return BadRequest($"No se encontró un ensamble con el id: {idEnsamble}");
            }

            ensambleExiste = _mapper.Map(updateDto, ensambleExiste);

            _context.inv_asignacion.Update(ensambleExiste);
            await _context.SaveChangesAsync();

            // Se usa el nombre correcto del parámetro para la acción GetId
            return CreatedAtAction(nameof(GetId), new { idEnsamble = ensambleExiste.IdEnsamble }, ensambleExiste);
        }

        
        //[HttpPatch("CambiarAsignado/{EnsambleId}")]
        //public async Task<ActionResult> Patch(int EnsambleId, AsignacionPatchDto patchDto)
        //{
        //    // Verificar si el ensamble existe
        //    var asignacionExistente = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == EnsambleId);
                
        //    if (asignacionExistente == null)
        //    {
        //        return BadRequest($"El ensamble {EnsambleId} no existe");
        //    }

        //    // Verificar si el nuevo usuario asignado existe
        //    var usuarioExiste = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == patchDto.IdPersona);
                
        //    if (usuarioExiste == null)
        //    {
        //        return BadRequest($"No existe el usuario: {patchDto.IdPersona}");
        //    }

        //    // Actualizar solo la propiedad IdPersona
        //    asignacionExistente.IdPersona = patchDto.IdPersona;

        //    // Guardar los cambios en la base de datos
        //    await _context.SaveChangesAsync();

        //    // Retornar la respuesta con CreatedAtAction
        //    return CreatedAtAction(nameof(GetId), new { idEnsamble = asignacionExistente.IdEnsamble }, asignacionExistente);
        //}


        [HttpDelete("{IdEnsamble}")]
        public async Task<ActionResult> Delete(int IdEnsamble)
        {
            var asignacion = await _context.inv_asignacion.FirstOrDefaultAsync(x => x.IdEnsamble == IdEnsamble);

            if (asignacion == null)
            {
                return BadRequest($"No existe el ensamble: {IdEnsamble}");
            }

            _context.inv_asignacion.Remove(asignacion);
            await _context.SaveChangesAsync();

            return Ok($"Se eliminó el ensamble: {IdEnsamble}");
        }
    }
}