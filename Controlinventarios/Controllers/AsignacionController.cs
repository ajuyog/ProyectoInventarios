using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Controlinventarios.Utildad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<object>> Get(string search, [FromQuery] PaginacionDTO paginacionDTO)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var asigancionesConfi = _context.inv_asignacion
                 .Join(_context.inv_persona, persona => persona.IdPersona, idpersona => idpersona.userId, (persona, idpersona) => new
                 {
                     persona,
                     idpersona
                 })
                 .Join(_context.inv_area, persona => persona.idpersona.IdArea, idarea => idarea.id, (persona, idarea) => new
                 {
                     persona,
                     idarea
                 })
                 .Join(_context.inv_empresa, persona => persona.persona.idpersona.idEmpresa, empresa => empresa.id, (persona, empresa) => new
                 {
                     persona,
                     empresa
                 })
                 .Join(_context.aspnetusers, userName => userName.persona.persona.idpersona.userId, user => user.Id, (userName, user) => new
                 {
                     userName.persona,
                     userName.persona.idarea,
                     userName.empresa,
                     user
                 })
                 .Join(_context.inv_ensamble, persona => persona.persona.persona.persona.IdEnsamble, ensamble => ensamble.Id, (persona, ensamble) => new
                 {
                     persona.persona,
                     persona.idarea,
                     persona.empresa,
                     persona.user,
                     ensamble
                 })
                 .Join(_context.inv_elementType, ensamble => ensamble.ensamble.IdElementType, elementType => elementType.id, (ensamble, elementType) => new
                 {
                     ensamble.persona,
                     ensamble.idarea,
                     ensamble.empresa,
                     ensamble.user,
                     ensamble.ensamble,
                     ElementTypeName = elementType.Nombre // 👈 nombre del tipo de elemento
                 })
                 .Join(_context.inv_marca, ensamble => ensamble.ensamble.IdMarca, marca => marca.id, (ensamble, marca) => new
                 {
                     ensamble.persona,
                     ensamble.idarea,
                     ensamble.empresa,
                     ensamble.user,
                     ensamble.ensamble,
                     ensamble.ElementTypeName,
                     MarcaName = marca.Nombre // 👈 nombre de la marca
                 })
                 .Where(x =>
                     x.persona.persona.idpersona.userId.Contains(search) ||
                     (x.persona.persona.idpersona.Nombres + " " + x.persona.persona.idpersona.Apellidos).Contains(search) ||
                     x.persona.persona.idpersona.identificacion.Contains(search) ||
                     x.ensamble.NumeroSerial.Contains(search) ||
                     x.MarcaName.Contains(search) ||
                     x.ElementTypeName.Contains(search)
                 )
                 .Select(x => new AsignacionDto
                 {
                     IdPersona = x.persona.persona.idpersona.userId,
                     IdEnsamble = x.ensamble.Id,
                     NombrePersona = x.persona.persona.idpersona.Nombres,
                     ApellidoPersona = x.persona.persona.idpersona.Apellidos,
                     CCPersonas = x.persona.persona.idpersona.identificacion,
                     AreaPersona = x.idarea.Nombre,
                     Email = x.user.Email,
                     Id = x.ensamble.Id,
                     NumeroSerial = x.ensamble.NumeroSerial,
                     Estado = x.ensamble.Estado,
                     Renting = x.ensamble.Renting,
                     TipoElemento = x.ElementTypeName,
                     NombreMarca = x.MarcaName,
                     FechaRegistroEquipo = x.ensamble.FechaRegistroEquipo
                 })
                 .OrderBy(e => e.IdPersona).AsQueryable();
                var personas = await asigancionesConfi.Paginar(paginacionDTO).ToListAsync();

                await HttpContext.InsertarParametrosPaginacionEnCabecera(asigancionesConfi);
                await HttpContext.TInsertarParametrosPaginacion(asigancionesConfi, paginacionDTO.RegistrosPorPagina);
                return Ok(personas);
            }
            else
            {
                var confiAsiganciones = _context.inv_asignacion
                .Join(_context.inv_persona, persona => persona.IdPersona, idpersona => idpersona.userId, (persona, idpersona) => new
                {
                    persona,
                    idpersona
                })
                .Join(_context.inv_area, persona => persona.idpersona.IdArea, idarea => idarea.id, (persona, idarea) => new
                {
                    persona,
                    idarea
                })
                .Join(_context.inv_empresa, persona => persona.persona.idpersona.idEmpresa, empresa => empresa.id, (persona, empresa) => new
                {
                    persona,
                    empresa
                })
                .Join(_context.aspnetusers, userName => userName.persona.persona.idpersona.userId, user => user.Id, (userName, user) => new
                {
                    userName.persona,
                    userName.persona.idarea,
                    userName.empresa,
                    user
                })
                .Join(_context.inv_ensamble, persona => persona.persona.persona.persona.IdEnsamble, ensamble => ensamble.Id, (persona, ensamble) => new
                {
                    persona.persona,
                    persona.idarea,
                    persona.empresa,
                    persona.user,
                    ensamble
                })
                .Join(_context.inv_elementType, ensamble => ensamble.ensamble.IdElementType, elementType => elementType.id, (ensamble, elementType) => new
                {
                    ensamble.persona,
                    ensamble.idarea,
                    ensamble.empresa,
                    ensamble.user,
                    ensamble.ensamble,
                    ElementTypeName = elementType.Nombre // 👈 nombre del tipo de elemento
                })
                .Join(_context.inv_marca, ensamble => ensamble.ensamble.IdMarca, marca => marca.id, (ensamble, marca) => new
                {
                    ensamble.persona,
                    ensamble.idarea,
                    ensamble.empresa,
                    ensamble.user,
                    ensamble.ensamble,
                    ensamble.ElementTypeName,
                    MarcaName = marca.Nombre // 👈 nombre de la marca
                })
                .Select(x => new AsignacionDto
                {
                    IdPersona = x.persona.persona.idpersona.userId,
                    IdEnsamble = x.ensamble.Id,
                    NombrePersona = x.persona.persona.idpersona.Nombres,
                    ApellidoPersona = x.persona.persona.idpersona.Apellidos,
                    CCPersonas = x.persona.persona.idpersona.identificacion,
                    AreaPersona = x.idarea.Nombre,
                    Email = x.user.Email,
                    Id = x.ensamble.Id,
                    NumeroSerial = x.ensamble.NumeroSerial,
                    Estado = x.ensamble.Estado,
                    Renting = x.ensamble.Renting,
                    TipoElemento = x.ElementTypeName,
                    NombreMarca = x.MarcaName,
                    FechaRegistroEquipo = x.ensamble.FechaRegistroEquipo
                })
                 .OrderBy(e => e.IdPersona).AsQueryable();

                var personas = await confiAsiganciones.Paginar(paginacionDTO).ToListAsync();

                await HttpContext.InsertarParametrosPaginacionEnCabecera(confiAsiganciones);
                await HttpContext.TInsertarParametrosPaginacion(confiAsiganciones, paginacionDTO.RegistrosPorPagina);
                return Ok(personas);
            }
        }
        [HttpGet("{idEnsamble}")]
        public async Task<ActionResult<AsignacionCreateDto>> GetById(int idEnsamble)
        {
            var ensamble = await _context.inv_asignacion
                .FirstOrDefaultAsync(x => x.IdEnsamble == idEnsamble);

            if (ensamble == null)
            {
                return NotFound($"No se encontró un ensamble con el id: {idEnsamble}");
            }

            var ensambleDto = _mapper.Map<AsignacionCreateDto>(ensamble);

            return Ok(ensambleDto);
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
            return Ok(asignacion);
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
            return Ok(ensambleExiste);
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