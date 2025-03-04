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
            //Incio mapeo de area
            CreateMap<AreaCreateDto, Area>().ReverseMap();
            CreateMap<Area, AreaDto>().ReverseMap();
            //Fin de mapeo de area

            //Incio mapeo Persona
            CreateMap<PersonaCreateDto, Persona>().ReverseMap();
            CreateMap<PersonaUpdateDto, Persona>().ReverseMap();
            CreateMap<Persona, PersonaDto>().ReverseMap();
            //Fin mapeo Persona

            //Incio Mapeo TipoElemento
            CreateMap<ElementTypeCreateDto, ElementType>().ReverseMap();
            CreateMap<ElementType, ElementTypeDto>().ReverseMap();
            //Fin Mapeo TipoElemento 

            //Inicio mapeo Propiedades
            CreateMap<PropiedadesCreateDto, Propiedades>().ReverseMap();
            CreateMap<PropiedadesUpdateDto, Propiedades>().ReverseMap();
            CreateMap<Propiedades, PropiedadesDto>().ReverseMap();
            //Fin mapeo Propiedades

            //Incio Mapeo Ensamble
            CreateMap<EnsambleCreateDto, Ensamble>().ReverseMap();
            CreateMap<Ensamble, EnsambleDto>().ReverseMap();
            CreateMap<Ensamble, ListaEnsambleDto>().ReverseMap();
            CreateMap<Ensamble, EnsambleDto2>().ReverseMap();
            //Fin mapeo ensamble

            //Incio Mapeo Asignacion
            CreateMap<AsignacionCreateDto, Asignacion>().ReverseMap();
            CreateMap<AsignacionPatchDto, Asignacion>().ReverseMap();
            CreateMap<AsignacionUpdateDto, Asignacion>().ReverseMap();
            CreateMap<Asignacion, AsignacionDto>().ReverseMap();
            CreateMap<Asignacion, ListaAsignacionDto>().ReverseMap();
            //Fin mapero asignacion

            //Inicio Mapeo FacturacionTMK
            CreateMap<FacturacionTMKCreateDto, FacturacionTMK>().ReverseMap();
            CreateMap<FacturacionTMK, FacturacionTMKDto>().ReverseMap();
            //CreateMap<FacturacionTMK, CentroDeCostoDto>().ReverseMap();
            //Fin Mapeo FacturacionTMK

            //Inicio mapeo marca
            CreateMap<MarcaCreateDto, Marca>().ReverseMap();
            CreateMap<Marca, MarcaDto>().ReverseMap();
            //Fin mapeo marca

            //Incio mapeo users
            CreateMap<AspnetUsers, AspnetUsersDto>().ReverseMap();
            //Fin mapeo users

            //Inicio mapeo Empresa
            CreateMap<EmpresaCreateDto, Empresa>().ReverseMap();
            CreateMap<Empresa, EmpresaDto>().ReverseMap();
            //Fin mapeo Empresa
        }
    }
}