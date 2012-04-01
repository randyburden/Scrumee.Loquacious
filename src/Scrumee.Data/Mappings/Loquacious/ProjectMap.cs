using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Scrumee.Data.Entities;

namespace Scrumee.Data.Mappings.Loquacious
{
    public class ProjectMap : ClassMapping<Project>
    {
        public ProjectMap()
        {
            Id( x => x.Id );
            Property( x => x.Name );
            Bag( x => x.UserStories, map => { map.Cascade( Cascade.All ); map.Inverse( true ); }, r => r.OneToMany() );
        }
    }
}
