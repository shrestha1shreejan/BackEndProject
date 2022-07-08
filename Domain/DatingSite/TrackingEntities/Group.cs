using System.ComponentModel.DataAnnotations;

namespace Domain.DatingSite.TrackingEntities
{
    /// <summary>
    /// For tracking if users are in same group at a time
    /// </summary>
    public class Group
    {
        // Default consturctor is needed for EF while creating table
        public Group()
        {

        }

        public Group(string name)
        {
            Name = name;            
        }

        [Key]
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
