using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

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
    }
}
