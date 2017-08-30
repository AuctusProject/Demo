using Auctus.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Auctus.Web.Model.Home
{
    public class Wizard
    {
        public Fund Fund { get; set; }
        public Company Company { get; set; }
        public Employee Employee { get; set; }
    }
}

