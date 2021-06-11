using System.Threading.Tasks;

namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// 最终响应的数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FinalApiResult<T>
    {
        public static async Task<ApiResult> Get(FormatApiResult<T> result)
        {
            return await Task.Run(() =>
            {
                ApiResult r = new();
                r.data.Add(result.getData());
                r.meta = result.getMeta();
                return r;
            });
        }
    }
}
