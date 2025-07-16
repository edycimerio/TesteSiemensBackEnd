using AutoMapper;
using SistemaLivros.API.Models.Request.Autores;
using SistemaLivros.API.Models.Request.Generos;
using SistemaLivros.API.Models.Request.Livros;
using SistemaLivros.API.Models.Response.Autores;
using SistemaLivros.API.Models.Response.Generos;
using SistemaLivros.API.Models.Response.Livros;
using SistemaLivros.Application.Commands.Autores;
using SistemaLivros.Application.Commands.Generos;
using SistemaLivros.Application.Commands.Livros;
using SistemaLivros.Application.DTOs;

namespace SistemaLivros.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Request -> Command
            CreateMap<GeneroRequest, CreateGeneroCommand>();
            CreateMap<GeneroRequest, UpdateGeneroCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID é definido na rota
            
            CreateMap<AutorRequest, CreateAutorCommand>();
            CreateMap<AutorRequest, UpdateAutorCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID é definido na rota
            
            CreateMap<LivroRequest, CreateLivroCommand>();
            CreateMap<LivroRequest, UpdateLivroCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID é definido na rota
            
            // DTO -> Response
            CreateMap<GeneroDto, GeneroResponse>();
            CreateMap<GeneroDetalhesDto, GeneroDetalhesResponse>();
            
            CreateMap<AutorDto, AutorResponse>();
            CreateMap<AutorDetalhesDto, AutorDetalhesResponse>();
            
            CreateMap<LivroDto, LivroResponse>();
            CreateMap<LivroDto, LivroSimplificadoResponse>();
            CreateMap<LivroDetalhesDto, LivroDetalhesResponse>();
        }
    }
}
