using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotonServerClient
{
    /// <summary>
    /// クライアントクラス
    /// </summary>
    public class PhotonClient : MonoBehaviour
    {
        #region Singleton
        public static PhotonClient Instance { get { return instance; } }
        private static PhotonClient instance = new PhotonClient();
        private PhotonClient() { }
        #endregion
    }
}
