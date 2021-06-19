using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using PhotonServerClient;

/// <summary>
/// テスト用クライアント
/// </summary>
public class TestClient : MonoBehaviour
{
    /// <summary>
    /// ボタン
    /// </summary>
    private Button button = null;

    void Awake()
    {
        button = GetComponent<Button>();
    }
}
