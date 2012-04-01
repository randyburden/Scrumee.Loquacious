using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Scrumee.Data.Entities;

namespace Scrumee.Data.Mappings.Loquacious
{
    public class UserStoryMap : ClassMapping<UserStory>
    {
        public UserStoryMap()
        {
            Id( x => x.Id );
            Property( x => x.Name );
            Bag( x => x.Tasks, map => { map.Inverse( true ); map.Cascade( Cascade.All ); }, m => m.OneToMany() );
            ManyToOne( x => x.Project );
        }
    }
}
