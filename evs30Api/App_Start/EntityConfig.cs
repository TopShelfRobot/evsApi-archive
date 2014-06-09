//using System;
//using System.Configuration;
//using System.Data.Entity;
//using evs.DAL.Migrations;
//using evs.DAL;

namespace evs30Api.App_Start
{
    public class EntityConfig
    {
        public static void RegisterEntity()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<evsContext,Configuration>());
            //System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<evs.DAL.evsContext);  //
        }
    }
}