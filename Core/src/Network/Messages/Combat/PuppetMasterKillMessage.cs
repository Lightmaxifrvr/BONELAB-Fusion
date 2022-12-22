﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LabFusion.Data;
using LabFusion.Representation;
using LabFusion.Utilities;
using LabFusion.Grabbables;
using LabFusion.Syncables;
using LabFusion.Patching;

using SLZ;
using SLZ.Interaction;
using SLZ.Props.Weapons;

namespace LabFusion.Network
{
    public class PuppetMasterKillData : IFusionSerializable, IDisposable
    {
        public byte smallId;
        public ushort puppetId;

        public void Serialize(FusionWriter writer)
        {
            writer.Write(smallId);
            writer.Write(puppetId);
        }

        public void Deserialize(FusionReader reader)
        {
            smallId = reader.ReadByte();
            puppetId = reader.ReadUInt16();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static PuppetMasterKillData Create(byte smallId, ushort puppetId)
        {
            return new PuppetMasterKillData()
            {
                smallId = smallId,
                puppetId = puppetId,
            };
        }
    }

    [Net.DelayWhileLoading]
    public class PuppetMasterKillMessage : FusionMessageHandler
    {
        public override byte? Tag => NativeMessageTag.PuppetMasterKill;

        public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
        {
            using (FusionReader reader = FusionReader.Create(bytes))
            {
                using (var data = reader.ReadFusionSerializable<PuppetMasterKillData>())
                {
                    // Send message to other clients if server
                    if (NetworkInfo.IsServer && isServerHandled) {
                        using (var message = FusionMessage.Create(Tag.Value, bytes)) {
                            MessageSender.BroadcastMessageExcept(data.smallId, NetworkChannel.Reliable, message, false);
                        }
                    }
                    else
                    {
                        if (SyncManager.TryGetSyncable(data.puppetId, out var puppet) && puppet is PropSyncable puppetSyncable)
                        {
                            // Kill the puppet
                            if (puppetSyncable.PuppetMaster)
                            {
                                PuppetMasterPatches.IgnorePatches = true;
                                puppetSyncable.PuppetMaster.Kill();
                                PuppetMasterPatches.IgnorePatches = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
