using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using CodeComb.vNextExperimentCenter.Models;
using CodeComb.vNextExperimentCenter.ViewModels;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ContestController : BaseController
    {
        public IActionResult Index()
        {
            ViewBag.InProgress = DB.Contests
                .Where(x => x.Begin <= DateTime.Now && DateTime.Now <= x.End)
                .OrderBy(x => x.End)
                .ToList();
            return PagedView(DB.Contests.Where(x => x.End <= DateTime.Now || DateTime.Now <= x.Begin).OrderByDescending(x => x.Begin), 5);
        }

        [Route("Contest/{id}")]
        public IActionResult Show(string id)
        {
            var ret = DB.Contests
                .Include(x => x.Experiments)
                .ThenInclude(x => x.Experiment)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ret == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var statistics = new List<ContestStatistics>();
            char number = 'A';
            foreach (var x in ret.Experiments.OrderBy(y => y.Point))
            {
                var s = new ContestStatistics
                {
                    Title = x.Contest.Title,
                    Point = x.Point,
                    Submitted = DB.Statuses.Where(y => y.Time >= ret.Begin && y.Time < ret.End).Count(),
                    Accepted = DB.Statuses.Where(y => y.Time >= ret.Begin && y.Time < ret.End && y.Result == StatusResult.Successful).Count(),
                    ExperimentId = x.ExperimentId,
                    Number = number++
                };
                try
                {
                    s.Average = DB.Statuses.Where(y => y.Time >= ret.Begin && y.Time < ret.End).Average(y => y.Accepted * x.Point / y.Total);
                }
                catch
                {
                }
                if (User.IsSignedIn())
                {
                    var last = DB.Statuses.Where(y => y.Time >= ret.Begin && y.Time < ret.End && y.UserId == User.Current.Id).LastOrDefault();
                    if (last == null)
                        s.Flag = "";
                    else if (last.Result == StatusResult.Successful)
                        s.Flag = "Successful";
                    else
                        s.Flag = Convert.ToInt32(last.Accepted * x.Point / last.Total).ToString();
                }
                statistics.Add(s);
            }
            ViewBag.Statistics = statistics;
            return View(ret);
        }
    }
}
