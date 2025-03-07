﻿namespace Web.Domain.DTOs.MercadoPago
{
    public class PreferenceItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public string CategoryId { get; set; }
        public int? Quantity { get; set; }
        public string CurrencyId { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
