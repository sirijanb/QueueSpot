namespace app.Models
{
    public class PlaceResult
    {
        public string PlaceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Rating { get; set; }
        public int UserRatingsTotal { get; set; }
        public bool? IsOpen { get; set; }
        public List<string> Types { get; set; } = new();
        public string BusinessStatus { get; set; } = string.Empty;
        public int PriceLevel { get; set; }
        public List<string> Photos { get; set; } = new();
    }

    
}
