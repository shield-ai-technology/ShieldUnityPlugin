using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;
#if (UNITY_IPHONE || UNITY_TVOS)
using System.Runtime.InteropServices;
#endif
public class ShieldiOSCodeRunner
{
     private string siteId;
    private string secretKey;
    private ShieldCallback callback;
    public ShieldiOSCodeRunner(string siteId, string secretKey, ShieldCallback callback = null){
        this.siteId = siteId;
        this.secretKey = secretKey;
        this.callback = callback;
    }
    #if (UNITY_IPHONE || UNITY_TVOS)
       [DllImport ("__Internal")]
       private static extern void _Shield_init(string siteId, string secretKey);
    #endif

    public void initShield() {
        Debug.Log("shield checking initialize");
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS) {
            Debug.Log("shield intialize");
			_Shield_init(siteId, secretKey);
		}
    }

      public string getSessionId() { 
            return "";
        }

        public void setDeviceResultStateCallback(ShieldDeviceResultStateCallback deviceResultStateClalback) {
           
        }

        public string getLatestDeviceResult() {
            return "";
        }

        public void sendAttributes(string screeName, Dictionary<string, string> data) {
           
        }

}
