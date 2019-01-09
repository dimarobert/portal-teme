using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PortalTeme.Services {
    public interface IUrlSlugService {
        string TransformText(string text);
    }

    public class UrlSlugService : IUrlSlugService {

        private Regex replacer;
        private readonly Regex cleaner;
        private const string spacer = "-";
        public UrlSlugService() {
            replacer = new Regex(@"[^\w0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            cleaner = new Regex($"{spacer}{spacer}*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public string TransformText(string text) {
            var slug = text.ToLower();

            slug = replacer.Replace(slug, spacer);
            slug = cleaner.Replace(slug, spacer);

            return slug;
        }
    }
}
