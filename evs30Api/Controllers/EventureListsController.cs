using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using evs.Model;
using evs.DAL;

namespace evs30Api.Controllers
{
    public class EventureListsController : ApiController
    {
        private evsContext db = new evsContext();

        // GET api/EventLists
        public IEnumerable<EventureList> GetEventLists()
        {
            //var eventlists = db.EventLists.Include(e => e.Event);
            //return eventlists.AsEnumerable();
            return db.EventureLists.ToArray();
        }

        // GET api/EventLists/5
        public EventureList GetEventList(int id)
        {
            EventureList eventlist = db.EventureLists.Find(id);
            if (eventlist == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return eventlist;
        }

        
        //GetEventureListsByEventureId
        public IEnumerable<EventureList> GetEventureListsByEventureId(int id)
        {
            return db.EventureLists.Where(e => e.EventureId == id);
        }
        //used id instead of ownerID so i can use same route since i hope to replace this soon anyway
        public IEnumerable<EventureList> GetEventureListsByOwnerId(int id)
        {
            var query = db.Eventures.Where(e => e.OwnerId == id).Select(e => e.Id);
            return db.EventureLists.Where(el => query.Contains(el.EventureId));
        }

        // PUT api/EventLists/5
        public HttpResponseMessage PutEventList(int id, EventureList eventlist)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != eventlist.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(eventlist).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/EventLists
        public HttpResponseMessage PostEventList(EventureList eventlist)
        {
            if (ModelState.IsValid)
            {
                db.EventureLists.Add(eventlist);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, eventlist);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = eventlist.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/EventLists/5
        public HttpResponseMessage DeleteEventList(int id)
        {
            EventureList eventlist = db.EventureLists.Find(id);
            if (eventlist == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.EventureLists.Remove(eventlist);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, eventlist);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}