using System;
using Unity.Netcode;
using Unity.Collections;

namespace Network
{
	public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
	{
		public ulong ClientId;
		public int ColorId;
		public FixedString64Bytes PlayerName;
		public FixedString64Bytes PlayerId;

		public PlayerData(ulong clientId, int colorId, FixedString64Bytes playerName)
		{
			ClientId = clientId;
			ColorId = colorId;
			PlayerName = playerName;
			PlayerId = "";
		}

		public bool Equals(PlayerData other)
		{
			return ClientId == other.ClientId && ColorId == other.ColorId && PlayerName == other.PlayerName && PlayerId == other.PlayerId;
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
			serializer.SerializeValue(ref PlayerName);
			serializer.SerializeValue(ref PlayerId);
		}
	}
}