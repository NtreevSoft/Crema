using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.Library.Commands;

namespace Ntreev.Crema.Commands
{
    static class DataSplitSetting
    {
        [CommandProperty]
        [Description("크레마 데이터 파일을 테이블 이름별로 출력합니다.\n이 옵션을 사용하면 필수인자 <filename> 은 출력할 디렉토리로 사용합니다.")]
        [DefaultValue(false)]
        public static bool Split
        {
            get; set;
        }
    }
}
