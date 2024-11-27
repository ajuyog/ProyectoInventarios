using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Controlinventarios.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {

        private readonly InventoryTIContext _context;
        private readonly IMapper _mapper;

        public EmpresaController(InventoryTIContext context, IMapper mapper)
        {

            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmpresaDto>>> Get()
        {

            var emporesa = await _context.inv_empresa.ToListAsync();
            if (emporesa == null)
            {
                return BadRequest($"No se encontraron datos");
            }

            var empresaDtos = _mapper.Map<List<EmpresaDto>>(emporesa);

            return Ok(empresaDtos);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<EmpresaDto>> GetId(string nombre)
        {
            var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.Nombre == nombre);
            if (empresa == null)
            {
                return BadRequest($"No existe la empresa: {nombre}");
            }

            var empresaDto = _mapper.Map<EmpresaDto>(empresa);
            return Ok(empresaDto);
        }

        [HttpPost]
        public async Task<ActionResult> Post(EmpresaCreateDto createDto)
        {
            // el dto verifica la tabla
            var empresa = _mapper.Map<Empresa>(createDto);

            // añade la entidad al contexto
            _context.inv_empresa.Add(empresa);
            // guardar los datos en la basee de datos
            await _context.SaveChangesAsync();
            //retorna lo guardado
            return CreatedAtAction(nameof(GetId), new { Nombre = empresa.Nombre }, empresa);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, EmpresaCreateDto updateDto)
        {
            var empresa = await _context.inv_empresa.FirstOrDefaultAsync(x => x.id == id);

            empresa = _mapper.Map(updateDto, empresa);

            _context.inv_empresa.Update(empresa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { empresa.Nombre }, empresa);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var empresa = await _context.inv_empresa.FindAsync(id);

            if (empresa == null)
            {
                return BadRequest($"No existe el id: {id}");
            }

            _context.inv_empresa.Remove(empresa);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}