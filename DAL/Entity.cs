using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
    public abstract class Entity : IObjectState
    {
        [NotMapped]
        public ObjectState State { get; set; }
    }
}
