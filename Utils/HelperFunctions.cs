using System.Net.Sockets;
using System.Text;
using ABXClient.Models;

namespace ABXClient.Utils;

public static class HelperFunctions
{
    public static TickerPacket? ReadPacket(NetworkStream stream)
    {
        try
        {
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);
            byte[] symbolBytes = reader.ReadBytes(4);
            if (symbolBytes.Length < 4) return null; //  at the end of stream or the requested payload does not exists 
            string symbol = Encoding.ASCII.GetString(symbolBytes);

            char side = (char)reader.ReadByte();

            int quantity = ReadInt32BigEndian(reader);
            int price = ReadInt32BigEndian(reader);
            int sequence = ReadInt32BigEndian(reader);

            Console.WriteLine($"Received packet: Symbol={symbol}, Side={side}, Quantity={quantity}, Price={price}, Sequence={sequence}");

            return new TickerPacket
            {
                Symbol = symbol,
                Side = side.ToString(),
                Quantity = quantity,
                Price = price,
                Sequence = sequence
            };
        }
        catch
        {
            return null;
        }
    }

    public static int ReadInt32BigEndian(BinaryReader reader)
    {
        //Endianness: Big Endian as specified
        byte[] bytes = reader.ReadBytes(4);
        if (bytes.Length < 4) throw new EndOfStreamException();
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
}