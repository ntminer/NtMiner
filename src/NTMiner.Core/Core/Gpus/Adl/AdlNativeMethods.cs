using System;
using System.Runtime.InteropServices;

namespace NTMiner.Core.Gpus.Adl {
    #region Export Delegates
    /// <summary> ADL Memory allocation function allows ADL to callback for memory allocation</summary>
    /// <param name="size">input size</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate IntPtr ADL_Main_Memory_Alloc(int size);

    // ///// <summary> ADL Create Function to create ADL Data</summary>
    /// <param name="callback">Call back functin pointer which is ised to allocate memeory </param>
    /// <param name="enumConnectedAdapters">If it is 1, then ADL will only retuen the physical exist adapters </param>
    ///// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

    internal delegate int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

    /// <summary> ADL Destroy Function to free up ADL Data</summary>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Main_Control_Destroy();

    internal delegate int ADL2_Main_Control_Destroy(IntPtr context);

    /// <summary> ADL Function to get the number of adapters</summary>
    /// <param name="numAdapters">return number of adapters</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

    /// <summary> ADL Function to get the GPU adapter information</summary>
    /// <param name="info">return GPU adapter information</param>
    /// <param name="inputSize">the size of the GPU adapter struct</param>
    /// <returns> retrun ADL Error Code</returns>
    internal delegate int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);

    internal delegate int ADL2_Adapter_AdapterInfo_Get(IntPtr context, IntPtr lpInfo, int iInputSize);

    /// <summary> Function to determine if the adapter is active or not.</summary>
    /// <remarks>The function is used to check if the adapter associated with iAdapterIndex is active</remarks>  
    /// <param name="adapterIndex"> Adapter Index.</param>
    /// <param name="status"> Status of the adapter. True: Active; False: Dsiabled</param>
    /// <returns>Non zero is successfull</returns> 
    internal delegate int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

    /// <summary>Get display information based on adapter index</summary>
    /// <param name="adapterIndex">Adapter Index</param>
    /// <param name="numDisplays">return the total number of supported displays</param>
    /// <param name="displayInfoArray">return ADLDisplayInfo Array for supported displays' information</param>
    /// <param name="forceDetect">force detect or not</param>
    /// <returns>return ADL Error Code</returns>
    internal delegate int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

    internal delegate int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

    internal delegate int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue temperature);

    internal delegate int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int iAdapterIndex, int iPowerType, ref int lpCurrentValue);

    internal delegate int ADL_Adapter_ID_Get(int iAdapterIndex, ref int lpAdapterID);

    internal delegate int ADL_Main_Control_Refresh();

    internal delegate int ADL2_Main_Control_Refresh(IntPtr hHandle);

    #endregion Export Delegates

    #region ADL Class
    /// <summary> ADL Class</summary>
    internal static class AdlNativeMethods {
        #region Class ADLImport
        /// <summary> ADLImport class</summary>
        private static class ADLImport {
            #region Internal Constant
            /// <summary> Atiadlxx_FileName </summary>
            internal const string Atiadlxx_FileName = "atiadlxx.dll";
            /// <summary> Kernel32_FileName </summary>
            internal const string Kernel32_FileName = "kernel32.dll";
            #endregion Internal Constant

            #region DLLImport
            [DllImport(Kernel32_FileName, CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr GetModuleHandle(string moduleName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, ref IntPtr context);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_Destroy();

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Main_Control_Destroy(IntPtr context);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Main_Control_IsFunctionValid(IntPtr module, string procName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern IntPtr ADL_Main_Control_GetProcAddress(IntPtr module, string procName);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_NumberOfAdapters_Get(ref int numAdapters);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_AdapterInfo_Get(IntPtr info, int inputSize);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Adapter_AdapterInfo_Get(IntPtr context, IntPtr lpInfo, int iInputSize);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Adapter_Active_Get(int adapterIndex, ref int status);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Display_DisplayInfo_Get(int adapterIndex, ref int numDisplays, out IntPtr displayInfoArray, int forceDetect);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL_Overdrive5_FanSpeed_Get(int adapterIndex, int thermalControllerIndex, ref ADLFanSpeedValue fanSpeedValue);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            internal static extern int ADL2_Overdrive6_CurrentPower_Get(IntPtr context, int iAdapterIndex, int iPowerType, ref int lpCurrentValue);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ADL_Adapter_ID_Get(int iAdapterIndex, ref int lpAdapterID);

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ADL_Main_Control_Refresh();

            [DllImport(Atiadlxx_FileName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int ADL2_Main_Control_Refresh(IntPtr hHandle);

            #endregion DLLImport
        }
        #endregion Class ADLImport

        #region Class ADLCheckLibrary
        /// <summary> ADLCheckLibrary class</summary>
        private class ADLCheckLibrary {
            #region Private Members
            private IntPtr ADLLibrary = IntPtr.Zero;
            #endregion Private Members

            #region Static Members
            /// <summary> new a private instance</summary>
            private static ADLCheckLibrary ADLCheckLibrary_ = new ADLCheckLibrary();
            #endregion Static Members

            #region Constructor
            /// <summary> Constructor</summary>
            private ADLCheckLibrary() {
                try {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(IntPtr.Zero, "ADL_Main_Control_Create")) {
                        ADLLibrary = ADLImport.GetModuleHandle(ADLImport.Atiadlxx_FileName);
                    }
                }
                catch (DllNotFoundException e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                catch (EntryPointNotFoundException e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            }
            #endregion Constructor

            #region Destructor
            /// <summary> Destructor to force calling ADL Destroy function before free up the ADL library</summary>
            ~ADLCheckLibrary() {
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary) {
                    ADLImport.ADL_Main_Control_Destroy();
                }
            }
            #endregion Destructor

            #region Static IsFunctionValid
            /// <summary> Check the import function to see it exists or not</summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return true, if function exists</returns>
            internal static bool IsFunctionValid(string functionName) {
                bool result = false;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary) {
                    if (1 == ADLImport.ADL_Main_Control_IsFunctionValid(ADLCheckLibrary_.ADLLibrary, functionName)) {
                        result = true;
                    }
                }
                return result;
            }
            #endregion Static IsFunctionValid

            #region Static GetProcAddress
            /// <summary> Get the unmanaged function pointer </summary>
            /// <param name="functionName"> function name</param>
            /// <returns>return function pointer, if function exists</returns>
            internal static IntPtr GetProcAddress(string functionName) {
                IntPtr result = System.IntPtr.Zero;
                if (System.IntPtr.Zero != ADLCheckLibrary_.ADLLibrary) {
                    result = ADLImport.ADL_Main_Control_GetProcAddress(ADLCheckLibrary_.ADLLibrary, functionName);
                }
                return result;
            }
            #endregion Static GetProcAddress
        }
        #endregion Class ADLCheckLibrary

        #region Export Functions

        #region ADL_Main_Memory_Alloc
        /// <summary> Build in memory allocation function</summary>
        internal static ADL_Main_Memory_Alloc ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;
        /// <summary> Build in memory allocation function</summary>
        /// <param name="size">input size</param>
        /// <returns>return the memory buffer</returns>
        private static IntPtr ADL_Main_Memory_Alloc_(int size) {
            IntPtr result = Marshal.AllocCoTaskMem(size);
            return result;
        }
        #endregion ADL_Main_Memory_Alloc

        #region ADL_Main_Memory_Free
        /// <summary> Build in memory free function</summary>
        /// <param name="buffer">input buffer</param>
        internal static void ADL_Main_Memory_Free(IntPtr buffer) {
            if (IntPtr.Zero != buffer) {
                Marshal.FreeCoTaskMem(buffer);
            }
        }
        #endregion ADL_Main_Memory_Free

        #region ADL_Main_Control_Create
        /// <summary> ADL_Main_Control_Create Delegates</summary>
        internal static ADL_Main_Control_Create ADL_Main_Control_Create {
            get {
                if (!ADL_Main_Control_Create_Check && null == ADL_Main_Control_Create_) {
                    ADL_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create")) {
                        ADL_Main_Control_Create_ = ADLImport.ADL_Main_Control_Create;
                    }
                }
                return ADL_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Create ADL_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Create_Check = false;
        /// <summary> ADL_Main_Control_Create Delegates</summary>
        internal static ADL2_Main_Control_Create ADL2_Main_Control_Create {
            get {
                if (!ADL2_Main_Control_Create_Check && null == ADL2_Main_Control_Create_) {
                    ADL2_Main_Control_Create_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Create")) {
                        ADL2_Main_Control_Create_ = ADLImport.ADL2_Main_Control_Create;
                    }
                }
                return ADL2_Main_Control_Create_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Create ADL2_Main_Control_Create_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Create_Check = false;
        #endregion ADL_Main_Control_Create

        #region ADL_Main_Control_Destroy
        /// <summary> ADL_Main_Control_Destroy Delegates</summary>
        internal static ADL_Main_Control_Destroy ADL_Main_Control_Destroy {
            get {
                if (!ADL_Main_Control_Destroy_Check && null == ADL_Main_Control_Destroy_) {
                    ADL_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy")) {
                        ADL_Main_Control_Destroy_ = ADLImport.ADL_Main_Control_Destroy;
                    }
                }
                return ADL_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Main_Control_Destroy ADL_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Main_Control_Destroy_Check = false;
        internal static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy {
            get {
                if (!ADL2_Main_Control_Destroy_Check && null == ADL2_Main_Control_Destroy_) {
                    ADL2_Main_Control_Destroy_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Destroy")) {
                        ADL2_Main_Control_Destroy_ = ADLImport.ADL2_Main_Control_Destroy;
                    }
                }
                return ADL2_Main_Control_Destroy_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Main_Control_Destroy ADL2_Main_Control_Destroy_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Main_Control_Destroy_Check = false;
        #endregion ADL_Main_Control_Destroy

        #region ADL_Adapter_NumberOfAdapters_Get
        /// <summary> ADL_Adapter_NumberOfAdapters_Get Delegates</summary>
        internal static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get {
            get {
                if (!ADL_Adapter_NumberOfAdapters_Get_Check && null == ADL_Adapter_NumberOfAdapters_Get_) {
                    ADL_Adapter_NumberOfAdapters_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_NumberOfAdapters_Get")) {
                        ADL_Adapter_NumberOfAdapters_Get_ = ADLImport.ADL_Adapter_NumberOfAdapters_Get;
                    }
                }
                return ADL_Adapter_NumberOfAdapters_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_NumberOfAdapters_Get ADL_Adapter_NumberOfAdapters_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_NumberOfAdapters_Get_Check = false;
        #endregion ADL_Adapter_NumberOfAdapters_Get

        #region ADL_Adapter_AdapterInfo_Get
        /// <summary> ADL_Adapter_AdapterInfo_Get Delegates</summary>
        internal static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get {
            get {
                if (!ADL_Adapter_AdapterInfo_Get_Check && null == ADL_Adapter_AdapterInfo_Get_) {
                    ADL_Adapter_AdapterInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get")) {
                        ADL_Adapter_AdapterInfo_Get_ = ADLImport.ADL_Adapter_AdapterInfo_Get;
                    }
                }
                return ADL_Adapter_AdapterInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_AdapterInfo_Get ADL_Adapter_AdapterInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_AdapterInfo_Get_Check = false;

        /// <summary> ADL_Adapter_AdapterInfo_Get Delegates</summary>
        internal static ADL2_Adapter_AdapterInfo_Get ADL2_Adapter_AdapterInfo_Get {
            get {
                if (!ADL2_Adapter_AdapterInfo_Get_Check && null == ADL2_Adapter_AdapterInfo_Get_) {
                    ADL2_Adapter_AdapterInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_AdapterInfo_Get")) {
                        ADL2_Adapter_AdapterInfo_Get_ = ADLImport.ADL2_Adapter_AdapterInfo_Get;
                    }
                }
                return ADL2_Adapter_AdapterInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL2_Adapter_AdapterInfo_Get ADL2_Adapter_AdapterInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL2_Adapter_AdapterInfo_Get_Check = false;
        #endregion ADL_Adapter_AdapterInfo_Get

        #region ADL_Adapter_Active_Get
        /// <summary> ADL_Adapter_Active_Get Delegates</summary>
        internal static ADL_Adapter_Active_Get ADL_Adapter_Active_Get {
            get {
                if (!ADL_Adapter_Active_Get_Check && null == ADL_Adapter_Active_Get_) {
                    ADL_Adapter_Active_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_Active_Get")) {
                        ADL_Adapter_Active_Get_ = ADLImport.ADL_Adapter_Active_Get;
                    }
                }
                return ADL_Adapter_Active_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Adapter_Active_Get ADL_Adapter_Active_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Adapter_Active_Get_Check = false;
        #endregion ADL_Adapter_Active_Get

        #region ADL_Display_DisplayInfo_Get
        /// <summary> ADL_Display_DisplayInfo_Get Delegates</summary>
        internal static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get {
            get {
                if (!ADL_Display_DisplayInfo_Get_Check && null == ADL_Display_DisplayInfo_Get_) {
                    ADL_Display_DisplayInfo_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Display_DisplayInfo_Get")) {
                        ADL_Display_DisplayInfo_Get_ = ADLImport.ADL_Display_DisplayInfo_Get;
                    }
                }
                return ADL_Display_DisplayInfo_Get_;
            }
        }
        /// <summary> Private Delegate</summary>
        private static ADL_Display_DisplayInfo_Get ADL_Display_DisplayInfo_Get_ = null;
        /// <summary> check flag to indicate the delegate has been checked</summary>
        private static bool ADL_Display_DisplayInfo_Get_Check = false;
        #endregion ADL_Display_DisplayInfo_Get

        internal static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get {
            get {
                if (!ADL_Overdrive5_Temperature_Get_Check && null == ADL_Overdrive5_Temperature_Get_) {
                    ADL_Overdrive5_Temperature_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_Temperature_Get")) {
                        ADL_Overdrive5_Temperature_Get_ = ADLImport.ADL_Overdrive5_Temperature_Get;
                    }
                }
                return ADL_Overdrive5_Temperature_Get_;
            }
        }
        private static ADL_Overdrive5_Temperature_Get ADL_Overdrive5_Temperature_Get_ = null;
        private static bool ADL_Overdrive5_Temperature_Get_Check = false;

        internal static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get {
            get {
                if (!ADL_Overdrive5_FanSpeed_Get_Check && null == ADL_Overdrive5_FanSpeed_Get_) {
                    ADL_Overdrive5_FanSpeed_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Overdrive5_FanSpeed_Get")) {
                        ADL_Overdrive5_FanSpeed_Get_ = ADLImport.ADL_Overdrive5_FanSpeed_Get;
                    }
                }
                return ADL_Overdrive5_FanSpeed_Get_;
            }
        }
        private static ADL_Overdrive5_FanSpeed_Get ADL_Overdrive5_FanSpeed_Get_ = null;
        private static bool ADL_Overdrive5_FanSpeed_Get_Check = false;

        internal static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get {
            get {
                if (!ADL2_Overdrive6_CurrentPower_Get_Check && null == ADL2_Overdrive6_CurrentPower_Get_) {
                    ADL2_Overdrive6_CurrentPower_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Overdrive6_CurrentPower_Get")) {
                        ADL2_Overdrive6_CurrentPower_Get_ = ADLImport.ADL2_Overdrive6_CurrentPower_Get;
                    }
                }

                return ADL2_Overdrive6_CurrentPower_Get_;
            }
        }

        private static ADL2_Overdrive6_CurrentPower_Get ADL2_Overdrive6_CurrentPower_Get_ = null;
        private static bool ADL2_Overdrive6_CurrentPower_Get_Check = false;

        internal static ADL_Adapter_ID_Get ADL_Adapter_ID_Get {
            get {
                if (!ADL_Adapter_ID_Get_Check && null == ADL_Adapter_ID_Get_) {
                    ADL_Adapter_ID_Get_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Adapter_ID_Get")) {
                        ADL_Adapter_ID_Get_ = ADLImport.ADL_Adapter_ID_Get;
                    }
                }

                return ADL_Adapter_ID_Get_;
            }
        }

        private static ADL_Adapter_ID_Get ADL_Adapter_ID_Get_ = null;
        private static bool ADL_Adapter_ID_Get_Check = false;

        internal static ADL_Main_Control_Refresh ADL_Main_Control_Refresh {
            get {
                if (!ADL_Main_Control_Refresh_Check && null == ADL_Main_Control_Refresh_) {
                    ADL_Main_Control_Refresh_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL_Main_Control_Refresh")) {
                        ADL_Main_Control_Refresh_ = ADLImport.ADL_Main_Control_Refresh;
                    }
                }

                return ADL_Main_Control_Refresh_;
            }
        }

        private static ADL_Main_Control_Refresh ADL_Main_Control_Refresh_ = null;
        private static bool ADL_Main_Control_Refresh_Check = false;

        internal static ADL2_Main_Control_Refresh ADL2_Main_Control_Refresh {
            get {
                if (!ADL2_Main_Control_Refresh_Check && null == ADL2_Main_Control_Refresh_) {
                    ADL2_Main_Control_Refresh_Check = true;
                    if (ADLCheckLibrary.IsFunctionValid("ADL2_Main_Control_Refresh")) {
                        ADL2_Main_Control_Refresh_ = ADLImport.ADL2_Main_Control_Refresh;
                    }
                }

                return ADL2_Main_Control_Refresh_;
            }
        }

        private static ADL2_Main_Control_Refresh ADL2_Main_Control_Refresh_ = null;
        private static bool ADL2_Main_Control_Refresh_Check = false;

        #endregion Export Functions
    }
    #endregion ADL Class
}
