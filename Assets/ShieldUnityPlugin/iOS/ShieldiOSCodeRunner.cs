using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;
#if (UNITY_IPHONE || UNITY_TVOS)
    using System.Runtime.InteropServices;
    using AOT;
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
        
       [DllImport ("__Internal")]
       private static extern string _Shield_get_session_id();

       [DllImport ("__Internal")]
       private static extern string _Shield_get_device_result();

       private delegate void DelegateMessage(string result);
 
      [MonoPInvokeCallback(typeof(DelegateMessage))] 
      private static void delegateMessageReceived(string result) {
           Debug.Log("Message received: " + result);
      }
     #endif

    public void initShield() {
        #if (UNITY_IPHONE || UNITY_TVOS)
         if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS) {
			    _Shield_init(siteId, secretKey);
		 }
        #endif
    }

      public string getSessionId() { 
            return _Shield_get_session_id();
        }

        public void setDeviceResultStateCallback(ShieldDeviceResultStateCallback deviceResultStateCallback) {
            //TODO
        }

        public string getLatestDeviceResult() {
            return _Shield_get_device_result();
        }

        public void sendAttributes(string screeName, Dictionary<string, string> data) {
           //TODO
        }

}
