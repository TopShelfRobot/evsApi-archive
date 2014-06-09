//﻿using System.Data.Entity;
//﻿using System.Data.Entity.Migrations;
//﻿using System.Web;
//using System.Web.Http;

using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;
using evs30Api.App_Start;
//﻿using evs;
//﻿using evs.DAL;
//﻿using evs.DAL.Migrations;
//using evs.DAL;
//using evs.Web.App_Start;

namespace evs30Api
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            EntityConfig.RegisterEntity();  //mjb
            //AuthConfig.RegisterAuth();

            //WebSecurity.InitializeDatabaseConnection("evsContext", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            //var migrator = new DbMigrator(new evs.DAL.Migrations.Configuration());
            //migrator.Update();  
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<evsContext, Configuration>());

           
        }
    }
}