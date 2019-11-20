using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Ntreev.Library;

namespace Ntreev.Crema.SvnModule
{
    struct SvnRevisionInfoEventArgs
    {
        public string Name { get; private set; }
        public long Revision { get; private set; }

        public static SvnRevisionInfoEventArgs[] Run(string path, long revision = -1)
        {
            var revisionString = revision == -1 ? "head" : revision.ToString();

            var text = SvnClientHost.Run("list", "-R", "-r", revisionString, "--xml", path.WrapQuot());
            return Parse(text);
        }

        private static SvnRevisionInfoEventArgs[] Parse(string text)
        {
            var list = new List<SvnRevisionInfoEventArgs>(100);
            using (var reader = new StringReader(text))
            {
                var doc = XDocument.Load(reader);
                foreach (var element in doc.XPathSelectElements("/lists/list/entry[@kind='file']"))
                {
                    var name = element.Element("name")?.Value;
                    var revision = long.Parse(element.Element("commit")?.Attribute("revision")?.Value ?? "0");

                    list.Add(new SvnRevisionInfoEventArgs
                    {
                        Name = name,
                        Revision = revision
                    });
                }
            }

            return list.ToArray();
        }
    }
}
