/*
 * 
 * HospitalServiceAssignment
 * 
 * Schema for storing which services are offered in a hospital. 
 * 
 * */
namespace app.Models
{
    public class HospitalServiceAssignment
    {
        public int Id { get; set; }
        public virtual HospitalService Service { get; set; } = null!;
        public virtual Hospital Hospital { get; set; } = null!;
    }
}
