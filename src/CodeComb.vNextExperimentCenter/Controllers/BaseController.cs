using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CodeComb.Net.EmailSender;
using CodeComb.Security.Aes;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class BaseController : BaseController<CenterContext, User, long>
    {
        [FromServices]
        public IEmailSender Mail { get; set; }
        
        [FromServices]
        public AesCrypto Aes { get; set; }
    }
}
