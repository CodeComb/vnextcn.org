using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextChina.Models;
using CodeComb.vNextChina.ViewModels;
using Newtonsoft.Json;

namespace CodeComb.vNextChina.Helpers
{
    public static class FlagBuilder
    {
        public static Task<string> BuildAsync(vNextChinaContext DB, long UserId)
        {
            return Task.Factory.StartNew(() =>
            {
                var tmp = DB.Statuses
                    .Where(x => x.UserId == UserId && x.ExperimentId != null)
                    .OrderByDescending(x => x.Time)
                    .Select(x => new Flag { Id = x.ExperimentId.Value, Status = x.Result == StatusResult.Successful ? StatusResult.Successful : StatusResult.Failed })
                    .OrderBy(x => x.Status)
                    .DistinctBy(x => x.Id)
                    .ToList();
                var json = JsonConvert.SerializeObject(tmp);
                var user = DB.Users.Where(x => x.Id == UserId).Single();
                user.ExperimentFlags = json;
                DB.SaveChanges();
                return json;
            });
        }
    }
}
