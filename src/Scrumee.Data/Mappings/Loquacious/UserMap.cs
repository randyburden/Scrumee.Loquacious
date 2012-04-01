using NHibernate.Mapping.ByCode.Conformist;
using Scrumee.Data.Entities;

namespace Scrumee.Data.Mappings.Loquacious
{
    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Id( x => x.Id );
            Property( x => x.FirstName );
            Property( x => x.LastName );
        }
    }
}
