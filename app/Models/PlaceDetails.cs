namespace app.Models
{
    public class PlaceDetails
    {
        public string Id { get; set; } = string.Empty;
        public string FormattedAddress { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
