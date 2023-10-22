/*
 * Copyright (c) 2020, NVIDIA CORPORATION.  All rights reserved.
 *
 * NVIDIA CORPORATION and its licensors retain all intellectual property
 * and proprietary rights in and to this software, related documentation
 * and any modifications thereto.  Any use, reproduction, disclosure or
 * distribution of this software and related documentation without an express
 * license agreement from NVIDIA CORPORATION is strictly prohibited.
 */
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace NVIDIA
{
    [AddComponentMenu("NVIDIA/Reflex")]
    public class Reflex : MonoBehaviour
    {
        [Tooltip("Is this the first camera to be rendered")]
        public bool isFirstCamera = true;
        private bool isAttachedBeforeRender = false;
        [Tooltip("Is this the last camera to be rendered")]
        public bool isLastCamera = true;
        private bool isAttachedAfterRender = false;
        [Tooltip("Minimum interval in microseconds for frame synchronization. 0 = no frame rate limit")]
        public uint intervalUs = 0;
        [Tooltip("NVIDIA Reflex Low Latency mode.")]
        public bool isLowLatencyMode = true;
        [Tooltip("NVIDIA Reflex Low Latency Boost")]
        public bool isLowLatencyBoost = false;
        [Tooltip("NVIDIA Reflex Low Latency Boost")]
        public bool useMarkersToOptimize = false;
        private static uint previousIntervalUs = 0;
        private static bool previousIsLowLatencyMode = false;
        private static bool previousIsLowLatencyBoost = true;
        private static bool previousUseMarkersToOptimize = false;
        private bool hasSimulationStarted = false;
        private CommandBuffer renderSubmitStart_CommandBuffer;
        private CommandBuffer renderSubmitEnd_CommandBuffer;
        public bool isIgnored = false;                          // Used by DLSS plugin for downsampled camera.

        public enum NvReflex_LATENCY_MARKER_TYPE
        {
            SIMULATION_START,
            SIMULATION_END,
            RENDERSUBMIT_START,
            RENDERSUBMIT_END,
            PRESENT_START,
            PRESENT_END,
            INPUT_SAMPLE,
            TRIGGER_FLASH,
            PC_LATENCY_PING
        }

        public enum NvReflex_Status : int
        {
            NvReflex_OK = 0,
            NvReflex_ERROR = -1,
            NvReflex_LIBRARY_NOT_FOUND = -2,
            NvReflex_NO_IMPLEMENTATION = -3,
            NvReflex_API_NOT_INITIALIZED = -4,
            NvReflex_INVALID_ARGUMENT = -5,
            NvReflex_NVIDIA_DEVICE_NOT_FOUND = -6,
            NvReflex_END_ENUMERATION = -7,
            NvReflex_INVALID_HANDLE = -8,
            NvReflex_INCOMPATIBLE_STRUCT_VERSION = -9,
            NvReflex_HANDLE_INVALIDATED = -10,
            NvReflex_OPENGL_CONTEXT_NOT_CURRENT = -11,
            NvReflex_INVALID_POINTER = -14,
            NvReflex_NO_GL_EXPERT = -12,
            NvReflex_INSTRUMENTATION_DISABLED = -13,
            NvReflex_NO_GL_NSIGHT = -15,
            NvReflex_EXPECTED_LOGICAL_GPU_HANDLE = -100,
            NvReflex_EXPECTED_PHYSICAL_GPU_HANDLE = -101,
            NvReflex_EXPECTED_DISPLAY_HANDLE = -102,
            NvReflex_INVALID_COMBINATION = -103,
            NvReflex_NOT_SUPPORTED = -104,
            NvReflex_PORTID_NOT_FOUND = -105,
            NvReflex_EXPECTED_UNATTACHED_DISPLAY_HANDLE = -106,
            NvReflex_INVALID_PERF_LEVEL = -107,
            NvReflex_DEVICE_BUSY = -108,
            NvReflex_NV_PERSIST_FILE_NOT_FOUND = -109,
            NvReflex_PERSIST_DATA_NOT_FOUND = -110,
            NvReflex_EXPECTED_TV_DISPLAY = -111,
            NvReflex_EXPECTED_TV_DISPLAY_ON_DCONNECTOR = -112,
            NvReflex_NO_ACTIVE_SLI_TOPOLOGY = -113,
            NvReflex_SLI_RENDERING_MODE_NOTALLOWED = -114,
            NvReflex_EXPECTED_DIGITAL_FLAT_PANEL = -115,
            NvReflex_ARGUMENT_EXCEED_MAX_SIZE = -116,
            NvReflex_DEVICE_SWITCHING_NOT_ALLOWED = -117,
            NvReflex_TESTING_CLOCKS_NOT_SUPPORTED = -118,
            NvReflex_UNKNOWN_UNDERSCAN_CONFIG = -119,
            NvReflex_TIMEOUT_RECONFIGURING_GPU_TOPO = -120,
            NvReflex_DATA_NOT_FOUND = -121,
            NvReflex_EXPECTED_ANALOG_DISPLAY = -122,
            NvReflex_NO_VIDLINK = -123,
            NvReflex_REQUIRES_REBOOT = -124,
            NvReflex_INVALID_HYBRID_MODE = -125,
            NvReflex_MIXED_TARGET_TYPES = -126,
            NvReflex_SYSWOW64_NOT_SUPPORTED = -127,
            NvReflex_IMPLICIT_SET_GPU_TOPOLOGY_CHANGE_NOT_ALLOWED = -128,
            NvReflex_REQUEST_USER_TO_CLOSE_NON_MIGRATABLE_APPS = -129,
            NvReflex_OUT_OF_MEMORY = -130,
            NvReflex_WAS_STILL_DRAWING = -131,
            NvReflex_FILE_NOT_FOUND = -132,
            NvReflex_TOO_MANY_UNIQUE_STATE_OBJECTS = -133,
            NvReflex_INVALID_CALL = -134,
            NvReflex_D3D10_1_LIBRARY_NOT_FOUND = -135,
            NvReflex_FUNCTION_NOT_FOUND = -136,
            NvReflex_INVALID_USER_PRIVILEGE = -137,
            NvReflex_EXPECTED_NON_PRIMARY_DISPLAY_HANDLE = -138,
            NvReflex_EXPECTED_COMPUTE_GPU_HANDLE = -139,
            NvReflex_STEREO_NOT_INITIALIZED = -140,
            NvReflex_STEREO_REGISTRY_ACCESS_FAILED = -141,
            NvReflex_STEREO_REGISTRY_PROFILE_TYPE_NOT_SUPPORTED = -142,
            NvReflex_STEREO_REGISTRY_VALUE_NOT_SUPPORTED = -143,
            NvReflex_STEREO_NOT_ENABLED = -144,
            NvReflex_STEREO_NOT_TURNED_ON = -145,
            NvReflex_STEREO_INVALID_DEVICE_INTERFACE = -146,
            NvReflex_STEREO_PARAMETER_OUT_OF_RANGE = -147,
            NvReflex_STEREO_FRUSTUM_ADJUST_MODE_NOT_SUPPORTED = -148,
            NvReflex_TOPO_NOT_POSSIBLE = -149,
            NvReflex_MODE_CHANGE_FAILED = -150,
            NvReflex_D3D11_LIBRARY_NOT_FOUND = -151,
            NvReflex_INVALID_ADDRESS = -152,
            NvReflex_STRING_TOO_SMALL = -153,
            NvReflex_MATCHING_DEVICE_NOT_FOUND = -154,
            NvReflex_DRIVER_RUNNING = -155,
            NvReflex_DRIVER_NOTRUNNING = -156,
            NvReflex_ERROR_DRIVER_RELOAD_REQUIRED = -157,
            NvReflex_SET_NOT_ALLOWED = -158,
            NvReflex_ADVANCED_DISPLAY_TOPOLOGY_REQUIRED = -159,
            NvReflex_SETTING_NOT_FOUND = -160,
            NvReflex_SETTING_SIZE_TOO_LARGE = -161,
            NvReflex_TOO_MANY_SETTINGS_IN_PROFILE = -162,
            NvReflex_PROFILE_NOT_FOUND = -163,
            NvReflex_PROFILE_NAME_IN_USE = -164,
            NvReflex_PROFILE_NAME_EMPTY = -165,
            NvReflex_EXECUTABLE_NOT_FOUND = -166,
            NvReflex_EXECUTABLE_ALREADY_IN_USE = -167,
            NvReflex_DATATYPE_MISMATCH = -168,
            NvReflex_PROFILE_REMOVED = -169,
            NvReflex_UNREGISTERED_RESOURCE = -170,
            NvReflex_ID_OUT_OF_RANGE = -171,
            NvReflex_DISPLAYCONFIG_VALIDATION_FAILED = -172,
            NvReflex_DPMST_CHANGED = -173,
            NvReflex_INSUFFICIENT_BUFFER = -174,
            NvReflex_ACCESS_DENIED = -175,
            NvReflex_MOSAIC_NOT_ACTIVE = -176,
            NvReflex_SHARE_RESOURCE_RELOCATED = -177,
            NvReflex_REQUEST_USER_TO_DISABLE_DWM = -178,
            NvReflex_D3D_DEVICE_LOST = -179,
            NvReflex_INVALID_CONFIGURATION = -180,
            NvReflex_STEREO_HANDSHAKE_NOT_DONE = -181,
            NvReflex_EXECUTABLE_PATH_IS_AMBIGUOUS = -182,
            NvReflex_DEFAULT_STEREO_PROFILE_IS_NOT_DEFINED = -183,
            NvReflex_DEFAULT_STEREO_PROFILE_DOES_NOT_EXIST = -184,
            NvReflex_CLUSTER_ALREADY_EXISTS = -185,
            NvReflex_DPMST_DISPLAY_ID_EXPECTED = -186,
            NvReflex_INVALID_DISPLAY_ID = -187,
            NvReflex_STREAM_IS_OUT_OF_SYNC = -188,
            NvReflex_INCOMPATIBLE_AUDIO_DRIVER = -189,
            NvReflex_VALUE_ALREADY_SET = -190,
            NvReflex_TIMEOUT = -191,
            NvReflex_GPU_WORKSTATION_FEATURE_INCOMPLETE = -192,
            NvReflex_STEREO_INIT_ACTIVATION_NOT_DONE = -193,
            NvReflex_SYNC_NOT_ACTIVE = -194,
            NvReflex_SYNC_MASTER_NOT_FOUND = -195,
            NvReflex_INVALID_SYNC_TOPOLOGY = -196,
            NvReflex_ECID_SIGN_ALGO_UNSUPPORTED = -197,
            NvReflex_ECID_KEY_VERIFICATION_FAILED = -198,
            NvReflex_FIRMWARE_OUT_OF_DATE = -199,
            NvReflex_FIRMWARE_REVISION_NOT_SUPPORTED = -200,
            NvReflex_LICENSE_CALLER_AUTHENTICATION_FAILED = -201,
            NvReflex_D3D_DEVICE_NOT_REGISTERED = -202,
            NvReflex_RESOURCE_NOT_ACQUIRED = -203,
            NvReflex_TIMING_NOT_SUPPORTED = -204,
            NvReflex_HDCP_ENCRYPTION_FAILED = -205,
            NvReflex_PCLK_LIMITATION_FAILED = -206,
            NvReflex_NO_CONNECTOR_FOUND = -207,
            NvReflex_HDCP_DISABLED = -208,
            NvReflex_API_IN_USE = -209,
            NvReflex_NVIDIA_DISPLAY_NOT_FOUND = -210,
            NvReflex_PRIV_SEC_VIOLATION = -211,
            NvReflex_INCORRECT_VENDOR = -212,
            NvReflex_DISPLAY_IN_USE = -213,
            NvReflex_UNSUPPORTED_CONFIG_NON_HDCP_HMD = -214,
            NvReflex_MAX_DISPLAY_LIMIT_REACHED = -215,
            NvReflex_INVALID_DIRECT_MODE_DISPLAY = -216,
            NvReflex_GPU_IN_DEBUG_MODE = -217,
            NvReflex_D3D_CONTEXT_NOT_FOUND = -218,
            NvReflex_STEREO_VERSION_MISMATCH = -219,
            NvReflex_GPU_NOT_POWERED = -220,
            NvReflex_ERROR_DRIVER_RELOAD_IN_PROGRESS = -221,
            NvReflex_WAIT_FOR_HW_RESOURCE = -222,
            NvReflex_REQUIRE_FURTHER_HDCP_ACTION = -223,
            NvReflex_DISPLAY_MUX_TRANSITION_FAILED = -224,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvReflex_FrameReport
        {
            public ulong frameID;
            public ulong inputSampleTime;
            public ulong simStartTime;
            public ulong simEndTime;
            public ulong renderSubmitStartTime;
            public ulong renderSubmitEndTime;
            public ulong presentStartTime;
            public ulong presentEndTime;
            public ulong driverStartTime;
            public ulong driverEndTime;
            public ulong osRenderQueueStartTime;
            public ulong osRenderQueueEndTime;
            public ulong gpuRenderStartTime;
            public ulong gpuRenderEndTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvReflex_LATENCY_RESULT_PARAMS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public NvReflex_FrameReport[] frameReport;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NvReflex_GET_SLEEP_STATUS_PARAMS
        {
            public byte bLowLatencyMode;
        }

        private IEnumerator SleepCoroutine()
        {
            while (true)
            {
                yield return (new WaitForEndOfFrame());

                if (NvReflex_Plugin_HasReceivedRenderEvent())
                {
                    if (isLastCamera)
                    {
                        if ((previousIsLowLatencyMode != isLowLatencyMode) || (previousIntervalUs != intervalUs) || (previousIsLowLatencyBoost != isLowLatencyBoost) || (previousUseMarkersToOptimize != useMarkersToOptimize))
                        {
                            NvReflex_Plugin_SetSleepMode(isLowLatencyMode, intervalUs, isLowLatencyBoost, useMarkersToOptimize);
                            previousIsLowLatencyMode = isLowLatencyMode;
                            previousIsLowLatencyBoost = isLowLatencyBoost;
                            previousIntervalUs = intervalUs;
                            previousUseMarkersToOptimize = useMarkersToOptimize;
                        }
                        if (isLowLatencyMode)
                        {
                            GL.Flush();
                            NvReflex_Plugin_Sleep();
                        }
                    }
                }
            }
        }

        private void Start()
        {
            renderSubmitStart_CommandBuffer = new CommandBuffer();
            renderSubmitStart_CommandBuffer.name = "Reflex Submit start";
            renderSubmitEnd_CommandBuffer = new CommandBuffer();
            renderSubmitEnd_CommandBuffer.name = "Reflex Submit End";
        }

        void OnDestroy()
        {
        }

        private void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] camera)
        {
            context.ExecuteCommandBuffer(renderSubmitStart_CommandBuffer);
        }

        private void OnEndFrameRendering(ScriptableRenderContext context, Camera[] camera)
        {
            context.ExecuteCommandBuffer(renderSubmitEnd_CommandBuffer);
            context.Submit();
        }

        private void AfterPresent()
        {
            if (hasSimulationStarted == false)
            {
                NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE.SIMULATION_START, (ulong)Time.frameCount);

                hasSimulationStarted = true;
            }
        }

        public void FixedUpdate()
        {
            if(isFirstCamera & !isIgnored & NvReflex_Plugin_HasReceivedRenderEvent())
            {
                AfterPresent();
            }
        }

        public void Update()
        {
            if (isFirstCamera & !isIgnored & NvReflex_Plugin_HasReceivedRenderEvent())
            {

                AfterPresent();
                NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE.INPUT_SAMPLE, (ulong)Time.frameCount);

                if (Input.GetKeyUp(KeyCode.F13))
                {
                    PCLatencyPing();
                }
            }
        }

        private void AttachDetachCommandBuffers()
        {
            if ((renderSubmitStart_CommandBuffer != null) & (renderSubmitEnd_CommandBuffer != null))
            {
                Camera camComponent = this.GetComponent<Camera>();
                bool shouldFirstCameraBeAttached = (isFirstCamera & !isIgnored & enabled);
                bool shouldLastCameraBeAttached = (isLastCamera & /*!isIgnored &*/ enabled);

                if (shouldFirstCameraBeAttached != isAttachedBeforeRender)
                {
                    if (shouldFirstCameraBeAttached)
                    {
                        // Send renderSubmitStart_CommandBuffer on BeforeDepthTexture, BeforeForwardOpaque and BeforeGBuffer events. This ensures whatever configuration the Legacy pipeline is using
                        // with regard to 2D/3D and Deferred/Rendering the event will be sent.  The plugin filters the SubmitStart event and only processes the first that occurs.
                        var oldBDCmdBuffers = camComponent.GetCommandBuffers(CameraEvent.BeforeDepthTexture);
                        var oldBOCmdBuffers = camComponent.GetCommandBuffers(CameraEvent.BeforeForwardOpaque);
                        var oldBDGmdBuffers = camComponent.GetCommandBuffers(CameraEvent.BeforeGBuffer);
                        camComponent.RemoveCommandBuffers(CameraEvent.BeforeDepthTexture);
                        camComponent.RemoveCommandBuffers(CameraEvent.BeforeForwardOpaque);
                        camComponent.RemoveCommandBuffers(CameraEvent.BeforeGBuffer);
                        camComponent.AddCommandBuffer(CameraEvent.BeforeDepthTexture, renderSubmitStart_CommandBuffer);
                        camComponent.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, renderSubmitStart_CommandBuffer);
                        camComponent.AddCommandBuffer(CameraEvent.BeforeGBuffer, renderSubmitStart_CommandBuffer);
                        foreach (var cmdBuff in oldBDCmdBuffers)
                        {
                            camComponent.AddCommandBuffer(CameraEvent.BeforeDepthTexture, cmdBuff);
                        }
                        foreach (var cmdBuff in oldBOCmdBuffers)
                        {
                            camComponent.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, cmdBuff);
                        }
                        foreach (var cmdBuff in oldBDGmdBuffers)
                        {
                            camComponent.AddCommandBuffer(CameraEvent.BeforeGBuffer, cmdBuff);
                        }
                        RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
                    }
                    else
                    {
                        camComponent.RemoveCommandBuffer(CameraEvent.BeforeDepthTexture, renderSubmitStart_CommandBuffer);
                        camComponent.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, renderSubmitStart_CommandBuffer);
                        camComponent.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, renderSubmitStart_CommandBuffer);
                        RenderPipelineManager.beginFrameRendering -= OnBeginFrameRendering;
                    }
                    isAttachedBeforeRender = shouldFirstCameraBeAttached;
                }

                if (shouldLastCameraBeAttached != isAttachedAfterRender)
                {
                    if (shouldLastCameraBeAttached)
                    {
                        camComponent.AddCommandBuffer(CameraEvent.AfterEverything, renderSubmitEnd_CommandBuffer);
                        RenderPipelineManager.endFrameRendering += OnEndFrameRendering;
                    }
                    else
                    {
                        camComponent.RemoveCommandBuffer(CameraEvent.AfterEverything, renderSubmitEnd_CommandBuffer);
                        RenderPipelineManager.endFrameRendering -= OnEndFrameRendering;
                    }
                    isAttachedAfterRender = shouldLastCameraBeAttached;
                }
            }
        }

        void OnEnable()
        {
            AttachDetachCommandBuffers();
            StartCoroutine(SleepCoroutine());
        }

        void OnDisable()
        {
            AttachDetachCommandBuffers();
            StopCoroutine(SleepCoroutine());
        }

        private void LateUpdate()
        {
            AttachDetachCommandBuffers();

            if (isFirstCamera & !isIgnored)
            {
                if (hasSimulationStarted & NvReflex_Plugin_HasReceivedRenderEvent())
                {
                    NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE.SIMULATION_END, (ulong)Time.frameCount);
                    hasSimulationStarted = false;
                }
                int startEventID = NvReflex_Plugin_GetEventIDForEvent((int)NvReflex_LATENCY_MARKER_TYPE.RENDERSUBMIT_START);
                renderSubmitStart_CommandBuffer.Clear();
                renderSubmitStart_CommandBuffer.IssuePluginEventAndData(NvReflex_Plugin_GetRenderEventAndDataFunc(), startEventID, (IntPtr)(ulong)Time.frameCount);
            }
            if (isLastCamera /*& !isIgnored*/)
            {
                int endEventID = NvReflex_Plugin_GetEventIDForEvent((int)NvReflex_LATENCY_MARKER_TYPE.RENDERSUBMIT_END);
                renderSubmitEnd_CommandBuffer.Clear();
                renderSubmitEnd_CommandBuffer.IssuePluginEventAndData(NvReflex_Plugin_GetRenderEventAndDataFunc(), endEventID, (IntPtr)(ulong)Time.frameCount);
            }
        }

        public static void TriggerFlash()
        {
            if (NvReflex_Plugin_HasReceivedRenderEvent())
            {
                NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE.TRIGGER_FLASH, (ulong)Time.frameCount);
            }
        }

        public static void PCLatencyPing()
        {
            if (NvReflex_Plugin_HasReceivedRenderEvent())
            {
                NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE.PC_LATENCY_PING, (ulong)Time.frameCount);
            }
        }

        public static NvReflex_Status IsReflexLowLatencySupported()
        {
            if (NvReflex_Plugin_HasReceivedRenderEvent())
            {
                NvReflex_Status status = NvReflex_Status.NvReflex_ERROR;
                NvReflex_GET_SLEEP_STATUS_PARAMS sleepParams;
                uint driverVersion = 0;

                status = NvReflex_Plugin_GetDriverVersion(out driverVersion);

                if ((status != NvReflex_Status.NvReflex_OK) || (driverVersion < 45500))
                {
                    return NvReflex_Status.NvReflex_ERROR;
                }

                status = NvReflex_Plugin_GetSleepStatus(out sleepParams);
                return (status == NvReflex_Status.NvReflex_OK) ? NvReflex_Status.NvReflex_OK : NvReflex_Status.NvReflex_ERROR;
            }
            return NvReflex_Status.NvReflex_API_NOT_INITIALIZED;
        }

        public static NvReflex_Status IsAutomaticReflexAnalyzerSupported()
        {
            if (NvReflex_Plugin_HasReceivedRenderEvent())
            {
                NvReflex_Status status = NvReflex_Status.NvReflex_ERROR;
                uint driverVersion = 0;

                status = NvReflex_Plugin_GetDriverVersion(out driverVersion);

                return ((status == NvReflex_Status.NvReflex_OK) && (driverVersion >= 51179)) ? NvReflex_Status.NvReflex_OK : NvReflex_Status.NvReflex_ERROR;
            }
            return NvReflex_Status.NvReflex_API_NOT_INITIALIZED;
        }

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_SetSleepMode(bool lowLatencyMode, uint minimumIntervalUs, bool lowLatencyBoost, bool markersOptimzation );

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_Sleep();

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_SetLatencyMarker(NvReflex_LATENCY_MARKER_TYPE marker, ulong frameID);

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_GetLatency(out NvReflex_LATENCY_RESULT_PARAMS markers);

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_GetSleepStatus(out NvReflex_GET_SLEEP_STATUS_PARAMS sleepStatusParams);

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int NvReflex_Plugin_GetEventIDForEvent(int inEvent);

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr NvReflex_Plugin_GetRenderEventAndDataFunc();

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool NvReflex_Plugin_HasReceivedRenderEvent();

        [DllImport("GfxPluginNVIDIAReflex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern NvReflex_Status NvReflex_Plugin_GetDriverVersion(out uint ver);
    }
}
