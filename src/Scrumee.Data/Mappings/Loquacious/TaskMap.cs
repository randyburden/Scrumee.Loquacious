using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Scrumee.Data.Entities;

namespace Scrumee.Data.Mappings.Loquacious
{
    public class TaskMap : ClassMapping<Task>
    {
        public TaskMap()
        {
            Id( x => x.Id );
            Property( x => x.Name );
            ManyToOne( x => x.UserStory );
            ManyToOne( x => x.User, map => map.Cascade( Cascade.Persist ) );
        }
    }
}
