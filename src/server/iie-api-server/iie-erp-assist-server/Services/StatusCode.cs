namespace iie_erp_assist_server.Services
{
    /// <summary>
    /// 响应状态码
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 请求/查询/更新成功
        /// </summary>
        OK = 200,
        /// <summary>
        /// 添加成功
        /// </summary>
        CREATED = 201,
        /// <summary>
        /// 删除成功
        /// </summary>
        DELETED = 204,
        /// <summary>
        /// 请求的地址不存在或者包含不支持的参数
        /// </summary>
        BAD_REQUEST = 400,
        /// <summary>
        /// 未授权
        /// </summary>
        UNAUTHORIZED = 401,
        /// <summary>
        /// 参数缺失
        /// </summary>
        PARAMSMISSING = 402,
        /// <summary>
        /// 被禁止访问
        /// </summary>
        FORBIDDEN = 403,
        /// <summary>
        /// 请求的资源不存在
        /// </summary>
        NOT_FOUND = 404,
        /// <summary>
        /// 主键重复
        /// </summary>
        PRIMARYKEYREPEAT = 405,
        /// <summary>
        /// [POST/PUT/PATCH] 当创建一个对象时，发生一个验证错误
        /// </summary>
        UNPROCESABLE_ENTITY = 422,
        /// <summary>
        /// 内部错误
        /// </summary>
        INTERNAL_SERVER_ERROR = 500
    }
}
