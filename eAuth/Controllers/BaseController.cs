using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAuth.Controllers
{
    public class BaseController : Controller
    {
        protected static ILog logger = LogManager.GetLogger("Web.Controller");
    }
}
