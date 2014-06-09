using evs.DAL;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using evs.Model;
//using System.Web.Mvc;

namespace evs30Api.Controllers
{
    public class ResourcesController : ApiController
    {
        evsContext db = new evsContext();

        public IEnumerable<Resource> GetResourcesByOwnerId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            return db.Resources.Where(v => v.OwnerId == id);
        }

        public object GetExpensesByEventureId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
            //return db.Expenses.Where(e => e.EventureId == id);
            return db.Expenses.Where(e => e.EventureId == id).Select(e => new { e.Id, e.Cost, e.CostType, e.PerRegNumber, item = e.ResourceItem.Name,category = e.ResourceItemCategory.Name });   //category = e.ItemCategory.Name
        }

        //public IEnumerable<EventurePlanItem> GetNotificationsOpenByEventureId(int id)
        //{
        //    //return db.Participants.Where(p => p.OwnerId == id);
        //    return db.PlanItems.Where(n => n.EventureId == id);
        //}

        public object GetNotificationsByOwnerId(int id)
        {
            //return db.Participants.Where(p => p.OwnerId == id);
           var resource =  from p in db.PlanItems
                join r in db.Resources
                   on p.ResourceId equals r.Id
                join e in db.Eventures
                   on p.EventureId equals e.Id
                where e.OwnerId == id
                orderby p.DateDue  
                select new { Task = p.Task, DateDue = p.DateDue, Resource = r.Name, Eventure = e.DisplayHeading };

            return resource.ToList();
        }

        public object GetNotificationsByEventureId(int id)
        {
            var resource = from p in db.PlanItems
                           join r in db.Resources
                           on p.ResourceId equals r.Id
                           //join e in db.Eventures
                           //on p.EventureId equals e.Id
                           where p.EventureId == id
                           orderby p.DateDue 
                           select new { Id = p.Id, Task = p.Task, DateDue = p.DateDue, Resource = r.Name, IsCompleted = p.IsCompleted };

            return resource.ToList();
        }

        //public IEnumerable<Resource> GetResourceById(int id)
        //{
        //    return db.Resources.Where(v => v.Id == id);
        //}

        //public IEnumerable<ResourceService> GetResourceServicesByOwnerId(int id)
        //{
        //    //lazy #2  only should be querying by owner id
        //    return db.ResourceServices;
        //}

        //public IEnumerable<ResourceItem> GetResourceItemsByOwnerId(int id)
        public object GetResourceItemsByOwnerId(int id)
        {
            //this needs to check by ownerid  //mjb
            var x = db.ResourceItems.Select(i => new { i.Id, i.Name, i.Cost, Category = i.ResourceItemCategory.Name, i.ResourceId });
            return x;
        }


        public object GetResourceItemsByResourceId(int id)
        {
            return db.ResourceItems.Where(i => i.ResourceId == id).Select(i => new { i.Id, i.Name, i.Cost, Category = i.ResourceItemCategory.Name, i.ResourceId });
        }

        //public IEnumerable<ResourceService> GetResourceServicesByResourceId(int id)
        //{
        //    //lazy #2  only should be querying by owner id
        //    return db.ResourceServices.Where(v => v.ResourceId == id);
        //}

        public IEnumerable<EventureService> GetEventureServiceByEventureId(int id)
        {
            //lazy #2  only should be querying by owner id
            return db.EventureServices.Where(s => s.EventureId == id);
        }


    }
}
