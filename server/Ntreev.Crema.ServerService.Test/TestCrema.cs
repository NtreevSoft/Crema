using Ntreev.Crema.SvnModule;
using Ntreev.Library.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.ServerService.Test
{
    public static class TestCrema
    {
        public static ICremaHost CreateInstance(string tempPath)
        {
            var path = PathUtility.GetTempPath(tempPath, false);
            DirectoryUtility.Prepare(path);
            SvnCrema.CreateRepository(path);
            //return SvnCrema.CreateInstance(path);
            throw new Exception();
        }

        public static ICremaHost GetInstance(string path)
        {
            if (DirectoryUtility.Exists(path) == false)
            {
                DirectoryUtility.Prepare(path);
                SvnCrema.CreateRepository(path);
            }
            else if (DirectoryUtility.IsEmpty(path) == true)
            {
                SvnCrema.CreateRepository(path);
            }

            //return SvnCrema.CreateInstance(path);
            throw new Exception();
        }
    }
}
