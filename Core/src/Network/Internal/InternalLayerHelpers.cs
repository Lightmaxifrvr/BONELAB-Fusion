﻿using LabFusion.Representation;
using LabFusion.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabFusion.Network
{
    /// <summary>
    /// Internal class used for creating network layers and updating them.
    /// </summary>
    internal static class InternalLayerHelpers
    {
        internal static NetworkLayer CurrentNetworkLayer { get; private set; }

        internal static void SetLayer(NetworkLayer layer) {
            CurrentNetworkLayer = layer;
            CurrentNetworkLayer.OnInitializeLayer();
        }

        internal static void OnLateInitializeLayer() {
            if (CurrentNetworkLayer != null)
                CurrentNetworkLayer.OnLateInitializeLayer();
        }

        internal static void OnCleanupLayer() {
            if (CurrentNetworkLayer != null)
                CurrentNetworkLayer.OnCleanupLayer();

            CurrentNetworkLayer = null;
        }

        internal static void OnUpdateLayer() {
            if (CurrentNetworkLayer != null)
                CurrentNetworkLayer.OnUpdateLayer();
        }

        internal static void OnLateUpdateLayer() {
            if (CurrentNetworkLayer != null)
                CurrentNetworkLayer.OnLateUpdateLayer();
        }

        internal static void OnGUILayer() {
            if (CurrentNetworkLayer != null)
                CurrentNetworkLayer.OnGUILayer();
        }
    }
}
