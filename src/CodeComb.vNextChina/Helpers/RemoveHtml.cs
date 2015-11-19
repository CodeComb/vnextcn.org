using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeComb.vNextChina.Helpers
{
    public static class RemoveHtml
    {
        public static string Remove(string html)
        {
            try
            {
                html = Regex.Replace(html, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"-->", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"<!--.*", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                html = Regex.Replace(html, @"<img[^>]*>;", "", RegexOptions.IgnoreCase);
                html = html.Replace("<", "");
                html = html.Replace(">", "");
                html = html.Replace("\r\n", "");
                return html;
            }
            catch
            {
                return "...";
            }
        }
    }
}
