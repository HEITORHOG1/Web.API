using AutoMapper;
using Web.Domain.DTOs.Endereco;
using Web.Domain.Entities;

namespace Web.Application.Mappers
{
    public class EnderecoClienteMapper : Profile
    {
        public EnderecoClienteMapper()
        {
            CreateMap<EnderecoCliente, EnderecoClienteDto>().ReverseMap();
        }
    }
}