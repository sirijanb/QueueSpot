/*
 * 
 * HospitalService
 * 
 * To store lists of available service name, (Eg : Emergency Dept, Internal Medicine, Pediatrics, Ophthalmology, etc...)
 * 
 * */
namespace app.Models
{
    public class HospitalService
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = "";
        public virtual List<HospitalServiceAssignment>? Assignments { get; set; }
    }
}
