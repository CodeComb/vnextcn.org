using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using CodeComb.vNextChina.ViewModels;

namespace CodeComb.vNextChina.Models
{
    public class User : IdentityUser<long>
    {
        public byte[] Avatar { get; set; }

        [MaxLength(32)]
        public string AvatarContentType { get; set; }

        public DateTime RegisteryTime { get; set; }

        [MaxLength(512)]
        public string Motto { get; set; }

        [MaxLength(128)]
        public string Organization { get; set; }

        [MaxLength(128)]
        public string WebSite { get; set; }

        public string ExperimentFlags { get; set; }

        [NotMapped]
        public List<Flag> ExpFlags
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<Flag>>(ExperimentFlags);
                }
                catch
                {
                    return new List<Flag>();
                }
            }
        }
    }
}
