using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

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

        void Update()
        {
            if (peer != null)
            {
                peer.Service();
            }
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

        public void OnEvent(EventData eventData)
        {
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
        }
    }
}
