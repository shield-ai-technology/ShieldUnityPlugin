#import "UnityInterface.h"

NSString* ToNSString(const char* string);
char* ToCString(const NSString* nsString);
typedef void (*DeviceResultCallbackFunction)();
