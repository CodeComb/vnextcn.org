using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using CodeComb.Net.EmailSender;
using CodeComb.Security.Aes;
using CodeComb.vNextChina.Models;
using CodeComb.vNextChina.Hub;
using CodeComb.vNextChina.Hubs;

namespace CodeComb.vNextChina.Controllers
{
    public class BaseController : BaseController<vNextChinaContext, User, long>
    {
        [Inject]
        public INodeProvider NodeProvider { get; set; }

        [Inject]
        public IEmailSender Mail { get; set; }
        
        [Inject]
        public AesCrypto Aes { get; set; }
        
        [Inject]
        public new AspNet.Extensions.SmartUser.SmartUser<User, long> User { get; set; }

        [Inject]
        public IHubContext<vNextChinaHub> vNextChinaHub { get; set; }

        public override void Prepare()
        {
            ViewBag.Nodes = NodeProvider.Nodes;
            ViewBag.Announcements = DB.Threads
                .Where(x => x.IsAnnouncement)
                .OrderByDescending(x => x.CreationTime)
                .Take(5)
                .ToList();
        }
    }
}
