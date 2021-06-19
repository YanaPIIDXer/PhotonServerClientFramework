using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using System.Threading;
using System;

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
        /// </summary>
        /// <param name="host">ホスト</param>
        /// <param name="applicationName">アプリケーション名</param>
        /// <param name="protocol">プロトコル</param>
        public async UniTask Connect(string host, string applicationName, ConnectionProtocol protocol, CancellationToken token = default)
        {
            Disconnect();   // 一旦切断
            connection = PhotonConnection.Create(protocol);

            var connTask = UniTask.WhenAny(
                connection.OnConnectedAsync.AsAsyncUnitUniTask(),
                connection.OnDisconnectedAsync.AsAsyncUnitUniTask()
            );

            connection.Connect(host, applicationName);

            var (idx, _, __) = await connTask.AttachExternalCancellation(token);

            if (idx != 0)
            {
                // TODO:自前のException定義する？
                throw new Exception("Connection Failed.");
            }
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
