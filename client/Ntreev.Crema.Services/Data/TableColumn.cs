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

using Ntreev.Crema.Services.Domains;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    class TableColumn : DomainBasedRow, ITableColumn
    {
        private readonly TableTemplateBase template;

        public TableColumn(TableTemplateBase template, DataRow row)
            : base(template.Domain, row)
        {
            this.template = template;
        }

        public TableColumn(TableTemplateBase template, DataTable table)
            : base(template.Domain, table)
        {
            this.template = template;
            var query = from DataRow item in table.Rows
                        where (item.RowState == DataRowState.Deleted || item.RowState == DataRowState.Detached) == false
                        select item.Field<string>(CremaSchema.ColumnName);

            var newName = NameUtility.GenerateNewName("Column", query);
            this.SetField(null, CremaSchema.ColumnName, newName);
        }

        public void SetIndex(Authentication authentication, int value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.SetField(authentication, CremaSchema.Index, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetIsKey(Authentication authentication, bool value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.IsKey, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetIsUnique(Authentication authentication, bool value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.IsUnique, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetName(Authentication authentication, string value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.ColumnName, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetDataType(Authentication authentication, string value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.DataType, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetDefaultValue(Authentication authentication, string value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.DefaultValue, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetComment(Authentication authentication, string value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.SetField(authentication, CremaSchema.Comment, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetAutoIncrement(Authentication authentication, bool value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.SetField(authentication, CremaSchema.AutoIncrement, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetTags(Authentication authentication, TagInfo value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
            this.SetField(authentication, CremaSchema.Tags, value.ToString());
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetIsReadOnly(Authentication authentication, bool value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.ReadOnly, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public void SetAllowNull(Authentication authentication, bool value)
        {
            try
            {
            this.DataBase.ValidateBeginInDataBase(authentication);
                this.SetField(authentication, CremaSchema.AllowNull, value);
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        public int Index
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<int>(CremaSchema.Index);
            }
        }

        public bool IsKey
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<bool>(CremaSchema.IsKey);
            }
        }

        public bool IsUnique
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<bool>(CremaSchema.IsUnique);
            }
        }

        public string Name
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<string>(CremaSchema.ColumnName);
            }
        }

        public string DataType
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<string>(CremaSchema.DataType);
            }
        }

        public string DefaultValue
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<string>(CremaSchema.DefaultValue);
            }
        }

        public string Comment
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<string>(CremaSchema.Comment);
            }
        }

        public bool AutoIncrement
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<bool>(CremaSchema.AutoIncrement);
            }
        }

        public TagInfo Tags
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return (TagInfo)(this.GetField<string>(CremaSchema.Tags));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<bool>(CremaSchema.ReadOnly);
            }
        }

        public bool AllowNull
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.GetField<bool>(CremaSchema.AllowNull);
            }
        }

        public override CremaDispatcher Dispatcher => this.template.Dispatcher;

        public override DataBase DataBase => this.template.DataBase;

        public override CremaHost CremaHost => this.template.CremaHost;

        #region ITableTemplate

        ITableTemplate ITableColumn.Template
        {
            get
            {
                this.Dispatcher?.VerifyAccess();
                return this.template;
            }
        }

        #endregion
    }
}
