using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.DTOs.Endereco;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class EnderecoClienteService : IEnderecoClienteService
    {
        private readonly IEnderecoClienteRepository _enderecoClienteRepository;
        private readonly EnderecoClienteValidator _enderecoClienteValidator;

        public EnderecoClienteService(IEnderecoClienteRepository enderecoClienteRepository, EnderecoClienteValidator enderecoClienteValidator)
        {
            _enderecoClienteRepository = enderecoClienteRepository;
            _enderecoClienteValidator = enderecoClienteValidator;
        }

        public async Task<IEnumerable<EnderecoCliente>> GetAllByUsuarioIdAsync(string usuarioId)
        {
            return await _enderecoClienteRepository.GetAllByUsuarioIdAsync(usuarioId);
        }

        public async Task<EnderecoCliente> AddEnderecoAsync(string userId, EnderecoClienteDto enderecoDto)
        {
            var endereco = new EnderecoCliente
            {
                UsuarioId = userId,
                Logradouro = enderecoDto.Logradouro,
                Numero = enderecoDto.Numero,
                Complemento = enderecoDto.Complemento,
                Bairro = enderecoDto.Bairro,
                Cidade = enderecoDto.Cidade,
                Estado = enderecoDto.Estado,
                CEP = enderecoDto.CEP,
                Principal = enderecoDto.Principal
            };
            var validationResult = await _enderecoClienteValidator.ValidateAsync(endereco);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var existingPrincipal = await _enderecoClienteRepository.GetPrincipalByUsuarioIdAsync(userId);

            if (endereco.Principal && existingPrincipal != null)
            {
                existingPrincipal.Principal = false;
                await _enderecoClienteRepository.UpdateAsync(existingPrincipal);
            }

            await _enderecoClienteRepository.AddAsync(endereco);
            return endereco;
        }
        public async Task<EnderecoCliente> GetPrincipalByUsuarioIdAsync(string usuarioId)
        {
            return await _enderecoClienteRepository.GetPrincipalByUsuarioIdAsync(usuarioId);
        }

        public async Task<EnderecoCliente> GetEnderecoByIdAsync(int id)
        {
            return await _enderecoClienteRepository.GetByIdAsync(id);
        }

        public async Task UpdateEnderecoAsync(int id, string userId, EnderecoClienteDto enderecoDto)
        {
            var endereco = await _enderecoClienteRepository.GetByIdAsync(id);
            if (endereco == null || endereco.UsuarioId != userId)
                throw new KeyNotFoundException("Endereço não encontrado ou não pertence ao usuário.");

            endereco.Logradouro = enderecoDto.Logradouro;
            endereco.Numero = enderecoDto.Numero;
            endereco.Complemento = enderecoDto.Complemento;
            endereco.Bairro = enderecoDto.Bairro;
            endereco.Cidade = enderecoDto.Cidade;
            endereco.Estado = enderecoDto.Estado;
            endereco.CEP = enderecoDto.CEP;
            endereco.Principal = enderecoDto.Principal;


            var validationResult = await _enderecoClienteValidator.ValidateAsync(endereco);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var existingPrincipal = await _enderecoClienteRepository.GetPrincipalByUsuarioIdAsync(userId);
            if (endereco.Principal && existingPrincipal != null && existingPrincipal.Id != endereco.Id)
            {
                existingPrincipal.Principal = false;
                await _enderecoClienteRepository.UpdateAsync(existingPrincipal);
            }

            await _enderecoClienteRepository.UpdateAsync(endereco);
        }
        public async Task DeleteEnderecoAsync(int id)
        {
            var existingEndereco = await _enderecoClienteRepository.GetByIdAsync(id);
            if (existingEndereco != null)
            {
                await _enderecoClienteRepository.DeleteAsync(id);
            }
        }
    }
}
