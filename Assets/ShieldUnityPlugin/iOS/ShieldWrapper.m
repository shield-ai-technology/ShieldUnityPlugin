#import "ShieldWrapper.h"
#import <ShieldFraud/ShieldFraud.h>

static BOOL isShieldInitialized = NO;
char* ToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

NSString* ToNSString(const char* string) {
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return nil;
}

void _Shield_init(const char* siteId, const char* secretKey) {
    if (!isShieldInitialized)
    {
      Configuration *config = [[Configuration alloc] initWithSiteId:ToNSString(siteId) secretKey:ToNSString(secretKey)];
      [Shield setUpWith:config];
      isShieldInitialized = YES;
    }
}


void _Shield_set_device_result_callback(DeviceResultCallbackFunction deviceResultCallback) {

  [[Shield shared] setDeviceResultStateListener:^{ // 
    deviceResultCallback();
  }];
}


char*  _Shield_get_device_result() {
    if (isShieldInitialized) {
        return ToCString([NSString stringWithFormat:@"%@", [[Shield shared]sessionId]]);
    }
    else {
        return ToCString(@"");
    }
}

char* _Shield_get_session_id() {
    if (isShieldInitialized) {
        return ToCString([[Shield shared]sessionId]);
    }
    else {
        return ToCString(@"");
    }
}

