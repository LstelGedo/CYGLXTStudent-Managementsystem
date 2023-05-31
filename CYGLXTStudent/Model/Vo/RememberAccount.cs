using System;

namespace CYGLXTStudent.Model.Vo
{
    /// <summary>
    /// 可序列化：自定义实体类
    /// </summary>
    [Serializable]
    public class RememberAccount
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string  MyUserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string  MyPassword { get; set; }
        /// <summary>
        /// 记住账号
        /// </summary>
        public bool  MyIsRemember { get; set; }
        /// <summary>
        /// 自动登录
        /// </summary>
        public bool  MyIsLogin { get; set; }
    }
}
