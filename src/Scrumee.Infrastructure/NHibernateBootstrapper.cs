using System;
using System.Configuration;
using System.IO;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace Scrumee.Infrastructure
{
    public class NHibernateBootstrapper
    {
        #region Public Properties

        public static ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? ( _sessionFactory = CreateSessionFactory() ); }
        }

        public static NHibernate.Cfg.Configuration Configuration { get; set; }

        #endregion Public Properties
        
        #region Private Fields

        private static ISessionFactory _sessionFactory;

        private const string SqLiteConnectionStringName = "SqliteProjects";

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Open a new NHibenate Session
        /// </summary>
        /// <returns>A new ISession</returns>
        public static ISession OpenSession()
        {
            var session = SessionFactory.OpenSession();

            session.SessionFactory.Statistics.IsStatisticsEnabled = true;

            return session;
        }
        
        #endregion Public Methods

        #region Private Methods

        private static ISessionFactory CreateSessionFactory()
        {
            if ( Configuration == null )
            {
                var mapper = new ModelMapper();

                //// Begin defining mapping conventions ////

                // Append 'Id' to the end of each Identifier and always use the Identity generator ( Auto-incrementing )
                mapper.BeforeMapClass += ( mi, t, map ) => map.Id( x => { x.Column( ( t.Name + "Id" ) ); x.Generator( Generators.Identity ); } );

                // Append 'Id' to the end of the column for each Bag
                mapper.BeforeMapBag += ( mi, t, map ) => map.Key( k => k.Column( t.GetContainerEntity( mi ).Name + "Id" ) );

                // Append 'Id' to the end of the column for each ManyToOne
                mapper.BeforeMapManyToOne += ( mi, t, map ) => map.Column( t.LocalMember.GetPropertyOrFieldType().Name + "Id" );

                //// End defining mapping conventions ////

                mapper.AddMappings( typeof ( Scrumee.Data.Mappings.Loquacious.ProjectMap ).Assembly.GetTypes() );

                var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

                Configuration = new NHibernate.Cfg.Configuration()
                    .DataBaseIntegration( db =>
                                              {
                                                  db.Dialect<SQLiteDialect>();
                                                  db.ConnectionStringName = SqLiteConnectionStringName;
                                                  db.Driver<NHibernate.Driver.SQLite20Driver>();
                                              } );

                Configuration.AddMapping( hbmMapping );
                
                // This is how we would normally add the .HBM mappings to the NHibernate configuration
                //.AddAssembly( typeof( Scrumee.Data.Entities.Entity ).Assembly );

                DropAndRecreateSqliteDatabase();
            }

            var sessionFactory = Configuration.BuildSessionFactory();

            return sessionFactory;
        }

        private static void UpdateSchema()
        {
            new SchemaUpdate( Configuration )
                .Execute( true, true );
        }

        private static void DropAndRecreateSqliteDatabase()
        {
            string path = PathToDatabase( SqLiteConnectionStringName );

            if ( File.Exists( path ) )
                File.Delete( path );

            UpdateSchema();
        }

        private static string PathToDatabase( string connectionStringName )
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ connectionStringName ].ConnectionString;
            
            string pathToAppData = AppDomain.CurrentDomain.GetData( "DataDirectory" ).ToString();

            // Example: "Data Source=|DataDirectory|Projects.db" => "Projects.db"
            string databaseName = connectionString.Replace( "Data Source=|DataDirectory|", "" );

            string fullPath =  Path.Combine( pathToAppData, databaseName );

            return fullPath;
        }

        #endregion Private Methods
    }
}
