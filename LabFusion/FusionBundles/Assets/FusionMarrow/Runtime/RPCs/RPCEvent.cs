using System.Collections;
using System.Collections.Generic;

#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;

using Il2CppUltEvents;

using LabFusion.Data;
using LabFusion.Network;

using MelonLoader;
#else
using UltEvents;
#endif

using UnityEngine;

namespace LabFusion.Marrow.Integration
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [HelpURL("https://github.com/Lakatrazz/BONELAB-Fusion/wiki/Remote-Procedure-Calls#rpc-event")]
#endif
    public class RPCEvent : MonoBehaviour
    {
        public enum RPCTarget
        {
            Server,
            Clients
        }

        public enum RPCChannel
        {
            Reliable = 0,
            Unreliable = 1,
        }

#if MELONLOADER
        public RPCEvent(IntPtr intPtr) : base(intPtr) { }

        public static readonly ComponentHashTable<RPCEvent> HashTable = new();

        public Il2CppValueField<int> target;

        public Il2CppValueField<int> channel;

        public Il2CppValueField<bool> requiresOwnership;

        public Il2CppReferenceField<UltEvent> onEventReceived;

        private UltEvent _onEventReceivedCached;

        private void Awake()
        {
            _onEventReceivedCached = onEventReceived.Get();

            var hash = GameObjectHasher.GetHierarchyHash(gameObject);

            HashTable.AddComponent(hash, this);
        }

        private void OnDestroy()
        {
            HashTable.RemoveComponent(this);
        }

        public void Receive()
        {
            _onEventReceivedCached?.Invoke();
        }

        public bool Invoke()
        {
            return RPCEventSender.Invoke(this);
        }
#else
        public RPCTarget target = RPCTarget.Server;

        public RPCChannel channel = RPCChannel.Reliable;

        public bool requiresOwnership = false;

        public UltEvent onEventReceived;

        public bool Invoke()
        {
            return false;
        }
#endif
    }
}