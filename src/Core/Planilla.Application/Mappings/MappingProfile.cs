using AutoMapper;
using Vorluno.Planilla.Application.DTOs;
using Vorluno.Planilla.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vorluno.Planilla.Application.Mappings
{
    /// <summary>
    /// Define las reglas de mapeo entre las entidades del dominio y los DTOs.
    /// AutoMapper escaneará este ensamblado en busca de clases que hereden de Profile.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Entidad a DTO (para operaciones de lectura)
            CreateMap<Empleado, EmpleadoVerDto>()
                .ForMember(dest => dest.DepartamentoNombre, opt => opt.MapFrom(src => src.Departamento != null ? src.Departamento.Nombre : null))
                .ForMember(dest => dest.PosicionNombre, opt => opt.MapFrom(src => src.Posicion != null ? src.Posicion.Nombre : null));

            // Mapeo de DTO a Entidad (para operaciones de escritura/actualización)
            CreateMap<EmpleadoCrearDto, Empleado>();
            CreateMap<EmpleadoActualizarDto, Empleado>();
        }
    }
}