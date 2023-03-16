using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shield.Unity;

public class ShieldInitializer: MonoBehaviour {
  // Start is called before the first frame update
  void Start() {

    ShieldFraud shield = new ShieldFraud("dda05c5ddac400e1c133a360e2714aada4cda052",
      "9ce44f88a25272b6d9cbb430ebbcfcf1", new ShieldCallback() {
        OnSuccess = (string result) => Debug.Log("Shield callback sucesss " + result),
          OnError = (string error) => Debug.Log(error)
      });
    shield.initShield();
    shield.setDeviceResultStateCallback(new ShieldDeviceResultStateCallback() {
      IsReady = () => {
        Debug.Log("SHIELD:: Device result is ready");
        Debug.Log("SHIELD:: session id: " + shield.getSessionId());
        var data = new Dictionary < string, string > () {
            {"user_id", "12345abcdef"}, 
            {"email", "test@gmail.com"}
          };
        Debug.Log("SHIELD:: sending attributes");
        shield.sendAttributes("test", data);
        Debug.Log("SHIELD:: sending attributes success");
        Debug.Log("SHIELD:: get latest device results: " + shield.getLatestDeviceResult());
      }
    });
  }

  // Update is called once per frame
  void Update() {

  }
}