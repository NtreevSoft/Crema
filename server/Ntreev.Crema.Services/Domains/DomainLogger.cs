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
using Ntreev.Crema.Services.Domains.Serializations;
using Ntreev.Library.IO;
using Ntreev.Library.Serialization;
using System.IO;
using System.Xml;

namespace Ntreev.Crema.Services.Domains
{
    class DomainLogger
    {
        public const string HeaderItemPath = "header";
        public const string SourceItemPath = "source";
        public const string PostedItemPath = "posted";
        public const string CompletedItemPath = "completed";

        private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };

        private readonly string basePath;

        private readonly string headerPath;
        private readonly string sourcePath;
        private readonly string postedPath;
        private readonly string completedPath;

        private DomainPostItemSerializationInfo currentPost;
        private IObjectSerializer serializer;
        private readonly DomainPostSerializationInfo postedList;
        private readonly DomainCompleteSerializationInfo completedList;

        public DomainLogger(IObjectSerializer serializer, Domain domain)
        {
            this.serializer = serializer;
            this.basePath = DirectoryUtility.Prepare(domain.Context.BasePath, domain.DataBaseID.ToString(), domain.Name);
            this.headerPath = Path.Combine(this.basePath, HeaderItemPath);
            this.sourcePath = Path.Combine(this.basePath, SourceItemPath);
            this.postedPath = Path.Combine(this.basePath, PostedItemPath);
            this.completedPath = Path.Combine(this.basePath, CompletedItemPath);
            this.serializer.Serialize(this.headerPath, domain.GetSerializationInfo(), ObjectSerializerSettings.Empty);
            this.serializer.Serialize(this.sourcePath, domain.Source, ObjectSerializerSettings.Empty);
            this.postedList = new DomainPostSerializationInfo() { Items = new SerializationItemCollection<DomainPostItemSerializationInfo>() };
            this.completedList = new DomainCompleteSerializationInfo() { Items = new SerializationItemCollection<DomainCompleteItemSerializationInfo>() };
        }

        public DomainLogger(IObjectSerializer serializer, string basePath)
        {
            this.serializer = serializer;
            this.basePath = basePath;
            this.headerPath = Path.Combine(this.basePath, HeaderItemPath);
            this.sourcePath = Path.Combine(this.basePath, SourceItemPath);
            this.postedPath = Path.Combine(this.basePath, PostedItemPath);
            this.completedPath = Path.Combine(this.basePath, CompletedItemPath);
            this.postedList = (DomainPostSerializationInfo)this.serializer.Deserialize(this.postedPath, typeof(DomainPostSerializationInfo), ObjectSerializerSettings.Empty);
            this.completedList = (DomainCompleteSerializationInfo)this.serializer.Deserialize(this.completedPath, typeof(DomainCompleteSerializationInfo), ObjectSerializerSettings.Empty);
        }

        public void Dispose(bool delete)
        {
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

            var id = this.ID++;
            var itemPath = Path.Combine(this.basePath, $"{id}");
            this.currentPost = new DomainPostItemSerializationInfo(id, action.GetType());
            this.postedList.Items.Add(this.currentPost);
            this.serializer.Serialize(itemPath, action, ObjectSerializerSettings.Empty);
            this.serializer.Serialize(this.postedPath, this.postedList, ObjectSerializerSettings.Empty);
        }

        public void Complete()
        {
            if (this.IsEnabled == false)
                return;

            this.completedList.Items.Add(new DomainCompleteItemSerializationInfo(this.currentPost.ID));
            this.serializer.Serialize(this.completedPath, this.completedList, ObjectSerializerSettings.Empty);
        }

        public long ID { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}
