using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using System.Threading;
using UniRx;
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
        /// リクエスト送信
        /// </summary>
        /// <param name="requestOperationCode">リクエストオペレーションコード</param>
        /// <param name="paramDic">パラメータDictionary</param>
        /// <param name="responseOperationCode">レスポンスオペレーションコード</param>
        /// <returns>レスポンスパラメータ</returns>
        public async UniTask<OperationResponse> SendOperationRequest(byte requestOperationCode, Dictionary<byte, object> paramDic, byte responseOperationCode, CancellationToken token = default)
        {
            if (connection == null) { return null; }

            var task = connection.SendOperationRequest(requestOperationCode, paramDic, responseOperationCode);
            var responseParams = await task.AttachExternalCancellation(token);

            return responseParams;
        }

        /// <summary>
        /// リクエスト送信
        /// ※投げっぱなしでレスポンスが存在しないパターンで使用する
        /// </summary>
        /// <param name="requestOperationCode">リクエストオペレーションコード</param>
        /// <param name="paramDic">パラメータDictionary</param>
        public void SendOperationRequest(byte requestOperationCode, Dictionary<byte, object> paramDic)
        {
            if (connection == null) { return; }

            connection.SendOperationRequest(requestOperationCode, paramDic);
        }

        /// <summary>
        /// Event受信時のObservableを取得
        /// </summary>
        /// <param name="eventCode">イベントコード</param>
        /// <returns>Observable</returns>
        public IObservable<EventData> GetEventObservable(byte eventCode)
        {
            if (connection == null) { return null; }
            return connection.GetEventObservable(eventCode);
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
