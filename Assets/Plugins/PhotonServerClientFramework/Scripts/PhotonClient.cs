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
    /// イベントコールバック
    /// </summary>
    public interface IEventCallback
    {
        /// <summary>
        /// イベント発生
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        void OnEvent(EventData eventData);
    }

    /// <summary>
    /// クライアントクラス
    /// </summary>
    public class PhotonClient : IEventCallback
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
            connection = PhotonConnection.Create(protocol, this);

            var connTask = UniTask.WhenAny(
                connection.OnConnectedAsync.AsAsyncUnitUniTask(),
                connection.OnDisconnectedAsync.AsAsyncUnitUniTask()
            );

            connection.Connect(host, applicationName);

            var (idx, _, __) = await connTask.AttachExternalCancellation(token);

            if (idx != 0)
            {
                throw new ConnectionFailedException();
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

        private Dictionary<byte, Subject<EventData>> events = new Dictionary<byte, Subject<EventData>>();

        /// <summary>
        /// Event受信時のObservableを取得
        /// </summary>
        /// <param name="eventCode">イベントコード</param>
        /// <returns>Observable</returns>
        public IObservable<EventData> GetEventObservable(byte eventCode)
        {
            if (!events.ContainsKey(eventCode))
            {
                events.Add(eventCode, new Subject<EventData>());
            }
            return events[eventCode];
        }

        /// <summary>
        /// イベント発生
        /// </summary>
        /// <param name="eventData">イベントデータ</param>
        public void OnEvent(EventData eventData)
        {
            if (events.ContainsKey(eventData.Code))
            {
                events[eventData.Code].OnNext(eventData);
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
