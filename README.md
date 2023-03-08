# ShieldUnityPlugin
Unity Plugin for Shield SDK (www.shield.com) 

SHIELD SDK helps developers to assess malicious activities performed on mobile devices and return risk intelligence based on user's behaviour. It collects device's fingerprint, social metrics and network information. SHIELD SDK is built with Java for Android and Swift for iOS.

## Getting Started

### Install the Library

Download the latest shieldunityplugin.unitypackage from GitHub [releases](https://github.com/shield-ai-technology/ShieldUnityPlugin/releases).
Double click shieldunityplugin.unitypackage to import the package into your Unity project.

### Initialise the Client

Initialise Shield at the beginning of app launch.

```
 ShieldFraud shield = new ShieldFraud("site_id",
            "secret_key", new ShieldCallback() {
                    OnSuccess  = (string result) => Debug.Log("Shield sucesss " + result),                
                    OnError  = (string error) => Debug.Log(error)
                });
 shield.initShield();
```

### Get Session ID
Session ID is the unique identifier of a user’s app session
```
shield.getSessionId()
```

### Get Device Result
#### - Retrieve device results via Optimised Listener

Pass a callback to ShieldConfig object to retrieve device result via Listener.

```
  ShieldFraud shield = new ShieldFraud("site_id",
            "secret_key", new ShieldCallback() {
                    OnSuccess  = (string result) => Debug.Log("Shield sucesss " + result),                
                    OnError  = (string error) => Debug.Log(error)
                });
```

#### - Retrieve device results via Customised Pull

You can also retrieve latest device result at any point.

```
   shield.setDeviceResultStateCallback(new ShieldDeviceResultStateCallback(){
            IsReady = () => {
                Debug.Log("Device result is ready");
                string result = shield.getLatestDeviceResult();
            }
        });
```

## Send Custom Attributes
Use the `sendAttributes` function to sent event-based attributes such as `user_id` for enhanced analytics. This function accepts two parameters:`screenName` where the function is triggered, and  `data` to provide any custom fields in key, value pairs.

```
var data = new Dictionary<string, string>(){
                	{"user_id", "12345abcdef"},
    	           {"email", "test@gmail.com"}
                };
Debug.Log("sending attributes");
shield.sendAttributes("test", data);
```
