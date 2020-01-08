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

using System.ComponentModel.Composition;
using System.Linq;
using GraphQL.Types;
using Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Models;
using Ntreev.Library;

namespace Ntreev.Crema.ServiceHosts.Http.Apis.V1.GQL.Types
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TableType : ObjectGraphType<TableModel>
    {
        public TableType()
        {
            Field(_ => _.Id, type: typeof(IdGraphType)).Name("ID");
            Field(_ => _.Name);
            Field(_ => _.TableName);
            Field(_ => _.Tags);
            Field(_ => _.DerivedTags);
            Field(_ => _.Comment);
            Field(_ => _.TemplatedParent);
            Field(_ => _.ParentName);
            Field(_ => _.CategoryPath);
            Field(_ => _.HashValue);
            Field(_ => _.Revision);
            Field(_ => _.Creator);
            Field(_ => _.CreatedDateTime, type: typeof(DateTimeGraphType));
            Field(_ => _.Modifier);
            Field(_ => _.ModifiedDateTime, type: typeof(DateTimeGraphType));
            Field(_ => _.ContentsModifier);
            Field(_ => _.ContentsModifiedDateTime, type: typeof(DateTimeGraphType));

            Field<ListGraphType<ColumnType>>()
                .Name("Columns")
                .Argument<StringGraphType, string>("name", "", "*")
                .Argument<StringGraphType>("tags", "")
                .Resolve(context =>
                {
                    var name = context.GetArgument<string>("name");
                    var tags = context.GetArgument<string>("tags");

                    var columns = context.Source.Columns
                        .Where(column => column.Name.Glob(name));
                    
                    if (!string.IsNullOrWhiteSpace(tags))
                    {
                        var tagInfo = (TagInfo) tags;
                        return columns
                            .Where(column => ((TagInfo)column.DerivedTags & tagInfo) != TagInfo.Unused)
                            .ToArray();
                    }

                    return columns.ToArray();
                });
        }
    }
}
