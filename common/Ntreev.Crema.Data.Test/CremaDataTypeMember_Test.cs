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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Random;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using Ntreev.Library.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data.Test
{
    [TestClass]
    public class CremaDataTypeMember_Test
    {
        private readonly string userID;
        private readonly CremaDataType dataType;

        public CremaDataTypeMember_Test()
        {
            this.userID = RandomUtility.NextWord();
            this.dataType = new CremaDataType() { SignatureDateProvider = new SignatureDateProvider(userID) };
            this.dataType.IsFlag = RandomUtility.Within(25);
            this.dataType.Comment = RandomUtility.Within(25) ? string.Empty : RandomUtility.NextString();
            this.dataType.AddRandomMembers(10);
            this.dataType.AcceptChanges();
        }

        [TestMethod]
        public void New()
        {
            var member = this.dataType.NewMember();

            Assert.AreEqual(-1, member.Index);
            Assert.AreEqual(TagInfo.All, member.Tags);
            Assert.AreEqual(true, member.IsEnabled);
            Assert.AreNotEqual(string.Empty, member.Name);
            Assert.AreEqual(string.Empty, member.Comment);
            Assert.AreEqual(string.Empty, member.CreationInfo.ID);
            Assert.AreEqual(DateTime.MinValue, member.CreationInfo.DateTime);
            Assert.AreEqual(DateTime.MinValue, member.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void NewOnLoading()
        {
            this.dataType.BeginLoadData();
            var member = this.dataType.NewMember();

            Assert.AreEqual(-1, member.Index);
            Assert.AreEqual(TagInfo.All, member.Tags);
            Assert.AreEqual(true, member.IsEnabled);
            Assert.AreNotEqual(string.Empty, member.Name);
            Assert.AreEqual(string.Empty, member.Comment);
            Assert.AreEqual(string.Empty, member.CreationInfo.ID);
            Assert.AreEqual(DateTime.MinValue, member.CreationInfo.DateTime);
            Assert.AreEqual(DateTime.MinValue, member.ModificationInfo.DateTime);
        }

        [TestMethod]
        public void SetNullNameToNewMemberOnLoading()
        {
            this.dataType.BeginLoadData();
            var member = this.dataType.NewMember();
            member.Name = RandomUtility.NextInvalidIdentifier();
            this.dataType.Members.Add(member);
        }

        [TestMethod]
        public void SetNullNameToMemberOnLoading()
        {
            this.dataType.BeginLoadData();
            var member = this.dataType.Members.Random();
            member.Name = RandomUtility.NextInvalidIdentifier();
        }

        [TestMethod]
        public void Add()
        {
            var member = this.dataType.AddRandomMember();
            Assert.AreEqual(member.CreationInfo.ID, userID);
            Assert.AreNotEqual(member.CreationInfo.DateTime, DateTime.MinValue);
            Assert.AreEqual(member.CreationInfo, member.ModificationInfo);
        }

        [TestMethod]
        public void Add_Change()
        {
            var member = this.dataType.AddRandomMember();
            var modificationInfo = this.dataType.ModificationInfo;
            Thread.Sleep(100);
            member.SetRandomValue();
            Assert.AreEqual(System.Data.DataRowState.Added, member.ItemState);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        public void Add_Delete()
        {
            var member = this.dataType.AddRandomMember();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Assert.AreEqual(System.Data.DataRowState.Added, member.ItemState);
            Thread.Sleep(100);
            member.Delete();
            Assert.AreEqual(memberCount - 1, this.dataType.Members.Count);
            Assert.AreEqual(System.Data.DataRowState.Detached, member.ItemState);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        public void Add_Remove()
        {
            var member = this.dataType.AddRandomMember();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Assert.AreEqual(System.Data.DataRowState.Added, member.ItemState);
            Thread.Sleep(100);
            this.dataType.Members.Remove(member);
            Assert.AreEqual(memberCount - 1, this.dataType.Members.Count);
            Assert.AreEqual(System.Data.DataRowState.Detached, member.ItemState);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Add()
        {
            var member = this.dataType.AddRandomMember();
            var creationInfo = member.CreationInfo;
            var modificationInfo = member.ModificationInfo;
            this.dataType.Members.Add(member);
            Assert.AreEqual(creationInfo, member.CreationInfo);
            Assert.AreEqual(modificationInfo, member.ModificationInfo);
        }

        [TestMethod]
        public void Delete()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Assert.AreEqual(System.Data.DataRowState.Unchanged, member.ItemState);
            Thread.Sleep(100);
            member.Delete();
            Assert.AreEqual(memberCount - 1, this.dataType.Members.Count);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        public void Delete_Reject()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Thread.Sleep(1000);
            member.Delete();
            member.RejectChanges();
            Assert.AreEqual(memberCount, this.dataType.Members.Count);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > member.ModificationInfo.DateTime);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        public void Delete_Accept()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Thread.Sleep(100);
            member.Delete();
            member.AcceptChanges();
            Assert.AreEqual(memberCount - 1, this.dataType.Members.Count);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Delete_Add_Fail()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            member.Delete();
            this.dataType.Members.Add(member);
        }

        [TestMethod]
        public void Remove()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            Thread.Sleep(100);
            this.dataType.Members.Remove(member);
            Assert.AreEqual(memberCount - 1, this.dataType.Members.Count);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }

        [TestMethod]
        public void Remove_Add()
        {
            var member = this.dataType.Members.Random();
            var memberCount = this.dataType.Members.Count;
            var modificationInfo = this.dataType.ModificationInfo;
            var memberInfo = member.TypeMemberInfo;
            Thread.Sleep(100);
            this.dataType.Members.Remove(member);
            Thread.Sleep(100);
            this.dataType.Members.Add(member);
            Assert.AreEqual(memberCount, this.dataType.Members.Count);
            Assert.AreEqual(this.dataType.ModificationInfo, member.CreationInfo);
            Assert.AreEqual(this.dataType.ModificationInfo, member.ModificationInfo);
            Assert.IsTrue(this.dataType.ModificationInfo.DateTime > modificationInfo.DateTime);
        }
    }
}
