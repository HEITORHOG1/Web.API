namespace Web.Application.Interfaces
{
    public interface IGeocodingService
    {
        /// <summary>
        /// Obtém as coordenadas de um endereço.
        /// </summary>
        /// <param name="address">Endereço completo (incluindo CEP).</param>
        /// <returns>Tupla contendo Latitude e Longitude.</returns>
        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address);
    }
}