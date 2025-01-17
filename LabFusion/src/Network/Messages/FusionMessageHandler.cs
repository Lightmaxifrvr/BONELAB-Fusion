﻿using System.Reflection;

using LabFusion.Utilities;

namespace LabFusion.Network;

public abstract class FusionMessageHandler : MessageHandler
{
    public virtual byte? Tag { get; } = null;

    // Handlers are created up front, they're not static
    public static void RegisterHandlersFromAssembly(Assembly targetAssembly)
    {
        if (targetAssembly == null) throw new NullReferenceException("Can't register from a null assembly!");

#if DEBUG
        FusionLogger.Log($"Populating MessageHandler list from {targetAssembly.GetName().Name}!");
#endif

        AssemblyUtilities.LoadAllValid<FusionMessageHandler>(targetAssembly, RegisterHandler);
    }

    public static void RegisterHandler<T>() where T : FusionMessageHandler => RegisterHandler(typeof(T));

    protected static void RegisterHandler(Type type)
    {
        FusionMessageHandler handler = Activator.CreateInstance(type) as FusionMessageHandler;

        if (handler.Tag == null)
        {
            FusionLogger.Warn($"Didn't register {type.Name} because its message index was null!");
        }
        else
        {
            handler.NetAttributes = type.GetCustomAttributes<Net.NetAttribute>().ToArray();

            byte index = handler.Tag.Value;

            if (Handlers[index] != null) throw new Exception($"{type.Name} has the same index as {Handlers[index].GetType().Name}, we can't replace handlers!");

#if DEBUG
            FusionLogger.Log($"Registered {type.Name}");
#endif

            Handlers[index] = handler;
        }
    }

    public static unsafe void ReadMessage(byte* buffer, int size, bool isServerHandled = false)
    {
        NetworkInfo.BytesDown += size;

        byte tag = 0;

        try
        {
            tag = buffer[0];
            byte[] message = ByteRetriever.Rent(size - 1);

            for (var i = 0; i < message.Length; i++)
                message[i] = buffer[i + 1];

            if (Handlers[tag] != null)
            {
                Handlers[tag].Internal_HandleMessage(message, isServerHandled);
            }
#if DEBUG
            else
            {
                FusionLogger.Warn($"Received message with invalid tag {tag}!");
            }
#endif
        }
        catch (Exception e)
        {
            FusionLogger.Error($"Failed handling network message of tag {tag} with reason: {e.Message}\nTrace:{e.StackTrace}");
        }
    }

    public static void ReadMessage(byte[] bytes, bool isServerHandled = false)
    {
        NetworkInfo.BytesDown += bytes.Length;

        try
        {
            byte tag = bytes[0];
            byte[] buffer = ByteRetriever.Rent(bytes.Length - 1);

            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = bytes[i + 1];

            Handlers[tag].Internal_HandleMessage(buffer, isServerHandled);
        }
        catch (Exception e)
        {
            FusionLogger.Error($"Failed handling network message with reason: {e.Message}\nTrace:{e.StackTrace}");
        }
    }

    public static readonly FusionMessageHandler[] Handlers = new FusionMessageHandler[byte.MaxValue];
}