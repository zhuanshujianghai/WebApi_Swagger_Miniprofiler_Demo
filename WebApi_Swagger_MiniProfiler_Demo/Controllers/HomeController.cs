using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApi_Swagger_MiniProfiler_Demo.Controllers
{
    [RoutePrefix("api/Home")]
    public class HomeController : ApiController
    {
        public IHttpActionResult Index()
        {
            
            using (face_dbEntities db = new face_dbEntities())
            {
                List<banner> listbanner = new List<banner>();
                listbanner = db.banner.ToList();
                return Json(listbanner);
            }
            
        }
        public string Index1()
        {
            return "";
        }
    }
}
