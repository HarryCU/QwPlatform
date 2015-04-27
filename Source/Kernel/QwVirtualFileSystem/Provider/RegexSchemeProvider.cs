using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QwVirtualFileSystem.Provider
{
    public abstract class RegexSchemeProvider : AbstractSchemeProvider
    {
        public readonly Regex RegexPatten;

        protected RegexSchemeProvider(string scheme, string patten)
            : base(scheme)
        {
            RegexPatten = new Regex(patten, RegexOptions.IgnoreCase);
        }

        protected override bool BeforeBuildFolder(string path)
        {
            return RegexPatten.IsMatch(string.Concat(Scheme, path));
        }

        protected RegexMatchItemTable Match(string path)
        {
            var table = new RegexMatchItemTable();
            var match = RegexPatten.Match(string.Concat(Scheme, path));
            var groupNames = RegexPatten.GetGroupNames();
            for (int i = 1; i < groupNames.Length; i++)
            {
                var group = match.Groups[i];
                table.Add(groupNames[i], group.Value);
            }
            return table;
        }
    }
}
