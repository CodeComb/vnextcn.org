﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ForumController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}