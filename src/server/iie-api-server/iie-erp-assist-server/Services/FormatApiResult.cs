using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// 统一WebApi数据返回格式
    /// </summary>

    public class FormatApiResult<T>
    {
        private static T data;
        private static Meta meta;

        private FormatApiResult(T d, Meta m)
        {
            meta = m;
            data = d;
        }

        public static FormatApiResult<T> result(T data, Meta meta)
        {
            return new FormatApiResult<T>(data, meta);
        }

        public T getData() => data;

        public Meta getMeta() => meta;
    }
}
