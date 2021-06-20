using System;

namespace PhotonServerClient
{
    /// <summary>
    /// 接続失敗時の例外
    /// </summary>
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConnectionFailedException()
            : base("Connection Failed")
        {
        }
    }
}
