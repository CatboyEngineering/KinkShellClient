using System;
using System.Runtime.InteropServices;
using System.Text;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace CatboyEngineering.KinkShellClient.Utilities
{
    public class Chat
    {
        private static class Signatures
        {
            internal const string SendChat = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8B F2 48 8B F9 45 84 C9";
        }

        private delegate void ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unused, byte a4);
        private ProcessChatBoxDelegate ProcessChatBox { get; }

        public Chat()
        {
            if (Plugin.SigScanner.TryScanText(Signatures.SendChat, out var processChatBoxPtr))
            {
                ProcessChatBox = Marshal.GetDelegateForFunctionPointer<ProcessChatBoxDelegate>(processChatBoxPtr);
            }
        }

        public unsafe void sendMessage(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var uiModule = (IntPtr)Framework.Instance()->GetUIModule();
            using var payload = new ChatPayload(bytes);
            var mem1 = Marshal.AllocHGlobal(400);

            Marshal.StructureToPtr(payload, mem1, false);
            ProcessChatBox(uiModule, mem1, IntPtr.Zero, 0);
            Marshal.FreeHGlobal(mem1);
        }

        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ChatPayload : IDisposable
        {
            [FieldOffset(0)]
            private readonly IntPtr textPtr;
            [FieldOffset(16)]
            private readonly ulong textLen;
            [FieldOffset(8)]
            private readonly ulong unk1;
            [FieldOffset(24)]
            private readonly ulong unk2;
            internal ChatPayload(byte[] stringBytes)
            {
                textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
                Marshal.Copy(stringBytes, 0, textPtr, stringBytes.Length);
                Marshal.WriteByte(textPtr + stringBytes.Length, 0);
                textLen = (ulong)(stringBytes.Length + 1);
                unk1 = 64;
                unk2 = 0;
            }
            public void Dispose()
            {
                Marshal.FreeHGlobal(textPtr);
            }
        }
    }
}
