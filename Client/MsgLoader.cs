using System;
using GuardShield;
using System.Threading.Tasks;
using GameServer.Game.MsgServer;
namespace GameServer.MsgLoader
{
    public static class MsgLoader
    {
        [PacketAttribute((ushort)MsgGuardShield.PacketIDs.MsgLoader)]
        public static async void Process(Client.GameClient client, ServerSockets.Packet packet)
        {
            packet.Seek(0);
            byte[] bytes = packet.ReadBytes(packet.Size);
            var msg = new MsgGuardShield.MsgLoader(bytes);
            switch (msg.Type)
            {
                case MsgGuardShield.MsgLoader.LoaderMessage.ClientFilesScaning:
                    {
                        client.ActiveClient = true;
                        client.LoaderTime = DateTime.Now;
                        if (msg.GuardVersion != 107)
                        {
                            msg.strParam = "ERROR Changed Loader Version\n\rplease download game patches or last patch";
                            Console.WriteLine("Account: {0} has get dissconnect due to use old Loader Version[{1}].", client.Player.Name, msg.GuardVersion);
                            client.TerminateLoader = true;
                        }
                        else if (!MsgGuardShield.Validated(msg.Conquer, msg.MagicType, msg.MagicEffect, msg.C3_WDB, msg.DLL_Hash, msg.StrRes, msg.Data_Servers, out msg.strParam))
                        {
                            msg.strParam = "Change FILES[" + msg.strParam + "]\n\rplease download game patches or last patch";
                            Database.SystemBannedAccount.AddBan(client, 2, ("Interruption." + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "")));
                            Console.WriteLine("Account: {0} has get banned due to changing client files.", client.Player.Name);
                            client.TerminateLoader = true;
                        }
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.MemoryChanged:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.TerminateLoader = true;
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.SpeedHack:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Account: " + client.Player.Name + " has been banned due to {0}.", msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.Aimbot:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.SuspendThreads:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.TerminateLoader = true;
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.AutoHotkey:
                case MsgGuardShield.MsgLoader.LoaderMessage.AutoClick:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.ScriptAutoClick:
                case MsgGuardShield.MsgLoader.LoaderMessage.ScriptAutoHotkey:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.AutoHunting:
                    {
                        if (client.Player.OnAutoHunt)
                            return;
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Console.WriteLine("Player: {0} has been Disconnect due to use {1}.", client.Player.Name, msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                case MsgGuardShield.MsgLoader.LoaderMessage.AbnormalOperation:
                    {
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        System.Console.WriteLine("Player: {0} has been Disconnect due to {1} Changed.", client.Player.Name, msg.strParam);
                        client.Socket.Disconnect();
                        break;
                    }
                default:
                    {
                        if (!Enum.IsDefined(typeof(MsgGuardShield.MsgLoader.LoaderMessage), msg.Type))
                        {
                            msg.strParam = string.Format("Unknown MsgLoader type: {0}, From:{1}", msg.Type.ToString(), client.Player.Name);
                            Console.WriteLine(msg.strParam);
                            client.TerminateLoader = true;
                            break;
                        }
                        msg.strParam = msg.Type.ToString() + (!String.IsNullOrEmpty(msg.strParam) ? (" (" + msg.strParam + ")") : "");
                        Database.SystemBannedAccount.AddBan(client, 2, msg.strParam);
                        Console.WriteLine("Player: {0} has been banned due to use {1}.", client.Player.Name, msg.strParam);
                        client.TerminateLoader = true;
                        break;
                    }
            }
            if (!string.IsNullOrEmpty(msg.strParam))
            {
                GuardShield.MsgGuardShield.ReportLogg(client.Player.Name, msg.strParam);
                if (client.TerminateLoader)
                {
                    if (msg.Type != MsgGuardShield.MsgLoader.LoaderMessage.ClientFilesScaning)
                        client.Send(MsgGuardShield.TerminateLoader("ERROR", "Suspected of using hack"));
                    else
                        client.Send(MsgGuardShield.TerminateLoader("ERROR", msg.strParam));
                    await Task.Delay(2500);
                    if (client.Socket.Alive || client.Socket.Connection.Connected)
                        client.Socket.Disconnect();
                }
            }
        }
        public static bool UpdateSpeedFlags(Client.GameClient client)
        {
            if (client.Player.ContainFlag(MsgUpdate.Flags.Cyclone)
               || client.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone)
               || client.Player.ContainFlag(MsgUpdate.Flags.FatalStrike)
               || client.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)
               || client.Player.ContainFlag(MsgUpdate.Flags.Superman)
               || client.Player.ContainFlag(MsgUpdate.Flags.Ride)
               || client.Player.ContainFlag(MsgUpdate.Flags.Fly)
               || client.Player.ContainFlag(MsgUpdate.Flags.CannonBarrage)
               || client.Player.ContainFlag(MsgUpdate.Flags.Oblivion)
               || client.Player.ContainFlag(MsgUpdate.Flags.Omnipotence)
               || client.Player.ContainFlag(MsgUpdate.Flags.BladeFlurry)
               || client.Player.ContainFlag(MsgUpdate.Flags.ThunderRampage)
               || client.Player.ContainFlag(MsgUpdate.Flags.Accelerated)
               || client.Player.ContainFlag(MsgUpdate.Flags.CelestialDance)
               || client.Player.ContainFlag(MsgUpdate.Flags.SageMode)//273
               || client.Player.ContainFlag(MsgUpdate.Flags.Immersion)//303
               || client.Player.OnTransform)
            {
                return true;
            }
            return false;
        }
    }
}
