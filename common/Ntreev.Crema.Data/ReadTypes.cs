using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Crema.Data
{
    public enum ReadTypes
    {
        /// <summary>
        /// 모든 내용을 읽어들입니다.
        /// </summary>
        All,

        /// <summary>
        /// 테이블의 내용을 읽지 않습니다.
        /// </summary>
        OmitContent,

        /// <summary>
        /// 타입만 읽어들입니다.
        /// </summary>
        TypeOnly,
    }
}
