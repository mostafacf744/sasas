using System;
using System.Collections.Generic;
using System.Linq;
using GuardShield;
using System.Text;
namespace GameServer.Game.MsgServer
{
    public struct MsgMachineInfo
    {
        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgMachineInfo)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet packet)
        {
            packet.Seek(0);
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgMachineInfo(bytes);
            Console.WriteLine(client.Player.Name + "'s [PC]: '" + msg.MachineName + "' [MAC]: '" + msg.MacAddress + "' [HD Serial]: " + msg.HDSerial.ToString());
        }
    }
}
