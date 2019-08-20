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
using System.Threading.Tasks;
using Ntreev.Crema.Runtime.Generation;
using Ntreev.Crema.Runtime.Serialization;

namespace Ntreev.Crema.Commands.OptionProcessor
{
    static class ReplaceOptionProcessor
    {
        public static Tuple<GenerationSet, SerializationSet> Process(Tuple<GenerationSet, SerializationSet> metaData)
        {
            var generationSet = metaData.Item1;
            var serializationSet = metaData.Item2;

            generationSet = Process(generationSet);
            serializationSet = Process(serializationSet);

            return new Tuple<GenerationSet, SerializationSet>(generationSet, serializationSet);
        }

        public static GenerationSet Process(GenerationSet generationSet)
        {
            if (ReplaceSettings.ReplaceRevision != ReplaceSettings.REPLACE_REVISION_DEFAULT_VALUE)
            {
                generationSet.Revision = ReplaceSettings.ReplaceRevision;
            }

            if (ReplaceSettings.ReplaceHashValue != ReplaceSettings.REPLACE_HASH_VALUE_DEFAULT_VALUE)
            {
                generationSet.TablesHashValue = ReplaceSettings.ReplaceHashValue;
                generationSet.TypesHashValue = ReplaceSettings.ReplaceHashValue;
            }

            return generationSet;
        }

        public static SerializationSet Process(SerializationSet serializationSet)
        {
            if (ReplaceSettings.ReplaceRevision != ReplaceSettings.REPLACE_REVISION_DEFAULT_VALUE)
            {
                serializationSet.Revision = ReplaceSettings.ReplaceRevision;
            }

            if (ReplaceSettings.ReplaceHashValue != ReplaceSettings.REPLACE_HASH_VALUE_DEFAULT_VALUE)
            {
                serializationSet.TablesHashValue = ReplaceSettings.ReplaceHashValue;
                serializationSet.TypesHashValue = ReplaceSettings.ReplaceHashValue;
            }

            return serializationSet;
        }
    }
}
