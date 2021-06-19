using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;

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

        /// <summary>
        /// 接続
        /// TODO:UniTaskを使う形にする
        /// </summary>
        /// <param name="host">ホスト</param>
        /// <param name="protocol">プロトコル</param>
        public void Connect(string host, ConnectionProtocol protocol)
        {
            Disconnect();   // 一旦切断
            connection = PhotonConnection.Create(protocol);
        }

        /// <summary>
        /// 切断
        /// </summary>
        public void Disconnect()
        {
            if (connection != null)
            {
                connection.Disconnect();
            }
        }
    }
}
