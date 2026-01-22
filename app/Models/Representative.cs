namespace app.Models
{
    public class Representative
    {
        public int Id { get; set; }
        public string Firstname { get; set; } = "";
        public string Lastname { get; set; } = "";

        public string Email { get; set; } = "";

        public string Contact { get; set; } = "";
        public string EmployeeID { get; set; } = "";

        public string Password { get; set; } = "";

        public int Status { get; set; } = 0;

        public virtual Hospital Hospital { get; set; } = null!;
    }
}
