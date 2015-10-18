using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class User : IdentityUser<long>
    {
        public byte[] Avatar { get; set; }
    }
}
