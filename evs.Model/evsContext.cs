using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace evs.Model
{
    public class evsContext : DbContext
    {
        //public TourismContext() : base("name=TourismContext")
        //{
        //}
        static evsContext()
        {
            Database.SetInitializer<evsContext>(null);
        }


        public DbSet<EventList> EventLists { get; set; }
        public DbSet<Event> Events { get; set; }
        //public DbSet<Registration> Regs { get; set; }
        //public DbSet<Participant> Parts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
            //modelBuilder.Configurations.Add(new CustomerMap());
            //modelBuilder.Configurations.Add(new LineItemMap());
            //modelBuilder.Configurations.Add(new OrderMap());
            //modelBuilder.Configurations.Add(new ProductMap());
            //modelBuilder.Configurations.Add(new sysdiagramMap());
            //modelBuilder.Configurations.Add(new custviewMap());
            //modelBuilder.Configurations.Add(new vSalesOrderDetailMap());
        }

    }
}
