using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;
public class ShieldInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    	
        ShieldFraud shield = new ShieldFraud("dda05c5ddac400e1c133a360e2714aada4cda052",
            "9ce44f88a25272b6d9cbb430ebbcfcf1", new ShieldCallback() {
                    OnSuccess  = (string result) => Debug.Log("Shield sucesss " + result),                
                    OnError  = (string error) => Debug.Log(error)
                });
        shield.initShield();
        shield.setDeviceResultStateCallback(new ShieldDeviceResultStateCallback(){
            IsReady = () => {
                Debug.Log("Device result is ready");
            
                var data = new Dictionary<string, string>(){
                	{"user_id", "12345abcdef"},
    	           {"email", "test@gmail.com"}
                };
                Debug.Log("sending attributes");
                shield.sendAttributes("test", data);
                Debug.Log("sending attributes success");
            }
        });

        Debug.Log("getting shield session id: " + shield.getSessionId());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
