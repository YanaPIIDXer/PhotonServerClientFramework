using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace PhotonServerClient
{
    /// <summary>
    /// クライアントクラス
    /// </summary>
    public class PhotonClient
    {
        /// <summary>
        /// 接続オブジェクト
        /// </summary>
        private PhotonConnection connection = null;

        #region Singleton
        public static PhotonClient Instance { get { return instance; } }
        private static PhotonClient instance = new PhotonClient();
        private PhotonClient() { }
        #endregion
    }
}
