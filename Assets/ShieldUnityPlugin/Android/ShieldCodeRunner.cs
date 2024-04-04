using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;

public class ShieldCodeRunner {
  private string siteId;
  private string secretKey;
  private ShieldCallback callback;
  
  public ShieldCodeRunner(string siteId, string secretKey, ShieldCallback callback = null) {
    this.siteId = siteId;
    this.secretKey = secretKey;
    this.callback = callback;
  }

  // Start is called before the first frame update
  class AndroidShieldCallback: AndroidJavaProxy {
    public string siteId;
    public string secretKey;
    public ShieldCallback callback;

    public AndroidShieldCallback(): base("com.shield.android.ShieldCallback") {}
    public void onSuccess(AndroidJavaObject result) {
      string resultString = result.Call < string > ("toString");
      if (callback != null) {
        callback.OnSuccess(resultString);
      }
    }
    public void onFailure(AndroidJavaObject error) {
      string errMessage = error.Call < string > ("getLocalizedMessage");
      if (callback != null) {
        callback.OnError(errMessage);
      }
    }

  }

  class AndroidDeviceResultCallback: AndroidJavaProxy {
    private ShieldDeviceResultStateCallback deviceResultStateClalback;
    public AndroidDeviceResultCallback(ShieldDeviceResultStateCallback deviceResultStateClalback): base("com.shield.android.Shield$DeviceResultStateListener") {
      this.deviceResultStateClalback = deviceResultStateClalback;
    }
    public void isReady() {
      deviceResultStateClalback.IsReady();
    }
  }

  public void initShield() {
    if (Application.platform == RuntimePlatform.Android) {

      // Retrieve the UnityPlayer class.
      using(AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
        using(AndroidJavaObject unityActivity = unityPlayerClass.GetStatic < AndroidJavaObject > ("currentActivity")) {
          using(AndroidJavaClass shieldClass = new AndroidJavaClass("com.shield.android.Shield")) {
            AndroidJavaObject shieldBuilderObject = new AndroidJavaObject("com.shield.android.Shield$Builder", unityActivity, siteId, secretKey);
            AndroidShieldCallback androidShieldCallback = new AndroidShieldCallback();
            androidShieldCallback.siteId = siteId;
            androidShieldCallback.secretKey = secretKey;
            androidShieldCallback.callback = callback;
            shieldBuilderObject = shieldBuilderObject.Call < AndroidJavaObject > ("registerDeviceShieldCallback", androidShieldCallback);
            using(AndroidJavaObject shieldObj = shieldBuilderObject.Call < AndroidJavaObject > ("build")) {
              shieldClass.CallStatic("setSingletonInstance", shieldObj);
            }
          }
        }
      }
    }
  }
  public string getSessionId() {
    string sessionId = "";
    using(AndroidJavaClass shieldClass = new AndroidJavaClass("com.shield.android.Shield")) {
      using(AndroidJavaObject secondShieldObj = shieldClass.CallStatic < AndroidJavaObject > ("getInstance")) {
        sessionId = secondShieldObj.Call < string > ("getSessionId");
      }
    }
    Debug.Log("session id: " + sessionId);
    return sessionId;
  }

  public void setDeviceResultStateCallback(ShieldDeviceResultStateCallback deviceResultStateClalback) {
    using(AndroidJavaClass shieldClass = new AndroidJavaClass("com.shield.android.Shield")) {
      using(AndroidJavaObject secondShieldObj = shieldClass.CallStatic < AndroidJavaObject > ("getInstance")) {
        secondShieldObj.Call("setDeviceResultStateListener", new AndroidDeviceResultCallback(deviceResultStateClalback));
      }
    }
  }

  public string getLatestDeviceResult() {
    string resultStr = "";
    using(AndroidJavaClass shieldClass = new AndroidJavaClass("com.shield.android.Shield")) {
      using(AndroidJavaObject secondShieldObj = shieldClass.CallStatic < AndroidJavaObject > ("getInstance")) {
        using(AndroidJavaObject result = secondShieldObj.Call < AndroidJavaObject > ("getLatestDeviceResult")) {
          resultStr = result.Call < string > ("toString");
        }
      }
    }

    return resultStr;
  }

  public void sendAttributes(string screenName, Dictionary < string, string > data) {
    using(AndroidJavaClass shieldClass = new AndroidJavaClass("com.shield.android.Shield")) {
      using(AndroidJavaObject shieldObj = shieldClass.CallStatic < AndroidJavaObject > ("getInstance")) {
        using(AndroidJavaObject hashMapObj = new AndroidJavaObject("java.util.HashMap")) {
          foreach(KeyValuePair < string, string > pair in data) {
            using(AndroidJavaObject keyString = new AndroidJavaObject("java.lang.String", pair.Key)) {
              using(AndroidJavaObject valueString = new AndroidJavaObject("java.lang.String", pair.Value)) {
                hashMapObj.Call < string > ("put", keyString, valueString);
              }
            }
          }
          shieldObj.Call("sendAttributes", screenName, hashMapObj);
        }
      }
    }
  }
}