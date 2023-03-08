NSString* ToNSString(const char* string);
char* ToCString(const NSString* nsString);
typedef void (*DelegateCallbackFunction)(const char* result);