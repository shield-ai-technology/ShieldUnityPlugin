using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;
using System;
#if(UNITY_IPHONE || UNITY_TVOS)
using System.Runtime.InteropServices;
#endif

public class ShieldiOSCodeRunner {
  private string siteId;
  private string secretKey;
  private ShieldCallback callback;
  private static ShieldDeviceResultStateCallback deviceResultStateCallbackStatic;

  public ShieldiOSCodeRunner(string siteId, string secretKey, ShieldCallback callback = null) {
    this.siteId = siteId;
    this.secretKey = secretKey;
    this.callback = callback;
  }

  #if(UNITY_IPHONE || UNITY_TVOS)
  [DllImport("__Internal")]
  private static extern void ShieldWrapper_initShieldWithSiteId(string siteId, string secretKey);

  [DllImport("__Internal")]
  private static extern string ShieldWrapper_getSessionId();

  [DllImport("__Internal")]
  private static extern string ShieldWrapper_getLatestDeviceResult();

  [DllImport("__Internal")]
  private static extern void sendAttributes(string screenName, string[] dataKeys, string[] dataValues, int numItems);

  private delegate void DelegateMessage();

  [AOT.MonoPInvokeCallback(typeof (DelegateMessage))]
  private static void delegateMessageReceived() {
    if (ShieldiOSCodeRunner.deviceResultStateCallbackStatic != null) {
      ShieldiOSCodeRunner.deviceResultStateCallbackStatic.IsReady();
    }
  }

  [DllImport("__Internal")]
  private static extern void _Shield_set_device_result_callback(DelegateMessage delegateMessageFunc);
  #endif

  public void initShield() {
    Debug.Log("shield checking initialize");
    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS) {
      Debug.Log("shield intialize");
      ShieldWrapper_initShieldWithSiteId(siteId, secretKey);
    }
  }

  public string getSessionId() {
    Debug.Log("shield session id is being fetched");
    return ShieldWrapper_getSessionId();
  }

  public void setDeviceResultStateCallback(ShieldDeviceResultStateCallback deviceResultStateCallback) {
    Debug.Log("trying to sync the delegate from C#");
    ShieldiOSCodeRunner.deviceResultStateCallbackStatic = deviceResultStateCallback;
    _Shield_set_device_result_callback(delegateMessageReceived);
  }

  public string getLatestDeviceResult() {
    Debug.Log("getLatestDeviceResult IS CALLED");
    return ShieldWrapper_getLatestDeviceResult();
  }

  public void sendAttributes(string screenName, Dictionary < string, string > data) {
    int numItems = data.Count;
    string[] dataKeys = new string[numItems];
    string[] dataValues = new string[numItems];

    int index = 0;
    foreach(KeyValuePair < string, string > entry in data) {
      dataKeys[index] = entry.Key;
      dataValues[index] = entry.Value;
      index++;
    }

    if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS) {
      sendAttributes(screenName, dataKeys, dataValues, numItems);
    }
  }
}