using System;
using System.Collections.Generic;
using System.Linq;
using GuardShield;
using System.Text;
namespace GameServer.Game.MsgServer
{
    public struct MsgPing
    {
        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgPing)]
        public unsafe static void Process(Client.GameClient client, ServerSockets.Packet packet)
        {
            packet.Seek(0);
            client.LoaderTime = DateTime.Now;
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgPing(bytes);
            if ((client.StampThreadMemory != 0 && client.StampThreadTimer != 0) && client.StampThreadMemory == msg.ThreadMemory  || client.StampThreadTimer == msg.ThreadTimer)
            {
                Console.WriteLine("Account: " + client.Player.Name + " has been banned due to loop scan has been stopped.");
                client.Socket.Disconnect();
            }     
            else if (msg.EditMemory)
            {
                GuardShield.MsgGuardShield.ReportLogg(client.Player.Name, msg.strParam);
                Database.SystemBannedAccount.AddBan(client, 2, "ModifdeMemory");
                Console.WriteLine("Account: " + client.Player.Name + " has been banned due to using programs that affected game memory ["+ msg.strParam + "].");
                client.Socket.Disconnect();
            }
            else if (!client.ActiveClient && !client.OnInterServer)
            {
                client.Socket.Disconnect();
            }
            client.StampThreadMemory = msg.ThreadMemory;
            client.StampThreadTimer = msg.ThreadTimer;
        }
    }
}
