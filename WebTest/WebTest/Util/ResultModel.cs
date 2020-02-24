using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTest.Util
{
    public class ResultModel
    {
        /// <summary>
        /// 消息code（1：正常（大于0：表示正常），-1：异常（小于0：表示异常））
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }

        public int count { get; set; }
    }
}
