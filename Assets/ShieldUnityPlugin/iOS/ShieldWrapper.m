#import "ShieldWrapper.h"

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
char* convertNSStringToCString(const NSString* nsString)
{
    if (nsString == NULL)
        return NULL;
    
    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);
    
    return cString;
}

NSString* convertDictionaryToString(NSDictionary<NSString *, id> * result) {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:result
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
    if (!jsonData) {
        NSLog(@"Error converting dictionary to JSON string: %@", error);
        return nil;
    } else {
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
}

NSString* getErrorDescription(NSError *error) {
    if (error) {
        return [error localizedDescription];
    } else {
        return nil;
    }
}

NSDictionary* createDictionary(const char **dataKeys, const char **dataValues, int numItems) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionaryWithCapacity:numItems];
    for (int i = 0; i < numItems; i++) {
        NSString *key = [NSString stringWithUTF8String:dataKeys[i]];
        NSString *value = [NSString stringWithUTF8String:dataValues[i]];
        [dictionary setObject:value forKey:key];
    }
    return [NSDictionary dictionaryWithDictionary:dictionary];
}

@implementation ShieldWrapper
ShieldCallbackSuccessFunction _successCallback;
ShieldCallbackErrorFunction _errorCallback;
- (instancetype)initShieldWithSiteId:(const char *)siteId secretKey:(const char *)secretKey successCallback: (ShieldCallbackSuccessFunction)successCallback errorCallback: (ShieldCallbackErrorFunction)errorCallback {
    self = [super init];
    if (self) {
        if (!isShieldInitialized){
            Configuration *config = [[Configuration alloc] initWithSiteId:ToNSString(siteId) secretKey:ToNSString(secretKey)];
            config.deviceShieldCallback = self;
            [Shield setUpWith:config];
            isShieldInitialized = YES;
            _successCallback = successCallback;
            _errorCallback = errorCallback;
        }
    }
    return self;
}

- (void)didSuccessWithResult:(NSDictionary<NSString *, id> *)result {
    // Handle successful result here
    if (result != NULL) {
            //do something with the result
            _successCallback(convertNSStringToCString(convertDictionaryToString(result)));
    } 
}

- (void)didErrorWithError:(NSError *)error {
    // Handle error here
     if (error != NULL) {
            //do something with the result
            _errorCallback(convertNSStringToCString(error.localizedDescription));
    } 
}

- (NSString *)getSessionId {
    if (isShieldInitialized) {
        NSLog(@"SHIELD_INIT_DONE:: SessionID %@", [[Shield shared] sessionId]);
        return [[Shield shared] sessionId];
    }else {
        return @"SHIELD_INIT_FAILED:: No SessionID";
    }
}

- (NSString *)getLatestDeviceResult {
    if (isShieldInitialized) {
        NSDictionary<NSString *, id> *result = [[Shield shared] getLatestDeviceResult];
        if (result != NULL) {
            //do something with the result
            NSLog(@"SHIELD_INIT_DONE:: LATEST DEVICE RESULT IS SUCCESSFUL");
            return convertDictionaryToString(result);
        } else {
            NSError *error = [[Shield shared] getErrorResponse];
            if (error != NULL) {
                // log error
                NSLog(@"SHIELD_INIT_DONE:: LATEST DEVICE RESULT IS ERROR");
                return getErrorDescription(error);
            }
        }
    }
    return @"SHIELD_INIT_FAILED:: No Device Result";
}

- (void)sendAttributes:(NSString *)screenName params:(NSDictionary *)data{
    if (isShieldInitialized) {
        [[Shield shared] setDeviceResultStateListener:^{
            [[Shield shared] sendAttributesWithScreenName:screenName data:data];
            NSLog(@"SHIELD_INIT_DONE:: Attributes Sent");
        }];
    } else {
            NSLog(@"SHIELD_INIT_FAILED:: Attributes NOT Sent");
    }
}

- (void)setDeviceResultsCallback:(DeviceResultCallbackFunction)deviceResultCallback{
    //Adding manual delay of 2 secs
    NSTimeInterval delayInSeconds = 2.0;
    dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, (int64_t)(delayInSeconds * NSEC_PER_SEC));
    dispatch_after(popTime, dispatch_get_main_queue(), ^(void){
        if (isShieldInitialized) {
            [[Shield shared] setDeviceResultStateListener:^{
                NSLog(@"SHIELD_INIT_DONE:: SDK READY");
                deviceResultCallback();
            }];
        }
    });
}

@end

static ShieldWrapper *ShieldWrapperIns;

void ShieldWrapper_initShieldWithSiteId(const char *siteId, const char *secretKey, ShieldCallbackSuccessFunction successCallback, ShieldCallbackErrorFunction errorCallback ) {
    if (!ShieldWrapperIns) {
        ShieldWrapperIns = [[ShieldWrapper alloc] initShieldWithSiteId:siteId secretKey:secretKey successCallback:successCallback errorCallback:errorCallback];
    }
}

char* ShieldWrapper_getSessionId(){
    if (!ShieldWrapperIns){
        return  NULL;
    }
    const NSString* sessionId = [ShieldWrapperIns getSessionId];
    return convertNSStringToCString(sessionId);
}

char* ShieldWrapper_getLatestDeviceResult(){
    if (!ShieldWrapperIns){
        return  NULL;
    }
    const NSString* getLatestDeviceResult = [ShieldWrapperIns getLatestDeviceResult];
    return convertNSStringToCString(getLatestDeviceResult);
}

void sendAttributes(const char *screenName, const char **dataKeys, const char **dataValues, int numItems) {
    // Implementation code here
    NSDictionary * Dictionary = createDictionary(dataKeys, dataValues, numItems);
    NSLog(@"SHIELD:: sendAttributes Payload: %@\n%@", Dictionary, ToNSString(screenName));
    [ShieldWrapperIns sendAttributes: ToNSString(screenName) params: Dictionary];
}

void _Shield_set_device_result_callback(DeviceResultCallbackFunction deviceResultCallback) {
    [ShieldWrapperIns setDeviceResultsCallback:deviceResultCallback];
}

