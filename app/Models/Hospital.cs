namespace app.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Distance { get; set; } = "";
        public string PlaceID { get; set; } = "";
        public string EstimatedWait { get; set; } = "";
        public string BedsOpen { get; set; } = "";
        public string Status { get; set; } = "";

        public string Address { get; set; } = "";

        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;

        public string Photo { get; set; } = "";
        public List<string> Services { get; set; } = new();
    }

}