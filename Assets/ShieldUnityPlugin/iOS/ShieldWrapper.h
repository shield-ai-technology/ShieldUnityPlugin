
#import <Foundation/Foundation.h>
#import <ShieldFraud/ShieldFraud-Swift.h>
typedef void (*ShieldCallbackSuccessFunction)(const char* result);
typedef void (*ShieldCallbackErrorFunction)(const char* error);
extern void ShieldWrapper_initShieldWithSiteId(const char *siteId, const char *secretKey, ShieldCallbackSuccessFunction successCallback, ShieldCallbackErrorFunction errorCallback);
extern char* ShieldWrapper_getSessionId();
extern void sendAttributes(const char *screenName, const char **dataKeys, const char **dataValues, int numItems);
char* ShieldWrapper_getLatestDeviceResult();

@interface ShieldWrapper : NSObject <DeviceShieldCallback>
typedef void (*DeviceResultCallbackFunction)();
extern void _Shield_set_device_result_callback(DeviceResultCallbackFunction deviceResultCallback);
@end
