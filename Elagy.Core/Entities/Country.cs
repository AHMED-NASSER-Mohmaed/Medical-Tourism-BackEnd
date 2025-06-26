namespace Elagy.Core.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Governorate> Governorates { get; set; }
    }

}
