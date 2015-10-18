using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class BaseController : BaseController<CenterContext, User, long>
    {
    }
}
