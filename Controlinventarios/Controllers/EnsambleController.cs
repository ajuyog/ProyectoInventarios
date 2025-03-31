using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Controlinventarios.Utildad;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controlinventarios.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EnsambleController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public EnsambleController(InventoryTIContext context, IMapper mapper)
        {

            _context = context;
            _mapper = mapper;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<EnsambleDto>>> Get()
        //{

        //    var ensamble = await _context.inv_ensamble.ToListAsync();
        //    if (ensamble == null)
        //    {
        //        return BadRequest($"No se encontraron datos en la tabla ensamble");
        //    }

        //    var ensambleDtos = _mapper.Map<List<EnsambleDto>>(ensamble);

        //    return Ok(ensambleDtos);
        //}


        [HttpGet]
        public async Task<ActionResult<object>> Get(string search, [FromQuery] PaginacionDTO paginacionDTO)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var ensambles2 = _context.inv_ensamble
                .Join(_context.inv_elementType, ensamble => ensamble.IdElementType, elementType => elementType.id, (ensamble, elementType) => new
                {
                    ensamble,
                    elementType
                })
                .Join(_context.inv_marca, ensambleElementType => ensambleElementType.ensamble.IdMarca, marca => marca.id, (ensambleElementType, marca) => new
                {
                    ensambleElementType.ensamble,
                    ensambleElementType.elementType,
                    marca
                })
                .Where(x => x.ensamble.NumeroSerial.Contains(search)
                    || x.elementType.Nombre.Contains(search)
                    || x.marca.Nombre.Contains(search))
                .Select(x => new EnsambleDto
                {
                    Id = x.ensamble.Id,
                    IdElementType = x.ensamble.IdElementType,
                    IdMarca = x.ensamble.IdMarca,
                    NumeroSerial = x.ensamble.NumeroSerial,
                    Estado = x.ensamble.Estado,
                    Descripcion = x.ensamble.Descripcion,
                    Renting = x.ensamble.Renting,
                    TipoElemento = x.elementType.Nombre ?? "Desconocido",
                    NombreMarca = x.marca.Nombre ?? "Desconocida",
                    FechaRegistroEquipo = x.ensamble.FechaRegistroEquipo
                })
                .OrderBy(e => e.Id).AsQueryable();

                var ensambles = await ensambles2.Paginar(paginacionDTO).ToListAsync();
                await HttpContext.InsertarParametrosPaginacionEnCabecera(ensambles2);
                await HttpContext.TInsertarParametrosPaginacion(ensambles2, paginacionDTO.RegistrosPorPagina);
                return Ok(ensambles);
            }
            else
            {
                var ensambles3 = _context.inv_ensamble
                .Join(_context.inv_elementType, ensamble => ensamble.IdElementType, elementType => elementType.id,
                (ensamble, elementType) => new
                {
                    ensamble,
                    elementType
                })
                .Join(_context.inv_marca, ensambleElementType => ensambleElementType.ensamble.IdMarca, marca => marca.id,
                (ensambleElementType, marca) => new
                {
                    ensambleElementType.ensamble,
                    ensambleElementType.elementType,
                    marca
                })
                .Select(x => new EnsambleDto
                {
                    Id = x.ensamble.Id,
                    IdElementType = x.ensamble.IdElementType,
                    IdMarca = x.ensamble.IdMarca,
                    NumeroSerial = x.ensamble.NumeroSerial,
                    Estado = x.ensamble.Estado,
                    Descripcion = x.ensamble.Descripcion,
                    Renting = x.ensamble.Renting,
                    TipoElemento = x.elementType.Nombre ?? "Desconocido",
                    NombreMarca = x.marca.Nombre ?? "Desconocida",
                    FechaRegistroEquipo = x.ensamble.FechaRegistroEquipo
                })
                .OrderBy(e => e.Id).AsQueryable();

                var ensambles = await ensambles3.Paginar(paginacionDTO).ToListAsync();
                await HttpContext.InsertarParametrosPaginacionEnCabecera(ensambles3);
                await HttpContext.TInsertarParametrosPaginacion(ensambles3, paginacionDTO.RegistrosPorPagina);
                return Ok(ensambles);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnsambleDto>> GetId(int id)
        {
            // Buscar el ensamble por su ID
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);
            if (ensamble == null)
            {
                return BadRequest($"No se encontraron ensambles con el id {id}");
            }

            // Obtener el tipo de elemento relacionado
            var elementype = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == ensamble.IdElementType);
            if (elementype == null)
            {
                return BadRequest("No se encontró el tipo de elemento asociado al ensamble.");
            }

            // Obtener la marca relacionada
            var marca = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == ensamble.IdMarca);
            if (marca == null)
            {
                return BadRequest("No se encontró la marca asociada al ensamble.");
            }

            // Mapear el ensamble a EnsambleDto
            var ensambleDto = new EnsambleDto
            {
                Id = ensamble.Id,
                IdElementType = ensamble.IdElementType,
                IdMarca = ensamble.IdMarca,
                NumeroSerial = ensamble.NumeroSerial,
                Estado = ensamble.Estado,
                Descripcion = ensamble.Descripcion,
                Renting = ensamble.Renting,
                TipoElemento = elementype.Nombre, // Nombre del tipo de elemento
                NombreMarca = marca.Nombre, // Nombre de la marca
                FechaRegistroEquipo = ensamble.FechaRegistroEquipo
            };

            // Devolver el DTO
            return Ok(ensambleDto);
        }

        //[HttpGet("Propiedades concatenadas")]
        //public async Task<ActionResult<List<EnsambleDto2>>> GetPropiedadesConcatenadas()
        //{
        //    var result = await _context.inv_ensamble
        //        .Join(_context.inv_propiedades,
        //              ie => ie.Id,
        //              ip => ip.IdEnsamble,
        //              (ie, ip) => new { ie.Id, ie.NumeroSerial, ip.Propiedad })
        //        .OrderByDescending(x => x.Id) // Aquí ordenas primero por Id
        //        .GroupBy(x => x.NumeroSerial)
        //        .Select(g => new EnsambleDto2
        //        {
        //            id = g.First().Id,
        //            NumeroSerial = g.Key,
        //            // ordenas dentro del grupo por id y concatenas las propiedades
        //            PropiedadesConcatenadas = string.Join("; ", g.OrderBy(x => x.Id).Select(x => x.Propiedad)),
        //        })
        //        .ToListAsync();

        //    if (result == null )
        //    {
        //        return BadRequest("No se encontraron datos de propiedades concatenadas");
        //    }

        //    return Ok(result);
        //}

        [HttpGet("PropiedadesConcatenadas")]
        public async Task<ActionResult<List<EnsambleDto2>>> GetPropiedadesConcatenadas()
        {
            // Conteo total de registros en la tabla inv_ensamble
            var totalRegistros = await _context.inv_ensamble.CountAsync();

            var result = await _context.inv_ensamble
                .Join(_context.inv_propiedades,
                      ie => ie.Id,
                      ip => ip.IdEnsamble,
                      (ie, ip) => new { ie.Id, ie.NumeroSerial, ip.Propiedad })
                .OrderByDescending(x => x.Id) // Ordena primero por Id
                .GroupBy(x => x.NumeroSerial)   
                .Select(g => new EnsambleDto2
                {
                    id = g.First().Id,
                    NumeroSerial = g.Key,
                    // Ordena dentro del grupo por id y concatena las propiedades
                    PropiedadesConcatenadas = string.Join("; ", g.OrderBy(x => x.Id).Select(x => x.Propiedad)),
                })
                .ToListAsync();

            if (result == null || !result.Any())
            {
                return BadRequest("No se encontraron datos de propiedades concatenadas");
            }

            // Devuelve el resultado junto con el conteo total de registros
            return Ok(new
            {
                TotalRegistros = totalRegistros,
                Resultados = result.OrderBy(x => x.id)
            });
        }

        [HttpGet("Busqueda/{busqueda}")]
        public async Task<ActionResult<List<EnsambleDto>>> GetBusqueda(string busqueda)
        {
            // Inicializar la consulta con joins para incluir ElementType y Marca
            var query = from e in _context.inv_ensamble
                        join et in _context.inv_elementType on e.IdElementType equals et.id
                        join m in _context.inv_marca on e.IdMarca equals m.id
                        select new { Ensamble = e, ElementType = et, Marca = m };

            // Si el parámetro de búsqueda no está vacío o nulo, aplicar filtros
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(x => x.Ensamble.NumeroSerial.Contains(busqueda) ||
                                         x.ElementType.Nombre.Contains(busqueda) ||
                                         x.Marca.Nombre.Contains(busqueda));
            }

            // Ejecutar la consulta
            var resultados = await query.ToListAsync();

            // Si no hay resultados, retornar BadRequest
            if (!resultados.Any())
            {
                return BadRequest("No existe ninguna coincidencia");
            }

            // Mapear a DTO
            var ensamblesDto = resultados.Select(x => new EnsambleDto
            {
                Id = x.Ensamble.Id,
                IdElementType = x.Ensamble.IdElementType,
                IdMarca = x.Ensamble.IdMarca,
                NumeroSerial = x.Ensamble.NumeroSerial,
                Estado = x.Ensamble.Estado,
                Descripcion = x.Ensamble.Descripcion,
                Renting = x.Ensamble.Renting,
                TipoElemento = x.ElementType.Nombre,
                NombreMarca = x.Marca.Nombre,
                FechaRegistroEquipo = x.Ensamble.FechaRegistroEquipo
            }).ToList();

            return Ok(ensamblesDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(EnsambleCreateDto createDto)
        {
            // El DTO verifica la tabla
            var ensamble = _mapper.Map<Ensamble>(createDto);

            // Verificación de ElementType
            var elementoExiste = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == createDto.IdElementType);
            if (elementoExiste == null)
            {
                return BadRequest($"El tipo de elemento con el ID {createDto.IdElementType} no fue encontrado.");
            }

            // Verificación sobre la marca
            var marcaExiste = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == createDto.IdMarca);
            if (marcaExiste == null)
            {
                return BadRequest($"La marca con ID {createDto.IdMarca} no fue encontrada.");
            }

            // asigna la fecha actual al campo FechaRegistroEquipo
            ensamble.FechaRegistroEquipo = DateOnly.FromDateTime(DateTime.Now);

            // añade la entidad al contexto
            _context.inv_ensamble.Add(ensamble);

            // guarda los datos en la base de datos
            await _context.SaveChangesAsync();

            // retorna lo guardar
            return CreatedAtAction(nameof(GetId), new { id = ensamble.Id }, ensamble);
        }

        [HttpPost("AsignacionAuto")]
        public async Task<ActionResult> Post2(EnsambleCreateDto createDto)
        {
            // El DTO verifica la tabla
            var ensamble = _mapper.Map<Ensamble>(createDto);

            // Verificación de ElementType
            var elementoExiste = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == createDto.IdElementType);
            if (elementoExiste == null)
            {
                return BadRequest($"El tipo de elemento con el ID {createDto.IdElementType} no fue encontrado.");
            }

            // Verificación sobre la marca
            var marcaExiste = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == createDto.IdMarca);
            if (marcaExiste == null)
            {
                return BadRequest($"La marca con ID {createDto.IdMarca} no fue encontrada.");
            }

            // Asigna la fecha actual al campo FechaRegistroEquipo
            ensamble.FechaRegistroEquipo = DateOnly.FromDateTime(DateTime.Now);

            // Añade la entidad al contexto
            _context.inv_ensamble.Add(ensamble);

            // Guarda los datos en la base de datos
            await _context.SaveChangesAsync();

            // ID del usuario específico al que siempre se asignará el ensamble
            var userIdEspecifico = "1bc91797-3b00-4fa4-b086-8b64927e0732";

            // Verificación del usuario específico
            var usuarioExiste = await _context.inv_persona.FirstOrDefaultAsync(x => x.userId == userIdEspecifico);
            if (usuarioExiste == null)
            {
                return BadRequest($"La persona con el ID {userIdEspecifico} no fue encontrada.");
            }

            var usuarioAsp = await _context.aspnetusers.FirstOrDefaultAsync(x => x.Id == usuarioExiste.userId);
            if (usuarioAsp == null)
            {
                return BadRequest("No se encontraron usuarios");
            }

            // Crear la asignación para el usuario específico
            var asignacionCreateDto = new AsignacionCreateDto
            {
                IdPersona = userIdEspecifico, // Asignar al usuario específico
                IdEnsamble = ensamble.Id,
                
                // Otros campos necesarios para la asignación
            };

            var asignacion = _mapper.Map<Asignacion>(asignacionCreateDto);

            // Verificación del id Ensamble
            var ensambleExiste = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == asignacionCreateDto.IdEnsamble);
            if (ensambleExiste == null)
            {
                return BadRequest($"El ensamble con ID {asignacionCreateDto.IdEnsamble} no fue encontrado.");
            }

            asignacion.FechaRegistro = DateOnly.FromDateTime(DateTime.Now);

            // Añade la entidad al contexto
            _context.inv_asignacion.Add(asignacion);

            // Guardar los datos en la base de datos
            await _context.SaveChangesAsync();

            // Retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { id = ensamble.Id }, ensamble);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EnsambleCreateDto updateDto)
        {
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);
            if (ensamble == null)
            {
                return BadRequest($"No se encontro el {id}.");
            }

            //verificacion de ElemenType
            var elementoExiste = await _context.inv_elementType.FirstOrDefaultAsync(x => x.id == updateDto.IdElementType);
            if (elementoExiste == null)
            {
                return BadRequest($"El tipo de elemento con el ID {updateDto.IdElementType} no fue encontrado.");
            }

            //verificacion sobre la marca
            var marcaExiste = await _context.inv_marca.FirstOrDefaultAsync(x => x.id == updateDto.IdMarca);
            if (marcaExiste == null)
            {
                return BadRequest($"La marca con el ID {updateDto.IdMarca} no fue encontrado.");
            }

            ensamble = _mapper.Map(updateDto, ensamble);

            _context.inv_ensamble.Update(ensamble);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { ensamble.Id }, ensamble);

        }

        [HttpDelete("EliminarConPropiedades/{id}")]
        public async Task<ActionResult> DeleteEnsamble(int id)
        {
            // Buscar el ensamble por su id
            var ensamble = await _context.inv_ensamble.FirstOrDefaultAsync(x => x.Id == id);

            if (ensamble == null)
            {
                return BadRequest($"No existe el ensamble con id: {id}");
            }

            // Buscar todas las propiedades asociadas al ensamble
            var propiedades = _context.inv_propiedades
                .Where(p => p.IdEnsamble == id)
                .ToList();

            // Eliminar las propiedades asociadas
            if (propiedades.Any())
            {
                _context.inv_propiedades.RemoveRange(propiedades);
            }

            // Eliminar el ensamble
            _context.inv_ensamble.Remove(ensamble);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok("Se elimino correctamente");
        }

    }
}