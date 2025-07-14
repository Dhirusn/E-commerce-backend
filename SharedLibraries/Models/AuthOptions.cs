using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraries.Models
{
    public class AuthOptions
    {
        public const string SectionName = "Auth0";
        public string Domain { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
