#ifndef lisa_dll_h
#define lisa_dll_h

#ifdef __cplusplus
extern "C" {
#endif

#ifdef LISA_Export
#define LISA_API __declspec (dllexport)
#else
#define LISA_API __declspec (dllimport)
#endif

	typedef int LISA_Status;

	enum {
		LISA_OK, // Function successfully
			 LISA_LogInFail, // Login failure (e.g. double login)
			 LISA_NoLogIn, // Function call without previous LogIn
			 LISA_OutOfRange, // Value out of range
			 LISA_DeviceNotFound, // No device found
			 LISA_MeasCycleRun, // Measurement cycle is running
			 LISA_NoValues,	// No values available, because meas. cycle is not running
			 LISA_TimeOut, // No reply from board. Time out error
			 LISA_NotNewValues, // Not new values since last call
			 LISA_SyncPulseLoss, // loss of the sync pulse at input
			 LISA_SyncPulseOutOfRange, // Freq. of sync puls at input out of range
		 };

	LISA_API LISA_Status __stdcall LISA_LogIn();
	LISA_API LISA_Status __stdcall LISA_LogOut();
	LISA_API LISA_Status __stdcall LISA_SetVDR(int value);
	LISA_API LISA_Status __stdcall LISA_SetVVR(int value);
	LISA_API LISA_Status __stdcall LISA_SetSampleFreq(int freq);
	LISA_API LISA_Status __stdcall LISA_SetSyncDelay(int value);
	LISA_API LISA_Status __stdcall LISA_SetSyncWidth(int value);
	LISA_API LISA_Status __stdcall LISA_SetSyncLowActive(bool lowactive);
	LISA_API LISA_Status __stdcall LISA_SetArraySize(int pixel);
	LISA_API LISA_Status __stdcall LISA_GetSensorTemp(int & temp);
	LISA_API LISA_Status __stdcall LISA_SetStart();
	LISA_API LISA_Status __stdcall LISA_SetStop();
	LISA_API LISA_Status __stdcall LISA_GetPixelVoltages(float *volt,
		 int number);
	LISA_API LISA_Status __stdcall LISA_GetPixelValues(int *values, int number);
	LISA_API LISA_Status __stdcall LISA_SetSyncDirection(bool input);
	LISA_API LISA_Status __stdcall LISA_GetSyncFreq(int & freq);

#ifdef __cplusplus
}
#endif

#endif
