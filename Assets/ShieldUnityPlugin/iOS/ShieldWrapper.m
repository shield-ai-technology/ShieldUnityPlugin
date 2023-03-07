#import "ShieldWrapper.h"
#import <ShieldFraud/ShieldFraud.h>

static BOOL isShieldInitialized = NO;

NSString* ToNSString(const char* string) {
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return nil;
}

void _Shield_init(const char* siteId, const char* secretKey) {
    if (!isShieldInitialized) 
    {
      NSLog(@"shield trying to initialize");
      Configuration *config = [[Configuration alloc] initWithSiteId:ToNSString(siteId) secretKey:ToNSString(secretKey)];
      [Shield setUpWith:config];
      isShieldInitialized = YES;
    }
}