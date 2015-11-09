using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using CodeComb.Net.EmailSender;
using CodeComb.Security.Aes;
using CodeComb.vNextChina.Models;
using CodeComb.vNextChina.Hub;
using CodeComb.vNextChina.Hubs;

namespace CodeComb.vNextChina.Controllers
{
    public class BaseController : BaseController<vNextChinaContext, User, long>
    {
        [FromServices]
        public INodeProvider NodeProvider { get; set; }

        [FromServices]
        public IEmailSender Mail { get; set; }
        
        [FromServices]
        public AesCrypto Aes { get; set; }
        
        [FromServices]
        public new AspNet.Extensions.SmartUser.SmartUser<User, long> User { get; set; }

        [FromServices]
        public IHubContext<vNextChinaHub> vNextChinaHub { get; set; }

        public override void Prepare()
        {
            ViewBag.Nodes = NodeProvider.Nodes;
            ViewBag.Announcements = DB.Topics
                .Where(x => x.IsAnnouncement)
                .OrderByDescending(x => x.CreationTime)
                .Take(5)
                .ToList();
        }
    }
}
