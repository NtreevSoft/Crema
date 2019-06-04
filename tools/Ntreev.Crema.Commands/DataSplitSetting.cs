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
        private static string ext;

        [CommandProperty]
        [Description("크레마의 테이블 이름별로 데이터 파일을 출력합니다.\n이 옵션을 사용하면 필수인자 <filename> 은 출력할 디렉토리로 사용합니다.")]
        [DefaultValue(false)]
        public static bool Split
        {
            get; set;
        }

        [CommandProperty]
        [Description("--split 옵션을 사용할 경우 이 옵션으로 출력 파일의 확장자를 지정합니다.")]
        [DefaultValue("dat")]
        public static string Ext
        {
            get => ext;
            set => ext = value.StartsWith(".") ? value.Remove(0, 1) : value;
        }
    }
}
