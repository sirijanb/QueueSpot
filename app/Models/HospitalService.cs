namespace app.Models
{
    public class HospitalService
    {
        public int ServiceId { get; set; } = 0;

        public virtual Hospital Hospital { get; set; } = null!;


    }
}
