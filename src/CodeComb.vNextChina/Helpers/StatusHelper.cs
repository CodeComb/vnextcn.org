using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor;
using CodeComb.vNextChina.Models;

namespace Microsoft.AspNet.Mvc.Rendering
{
    public static class StatusHelper
    {
        public static HtmlString ColorStatus(this IHtmlHelper self, StatusResult result)
        {
            switch(result)
            {
                case StatusResult.Building:
                    return new HtmlString("<span class=\"status-building\">Building</span>");
                case StatusResult.Queued:
                    return new HtmlString("<span class=\"status-queued\">Queued</span>");
                case StatusResult.Failed:
                    return new HtmlString("<span class=\"status-failed\">Failed</span>");
                case StatusResult.Successful:
                    return new HtmlString("<span class=\"status-successful\">Successful</span>");
                case StatusResult.Ignored:
                    return new HtmlString("<span class=\"status-ignored\">Ignored</span>");
                default:
                    return new HtmlString("<span>Unknown</span>");
            }
        }

        public static HtmlString ColorFlag(this IHtmlHelper self, StatusResult result)
        {
            if (result == StatusResult.Successful)
                return new HtmlString("<span class=\"status-successful\">Passed</span>");
            else
                return new HtmlString("<span class=\"status-failed\">Failed</span>");
        }
    }
}
