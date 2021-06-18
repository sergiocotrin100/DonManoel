using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DonManoel.Controllers
{
    public class MainController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                ViewData["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch
            {
                ViewData["version"] = DateTime.Now.Ticks.ToString();
            }

            base.OnActionExecuting(filterContext);

        }
    }
}
