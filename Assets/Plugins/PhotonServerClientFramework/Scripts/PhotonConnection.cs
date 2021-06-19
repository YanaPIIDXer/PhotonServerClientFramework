using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonServerClient
{
    /// <summary>
    /// 接続クラス
    /// </summary>
    public class PhotonConnection : MonoBehaviour
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <returns>PhotonConnectionインスタンス</returns>
        public static PhotonConnection Create()
        {
            var obj = new GameObject("PhotonConnection");
            DontDestroyOnLoad(obj);

            var connection = obj.AddComponent<PhotonConnection>();
            return connection;
        }
    }
}
