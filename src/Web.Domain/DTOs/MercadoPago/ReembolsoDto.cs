namespace Web.Domain.DTOs.MercadoPago
{
    public class ReembolsoDto
    {
        public bool Parcial { get; set; } = false;
        public decimal? Valor { get; set; }
    }
}
