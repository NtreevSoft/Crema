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

using Ntreev.Crema.Services;
using Ntreev.Crema.ServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using Ntreev.Crema.Data;
using Ntreev.Library.Random;

namespace Ntreev.Crema.Services.Random
{
    public static class CremaTypeCreator
    {
        public static void CreateNormalType(CremaDataSet dataSet, string typeName, string categoryPath)
        {
            var dataType = dataSet.Types.Add(typeName, categoryPath);

            for (var i = 0; i < 128; i++)
            {
                dataType.AddMember($"{typeName}_member_{i}", i, RandomUtility.NextString());
            }
        }

        public static void CreateFlagType(CremaDataSet dataSet, string typeName, string categoryPath)
        {
            var dataType = dataSet.Types.Add(typeName, categoryPath);
            dataType.IsFlag = true;

            dataType.AddMember("none", 0);
            for (var i = 0; i < 32; i++)
            {
                dataType.AddMember($"{typeName}_flag_{i}", (long)0x1 << i, RandomUtility.NextString());
            }
            dataType.AddMember("all", -1);
        }

        public static void CreateRandomType(CremaDataSet dataSet, string typeName, string categoryPath)
        {
            var dataType = dataSet.Types.Add(typeName, categoryPath);
            dataType.IsFlag = RandomUtility.NextBoolean();

            if (dataType.IsFlag == true)
            {
                for (var i = 0; i < RandomUtility.Next(1, 128); i++)
                {
                    dataType.AddMember($"{typeName}_member_{RandomUtility.NextIdentifier()}", i, RandomUtility.NextString());
                }
            }
            else
            {
                dataType.AddMember("none", 0);
                for (var i = 0; i < RandomUtility.Next(32); i++)
                {
                    dataType.AddMember($"{typeName}_flag_{RandomUtility.NextIdentifier()}", (long)0x1 << RandomUtility.Next(32), RandomUtility.NextString());
                }
                dataType.AddMember("all", -1);
            }
        }
    }
}