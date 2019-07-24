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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Ntreev.Crema.Services;
using Ntreev.Crema.Data.Xml;
using Ntreev.Crema.ServiceModel;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using Ntreev.Crema.Services.Domains.Actions;
using Ntreev.Library;

namespace Ntreev.Crema.Services.Domains
{
    class DomainLogger
    {
        public const string HeaderFileName = "info.dat";
        public const string PostedFileName = "posted.log";
        public const string CompletedFileName = "completed.log";
        public const string UsersFileName = "users.xml";

        private static XmlWriterSettings writerSettings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };

        private string workingPath;

        private StreamWriter postedWriter;
        private StreamWriter completedWriter;
        private long id;
        private DomainActionBase current;
        private bool isEnabled = true;

        public DomainLogger(Domain domain)
        {
            this.Initialize(domain);
        }

        public DomainLogger(string workingPath)
        {
            this.workingPath = workingPath;

            this.postedWriter = new StreamWriter(Path.Combine(workingPath, DomainLogger.PostedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };

            this.completedWriter = new StreamWriter(Path.Combine(workingPath, DomainLogger.CompletedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
        }

        public void Initialize(Domain domain)
        {
            this.workingPath = DirectoryUtility.Prepare(domain.Context.BasePath, domain.DataBaseID.ToString(), domain.Name);

            domain.Write(Path.Combine(workingPath, DomainLogger.HeaderFileName));

            this.postedWriter = new StreamWriter(Path.Combine(workingPath, DomainLogger.PostedFileName), true, Encoding.UTF8)
            {
                AutoFlush = true,
            };

            this.completedWriter = new StreamWriter(Path.Combine(workingPath, DomainLogger.CompletedFileName), true, Encoding.UTF8)
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
                DirectoryUtility.Delete(this.workingPath);
            }
        }

        public void NewRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.isEnabled == false)
                return;

            var action = new NewRowAction() { UserID = authentication.ID, Rows = rows, AcceptTime = authentication.SignatureDate.DateTime };
            this.Post(action);
        }

        public void SetRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.isEnabled == false)
                return;

            var action = new SetRowAction() { UserID = authentication.ID, Rows = rows, AcceptTime = authentication.SignatureDate.DateTime };
            this.Post(action);
        }

        public void RemoveRow(Authentication authentication, DomainRowInfo[] rows)
        {
            if (this.isEnabled == false)
                return;

            var action = new RemoveRowAction() { UserID = authentication.ID, Rows = rows, AcceptTime = authentication.SignatureDate.DateTime };
            this.Post(action);
        }

        public void SetProperty(Authentication authentication, string propertyName, object value)
        {
            if (this.isEnabled == false)
                return;

            var action = new SetPropertyAction() { UserID = authentication.ID, PropertyName = propertyName, Value = value, AcceptTime = authentication.SignatureDate.DateTime };
            this.Post(action);
        }

        public void Join(Authentication authentication, DomainAccessType accessType)
        {
            var action = new JoinAction() { UserID = authentication.ID, AccessType = accessType, AcceptTime = authentication.SignatureDate.DateTime};
            this.Post(action);
        }

        public void Disjoin(Authentication authentication, RemoveInfo removeInfo)
        {
            var action = new DisjoinAction() { UserID = authentication.ID, RemoveInfo = removeInfo, AcceptTime = authentication.SignatureDate.DateTime };
            this.Post(action);
        }

        public void Kick(Authentication authentication, string userID, Guid token, string comment)
        {
            var action = new KickAction()
            {
                UserID = authentication.ID,
                TargetID = userID,
                TargetToken = token,
                Comment = comment,
                AcceptTime = authentication.SignatureDate.DateTime
            };
            this.Post(action);
        }

        public void SetOwner(Authentication authentication, string userID, Guid token)
        {
            var action = new SetOwnerAction()
            {
                UserID = authentication.ID,
                TargetID = userID,
                TargetToken = token,
                AcceptTime = authentication.SignatureDate.DateTime
            };
            this.Post(action);
        }

        public void Post(DomainActionBase action)
        {
            if (this.isEnabled == false)
                return;

            var message = string.Empty;

            this.current = action;
            action.ID = this.id++;

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
            if (this.isEnabled == false)
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

        public long ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.isEnabled = value; }
        }

        private void SerializeDomain(Domain domain)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(Path.Combine(workingPath, DomainLogger.HeaderFileName), FileMode.Create))
            {
                formatter.Serialize(stream, domain);
                stream.Close();
            }
        }
    }
}
