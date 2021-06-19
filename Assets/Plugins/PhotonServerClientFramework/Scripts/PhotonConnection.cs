using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Cysharp.Threading.Tasks;

namespace PhotonServerClient
{
    /// <summary>
    /// 接続クラス
    /// </summary>
    public class PhotonConnection : MonoBehaviour, IPhotonPeerListener
    {
        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="protocol">接続プロトコル</param>
        /// <returns>PhotonConnectionオブジェクト</returns>
        public static PhotonConnection Create(ConnectionProtocol protocol)
        {
            var obj = new GameObject("PhotonConnection");
            DontDestroyOnLoad(obj);

            var connection = obj.AddComponent<PhotonConnection>();
            connection.peer = new PhotonPeer(connection, protocol);
            return connection;
        }

        /// <summary>
        /// Peer
        /// </summary>
        private PhotonPeer peer = null;

        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="host">ホスト</param>
        /// <param name="applicationName">アプリケーション名</param>
        public void Connect(string host, string applicationName)
        {
            peer.Connect(host, applicationName);
        }

        /// <summary>
        /// 切断
        /// </summary>
        public void Disconnect()
        {
            Destroy(gameObject);
        }

        void Update()
        {
            peer.Service();
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            switch (level)
            {
                case DebugLevel.WARNING:

                    Debug.LogWarning(string.Format("[{0}]: {1}", level.ToString(), message));
                    break;

                case DebugLevel.ERROR:

                    Debug.LogError(string.Format("[{0}]: {1}", level.ToString(), message));
                    break;

                default:

                    Debug.Log(string.Format("[{0}]: {1}", level.ToString(), message));
                    break;
            }
        }

        #region Event

        public void OnEvent(EventData eventData)
        {
        }

        #endregion

        #region OperationRequest and Response

        Dictionary<byte, AsyncReactiveProperty<OperationResponse>> responses = new Dictionary<byte, AsyncReactiveProperty<OperationResponse>>();

        /// <summary>
        /// リクエスト送信
        /// </summary>
        /// <param name="requestOperationCode">リクエストオペレーションコード</param>
        /// <param name="paramDic">パラメータDictionary</param>
        /// <param name="responseOpretaionCode">レスポンスオペレーションコード</param>
        public UniTask<OperationResponse> SendOperationRequest(byte requestOperationCode, Dictionary<byte, object> paramDic, byte responseOperationCode)
        {
            var prop = new AsyncReactiveProperty<OperationResponse>(null);
            prop.AddTo(this.GetCancellationTokenOnDestroy());
            responses.Add(responseOperationCode, prop);
            peer.OpCustom(requestOperationCode, paramDic, false);
            return prop.WaitAsync();
        }

        /// <summary>
        /// リクエスト送信
        /// ※投げっぱなしでレスポンスが存在しないパターンで使用する
        /// </summary>
        /// <param name="requestOperationCode">リクエストオペレーションコード</param>
        /// <param name="paramDic">パラメータDictionary</param>
        public void SendOperationRequest(byte requestOperationCode, Dictionary<byte, object> paramDic)
        {
            peer.OpCustom(requestOperationCode, paramDic, false);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            if (responses.ContainsKey(operationResponse.OperationCode))
            {
                responses[operationResponse.OperationCode].Value = operationResponse;
            }
        }

        #endregion

        #region Connect and Disconnect

        private AsyncReactiveProperty<AsyncUnit> OnConnectedProp = null;

        public UniTask OnConnectedAsync
        {
            get
            {
                if (OnConnectedProp == null)
                {
                    OnConnectedProp = new AsyncReactiveProperty<AsyncUnit>(AsyncUnit.Default);
                    OnConnectedProp.AddTo(this.GetCancellationTokenOnDestroy());
                }
                return OnConnectedProp.WaitAsync();
            }
        }

        private AsyncReactiveProperty<AsyncUnit> OnDisconnectedProp = null;

        public UniTask OnDisconnectedAsync
        {
            get
            {
                if (OnDisconnectedProp == null)
                {
                    OnDisconnectedProp = new AsyncReactiveProperty<AsyncUnit>(AsyncUnit.Default);
                    OnDisconnectedProp.AddTo(this.GetCancellationTokenOnDestroy());
                }
                return OnDisconnectedProp.WaitAsync();
            }
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:

                    if (OnConnectedProp != null)
                    {
                        OnConnectedProp.Value = AsyncUnit.Default;
                    }
                    break;

                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:

                    if (OnDisconnectedProp != null)
                    {
                        OnDisconnectedProp.Value = AsyncUnit.Default;
                    }
                    break;
            }
        }

        #endregion
    }
}
