using System.Linq;
using System.Web.Mvc;
using evs.DAL;


namespace evs30Api.Controllers
{
    public class HomeController : Controller
    {
        private evsContext db = new evsContext();
        

        public ActionResult Index()
        {

            //db.Events.AsEnumerable();
            using (var ct = new evsContext())
            {
                var query = from p in db.Participants where p.Id == 235 select p;
                var parts = query.ToList();
                //string x = parts.firstna
                foreach(var part in parts)
                {
                    string x = part.FirstName;
                }
                

            foreach (var part in ct.Participants.ToList())
            {
                //Console.WriteLine("{0} Order Count: {1}",
                //  customer.FullName,
                //  customer.Orders.Count());
                string x = part.FirstName.ToString();
            }
            }
            return View();
        }
    }
}
