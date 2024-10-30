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

            CreateMap<ElementCreateDto, Element>().ReverseMap();
            CreateMap<Element, ElementDto>().ReverseMap();

            //CreateMap<EnsambleCreateDto, Ensamble>().ReverseMap();
            //CreateMap<Ensamble, EnsambleDto>().ReverseMap();

            CreateMap<IdentificadorCreateDto, Identificador>().ReverseMap();
            CreateMap<Identificador, IdentificadorDto>().ReverseMap();

        }
    }
}