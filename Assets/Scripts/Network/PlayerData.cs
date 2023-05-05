using System;
using Unity.Netcode;

namespace Network
{
	public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
	{
		public ulong ClientId;
		public int ColorId;

		public PlayerData(ulong clientId, int colorId = 0)
		{
			ClientId = clientId;
			ColorId = colorId;
		}

		public bool Equals(PlayerData other)
		{
			return ClientId == other.ClientId && ColorId == other.ColorId;
		}

		public override bool Equals(object obj)
		{
			return obj is PlayerData other && Equals(other);
		}

		public override int GetHashCode()
		{
			return ClientId.GetHashCode();
		}

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref ClientId);
			serializer.SerializeValue(ref ColorId);
		}
	}
}