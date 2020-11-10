﻿using System.Net;
using BeatTogether.MasterServer.Messaging.Abstractions.Messages;
using BeatTogether.MasterServer.Messaging.Extensions;
using Krypton.Buffers;

namespace BeatTogether.MasterServer.Messaging.Implementations.Messages.User
{
    public class PrepareForConnectionRequest : BaseMessage, IReliableRequest, IEncryptedMessage
    {
        public uint SequenceId { get; set; }
        public uint RequestId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public byte[] Random { get; set; }
        public byte[] PublicKey { get; set; }
        public bool IsConnectionOwner { get; set; }
        public bool IsDedicatedServer { get; set; }

        public override void WriteTo(ref GrowingSpanBuffer buffer)
        {
            buffer.WriteString(UserId);
            buffer.WriteString(UserName);
            buffer.WriteIPEndPoint(RemoteEndPoint);
            buffer.WriteBytes(Random);
            buffer.WriteVarBytes(PublicKey);
            buffer.WriteUInt8((byte)((IsConnectionOwner ? 1 : 0) | (IsDedicatedServer ? 2 : 0)));
        }

        public override void ReadFrom(ref SpanBufferReader bufferReader)
        {
            UserId = bufferReader.ReadString();
            UserName = bufferReader.ReadString();
            RemoteEndPoint = bufferReader.ReadIPEndPoint();
            Random = bufferReader.ReadBytes(32).ToArray();
            PublicKey = bufferReader.ReadVarBytes().ToArray();
            var flags = bufferReader.ReadByte();
            IsConnectionOwner = (flags & 1) > 0;
            IsDedicatedServer = (flags & 2) > 0;
        }
    }
}
