//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Domains.Actions;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ntreev.Crema.Services.Domains
{
    class DomainLogger
    {
        public const string HeaderFileName = "info.dat";
        public const string PostedFileName = "posted.log";
        public const string CompletedFileName = "completed.log";
        //public const string UsersFileName = "users.xml";

        private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };

        private string basePath;

        private StreamWriter postedWriter;
        private StreamWriter completedWriter;
        private DomainActionBase current;

        public DomainLogger(Domain domain)
        {
            this.Initialize(domain);
        }

        public DomainLogger(string basePath)
        {
            this.basePath = basePath;

            this.postedWriter = new StreamWriter(Path.Combine(basePath, DomainLogger.PostedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };

            this.completedWriter = new StreamWriter(Path.Combine(basePath, DomainLogger.CompletedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
        }

        public void Initialize(Domain domain)
        {
            this.basePath = DirectoryUtility.Prepare(domain.Context.BasePath, domain.DataBaseID.ToString(), domain.Name);

            domain.Write(Path.Combine(basePath, DomainLogger.HeaderFileName));

            this.postedWriter = new StreamWriter(Path.Combine(basePath, DomainLogger.PostedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };

            this.completedWriter = new StreamWriter(Path.Combine(basePath, DomainLogger.CompletedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
        }

        public void Dispose(bool delete)
        {
            this.postedWriter?.Close();
            this.postedWriter = null;

            this.completedWriter?.Close();
            this.completedWriter = null;

            if (delete == true)
            {
                DirectoryUtility.Delete(this.basePath);
            }
        }

        public void NewRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.IsEnabled == false)
                return;

            this.Post(new NewRowAction()
            {
                UserID = authentication.ID,
                Rows = rows,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void SetRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.IsEnabled == false)
                return;

            this.Post(new SetRowAction()
            {
                UserID = authentication.ID,
                Rows = rows,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void RemoveRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.IsEnabled == false)
                return;

            this.Post(new RemoveRowAction()
            {
                UserID = authentication.ID,
                Rows = rows,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void SetProperty(Authentication authentication, string propertyName, object value)
        {
            if (this.IsEnabled == false)
                return;

            this.Post(new SetPropertyAction()
            {
                UserID = authentication.ID,
                PropertyName = propertyName,
                Value = value,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void Join(Authentication authentication, DomainAccessType accessType)
        {
            this.Post(new JoinAction()
            {
                UserID = authentication.ID,
                AccessType = accessType,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void Disjoin(Authentication authentication, RemoveInfo removeInfo)
        {
            this.Post(new DisjoinAction()
            {
                UserID = authentication.ID,
                RemoveInfo = removeInfo,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void Kick(Authentication authentication, string userID, string comment)
        {
            this.Post(new KickAction()
            {
                UserID = authentication.ID,
                TargetID = userID,
                Comment = comment,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void SetOwner(Authentication authentication, string userID)
        {
            this.Post(new SetOwnerAction()
            {
                UserID = authentication.ID,
                TargetID = userID,
                AcceptTime = authentication.SignatureDate.DateTime
            });
        }

        public void Post(DomainActionBase action)
        {
            if (this.IsEnabled == false)
                return;

            var message = string.Empty;

            this.current = action;
            action.ID = this.ID++;

            var s = XmlSerializerUtility.GetSerializer(action.GetType());

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            ns.Add("fn", action.GetType().AssemblyQualifiedName);

            using (var sw = new Utf8StringWriter())
            using (var writer = XmlWriter.Create(sw, writerSettings))
            {
                DataContractSerializerUtility.Write(writer, action);
                writer.Close();
                message = sw.ToString();
            }

            this.postedWriter.WriteLine(message);
        }

        public void Complete()
        {
            if (this.IsEnabled == false)
                return;

            var message = string.Empty;
            using (var sw = new Utf8StringWriter())
            using (var writer = XmlWriter.Create(sw, writerSettings))
            {
                writer.WriteStartElement("Action");
                {
                    writer.WriteAttributeString("ID", this.current.ID.ToString());
                    writer.WriteAttributeString("DateTime", DateTime.Now.ToString("o"));
                }
                writer.WriteEndElement();
                writer.Close();
                message = sw.ToString();
            }

            this.completedWriter.WriteLine(message);
            this.current = null;
        }

        public long ID { get; set; }

        public bool IsEnabled { get; set; } = true;

        private void SerializeDomain(Domain domain)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(Path.Combine(basePath, DomainLogger.HeaderFileName), FileMode.Create))
            {
                formatter.Serialize(stream, domain);
                stream.Close();
            }
        }
    }
}
