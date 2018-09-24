using System;
using UnityEngine;

public static class SeriallizeData {
    #region Vector2
    public static byte[] Vector2ToBytes(Vector3 position, Vector3 direction) {
        byte[] data = new byte[4 * sizeof(float)];
        Buffer.BlockCopy(BitConverter.GetBytes(position.x), 0, data, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(position.y), 0, data, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(direction.x), 0, data, 3 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(direction.y), 0, data, 4 * sizeof(float), sizeof(float));
        return data;
    }

    public static Vector2 ByteToVector2(byte[] data) {
        Vector2 position = Vector2.zero;
        position.x = BitConverter.ToSingle(data, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(data, 1 * sizeof(float));

        return position;
    }
    #endregion
}
