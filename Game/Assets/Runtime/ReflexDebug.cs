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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;
using NVIDIA;

namespace NVIDIA
{
    [AddComponentMenu("NVIDIA/Reflex Debug")]
    public class ReflexDebug : MonoBehaviour
    {
        private bool m_Enabled = false;
        private Reflex.NvReflex_LATENCY_RESULT_PARAMS markers;
        private Reflex.NvReflex_GET_SLEEP_STATUS_PARAMS sleepStatusParams;
        private Reflex m_Reflex;

        void Start()
        {
            m_Reflex = GetComponent<Reflex>();
        }

        private void Update()
        {
            if (Reflex.NvReflex_Plugin_HasReceivedRenderEvent())
            {
                Reflex.NvReflex_Plugin_GetLatency(out markers);
                Reflex.NvReflex_Plugin_GetSleepStatus(out sleepStatusParams);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                m_Enabled = !m_Enabled;
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                m_Reflex.isLowLatencyMode = !m_Reflex.isLowLatencyMode;
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                m_Reflex.isLowLatencyBoost = !m_Reflex.isLowLatencyBoost;
            }
        }

        private void AddScrollbar(float range, float size, float start)
        {
            GUILayout.HorizontalScrollbar(start+(size / 2.0f), size, 0, range, GUILayout.Width(range * 5));
        }

        private void OnGUI()
        {
            if (m_Enabled)
            {
                Reflex.NvReflex_Status rllSupported = Reflex.IsReflexLowLatencySupported();
                Reflex.NvReflex_Status araSupported = Reflex.IsAutomaticReflexAnalyzerSupported();

                GUILayout.BeginArea(new Rect { x = 10, y = 10, width = 340, height = 500 }, "Reflex Information", GUI.skin.window);

                GUILayout.Label(String.Format("NVIDIA Reflex Low Latency mode supported: {0}", (rllSupported == Reflex.NvReflex_Status.NvReflex_OK) ? "Yes" : ((rllSupported == Reflex.NvReflex_Status.NvReflex_ERROR) ? "No" : "Unknown")));
                GUILayout.Label(String.Format("NVIDIA Automatic Reflex analyzer supported: {0}", (araSupported == Reflex.NvReflex_Status.NvReflex_OK) ? "Yes" : ((araSupported == Reflex.NvReflex_Status.NvReflex_ERROR) ? "No" : "Unknown")));

                GUILayout.Label(String.Format("Requested NVIDIA Reflex Low Latency mode: {0}", m_Reflex.isLowLatencyMode));
                GUILayout.Label(String.Format("Reported NVIDIA Reflex Low Latency mode: {0}", sleepStatusParams.bLowLatencyMode == 0 ? false : true));
                GUILayout.Label(String.Format("NVIDIA Reflex Low Latency Boost: {0}", m_Reflex.isLowLatencyBoost));
                GUILayout.Label(String.Format("Frame ID: {0}", markers.frameReport[63].frameID));
                GUILayout.Label(String.Format("Game and Render Latency(ms): {0:0.00}", (markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0));
                GUILayout.Label(String.Format("Simulation Delta(ms): {0:0.00}", (markers.frameReport[63].simEndTime - markers.frameReport[63].simStartTime) / 1000.0));
                AddScrollbar((markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (markers.frameReport[63].simEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (0) / 1000.0f
                    );
                GUILayout.Label(String.Format("Render Submit Delta(ms): {0:0.00}", (markers.frameReport[63].renderSubmitEndTime - markers.frameReport[63].renderSubmitStartTime) / 1000.0));
                AddScrollbar((markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (markers.frameReport[63].renderSubmitEndTime - markers.frameReport[63].renderSubmitStartTime) / 1000.0f,
                    (markers.frameReport[63].renderSubmitStartTime - markers.frameReport[63].simStartTime) / 1000.0f
                    );
                GUILayout.Label(String.Format("Driver Delta(ms): {0:0.00}", (markers.frameReport[63].driverEndTime - markers.frameReport[63].driverStartTime) / 1000.0));
                AddScrollbar((markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (markers.frameReport[63].driverEndTime - markers.frameReport[63].driverStartTime) / 1000.0f,
                    (markers.frameReport[63].driverStartTime - markers.frameReport[63].simStartTime) / 1000.0f
                    );
                GUILayout.Label(String.Format("OS Render Queue Delta(ms): {0:0.00}", (markers.frameReport[63].osRenderQueueEndTime - markers.frameReport[63].osRenderQueueStartTime) / 1000.0));
                AddScrollbar((markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (markers.frameReport[63].osRenderQueueEndTime - markers.frameReport[63].osRenderQueueStartTime) / 1000.0f,
                    (markers.frameReport[63].osRenderQueueStartTime - markers.frameReport[63].simStartTime) / 1000.0f
                    );
                GUILayout.Label(String.Format("GPU Render Delta(ms): {0:0.00}", (markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].gpuRenderStartTime) / 1000.0));
                AddScrollbar((markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].simStartTime) / 1000.0f,
                    (markers.frameReport[63].gpuRenderEndTime - markers.frameReport[63].gpuRenderStartTime) / 1000.0f,
                    (markers.frameReport[63].gpuRenderStartTime - markers.frameReport[63].simStartTime) / 1000.0f
                    );

                GUILayout.EndArea();
            }
        }
    }
}
