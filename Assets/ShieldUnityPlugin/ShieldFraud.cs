using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Shield.Unity 
{   
    
    public class ShieldCallback
    {
        public Action<string> OnSuccess { set; get; }
        public Action<string> OnError { set; get; }
    }

    public class ShieldDeviceResultStateCallback {
        public Action IsReady {set; get;}
    }

    public class ShieldFraud
    {
        public static bool isShieldInitialized = false;
        private string siteId;
        private string secretKey;
        private ShieldCallback callback;
        #if UNITY_ANDROID
            private ShieldCodeRunner codeRunner;
        #endif
        #if (UNITY_IOS || UNITY_TVOS) 
            private ShieldiOSCodeRunner codeRunner;
        #endif
        public ShieldFraud(string siteId, string secretKey, ShieldCallback callback = null) {
            this.callback = callback;
            this.siteId = siteId;
            this.secretKey = secretKey;
             #if UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android) {
                    codeRunner = new ShieldCodeRunner(siteId, secretKey, callback);
                }
            #endif
            
            #if (UNITY_IOS || UNITY_TVOS) 
                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS) {
                    codeRunner = new ShieldiOSCodeRunner(siteId, secretKey, callback);
                }
            #endif
        }

        public void initShield() {
            #if (UNITY_IOS || UNITY_TVOS || UNITY_ANDROID) 
                if (!ShieldFraud.isShieldInitialized) {
                  codeRunner.initShield();
                  ShieldFraud.isShieldInitialized = true;

            }
            #endif
        }

        public string getSessionId() { 
            #if (UNITY_IOS || UNITY_TVOS || UNITY_ANDROID) 
                return codeRunner.getSessionId();
            #else
                return "";
            #endif
    
        }

        public void setDeviceResultStateCallback(ShieldDeviceResultStateCallback deviceResultStateClalback) {
            #if (UNITY_IOS || UNITY_TVOS || UNITY_ANDROID) 
                codeRunner.setDeviceResultStateCallback(deviceResultStateClalback);
            #endif
        }

        public string getLatestDeviceResult() {
            #if (UNITY_IOS || UNITY_TVOS || UNITY_ANDROID) 
                return codeRunner.getLatestDeviceResult();
            #else
                return "";
            #endif
        }

        public void sendAttributes(string screeName, Dictionary<string, string> data) {
            #if (UNITY_IOS || UNITY_TVOS || UNITY_ANDROID) 
                codeRunner.sendAttributes(screeName, data);
            #endif
            
        }
    }

}
