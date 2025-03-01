using AutoMapper;
using Web.Domain.DTOs.Categorias;
using Web.Domain.Entities;

namespace Web.Application.Mappers
{
    public class CategoriaMapper : Profile
    {
        public CategoriaMapper()
        {
            CreateMap<Categoria, CategoriaDto>();
            CreateMap<CategoriaCreateDto, Categoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                 .ForMember(dest => dest.Produtos, opt => opt.Ignore())
                  .ForMember(dest => dest.Estabelecimento, opt => opt.Ignore());

            CreateMap<CategoriaUpdateDto, Categoria>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
               .ForMember(dest => dest.EstabelecimentoId, opt => opt.Ignore())
                .ForMember(dest => dest.Produtos, opt => opt.Ignore())
                 .ForMember(dest => dest.Estabelecimento, opt => opt.Ignore());
        }
    }
}