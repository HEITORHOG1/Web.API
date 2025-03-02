namespace Web.Domain.DTOs.MercadoPago
{
    public class CustomerDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressDto Address { get; set; }
        public IdentificationDto Identification { get; set; }
        public PhoneDto Phone { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public class AddressDto
        {
            public string ZipCode { get; set; }
            public string StreetName { get; set; }
            public string StreetNumber { get; set; }
        }

        public class IdentificationDto
        {
            public string Type { get; set; }
            public string Number { get; set; }
        }

        public class PhoneDto
        {
            public string AreaCode { get; set; }
            public string Number { get; set; }
        }
    }
}
