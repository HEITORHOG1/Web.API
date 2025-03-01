using System.Globalization;

namespace Web.Application.Validators
{
    public static class PriceFormatter
    {
        public static decimal FormatPrice(decimal value)
        {
            // Primeiro, converte para string sem decimais
            string priceString = value.ToString("F0", CultureInfo.InvariantCulture);

            // Pega o comprimento do string
            int length = priceString.Length;

            // Baseado no comprimento, determina onde colocar o ponto decimal
            decimal result = value;

            if (length >= 3)
            {
                // Move o ponto decimal duas casas para a esquerda
                result = value / 100M;
            }

            // Arredonda para duas casas decimais
            return decimal.Round(result, 2, MidpointRounding.AwayFromZero);
        }
    }
}