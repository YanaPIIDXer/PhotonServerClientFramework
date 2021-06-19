using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using PhotonServerClient;
using System;
using ExitGames.Client.Photon;
using Photon.MmoDemo.Common;
using Cysharp.Threading.Tasks;

/// <summary>
/// テスト用クライアント
/// </summary>
public class TestClient : MonoBehaviour
{
    /// <summary>
    /// ボタン
    /// </summary>
    private Button button = null;

    /// <summary>
    /// クライアント
    /// </summary>
    private PhotonClient client = new PhotonClient();

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Vector), (byte)Photon.MmoDemo.Common.Protocol.CustomTypeCodes.Vector, Photon.MmoDemo.Common.Protocol.SerializeVector, Photon.MmoDemo.Common.Protocol.DeserializeVector);
        PhotonPeer.RegisterType(typeof(BoundingBox), (byte)Photon.MmoDemo.Common.Protocol.CustomTypeCodes.BoundingBox, Photon.MmoDemo.Common.Protocol.SerializeBoundingBox, Photon.MmoDemo.Common.Protocol.DeserializeBoundingBox);

        button = GetComponent<Button>();
        button.OnClickAsObservable()
              .Subscribe(async _ =>
              {
                  button.interactable = false;
                  var token = this.GetCancellationTokenOnDestroy();

                  try
                  {
                      await client.Connect("127.0.0.1:4530", "MMODemo", ConnectionProtocol.Tcp, token);
                      Debug.Log("Connection Success!");
                      await UniTask.Delay(1000);

                      var paramDic = new Dictionary<byte, object>
                                    {
                                        { (byte)ParameterCode.WorldName, "World" },
                                        { (byte)ParameterCode.BoundingBox, new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)) },
                                        { (byte)ParameterCode.TileDimensions,  new Vector(1f, 1f, 0f) }
                                    };
                      var response = await client.SendOperationRequest((byte)OperationCode.CreateWorld, paramDic, (byte)OperationCode.CreateWorld, token);
                      Debug.Log(string.Format("Create World Result:{0}", response.ReturnCode));
                  }
                  catch (Exception e)
                  {
                      Debug.LogError(e.Message);
                  }
                  finally
                  {
                      client.Disconnect();
                      Debug.Log("Disconnected.");
                  }

                  button.interactable = true;
              })
              .AddTo(gameObject);
    }
}
