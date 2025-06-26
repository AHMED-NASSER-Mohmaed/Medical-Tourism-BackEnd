namespace Elagy.Core.Entities
{
    public class Patient : User
    {
        public string BloodGroup { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        //public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}