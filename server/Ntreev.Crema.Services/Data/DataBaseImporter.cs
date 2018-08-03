using Ntreev.Crema.Data;
using Ntreev.Crema.Data.Xml.Schema;
using Ntreev.Crema.ServiceModel;
using Ntreev.Crema.Services.Properties;
using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Services.Data
{
    partial class DataBase
    {
        public void Import(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            try
            {
                this.Dispatcher?.VerifyAccess();
                this.CremaHost.DebugMethod(authentication, this, nameof(Import), comment);
                this.ValidateImport(authentication, dataSet, comment);
                this.Sign(authentication);
                var filterExpression = string.Join(";", dataSet.Tables);
                var targetSet = this.GetDataSet(authentication, null, filterExpression, ReadTypes.OmitContent);
                this.LockTypes(authentication, targetSet, comment);
                this.LockTables(authentication, targetSet, comment);

                try
                {
                    targetSet.SignatureDateProvider = new SignatureDateProvider(authentication.ID);
                    foreach (var item in targetSet.Tables)
                    {
                        var dataTable = dataSet.Tables[item.Name];
                        foreach (var row in dataTable.Rows)
                        {
                            item.ImportRow(row);
                        }
                        item.BeginLoad();
                        foreach (var row in item.Rows)
                        {
                            row.CreationInfo = authentication.SignatureDate;
                            row.ModificationInfo = authentication.SignatureDate;
                        }
                        item.ContentsInfo = authentication.SignatureDate;
                        item.EndLoad();
                    }
                    try
                    {
                        this.Repository.Modify(targetSet);
                        this.Repository.Commit(authentication, comment);
                    }
                    catch
                    {
                        this.Repository.Revert();
                        throw;
                    }

                    this.UpdateTables(authentication, targetSet);
                }
                finally
                {
                    this.UnlockTypes(authentication, targetSet);
                    this.UnlockTables(authentication, targetSet);
                }
            }
            catch (Exception e)
            {
                this.CremaHost.Error(e);
                throw;
            }
        }

        private void ValidateImport(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            if (dataSet == null)
                throw new ArgumentNullException(nameof(dataSet));
            //if (dataSet.Tables.Any() == false)
            //    throw new ArgumentException(Resources.Exception_EmptyDataSetCannotImport, nameof(dataSet));
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));
            if (comment == string.Empty)
                throw new ArgumentException(Resources.Exception_EmptyStringIsNotAllowed);

            if (this.IsLoaded == false)
                throw new InvalidOperationException(Resources.Exception_DataBaseHasNotBeenLoaded);

            this.ValidateAccessType(authentication, AccessType.Editor);

            var tableList = new List<Table>(dataSet.Tables.Count);
            foreach (var item in dataSet.Tables)
            {
                var table = this.TableContext.Tables[item.Name];
                if (table == null)
                    throw new TableNotFoundException(item.Name);
                tableList.Add(table);
                table.ValidateAccessType(authentication, AccessType.Editor);
                table.ValidateHasNotBeingEditedType();
                table.ValidateNotBeingEdited();
            }

            var query = from item in tableList
                        where item.LockInfo.Path != item.Path
                        select item;

            foreach (var item in query)
            {
                item.ValidateLockInternal(authentication);
            }
        }

        private void LockTypes(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            Authentication.System.Sign(authentication.SignatureDate.DateTime);
            var query = from item in dataSet.Types
                        let type = this.TypeContext.Types[item.Name]
                        where type.LockInfo.Path != type.Path
                        select type;

            var items = query.ToArray();
            var comments = Enumerable.Repeat(comment, items.Length).ToArray();
            foreach (var item in items)
            {
                item.LockInternal(Authentication.System, comment);
                dataSet.Types[item.Name].ExtendedProperties[this] = true;
            }
            this.TypeContext.InvokeItemsLockedEvent(authentication, items, comments);
        }

        private void LockTables(Authentication authentication, CremaDataSet dataSet, string comment)
        {
            Authentication.System.Sign(authentication.SignatureDate.DateTime);
            var query = from item in dataSet.Tables
                        let table = this.TableContext.Tables[item.Name]
                        where table.LockInfo.Path != table.Path
                        select table;

            var items = query.ToArray();
            var comments = Enumerable.Repeat(comment, items.Length).ToArray();
            foreach (var item in items)
            {
                item.LockInternal(Authentication.System, comment);
                dataSet.Tables[item.Name].ExtendedProperties[this] = true;
            }
            this.TableContext.InvokeItemsLockedEvent(Authentication.System, items, comments);
        }

        private void UnlockTypes(Authentication authentication, CremaDataSet dataSet)
        {
            Authentication.System.Sign(authentication.SignatureDate.DateTime);
            var query = from item in dataSet.Types
                        where item.ExtendedProperties.Contains(this)
                        select this.TypeContext.Types[item.Name];

            var items = query.ToArray();
            foreach (var item in items)
            {
                item.UnlockInternal(Authentication.System);
            }
            this.TypeContext.InvokeItemsUnlockedEvent(Authentication.System, items);
        }

        private void UnlockTables(Authentication authentication, CremaDataSet dataSet)
        {
            Authentication.System.Sign(authentication.SignatureDate.DateTime);
            var query = from item in dataSet.Tables
                        where item.ExtendedProperties.Contains(this)
                        select this.TableContext.Tables[item.Name];

            var items = query.ToArray();
            foreach (var item in items)
            {
                item.UnlockInternal(Authentication.System);
            }
            this.TableContext.InvokeItemsUnlockedEvent(Authentication.System, items);
        }

        private void UpdateTables(Authentication authentication, CremaDataSet dataSet)
        {
            var tableList = new List<Table>(dataSet.Tables.Count);
            foreach (var item in dataSet.Tables)
            {
                var table = this.TableContext.Tables[item.Name];
                tableList.Add(table);
                table.UpdateContent(item.TableInfo);
            }
            this.TableContext.InvokeItemsChangedEvent(authentication, tableList.ToArray(), dataSet);
        }
    }
}
