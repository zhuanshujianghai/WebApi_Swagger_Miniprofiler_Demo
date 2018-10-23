using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi_Swagger_MiniProfiler_Demo.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            using (face_dbEntities db = new face_dbEntities())
            {
                var ss = Json(db.banner.ToList());
                return Json(ss,JsonRequestBehavior.AllowGet);
            }
        }
    }
}