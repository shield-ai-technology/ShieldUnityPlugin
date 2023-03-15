#import <Foundation/Foundation.h>
#import <ShieldFraud/ShieldFraud.h>

extern void ShieldWrapper_initShieldWithSiteId(const char *siteId, const char *secretKey);
extern char* ShieldWrapper_getSessionId();
extern void ShieldWrapper_getLatestDeviceResults_completionHandler(void (*completion_handler)(const char *result, const char *error));
extern void sendAttributes(const char *screenName, const char **dataKeys, const char **dataValues, int numItems);

@interface ShieldWrapper : NSObject <DeviceShieldCallback>

@end
