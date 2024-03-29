﻿using System.Collections;
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

        // イベントのSubscribe
        client.GetEventObservable((byte)EventCode.ItemSubscribed)
              .Subscribe(eventCode =>
              {
                  var itemId = eventCode.Parameters[(byte)ParameterCode.ItemId];
                  Debug.Log(string.Format("ItemID:{0} Subscribe.", itemId));
              })
              .AddTo(gameObject);

        button = GetComponent<Button>();
        button.OnClickAsObservable()
              .Subscribe(async _ =>
              {
                  button.interactable = false;
                  var token = this.GetCancellationTokenOnDestroy();

                  try
                  {
                      // Connect
                      await client.Connect("127.0.0.1:4530", "MMODemo", ConnectionProtocol.Tcp, token);
                      Debug.Log("Connection Success!");
                      await UniTask.Delay(1000);

                      // Request/Response
                      var paramDic = new Dictionary<byte, object>
                                    {
                                        { (byte)ParameterCode.WorldName, "World" },
                                        { (byte)ParameterCode.BoundingBox, new BoundingBox(new Vector(0f, 0f, 0f), new Vector(10f, 10f, 0f)) },
                                        { (byte)ParameterCode.TileDimensions,  new Vector(1f, 1f, 0f) }
                                    };
                      var response = await client.SendOperationRequest((byte)OperationCode.CreateWorld, paramDic, (byte)OperationCode.CreateWorld, token);
                      Debug.Log(string.Format("Create World Result:{0}", response.ReturnCode));
                      paramDic = new Dictionary<byte, object>
                                {
                                    { (byte)ParameterCode.WorldName, "World" },
                                    { (byte)ParameterCode.Username, "Test" },
                                    { (byte)ParameterCode.Position, new Vector(1.0f, 1.0f, 0.0f) },
                                    { (byte)ParameterCode.ViewDistanceEnter, new Vector(1.0f, 1.0f, 0.0f) },
                                    { (byte)ParameterCode.ViewDistanceExit, new Vector(2.0f, 2.0f, 0.0f) }
                                };
                      response = await client.SendOperationRequest((byte)OperationCode.EnterWorld, paramDic, token);
                      Debug.Log(string.Format("Enter World Result:{0}", response.ReturnCode));
                      await UniTask.Delay(1000);
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
