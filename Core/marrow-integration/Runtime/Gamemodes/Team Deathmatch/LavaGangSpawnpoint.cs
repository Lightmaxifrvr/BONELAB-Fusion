﻿using System;

using UnityEngine;

#if MELONLOADER
using MelonLoader;

using LabFusion.Utilities;
#endif

namespace LabFusion.MarrowIntegration {
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("BONELAB Fusion/Gamemodes/Lava Gang Spawnpoint")]
    [DisallowMultipleComponent]
#endif
    public sealed class LavaGangSpawnpoint : FusionMarrowBehaviour {
#if MELONLOADER
        public LavaGangSpawnpoint(IntPtr intPtr) : base(intPtr) { }

        public static readonly FusionComponentCache<GameObject, LavaGangSpawnpoint> Cache = new FusionComponentCache<GameObject, LavaGangSpawnpoint>();

        private void Awake() {
            Cache.Add(gameObject, this);
        }

        private void OnDestroy() {
            Cache.Remove(gameObject);
        }
#else
        public override string Comment => "Creates a spawn point for players on the Lava Gang team during Team Deathmatch.\n" +
            "You can have as many of these in your scene as you want, and it will become a random spawn.";
#endif
    }
}