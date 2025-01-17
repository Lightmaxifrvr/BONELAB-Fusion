﻿using Il2CppSLZ.Bonelab;

using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Preferences;
using LabFusion.Senders;

using UnityEngine;

namespace LabFusion.Utilities;

internal static class PhysicsUtilities
{
    internal static void OnUpdateTimescale()
    {
        if (!NetworkInfo.HasServer)
        {
            return;
        }

        var mode = FusionPreferences.TimeScaleMode;
        var references = RigData.RigReferences;
        var rm = references.RigManager;

        switch (mode)
        {
            case TimeScaleMode.LOW_GRAVITY:
                if (!RigData.HasPlayer)
                {
                    break;
                }

                float intensity = TimeManager.cur_intensity;
                if (intensity <= 0f)
                {
                    break;
                }

                float mult = 1f - (1f / TimeManager.cur_intensity);

                Vector3 force = -Physics.gravity * mult;

                foreach (var rb in rm.physicsRig.selfRbs)
                {
                    if (rb.useGravity)
                    {
                        rb.AddForce(force, ForceMode.Acceleration);
                    }
                }

                break;
        }
    }
}