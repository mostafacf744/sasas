using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace GuardShield
{
    public static class MsgGuardShield
    {
        [DllImport("GuardShield.dll")]
        public static extern IntPtr IntlizingLoader(bool load);
        [DllImport("GuardShield.dll")]
        public static extern IntPtr HandleBuffer(byte[] buffer, int Length);
        private static string[] ConquerHashes = new string[2];
        private static string[] DLLHash = new string[2];
        private static string MagicHash/*, C3Hash*/, MagicEffectHash, StrResHash, DataServersHash;
        private static bool HdKeyVaild = true;
        public static bool CheakConquer = false;
        public static void Load(bool Read = true)
        {
            IntlizingLoader(true);
            if (Read)
            {
                ConquerHashes[0] = CalculateMD5(@"Files\Env_DX8\Conquer.exe");
                ConquerHashes[1] = CalculateMD5(@"Files\Env_DX9\Conquer.exe");
                MagicHash = CalculateMD5(@"Files\ini\magictype.dat");
                MagicEffectHash = CalculateMD5(@"Files\ini\MagicEffect.ini");
                StrResHash = CalculateMD5(@"Files\ini\StrRes.ini");
                DataServersHash = CalculateMD5(@"Files\Guard.dat");
                // C3Hash = CalculateMD5(@"Files\ini\c3.wdb");
                DLLHash[0] = CalculateMD5(@"Files\Env_DX8\GuardShield.dll");
                DLLHash[1] = CalculateMD5(@"Files\Env_DX9\GuardShield.dll");
            }
        }
        private static string CalculateMD5(string filename)
        {
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = System.IO.File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        string H = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        return H;
                    }
                }
            }
            catch
            {
                HdKeyVaild = false;
                Console.WriteLine(string.Format("{0} Not Find Hash", filename));
            }
            return "";
        }
        public static bool Validated(string conquer, string magicType, string magicEffect, string c3_WDB, string dLL_Hash, string StrRes, string Data_Servers, out string filechanged)
        {
            filechanged = "";
            if (CheakConquer)
            {
                if (conquer != ConquerHashes[0] && conquer != ConquerHashes[1])
                    filechanged += " Conquer.exe";
            }
            if (magicType != MagicHash)
                filechanged += " magictype.dat";
            if (magicEffect != MagicEffectHash)
                filechanged += " MagicEffect.ini";
            //if (c3_WDB != C3Hash)
            //    filechanged += " c3.wdb";
            if (dLL_Hash != DLLHash[0] && dLL_Hash != DLLHash[1])
                filechanged += " GuardShield.dll";
            if (StrRes != StrResHash)
                filechanged += " StrRes.ini";
            if (Data_Servers != DataServersHash)
                filechanged += " Guard.dat";
            return filechanged == "" ? true : false;
        }
        public static void ReportLogg(String Name, string reason)
        {
            DateTime timer = DateTime.Now;
            string logs = string.Format("CHEAT [Player] {0} -- REASON: {1}", Name, !String.IsNullOrEmpty(reason) ? reason : "Abnormal operation");
            OnDequeue(logs, timer.Millisecond);
        }
        private static void OnDequeue(object obj, int time)
        {
            try
            {
                if (obj is string)
                {
                    string text = obj as string;
                    string identifier = text.Substring(0, text.IndexOf("]") + 1);
                    string UnhandledExceptionsPath = Application.StartupPath + "\\LoaderLogg\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    UnhandledExceptionsPath += "[Logs]\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    UnhandledExceptionsPath += identifier + "\\";
                    if (!Directory.Exists(UnhandledExceptionsPath))
                        Directory.CreateDirectory(UnhandledExceptionsPath);
                    string fileName = UnhandledExceptionsPath + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + ".txt";
                    if (!File.Exists(fileName))
                        File.WriteAllLines(fileName, new string[0]);
                    using (var SW = File.AppendText(fileName))
                    {
                        SW.WriteLine(text.Replace(identifier, DateTime.Now.ToString("[hh:mm:ss tt]:")));
                        SW.Close();
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
        public static void HandleBuffer(byte[] buffer)
        {
            if (HdKeyVaild)
            {
                HandleBuffer(buffer, buffer.Length);
            }
        }
        public static string RemoveIllegalCharacters(this string str, bool path, bool file)
        {
            string myString = str;
            if (file)
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    myString = myString.Replace(c, '_');
            if (path)
                foreach (char c in System.IO.Path.GetInvalidPathChars())
                    myString = myString.Replace(c, '_');
            return myString;
        }
        public static byte[] RequestOpenedProcesses()
        {
            byte[] buffer = new byte[4 + 30 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgRequestOpenedProcesses;
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public static byte[] RequestMachineInfo()
        {
            byte[] buffer = new byte[4 + 30 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgRequestMachineInfo;
                    }
                    else return new byte[0];
                }
            }
            return buffer;
        }
        public static byte[] PingStatuesLoader(string PingStatuesMsg = "")
        {
            byte[] buffer = new byte[6 + PingStatuesMsg.Length + 2 + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.PingStatuesLoader;
                        *((byte*)(Buffer + 5)) = (byte)PingStatuesMsg.Length;
                        ushort i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(PingStatuesMsg).Length)
                        {
                            *((byte*)(Buffer + 6 + i)) = (byte)System.Text.Encoding.Default.GetBytes(PingStatuesMsg)[i];
                            i++;
                        }
                    }
                    else return new byte[0];
                }
            }
            HandleBuffer(buffer);
            return buffer;
        }
        public static byte[] TerminateLoader(string MessageCaption = "", string MessageText = "")
        {
            byte[] buffer = new byte[6 + MessageCaption.Length + MessageText.Length + 8];
            unsafe
            {
                fixed (byte* Buffer = buffer)
                {
                    if (HdKeyVaild)
                    {
                        *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                        *((ushort*)(Buffer + 2)) = (ushort)PacketIDs.MsgTerminateLoader;
                        *((byte*)(Buffer + 4)) = (byte)MessageCaption.Length;
                        ushort i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(MessageCaption).Length)
                        {
                            *((byte*)(Buffer + 5 + i)) = (byte)System.Text.Encoding.Default.GetBytes(MessageCaption)[i];
                            i++;
                        }
                        *((byte*)(Buffer + 5 + System.Text.Encoding.Default.GetBytes(MessageCaption).Length)) = (byte)MessageText.Length;
                        i = 0;
                        while (i < System.Text.Encoding.Default.GetBytes(MessageText).Length)
                        {
                            *((byte*)(Buffer + 6 + i + System.Text.Encoding.Default.GetBytes(MessageCaption).Length)) = (byte)System.Text.Encoding.Default.GetBytes(MessageText)[i];
                            i++;
                        }
                    }
                    else return new byte[0];
                }
            }
            HandleBuffer(buffer);
            return buffer;
        }
        public static byte[] ProfileInfo(uint ID, ulong dwParam1, ushort Action, uint timestamp, uint dwParam2,
           uint dwParam3, uint dwparam4, ushort VipLevel, bool sendping, bool updateflags, bool AutoHunt)
        {
            byte[] buffer = new byte[110 + 8];
            if (Action == 0x1B)
            {
                unsafe
                {
                    fixed (byte* Buffer = buffer)
                    {
                        if (HdKeyVaild)
                        {
                            *((ushort*)(Buffer + 0)) = (ushort)(buffer.Length - 8);
                            *((ushort*)(Buffer + 2)) = (ushort)2137;
                            *((uint*)(Buffer + 4)) = (uint)ID;
                            *((ulong*)(Buffer + 8)) = (ulong)dwParam1;
                            *((ushort*)(Buffer + 16)) = (ushort)Action;
                            *((uint*)(Buffer + 18)) = (uint)timestamp;
                            *((uint*)(Buffer + 22)) = (uint)dwParam2;
                            *((uint*)(Buffer + 26)) = (uint)dwParam3;
                            *((uint*)(Buffer + 30)) = (uint)dwparam4;
                            *((ushort*)(Buffer + 90)) = (ushort)VipLevel;
                            *((ushort*)(Buffer + 90 + 2)) = sendping == true ? (ushort)999 : (ushort)0;
                            *((ushort*)(Buffer + 90 + 2 + 2)) = updateflags == true ? (ushort)999 : (ushort)0;
                            *((ushort*)(Buffer + 90 + 2 + 2 + 2)) = AutoHunt == true ? (ushort)999 : (ushort)0;
                        }
                        else return new byte[0];
                    }
                }
            }
            return buffer;
        }
        public enum PacketIDs : ushort
        {
            MsgAuthentication = 0xDEAD,
            MsgRequestOpenedProcesses,
            MsgOpenedProcesses,
            MsgTerminateLoader,
            MsgLoader,
            MsgRequestMachineInfo,
            MsgMachineInfo,
            MsgLoginGame,
            PingStatuesLoader,
            MsgRequestScreenPacket,
            MsgScreenPacket,
            MsgPing
        }
        public class Authentication
        {
            public string Username, Password, SecurtyCode, HWID, Server, MacAddress;
            public uint HDSerial;
            public Authentication(byte[] Buffer)
            {
                const ushort length = 472;
                if (Buffer != null && Buffer.Length >= length)
                {
                    if (BitConverter.ToUInt16(Buffer, 0) == length && BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgAuthentication)
                    {
                        if (HdKeyVaild)
                        {
                            HandleBuffer(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            Username = System.Text.Encoding.Default.GetString(rdr.ReadBytes(128)).Replace("\0", "");
                            Password = System.Text.Encoding.Default.GetString(rdr.ReadBytes(64)).Replace("\0", "");//132
                            Server = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");//196
                            MacAddress = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");//228
                            SecurtyCode = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");//244
                            HWID = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");//260
                            HDSerial = rdr.ReadUInt32();//292
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgPing
        {
            private uint OrignalHash, FirstHash, SecondHash;

            public uint ThreadMemory, ThreadTimer;

            public byte Direct;
            public bool EditMemory = false;
            public string strParam;
            public MsgPing(byte[] Buffer)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 + 2 && BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgPing)
                {
                    if (HdKeyVaild)
                    {
                        HandleBuffer(Buffer);

                        BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                        rdr.BaseStream.Seek(4, SeekOrigin.Current);
                        OrignalHash = rdr.ReadUInt32();//4
                        FirstHash = rdr.ReadUInt32();//8
                        SecondHash = rdr.ReadUInt32();//12
                        ThreadMemory = rdr.ReadUInt32();//16
                        ThreadTimer = rdr.ReadUInt32();//20
                        Direct = rdr.ReadByte();//24
                        rdr.Close();
                        if (FirstHash != SecondHash)
                        {
                            strParam = string.Format("ModifdeMemory[{0}]", Direct);
                            EditMemory = true;
                            return;
                        }
                        if (!EditMemory)
                        {
                            if (Direct != 8 && Direct != 9)
                            {
                                strParam = "UnknowDirect";
                                EditMemory = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        public class MsgLoader
        {
            [Flags]
            public enum LoaderMessage : byte
            {
                MemoryChanged = 1,
                AutoHunting,
                AutoClick,
                ScriptAutoClick,
                AutoHotkey,
                ScriptAutoHotkey,
                ClientFilesScaning,
                SpeedHack,
                FunctionChanged,
                Injectdll,
                InjectCode,
                Aimbot,
                ZoomHack,
                DebuggerPresent,
                SuspendThreads,
                AbnormalOperation
            }
            public LoaderMessage Type;
            public int GuardVersion;
            public string strParam, Conquer, MagicType, MagicEffect, C3_WDB, DLL_Hash, StrRes, Data_Servers;
            public MsgLoader(byte[] Buffer)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 + 2)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgLoader)
                    {
                        if (HdKeyVaild)
                        {
                            HandleBuffer(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            Type = (LoaderMessage)rdr.ReadByte();
                            if (Type == LoaderMessage.ClientFilesScaning)
                            {
                                Conquer = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                DLL_Hash = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                MagicType = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                MagicEffect = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                StrRes = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                C3_WDB = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                Data_Servers = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                                GuardVersion = rdr.ReadByte();
                            }
                            else
                            {
                                strParam = System.Text.Encoding.Default.GetString(rdr.ReadBytes(33)).Replace("\0", "");
                            }
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgMachineInfo
        {
            public string MachineName, MacAddress;
            public uint HDSerial;
            public MsgMachineInfo(byte[] Buffer)
            {
                if (Buffer != null)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgMachineInfo)
                    {
                        if (HdKeyVaild)
                        {
                            HandleBuffer(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            MacAddress = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");
                            MachineName = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");
                            HDSerial = rdr.ReadUInt32();
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgLoginGame
        {
            public string MachineName, MacAddress, SecurtyCode, HWID;
            public uint Key, AccountHash, HDSerial, OwnerAttackEncrypt;
            public MsgLoginGame(byte[] Buffer)
            {
                if (Buffer != null)
                {
                    if (BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgLoginGame)
                    {
                        if (HdKeyVaild)
                        {
                            HandleBuffer(Buffer);
                            BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                            rdr.BaseStream.Seek(4, SeekOrigin.Current);
                            Key = rdr.ReadUInt32();//4
                            AccountHash = rdr.ReadUInt32();//8
                            MachineName = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");//12
                            MacAddress = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");//44
                            SecurtyCode = System.Text.Encoding.Default.GetString(rdr.ReadBytes(16)).Replace("\0", "");//60
                            HWID = System.Text.Encoding.Default.GetString(rdr.ReadBytes(32)).Replace("\0", "");//76
                            HDSerial = rdr.ReadUInt32();//108
                            OwnerAttackEncrypt = rdr.ReadUInt32();//112
                            rdr.Close();
                        }
                    }
                }
            }
        }
        public class MsgOpenedProcesses
        {
            public enum Type : byte
            {
                Start,
                Insert,
                Finish
            }
            public Type ActionType;
            public List<string> Processes;
            public MsgOpenedProcesses(byte[] Buffer)
            {
                if (Buffer != null && Buffer.Length >= 4 + 1 && BitConverter.ToUInt16(Buffer, 2) == (ushort)PacketIDs.MsgOpenedProcesses)
                {
                    if (HdKeyVaild)
                    {
                        HandleBuffer(Buffer);
                        BinaryReader rdr = new BinaryReader(new MemoryStream(Buffer));
                        rdr.BaseStream.Seek(4, SeekOrigin.Current);
                        ActionType = (Type)rdr.ReadByte();
                        if (ActionType == Type.Insert)
                        {
                            int count = rdr.ReadUInt16();
                            Processes = new List<string>(count);
                            for (int i = 0; i < count; i++)
                                Processes.Add(System.Text.Encoding.Default.GetString(rdr.ReadBytes(rdr.ReadUInt16())).Replace("\0", ""));
                        }
                        rdr.Close();
                    }
                }
            }
        }
    }
}
