﻿using LabFusion.Data;
using LabFusion.Syncables;
using LabFusion.Patching;
using LabFusion.Extensions;

namespace LabFusion.Network
{
    public class MagazineInsertData : IFusionSerializable
    {
        public const int Size = sizeof(byte) + sizeof(ushort) * 2;

        public byte smallId;
        public ushort magazineId;
        public ushort gunId;

        public void Serialize(FusionWriter writer)
        {
            writer.Write(smallId);
            writer.Write(magazineId);
            writer.Write(gunId);
        }

        public void Deserialize(FusionReader reader)
        {
            smallId = reader.ReadByte();
            magazineId = reader.ReadUInt16();
            gunId = reader.ReadUInt16();
        }

        public static MagazineInsertData Create(byte smallId, ushort magazineId, ushort gunId)
        {
            return new MagazineInsertData()
            {
                smallId = smallId,
                magazineId = magazineId,
                gunId = gunId,
            };
        }
    }

    [Net.DelayWhileTargetLoading]
    public class MagazineInsertMessage : FusionMessageHandler
    {
        public override byte? Tag => NativeMessageTag.MagazineInsert;

        public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
        {
            using FusionReader reader = FusionReader.Create(bytes);
            var data = reader.ReadFusionSerializable<MagazineInsertData>();
            // Send message to other clients if server
            if (NetworkInfo.IsServer && isServerHandled)
            {
                using var message = FusionMessage.Create(Tag.Value, bytes);
                MessageSender.BroadcastMessageExcept(data.smallId, NetworkChannel.Reliable, message, false);
            }
            else
            {
                if (SyncManager.TryGetSyncable<PropSyncable>(data.magazineId, out var mag) && mag.TryGetExtender<MagazineExtender>(out var magExtender) && SyncManager.TryGetSyncable<PropSyncable>(data.gunId, out var gun) && gun.TryGetExtender<AmmoSocketExtender>(out var socketExtender))
                {
                    // Insert mag into gun
                    if (socketExtender.Component._magazinePlug)
                    {
                        var otherPlug = socketExtender.Component._magazinePlug;

                        if (otherPlug != magExtender.Component.magazinePlug)
                        {
                            AmmoSocketPatches.IgnorePatch = true;
                            if (otherPlug)
                            {
                                otherPlug.ForceEject();

                                if (MagazineExtender.Cache.TryGet(otherPlug.magazine, out var otherMag))
                                {
                                    otherMag.SetRigidbodiesDirty();
                                }
                            }
                            AmmoSocketPatches.IgnorePatch = false;
                        }
                    }

                    magExtender.Component.magazinePlug.host.TryDetach();

                    AmmoSocketPatches.IgnorePatch = true;
                    magExtender.Component.magazinePlug.InsertPlug(socketExtender.Component);
                    AmmoSocketPatches.IgnorePatch = false;
                }
            }
        }
    }
}