using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using PhotonServerClient;
using System;
using ExitGames.Client.Photon;

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
        button = GetComponent<Button>();
        button.OnClickAsObservable()
              .Subscribe(async _ =>
              {
                  button.interactable = false;

                  try
                  {
                      await client.Connect("127.0.0.1:4530", "MMODemo", ConnectionProtocol.Tcp);
                      Debug.Log("Connection Success!");
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
