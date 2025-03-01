using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Models
{
    public class UserRegistrationModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CPF_CNPJ { get; set; }
        public string NomeUsuario { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
    }
}
