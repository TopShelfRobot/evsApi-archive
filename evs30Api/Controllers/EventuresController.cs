using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using evs.Model;
using evs.DAL;

namespace evs30Api.Controllers
{
    public class EventuresController : ApiController
    {
        private evsContext db = new evsContext();
       
       // GET api/Eventures
        public IEnumerable<Eventure> GetEventures()
        {
            return db.Eventures.AsEnumerable();
        }

        // GET api/Eventures/5
        public Eventure GetEventure(int id)
        {
            Eventure eventure = db.Eventures.Find(id);
            if (eventure == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return eventure;
        }

        //// GET api/Eventures 
        //public IEnumerable<Eventure> GetAllEventuresByOwnerId(int id)   //mjb moved
        //{
        //    return db.Eventures.Where(e => e.OwnerId == id).OrderByDescending(e => e.Id);
        //}

        // GET api/Eventures
        public IEnumerable<Eventure> GetOpenEventuresByOwnerId(int id)
        {
            DateTime now = DateTime.Now;
            return db.Eventures.Where(e => e.OwnerId == id && (e.DateEventure > now || e.Active ));
        }

        

        // PUT api/Eventures/5
        public HttpResponseMessage PutEventure(int id, Eventure eventure)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != eventure.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(eventure).State = EntityState.Modified;

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

        // POST api/Eventures
        public HttpResponseMessage PostEventure(Eventure eventure)
        {
            if (ModelState.IsValid)
            {
                db.Eventures.Add(eventure);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, eventure);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = eventure.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Eventures/5
        public HttpResponseMessage DeleteEventure(int id)
        {
            Eventure xevent = db.Eventures.Find(id);
            if (xevent == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Eventures.Remove(xevent);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, xevent);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}