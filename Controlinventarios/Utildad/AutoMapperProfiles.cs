using AutoMapper;
using Controlinventarios.Dto;
using Controlinventarios.Model;
//using Controlinventarios.Dto;
//using Controlinventarios.Model.Valuez;



namespace Controlinventarios.Utilidad
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            
            CreateMap<AreaCreateDto, Area>().ReverseMap();
            CreateMap<Area, AreaDto>().ReverseMap();

            CreateMap<PersonaCreateDto, Persona>().ReverseMap();
            CreateMap<Persona, PersonaDto>().ReverseMap();

            CreateMap<ElementTypeCreateDto, ElementType>().ReverseMap();
            CreateMap<ElementType, ElementTypeDto>().ReverseMap();

            //CreateMap<EnsambleCreateDto, Ensamble>().ReverseMap();
            //CreateMap<Ensamble, EnsambleDto>().ReverseMap();

            CreateMap<PropiedadesCreateDto, Propiedades>().ReverseMap();
            CreateMap<Propiedades, PropiedadesDto>().ReverseMap();

            CreateMap<EnsambleCreateDto, Ensamble>().ReverseMap();
            CreateMap<Ensamble, EnsambleDto>().ReverseMap();
            
            CreateMap<AsignacionCreateDto, Asignacion>().ReverseMap();
            CreateMap<Asignacion, AsignacionDto>().ReverseMap();

            CreateMap<FacturacionTMKCreateDto, FacturacionTMK>().ReverseMap();
            CreateMap<FacturacionTMK, FacturacionTMKDto>().ReverseMap();
            CreateMap<FacturacionTMK, CentroDeCostoDto>().ReverseMap();

            CreateMap<MarcaCreateDto, Marca>().ReverseMap();
            CreateMap<Marca, MarcaDto>().ReverseMap();
            
            CreateMap<AspnetUsers, AspnetUsersDto>().ReverseMap();

            CreateMap<EmpresaCreateDto, Empresa>().ReverseMap();
            CreateMap<Empresa, EmpresaDto>().ReverseMap();
        }
    }
}