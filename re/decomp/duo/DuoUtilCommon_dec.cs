using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Xml;
using CERTENROLLLib;
using Microsoft.Win32;

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCopyright("2024")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyProduct("DuoUtilCommon.Properties")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.1", FrameworkDisplayName = ".NET Framework 4.7.1")]
[assembly: Dotfuscator("464981:1:0:7.5.0+b3e290d373", 0)]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyTitle("DuoUtilCommon")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Duo Security")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyVersion("1.0.0.0")]
[ComVisible(false)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class DotfuscatorAttribute : Attribute
{
	private string a;

	private int c;

	public string A => a;

	public int C => c;

	public DotfuscatorAttribute(string a, int c)
	{
		this.a = a;
		this.c = c;
	}
}
namespace Duo;

public static class DuoUtil
{
	public class Logger : IDisposable, IDuoLogger
	{
		[DataContract]
		public enum TargetDirectoryType
		{
			[EnumMember]
			Temp,
			[EnumMember]
			CommonAppDataSubdirectory,
			[EnumMember]
			Specified
		}

		[DataContract]
		public enum CharacterEncoding
		{
			[EnumMember]
			Unicode,
			[EnumMember]
			Mbcs
		}

		[DataContract]
		public class Configuration : ReadOnlyBase, ICloneable
		{
			[DataMember(IsRequired = true)]
			private string a = "duo.log";

			[DataMember(IsRequired = true)]
			private TargetDirectoryType b;

			[DataMember(IsRequired = true)]
			private string c;

			[DataMember(IsRequired = true)]
			private string d;

			[DataMember]
			private CharacterEncoding e;

			[DataMember]
			private bool f;

			[DataMember]
			private bool g = true;

			[DataMember]
			private bool h;

			public TargetDirectoryType targetDirectoryType
			{
				get
				{
					short num = -6186;
					short num2 = num;
					num = -6186;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						break;
					case true:
						break;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					return b;
				}
				set
				{
					short num = 9965;
					short num2 = num;
					num = 9965;
					switch (num2 == num)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 1;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref b, value);
				}
			}

			public string targetDirectoryAppDataSubdirectory
			{
				get
				{
					short num = -13000;
					short num2 = num;
					num = -13000;
					switch (num2 == num)
					{
					default:
						num = 0;
						if (num != 0)
						{
						}
						num = 0;
						_ = num;
						num = 1;
						if (num != 0)
						{
						}
						return c;
					}
				}
				set
				{
					short num = 15998;
					short num2 = num;
					num = 15998;
					switch (num2 == num)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					SetOrThrowIfReadOnly(ref c, value);
				}
			}

			public string targetDirectorySpecified
			{
				get
				{
					short num = 7089;
					short num2 = num;
					num = 7089;
					switch (num2 == num)
					{
					default:
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						if (num != 0)
						{
						}
						num = 0;
						_ = num;
						return d;
					}
				}
				set
				{
					short num = 0;
					_ = num;
					num = -8963;
					short num2 = num;
					num = -8963;
					switch (num2 == num)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref d, value);
				}
			}

			public string targetFileNodeName
			{
				get
				{
					short num = 16808;
					short num2 = num;
					num = 16808;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						break;
					case true:
						break;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					return a;
				}
				set
				{
					short num = 24256;
					short num2 = num;
					num = 24256;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						break;
					case true:
						break;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref a, value);
				}
			}

			public CharacterEncoding encoding
			{
				get
				{
					short num = 25350;
					short num2 = num;
					num = 25350;
					switch (num2 == num)
					{
					default:
						num = 0;
						if (num != 0)
						{
						}
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						_ = num;
						return e;
					}
				}
				set
				{
					short num = 0;
					_ = num;
					num = 1;
					if (num != 0)
					{
					}
					num = -15546;
					short num2 = num;
					num = -15546;
					switch (num2 == num)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref e, value);
				}
			}

			public bool onOpenEnsureEndsInNewline
			{
				get
				{
					short num = 27003;
					short num2 = num;
					num = 27003;
					switch (num2 == num)
					{
					default:
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						_ = num;
						num = 0;
						if (num != 0)
						{
						}
						return f;
					}
				}
				set
				{
					short num = 0;
					_ = num;
					num = -22012;
					short num2 = num;
					num = -22012;
					switch (num2 == num)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref f, value);
				}
			}

			public bool logToFile
			{
				get
				{
					short num = -32213;
					short num2 = num;
					num = -32213;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						num = 0;
						if (num != 0)
						{
						}
						num = 1;
						if (num != 0)
						{
						}
						return g;
					}
				}
				set
				{
					short num = 0;
					_ = num;
					num = -345;
					short num2 = num;
					num = -345;
					switch (num2 == num)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref g, value);
				}
			}

			public bool logToStdout
			{
				get
				{
					short num = 1;
					if (num != 0)
					{
					}
					num = -12067;
					short num2 = num;
					num = -12067;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						num = 0;
						if (num != 0)
						{
						}
						return h;
					}
				}
				set
				{
					short num = -27203;
					short num2 = num;
					num = -27203;
					switch (num2 == num)
					{
					}
					num = 0;
					_ = num;
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					SetOrThrowIfReadOnly(ref h, value);
				}
			}

			public object Clone()
			{
				short num = -18270;
				short num2 = num;
				num = -18270;
				switch (num2 == num)
				{
				default:
					num = 0;
					_ = num;
					break;
				case true:
					break;
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return (Configuration)MemberwiseClone();
			}
		}

		[DataContract]
		[Flags]
		private enum a : uint
		{
			[EnumMember]
			a = 0u,
			[EnumMember]
			b = 1u,
			[EnumMember]
			c = 2u,
			[EnumMember]
			d = 4u
		}

		[Flags]
		[DataContract]
		private enum b : uint
		{
			[EnumMember]
			a = 0x80000000u,
			[EnumMember]
			b = 0x40000000u,
			[EnumMember]
			c = 0x20000000u,
			[EnumMember]
			d = 0x10000000u,
			[EnumMember]
			e = 0x8000000u,
			[EnumMember]
			f = 0x4000000u,
			[EnumMember]
			g = 0x2000000u,
			[EnumMember]
			h = 0x1000000u,
			[EnumMember]
			i = 0x200000u,
			[EnumMember]
			j = 0x100000u,
			[EnumMember]
			k = 0x80000u,
			[EnumMember]
			l = 1u,
			[EnumMember]
			m = 2u,
			[EnumMember]
			n = 4u,
			[EnumMember]
			o = 0x10u,
			[EnumMember]
			p = 0x20u,
			[EnumMember]
			q = 0x40u,
			[EnumMember]
			r = 0x80u,
			[EnumMember]
			s = 0x100u,
			[EnumMember]
			t = 0x200u,
			[EnumMember]
			u = 0x400u,
			[EnumMember]
			v = 0x800u,
			[EnumMember]
			w = 0x1000u,
			[EnumMember]
			x = 0x2000u,
			[EnumMember]
			y = 0x4000u,
			[EnumMember]
			z = 0x10000u
		}

		[Flags]
		[DataContract]
		private enum c : uint
		{
			[EnumMember]
			a = 1u,
			[EnumMember]
			b = 2u,
			[EnumMember]
			c = 3u,
			[EnumMember]
			d = 4u,
			[EnumMember]
			e = 5u
		}

		[DataContract]
		[Flags]
		private enum d : uint
		{
			[EnumMember]
			a = 4u,
			[EnumMember]
			b = 0x80000000u,
			[EnumMember]
			c = 0x40000000u,
			[EnumMember]
			d = 0x10000000u
		}

		[CompilerGenerated]
		private sealed class e
		{
			public Logger a;

			public string b;

			public object[] c;

			internal void d()
			{
				short num = -4335;
				short num2 = num;
				num = -4335;
				switch (num2 == num)
				{
				}
				num = 0;
				_ = num;
				num = 0;
				if (num != 0)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				a.WriteLine(string.Format(b, c));
			}
		}

		[CompilerGenerated]
		private LogLevel m_a;

		[CompilerGenerated]
		private string m_b;

		private static readonly IntPtr m_c;

		private bool m_d;

		private Configuration m_e;

		private string f;

		private IntPtr g;

		private object h = new object();

		private static Logger i;

		private static object j;

		private const string k = "** Logging started";

		public static Logger globalLogger
		{
			get
			{
				object obj = j;
				bool lockTaken = false;
				Logger result = default(Logger);
				short num2;
				try
				{
					int num = default(int);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num)
							{
							case 2:
							{
								num2 = -20601;
								short num3 = num2;
								num2 = -20601;
								switch (num3 == num2)
								{
								default:
									num2 = 0;
									if (num2 != 0)
									{
									}
									if (i == null)
									{
										num2 = 0;
										num = num2;
										continue;
									}
									break;
								case false:
								case true:
									break;
								}
								goto case 3;
							}
							case 0:
								i = new Logger();
								num2 = 3;
								num = num2;
								continue;
							case 3:
								result = i;
								num2 = 1;
								num = num2;
								continue;
							case 1:
								goto end_IL_0014;
							}
							goto case 0;
							continue;
							end_IL_0014:
							break;
						}
						break;
					case 0:
						num2 = 1;
						if (num2 != 0)
						{
						}
						Monitor.Enter(obj, ref lockTaken);
						num2 = 2;
						num = num2;
						goto default;
					}
				}
				finally
				{
					num2 = 1;
					int num = num2;
					while (true)
					{
						switch (num)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (lockTaken)
							{
								num2 = 0;
								num = num2;
								continue;
							}
							break;
						case 0:
							Monitor.Exit(obj);
							num2 = 2;
							num = num2;
							continue;
						case 2:
							break;
						}
						break;
					}
				}
				num2 = 0;
				_ = num2;
				return result;
			}
		}

		public LogLevel LogLevel
		{
			[CompilerGenerated]
			get
			{
				short num = 21268;
				short num2 = num;
				num = 21268;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 0;
					if (num != 0)
					{
					}
					return this.m_a;
				}
			}
			[CompilerGenerated]
			set
			{
				short num = 15244;
				short num2 = num;
				num = 15244;
				switch (num2 == num)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				this.m_a = value;
			}
		}

		public string resolvedLogFilePathname
		{
			[CompilerGenerated]
			get
			{
				short num = 0;
				_ = num;
				num = 14114;
				short num2 = num;
				num = 14114;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					return this.m_b;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = 4506;
				short num2 = num;
				num = 4506;
				switch (num2 == num)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				this.m_b = value;
			}
		}

		public Exception Initialize(Configuration newConfiguration)
		{
			int a_ = 12;
			string a_2 = "24HRXI8PCAVFVYB8A10FE6Q6TYQRW2R1GFF14AK2SFA69KRXA3IP1RT9Z5KW7A";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				Exception ex = NotYetInitialized;
				try
				{
					bool A_ = default(bool);
					while (true)
					{
						object obj = h;
						bool lockTaken = false;
						try
						{
							switch (0)
							{
							case 0:
								goto IL_00b3;
							}
							goto IL_0058;
							IL_0058:
							while (true)
							{
								switch (num2)
								{
								case 14:
									if (!this.m_d)
									{
										num = 5;
										num2 = num;
										continue;
									}
									throw new InvalidOperationException("This object has already been initialized.");
								case 13:
									if (newConfiguration.targetDirectoryType == TargetDirectoryType.Specified)
									{
										num = 12;
										num2 = num;
										continue;
									}
									goto IL_0315;
								case 2:
									if (ex == null)
									{
										num = 17;
										num2 = num;
										continue;
									}
									goto case 11;
								case 15:
									WriteLine("** Logging started");
									num = 11;
									num2 = num;
									continue;
								case 7:
									if (!string.IsNullOrWhiteSpace(newConfiguration.targetFileNodeName))
									{
										this.m_e = (Configuration)newConfiguration.Clone();
										this.m_e.readOnly = true;
										this.m_d = true;
										ex = a(out A_);
										num = 2;
										num2 = num;
									}
									else
									{
										num = 18;
										num2 = num;
									}
									continue;
								case 12:
									num = 4;
									num2 = num;
									continue;
								case 4:
									if (string.IsNullOrWhiteSpace(newConfiguration.targetDirectorySpecified))
									{
										num = 20;
										num2 = num;
										continue;
									}
									goto IL_0315;
								case 8:
									WriteLine();
									num = 15;
									num2 = num;
									continue;
								case 3:
									if (newConfiguration.targetDirectoryType == TargetDirectoryType.CommonAppDataSubdirectory)
									{
										num = 16;
										num2 = num;
										continue;
									}
									goto IL_0218;
								case 5:
									num = 19;
									num2 = num;
									continue;
								case 19:
									if (newConfiguration != null)
									{
										num = 13;
										num2 = num;
									}
									else
									{
										num = 6;
										num2 = num;
									}
									continue;
								case 20:
									throw new ArgumentException("newConfiguration.targetDirectorySpecified must be non-whitespace when targetDirectoryType==Specified", "newConfiguration");
								case 6:
									throw new ArgumentNullException("newConfiguration");
								case 16:
									num = 0;
									num2 = num;
									continue;
								case 0:
									if (string.IsNullOrWhiteSpace(newConfiguration.targetDirectoryAppDataSubdirectory))
									{
										num = 9;
										num2 = num;
										continue;
									}
									goto IL_0218;
								case 17:
									num = 1;
									num2 = num;
									continue;
								case 1:
									if (!A_)
									{
										num = 8;
										num2 = num;
										continue;
									}
									goto case 15;
								case 18:
									throw new ArgumentException("newConfiguration.targetFileNodeName must be non-whitespace", "newConfiguration");
								case 9:
									throw new ArgumentException("newConfiguration.targetDirectoryAppDataSubdirectory must be non-whitespace when targetDirectoryType==CommonAppDataSubdirectory", "newConfiguration");
								case 11:
									num = 10;
									num2 = num;
									continue;
								case 10:
									goto end_IL_0058;
									IL_0315:
									num = 3;
									num2 = num;
									continue;
									IL_0218:
									num = 7;
									num2 = num;
									continue;
								}
								goto IL_00b3;
								continue;
								end_IL_0058:
								break;
							}
							goto end_IL_004e;
							IL_00b3:
							Monitor.Enter(obj, ref lockTaken);
							A_ = false;
							num = 14;
							num2 = num;
							goto IL_0058;
							end_IL_004e:;
						}
						finally
						{
							num = 1;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 1:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (lockTaken)
									{
										num = 0;
										num2 = num;
										continue;
									}
									break;
								case 0:
									Monitor.Exit(obj);
									num = 2;
									num2 = num;
									continue;
								case 2:
									break;
								}
								break;
							}
						}
						num = 19243;
						short num3 = num;
						num = 19243;
						switch (num3 == num)
						{
						case false:
						case true:
							continue;
						}
						num = 0;
						if (num == 0)
						{
						}
						break;
					}
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				return ex;
			}
			}
		}

		public void WriteLine(string logLineText = null, LogLevel messageLogLevel = LogLevel.Information)
		{
			int a_ = 17;
			string a_2 = "6L81JX7XSYK28R77VW4BESQTXBU426MASOWF936KYUSHUHBI0RA";
			short num;
			try
			{
				num = 0;
				switch (num)
				{
				default:
				{
					num = 4;
					int num2 = num;
					object obj = default(object);
					bool lockTaken = default(bool);
					StringBuilder stringBuilder = default(StringBuilder);
					StringBuilder stringBuilder2 = default(StringBuilder);
					bool A_ = default(bool);
					while (true)
					{
						switch (num2)
						{
						case 4:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (!this.m_d)
							{
								num = 8;
								num2 = num;
							}
							else
							{
								num = 3;
								num2 = num;
							}
							continue;
						case 6:
							obj = h;
							lockTaken = false;
							num = 2;
							num2 = num;
							continue;
						case 9:
							if (g == Logger.m_c)
							{
								num = 6;
								num2 = num;
								continue;
							}
							break;
						case 3:
							if (IncludeEntriesForLogLevel(messageLogLevel))
							{
								DateTimeOffset now = DateTimeOffset.Now;
								stringBuilder = new StringBuilder(100);
								stringBuilder2 = new StringBuilder(200);
								stringBuilder.AppendFormat("{0:yyyy MMM d HH:mm:ss.fff} 0x{1:X8}/{2:000}  ", now, AppDomain.GetCurrentThreadId(), Thread.CurrentThread.ManagedThreadId);
								stringBuilder2.AppendFormat("{0}  {1}\r\n", stringBuilder.ToString(), logLineText);
								num = 9;
								num2 = num;
							}
							else
							{
								num = 0;
								num2 = num;
							}
							continue;
						case 8:
							num = 5;
							num2 = num;
							continue;
						case 5:
							goto end_IL_0061;
						case 2:
							try
							{
								switch (0)
								{
								default:
									while (true)
									{
										switch (num2)
										{
										case 5:
										{
											num = 13467;
											short num3 = num;
											num = 13467;
											switch (num3 == num)
											{
											case false:
											case true:
												goto IL_0420;
											}
											num = 0;
											if (num != 0)
											{
											}
											if (g == Logger.m_c)
											{
												num = 0;
												num2 = num;
												continue;
											}
											goto case 4;
										}
										case 8:
											a("\r\n");
											num = 3;
											num2 = num;
											continue;
										case 0:
											num = 1;
											num2 = num;
											continue;
										case 1:
											if (a(out A_) == null)
											{
												num = 2;
												num2 = num;
												continue;
											}
											goto case 4;
										case 3:
											a(string.Format("{0}  {1}\r\n", stringBuilder, "** Logging started"));
											num = 4;
											num2 = num;
											continue;
										case 2:
											num = 6;
											num2 = num;
											continue;
										case 6:
											goto IL_0420;
										case 4:
											num = 7;
											num2 = num;
											continue;
										case 7:
											goto end_IL_0237;
											IL_0420:
											if (!A_)
											{
												num = 8;
												num2 = num;
												continue;
											}
											goto case 3;
										}
										goto case 0;
										continue;
										end_IL_0237:
										break;
									}
									break;
								case 0:
									Monitor.Enter(obj, ref lockTaken);
									num = 5;
									num2 = num;
									goto default;
								}
							}
							finally
							{
								num = 0;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 0:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (lockTaken)
										{
											num = 2;
											num2 = num;
											continue;
										}
										break;
									case 2:
										Monitor.Exit(obj);
										num = 1;
										num2 = num;
										continue;
									case 1:
										break;
									}
									break;
								}
							}
							break;
						case 0:
							num = 1;
							num2 = num;
							continue;
						case 1:
							goto end_IL_0061;
						case 7:
							goto end_IL_0061;
						}
						a(stringBuilder2.ToString());
						num = 7;
						num2 = num;
						continue;
						end_IL_0061:
						break;
					}
					break;
				}
				}
			}
			catch
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num == 0)
			{
			}
		}

		public void WriteLine(string formatString, params object[] arguments)
		{
			short num = -12121;
			short num2 = num;
			num = -12121;
			switch (num2 == num)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			DoNothrow(delegate
			{
				short num3 = -4335;
				short num4 = num3;
				num3 = -4335;
				switch (num4 == num3)
				{
				default:
					num3 = 0;
					_ = num3;
					num3 = 0;
					if (num3 != 0)
					{
					}
					num3 = 1;
					if (num3 != 0)
					{
					}
					WriteLine(string.Format(formatString, arguments));
					break;
				}
			});
		}

		public void WriteException(Exception exceptionToLog, string contextInformation = null)
		{
			int a_ = 8;
			string a_2 = "ODVJ7ZZRDHMOG6E0LXB5XR5ZCTYGT0UZJUTTPXUUER9MU9JTA8OU9JAJ4PWY4D";
			short num;
			try
			{
				num = 0;
				int num2 = num;
				switch (num2)
				{
				default:
				{
					string text = default(string);
					switch (0)
					{
					default:
					{
						string logLineText = default(string);
						WebException ex = default(WebException);
						while (true)
						{
							switch (num2)
							{
							case 6:
								if (exceptionToLog != null)
								{
									num = 20;
									num2 = num;
									continue;
								}
								goto case 16;
							case 12:
								text = exceptionToLog.Message;
								num = 1;
								num2 = num;
								continue;
							case 2:
								if (exceptionToLog is Win32Exception)
								{
									num = 5;
									num2 = num;
								}
								else
								{
									num = 14;
									num2 = num;
								}
								continue;
							case 13:
								logLineText = string.Format("Exception {0} occurred:  {1}", exceptionToLog.GetType().Name, text);
								num = 22;
								num2 = num;
								continue;
							case 19:
							{
								COMException ex2 = exceptionToLog as COMException;
								text = string.Format("Error 0x{0}:  {1}", ex2.ErrorCode, ex2.Message);
								num = 18;
								num2 = num;
								continue;
							}
							case 1:
								logLineText = null;
								num = 9;
								num2 = num;
								continue;
							case 9:
								if (string.IsNullOrWhiteSpace(contextInformation))
								{
									num = 13;
									num2 = num;
								}
								else
								{
									logLineText = string.Format("Exception {0} {1}:  {2}", exceptionToLog.GetType().Name, contextInformation, text);
									num = 0;
									num2 = num;
								}
								continue;
							case 0:
							case 22:
								WriteLine(logLineText, LogLevel.Error);
								WriteLine(exceptionToLog.StackTrace, LogLevel.Error);
								num = 16;
								num2 = num;
								continue;
							case 8:
								text = string.Format("Error {0} from {1}:  {2}", ex.Status, ex.Response.ResponseUri, ex.Message);
								num = 3;
								num2 = num;
								continue;
							case 5:
							{
								Win32Exception ex3 = exceptionToLog as Win32Exception;
								text = string.Format("Error {0}:  {1}", ex3.NativeErrorCode, ex3.Message);
								num = 7;
								num2 = num;
								continue;
							}
							case 3:
							case 7:
							case 18:
							case 21:
								num = 17;
								num2 = num;
								continue;
							case 17:
								if (text == null)
								{
									num = 12;
									num2 = num;
									continue;
								}
								goto case 1;
							case 14:
								if (exceptionToLog is WebException)
								{
									num = 10;
									num2 = num;
									continue;
								}
								goto case 3;
							case 20:
								num = 11;
								num2 = num;
								continue;
							case 11:
								if (!(exceptionToLog is COMException))
								{
									num = 8740;
									short num3 = num;
									num = 8740;
									switch (num3 == num)
									{
									default:
										num = 0;
										if (num != 0)
										{
										}
										num = 2;
										num2 = num;
										continue;
									case false:
									case true:
										break;
									}
									goto case 19;
								}
								num = 19;
								num2 = num;
								continue;
							case 10:
								ex = exceptionToLog as WebException;
								num = 4;
								num2 = num;
								continue;
							case 4:
								if (ex.Response == null)
								{
									text = string.Format("Error {0}:  {1}", ex.Status, ex.Message);
									num = 21;
									num2 = num;
								}
								else
								{
									num = 8;
									num2 = num;
								}
								continue;
							case 16:
								num = 15;
								num2 = num;
								continue;
							case 15:
								goto end_IL_0041;
							}
							goto case 0;
							continue;
							end_IL_0041:
							break;
						}
						break;
					}
					case 0:
						text = null;
						num = 6;
						num2 = num;
						goto default;
					}
					break;
				}
				}
			}
			catch
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num == 0)
			{
			}
		}

		public bool IncludeEntriesForLogLevel(LogLevel messageLogLevel)
		{
			short num = -21008;
			short num2 = num;
			num = -21008;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return messageLogLevel >= LogLevel;
			}
		}

		public void LogProcessInfo()
		{
			int a_ = 2;
			string a_2 = "NOHYEMP6IEY9CNJRUVUGP5IGTYD6IUUR87Z9VB8XXXOEJAOXD81332JNHGOKG";
			short num;
			try
			{
				num = 0;
				int num2 = num;
				switch (num2)
				{
				default:
				{
					string text2 = default(string);
					string text = default(string);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num2)
							{
							case 6:
								if (!Environment.Is64BitOperatingSystem)
								{
									num = 0;
									num2 = num;
									continue;
								}
								goto case 4;
							case 2:
								text2 = "32-bit";
								num = 5;
								num2 = num;
								continue;
							case 4:
								num = 1;
								num2 = num;
								continue;
							case 1:
								if (!Environment.Is64BitProcess)
								{
									goto IL_020c;
								}
								goto case 5;
							case 0:
								text = "32-bit";
								num = 4;
								num2 = num;
								continue;
							case 5:
								WriteLine("    {0} process, {1} OS.  {2}", text2, text, Environment.OSVersion.VersionString);
								WriteLine("");
								num = 3;
								num2 = num;
								continue;
							case 3:
								{
									num = -30768;
									short num3 = num;
									num = -30768;
									switch (num3 == num)
									{
									case false:
									case true:
										break;
									default:
										num = 0;
										if (num == 0)
										{
										}
										goto end_IL_003f;
									}
									goto IL_020c;
								}
								IL_020c:
								num = 2;
								num2 = num;
								continue;
							}
							goto case 0;
							continue;
							end_IL_003f:
							break;
						}
						break;
					case 0:
					{
						Process currentProcess = Process.GetCurrentProcess();
						ProcessModule mainModule = Process.GetCurrentProcess().MainModule;
						FileVersionInfo fileVersionInfo = mainModule.FileVersionInfo;
						text = "64-bit";
						text2 = "64-bit";
						WriteLine("    Process {0} {1}", currentProcess.Id, mainModule.FileName);
						WriteLine("    Product version {0}.{1}.{2}.{3}, file version {4}.{5}.{6}.{7}", fileVersionInfo.ProductMajorPart, fileVersionInfo.ProductMinorPart, fileVersionInfo.ProductPrivatePart, fileVersionInfo.ProductBuildPart, fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FilePrivatePart, fileVersionInfo.FileBuildPart);
						num = 6;
						num2 = num;
						goto default;
					}
					}
					break;
				}
				}
			}
			catch
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
		}

		public void LogProxyInfo(IWebProxy proxyObject, string name = null)
		{
			int a_ = 8;
			string a_2 = "BV24F9WJXT79XJQ9GDXDMZ4AEI8ZBYABACGLJ672L3EBO7SZ1CCGCOUGJN";
			short num = 0;
			switch (num)
			{
			default:
			{
				num = 35;
				int num2 = num;
				WebProxy webProxy = default(WebProxy);
				string[] bypassList2 = default(string[]);
				int num4 = default(int);
				FieldInfo field = default(FieldInfo);
				while (true)
				{
					switch (num2)
					{
					case 35:
						switch (0)
						{
						default:
							goto end_IL_0063;
						case 0:
							break;
						}
						goto default;
					default:
						if (proxyObject != null)
						{
							num = 24;
							num2 = num;
						}
						else
						{
							num = 2;
							num2 = num;
						}
						break;
					case 26:
						if (name == null)
						{
							num = 16;
							num2 = num;
							break;
						}
						globalLogger.WriteLine("Proxy {0}:  Not Configured", name);
						return;
					case 0:
						num = 28;
						num2 = num;
						break;
					case 28:
						if (name == null)
						{
							num = 5;
							num2 = num;
							break;
						}
						globalLogger.WriteLine("Proxy {0} Details", name);
						num = 14;
						num2 = num;
						break;
					case 29:
						globalLogger.WriteLine("Proxy:  Unrecognized type {0}, cannot describe", webProxy.GetType().Name);
						return;
					case 31:
						num = 36;
						num2 = num;
						break;
					case 36:
						if (webProxy == null)
						{
							num = 21;
							num2 = num;
							break;
						}
						goto IL_0266;
					case 2:
						if (name == null)
						{
							num = 38;
							num2 = num;
							break;
						}
						globalLogger.WriteLine("Proxy {0}:  Not Configured", name);
						num = 4;
						num2 = num;
						break;
					case 38:
						num = 1;
						if (num != 0)
						{
						}
						globalLogger.WriteLine("Proxy:  Not Configured");
						return;
					case 32:
					{
						Logger logger = globalLogger;
						string formatString = "   Bypass ({0} entries)";
						object[] bypassList = webProxy.BypassList;
						logger.WriteLine(formatString, bypassList);
						bypassList2 = webProxy.BypassList;
						num4 = 0;
						num = 34;
						num2 = num;
						break;
					}
					case 4:
						return;
					case 6:
					case 27:
						num = 1;
						num2 = num;
						break;
					case 1:
						if (webProxy.BypassProxyOnLocal)
						{
							num = 17;
							num2 = num;
							break;
						}
						goto case 11;
					case 24:
						webProxy = proxyObject as WebProxy;
						num = 19;
						num2 = num;
						break;
					case 19:
						if (webProxy == null)
						{
							num = 0;
							_ = num;
							num = 30;
							num2 = num;
							break;
						}
						goto case 31;
					case 5:
						globalLogger.WriteLine("Proxy Details");
						num = 25;
						num2 = num;
						break;
					case 13:
						num = 7;
						num2 = num;
						break;
					case 7:
						if (!(webProxy.Address != null))
						{
							num = 26;
							num2 = num;
						}
						else
						{
							num = 0;
							num2 = num;
						}
						break;
					case 33:
						globalLogger.WriteLine("   using Default port");
						num = 6;
						num2 = num;
						break;
					case 21:
						if (name != null)
						{
							globalLogger.WriteLine("Proxy {0}:  Unrecognized type {1}, cannot describe", name, webProxy.GetType().Name);
							return;
						}
						num = 29;
						num2 = num;
						break;
					case 16:
						globalLogger.WriteLine("Proxy:  Not Configured");
						return;
					case 17:
						globalLogger.WriteLine("   Local bypass ON");
						num = 11;
						num2 = num;
						break;
					case 15:
					case 34:
						num = 20;
						num2 = num;
						break;
					case 20:
						if (num4 < bypassList2.Length)
						{
							string text = bypassList2[num4];
							globalLogger.WriteLine("   {0}", text);
							num4++;
							num = 15;
							num2 = num;
						}
						else
						{
							num = 23;
							num2 = num;
						}
						break;
					case 14:
					case 25:
						globalLogger.WriteLine("   Address = {0}", webProxy.Address);
						num = 37;
						num2 = num;
						break;
					case 37:
						if (!webProxy.Address.IsDefaultPort)
						{
							globalLogger.WriteLine("   using port {0}", webProxy.Address.Port);
							num = 27;
							num2 = num;
						}
						else
						{
							num = 33;
							num2 = num;
						}
						break;
					case 12:
						webProxy = field.GetValue(proxyObject) as WebProxy;
						num = 31;
						num2 = num;
						break;
					case 11:
						num = 8;
						num2 = num;
						break;
					case 8:
						if (webProxy.BypassList != null)
						{
							num = 18;
							num2 = num;
							break;
						}
						goto case 23;
					case 18:
						num = 10;
						num2 = num;
						break;
					case 10:
						if (webProxy.BypassList.Length != 0)
						{
							num = 32;
							num2 = num;
							break;
						}
						goto case 23;
					case 30:
						field = proxyObject.GetType().GetField("webProxy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						num = 9;
						num2 = num;
						break;
					case 9:
						if (field != null)
						{
							num = 12;
							num2 = num;
							break;
						}
						goto case 31;
					case 23:
						num = 22;
						num2 = num;
						break;
					case 22:
						if (webProxy.UseDefaultCredentials)
						{
							num = 3;
							num2 = num;
							break;
						}
						return;
					case 3:
						{
							num = 24787;
							short num3 = num;
							num = 24787;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								globalLogger.WriteLine("   Using default credentials");
								return;
							}
							goto IL_0266;
						}
						IL_0266:
						num = 13;
						num2 = num;
						break;
						end_IL_0063:
						break;
					}
				}
			}
			}
		}

		public static Logger Create(TargetDirectoryType targetDirectoryType, string subDirectoryName, string logFileName, LogLevel logLevel = LogLevel.Information)
		{
			int a_ = 12;
			string a_2 = "IQWIPIE6UIFHQNJ1GKZY6K8GVZDM6NKON5H9GCVA9D53IV46MNYNQS5F";
			short num = 0;
			_ = num;
			num = 18221;
			short num2 = num;
			num = 18221;
			switch (num2 == num)
			{
			default:
			{
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				Logger obj = new Logger
				{
					LogLevel = logLevel
				};
				Configuration newConfiguration = new Configuration
				{
					targetDirectoryType = targetDirectoryType,
					targetDirectoryAppDataSubdirectory = subDirectoryName,
					targetFileNodeName = logFileName
				};
				Exception ex = obj.Initialize(newConfiguration);
				if (ex != null)
				{
					Console.WriteLine("Logger failed to initialize. Logging is disabled. Result: " + ex.Message);
				}
				return obj;
			}
			}
		}

		public void Dispose()
		{
			short num = 26922;
			short num2 = num;
			num = 26922;
			switch (num2 == num)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			a(A_0: true);
		}

		private void a(bool A_0)
		{
			int a_ = 0;
			string a_2 = "D7260C5Z81KEMXGDRZE2QPDQY1RVH0SYWXX9O3PACE7F2BXNNV8";
			int num = default(int);
			while (true)
			{
				GC.SuppressFinalize(this);
				object obj = h;
				bool lockTaken = false;
				short num2;
				try
				{
					switch (0)
					{
					default:
						while (true)
						{
							switch (num)
							{
							case 2:
								if (A_0)
								{
									num2 = 4;
									num = num2;
								}
								else
								{
									WriteLine("** Logging shutdown (improper shutdown detected)");
									num2 = 0;
									num = num2;
								}
								continue;
							case 4:
								WriteLine("** Logging shutdown");
								num2 = 3;
								num = num2;
								continue;
							case 0:
							case 3:
								DoNothrow(delegate
								{
									short num4 = 1;
									int num5 = num4;
									while (true)
									{
										switch (num5)
										{
										case 1:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											if (g != Logger.m_c)
											{
												num4 = 0;
												num5 = num4;
												continue;
											}
											return;
										case 0:
											break;
										case 2:
											return;
										}
										while (true)
										{
											num4 = 1;
											if (num4 != 0)
											{
											}
											num4 = 7001;
											short num6 = num4;
											num4 = 7001;
											switch (num6 == num4)
											{
											case false:
											case true:
												continue;
											}
											break;
										}
										num4 = 0;
										_ = num4;
										num4 = 0;
										if (num4 != 0)
										{
										}
										CloseHandle(g);
										g = IntPtr.Zero;
										num4 = 2;
										num5 = num4;
									}
								});
								this.m_e = null;
								resolvedLogFilePathname = null;
								f = null;
								this.m_d = false;
								num2 = 1;
								num = num2;
								continue;
							case 1:
								goto end_IL_002d;
							}
							goto case 0;
							continue;
							end_IL_002d:
							break;
						}
						break;
					case 0:
						Monitor.Enter(obj, ref lockTaken);
						num2 = 2;
						num = num2;
						goto default;
					}
				}
				finally
				{
					num2 = 2;
					num = num2;
					while (true)
					{
						switch (num)
						{
						case 2:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (lockTaken)
							{
								num2 = 1;
								num = num2;
								continue;
							}
							break;
						case 1:
							Monitor.Exit(obj);
							num2 = 0;
							num = num2;
							continue;
						case 0:
							break;
						}
						break;
					}
				}
				num2 = 28324;
				short num3 = num2;
				num2 = 28324;
				switch (num3 == num2)
				{
				case false:
				case true:
					continue;
				}
				num2 = 1;
				if (num2 != 0)
				{
				}
				num2 = 0;
				if (num2 != 0)
				{
				}
				num2 = 0;
				_ = num2;
				return;
			}
		}

		~Logger()
		{
			short num;
			try
			{
				num = -27234;
				short num2 = num;
				num = -27234;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					Dispose();
					break;
				}
			}
			finally
			{
				base.Finalize();
			}
			num = 0;
			_ = num;
			num = 1;
			if (num == 0)
			{
			}
		}

		private void b(string A_0)
		{
			while (true)
			{
				short num = 0;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 0:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (!Directory.Exists(A_0))
						{
							num = 1;
							if (num != 0)
							{
							}
							num = 1;
							num2 = num;
							continue;
						}
						return;
					case 1:
						break;
					case 2:
						num = 0;
						_ = num;
						return;
					}
					num = 32372;
					short num3 = num;
					num = 32372;
					switch (num3 == num)
					{
					case false:
					case true:
						break;
					default:
						num = 0;
						if (num != 0)
						{
						}
						Directory.CreateDirectory(A_0);
						num = 2;
						num2 = num;
						continue;
					}
					break;
				}
			}
		}

		private Exception a(out bool A_0)
		{
			int a_ = 12;
			string a_2 = "0KAJN7SCSWXHDDYMRVME999W85N06BOLLZ0IY5THAOQRM87E2A18KMI8";
			short num = 0;
			switch (num)
			{
			default:
			{
				bool flag = false;
				Exception result = NotYetInitialized;
				A_0 = false;
				try
				{
					num = 1;
					int num2 = num;
					string text = default(string);
					string a_3 = default(string);
					IntPtr A_ = default(IntPtr);
					FileStream fileStream = default(FileStream);
					byte[] bytes = default(byte[]);
					bool flag2 = default(bool);
					byte[] array = default(byte[]);
					while (true)
					{
						switch (num2)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (this.m_e.targetDirectoryType == TargetDirectoryType.Temp)
							{
								num = 11;
								num2 = num;
							}
							else
							{
								num = 6;
								num2 = num;
							}
							continue;
						case 14:
							text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), this.m_e.targetDirectoryAppDataSubdirectory);
							num = 9;
							num2 = num;
							continue;
						case 2:
							result = a(a_3, Logger.d.a | Logger.d.b, Logger.a.b | Logger.a.c, IntPtr.Zero, Logger.c.d, Logger.b.a | Logger.b.r, out A_);
							num = 0;
							num2 = num;
							continue;
						case 0:
							if (A_ != Logger.m_c)
							{
								num = 3;
								num2 = num;
								continue;
							}
							break;
						case 6:
							if (this.m_e.targetDirectoryType == TargetDirectoryType.CommonAppDataSubdirectory)
							{
								num = 14;
								num2 = num;
							}
							else
							{
								text = this.m_e.targetDirectorySpecified;
								num = 12;
								num2 = num;
							}
							continue;
						case 3:
							g = A_;
							resolvedLogFilePathname = a_3;
							f = text;
							result = null;
							num = 13;
							num2 = num;
							continue;
						case 4:
							fileStream = new FileStream(A_, FileAccess.ReadWrite);
							num = 10;
							num2 = num;
							continue;
						case 10:
							try
							{
								num = 17;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 17:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (fileStream.Length == 0L)
										{
											num = 18;
											num2 = num;
										}
										else
										{
											num = 6;
											num2 = num;
										}
										continue;
									case 4:
										flag = true;
										num = 9;
										num2 = num;
										continue;
									case 8:
									{
										byte[] preamble = new UnicodeEncoding(bigEndian: false, byteOrderMark: true).GetPreamble();
										fileStream.Write(preamble, 0, preamble.Length);
										fileStream.Flush();
										num = 4;
										num2 = num;
										continue;
									}
									case 6:
										if (this.m_e.onOpenEnsureEndsInNewline)
										{
											num = 10;
											num2 = num;
											continue;
										}
										break;
									case 0:
										fileStream.Write(bytes, 0, bytes.Length);
										fileStream.Flush();
										num = 11;
										num2 = num;
										continue;
									case 5:
										flag2 = array.EqualsContents(bytes);
										num = 15;
										num2 = num;
										continue;
									case 12:
										bytes = Encoding.Unicode.GetBytes("\r\n");
										num = 1;
										num2 = num;
										continue;
									case 10:
										num = 16;
										num2 = num;
										continue;
									case 16:
										if (this.m_e.encoding == CharacterEncoding.Unicode)
										{
											num = 12;
											num2 = num;
										}
										else
										{
											bytes = Encoding.ASCII.GetBytes("\r\n");
											num = 7;
											num2 = num;
										}
										continue;
									case 1:
									case 7:
										array = new byte[bytes.Length];
										fileStream.Seek(-array.Length, SeekOrigin.End);
										num = 3;
										num2 = num;
										continue;
									case 3:
									{
										int num4 = fileStream.Read(array, 0, array.Length);
										flag2 = false;
										if (num4 == bytes.Length)
										{
											num = 5;
											num2 = num;
											continue;
										}
										goto case 15;
									}
									case 18:
										num = 2;
										num2 = num;
										continue;
									case 2:
										if (this.m_e.encoding == CharacterEncoding.Unicode)
										{
											num = 8;
											num2 = num;
											continue;
										}
										goto case 4;
									case 15:
										num = 13;
										num2 = num;
										continue;
									case 13:
										if (!flag2)
										{
											num = 0;
											num2 = num;
											continue;
										}
										break;
									case 9:
									case 11:
										break;
									case 14:
										goto end_IL_02d7;
									}
									num = 14;
									num2 = num;
									continue;
									end_IL_02d7:
									break;
								}
							}
							finally
							{
								num = 0;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 0:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (fileStream != null)
										{
											num = 2;
											num2 = num;
											continue;
										}
										break;
									case 2:
										((IDisposable)fileStream).Dispose();
										num = 1;
										num2 = num;
										continue;
									case 1:
										break;
									}
									break;
								}
							}
							A_ = IntPtr.Zero;
							num = 2;
							num2 = num;
							continue;
						case 11:
							text = Path.GetTempPath();
							num = 8;
							num2 = num;
							continue;
						case 8:
						case 9:
						case 12:
							a_3 = Path.Combine(text, this.m_e.targetFileNodeName);
							b(text);
							a(a_3, Logger.d.b | Logger.d.c, Logger.a.a, IntPtr.Zero, Logger.c.d, Logger.b.a | Logger.b.r, out A_);
							num = 5;
							num2 = num;
							continue;
						case 5:
							if (A_ != Logger.m_c)
							{
								num = 4;
								num2 = num;
								continue;
							}
							goto case 2;
						case 13:
							break;
						case 7:
						{
							num = 26338;
							short num3 = num;
							num = 26338;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num == 0)
								{
								}
								goto end_IL_0070;
							}
							break;
						}
						}
						num = 7;
						num2 = num;
						continue;
						end_IL_0070:
						break;
					}
				}
				catch (Exception ex)
				{
					result = ex;
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				A_0 = flag;
				return result;
			}
			}
		}

		private void a(string A_0)
		{
			short num = 0;
			int num2 = num;
			byte[] bytes = default(byte[]);
			object obj = default(object);
			bool lockTaken = default(bool);
			while (true)
			{
				switch (num2)
				{
				case 0:
					num = 0;
					_ = num;
					switch (0)
					{
					default:
						goto end_IL_0068;
					case 0:
						break;
					}
					goto default;
				default:
					if (this.m_e.encoding == CharacterEncoding.Unicode)
					{
						num = 2;
						num2 = num;
					}
					else
					{
						bytes = Encoding.Default.GetBytes(A_0.ToString());
						num = 7;
						num2 = num;
					}
					break;
				case 2:
					num = 1;
					if (num != 0)
					{
					}
					bytes = Encoding.Unicode.GetBytes(A_0.ToString());
					num = 6;
					num2 = num;
					break;
				case 5:
					try
					{
						switch (0)
						{
						default:
							while (true)
							{
								switch (num2)
								{
								case 1:
									if (a(g, bytes) != null)
									{
										num = 0;
										num2 = num;
										continue;
									}
									goto case 3;
								case 0:
									a(g);
									g = Logger.m_c;
									num = 3;
									num2 = num;
									continue;
								case 3:
									num = 2;
									num2 = num;
									continue;
								case 2:
									goto end_IL_00fe;
								}
								goto case 0;
								continue;
								end_IL_00fe:
								break;
							}
							break;
						case 0:
							Monitor.Enter(obj, ref lockTaken);
							num = 1;
							num2 = num;
							goto default;
						}
					}
					finally
					{
						num = 0;
						num2 = num;
						while (true)
						{
							switch (num2)
							{
							case 0:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (lockTaken)
								{
									num = 1;
									num2 = num;
									continue;
								}
								break;
							case 1:
								Monitor.Exit(obj);
								num = 2;
								num2 = num;
								continue;
							case 2:
								break;
							}
							break;
						}
					}
					num = 3;
					num2 = num;
					break;
				case 4:
					Console.WriteLine(A_0);
					num = 1;
					num2 = num;
					break;
				case 1:
					return;
				case 3:
				{
					num = -7576;
					short num3 = num;
					num = -7576;
					switch (num3 == num)
					{
					default:
						num = 0;
						if (num != 0)
						{
						}
						if (this.m_e.logToStdout)
						{
							num = 4;
							num2 = num;
							goto end_IL_0068;
						}
						return;
					case false:
					case true:
						break;
					}
					goto IL_03b0;
				}
				case 6:
				case 7:
					{
						obj = h;
						lockTaken = false;
						goto IL_03b0;
					}
					IL_03b0:
					num = 5;
					num2 = num;
					break;
					end_IL_0068:
					break;
				}
			}
		}

		private static Exception a(string A_0, d A_1, a A_2, IntPtr A_3, c A_4, b A_5, out IntPtr A_6)
		{
			Exception result = NotYetInitialized;
			A_6 = Logger.m_c;
			short num2;
			try
			{
				int num = default(int);
				switch (0)
				{
				default:
					while (true)
					{
						switch (num)
						{
						case 1:
							if (A_6 != Logger.m_c)
							{
								num2 = 15288;
								short num3 = num2;
								num2 = 15288;
								switch (num3 == num2)
								{
								case false:
								case true:
									continue;
								}
								num2 = 0;
								if (num2 != 0)
								{
								}
								num2 = 0;
								num = num2;
								continue;
							}
							throw new Win32Exception(Marshal.GetLastWin32Error());
						case 0:
							result = null;
							num2 = 3;
							num = num2;
							continue;
						case 3:
							num2 = 2;
							num = num2;
							continue;
						case 2:
							goto end_IL_001a;
						}
						goto case 0;
						continue;
						end_IL_001a:
						break;
					}
					break;
				case 0:
					A_6 = CreateFile(A_0, A_1, A_2, A_3, A_4, A_5, IntPtr.Zero);
					num2 = 1;
					num = num2;
					goto default;
				}
			}
			catch (Exception ex)
			{
				result = ex;
			}
			num2 = 0;
			_ = num2;
			num2 = 1;
			if (num2 != 0)
			{
			}
			return result;
		}

		private static Exception a(IntPtr A_0)
		{
			Exception result = NotYetInitialized;
			short num;
			try
			{
				num = 2;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 2:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
					{
						num = -31492;
						short num3 = num;
						num = -31492;
						switch (num3 == num)
						{
						default:
							num = 0;
							if (num != 0)
							{
							}
							if (!CloseHandle(A_0))
							{
								num = 0;
								num2 = num;
								continue;
							}
							throw new Win32Exception(Marshal.GetLastWin32Error());
						case false:
						case true:
							break;
						}
						break;
					}
					case 0:
						result = null;
						num = 1;
						num2 = num;
						continue;
					case 1:
						num = 3;
						num2 = num;
						continue;
					case 3:
						break;
					}
					break;
				}
			}
			catch (Exception ex)
			{
				result = ex;
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return result;
		}

		private static Exception a(IntPtr A_0, byte[] A_1)
		{
			Exception result = NotYetInitialized;
			uint A_2 = 0u;
			short num;
			try
			{
				num = 1;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 1:
					{
						num = -255;
						short num3 = num;
						num = -255;
						switch (num3 == num)
						{
						case false:
						case true:
							continue;
						}
						num = 0;
						if (num != 0)
						{
						}
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					}
					default:
						if (WriteFile(A_0, A_1, (uint)A_1.Length, out A_2, IntPtr.Zero) != 0)
						{
							num = 2;
							num2 = num;
							continue;
						}
						throw new Win32Exception(Marshal.GetLastWin32Error());
					case 2:
						result = null;
						num = 0;
						num2 = num;
						continue;
					case 0:
						num = 3;
						num2 = num;
						continue;
					case 3:
						break;
					}
					break;
				}
			}
			catch (Exception ex)
			{
				result = ex;
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return result;
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr CreateFile(string A_0, d A_1, a A_2, IntPtr A_3, c A_4, b A_5, IntPtr A_6);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int WriteFile(IntPtr A_0, byte[] A_1, uint A_2, out uint A_3, IntPtr A_4);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr A_0);

		public void AppendDebugLine(string debugLine)
		{
			short num = -25663;
			short num2 = num;
			num = -25663;
			switch (num2 == num)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			WriteLine(debugLine, LogLevel.Debug);
		}

		static Logger()
		{
			short num = 28705;
			short num2 = num;
			num = 28705;
			switch (num2 == num)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			Logger.m_c = new IntPtr(-1);
			j = new object();
		}

		[CompilerGenerated]
		private void a()
		{
			short num = 1;
			int num2 = num;
			while (true)
			{
				switch (num2)
				{
				case 1:
					switch (0)
					{
					default:
						continue;
					case 0:
						break;
					}
					goto default;
				default:
					if (g != Logger.m_c)
					{
						num = 0;
						num2 = num;
						continue;
					}
					return;
				case 0:
					break;
				case 2:
					return;
				}
				while (true)
				{
					num = 1;
					if (num != 0)
					{
					}
					num = 7001;
					short num3 = num;
					num = 7001;
					switch (num3 == num)
					{
					case false:
					case true:
						continue;
					}
					break;
				}
				num = 0;
				_ = num;
				num = 0;
				if (num != 0)
				{
				}
				CloseHandle(g);
				g = IntPtr.Zero;
				num = 2;
				num2 = num;
			}
		}
	}

	[DataContract]
	public abstract class ReadOnlyBase : IReadOnly
	{
		protected bool property_readOnly;

		public bool readOnly
		{
			get
			{
				short num = -15535;
				short num2 = num;
				num = -15535;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 0;
					if (num != 0)
					{
					}
					return property_readOnly;
				}
			}
			set
			{
				short num = 3;
				int num2 = num;
				while (true)
				{
					num = 0;
					_ = num;
					switch (num2)
					{
					case 3:
						switch (0)
						{
						case 0:
							goto IL_0076;
						}
						break;
					default:
						goto IL_0076;
					case 0:
						num = 1;
						if (num != 0)
						{
						}
						throw new ReadOnlyException();
					case 2:
						if (!value)
						{
							num = 0;
							num2 = num;
							break;
						}
						return;
					case 1:
						{
							property_readOnly = value;
							return;
						}
						IL_0076:
						if (!property_readOnly)
						{
							num = -7789;
							short num3 = num;
							num = -7789;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								goto IL_00c8;
							}
							goto case 3;
						}
						num = 2;
						num2 = num;
						break;
						IL_00c8:
						num = 0;
						if (num != 0)
						{
						}
						num = 1;
						num2 = num;
						break;
					}
				}
			}
		}

		protected bool SetOrThrowIfReadOnly<T>(ref T backingField, T newValue)
		{
			int num = default(int);
			bool result = default(bool);
			switch (0)
			{
			default:
			{
				T val = default(T);
				while (true)
				{
					short num2;
					switch (num)
					{
					case 14:
						if (!readOnly)
						{
							num2 = 1;
							num = num2;
							continue;
						}
						throw new ReadOnlyException();
					case 1:
					{
						num2 = 22794;
						short num3 = num2;
						num2 = 22794;
						switch (num3 == num2)
						{
						default:
							num2 = 0;
							if (num2 != 0)
							{
							}
							val = backingField;
							num2 = 8;
							num = num2;
							continue;
						case false:
						case true:
							break;
						}
						goto case 4;
					}
					case 4:
						result = true;
						num2 = 9;
						num = num2;
						continue;
					case 0:
						if (val == null)
						{
							num2 = 13;
							num = num2;
							continue;
						}
						goto case 4;
					case 12:
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 2;
						num = num2;
						continue;
					case 2:
						if (newValue != null)
						{
							num2 = 11;
							num = num2;
							continue;
						}
						goto IL_011e;
					case 5:
					case 6:
					case 9:
						backingField = newValue;
						num2 = 3;
						num = num2;
						continue;
					case 8:
						if (val != null)
						{
							num2 = 12;
							num = num2;
							continue;
						}
						goto IL_011e;
					case 13:
						num2 = 10;
						num = num2;
						continue;
					case 10:
						if (newValue != null)
						{
							num2 = 4;
							num = num2;
							continue;
						}
						goto case 5;
					case 7:
					{
						string obj = (string)(object)val;
						string value = (string)(object)newValue;
						result = !obj.Equals(value, StringComparison.CurrentCultureIgnoreCase);
						num2 = 6;
						num = num2;
						continue;
					}
					case 11:
						num2 = 15;
						num = num2;
						continue;
					case 15:
						if (typeof(T).Equals(typeof(string)))
						{
							num2 = 7;
							num = num2;
							continue;
						}
						num2 = 0;
						_ = num2;
						result = !val.Equals(newValue);
						num2 = 5;
						num = num2;
						continue;
					case 3:
						{
							return result;
						}
						IL_011e:
						num2 = 0;
						num = num2;
						continue;
					}
					break;
				}
				goto case 0;
			}
			case 0:
			{
				result = false;
				short num2 = 14;
				num = num2;
				goto default;
			}
			}
		}
	}

	public interface IReadOnly
	{
		[IgnoreDataMember]
		bool readOnly { get; set; }
	}

	public enum WindowsServerFeatures
	{
		ApplicationServer = 1,
		WebServer = 2,
		AspNet = 148,
		RemoteDesktopGateway = 132,
		RemoteDesktopWeb = 134
	}

	public enum IpType
	{
		Any,
		Ipv4,
		Ipv6
	}

	public static class Serialization
	{
		public static class JsonEnumSerializer
		{
			private class a
			{
				public Dictionary<string, Enum> a;

				public Dictionary<Enum, string> b;

				public a(Type A_0)
				{
					int a_ = 11;
					string a_2 = "D4QMKQW574E8K7OQIC8RP08P9C2FITSRWJPFJ5829V935GEV2AC4FXW53";
					a = new Dictionary<string, Enum>(StringComparer.InvariantCultureIgnoreCase);
					b = new Dictionary<Enum, string>();
					base..ctor();
					if (A_0.IsEnum)
					{
						FieldInfo[] fields = A_0.GetFields(BindingFlags.Static | BindingFlags.Public);
						int num = 0;
						Enum @enum;
						while (true)
						{
							if (num < fields.Length)
							{
								FieldInfo obj = fields[num];
								@enum = (Enum)obj.GetValue(null);
								EnumMemberAttribute[] array = (EnumMemberAttribute[])obj.GetCustomAttributes(typeof(EnumMemberAttribute), inherit: true);
								if (array.Length == 0)
								{
									break;
								}
								if (string.IsNullOrEmpty(array[0].Value))
								{
									throw new ArgumentException(string.Format("Enum value {0};s EnumMember attribute does not have a Value property set", @enum), "enumType");
								}
								a.Add(array[0].Value, @enum);
								b.Add(@enum, array[0].Value);
								num++;
								continue;
							}
							return;
						}
						throw new ArgumentException(string.Format("Enum value {0} does not have an EnumMember attribute", @enum), "enumType");
					}
					throw new ArgumentException("enumType must be an enumeration type", "enumType");
				}
			}

			private static Dictionary<Type, a> m_a;

			public static string GetStringForEnum<T>(Enum enumValue) where T : struct
			{
				int a_ = 4;
				string a_2 = "E6VCWX1P5Z3V69MVFFE7UQXCDJ2EVED824RHQ6XEWX49KR02NMTJFYEPOJ5C1";
				short num = 5;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 5:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
					{
						num = -29362;
						short num3 = num;
						num = -29362;
						switch (num3 == num)
						{
						default:
							num = 1;
							if (num != 0)
							{
							}
							num = 0;
							if (num != 0)
							{
							}
							if (typeof(T).IsEnum)
							{
								num = 0;
								num2 = num;
								break;
							}
							goto case false;
						case false:
						case true:
							num = 3;
							num2 = num;
							break;
						}
						continue;
					}
					case 6:
						throw new ArgumentException("enumValue must be of same type as method generic type", "enumValue");
					case 4:
						if (enumValue.GetType() != typeof(T))
						{
							num = 6;
							num2 = num;
						}
						else
						{
							num = 2;
							num2 = num;
						}
						continue;
					case 0:
						if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
						{
							num = 4;
							num2 = num;
						}
						else
						{
							num = 1;
							num2 = num;
						}
						continue;
					case 7:
						JsonEnumSerializer.m_a.Add(typeof(T), new a(typeof(T)));
						num = 8;
						num2 = num;
						continue;
					case 3:
						throw new ArgumentException("T must be an enumeration", "T");
					case 2:
						if (!JsonEnumSerializer.m_a.ContainsKey(enumValue.GetType()))
						{
							num = 7;
							num2 = num;
							continue;
						}
						break;
					case 1:
						throw new ArgumentException("Flags enumerations are not supported", "T");
					case 8:
						break;
					}
					break;
				}
				num = 0;
				_ = num;
				return JsonEnumSerializer.m_a[enumValue.GetType()].b[enumValue];
			}

			public static T GetEnumFromString<T>(string enumName, T valueIfUnrecognized) where T : struct
			{
				int a_ = 14;
				string a_2 = "297OE84STUY3YUP7JL96OMJH5ZZKKLJ0027XG96IIEUUG9YTPHCVP4NN71Y74A";
				short num = 8;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 8:
						switch (0)
						{
						default:
							goto end_IL_0037;
						case 0:
							break;
						}
						goto default;
					default:
						num = 1;
						if (num != 0)
						{
						}
						if (!typeof(T).IsEnum)
						{
							num = 5;
							num2 = num;
						}
						else
						{
							num = 2;
							num2 = num;
						}
						break;
					case 0:
					{
						num = 0;
						_ = num;
						num = 12185;
						short num3 = num;
						num = 12185;
						switch (num3 == num)
						{
						default:
							num = 0;
							if (num != 0)
							{
							}
							JsonEnumSerializer.m_a.Add(typeof(T), new a(typeof(T)));
							num = 1;
							num2 = num;
							goto end_IL_0037;
						case false:
						case true:
							break;
						}
						goto IL_01c0;
					}
					case 3:
						if (!JsonEnumSerializer.m_a.ContainsKey(typeof(T)))
						{
							num = 0;
							num2 = num;
							break;
						}
						goto case 1;
					case 2:
						if (Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
						{
							num = 4;
							num2 = num;
							break;
						}
						goto IL_01c0;
					case 6:
						return (T)(ValueType)JsonEnumSerializer.m_a[typeof(T)].a[enumName];
					case 5:
						throw new ArgumentException("T must be an enumeration", "T");
					case 1:
						num = 7;
						num2 = num;
						break;
					case 7:
						if (JsonEnumSerializer.m_a[typeof(T)].a.ContainsKey(enumName))
						{
							num = 6;
							num2 = num;
							break;
						}
						return valueIfUnrecognized;
					case 4:
						{
							throw new ArgumentException("Flags enumerations are not supported", "T");
						}
						IL_01c0:
						num = 3;
						num2 = num;
						break;
						end_IL_0037:
						break;
					}
				}
			}

			public static T GetEnumFromString<T>(string enumName) where T : struct
			{
				int a_ = 11;
				string a_2 = "H55WCAB1LZLVM9BA587QQK6Y69LRH6ZR1LIBC558QAWAT1RHLS8";
				short num = 0;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 0:
					{
						num = 0;
						_ = num;
						num = -13229;
						short num3 = num;
						num = -13229;
						switch (num3 == num)
						{
						case false:
						case true:
							goto IL_01e5;
						}
						num = 0;
						if (num != 0)
						{
						}
						num = 1;
						if (num != 0)
						{
						}
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					}
					default:
						if (!typeof(T).IsEnum)
						{
							num = 1;
							num2 = num;
						}
						else
						{
							num = 5;
							num2 = num;
						}
						continue;
					case 3:
						if (!JsonEnumSerializer.m_a.ContainsKey(typeof(T)))
						{
							num = 6;
							num2 = num;
							continue;
						}
						break;
					case 4:
						throw new ArgumentException("Flags enumerations are not supported", "T");
					case 1:
						goto IL_01e5;
					case 6:
						JsonEnumSerializer.m_a.Add(typeof(T), new a(typeof(T)));
						num = 2;
						num2 = num;
						continue;
					case 5:
						if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
						{
							num = 3;
							num2 = num;
						}
						else
						{
							num = 4;
							num2 = num;
						}
						continue;
					case 2:
						break;
						IL_01e5:
						throw new ArgumentException("T must be an enumeration", "T");
					}
					break;
				}
				return (T)(ValueType)JsonEnumSerializer.m_a[typeof(T)].a[enumName];
			}

			static JsonEnumSerializer()
			{
				short num = 12923;
				short num2 = num;
				num = 12923;
				switch (num2 == num)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				JsonEnumSerializer.m_a = new Dictionary<Type, a>();
			}
		}

		public static class Registry
		{
			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberCustomAttribute : SerializableAttributeBase
			{
				[CompilerGenerated]
				private Type a;

				public Type ObjectType
				{
					[CompilerGenerated]
					get
					{
						short num = 29212;
						short num2 = num;
						num = 29212;
						switch (num2 == num)
						{
						default:
							num = 0;
							_ = num;
							break;
						case true:
							break;
						}
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						if (num != 0)
						{
						}
						return a;
					}
					[CompilerGenerated]
					set
					{
						short num = -13568;
						short num2 = num;
						num = -13568;
						switch (num2 == num)
						{
						}
						num = 0;
						if (num != 0)
						{
						}
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						_ = num;
						a = value;
					}
				}

				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					int a_ = 2;
					string a_2 = "9QA54WTXK2S4TQUY26C2TQWJ8CPS7RSS3KTMD2WVHTHTAWLAPYM56A8GIDT";
					short num = 1;
					if (num != 0)
					{
					}
					num = 0;
					int num2 = num;
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					FieldInfo memberInfoField = default(FieldInfo);
					Type type = default(Type);
					bool flag = default(bool);
					object valueFromObject = default(object);
					object obj = default(object);
					ICustomSerialization customSerialization = default(ICustomSerialization);
					switch (num2)
					{
					default:
						{
							switch (0)
							{
							case 0:
								goto IL_00cb;
							}
							goto IL_0078;
						}
						IL_0078:
						while (true)
						{
							switch (num2)
							{
							case 5:
								if (CheckAndCastMemberInfo(sourceMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									num = 6;
									num2 = num;
								}
								else
								{
									type = memberInfoField.FieldType;
									num = 1;
									num2 = num;
								}
								continue;
							case 7:
							case 8:
							case 10:
								num = 16;
								num2 = num;
								continue;
							case 16:
								if (flag)
								{
									num = 15;
									num2 = num;
									continue;
								}
								num = 0;
								_ = num;
								return;
							case 4:
								if (valueFromObject.Equals(obj))
								{
									num = 9;
									num2 = num;
									continue;
								}
								goto case 7;
							case 9:
							{
								num = 18167;
								short num3 = num;
								num = 18167;
								switch (num3 == num)
								{
								case false:
								case true:
									continue;
								}
								num = 0;
								if (num != 0)
								{
								}
								flag = false;
								num = 7;
								num2 = num;
								continue;
							}
							case 1:
							case 13:
								num = 0;
								num2 = num;
								continue;
							case 0:
								if (valueFromObject != null)
								{
									num = 14;
									num2 = num;
								}
								else
								{
									flag = false;
									num = 10;
									num2 = num;
								}
								continue;
							case 18:
								customSerialization.Serialize(targetKey, targetKeyPathname, obj);
								return;
							case 17:
								flag = false;
								num = 8;
								num2 = num;
								continue;
							case 14:
								num = 2;
								num2 = num;
								continue;
							case 2:
								if (defaultValues != null)
								{
									num = 3;
									num2 = num;
									continue;
								}
								goto case 7;
							case 15:
								customSerialization = valueFromObject as ICustomSerialization;
								num = 11;
								num2 = num;
								continue;
							case 11:
								if (customSerialization == null)
								{
									throw new ArgumentException(string.Format("Type {0} does not implement ICustomSerialization.", type.Name));
								}
								num = 18;
								num2 = num;
								continue;
							case 6:
								type = memberInfoProperty.PropertyType;
								num = 13;
								num2 = num;
								continue;
							case 3:
								obj = GetValueFromObject<object>(defaultValues, sourceMember);
								num = 12;
								num2 = num;
								continue;
							case 12:
								if (valueFromObject != obj)
								{
									num = 4;
									num2 = num;
								}
								else
								{
									num = 17;
									num2 = num;
								}
								continue;
							}
							break;
						}
						goto IL_00cb;
						IL_00cb:
						memberInfoProperty = null;
						memberInfoField = null;
						type = null;
						valueFromObject = GetValueFromObject<object>(sourceObject, sourceMember);
						obj = null;
						flag = true;
						num = 5;
						num2 = num;
						goto IL_0078;
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					int a_ = 7;
					string a_2 = "1VD0NO83EN75WYEF9EHHAU1HP4JB6DGHOQRIN2TGTYMB794X00IC30IDI";
					short num = 0;
					int num2 = num;
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					FieldInfo memberInfoField = default(FieldInfo);
					Type type = default(Type);
					object defaultValue = default(object);
					object value = default(object);
					ICustomSerialization customSerialization = default(ICustomSerialization);
					object obj = default(object);
					switch (num2)
					{
					default:
						{
							switch (0)
							{
							case 0:
								goto IL_007e;
							}
							goto IL_0047;
						}
						IL_0047:
						while (true)
						{
							switch (num2)
							{
							case 8:
								if (CheckAndCastMemberInfo(targetMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									num = 5;
									num2 = num;
									continue;
								}
								type = memberInfoField.FieldType;
								defaultValue = memberInfoField.GetValue(targetObject);
								num = 0;
								_ = num;
								num = 3;
								num2 = num;
								continue;
							case 4:
								value = customSerialization.Deserialize(sourceKey, sourceKeyPathname, defaultValue);
								num = 6;
								num2 = num;
								continue;
							case 6:
								if (memberInfoProperty != null)
								{
									num = 11;
									num2 = num;
									continue;
								}
								memberInfoField.SetValue(targetObject, value);
								return;
							case 11:
							{
								num = -26369;
								short num3 = num;
								num = -26369;
								switch (num3 == num)
								{
								default:
									num = 0;
									if (num != 0)
									{
									}
									memberInfoProperty.SetValue(targetObject, value, null);
									return;
								case false:
								case true:
									break;
								}
								goto case 9;
							}
							case 1:
								num = 1;
								if (num == 0)
								{
								}
								goto case 2;
							case 5:
								type = memberInfoProperty.PropertyType;
								defaultValue = memberInfoProperty.GetValue(targetObject, null);
								num = 7;
								num2 = num;
								continue;
							case 2:
								customSerialization = obj as ICustomSerialization;
								num = 10;
								num2 = num;
								continue;
							case 10:
								if (customSerialization != null)
								{
									num = 4;
									num2 = num;
									continue;
								}
								throw new ArgumentException(string.Format("Type {0} does not implement ICustomSerialization.", type.Name));
							case 0:
								obj = Activator.CreateInstance(ObjectType);
								num = 2;
								num2 = num;
								continue;
							case 3:
							case 7:
								num = 9;
								num2 = num;
								continue;
							case 9:
								if (!(ObjectType != null))
								{
									obj = Activator.CreateInstance(type);
									num = 1;
									num2 = num;
								}
								else
								{
									num = 0;
									num2 = num;
								}
								continue;
							}
							break;
						}
						goto IL_007e;
						IL_007e:
						memberInfoProperty = null;
						memberInfoField = null;
						type = null;
						obj = null;
						defaultValue = null;
						value = null;
						num = 8;
						num2 = num;
						goto IL_0047;
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public abstract class SerializableAttributeBase : Attribute
			{
				protected enum MemberType
				{
					Property,
					Field
				}

				[CompilerGenerated]
				private string a;

				public string ValueName
				{
					[CompilerGenerated]
					get
					{
						short num = 7448;
						short num2 = num;
						num = 7448;
						switch (num2 == num)
						{
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 1;
							if (num != 0)
							{
							}
							num = 0;
							_ = num;
							return a;
						}
					}
					[CompilerGenerated]
					set
					{
						short num = 1;
						if (num != 0)
						{
						}
						num = 28621;
						short num2 = num;
						num = 28621;
						switch (num2 == num)
						{
						default:
							num = 0;
							_ = num;
							break;
						case true:
							break;
						}
						num = 0;
						if (num != 0)
						{
						}
						a = value;
					}
				}

				public abstract void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname);

				public abstract void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember);

				protected MemberType CheckAndCastMemberInfo(MemberInfo memberInfo, ref PropertyInfo memberInfoProperty, ref FieldInfo memberInfoField)
				{
					int a_ = 13;
					string a_2 = "Q783O798VIA1F9ICOC6W2TTY865POOM5FHUDJD7MFTNDMF75ZHCJP9Q9P";
					int num = default(int);
					MemberType result = default(MemberType);
					switch (0)
					{
					default:
						while (true)
						{
							short num2;
							switch (num)
							{
							case 2:
								if (!(memberInfo is PropertyInfo))
								{
									num2 = 0;
									num = num2;
									continue;
								}
								goto IL_009a;
							case 0:
								if (memberInfo is FieldInfo)
								{
									num2 = 4;
									num = num2;
									continue;
								}
								throw new ArgumentException(string.Format("Member {0}.{1} is not a supported type for use with {2}", memberInfo.GetType().Name, memberInfo.Name, GetType().Name));
							case 4:
							{
								num2 = -18753;
								short num3 = num2;
								num2 = -18753;
								switch (num3 == num2)
								{
								case false:
								case true:
									break;
								default:
									num2 = 0;
									if (num2 != 0)
									{
									}
									memberInfoField = (FieldInfo)memberInfo;
									num2 = 3;
									num = num2;
									continue;
								}
								goto IL_009a;
							}
							case 1:
								result = MemberType.Property;
								memberInfoProperty = (PropertyInfo)memberInfo;
								num2 = 5;
								num = num2;
								continue;
							case 3:
							case 5:
								{
									num2 = 1;
									if (num2 != 0)
									{
									}
									return result;
								}
								IL_009a:
								num2 = 1;
								num = num2;
								continue;
							}
							break;
						}
						goto case 0;
					case 0:
					{
						result = MemberType.Field;
						short num2 = 0;
						_ = num2;
						num2 = 2;
						num = num2;
						goto default;
					}
					}
				}

				protected void SetObjectMemberValue(object targetObject, MemberInfo targetMember, object valueToStore)
				{
					FieldInfo memberInfoField;
					while (true)
					{
						PropertyInfo memberInfoProperty = null;
						memberInfoField = null;
						if (CheckAndCastMemberInfo(targetMember, ref memberInfoProperty, ref memberInfoField) != 0)
						{
							break;
						}
						short num = -30236;
						short num2 = num;
						num = -30236;
						switch (num2 == num)
						{
						case false:
						case true:
							continue;
						}
						num = 0;
						_ = num;
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						if (num != 0)
						{
						}
						memberInfoProperty.SetValue(targetObject, valueToStore, null);
						return;
					}
					memberInfoField.SetValue(targetObject, valueToStore);
				}

				protected T GetValueFromObject<T>(object sourceObject, MemberInfo sourceMember)
				{
					int num = default(int);
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					FieldInfo memberInfoField = default(FieldInfo);
					T result = default(T);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num)
							{
							case 3:
								if (CheckAndCastMemberInfo(sourceMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									short num2 = 0;
									num = num2;
								}
								else
								{
									result = (T)memberInfoField.GetValue(sourceObject);
									short num2 = 2;
									num = num2;
								}
								continue;
							case 2:
							{
								short num2 = 12280;
								short num3 = num2;
								num2 = 12280;
								switch (num3 == num2)
								{
								default:
									num2 = 0;
									if (num2 == 0)
									{
									}
									break;
								case false:
								case true:
									break;
								}
								goto case 1;
							}
							case 0:
							{
								short num2 = 0;
								_ = num2;
								result = (T)memberInfoProperty.GetValue(sourceObject, null);
								num2 = 1;
								num = num2;
								continue;
							}
							case 1:
							{
								short num2 = 1;
								if (num2 != 0)
								{
								}
								return result;
							}
							}
							break;
						}
						goto case 0;
					case 0:
					{
						memberInfoProperty = null;
						memberInfoField = null;
						result = default(T);
						short num2 = 3;
						num = num2;
						goto default;
					}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberStringAttribute : SerializableAttributeBase
			{
				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					short num = 0;
					int num2 = num;
					switch (num2)
					{
					default:
					{
						bool flag = default(bool);
						string valueFromObject = default(string);
						string valueName = default(string);
						switch (0)
						{
						default:
						{
							string valueFromObject2 = default(string);
							while (true)
							{
								switch (num2)
								{
								case 2:
									if (string.IsNullOrEmpty(base.ValueName))
									{
										num = 5;
										num2 = num;
										continue;
									}
									goto IL_02a2;
								case 7:
									num = 9;
									num2 = num;
									continue;
								case 9:
									if (flag)
									{
										num = 8;
										num2 = num;
										continue;
									}
									return;
								case 0:
									valueFromObject2 = GetValueFromObject<string>(defaultValues, sourceMember);
									num = 6;
									num2 = num;
									continue;
								case 6:
									if (string.Equals(valueFromObject, valueFromObject2))
									{
										num = 1;
										num2 = num;
										continue;
									}
									goto case 7;
								case 5:
									num = 0;
									_ = num;
									valueName = sourceMember.Name;
									num = 4;
									num2 = num;
									continue;
								case 4:
								{
									num = -16075;
									short num3 = num;
									num = -16075;
									switch (num3 == num)
									{
									default:
										num = 0;
										if (num == 0)
										{
										}
										break;
									case false:
									case true:
										break;
									}
									goto IL_02a2;
								}
								case 8:
									targetKey.SetValueAnyString(valueName, valueFromObject);
									num = 3;
									num2 = num;
									continue;
								case 3:
									return;
								case 10:
									if (defaultValues != null)
									{
										num = 0;
										num2 = num;
										continue;
									}
									goto case 7;
								case 1:
									{
										flag = false;
										num = 7;
										num2 = num;
										continue;
									}
									IL_02a2:
									num = 10;
									num2 = num;
									continue;
								}
								break;
							}
							goto case 0;
						}
						case 0:
							num = 1;
							if (num != 0)
							{
							}
							valueName = base.ValueName;
							valueFromObject = GetValueFromObject<string>(sourceObject, sourceMember);
							flag = true;
							num = 2;
							num2 = num;
							goto default;
						}
					}
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					int num = default(int);
					string valueToStore = default(string);
					string valueName = default(string);
					bool valueExists = default(bool);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num)
							{
							case 5:
							{
								short num2 = 0;
								_ = num2;
								if (string.IsNullOrEmpty(base.ValueName))
								{
									num2 = 4;
									num = num2;
									continue;
								}
								goto case 3;
							}
							case 1:
								return;
							case 3:
							{
								valueToStore = sourceKey.SafeReadString(valueName, out valueExists);
								short num2 = 2;
								num = num2;
								continue;
							}
							case 2:
								if (valueExists)
								{
									short num2 = 0;
									num = num2;
									continue;
								}
								return;
							case 0:
							{
								short num2 = 1;
								if (num2 != 0)
								{
								}
								SetObjectMemberValue(targetObject, targetMember, valueToStore);
								num2 = 1;
								num = num2;
								continue;
							}
							case 4:
							{
								valueName = targetMember.Name;
								short num2 = 3;
								num = num2;
								continue;
							}
							}
							break;
						}
						goto case 0;
					case 0:
					{
						valueName = base.ValueName;
						valueToStore = null;
						valueExists = false;
						short num2 = 855;
						short num3 = num2;
						num2 = 855;
						switch (num3 == num2)
						{
						case false:
						case true:
							return;
						}
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 5;
						num = num2;
						goto default;
					}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberDwordAttribute : SerializableAttributeBase
			{
				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					short num = 0;
					int num2 = num;
					string name = default(string);
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					FieldInfo memberInfoField = default(FieldInfo);
					int num3 = default(int);
					int num4 = default(int);
					switch (num2)
					{
					default:
						{
							num = 1;
							if (num != 0)
							{
							}
							switch (0)
							{
							case 0:
								goto IL_00b6;
							}
							goto IL_006b;
						}
						IL_00b6:
						name = base.ValueName;
						memberInfoProperty = null;
						memberInfoField = null;
						num3 = 0;
						num4 = 0;
						num = 14;
						num2 = num;
						goto IL_006b;
						IL_006b:
						while (true)
						{
							switch (num2)
							{
							case 14:
								if (string.IsNullOrEmpty(base.ValueName))
								{
									num = 7;
									num2 = num;
									continue;
								}
								goto case 3;
							case 4:
							case 8:
								num = 12;
								num2 = num;
								continue;
							case 12:
								if (defaultValues != null)
								{
									num = 1;
									num2 = num;
									continue;
								}
								targetKey.SetValue(name, num3, RegistryValueKind.DWord);
								return;
							case 13:
								num4 = memberInfoProperty.GetValueAsInt(defaultValues);
								num = 8;
								num2 = num;
								continue;
							case 9:
								num3 = memberInfoProperty.GetValueAsInt(sourceObject);
								num = 0;
								num2 = num;
								continue;
							case 16:
								targetKey.SetValue(name, num3, RegistryValueKind.DWord);
								return;
							case 3:
								num = 6;
								num2 = num;
								continue;
							case 6:
								if (CheckAndCastMemberInfo(sourceMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									num = 9;
									num2 = num;
									continue;
								}
								num = 0;
								_ = num;
								num3 = memberInfoField.GetValueAsInt(sourceObject);
								num = 5;
								num2 = num;
								continue;
							case 0:
							case 5:
								num = 15;
								num2 = num;
								continue;
							case 15:
							{
								if (defaultValues == null)
								{
									goto case 4;
								}
								num = 25250;
								short num5 = num;
								num = 25250;
								switch (num5 == num)
								{
								default:
									num = 0;
									if (num != 0)
									{
									}
									num = 10;
									num2 = num;
									continue;
								case false:
								case true:
									break;
								}
								goto case 2;
							}
							case 7:
								name = sourceMember.Name;
								num = 3;
								num2 = num;
								continue;
							case 1:
								num = 11;
								num2 = num;
								continue;
							case 11:
								if (num3 == num4)
								{
									targetKey.DeleteValue(name, throwOnMissingValue: false);
									return;
								}
								num = 16;
								num2 = num;
								continue;
							case 10:
								num = 2;
								num2 = num;
								continue;
							case 2:
								if (!(memberInfoProperty != null))
								{
									num4 = memberInfoField.GetValueAsInt(defaultValues);
									num = 4;
									num2 = num;
								}
								else
								{
									num = 13;
									num2 = num;
								}
								continue;
							}
							break;
						}
						goto IL_00b6;
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					short num = 0;
					int num2 = num;
					switch (num2)
					{
					default:
					{
						PropertyInfo memberInfoProperty = default(PropertyInfo);
						string valueName = default(string);
						bool valueExists = default(bool);
						FieldInfo memberInfoField = default(FieldInfo);
						switch (0)
						{
						default:
						{
							int newValue = default(int);
							while (true)
							{
								num = 2130;
								short num3 = num;
								num = 2130;
								switch (num3 == num)
								{
								default:
									num = 0;
									if (num != 0)
									{
									}
									switch (num2)
									{
									case 0:
										goto IL_00df;
									case 3:
										memberInfoProperty.SetValueFromInt(targetObject, newValue);
										return;
									case 1:
										goto IL_0122;
									case 4:
										goto IL_0140;
									case 5:
										goto IL_01b1;
									case 6:
										num = 1;
										if (num == 0)
										{
										}
										return;
									case 7:
										goto IL_024a;
									case 2:
										goto IL_0279;
									}
									break;
								case false:
								case true:
									goto IL_0140;
									IL_024a:
									newValue = (int)sourceKey.SafeReadDword(valueName, out valueExists);
									num = 2;
									num2 = num;
									continue;
									IL_0279:
									if (valueExists)
									{
										num = 1;
										num2 = num;
										continue;
									}
									return;
									IL_0122:
									num = 4;
									num2 = num;
									continue;
									IL_01b1:
									valueName = targetMember.Name;
									num = 7;
									num2 = num;
									continue;
									IL_0140:
									if (CheckAndCastMemberInfo(targetMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
									{
										num = 0;
										_ = num;
										num = 3;
										num2 = num;
									}
									else
									{
										memberInfoField.SetValueFromInt(targetObject, newValue);
										num = 6;
										num2 = num;
									}
									continue;
									IL_00df:
									if (string.IsNullOrEmpty(base.ValueName))
									{
										num = 5;
										num2 = num;
										continue;
									}
									goto IL_024a;
								}
								break;
							}
							goto case 0;
						}
						case 0:
							valueName = base.ValueName;
							memberInfoProperty = null;
							memberInfoField = null;
							valueExists = false;
							num = 0;
							num2 = num;
							goto default;
						}
					}
					}
				}

				public static bool IsSupportedType(Type typeToCheck)
				{
					short num = 5;
					int num2 = num;
					while (true)
					{
						switch (num2)
						{
						case 5:
							switch (0)
							{
							default:
								goto end_IL_002b;
							case 0:
								break;
							}
							goto default;
						default:
							if (!typeToCheck.Equals(typeof(ulong)))
							{
								num = 9;
								num2 = num;
								break;
							}
							goto IL_0203;
						case 12:
							num = 2;
							num2 = num;
							break;
						case 2:
							if (!typeToCheck.Equals(typeof(byte)))
							{
								num = 3;
								num2 = num;
								break;
							}
							goto IL_0203;
						case 6:
							num = 4;
							num2 = num;
							break;
						case 4:
							if (!typeToCheck.Equals(typeof(short)))
							{
								num = -4602;
								short num3 = num;
								num = -4602;
								switch (num3 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 0;
									if (num != 0)
									{
									}
									num = 12;
									num2 = num;
									break;
								}
								break;
							}
							goto IL_0203;
						case 1:
							num = 15;
							num2 = num;
							break;
						case 15:
							if (!typeToCheck.Equals(typeof(uint)))
							{
								num = 13;
								num2 = num;
								break;
							}
							goto IL_0203;
						case 9:
							num = 8;
							num2 = num;
							break;
						case 8:
							if (!typeToCheck.Equals(typeof(long)))
							{
								num = 1;
								num2 = num;
								break;
							}
							goto IL_0203;
						case 3:
							num = 0;
							num2 = num;
							break;
						case 0:
							if (typeToCheck.Equals(typeof(sbyte)))
							{
								num = 7;
								num2 = num;
								break;
							}
							return false;
						case 7:
							num = 1;
							if (num == 0)
							{
							}
							goto IL_0203;
						case 10:
							num = 14;
							num2 = num;
							break;
						case 14:
							if (!typeToCheck.Equals(typeof(ushort)))
							{
								num = 6;
								num2 = num;
								break;
							}
							goto IL_0203;
						case 13:
							num = 11;
							num2 = num;
							break;
						case 11:
							{
								if (!typeToCheck.Equals(typeof(int)))
								{
									num = 0;
									_ = num;
									num = 10;
									num2 = num;
									break;
								}
								goto IL_0203;
							}
							IL_0203:
							return true;
							end_IL_002b:
							break;
						}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberBoolAttribute : SerializableAttributeBase
			{
				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					bool flag = default(bool);
					bool flag2 = default(bool);
					FieldInfo memberInfoField = default(FieldInfo);
					string valueName = default(string);
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					while (true)
					{
						short num = 0;
						int num2 = num;
						switch (num2)
						{
						}
						switch (0)
						{
						default:
							while (true)
							{
								switch (num2)
								{
								case 4:
									if (string.IsNullOrEmpty(base.ValueName))
									{
										num = 2;
										num2 = num;
										continue;
									}
									goto case 3;
								case 15:
									num = 9;
									num2 = num;
									continue;
								case 9:
									if (flag != flag2)
									{
										num = 5;
										num2 = num;
										continue;
									}
									return;
								case 7:
									flag2 = (bool)memberInfoField.GetValue(defaultValues);
									num = 6;
									num2 = num;
									continue;
								case 3:
									num = 8;
									num2 = num;
									continue;
								case 8:
									goto IL_0189;
								case 5:
									targetKey.SetBool(valueName, flag, forceWrite: true);
									return;
								case 2:
									valueName = sourceMember.Name;
									num = 3;
									num2 = num;
									continue;
								case 1:
								case 6:
									num = 11;
									num2 = num;
									continue;
								case 11:
									goto IL_0216;
								case 0:
									return;
								case 14:
									flag2 = (bool)memberInfoProperty.GetValue(defaultValues, null);
									num = 1;
									num2 = num;
									continue;
								case 10:
									flag = (bool)memberInfoProperty.GetValue(sourceObject, null);
									num = 1;
									if (num != 0)
									{
									}
									num = 13;
									num2 = num;
									continue;
								case 13:
									num = 0;
									_ = num;
									if (defaultValues != null)
									{
										num = 14;
										num2 = num;
										continue;
									}
									goto case 1;
								case 12:
									if (defaultValues != null)
									{
										num = 7;
										num2 = num;
										continue;
									}
									goto case 1;
								}
								break;
								IL_0216:
								num = -18974;
								short num3 = num;
								num = -18974;
								switch (num3 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 0;
									if (num != 0)
									{
									}
									if (defaultValues != null)
									{
										num = 15;
										num2 = num;
									}
									else
									{
										targetKey.SetBool(valueName, flag, forceWrite: true);
										num = 0;
										num2 = num;
									}
									continue;
								}
								goto end_IL_0024;
								IL_0189:
								if (CheckAndCastMemberInfo(sourceMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									num = 10;
									num2 = num;
								}
								else
								{
									flag = (bool)memberInfoField.GetValue(sourceObject);
									num = 12;
									num2 = num;
								}
							}
							goto case 0;
						case 0:
							{
								valueName = base.ValueName;
								memberInfoProperty = null;
								memberInfoField = null;
								flag = false;
								flag2 = false;
								num = 4;
								num2 = num;
								goto default;
							}
							end_IL_0024:
							break;
						}
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					int num = default(int);
					bool flag = default(bool);
					string valueName = default(string);
					bool valueExists = default(bool);
					switch (0)
					{
					default:
						while (true)
						{
							short num2;
							switch (num)
							{
							case 2:
								if (string.IsNullOrEmpty(base.ValueName))
								{
									num2 = 0;
									_ = num2;
									num2 = 3;
									num = num2;
									continue;
								}
								goto case 4;
							case 0:
								SetObjectMemberValue(targetObject, targetMember, flag);
								num2 = 5;
								num = num2;
								continue;
							case 5:
								return;
							case 4:
								flag = sourceKey.SafeReadBoolean(valueName, out valueExists);
								goto IL_00ff;
							case 1:
								num2 = 1;
								if (num2 != 0)
								{
								}
								if (valueExists)
								{
									num2 = 0;
									num = num2;
									continue;
								}
								return;
							case 3:
								{
									valueName = targetMember.Name;
									num2 = -2728;
									short num3 = num2;
									num2 = -2728;
									switch (num3 == num2)
									{
									case false:
									case true:
										break;
									default:
										num2 = 0;
										if (num2 != 0)
										{
										}
										num2 = 4;
										num = num2;
										continue;
									}
									goto IL_00ff;
								}
								IL_00ff:
								num2 = 1;
								num = num2;
								continue;
							}
							break;
						}
						goto case 0;
					case 0:
					{
						valueName = base.ValueName;
						flag = false;
						valueExists = false;
						short num2 = 2;
						num = num2;
						goto default;
					}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberUriAttribute : SerializableAttributeBase
			{
				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					int num = default(int);
					string valueName = default(string);
					Uri valueFromObject = default(Uri);
					switch (0)
					{
					default:
						while (true)
						{
							short num2;
							switch (num)
							{
							case 0:
								if (string.IsNullOrEmpty(base.ValueName))
								{
									goto IL_0097;
								}
								goto case 2;
							case 1:
							{
								num2 = 29338;
								short num3 = num2;
								num2 = 29338;
								switch (num3 == num2)
								{
								case false:
								case true:
									break;
								default:
									num2 = 0;
									if (num2 != 0)
									{
									}
									valueName = sourceMember.Name;
									num2 = 0;
									_ = num2;
									num2 = 2;
									num = num2;
									continue;
								}
								goto IL_0097;
							}
							case 2:
								{
									targetKey.SetValueUri(valueName, valueFromObject);
									return;
								}
								IL_0097:
								num2 = 1;
								num = num2;
								continue;
							}
							break;
						}
						goto case 0;
					case 0:
					{
						valueName = base.ValueName;
						valueFromObject = GetValueFromObject<Uri>(sourceObject, sourceMember);
						short num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 0;
						num = num2;
						goto default;
					}
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					int num = default(int);
					Uri valueToStore = default(Uri);
					string valueName = default(string);
					bool valueExists = default(bool);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num)
							{
							case 1:
								if (string.IsNullOrEmpty(base.ValueName))
								{
									short num2 = 0;
									num = num2;
									continue;
								}
								goto case 3;
							case 2:
							{
								short num2;
								while (true)
								{
									SetObjectMemberValue(targetObject, targetMember, valueToStore);
									num2 = 17953;
									short num3 = num2;
									num2 = 17953;
									switch (num3 == num2)
									{
									case false:
									case true:
										continue;
									}
									break;
								}
								num2 = 0;
								if (num2 != 0)
								{
								}
								num2 = 1;
								if (num2 != 0)
								{
								}
								num2 = 5;
								num = num2;
								continue;
							}
							case 5:
							{
								short num2 = 0;
								_ = num2;
								return;
							}
							case 3:
							{
								valueToStore = sourceKey.SafeReadUri(valueName, out valueExists);
								short num2 = 4;
								num = num2;
								continue;
							}
							case 4:
								if (valueExists)
								{
									short num2 = 2;
									num = num2;
									continue;
								}
								return;
							case 0:
							{
								valueName = targetMember.Name;
								short num2 = 3;
								num = num2;
								continue;
							}
							}
							break;
						}
						goto case 0;
					case 0:
					{
						valueName = base.ValueName;
						valueToStore = null;
						valueExists = false;
						short num2 = 1;
						num = num2;
						goto default;
					}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
			public class DataMemberTimespanAttribute : SerializableAttributeBase
			{
				public override void Serialize(object sourceObject, MemberInfo sourceMember, object defaultValues, RegistryKey targetKey, string targetKeyPathname)
				{
					short num = 0;
					int num2 = num;
					int num3 = default(int);
					TimeSpan timeSpan2 = default(TimeSpan);
					string name = default(string);
					TimeSpan timeSpan = default(TimeSpan);
					PropertyInfo memberInfoProperty = default(PropertyInfo);
					FieldInfo memberInfoField = default(FieldInfo);
					switch (num2)
					{
					default:
						{
							switch (0)
							{
							case 0:
								goto IL_007e;
							}
							goto IL_0033;
						}
						IL_0033:
						while (true)
						{
							switch (num2)
							{
							case 3:
								if (string.IsNullOrEmpty(base.ValueName))
								{
									num = 10;
									num2 = num;
									continue;
								}
								goto case 6;
							case 0:
							case 16:
								num3 = (int)timeSpan2.TotalSeconds;
								num = 5;
								num2 = num;
								continue;
							case 5:
								if (defaultValues != null)
								{
									num = 4;
									num2 = num;
									continue;
								}
								targetKey.SetValue(name, num3, RegistryValueKind.DWord);
								return;
							case 15:
								timeSpan = (TimeSpan)memberInfoProperty.GetValue(sourceObject, null);
								num = 16;
								num2 = num;
								continue;
							case 8:
								timeSpan2 = (TimeSpan)memberInfoProperty.GetValue(sourceObject, null);
								num = 1;
								num2 = num;
								continue;
							case 6:
								num = 0;
								_ = num;
								num = 7;
								num2 = num;
								continue;
							case 7:
								if (CheckAndCastMemberInfo(sourceMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
								{
									num = 8;
									num2 = num;
								}
								else
								{
									timeSpan2 = (TimeSpan)memberInfoField.GetValue(sourceObject);
									num = 13;
									num2 = num;
								}
								continue;
							case 1:
							case 13:
								num = 14;
								num2 = num;
								continue;
							case 14:
								if (defaultValues != null)
								{
									num = 2;
									num2 = num;
									continue;
								}
								goto case 0;
							case 10:
								name = sourceMember.Name;
								num = 6;
								num2 = num;
								continue;
							case 4:
								num = 12;
								num2 = num;
								continue;
							case 12:
								if (!(timeSpan2 != timeSpan))
								{
									targetKey.DeleteValue(name, throwOnMissingValue: false);
									return;
								}
								num = 11;
								num2 = num;
								continue;
							case 11:
							{
								num = 20506;
								short num4 = num;
								num = 20506;
								switch (num4 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 1;
									if (num != 0)
									{
									}
									num = 0;
									if (num != 0)
									{
									}
									targetKey.SetValue(name, num3, RegistryValueKind.DWord);
									return;
								}
								goto case 10;
							}
							case 2:
								num = 9;
								num2 = num;
								continue;
							case 9:
								if (!(memberInfoProperty != null))
								{
									timeSpan = (TimeSpan)memberInfoField.GetValue(sourceObject);
									num = 0;
									num2 = num;
								}
								else
								{
									num = 15;
									num2 = num;
								}
								continue;
							}
							break;
						}
						goto IL_007e;
						IL_007e:
						name = base.ValueName;
						memberInfoProperty = null;
						memberInfoField = null;
						timeSpan2 = TimeSpan.Zero;
						timeSpan = TimeSpan.Zero;
						num = 3;
						num2 = num;
						goto IL_0033;
					}
				}

				public override void Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object targetObject, MemberInfo targetMember)
				{
					short num = 0;
					int num2 = num;
					switch (num2)
					{
					default:
					{
						PropertyInfo memberInfoProperty = default(PropertyInfo);
						FieldInfo memberInfoField = default(FieldInfo);
						string valueName = default(string);
						bool valueExists = default(bool);
						switch (0)
						{
						default:
						{
							int num3 = default(int);
							while (true)
							{
								switch (num2)
								{
								case 3:
									if (string.IsNullOrEmpty(base.ValueName))
									{
										num = 4;
										num2 = num;
										continue;
									}
									goto case 7;
								case 6:
									memberInfoProperty.SetValue(targetObject, TimeSpan.FromSeconds(num3), null);
									return;
								case 5:
									num = 1;
									num2 = num;
									continue;
								case 1:
									if (CheckAndCastMemberInfo(targetMember, ref memberInfoProperty, ref memberInfoField) == MemberType.Property)
									{
										num = 0;
										_ = num;
										num = 29671;
										short num4 = num;
										num = 29671;
										switch (num4 == num)
										{
										default:
											num = 0;
											if (num != 0)
											{
											}
											num = 1;
											if (num != 0)
											{
											}
											num = 6;
											num2 = num;
											continue;
										case false:
										case true:
											break;
										}
										goto case 4;
									}
									memberInfoField.SetValue(targetObject, TimeSpan.FromSeconds(num3));
									num = 2;
									num2 = num;
									continue;
								case 4:
									valueName = targetMember.Name;
									num = 7;
									num2 = num;
									continue;
								case 2:
									return;
								case 7:
									num3 = (int)sourceKey.SafeReadDword(valueName, out valueExists);
									num = 0;
									num2 = num;
									continue;
								case 0:
									if (valueExists)
									{
										num = 5;
										num2 = num;
										continue;
									}
									return;
								}
								break;
							}
							goto case 0;
						}
						case 0:
							valueName = base.ValueName;
							memberInfoProperty = null;
							memberInfoField = null;
							valueExists = false;
							num = 3;
							num2 = num;
							goto default;
						}
					}
					}
				}
			}

			[AttributeUsage(AttributeTargets.Method)]
			public class OnSerializedAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Method)]
			public class OnDeserializedAttribute : Attribute
			{
			}

			public interface ICustomSerialization
			{
				Exception Serialize(RegistryKey targetKey, string targetKeyPathname, object defaultValue);

				object Deserialize(RegistryKey sourceKey, string sourceKeyPathname, object defaultValue);
			}

			public delegate void OnSerializedHandler(RegistryKey targetKey, string targetKeyPath);

			public delegate void OnDeserializedHandler(RegistryKey targetKey, string targetKeyPath);

			public delegate bool SerializationErrorHandler(Exception errorDescription, MemberInfo targetMember, SerializableAttributeBase targetMemberInfo, string keyPathname, object defaultValue);

			private class a
			{
				public Tuple<MemberInfo, SerializableAttributeBase>[] a;

				public MethodInfo[] b;

				public MethodInfo[] c;
			}

			public enum EnumByte : byte
			{
				A,
				B
			}

			public enum EnumInt
			{
				A,
				B
			}

			public enum EnumUlong : ulong
			{
				A,
				B
			}

			[CompilerGenerated]
			private sealed class b<a> where a : class, new()
			{
				public string a;

				public a b;

				public OnDeserializedHandler c;

				public RegistryKey d;

				internal void e()
				{
					short num = 1;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 30706;
					short num2 = num;
					num = 30706;
					switch (num2 == num)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					c(d, this.a);
				}
			}

			[CompilerGenerated]
			private sealed class c<a> where a : class, new()
			{
				public Tuple<MemberInfo, SerializableAttributeBase> a;

				public b<a> b;

				internal void c()
				{
					short num = 0;
					_ = num;
					num = 23737;
					short num2 = num;
					num = 23737;
					switch (num2 == num)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					this.a.Item2.Deserialize(b.d, b.a, b.b, this.a.Item1);
				}
			}

			[CompilerGenerated]
			private sealed class d<a> where a : class, new()
			{
				public a a;

				public string b;

				public a c;

				public SerializationErrorHandler d;

				internal void e()
				{
					short num = -23843;
					short num2 = num;
					num = -23843;
					switch (num2 == num)
					{
					default:
						num = 0;
						_ = num;
						break;
					case true:
						break;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					this.a = DeserializeObject(b, c, d);
				}
			}

			[CompilerGenerated]
			private sealed class e
			{
				public PropertyInfo a;

				internal bool b(Tuple<MemberInfo, SerializableAttributeBase> A_0)
				{
					short num = 2;
					int num2 = num;
					while (true)
					{
						num = 0;
						_ = num;
						switch (num2)
						{
						case 2:
							num = 1;
							if (num != 0)
							{
							}
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (A_0.Item1.Name.Equals(a.Name))
							{
								num = 0;
								num2 = num;
								continue;
							}
							break;
						case 3:
						{
							num = 10886;
							short num3 = num;
							num = 10886;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								return ((PropertyInfo)A_0.Item1).PropertyType == a.PropertyType;
							}
							goto case 2;
						}
						case 0:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (A_0.Item1 is PropertyInfo)
							{
								num = 3;
								num2 = num;
								continue;
							}
							break;
						}
						break;
					}
					return false;
				}
			}

			public static readonly SerializationErrorHandler loggerErrorHandler;

			private static Dictionary<object, a> m_a;

			public static void SerializeObject(object objectToSerialize, string targetKeyPathname, object defaultValues = null)
			{
				int a_ = 15;
				string a_2 = "U61P3T8FE0WHXZZ21VKL3QO1COUEBJRGYSZ79OY3H175OOLJETHEZMA5JUJR9";
				short num = -13602;
				short num2 = num;
				num = -13602;
				switch (num2 == num)
				{
				case false:
				case true:
					goto IL_046f;
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				switch (num)
				{
				}
				num = 2;
				int num3 = num;
				goto IL_00c9;
				IL_046f:
				Tuple<MemberInfo, SerializableAttributeBase>[] A_ = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
				OnSerializedHandler A_2 = default(OnSerializedHandler);
				RegistryKey registryKey = default(RegistryKey);
				if (targetKeyPathname != null)
				{
					a(objectToSerialize, out A_, out A_2, out var _);
					registryKey = OpenRegistryKey(targetKeyPathname, openForWrite: true);
					num = 4;
					num3 = num;
				}
				else
				{
					num = 6;
					num3 = num;
				}
				goto IL_00c9;
				IL_00c9:
				int num4 = default(int);
				Tuple<MemberInfo, SerializableAttributeBase>[] array = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
				while (true)
				{
					switch (num3)
					{
					case 2:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (objectToSerialize == null)
						{
							num = 5;
							num3 = num;
						}
						else
						{
							num = 1;
							num3 = num;
						}
						continue;
					case 5:
						num = 1;
						if (num != 0)
						{
						}
						throw new ArgumentNullException("T");
					case 6:
						throw new ArgumentNullException("targetKeyPathname");
					case 1:
						num = 0;
						_ = num;
						if (objectToSerialize is ExpandoObject)
						{
							num = 0;
							num3 = num;
						}
						else
						{
							num = 3;
							num3 = num;
						}
						continue;
					case 4:
						try
						{
							switch (0)
							{
							default:
								while (true)
								{
									switch (num3)
									{
									case 5:
										A_2(registryKey, targetKeyPathname);
										num = 6;
										num3 = num;
										continue;
									case 2:
										num = 4;
										num3 = num;
										continue;
									case 4:
										if (A_2 != null)
										{
											num = 5;
											num3 = num;
											continue;
										}
										goto case 6;
									case 1:
									case 3:
										num = 0;
										num3 = num;
										continue;
									case 0:
										if (num4 < array.Length)
										{
											Tuple<MemberInfo, SerializableAttributeBase> tuple = array[num4];
											tuple.Item2.Serialize(objectToSerialize, tuple.Item1, defaultValues, registryKey, targetKeyPathname);
											num4++;
											num = 3;
											num3 = num;
										}
										else
										{
											num = 2;
											num3 = num;
										}
										continue;
									case 6:
										num = 7;
										num3 = num;
										continue;
									case 7:
										return;
									}
									break;
								}
								goto case 0;
							case 0:
								array = A_;
								num4 = 0;
								num = 1;
								num3 = num;
								goto default;
							}
						}
						finally
						{
							num = 1;
							num3 = num;
							while (true)
							{
								switch (num3)
								{
								case 1:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (registryKey != null)
									{
										num = 0;
										num3 = num;
										continue;
									}
									break;
								case 0:
									((IDisposable)registryKey).Dispose();
									num = 2;
									num3 = num;
									continue;
								case 2:
									break;
								}
								break;
							}
						}
					case 3:
						break;
					case 0:
						throw new ArgumentException("Dynamic objects are not supported", "T");
					}
					break;
				}
				goto IL_046f;
			}

			public static T DeserializeObject<T>(string sourceKeyPath, T defaultValues = null, SerializationErrorHandler errorHandler = null) where T : class, new()
			{
				int a_ = 6;
				string a_2 = "XXAI363RCVNDIOR9QMKTZVXDD3A0GGYXWEMFJO3M6T448R2UR";
				short num = 0;
				int num2 = num;
				b<T> b = default(b<T>);
				Tuple<MemberInfo, SerializableAttributeBase>[] A_ = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
				Exception ex = default(Exception);
				object defaultValue = default(object);
				c<T> c = default(c<T>);
				Tuple<MemberInfo, SerializableAttributeBase>[] array = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
				int num3 = default(int);
				switch (num2)
				{
				default:
					{
						switch (0)
						{
						case 0:
							goto IL_006a;
						}
						goto IL_003f;
					}
					IL_003f:
					while (true)
					{
						switch (num2)
						{
						case 5:
							if (string.IsNullOrEmpty(b.a))
							{
								num = 8;
								num2 = num;
							}
							else
							{
								num = 4;
								num2 = num;
							}
							continue;
						case 8:
						{
							num = -27496;
							short num4 = num;
							num = -27496;
							switch (num4 == num)
							{
							case false:
							case true:
								continue;
							}
							num = 0;
							if (num != 0)
							{
							}
							throw new ArgumentNullException("sourceKeyPath");
						}
						case 6:
							num = 7;
							num2 = num;
							continue;
						case 7:
							if (!(b.b is ExpandoObject))
							{
								a(b.b, out A_, out var _, out b.c);
								b.d = OpenRegistryKey(b.a, openForWrite: false);
								num = 3;
								num2 = num;
							}
							else
							{
								num = 0;
								num2 = num;
							}
							continue;
						case 4:
						{
							if (defaultValues == null)
							{
								num = 1;
								num2 = num;
								continue;
							}
							ICloneable cloneable = (ICloneable)defaultValues;
							b.b = (T)cloneable.Clone();
							num = 1;
							if (num != 0)
							{
							}
							num = 2;
							num2 = num;
							continue;
						}
						case 3:
							try
							{
								num = 28;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 28:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (b.d != null)
										{
											num = 16;
											num2 = num;
											continue;
										}
										break;
									case 27:
										num = 0;
										num2 = num;
										continue;
									case 0:
										if (errorHandler != null)
										{
											num = 22;
											num2 = num;
											continue;
										}
										break;
									case 21:
										RethrowExceptionSameStackTrace(ex);
										num = 18;
										num2 = num;
										continue;
									case 11:
										defaultValue = ((FieldInfo)c.a.Item1).GetValue(c.b.b);
										num = 7;
										num2 = num;
										continue;
									case 16:
										array = A_;
										num3 = 0;
										num = 8;
										num2 = num;
										continue;
									case 15:
										num = 26;
										num2 = num;
										continue;
									case 26:
										if (c.a.Item1 is PropertyInfo)
										{
											num = 1;
											num2 = num;
										}
										else
										{
											num = 24;
											num2 = num;
										}
										continue;
									case 23:
										num = 9;
										num2 = num;
										continue;
									case 9:
										if (errorHandler != null)
										{
											num = 15;
											num2 = num;
											continue;
										}
										goto case 4;
									case 24:
										if (c.a.Item1 is FieldInfo)
										{
											num = 11;
											num2 = num;
											continue;
										}
										goto case 7;
									case 22:
										num = 20;
										num2 = num;
										continue;
									case 20:
										if (!errorHandler(ex, null, null, b.a, null))
										{
											num = 21;
											num2 = num;
											continue;
										}
										break;
									case 3:
										num = 2;
										num2 = num;
										continue;
									case 2:
										if (b.c != null)
										{
											num = 25;
											num2 = num;
											continue;
										}
										break;
									case 7:
									case 10:
										num = 5;
										num2 = num;
										continue;
									case 5:
										if (!errorHandler(ex, c.a.Item1, c.a.Item2, c.b.a, defaultValue))
										{
											num = 6;
											num2 = num;
											continue;
										}
										goto case 4;
									case 6:
										RethrowExceptionSameStackTrace(ex);
										num = 4;
										num2 = num;
										continue;
									case 8:
									case 12:
										num = 17;
										num2 = num;
										continue;
									case 17:
										if (num3 >= array.Length)
										{
											num = 3;
											num2 = num;
											continue;
										}
										c = new c<T>();
										c.b = b;
										c.a = array[num3];
										ex = DoNothrow(c.c);
										num = 14;
										num2 = num;
										continue;
									case 1:
										defaultValue = ((PropertyInfo)c.a.Item1).GetValue(c.b.b, null);
										num = 10;
										num2 = num;
										continue;
									case 14:
										if (ex != null)
										{
											num = 23;
											num2 = num;
											continue;
										}
										goto case 4;
									case 25:
										ex = DoNothrow(b.e);
										num = 13;
										num2 = num;
										continue;
									case 13:
										if (ex != null)
										{
											num = 27;
											num2 = num;
											continue;
										}
										break;
									case 4:
										num3++;
										num = 12;
										num2 = num;
										continue;
									case 18:
										break;
									case 19:
										goto end_IL_0258;
									}
									num = 19;
									num2 = num;
									continue;
									end_IL_0258:
									break;
								}
							}
							finally
							{
								num = 1;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 1:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (b.d != null)
										{
											num = 0;
											num2 = num;
											continue;
										}
										break;
									case 0:
										((IDisposable)b.d).Dispose();
										num = 2;
										num2 = num;
										continue;
									case 2:
										break;
									}
									break;
								}
							}
							return b.b;
						case 0:
							throw new ArgumentException("Dynamic objects are not supported", "T");
						case 2:
							num = 0;
							_ = num;
							goto case 6;
						case 1:
							b.b = new T();
							num = 6;
							num2 = num;
							continue;
						}
						break;
					}
					goto IL_006a;
					IL_006a:
					b = new b<T>();
					b.a = sourceKeyPath;
					defaultValue = null;
					num = 5;
					num2 = num;
					goto IL_003f;
				}
			}

			public static Exception DeserializeObjectNoThrow<T>(string sourceKeyPath, out T deserializedObject, T defaultValues = null, SerializationErrorHandler errorHandler = null) where T : class, new()
			{
				short num = -15428;
				short num2 = num;
				num = -15428;
				switch (num2 == num)
				{
				default:
				{
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 0;
					if (num != 0)
					{
					}
					T a = null;
					_ = NotYetInitialized;
					deserializedObject = null;
					Exception result = DoNothrow(delegate
					{
						short num3 = -23843;
						short num4 = num3;
						num3 = -23843;
						switch (num4 == num3)
						{
						default:
							num3 = 0;
							_ = num3;
							break;
						case true:
							break;
						}
						num3 = 1;
						if (num3 != 0)
						{
						}
						num3 = 0;
						if (num3 != 0)
						{
						}
						a = DeserializeObject(sourceKeyPath, defaultValues, errorHandler);
					});
					deserializedObject = a;
					return result;
				}
				}
			}

			private static void a(object A_0, out Tuple<MemberInfo, SerializableAttributeBase>[] A_1, out OnSerializedHandler A_2, out OnDeserializedHandler A_3)
			{
				short num = 0;
				int num2 = num;
				switch (num2)
				{
				default:
				{
					OnSerializedHandler onSerializedHandler = default(OnSerializedHandler);
					List<Tuple<MemberInfo, SerializableAttributeBase>> list2 = default(List<Tuple<MemberInfo, SerializableAttributeBase>>);
					Type type = default(Type);
					OnDeserializedHandler onDeserializedHandler = default(OnDeserializedHandler);
					BindingFlags bindingAttr = default(BindingFlags);
					List<MethodInfo> list3 = default(List<MethodInfo>);
					a a2 = default(a);
					List<MethodInfo> list = default(List<MethodInfo>);
					switch (0)
					{
					default:
					{
						int num3 = default(int);
						MethodInfo[] array = default(MethodInfo[]);
						MethodInfo methodInfo = default(MethodInfo);
						PropertyInfo[] properties = default(PropertyInfo[]);
						e e = default(e);
						SerializableAttributeBase serializableAttributeBase = default(SerializableAttributeBase);
						a a = default(a);
						FieldInfo[] fields = default(FieldInfo[]);
						FieldInfo fieldInfo = default(FieldInfo);
						while (true)
						{
							switch (num2)
							{
							case 43:
								if (!Registry.m_a.ContainsKey(A_0.GetType()))
								{
									num = 21;
									num2 = num;
									continue;
								}
								goto case 25;
							case 8:
								if (A_2 == null)
								{
									num = 10;
									num2 = num;
								}
								else
								{
									Delegate.Combine(onSerializedHandler, A_2);
									num = 20;
									num2 = num;
								}
								continue;
							case 9:
							case 30:
								num = 38;
								num2 = num;
								continue;
							case 38:
							{
								num = -5320;
								short num4 = num;
								num = -5320;
								switch (num4 == num)
								{
								default:
									num = 0;
									if (num != 0)
									{
									}
									if (num3 >= array.Length)
									{
										num = 23;
										num2 = num;
									}
									else
									{
										methodInfo = array[num3];
										num = 28;
										num2 = num;
									}
									continue;
								case false:
								case true:
									break;
								}
								goto IL_0443;
							}
							case 35:
							case 48:
								num = 13;
								num2 = num;
								continue;
							case 13:
								if (num3 >= properties.Length)
								{
									num = 27;
									num2 = num;
									continue;
								}
								e = new e();
								e.a = properties[num3];
								serializableAttributeBase = (SerializableAttributeBase)Attribute.GetCustomAttribute(e.a, typeof(SerializableAttributeBase));
								num = 2;
								num2 = num;
								continue;
							case 15:
								A_3 = null;
								array = a.c;
								num3 = 0;
								num = 11;
								num2 = num;
								continue;
							case 7:
								list2.Add(new Tuple<MemberInfo, SerializableAttributeBase>(e.a, serializableAttributeBase));
								num = 26;
								num2 = num;
								continue;
							case 25:
								a = Registry.m_a[A_0.GetType()];
								A_1 = a.a;
								A_2 = null;
								array = a.b;
								num3 = 0;
								num = 19;
								num2 = num;
								continue;
							case 23:
								type = type.BaseType;
								num = 32;
								num2 = num;
								continue;
							case 0:
								if (serializableAttributeBase != null)
								{
									goto IL_0443;
								}
								goto case 36;
							case 31:
								if (A_3 != null)
								{
									Delegate.Combine(onSerializedHandler, A_3);
									num = 39;
									num2 = num;
								}
								else
								{
									num = 37;
									num2 = num;
								}
								continue;
							case 11:
							case 44:
								num = 24;
								num2 = num;
								continue;
							case 24:
								if (num3 < array.Length)
								{
									MethodInfo method2 = array[num3];
									onDeserializedHandler = (OnDeserializedHandler)Delegate.CreateDelegate(typeof(OnDeserializedHandler), A_0, method2);
									num = 31;
									num2 = num;
								}
								else
								{
									num = 3;
									num2 = num;
								}
								continue;
							case 3:
								return;
							case 28:
								if (Attribute.GetCustomAttribute(methodInfo, typeof(OnSerializedAttribute)) != null)
								{
									num = 4;
									num2 = num;
									continue;
								}
								goto case 16;
							case 21:
								type = A_0.GetType();
								num = 14;
								num2 = num;
								continue;
							case 27:
								fields = type.GetFields(bindingAttr);
								num3 = 0;
								num = 5;
								num2 = num;
								continue;
							case 26:
								num3++;
								num = 35;
								num2 = num;
								continue;
							case 36:
								num = 0;
								_ = num;
								num3++;
								num = 41;
								num2 = num;
								continue;
							case 37:
								A_3 = onDeserializedHandler;
								num = 34;
								num2 = num;
								continue;
							case 4:
								list3.Add(methodInfo);
								num = 16;
								num2 = num;
								continue;
							case 29:
								array = A_0.GetType().GetMethods(bindingAttr);
								num3 = 0;
								num = 9;
								num2 = num;
								continue;
							case 17:
								a2.a = list2.ToArray();
								a2.b = list3.ToArray();
								a2.c = list.ToArray();
								Registry.m_a[A_0.GetType()] = a2;
								num = 25;
								num2 = num;
								continue;
							case 5:
							case 41:
								num = 6;
								num2 = num;
								continue;
							case 6:
								if (num3 < fields.Length)
								{
									fieldInfo = fields[num3];
									serializableAttributeBase = (SerializableAttributeBase)Attribute.GetCustomAttribute(fieldInfo, typeof(SerializableAttributeBase));
									num = 0;
									num2 = num;
								}
								else
								{
									num = 29;
									num2 = num;
								}
								continue;
							case 40:
								num = 33;
								num2 = num;
								continue;
							case 33:
								num = 1;
								if (num != 0)
								{
								}
								if (list2.FirstOrDefault(e.b) == null)
								{
									num = 7;
									num2 = num;
									continue;
								}
								goto case 26;
							case 19:
							case 42:
								num = 45;
								num2 = num;
								continue;
							case 45:
								if (num3 < array.Length)
								{
									MethodInfo method = array[num3];
									onSerializedHandler = (OnSerializedHandler)Delegate.CreateDelegate(typeof(OnSerializedHandler), A_0, method);
									num = 8;
									num2 = num;
								}
								else
								{
									num = 15;
									num2 = num;
								}
								continue;
							case 47:
								list2.Add(new Tuple<MemberInfo, SerializableAttributeBase>(fieldInfo, serializableAttributeBase));
								num = 36;
								num2 = num;
								continue;
							case 16:
								num = 18;
								num2 = num;
								continue;
							case 18:
								if (Attribute.GetCustomAttribute(methodInfo, typeof(OnDeserializedAttribute)) != null)
								{
									num = 46;
									num2 = num;
									continue;
								}
								goto case 12;
							case 2:
								if (serializableAttributeBase != null)
								{
									num = 40;
									num2 = num;
									continue;
								}
								goto case 26;
							case 14:
							case 32:
								num = 22;
								num2 = num;
								continue;
							case 22:
								if (!(type != null))
								{
									num = 17;
									num2 = num;
									continue;
								}
								properties = type.GetProperties(bindingAttr);
								num3 = 0;
								num = 48;
								num2 = num;
								continue;
							case 10:
								A_2 = onSerializedHandler;
								num = 1;
								num2 = num;
								continue;
							case 12:
								num3++;
								num = 30;
								num2 = num;
								continue;
							case 34:
							case 39:
								num3++;
								num = 44;
								num2 = num;
								continue;
							case 46:
								list.Add(methodInfo);
								num = 12;
								num2 = num;
								continue;
							case 1:
							case 20:
								{
									num3++;
									num = 42;
									num2 = num;
									continue;
								}
								IL_0443:
								num = 47;
								num2 = num;
								continue;
							}
							break;
						}
						goto case 0;
					}
					case 0:
						type = null;
						bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
						list2 = new List<Tuple<MemberInfo, SerializableAttributeBase>>();
						a2 = new a();
						list3 = new List<MethodInfo>();
						list = new List<MethodInfo>();
						onSerializedHandler = null;
						onDeserializedHandler = null;
						num = 43;
						num2 = num;
						goto default;
					}
				}
				}
			}

			public static string GetSerializationValueName(object sampleObject, string memberName)
			{
				short num = 23574;
				short num2 = num;
				num = 23574;
				switch (num2 == num)
				{
				default:
				{
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					int num3 = num;
					switch (num3)
					{
					}
					Tuple<MemberInfo, SerializableAttributeBase>[] A_ = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
					switch (0)
					{
					default:
					{
						Tuple<MemberInfo, SerializableAttributeBase> tuple = default(Tuple<MemberInfo, SerializableAttributeBase>);
						Tuple<MemberInfo, SerializableAttributeBase>[] array = default(Tuple<MemberInfo, SerializableAttributeBase>[]);
						int num4 = default(int);
						while (true)
						{
							switch (num3)
							{
							case 3:
								if (A_ != null)
								{
									num = 5;
									num3 = num;
									continue;
								}
								goto end_IL_0087;
							case 8:
								return tuple.Item2.ValueName;
							case 4:
								goto IL_018e;
							case 6:
							case 7:
								num = 1;
								num3 = num;
								continue;
							case 1:
								goto IL_0221;
							case 5:
								array = A_;
								num4 = 0;
								num = 6;
								num3 = num;
								continue;
							case 9:
								num = 0;
								num3 = num;
								continue;
							case 0:
								goto IL_02c9;
							case 2:
								goto end_IL_0087;
							}
							break;
							IL_02c9:
							if (tuple.Item2 is DataMemberCustomAttribute)
							{
								return null;
							}
							num = 8;
							num3 = num;
							continue;
							IL_0221:
							if (num4 < array.Length)
							{
								tuple = array[num4];
								num = 4;
								num3 = num;
							}
							else
							{
								num = 2;
								num3 = num;
							}
							continue;
							IL_018e:
							if (tuple.Item1.Name.EqualsNoCase(memberName))
							{
								num = 1;
								if (num != 0)
								{
								}
								num = 9;
								num3 = num;
							}
							else
							{
								num4++;
								num = 7;
								num3 = num;
							}
						}
						goto case 0;
					}
					case 0:
						{
							num = 0;
							_ = num;
							a(sampleObject, out A_, out var _, out var _);
							num = 3;
							num3 = num;
							goto default;
						}
						end_IL_0087:
						break;
					}
					break;
				}
				case false:
				case true:
					break;
				}
				return null;
			}

			static Registry()
			{
				short num = -1196;
				short num2 = num;
				num = -1196;
				switch (num2 == num)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				loggerErrorHandler = delegate(Exception A_0, MemberInfo A_1, SerializableAttributeBase A_2, string A_3, object A_4)
				{
					int a_ = 15;
					string a_2 = "X3AMH7W2HC81AWBPRNYCKI65C1RTM53DO54BI2GXABQQFFYLQ3YY5";
					int num3 = default(int);
					string contextInformation = default(string);
					switch (0)
					{
					default:
					{
						Type type = default(Type);
						while (true)
						{
							switch (num3)
							{
							case 6:
								if (A_0 is InvalidEnumArgumentException)
								{
									short num4 = 13;
									num3 = num4;
								}
								else
								{
									short num4 = 7;
									num3 = num4;
								}
								continue;
							case 14:
							{
								short num4 = 12;
								num3 = num4;
								continue;
							}
							case 12:
								if (!(A_2 is DataMemberCustomAttribute))
								{
									short num4 = 15;
									num3 = num4;
									continue;
								}
								goto case 10;
							case 7:
								if (A_1 != null)
								{
									short num4 = 3;
									num3 = num4;
									continue;
								}
								goto case 10;
							case 5:
							{
								type = ((PropertyInfo)A_1).PropertyType;
								short num4 = 0;
								num3 = num4;
								continue;
							}
							case 15:
							{
								contextInformation = string.Format("Error loading value {0} from {1}.  Using default value {2}.", A_1.Name, A_2.ValueName, A_4);
								short num4 = 10;
								num3 = num4;
								continue;
							}
							case 13:
							{
								short num4 = 1;
								if (num4 != 0)
								{
								}
								type = null;
								num4 = 11;
								num3 = num4;
								continue;
							}
							case 11:
								if (A_1 is PropertyInfo)
								{
									short num4 = 5;
									num3 = num4;
								}
								else
								{
									short num4 = 1;
									num3 = num4;
								}
								continue;
							case 3:
							{
								short num4 = 8;
								num3 = num4;
								continue;
							}
							case 8:
							{
								short num4 = 0;
								_ = num4;
								if (A_2 != null)
								{
									num4 = 14;
									num3 = num4;
									continue;
								}
								goto case 10;
							}
							case 10:
							{
								Logger.globalLogger.WriteException(A_0, contextInformation);
								short num4 = 2;
								num3 = num4;
								continue;
							}
							case 16:
							{
								short num4 = 9022;
								short num5 = num4;
								num4 = 9022;
								switch (num5 == num4)
								{
								case false:
								case true:
									break;
								default:
									num4 = 0;
									if (num4 != 0)
									{
									}
									type = ((FieldInfo)A_1).FieldType;
									num4 = 4;
									num3 = num4;
									continue;
								}
								goto case 15;
							}
							case 1:
								if (A_1 is FieldInfo)
								{
									short num4 = 16;
									num3 = num4;
									continue;
								}
								goto case 0;
							case 0:
							case 4:
							{
								Logger.globalLogger.WriteLine("Invalid {0} value {1} loaded from {2}.{3} for {4}.{5}.  Using default value {6}.", type.Name, A_0.Data["AttemptedNewValue"], A_3, A_2.ValueName, A_1.ReflectedType.Name, A_1.Name, A_4);
								short num4 = 9;
								num3 = num4;
								continue;
							}
							case 2:
							case 9:
								return true;
							}
							break;
						}
						goto case 0;
					}
					case 0:
					{
						contextInformation = null;
						short num4 = 6;
						num3 = num4;
						goto default;
					}
					}
				};
				Registry.m_a = new Dictionary<object, a>();
			}
		}

		[CompilerGenerated]
		private sealed class a
		{
			public string a;

			internal bool b(string A_0)
			{
				short num = -3815;
				short num2 = num;
				num = -3815;
				switch (num2 == num)
				{
				default:
					num = 0;
					_ = num;
					break;
				case true:
					break;
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return A_0.Equals(a, StringComparison.CurrentCultureIgnoreCase);
			}
		}

		[CompilerGenerated]
		private sealed class b
		{
			public string a;

			internal bool b(string A_0)
			{
				short num = -31914;
				short num2 = num;
				num = -31914;
				switch (num2 == num)
				{
				default:
					num = 0;
					_ = num;
					break;
				case true:
					break;
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				return A_0.Equals(a, StringComparison.CurrentCultureIgnoreCase);
			}
		}

		public static bool HasKeyword(string[] keywordArray, string keyword)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				bool result = default(bool);
				switch (0)
				{
				default:
				{
					string[] array = default(string[]);
					int num3 = default(int);
					string value = default(string);
					while (true)
					{
						switch (num2)
						{
						case 0:
							if (keywordArray != null)
							{
								num = 4;
								num2 = num;
								continue;
							}
							goto case 5;
						case 8:
						{
							num = 20651;
							short num4 = num;
							num = 20651;
							switch (num4 == num)
							{
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 1;
								if (num == 0)
								{
								}
								break;
							case false:
							case true:
								break;
							}
							goto case 1;
						}
						case 6:
							result = true;
							num = 3;
							num2 = num;
							continue;
						case 3:
							num = 0;
							_ = num;
							goto case 5;
						case 4:
							array = keywordArray;
							num3 = 0;
							num = 1;
							num2 = num;
							continue;
						case 1:
							num = 2;
							num2 = num;
							continue;
						case 2:
							if (num3 >= array.Length)
							{
								num = 5;
								num2 = num;
							}
							else
							{
								value = array[num3];
								num = 7;
								num2 = num;
							}
							continue;
						case 7:
							if (!keyword.Equals(value, StringComparison.CurrentCultureIgnoreCase))
							{
								num3++;
								num = 8;
								num2 = num;
							}
							else
							{
								num = 6;
								num2 = num;
							}
							continue;
						case 5:
							return result;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					result = false;
					num = 0;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static string[] SetKeyword(string[] keywordArray, string keyword, bool enableKeyword)
		{
			int num = default(int);
			a a = default(a);
			string[] array = default(string[]);
			switch (0)
			{
			default:
				while (true)
				{
					switch (num)
					{
					case 1:
					{
						short num2;
						if (HasKeyword(keywordArray, a.a))
						{
							num2 = 2;
							num = num2;
							continue;
						}
						num2 = 10411;
						short num3 = num2;
						num2 = 10411;
						switch (num3 == num2)
						{
						default:
							num2 = 0;
							if (num2 != 0)
							{
							}
							num2 = 9;
							num = num2;
							continue;
						case false:
						case true:
							break;
						}
						goto case 9;
					}
					case 5:
					{
						array = new List<string>(array) { a.a }.ToArray();
						short num2 = 7;
						num = num2;
						continue;
					}
					case 7:
					{
						short num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 0;
						_ = num2;
						goto case 0;
					}
					case 9:
						if (enableKeyword)
						{
							short num2 = 6;
							num = num2;
							continue;
						}
						goto case 0;
					case 2:
					{
						short num2 = 3;
						num = num2;
						continue;
					}
					case 3:
						if (!enableKeyword)
						{
							short num2 = 4;
							num = num2;
							continue;
						}
						goto case 0;
					case 4:
					{
						List<string> list = new List<string>(array);
						list.RemoveAll(a.b);
						array = list.ToArray();
						short num2 = 8;
						num = num2;
						continue;
					}
					case 6:
					{
						short num2 = 10;
						num = num2;
						continue;
					}
					case 10:
						if (array == null)
						{
							array = new string[1] { a.a };
							short num2 = 0;
							num = num2;
						}
						else
						{
							short num2 = 5;
							num = num2;
						}
						continue;
					case 0:
					case 8:
						return array;
					}
					break;
				}
				goto case 0;
			case 0:
			{
				a = new a();
				a.a = keyword;
				array = keywordArray;
				short num2 = 1;
				num = num2;
				goto default;
			}
			}
		}

		public static bool HasKeyword(string sourceString, string keyword)
		{
			int num = default(int);
			bool result = default(bool);
			short num2;
			switch (0)
			{
			default:
				while (true)
				{
					switch (num)
					{
					case 0:
						if (sourceString != null)
						{
							num2 = 2;
							num = num2;
							continue;
						}
						goto case 1;
					case 2:
						goto IL_0080;
					case 1:
						return result;
					}
					break;
					IL_0080:
					num2 = 0;
					_ = num2;
					num2 = -30845;
					short num3 = num2;
					num2 = -30845;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 1;
						if (num2 != 0)
						{
						}
						sourceString.ToLower();
						result = sourceString.IndexOf(keyword) != -1;
						num2 = 1;
						num = num2;
						continue;
					}
					goto IL_001e;
				}
				goto case 0;
			case 0:
				{
					result = false;
					goto IL_001e;
				}
				IL_001e:
				num2 = 0;
				num = num2;
				goto default;
			}
		}

		public static string SetKeyword(string sourceString, string keyword, bool enableKeyword)
		{
			int a_ = 8;
			string a_2 = "62PCQL30915X087MWOEZQI9ANHLUBHCD16RCV23MON9T3F28N1IWPJJ";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				b b = default(b);
				string text = default(string);
				switch (0)
				{
				default:
				{
					List<string>.Enumerator enumerator = default(List<string>.Enumerator);
					string current = default(string);
					while (true)
					{
						switch (num2)
						{
						case 1:
							if (enableKeyword)
							{
								num = 10;
								num2 = num;
							}
							else
							{
								num = 2;
								num2 = num;
							}
							continue;
						case 3:
						{
							List<string> list = new List<string>(sourceString.Split(new char[1] { ',' }));
							list.RemoveAll(b.b);
							text = null;
							enumerator = list.GetEnumerator();
							num = 6;
							num2 = num;
							continue;
						}
						case 2:
							if (HasKeyword(sourceString, b.a))
							{
								num = 9030;
								short num3 = num;
								num = 9030;
								switch (num3 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 0;
									if (num != 0)
									{
									}
									num = 3;
									num2 = num;
									continue;
								}
								break;
							}
							goto case 5;
						case 8:
							num = 9;
							num2 = num;
							continue;
						case 9:
							if (sourceString == null)
							{
								num = 0;
								num2 = num;
								continue;
							}
							text = string.Format("{0},{1}", sourceString, b.a);
							num = 0;
							_ = num;
							num = 5;
							num2 = num;
							continue;
						case 10:
							num = 4;
							num2 = num;
							continue;
						case 4:
							num = 1;
							if (num != 0)
							{
							}
							if (!HasKeyword(sourceString, b.a))
							{
								num = 8;
								num2 = num;
								continue;
							}
							goto case 5;
						case 6:
							try
							{
								num = 5;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 5:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									case 2:
										if (text == null)
										{
											num = 3;
											num2 = num;
										}
										else
										{
											text = string.Format("{0},{1}", sourceString, b.a);
											num = 7;
											num2 = num;
										}
										continue;
									default:
										num = 1;
										num2 = num;
										continue;
									case 1:
										if (enumerator.MoveNext())
										{
											current = enumerator.Current;
											num = 2;
											num2 = num;
										}
										else
										{
											num = 0;
											num2 = num;
										}
										continue;
									case 3:
										text = current;
										num = 4;
										num2 = num;
										continue;
									case 0:
										num = 6;
										num2 = num;
										continue;
									case 6:
										break;
									}
									break;
								}
							}
							finally
							{
								((IDisposable)enumerator).Dispose();
							}
							goto case 5;
						case 0:
							text = b.a;
							num = 7;
							num2 = num;
							continue;
						case 5:
						case 7:
							return text;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					b = new b();
					b.a = keyword;
					text = sourceString;
					num = 1;
					num2 = num;
					goto default;
				}
			}
			}
		}
	}

	public static class FileSecurity
	{
		public const FileSystemRights StandardFileReadAndWriteRights = FileSystemRights.Modify | FileSystemRights.Synchronize;

		public static void GrantRightsToTree(string accountName, string targetDirectoryPathname, FileSystemRights rightsToGrant, AccessControlType grantType = AccessControlType.Allow)
		{
			short num = 0;
			_ = num;
			num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				IdentityReference obj = default(IdentityReference);
				bool flag = default(bool);
				IEnumerator enumerator = default(IEnumerator);
				InheritanceFlags inheritanceFlags = default(InheritanceFlags);
				PropagationFlags propagationFlags = default(PropagationFlags);
				DirectorySecurity accessControl = default(DirectorySecurity);
				NTAccount nTAccount = default(NTAccount);
				DirectoryInfo directoryInfo = default(DirectoryInfo);
				switch (0)
				{
				default:
				{
					FileSystemAccessRule fileSystemAccessRule = default(FileSystemAccessRule);
					IDisposable disposable = default(IDisposable);
					while (true)
					{
						switch (num2)
						{
						case 2:
							num = 1;
							if (num != 0)
							{
							}
							try
							{
								num = 7;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 7:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									case 5:
										if (((SecurityIdentifier)fileSystemAccessRule.IdentityReference).Equals(obj))
										{
											goto IL_020d;
										}
										goto default;
									case 4:
										flag = true;
										num = 3;
										num2 = num;
										continue;
									default:
										num = 2;
										num2 = num;
										continue;
									case 2:
										if (enumerator.MoveNext())
										{
											fileSystemAccessRule = (FileSystemAccessRule)enumerator.Current;
											num = 5;
											num2 = num;
										}
										else
										{
											num = 11;
											num2 = num;
										}
										continue;
									case 12:
										num = 13;
										num2 = num;
										continue;
									case 13:
										if (fileSystemAccessRule.InheritanceFlags == inheritanceFlags)
										{
											num = 14;
											num2 = num;
											continue;
										}
										goto default;
									case 0:
										num = 8;
										num2 = num;
										continue;
									case 8:
										if (fileSystemAccessRule.FileSystemRights == rightsToGrant)
										{
											num = 9;
											num2 = num;
											continue;
										}
										goto default;
									case 9:
									{
										num = 19672;
										short num3 = num;
										num = 19672;
										switch (num3 == num)
										{
										case false:
										case true:
											break;
										default:
											num = 0;
											if (num != 0)
											{
											}
											num = 10;
											num2 = num;
											continue;
										}
										goto IL_020d;
									}
									case 14:
										num = 1;
										num2 = num;
										continue;
									case 1:
										if (fileSystemAccessRule.AccessControlType == grantType)
										{
											num = 4;
											num2 = num;
											continue;
										}
										goto default;
									case 10:
										if (fileSystemAccessRule.PropagationFlags == propagationFlags)
										{
											num = 12;
											num2 = num;
											continue;
										}
										goto default;
									case 11:
										num = 6;
										num2 = num;
										continue;
									case 6:
										break;
										IL_020d:
										num = 0;
										num2 = num;
										continue;
									}
									break;
								}
							}
							finally
							{
								switch (0)
								{
								default:
									while (true)
									{
										switch (num2)
										{
										case 0:
											if (disposable != null)
											{
												num = 2;
												num2 = num;
												continue;
											}
											goto end_IL_0471;
										case 2:
											disposable.Dispose();
											num = 1;
											num2 = num;
											continue;
										case 1:
											goto end_IL_0471;
										}
										goto case 0;
										continue;
										end_IL_0471:
										break;
									}
									break;
								case 0:
									disposable = enumerator as IDisposable;
									num = 0;
									num2 = num;
									goto default;
								}
							}
							num = 3;
							num2 = num;
							continue;
						case 3:
							if (!flag)
							{
								num = 1;
								num2 = num;
								continue;
							}
							return;
						case 1:
							accessControl.AddAccessRule(new FileSystemAccessRule(nTAccount, rightsToGrant, inheritanceFlags, propagationFlags, grantType));
							directoryInfo.SetAccessControl(accessControl);
							num = 0;
							num2 = num;
							continue;
						case 0:
							return;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					nTAccount = new NTAccount(accountName);
					obj = nTAccount.Translate(typeof(SecurityIdentifier));
					directoryInfo = new DirectoryInfo(targetDirectoryPathname);
					accessControl = directoryInfo.GetAccessControl();
					inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
					propagationFlags = PropagationFlags.None;
					flag = false;
					enumerator = accessControl.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(SecurityIdentifier)).GetEnumerator();
					num = 2;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static void RemoveInheritedRightsToTree(string targetDirectoryPathname, SecurityIdentifier targetAccount, Action<FileSystemAccessRule> ruleActionNotifier = null)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				DirectoryInfo directoryInfo = default(DirectoryInfo);
				List<FileSystemAccessRule> list = default(List<FileSystemAccessRule>);
				IEnumerator enumerator2 = default(IEnumerator);
				DirectorySecurity accessControl = default(DirectorySecurity);
				switch (0)
				{
				default:
				{
					FileSystemAccessRule current = default(FileSystemAccessRule);
					List<FileSystemAccessRule>.Enumerator enumerator = default(List<FileSystemAccessRule>.Enumerator);
					FileSystemAccessRule fileSystemAccessRule = default(FileSystemAccessRule);
					IDisposable disposable = default(IDisposable);
					while (true)
					{
						switch (num2)
						{
						case 3:
							try
							{
								num = 0;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 0:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									case 5:
										accessControl.RemoveAccessRuleSpecific(current);
										num = 1;
										num2 = num;
										continue;
									case 7:
										if (ruleActionNotifier != null)
										{
											num = 2;
											num2 = num;
											continue;
										}
										goto case 5;
									default:
										num = 4;
										num2 = num;
										continue;
									case 4:
										if (enumerator.MoveNext())
										{
											current = enumerator.Current;
											num = 7;
											num2 = num;
										}
										else
										{
											num = 6;
											num2 = num;
										}
										continue;
									case 2:
										ruleActionNotifier(current);
										num = 5;
										num2 = num;
										continue;
									case 6:
										num = 3;
										num2 = num;
										continue;
									case 3:
										break;
									}
									break;
								}
							}
							finally
							{
								((IDisposable)enumerator).Dispose();
							}
							directoryInfo.SetAccessControl(accessControl);
							num = 2;
							num2 = num;
							continue;
						case 2:
							return;
						case 4:
							try
							{
								num = 5;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 5:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									case 3:
										list.Add(fileSystemAccessRule);
										num = 4;
										num2 = num;
										continue;
									case 6:
										if (((SecurityIdentifier)fileSystemAccessRule.IdentityReference).Equals(targetAccount))
										{
											num = 3;
											num2 = num;
											continue;
										}
										goto default;
									default:
										num = 2;
										num2 = num;
										continue;
									case 2:
										if (enumerator2.MoveNext())
										{
											fileSystemAccessRule = (FileSystemAccessRule)enumerator2.Current;
											num = 6;
											num2 = num;
										}
										else
										{
											num = 0;
											num2 = num;
										}
										continue;
									case 0:
										num = 1;
										num2 = num;
										continue;
									case 1:
										break;
									}
									break;
								}
							}
							finally
							{
								switch (0)
								{
								default:
									while (true)
									{
										switch (num2)
										{
										case 2:
											if (disposable != null)
											{
												num = 0;
												num2 = num;
												continue;
											}
											goto end_IL_0447;
										case 0:
											disposable.Dispose();
											num = 1;
											num2 = num;
											continue;
										case 1:
											goto end_IL_0447;
										}
										goto case 0;
										continue;
										end_IL_0447:
										break;
									}
									break;
								case 0:
									disposable = enumerator2 as IDisposable;
									num = 2;
									num2 = num;
									goto default;
								}
							}
							goto IL_04c5;
						case 0:
							if (list.Count > 0)
							{
								num = 1;
								num2 = num;
								continue;
							}
							return;
						case 1:
							{
								num = 18044;
								short num3 = num;
								num = 18044;
								switch (num3 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 0;
									if (num != 0)
									{
									}
									enumerator = list.GetEnumerator();
									num = 3;
									num2 = num;
									continue;
								}
								goto IL_04c5;
							}
							IL_04c5:
							num = 0;
							num2 = num;
							continue;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					num = 1;
					if (num != 0)
					{
					}
					directoryInfo = new DirectoryInfo(targetDirectoryPathname);
					accessControl = directoryInfo.GetAccessControl();
					list = new List<FileSystemAccessRule>();
					accessControl.SetAccessRuleProtection(isProtected: true, preserveInheritance: true);
					directoryInfo.SetAccessControl(accessControl);
					accessControl = directoryInfo.GetAccessControl();
					enumerator2 = accessControl.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(SecurityIdentifier)).GetEnumerator();
					num = 0;
					_ = num;
					num = 4;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static void PreserveInheritanceRules(string filepath)
		{
			short num = 30090;
			short num2 = num;
			num = 30090;
			switch (num2 == num)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			System.Security.AccessControl.FileSecurity accessControl = File.GetAccessControl(filepath);
			accessControl.SetAccessRuleProtection(isProtected: false, preserveInheritance: false);
			File.SetAccessControl(filepath, accessControl);
		}
	}

	public static class RegistrySecurity
	{
		public const RegistryRights StandardKeyReadAndWriteRights = RegistryRights.ExecuteKey | RegistryRights.SetValue | RegistryRights.CreateSubKey;

		public static void GrantRightsToTree(string targetKeyPathname, SecurityIdentifier targetUserIdentitySid, RegistryRights rightsToGrant, AccessControlType grantType = AccessControlType.Allow)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				num = 0;
				_ = num;
				RegistryKey registryKey = OpenRegistryKey(targetKeyPathname, openForWrite: true);
				try
				{
					IEnumerator enumerator = default(IEnumerator);
					InheritanceFlags inheritanceFlags = default(InheritanceFlags);
					bool flag = default(bool);
					PropagationFlags propagationFlags = default(PropagationFlags);
					System.Security.AccessControl.RegistrySecurity accessControl = default(System.Security.AccessControl.RegistrySecurity);
					switch (0)
					{
					default:
					{
						RegistryAccessRule registryAccessRule = default(RegistryAccessRule);
						IDisposable disposable = default(IDisposable);
						while (true)
						{
							switch (num2)
							{
							case 3:
								try
								{
									num = 15761;
									short num3 = num;
									num = 15761;
									switch (num3 == num)
									{
									default:
										num = 0;
										if (num != 0)
										{
										}
										num = 10;
										num2 = num;
										break;
									case false:
									case true:
										break;
									}
									while (true)
									{
										switch (num2)
										{
										case 10:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											num = 7;
											num2 = num;
											continue;
										case 7:
											if (!enumerator.MoveNext())
											{
												num = 0;
												num2 = num;
											}
											else
											{
												registryAccessRule = (RegistryAccessRule)enumerator.Current;
												num = 13;
												num2 = num;
											}
											continue;
										case 2:
											num = 14;
											num2 = num;
											continue;
										case 14:
											if (registryAccessRule.InheritanceFlags == inheritanceFlags)
											{
												num = 6;
												num2 = num;
												continue;
											}
											goto default;
										case 9:
											num = 8;
											num2 = num;
											continue;
										case 8:
											if (registryAccessRule.RegistryRights == rightsToGrant)
											{
												num = 12;
												num2 = num;
												continue;
											}
											goto default;
										case 5:
											flag = true;
											num = 3;
											num2 = num;
											continue;
										case 6:
											num = 11;
											num2 = num;
											continue;
										case 11:
											if (registryAccessRule.AccessControlType == grantType)
											{
												num = 5;
												num2 = num;
												continue;
											}
											goto default;
										case 13:
											if (((SecurityIdentifier)registryAccessRule.IdentityReference).Equals(targetUserIdentitySid))
											{
												num = 9;
												num2 = num;
												continue;
											}
											goto default;
										case 12:
											num = 1;
											num2 = num;
											continue;
										case 1:
											if (registryAccessRule.PropagationFlags == propagationFlags)
											{
												num = 2;
												num2 = num;
												continue;
											}
											goto default;
										case 0:
											num = 4;
											num2 = num;
											continue;
										case 4:
											break;
										}
										break;
									}
								}
								finally
								{
									switch (0)
									{
									default:
										while (true)
										{
											switch (num2)
											{
											case 2:
												if (disposable != null)
												{
													num = 0;
													num2 = num;
													continue;
												}
												goto end_IL_03fb;
											case 0:
												disposable.Dispose();
												num = 1;
												if (num != 0)
												{
												}
												num = 1;
												num2 = num;
												continue;
											case 1:
												goto end_IL_03fb;
											}
											goto case 0;
											continue;
											end_IL_03fb:
											break;
										}
										break;
									case 0:
										disposable = enumerator as IDisposable;
										num = 2;
										num2 = num;
										goto default;
									}
								}
								num = 1;
								num2 = num;
								continue;
							case 4:
								accessControl.AddAccessRule(new RegistryAccessRule(targetUserIdentitySid, rightsToGrant, inheritanceFlags, propagationFlags, grantType));
								registryKey.SetAccessControl(accessControl);
								num = 2;
								num2 = num;
								continue;
							case 1:
								if (!flag)
								{
									num = 4;
									num2 = num;
									continue;
								}
								goto case 2;
							case 2:
								num = 0;
								num2 = num;
								continue;
							case 0:
								return;
							}
							break;
						}
						goto case 0;
					}
					case 0:
						accessControl = registryKey.GetAccessControl();
						inheritanceFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
						propagationFlags = PropagationFlags.None;
						flag = false;
						enumerator = accessControl.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(SecurityIdentifier)).GetEnumerator();
						num = 3;
						num2 = num;
						goto default;
					}
				}
				finally
				{
					num = 1;
					num2 = num;
					while (true)
					{
						switch (num2)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey != null)
							{
								num = 2;
								num2 = num;
								continue;
							}
							break;
						case 2:
							((IDisposable)registryKey).Dispose();
							num = 0;
							num2 = num;
							continue;
						case 0:
							break;
						}
						break;
					}
				}
			}
			}
		}

		public static void RemoveInheritedRightsToKey(string targetKeyPathname, SecurityIdentifier targetAccount, Action<RegistryAccessRule> ruleActionNotifier = null)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				RegistryKey registryKey = OpenRegistryKey(targetKeyPathname, openForWrite: true);
				try
				{
					IEnumerator enumerator = default(IEnumerator);
					List<RegistryAccessRule> list = default(List<RegistryAccessRule>);
					System.Security.AccessControl.RegistrySecurity accessControl = default(System.Security.AccessControl.RegistrySecurity);
					switch (0)
					{
					default:
					{
						RegistryAccessRule registryAccessRule = default(RegistryAccessRule);
						IDisposable disposable = default(IDisposable);
						RegistryAccessRule current = default(RegistryAccessRule);
						List<RegistryAccessRule>.Enumerator enumerator2 = default(List<RegistryAccessRule>.Enumerator);
						while (true)
						{
							switch (num2)
							{
							case 1:
								try
								{
									num = 2;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 2:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											num = 5;
											num2 = num;
											continue;
										case 5:
											if (!enumerator.MoveNext())
											{
												num = 1;
												num2 = num;
											}
											else
											{
												registryAccessRule = (RegistryAccessRule)enumerator.Current;
												num = 0;
												num2 = num;
											}
											continue;
										case 3:
											list.Add(registryAccessRule);
											num = 4;
											num2 = num;
											continue;
										case 0:
											if (((SecurityIdentifier)registryAccessRule.IdentityReference).Equals(targetAccount))
											{
												num = 3;
												num2 = num;
												continue;
											}
											goto default;
										case 1:
											num = 6;
											num2 = num;
											continue;
										case 6:
											break;
										}
										break;
									}
								}
								finally
								{
									switch (0)
									{
									default:
										while (true)
										{
											switch (num2)
											{
											case 2:
												if (disposable != null)
												{
													num = 1;
													num2 = num;
													continue;
												}
												goto end_IL_027e;
											case 1:
												disposable.Dispose();
												num = 0;
												num2 = num;
												continue;
											case 0:
												goto end_IL_027e;
											}
											goto case 0;
											continue;
											end_IL_027e:
											break;
										}
										break;
									case 0:
										disposable = enumerator as IDisposable;
										num = 2;
										num2 = num;
										goto default;
									}
								}
								num = 3;
								num2 = num;
								continue;
							case 5:
								try
								{
									num = 2;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 2:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										case 6:
										{
											num = -1677;
											short num3 = num;
											num = -1677;
											switch (num3 == num)
											{
											case false:
											case true:
												continue;
											}
											num = 0;
											if (num != 0)
											{
											}
											if (ruleActionNotifier != null)
											{
												num = 0;
												num2 = num;
												continue;
											}
											goto case 3;
										}
										case 0:
											ruleActionNotifier(current);
											num = 3;
											num2 = num;
											continue;
										case 3:
											accessControl.RemoveAccessRuleSpecific(current);
											num = 7;
											num2 = num;
											continue;
										default:
											num = 1;
											num2 = num;
											continue;
										case 1:
											if (enumerator2.MoveNext())
											{
												current = enumerator2.Current;
												num = 6;
												num2 = num;
											}
											else
											{
												num = 5;
												num2 = num;
											}
											continue;
										case 5:
											num = 4;
											num2 = num;
											continue;
										case 4:
											break;
										}
										break;
									}
								}
								finally
								{
									((IDisposable)enumerator2).Dispose();
								}
								registryKey.SetAccessControl(accessControl);
								num = 4;
								num2 = num;
								continue;
							case 2:
								enumerator2 = list.GetEnumerator();
								num = 5;
								num2 = num;
								continue;
							case 3:
								if (list.Count > 0)
								{
									num = 2;
									num2 = num;
									continue;
								}
								goto case 4;
							case 4:
								num = 0;
								num2 = num;
								continue;
							case 0:
								return;
							}
							break;
						}
						goto case 0;
					}
					case 0:
						accessControl = registryKey.GetAccessControl();
						list = new List<RegistryAccessRule>();
						accessControl.SetAccessRuleProtection(isProtected: true, preserveInheritance: true);
						registryKey.SetAccessControl(accessControl);
						accessControl = registryKey.GetAccessControl();
						enumerator = accessControl.GetAccessRules(includeExplicit: true, includeInherited: true, typeof(SecurityIdentifier)).GetEnumerator();
						num = 1;
						num2 = num;
						goto default;
					}
				}
				finally
				{
					num = 1;
					num2 = num;
					while (true)
					{
						switch (num2)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey != null)
							{
								num = 0;
								num2 = num;
								continue;
							}
							break;
						case 0:
							((IDisposable)registryKey).Dispose();
							num = 2;
							num2 = num;
							continue;
						case 2:
							break;
						}
						break;
					}
				}
			}
			}
		}
	}

	public static class Ini
	{
		public static string GetIniKey(string sourceFilename, string sectionName, string keyName, string defaultValue = null)
		{
			int a_ = 0;
			string a_2 = "OCGXM790L83CEN4S95TF5LM5WOMXK7FSDJOBOWS6KVV3PR4MUZ1S";
			switch (0)
			{
			case 0:
				goto IL_0056;
			}
			goto IL_001c;
			IL_001c:
			int num = default(int);
			string result = default(string);
			StringBuilder stringBuilder = default(StringBuilder);
			uint num4 = default(uint);
			short num2;
			while (true)
			{
				switch (num)
				{
				case 0:
					num2 = 0;
					_ = num2;
					if (sourceFilename == null)
					{
						num2 = 1;
						num = num2;
					}
					else
					{
						num2 = 7;
						num = num2;
					}
					continue;
				case 3:
					result = stringBuilder.ToString();
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = 12;
					num = num2;
					continue;
				case 11:
					if (num4 != 0)
					{
						num2 = 5;
						num = num2;
					}
					else
					{
						result = defaultValue;
						num2 = 2;
						num = num2;
					}
					continue;
				case 7:
					if (sectionName == null)
					{
						num2 = 8;
						num = num2;
					}
					else
					{
						num2 = 9;
						num = num2;
					}
					continue;
				case 1:
					throw new ArgumentNullException("sourceFilename");
				case 5:
					num2 = 10;
					num = num2;
					continue;
				case 10:
					if (num4 == 511)
					{
						stringBuilder.Capacity = (int)(num4 + 1);
						GetPrivateProfileString(sectionName, keyName, defaultValue, stringBuilder, 512u, sourceFilename);
						result = stringBuilder.ToString();
						num2 = 4;
						num = num2;
					}
					else
					{
						num2 = 3;
						num = num2;
					}
					continue;
				case 9:
					if (keyName != null)
					{
						num4 = GetPrivateProfileString(sectionName, keyName, defaultValue, stringBuilder, 512u, sourceFilename);
						num2 = 11;
						num = num2;
					}
					else
					{
						num2 = 6;
						num = num2;
					}
					continue;
				case 8:
				{
					num2 = 16581;
					short num3 = num2;
					num2 = 16581;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						throw new ArgumentNullException("sectionName");
					case false:
					case true:
						break;
					}
					goto case 2;
				}
				case 6:
					throw new ArgumentNullException("keyName");
				case 2:
				case 4:
				case 12:
					return result;
				}
				break;
			}
			goto IL_0056;
			IL_0056:
			stringBuilder = new StringBuilder(512);
			num4 = uint.MaxValue;
			result = null;
			num2 = 0;
			num = num2;
			goto IL_001c;
		}

		public static void SetIniKey(string targetFilename, string sectionName, string keyName, string newValue)
		{
			int a_ = 19;
			string a_2 = "5CES64BQ41YZD8K6XYMU1EYFMZXF5HNS3FAL8UICQA2RPFCD0H2GVH6COZ27";
			short num = 0;
			int num2 = num;
			while (true)
			{
				switch (num2)
				{
				case 0:
					num = 0;
					_ = num;
					switch (0)
					{
					default:
						goto end_IL_0072;
					case 0:
						break;
					}
					goto default;
				default:
					if (targetFilename == null)
					{
						num = 2;
						num2 = num;
					}
					else
					{
						num = 4;
						num2 = num;
					}
					break;
				case 7:
					if (keyName == null)
					{
						num = 1;
						num2 = num;
					}
					else
					{
						num = 3;
						num2 = num;
					}
					break;
				case 1:
					num = 1;
					if (num != 0)
					{
					}
					throw new ArgumentNullException("keyName");
				case 4:
				{
					if (sectionName != null)
					{
						num = 7;
						num2 = num;
						break;
					}
					num = -28462;
					short num3 = num;
					num = -28462;
					switch (num3 == num)
					{
					case false:
					case true:
						break;
					default:
						num = 0;
						if (num != 0)
						{
						}
						num = 5;
						num2 = num;
						goto end_IL_0072;
					}
					goto case 7;
				}
				case 6:
					return;
				case 2:
					throw new ArgumentNullException("targetFilename");
				case 3:
					if (WritePrivateProfileString(sectionName, keyName, newValue, targetFilename) != 0)
					{
						num = 6;
						num2 = num;
						break;
					}
					throw new Win32Exception(Marshal.GetLastWin32Error());
				case 5:
					{
						throw new ArgumentNullException("sectionName");
					}
					end_IL_0072:
					break;
				}
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint GetPrivateProfileString(string A_0, string A_1, string A_2, StringBuilder A_3, uint A_4, string A_5);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint GetPrivateProfileInt(string A_0, string A_1, int A_2, string A_3);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint WritePrivateProfileString(string A_0, string A_1, string A_2, string A_3);
	}

	public static class WindowsService
	{
		public enum ServiceControlAction
		{
			Stop,
			Start
		}

		[CompilerGenerated]
		private sealed class a
		{
			public string a;

			public bool b;

			internal void c()
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Expected O, but got Unknown
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Invalid comparison between Unknown and I4
				while (true)
				{
					ServiceController val = new ServiceController(a);
					short num;
					try
					{
						b = (int)val.Status == 4;
					}
					finally
					{
						num = 2;
						int num2 = num;
						while (true)
						{
							switch (num2)
							{
							case 2:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (val != null)
								{
									num = 1;
									num2 = num;
									continue;
								}
								break;
							case 1:
								((IDisposable)val).Dispose();
								num = 0;
								num2 = num;
								continue;
							case 0:
								break;
							}
							break;
						}
					}
					num = 0;
					_ = num;
					num = 1;
					if (num != 0)
					{
					}
					num = 5263;
					short num3 = num;
					num = 5263;
					switch (num3 == num)
					{
					case false:
					case true:
						continue;
					}
					num = 0;
					if (num == 0)
					{
					}
					return;
				}
			}
		}

		public static bool IsServiceRunning(string serviceInternalName)
		{
			short num = -4633;
			short num2 = num;
			num = -4633;
			switch (num2 == num)
			{
			default:
			{
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				bool b = false;
				DoNothrow(delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Expected O, but got Unknown
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Invalid comparison between Unknown and I4
					while (true)
					{
						ServiceController val = new ServiceController(serviceInternalName);
						short num3;
						try
						{
							b = (int)val.Status == 4;
						}
						finally
						{
							num3 = 2;
							int num4 = num3;
							while (true)
							{
								switch (num4)
								{
								case 2:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (val != null)
									{
										num3 = 1;
										num4 = num3;
										continue;
									}
									break;
								case 1:
									((IDisposable)val).Dispose();
									num3 = 0;
									num4 = num3;
									continue;
								case 0:
									break;
								}
								break;
							}
						}
						num3 = 0;
						_ = num3;
						num3 = 1;
						if (num3 != 0)
						{
						}
						num3 = 5263;
						short num5 = num3;
						num3 = 5263;
						switch (num5 == num3)
						{
						case false:
						case true:
							break;
						default:
							num3 = 0;
							if (num3 == 0)
							{
							}
							return;
						}
					}
				});
				return b;
			}
			}
		}

		public static void ServiceControl(string[] serviceNames, ServiceControlAction desiredAction, int maxWaitTimeSeconds, out bool alreadyInDesiredState)
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Expected O, but got Unknown
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Invalid comparison between Unknown and I4
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			int a_ = 0;
			string a_2 = "JAIOQZGB6K2BDNJOKHN8GD5L9P8X555F04P1XCIDF93EHRRLLMZG5";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				string text2 = default(string);
				ServiceControllerStatus val2 = default(ServiceControllerStatus);
				string arg = default(string);
				string arg2 = default(string);
				bool flag2 = default(bool);
				int num3 = default(int);
				bool flag = default(bool);
				switch (0)
				{
				default:
				{
					string[] array = default(string[]);
					int num5 = default(int);
					string text = default(string);
					ServiceController val = default(ServiceController);
					TimeSpan timeSpan = default(TimeSpan);
					DateTime now2 = default(DateTime);
					DateTime now = default(DateTime);
					while (true)
					{
						switch (num2)
						{
						case 0:
							if (desiredAction == ServiceControlAction.Start)
							{
								num = 6;
								num2 = num;
								continue;
							}
							goto case 3;
						case 7:
							throw new TimeoutException(text2);
						case 3:
							array = serviceNames;
							num5 = 0;
							num = 2;
							num2 = num;
							continue;
						case 6:
							val2 = (ServiceControllerStatus)4;
							arg = "start";
							arg2 = "started";
							num = 3;
							num2 = num;
							continue;
						case 5:
							if (!flag2)
							{
								num5++;
								num = 1;
								num2 = num;
							}
							else
							{
								num = 7;
								num2 = num;
							}
							continue;
						case 1:
						case 2:
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (num5 >= array.Length)
							{
								num = 9;
								num2 = num;
								continue;
							}
							text = array[num5];
							val = new ServiceController(text);
							num = 4;
							num2 = num;
							continue;
						case 9:
							return;
						case 4:
							num = 0;
							_ = num;
							try
							{
								num = 16;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 16:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (val.Status != val2)
										{
											num = 9;
											num2 = num;
										}
										else
										{
											alreadyInDesiredState = true;
											num = 1;
											num2 = num;
										}
										continue;
									case 2:
										Console.WriteLine("Service {0} successfully {1}.", text, arg2);
										num = 18;
										num2 = num;
										continue;
									case 18:
										break;
									case 15:
									{
										num = 30012;
										short num4 = num;
										num = 30012;
										switch (num4 == num)
										{
										default:
											num = 0;
											if (num == 0)
											{
											}
											return;
										case false:
										case true:
											break;
										}
										goto IL_0649;
									}
									case 14:
										if (timeSpan.TotalSeconds >= (double)num3 || flag)
										{
											num = 17;
											num2 = num;
											continue;
										}
										goto case 13;
									case 9:
										num = 8;
										num2 = num;
										continue;
									case 8:
										if ((int)val2 == 1)
										{
											num = 11;
											num2 = num;
										}
										else
										{
											val.Start();
											num = 19;
											num2 = num;
										}
										continue;
									case 5:
									case 19:
										now2 = DateTime.Now;
										now = DateTime.Now;
										val.Refresh();
										num = 6;
										num2 = num;
										continue;
									case 17:
										Console.WriteLine("Waiting for {0} to {1}, this may take several minutes...", text, arg);
										now = DateTime.Now;
										flag = false;
										num = 13;
										num2 = num;
										continue;
									case 3:
										num = 7;
										num2 = num;
										continue;
									case 7:
										if (val.Status != val2)
										{
											text2 = string.Format("Timeout waiting for service {0} to {1}.", text, arg);
											Console.WriteLine(text2);
											flag2 = true;
											num = 15;
											num2 = num;
										}
										else
										{
											num = 2;
											num2 = num;
										}
										continue;
									case 10:
										timeSpan = now2.Subtract(DateTime.Now);
										num = 0;
										num2 = num;
										continue;
									case 0:
										if (timeSpan.TotalSeconds < (double)maxWaitTimeSeconds)
										{
											Thread.Sleep(200);
											timeSpan = DateTime.Now.Subtract(now);
											num = 14;
											num2 = num;
										}
										else
										{
											num = 3;
											num2 = num;
										}
										continue;
									case 13:
										val.Refresh();
										goto IL_0649;
									case 11:
										val.Stop();
										num = 5;
										num2 = num;
										continue;
									case 6:
									case 12:
										num = 4;
										num2 = num;
										continue;
									case 4:
										if (val.Status != val2)
										{
											num = 10;
											num2 = num;
											continue;
										}
										goto case 3;
									case 1:
										break;
										IL_0649:
										num = 12;
										num2 = num;
										continue;
									}
									break;
								}
							}
							finally
							{
								num = 1;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 1:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (val != null)
										{
											num = 0;
											num2 = num;
											continue;
										}
										break;
									case 0:
										((IDisposable)val).Dispose();
										num = 2;
										num2 = num;
										continue;
									case 2:
										break;
									}
									break;
								}
							}
							num = 5;
							num2 = num;
							continue;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					num = 1;
					if (num != 0)
					{
					}
					text2 = "";
					num3 = 5;
					flag = true;
					flag2 = false;
					val2 = (ServiceControllerStatus)1;
					arg = "stop";
					arg2 = "stopped";
					alreadyInDesiredState = false;
					num = 0;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static void ServiceControl(string[] serviceNames, ServiceControlAction desiredAction, int maxWaitTimeSeconds)
		{
			short num = 15688;
			short num2 = num;
			num = 15688;
			switch (num2 == num)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			bool alreadyInDesiredState = false;
			ServiceControl(serviceNames, desiredAction, maxWaitTimeSeconds, out alreadyInDesiredState);
		}
	}

	public static class COM
	{
		[CompilerGenerated]
		private sealed class a
		{
			public RegistryKey a;
		}

		[CompilerGenerated]
		private sealed class b
		{
			public string a;

			public a b;

			internal void c()
			{
				short num = 0;
				_ = num;
				num = -8468;
				short num2 = num;
				num = -8468;
				switch (num2 == num)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				b.a.DeleteSubKeyTree(a);
			}
		}

		public static readonly Exception NotYetInitialized;

		public const int ERROR_SUCCESS = 0;

		public const int ERROR_INVALID_FUNCTION = 1;

		public const int ERROR_FILE_NOT_FOUND = 2;

		public const int ERROR_PATH_NOT_FOUND = 3;

		public const int ERROR_ACCESS_DENIED = 5;

		public const int ERROR_BAD_FORMAT = 11;

		public const int ERROR_ALREADY_EXISTS = 183;

		public const int ERROR_NOT_FOUND = 1168;

		public static readonly int E_FAIL;

		public static readonly int E_ABORT;

		public static readonly int E_ACCESSDENIED;

		public static readonly int E_NOTIMPL;

		public static readonly int E_INVALIDARG;

		public const int S_OK = 0;

		public const int S_FALSE = 1;

		public const int WSAHOST_NOT_FOUND = 11001;

		public const int FACILITY_NULL = 0;

		public const int FACILITY_WINDOWS = 8;

		public const int FACILITY_SECURITY = 9;

		public const int HRESULT_STATUSKIND_ERROR = int.MinValue;

		public const int FACILITY_MASK = 20479;

		public const int FACILITY_MASK_FORHRESULT = 1342111744;

		public static int HRESULT_FROM_WIN32(short errorCode)
		{
			short num = 0;
			_ = num;
			num = 22446;
			short num2 = num;
			num = 22446;
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				return HRESULT_FROM_WIN32(8, errorCode);
			}
		}

		public static int HRESULT_FROM_WIN32(short facilityCode, short errorCode)
		{
			short num = 0;
			_ = num;
			num = 14707;
			short num2 = num;
			num = 14707;
			switch (num2 == num)
			{
			default:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return HRESULT_FROM_WIN32(successCode: false, facilityCode, errorCode);
			}
		}

		public static int HRESULT_FROM_WIN32(bool successCode, short facilityCode, short errorCode)
		{
			short num = 23638;
			short num2 = num;
			num = 23638;
			int result = default(int);
			int num3 = default(int);
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				switch (0)
				{
				case 0:
					goto IL_0084;
				}
				goto IL_006e;
			case false:
			case true:
				goto IL_0129;
				IL_0084:
				result = 0;
				num = 1;
				num3 = num;
				goto IL_006e;
				IL_006e:
				while (true)
				{
					switch (num3)
					{
					case 1:
						goto IL_00bb;
					case 3:
						goto IL_0129;
					case 2:
						num = 1;
						if (num == 0)
						{
						}
						goto case 0;
					case 0:
						return result;
					}
					break;
					IL_00bb:
					num = 0;
					_ = num;
					if (!successCode)
					{
						num = 3;
						num3 = num;
					}
					else
					{
						result = ((facilityCode & 0x4FFF) << 16) | errorCode;
						num = 0;
						num3 = num;
					}
				}
				goto IL_0084;
				IL_0129:
				result = int.MinValue | ((facilityCode & 0x4FFF) << 16) | errorCode;
				num = 2;
				num3 = num;
				goto IL_006e;
			}
		}

		public static void RemoveOtherComRegistrationVersions(Type typeToRegister)
		{
			int a_ = 9;
			string a_2 = "LTL26747HVLWRBQAVJQ042UXZZ4UDYISEO1CT3CKVWNC48378AIKTM";
			short num = 0;
			switch (num)
			{
			default:
			{
				num = 0;
				int num2 = num;
				a a = default(a);
				string[] subKeyNames = default(string[]);
				int num4 = default(int);
				b b = default(b);
				Version result = default(Version);
				while (true)
				{
					switch (num2)
					{
					case 0:
						num = 1;
						if (num != 0)
						{
						}
						switch (0)
						{
						default:
							goto end_IL_009b;
						case 0:
							break;
						}
						goto default;
					default:
					{
						if (typeToRegister == null)
						{
							num = 1;
							num2 = num;
							break;
						}
						GuidAttribute guidAttribute = typeToRegister.GetCustomAttributes(typeof(GuidAttribute), inherit: true)[0] as GuidAttribute;
						string keyPathname = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}\\InProcServer32", guidAttribute.Value);
						a = new a();
						a.a = OpenRegistryKey(keyPathname, openForWrite: true);
						num = 2;
						num2 = num;
						break;
					}
					case 2:
						num = 0;
						_ = num;
						try
						{
							num = 3;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 3:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (a.a != null)
									{
										num = 11;
										num2 = num;
										continue;
									}
									break;
								case 11:
									subKeyNames = a.a.GetSubKeyNames();
									num4 = 0;
									num = 0;
									num2 = num;
									continue;
								case 7:
									num4++;
									num = 9;
									num2 = num;
									continue;
								case 0:
								case 9:
									num = 5;
									num2 = num;
									continue;
								case 5:
									if (num4 >= subKeyNames.Length)
									{
										num = 10;
										num2 = num;
										continue;
									}
									b = new b();
									b.b = a;
									b.a = subKeyNames[num4];
									Version.TryParse(b.a, out result);
									num = 8;
									num2 = num;
									continue;
								case 6:
									num = 2;
									num2 = num;
									continue;
								case 2:
									if (!result.Equals(typeToRegister.Assembly.GetName().Version))
									{
										num = 4;
										num2 = num;
										continue;
									}
									goto case 7;
								case 8:
									if (result != null)
									{
										num = 6;
										num2 = num;
										continue;
									}
									goto case 7;
								case 4:
									DoNothrow(b.c);
									num = 7;
									num2 = num;
									continue;
								case 10:
									break;
								case 1:
									return;
								}
								num = 1;
								num2 = num;
							}
						}
						finally
						{
							num = 2;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 2:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (a.a != null)
									{
										num = 1;
										num2 = num;
										continue;
									}
									break;
								case 1:
									((IDisposable)a.a).Dispose();
									num = 0;
									num2 = num;
									continue;
								case 0:
									break;
								}
								break;
							}
						}
					case 1:
						{
							num = 20322;
							short num3 = num;
							num = 20322;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								throw new ArgumentNullException("typeToRegister");
							}
							goto case 0;
						}
						end_IL_009b:
						break;
					}
				}
			}
			}
		}

		static COM()
		{
			int a_ = 16;
			string a_2 = "3MAYZCJJS4XK2KJ2U9CS3356FBECAQDPFH2NAR5613XH1T3ANTM3AKIG";
			short num = 30796;
			short num2 = num;
			num = 30796;
			switch (num2 == num)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			NotYetInitialized = new Exception("Not Yet Initialized");
			E_FAIL = HRESULT_FROM_WIN32(0, 4005);
			E_ABORT = HRESULT_FROM_WIN32(0, 4004);
			E_ACCESSDENIED = HRESULT_FROM_WIN32(8, 5);
			E_NOTIMPL = HRESULT_FROM_WIN32(0, 4001);
			E_INVALIDARG = HRESULT_FROM_WIN32(8, 57);
		}
	}

	public static class GeneralWindowsInterop
	{
		public enum GetSystemMetrics_Id
		{
			SM_CYSCREEN = 1,
			SM_CXVSCROLL = 2,
			SM_CYHSCROLL = 3,
			SM_CYCAPTION = 4,
			SM_CXBORDER = 5,
			SM_CYBORDER = 6,
			SM_CXDLGFRAME = 7,
			SM_CYDLGFRAME = 8,
			SM_CYVTHUMB = 9,
			SM_CXHTHUMB = 10,
			SM_CXICON = 11,
			SM_CYICON = 12,
			SM_CXCURSOR = 13,
			SM_CYCURSOR = 14,
			SM_CYMENU = 15,
			SM_CXFULLSCREEN = 16,
			SM_CYFULLSCREEN = 17,
			SM_CYKANJIWINDOW = 18,
			SM_MOUSEPRESENT = 19,
			SM_CYVSCROLL = 20,
			SM_CXHSCROLL = 21,
			SM_DEBUG = 22,
			SM_SWAPBUTTON = 23,
			SM_RESERVED1 = 24,
			SM_RESERVED2 = 25,
			SM_RESERVED3 = 26,
			SM_RESERVED4 = 27,
			SM_CXMIN = 28,
			SM_CYMIN = 29,
			SM_CXSIZE = 30,
			SM_CYSIZE = 31,
			SM_CXFRAME = 32,
			SM_CYFRAME = 33,
			SM_CXMINTRACK = 34,
			SM_CYMINTRACK = 35,
			SM_CXDOUBLECLK = 36,
			SM_CYDOUBLECLK = 37,
			SM_CXICONSPACING = 38,
			SM_CYICONSPACING = 39,
			SM_MENUDROPALIGNMENT = 40,
			SM_PENWINDOWS = 41,
			SM_DBCSENABLED = 42,
			SM_CMOUSEBUTTONS = 43,
			SM_CXFIXEDFRAME = 7,
			SM_CYFIXEDFRAME = 8,
			SM_CXSIZEFRAME = 32,
			SM_CYSIZEFRAME = 33,
			SM_SECURE = 44,
			SM_CXEDGE = 45,
			SM_CYEDGE = 46,
			SM_CXMINSPACING = 47,
			SM_CYMINSPACING = 48,
			SM_CXSMICON = 49,
			SM_CYSMICON = 50,
			SM_CYSMCAPTION = 51,
			SM_CXSMSIZE = 52,
			SM_CYSMSIZE = 53,
			SM_CXMENUSIZE = 54,
			SM_CYMENUSIZE = 55,
			SM_ARRANGE = 56,
			SM_CXMINIMIZED = 57,
			SM_CYMINIMIZED = 58,
			SM_CXMAXTRACK = 59,
			SM_CYMAXTRACK = 60,
			SM_CXMAXIMIZED = 61,
			SM_CYMAXIMIZED = 62,
			SM_NETWORK = 63,
			SM_CLEANBOOT = 67,
			SM_CXDRAG = 68,
			SM_CYDRAG = 69,
			SM_CXMENUCHECK = 71,
			SM_CYMENUCHECK = 72,
			SM_SLOWMACHINE = 73,
			SM_MIDEASTENABLED = 74,
			SM_MOUSEWHEELPRESENT = 75,
			SM_XVIRTUALSCREEN = 76,
			SM_YVIRTUALSCREEN = 77,
			SM_CXVIRTUALSCREEN = 78,
			SM_CYVIRTUALSCREEN = 79,
			SM_CMONITORS = 80,
			SM_SAMEDISPLAYFORMAT = 81,
			SM_IMMENABLED = 82,
			SM_CXFOCUSBORDER = 83,
			SM_CYFOCUSBORDER = 84,
			SM_TABLETPC = 86,
			SM_MEDIACENTER = 87,
			SM_STARTER = 88,
			SM_SERVERR2 = 89,
			SM_MOUSEHORIZONTALWHEELPRESENT = 91,
			SM_CXPADDEDBORDER = 92,
			SM_DIGITIZER = 94,
			SM_MAXIMUMTOUCHES = 95,
			SM_CMETRICS_ORIGINAL = 76,
			SM_CMETRICS_2000 = 83,
			SM_CMETRICS_XP = 91,
			SM_CMETRICS_VISTA = 93,
			SM_CMETRICS = 97,
			SM_REMOTESESSION = 4096,
			SM_SHUTTINGDOWN = 8192,
			SM_REMOTECONTROL = 8193,
			SM_CARETBLINKINGENABLED = 8194
		}

		public enum AddressFamily
		{
			AF_UNSPEC = 0,
			AF_UNIX = 1,
			AF_INET = 2,
			AF_IMPLINK = 3,
			AF_PUP = 4,
			AF_CHAOS = 5,
			AF_NS = 6,
			AF_IPX = 6,
			AF_ISO = 7,
			AF_OSI = 7,
			AF_ECMA = 8,
			AF_DATAKIT = 9,
			AF_CCITT = 10,
			AF_SNA = 11,
			AF_DECnet = 12,
			AF_DLI = 13,
			AF_LAT = 14,
			AF_HYLINK = 15,
			AF_APPLETALK = 16,
			AF_NETBIOS = 17,
			AF_VOICEVIEW = 18,
			AF_FIREFOX = 19,
			AF_UNKNOWN1 = 20,
			AF_BAN = 21,
			AF_ATM = 22,
			AF_INET6 = 23,
			AF_CLUSTER = 24,
			AF_12844 = 25,
			AF_IRDA = 26,
			AF_NETDES = 28,
			AF_TCNPROCESS = 29,
			AF_TCNMESSAGE = 30,
			AF_ICLFXBM = 31,
			AF_BTH = 32,
			AF_MAX = 33
		}

		public enum MoveFileFlags
		{
			MOVEFILE_REPLACE_EXISTING = 1,
			MOVEFILE_COPY_ALLOWED = 2,
			MOVEFILE_DELAY_UNTIL_REBOOT = 4,
			MOVEFILE_WRITE_THROUGH = 8
		}

		public const int SM_SHUTTINGDOWN = 8192;

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(GetSystemMetrics_Id metricId);

		[DllImport("kernel32.dll")]
		public static extern bool MoveFileEx(string targetFilename, string newFilename, MoveFileFlags dwFlags);
	}

	public static class Net
	{
		private enum a
		{
			a = 0,
			b = 1,
			c = 3,
			d = 4
		}

		[StructLayout(LayoutKind.Sequential)]
		private class b
		{
			public int a;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string b;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string c;
		}

		[CompilerGenerated]
		private sealed class c
		{
			public ManagementObject a;

			internal void b()
			{
				int a_ = 15;
				string a_2 = "MQYORHW5XF3I7IBTI95V8VBGENMYU4T4DST1NVSDUTIWTVVWWK8KQA2VYD21";
				short num = 0;
				_ = num;
				num = -17754;
				short num2 = num;
				num = -17754;
				switch (num2 == num)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				a.InvokeMethod("Disable", (object[])null);
			}
		}

		[CompilerGenerated]
		private sealed class d
		{
			public ManagementObject a;

			internal void b()
			{
				int a_ = 3;
				string a_2 = "YTDJEN4K58LJP3R23PDWRZ9NS781S7OPL81I4I22BNRTBO7MGC";
				short num = 27096;
				short num2 = num;
				num = 27096;
				switch (num2 == num)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				a.InvokeMethod("Enable", (object[])null);
			}
		}

		private static SecurityProtocolType m_a;

		private static SecurityProtocolType m_b;

		public static bool supportsTls11
		{
			get
			{
				short num = -13358;
				short num2 = num;
				num = -13358;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					return Net.m_a != SecurityProtocolType.Ssl3;
				}
			}
		}

		public static bool supportsTls12
		{
			get
			{
				short num = -32020;
				short num2 = num;
				num = -32020;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					return Net.m_b != SecurityProtocolType.Ssl3;
				}
			}
		}

		public static SecurityProtocolType tls11Flag
		{
			get
			{
				short num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				num = -1885;
				short num2 = num;
				num = -1885;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					return Net.m_a;
				}
			}
		}

		public static SecurityProtocolType tls12Flag
		{
			get
			{
				short num = 0;
				_ = num;
				num = -22304;
				short num2 = num;
				num = -22304;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					return Net.m_b;
				}
			}
		}

		public static bool HasProxyServer(HttpWebRequest requestObject)
		{
			int a_ = 2;
			string a_2 = "F2ZKLF9LZEICE9306LC3EGSNDJ4ZEV4HQC1VBLLZI3I657BSJ";
			int num = default(int);
			bool result = default(bool);
			WebProxy webProxy = default(WebProxy);
			switch (0)
			{
			default:
			{
				FieldInfo field = default(FieldInfo);
				while (true)
				{
					switch (num)
					{
					case 5:
						if (requestObject.Proxy != null)
						{
							short num2 = 2;
							num = num2;
						}
						else
						{
							result = false;
							short num2 = 8;
							num = num2;
						}
						continue;
					case 1:
					case 6:
					{
						result = webProxy.Address != null;
						short num2 = 7;
						num = num2;
						continue;
					}
					case 2:
					{
						short num2 = 0;
						num = num2;
						continue;
					}
					case 0:
					{
						short num2;
						if (requestObject.Proxy is WebProxy)
						{
							webProxy = requestObject.Proxy as WebProxy;
							num2 = -4943;
							short num3 = num2;
							num2 = -4943;
							switch (num3 == num2)
							{
							case false:
							case true:
								break;
							default:
								num2 = 0;
								if (num2 != 0)
								{
								}
								num2 = 6;
								num = num2;
								continue;
							}
						}
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 9;
						num = num2;
						continue;
					}
					case 9:
					{
						field = requestObject.Proxy.GetType().GetField("webProxy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						short num2 = 3;
						num = num2;
						continue;
					}
					case 3:
						if (field != null)
						{
							short num2 = 4;
							num = num2;
							continue;
						}
						goto case 1;
					case 4:
					{
						short num2 = 0;
						_ = num2;
						webProxy = field.GetValue(requestObject.Proxy) as WebProxy;
						num2 = 1;
						num = num2;
						continue;
					}
					case 7:
					case 8:
						return result;
					}
					break;
				}
				goto case 0;
			}
			case 0:
			{
				webProxy = null;
				result = false;
				short num2 = 5;
				num = num2;
				goto default;
			}
			}
		}

		[Obsolete]
		public static WebProxy GetWinhttpProxy()
		{
			short num = 28565;
			short num2 = num;
			num = 28565;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				break;
			case true:
				break;
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			return DuoApi.GetWinhttpProxy();
		}

		public static void SetSystemNetLogging(bool enableSystemNet, bool enableHttpListener, bool enableSockets, bool enableWebSockets, bool enableCache, bool enableHttp, TextWriterTraceListener targetListener)
		{
			int a_ = 6;
			string a_2 = "N02Q04SM836CRJKQ9XE6RNTYV6UB3XVAL4IPIH7532EWTDOOTNQRKFBQ4";
			short num = 0;
			switch (num)
			{
			}
			num = 4;
			int num2 = num;
			Type type = default(Type);
			BindingFlags bindingAttr = default(BindingFlags);
			FieldInfo field = default(FieldInfo);
			while (true)
			{
				switch (num2)
				{
				case 4:
					switch (0)
					{
					default:
						continue;
					case 0:
						break;
					}
					goto default;
				default:
					if (enableSystemNet || enableHttpListener || enableSockets || enableWebSockets || enableCache || enableHttp)
					{
						num = 8;
						num2 = num;
						continue;
					}
					goto IL_03c1;
				case 0:
					throw new Exception();
				case 7:
					num = 14;
					num2 = num;
					continue;
				case 14:
				{
					num = 25333;
					short num3 = num;
					num = 25333;
					switch (num3 == num)
					{
					case false:
					case true:
						goto IL_0396;
					}
					num = 0;
					if (num != 0)
					{
					}
					if (targetListener != null)
					{
						num = 20;
						num2 = num;
						continue;
					}
					goto IL_0256;
				}
				case 19:
					num = 13;
					num2 = num;
					continue;
				case 13:
					if (!enableHttpListener)
					{
						num = 17;
						num2 = num;
						continue;
					}
					goto IL_0256;
				case 22:
					throw new MissingFieldException("System.Net.Logging.s_LoggingEnabled not found, cannot enable logging!");
				case 1:
					throw new ArgumentNullException("targetListener", "targetListener cannot be null when at least one trace method is enabled");
				case 9:
					if (type == null)
					{
						num = 0;
						num2 = num;
						continue;
					}
					num = 0;
					_ = num;
					num = 3;
					num2 = num;
					continue;
				case 18:
					type.GetMethod("InitializeLogging", bindingAttr).Invoke(null, null);
					num = 15;
					num2 = num;
					continue;
				case 15:
					num = 1;
					if (num == 0)
					{
					}
					break;
				case 21:
					num = 11;
					num2 = num;
					continue;
				case 11:
					if (enableHttp)
					{
						goto IL_0256;
					}
					goto IL_0396;
				case 6:
					if (!enableSystemNet)
					{
						num = 19;
						num2 = num;
						continue;
					}
					goto IL_0256;
				case 16:
					if (!(bool)type.GetField("s_LoggingInitialized", bindingAttr).GetValue(null))
					{
						num = 18;
						num2 = num;
						continue;
					}
					break;
				case 3:
					if (!(field == null))
					{
						num = 16;
						num2 = num;
					}
					else
					{
						num = 22;
						num2 = num;
					}
					continue;
				case 5:
					num = 24;
					num2 = num;
					continue;
				case 24:
					if (!enableWebSockets)
					{
						num = 23;
						num2 = num;
						continue;
					}
					goto IL_0256;
				case 8:
					num = 12;
					num2 = num;
					continue;
				case 12:
					if (targetListener == null)
					{
						num = 1;
						num2 = num;
						continue;
					}
					goto IL_03c1;
				case 17:
					num = 2;
					num2 = num;
					continue;
				case 2:
					if (!enableSockets)
					{
						num = 5;
						num2 = num;
						continue;
					}
					goto IL_0256;
				case 23:
					num = 10;
					num2 = num;
					continue;
				case 10:
					if (!enableCache)
					{
						num = 21;
						num2 = num;
						continue;
					}
					goto IL_0256;
				case 20:
					{
						throw new ArgumentException("targetListener must be NULL if all sources are disabled", "targetListener");
					}
					IL_0396:
					num = 7;
					num2 = num;
					continue;
					IL_03c1:
					num = 6;
					num2 = num;
					continue;
					IL_0256:
					bindingAttr = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
					type = typeof(WebRequest).Assembly.GetType("System.Net.Logging");
					field = type.GetField("s_LoggingEnabled", bindingAttr);
					num = 9;
					num2 = num;
					continue;
				}
				break;
			}
			TraceSource traceSource = (TraceSource)type.GetField("s_WebTraceSource", bindingAttr).GetValue(null);
			TraceSource traceSource2 = (TraceSource)type.GetField("s_HttpListenerTraceSource", bindingAttr).GetValue(null);
			TraceSource traceSource3 = (TraceSource)type.GetField("s_SocketsTraceSource", bindingAttr).GetValue(null);
			TraceSource traceSource4 = (TraceSource)type.GetField("s_CacheTraceSource", bindingAttr).GetValue(null);
			TraceSource traceSource5 = (TraceSource)type.GetField("s_WebSocketsTraceSource", bindingAttr).GetValue(null);
			TraceSource obj = (TraceSource)type.GetField("s_TraceSourceHttpName", bindingAttr).GetValue(null);
			traceSource.Switch.Level = SourceLevels.All;
			traceSource2.Listeners.Add(targetListener);
			traceSource2.Switch.Level = SourceLevels.All;
			traceSource3.Listeners.Add(targetListener);
			traceSource3.Switch.Level = SourceLevels.All;
			traceSource4.Listeners.Add(targetListener);
			traceSource4.Switch.Level = SourceLevels.All;
			traceSource5.Listeners.Add(targetListener);
			traceSource5.Switch.Level = SourceLevels.All;
			obj.Listeners.Add(targetListener);
			obj.Switch.Level = SourceLevels.All;
			field.SetValue(null, true);
			Trace.AutoFlush = true;
		}

		public static void EnableSystemNetLogging(string targetFilename)
		{
			short num = -44;
			short num2 = num;
			num = -44;
			switch (num2 == num)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			TextWriterTraceListener textWriterTraceListener = new TextWriterTraceListener(targetFilename);
			textWriterTraceListener.TraceOutputOptions = TraceOptions.DateTime;
			SetSystemNetLogging(enableSystemNet: true, enableHttpListener: true, enableSockets: true, enableWebSockets: true, enableCache: true, enableHttp: true, textWriterTraceListener);
		}

		public static List<string> DisableNetworkAdapters()
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Expected O, but got Unknown
			int a_ = 17;
			string a_2 = "R447LGQ1G5LLXFFBPAKBLOKJPHGLAACZOCNEGU6LZ3MFJCYBEXJ";
			short num = 0;
			switch (num)
			{
			default:
			{
				num = 0;
				_ = num;
				List<string> list = new List<string>();
				ManagementObjectEnumerator enumerator = new ManagementClass("Win32_NetworkAdapter").GetInstances().GetEnumerator();
				try
				{
					num = 9;
					int num2 = num;
					string item = default(string);
					c c = default(c);
					string arg = default(string);
					bool? flag = default(bool?);
					while (true)
					{
						switch (num2)
						{
						case 9:
							switch (0)
							{
							default:
								goto end_IL_00c7;
							case 0:
								break;
							}
							goto default;
						case 5:
							item = (string)((ManagementBaseObject)c.a).Properties["DeviceID"].Value;
							arg = ((ManagementBaseObject)c.a).Properties["Name"].Value.ToString();
							num = 0;
							num2 = num;
							break;
						case 0:
							num = 1;
							if (num != 0)
							{
							}
							if (DoNothrow(c.b) == null)
							{
								num = 8;
								num2 = num;
							}
							else
							{
								Console.WriteLine("Unable to disable {0}", arg);
								num = 2;
								num2 = num;
							}
							break;
						default:
							num = 6;
							num2 = num;
							break;
						case 6:
							if (!enumerator.MoveNext())
							{
								num = 7;
								num2 = num;
								break;
							}
							c = new c();
							c.a = (ManagementObject)enumerator.Current;
							flag = (bool?)((ManagementBaseObject)c.a).Properties["NetEnabled"].Value;
							num = 1;
							num2 = num;
							break;
						case 1:
							if (flag.GetValueOrDefault(false))
							{
								num = 5;
								num2 = num;
								break;
							}
							goto default;
						case 8:
						{
							num = 3935;
							short num3 = num;
							num = 3935;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								list.Add(item);
								Console.WriteLine("Disabled network adapter {0}", arg);
								num = 3;
								num2 = num;
								goto end_IL_00c7;
							}
							goto case 5;
						}
						case 7:
							num = 4;
							num2 = num;
							break;
						case 4:
							{
								return list;
							}
							end_IL_00c7:
							break;
						}
					}
				}
				finally
				{
					num = 2;
					int num2 = num;
					while (true)
					{
						switch (num2)
						{
						case 2:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (enumerator != null)
							{
								num = 1;
								num2 = num;
								continue;
							}
							break;
						case 1:
							((IDisposable)enumerator).Dispose();
							num = 0;
							num2 = num;
							continue;
						case 0:
							break;
						}
						break;
					}
				}
			}
			}
		}

		public static void EnableNetworkAdapters(List<string> adaptersToEnable)
		{
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected O, but got Unknown
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			int a_ = 10;
			string a_2 = "AMSPIDB920RQ10JVB0CJY4HGJT3NNUM33BC3EM3HDI38NRW2WA88UATLG5";
			short num = 0;
			switch (num)
			{
			}
			using (List<string>.Enumerator enumerator = adaptersToEnable.GetEnumerator())
			{
				num = 2;
				int num2 = num;
				ManagementObjectCollection val = default(ManagementObjectCollection);
				ManagementObject a = default(ManagementObject);
				string text = default(string);
				Exception ex = default(Exception);
				while (true)
				{
					switch (num2)
					{
					case 2:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					case 9:
					{
						ManagementObjectEnumerator enumerator2 = val.GetEnumerator();
						enumerator2.MoveNext();
						ref ManagementObject reference = ref a;
						ManagementBaseObject current2 = enumerator2.Current;
						reference = (ManagementObject)(object)((current2 is ManagementObject) ? current2 : null);
						text = ((ManagementBaseObject)a).Properties["Name"].Value.ToString();
						ex = DoNothrow(delegate
						{
							int a_3 = 3;
							string a_4 = "YTDJEN4K58LJP3R23PDWRZ9NS781S7OPL81I4I22BNRTBO7MGC";
							short num4 = 27096;
							short num5 = num4;
							num4 = 27096;
							switch (num5 == num4)
							{
							default:
								num4 = 1;
								if (num4 != 0)
								{
								}
								num4 = 0;
								if (num4 != 0)
								{
								}
								num4 = 0;
								_ = num4;
								a.InvokeMethod("Enable", (object[])null);
								break;
							}
						});
						num = 0;
						num2 = num;
						continue;
					}
					case 0:
						if (ex != null)
						{
							Console.WriteLine("Error {0} re-enabling adapter {1}:  {2}", ex.GetType().Name, text, ex.Message);
							num = 3;
							num2 = num;
							continue;
						}
						goto IL_0128;
					default:
						num = 5;
						num2 = num;
						continue;
					case 5:
					{
						if (!enumerator.MoveNext())
						{
							num = 6;
							num2 = num;
							continue;
						}
						string current = enumerator.Current;
						val = new ManagementObjectSearcher((ObjectQuery)new SelectQuery("Win32_NetworkAdapter", "DeviceID = " + current)).Get();
						num = 8;
						num2 = num;
						continue;
					}
					case 8:
						if (val.Count == 1)
						{
							num = 9;
							num2 = num;
							continue;
						}
						goto default;
					case 7:
						Console.WriteLine("Enabled network adapter {0}", text);
						num = 1;
						num2 = num;
						continue;
					case 6:
						num = 4;
						num2 = num;
						continue;
					case 4:
						{
							num = 1;
							if (num != 0)
							{
							}
							num = 9122;
							short num3 = num;
							num = 9122;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num == 0)
								{
								}
								goto end_IL_006d;
							}
							goto IL_0128;
						}
						IL_0128:
						num = 7;
						num2 = num;
						continue;
						end_IL_006d:
						break;
					}
					break;
				}
			}
			num = 0;
			_ = num;
		}

		public static void AddHostsEntry(string dnsName, IPAddress targetIp, string hostsEntryComment)
		{
			int a_ = 15;
			string a_2 = "1RDXH5K44I1RTMG0PYKAHKI6QUP2C95KJLN2JYFFXKRX7G04K8HT43";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				int num3 = default(int);
				List<string> list = default(List<string>);
				bool flag = default(bool);
				string path = default(string);
				switch (0)
				{
				default:
				{
					string[] array = default(string[]);
					while (true)
					{
						switch (num2)
						{
						case 4:
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (num3 >= list.Count)
							{
								num = 7;
								num2 = num;
								continue;
							}
							array = list[num3].Split(' ', '\t');
							num = 13;
							num2 = num;
							continue;
						case 5:
							num = 1;
							if (num == 0)
							{
							}
							goto case 0;
						case 11:
							num = 22;
							num2 = num;
							continue;
						case 22:
							if (array[1].EqualsNoCase(dnsName))
							{
								num = 21;
								num2 = num;
								continue;
							}
							goto case 9;
						case 2:
							num = 6;
							num2 = num;
							continue;
						case 6:
							if (hostsEntryComment != null)
							{
								num = 19;
								num2 = num;
							}
							else
							{
								list.Add(string.Format("{0} {1}", targetIp, dnsName));
								num = 3;
								num2 = num;
							}
							continue;
						case 16:
							num = 12;
							num2 = num;
							continue;
						case 12:
							if (array.Length >= 2)
							{
								num = 11;
								num2 = num;
								continue;
							}
							goto case 9;
						case 0:
						case 7:
							num = 14;
							num2 = num;
							continue;
						case 14:
							if (!flag)
							{
								num = 2;
								num2 = num;
								continue;
							}
							goto case 3;
						case 15:
							num = 0;
							_ = num;
							list[num3] = string.Format("{0} {1} #{2}", targetIp, dnsName, hostsEntryComment);
							num = 0;
							num2 = num;
							continue;
						case 21:
							flag = true;
							num = 9;
							num2 = num;
							continue;
						case 19:
							list.Add(string.Format("{0} {1} #{2}", targetIp, dnsName, hostsEntryComment));
							num = 17;
							num2 = num;
							continue;
						case 10:
						{
							num = -1194;
							short num4 = num;
							num = -1194;
							switch (num4 == num)
							{
							case false:
							case true:
								break;
							default:
								goto IL_04ad;
							}
							goto case 8;
						}
						case 9:
							num = 20;
							num2 = num;
							continue;
						case 20:
							if (!flag)
							{
								num3++;
								num = 10;
								num2 = num;
							}
							else
							{
								num = 18;
								num2 = num;
							}
							continue;
						case 13:
							if (array != null)
							{
								num = 16;
								num2 = num;
								continue;
							}
							goto case 9;
						case 18:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (hostsEntryComment == null)
							{
								list[num3] = string.Format("{0} {1}", targetIp, dnsName);
								num = 5;
								num2 = num;
							}
							else
							{
								num = 15;
								num2 = num;
							}
							continue;
						case 3:
						case 17:
							{
								File.WriteAllLines(path, list, new ASCIIEncoding());
								return;
							}
							IL_04ad:
							num = 0;
							if (num == 0)
							{
							}
							goto case 4;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
					list = new List<string>((string[])File.ReadAllLines(path).Clone());
					flag = false;
					num3 = 0;
					num = 4;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static void RemoveHostsEntry(string dnsName)
		{
			int a_ = 6;
			string a_2 = "7UERXIW8ZLGHLVSQXQ7NT0XC1M9TS52ROT3BGXJMJMGM7NBY0B5O7EUULYNCP";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				List<string> list = default(List<string>);
				int num3 = default(int);
				bool flag = default(bool);
				string path = default(string);
				switch (0)
				{
				default:
				{
					string[] array = default(string[]);
					while (true)
					{
						switch (num2)
						{
						case 11:
						{
							list.RemoveAt(num3);
							num = -30330;
							short num4 = num;
							num = -30330;
							switch (num4 == num)
							{
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 1;
								num2 = num;
								continue;
							case false:
							case true:
								break;
							}
							goto case 1;
						}
						case 2:
							num = 5;
							num2 = num;
							continue;
						case 5:
							if (flag)
							{
								num = 11;
								num2 = num;
							}
							else
							{
								num3++;
								num = 3;
								num2 = num;
							}
							continue;
						case 4:
							num = 6;
							num2 = num;
							continue;
						case 6:
							if (array.Length >= 2)
							{
								num = 13;
								num2 = num;
								continue;
							}
							goto case 2;
						case 3:
						case 10:
							num = 7;
							num2 = num;
							continue;
						case 7:
							if (num3 >= list.Count)
							{
								num = 12;
								num2 = num;
								continue;
							}
							array = list[num3].Split(' ', '\t');
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (array != null)
							{
								num = 4;
								num2 = num;
								continue;
							}
							goto case 2;
						case 9:
							flag = true;
							num = 2;
							num2 = num;
							continue;
						case 13:
							num = 1;
							if (num != 0)
							{
							}
							num = 0;
							num2 = num;
							continue;
						case 0:
							if (array[1].EqualsNoCase(dnsName))
							{
								num = 9;
								num2 = num;
								continue;
							}
							goto case 2;
						case 1:
						case 12:
							File.WriteAllLines(path, list, new ASCIIEncoding());
							return;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					num = 0;
					_ = num;
					path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
					list = new List<string>((string[])File.ReadAllLines(path).Clone());
					flag = false;
					num3 = 0;
					num = 10;
					num2 = num;
					goto default;
				}
			}
			}
		}

		static Net()
		{
			int a_ = 6;
			string a_2 = "PH0PQ2A0LFBU314VSBUBYXESXDBYLQ44COIKNPFVJECGQ6O9W74KM4";
			short num = 13710;
			short num2 = num;
			num = 13710;
			switch (num2 == num)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			Net.m_a = SecurityProtocolType.Ssl3;
			Net.m_b = SecurityProtocolType.Ssl3;
			Enum.TryParse<SecurityProtocolType>("Tls11", out Net.m_a);
			Enum.TryParse<SecurityProtocolType>("Tls12", out Net.m_b);
		}

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern bool WinHttpGetDefaultProxyConfiguration([In][Out] b A_0);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern int WinHttpOpen([MarshalAs(UnmanagedType.LPWStr)] string A_0, a A_1, [MarshalAs(UnmanagedType.LPWStr)] string A_2, [MarshalAs(UnmanagedType.LPWStr)] string A_3, int A_4);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern bool WinHttpCloseHandle(int A_0);
	}

	public class AbortableBackgroundWorker : BackgroundWorker
	{
		[CompilerGenerated]
		private Thread a;

		[CompilerGenerated]
		private string b;

		public Thread workerThread
		{
			[CompilerGenerated]
			get
			{
				short num = -2966;
				short num2 = num;
				num = -2966;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 1;
					if (num != 0)
					{
					}
					return a;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = 7289;
				short num2 = num;
				num = 7289;
				switch (num2 == num)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				a = value;
			}
		}

		public string name
		{
			[CompilerGenerated]
			get
			{
				short num = 27135;
				short num2 = num;
				num = 27135;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					return b;
				}
			}
			[CompilerGenerated]
			set
			{
				short num = -14904;
				short num2 = num;
				num = -14904;
				switch (num2 == num)
				{
				}
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				b = value;
			}
		}

		public void Abort()
		{
			short num = 0;
			int num2 = num;
			while (true)
			{
				switch (num2)
				{
				case 0:
					switch (0)
					{
					default:
						goto end_IL_002c;
					case 0:
						break;
					}
					goto default;
				default:
					if (workerThread != null)
					{
						num = 2;
						num2 = num;
						break;
					}
					goto case 1;
				case 2:
					workerThread.Abort();
					workerThread = null;
					num = 1;
					num2 = num;
					break;
				case 1:
					{
						num = -3796;
						short num3 = num;
						num = -3796;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							_ = num;
							num = 1;
							if (num != 0)
							{
							}
							num = 0;
							if (num == 0)
							{
							}
							return;
						}
						goto default;
					}
					end_IL_002c:
					break;
				}
			}
		}

		protected override void OnDoWork(DoWorkEventArgs e)
		{
			while (true)
			{
				Exception error = null;
				bool cancelled = false;
				workerThread = Thread.CurrentThread;
				try
				{
					workerThread.Name = name;
					base.OnDoWork(new DoWorkEventArgs(null));
				}
				catch (Exception ex)
				{
					if (ex is ThreadAbortException)
					{
						cancelled = true;
						e.Cancel = true;
						Thread.ResetAbort();
					}
					else
					{
						error = ex;
					}
				}
				short num = 1;
				if (num != 0)
				{
				}
				num = 2529;
				short num2 = num;
				num = 2529;
				switch (num2 == num)
				{
				case false:
				case true:
					continue;
				}
				num = 0;
				_ = num;
				num = 0;
				if (num != 0)
				{
				}
				OnRunWorkerCompleted(new RunWorkerCompletedEventArgs(null, error, cancelled));
				return;
			}
		}
	}

	public class TreeMap<T> where T : class
	{
		public enum ApplicabilityScope
		{
			None,
			ImmediateChildrenOnly,
			Subtree
		}

		private class a
		{
			public T a;

			public ApplicabilityScope b;

			public Dictionary<string, a> c;

			public a d;

			public a(T A_0, ApplicabilityScope A_1, a A_2)
			{
				a = A_0;
				b = A_1;
				d = A_2;
			}
		}

		private a m_a;

		public void AddObject(string path, T objectToStore, ApplicabilityScope applicationMode)
		{
			int a_ = 18;
			string a_2 = "C5UNBMIL27LUCI2RPLJERV2P4A0HTGVHPRMH7PYZP2DQXDGK8";
			short num = 0;
			int num2 = num;
			a a = default(a);
			string key = default(string);
			bool A_2 = default(bool);
			int num3 = default(int);
			string[] array = default(string[]);
			string[] A_ = default(string[]);
			switch (num2)
			{
			default:
				{
					switch (0)
					{
					case 0:
						goto IL_0089;
					}
					goto IL_0046;
				}
				IL_0046:
				while (true)
				{
					switch (num2)
					{
					case 4:
						if (objectToStore == null)
						{
							num = 4811;
							short num4 = num;
							num = 4811;
							switch (num4 == num)
							{
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 7;
								num2 = num;
								continue;
							case false:
							case true:
								break;
							}
							goto case 7;
						}
						num = 1;
						num2 = num;
						continue;
					case 14:
						a = a.c[key];
						num = 2;
						num2 = num;
						continue;
					case 1:
						if (!A_2)
						{
							num = 0;
							_ = num;
							num = 0;
							num2 = num;
							continue;
						}
						this.a = new a(objectToStore, applicationMode, null);
						return;
					case 8:
					case 11:
						num = 13;
						num2 = num;
						continue;
					case 13:
						if (num3 >= array.Length)
						{
							num = 12;
							num2 = num;
						}
						else
						{
							key = array[num3];
							num = 3;
							num2 = num;
						}
						continue;
					case 12:
						num = 1;
						if (num != 0)
						{
						}
						a.a = objectToStore;
						a.b = applicationMode;
						return;
					case 7:
						throw new ArgumentNullException("objectToStore");
					case 2:
					case 6:
					case 10:
						num3++;
						num = 8;
						num2 = num;
						continue;
					case 5:
						num = 9;
						num2 = num;
						continue;
					case 9:
						if (a.c.ContainsKey(key))
						{
							num = 14;
							num2 = num;
							continue;
						}
						a.c.Add(key, new a(null, ApplicabilityScope.None, a));
						a = a.c[key];
						num = 10;
						num2 = num;
						continue;
					case 0:
						array = A_;
						num3 = 0;
						num = 11;
						num2 = num;
						continue;
					case 3:
						if (a.c == null)
						{
							a.c = new Dictionary<string, a>(StringComparer.CurrentCultureIgnoreCase);
							a.c[key] = new a(null, ApplicabilityScope.None, a);
							a = a.c[key];
							num = 6;
							num2 = num;
						}
						else
						{
							num = 5;
							num2 = num;
						}
						continue;
					}
					break;
				}
				goto IL_0089;
				IL_0089:
				A_2 = false;
				a = this.a;
				this.a(path, out A_2, out A_);
				num = 4;
				num2 = num;
				goto IL_0046;
			}
		}

		public T FindClosestApplicableObject(string path)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				bool A_2 = default(bool);
				T result = default(T);
				int num3 = default(int);
				string[] A_ = default(string[]);
				a a2 = default(a);
				a a = default(a);
				switch (0)
				{
				default:
					while (true)
					{
						switch (num2)
						{
						case 10:
							if (!A_2)
							{
								num = 7;
								num2 = num;
							}
							else
							{
								result = this.a.a;
								num = 12;
								num2 = num;
							}
							continue;
						case 19:
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (num3 == A_.Length - 1)
							{
								num = 5;
								num2 = num;
								continue;
							}
							goto IL_067e;
						case 6:
						case 15:
							num = 0;
							num2 = num;
							continue;
						case 0:
							if (num3 >= A_.Length)
							{
								num = 29;
								num2 = num;
							}
							else
							{
								num = 13;
								num2 = num;
							}
							continue;
						case 14:
						case 26:
						case 28:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (a2 != null)
							{
								num = 0;
								_ = num;
								num = 9;
								num2 = num;
								continue;
							}
							goto case 12;
						case 25:
						{
							num = 24366;
							short num4 = num;
							num = 24366;
							switch (num4 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 21;
								num2 = num;
								continue;
							}
							goto case 14;
						}
						case 21:
							if (a.c.ContainsKey(A_[num3]))
							{
								num = 22;
								num2 = num;
								continue;
							}
							goto case 29;
						case 3:
						case 24:
						case 33:
							num = 4;
							num2 = num;
							continue;
						case 4:
							if (a != null)
							{
								num = 23;
								num2 = num;
								continue;
							}
							goto case 14;
						case 20:
							if (a.b == ApplicabilityScope.ImmediateChildrenOnly)
							{
								num = 19;
								num2 = num;
								continue;
							}
							goto IL_067e;
						case 29:
							num = 17;
							num2 = num;
							continue;
						case 17:
							if (num3 == A_.Length)
							{
								result = a.a;
								num = 32;
								num2 = num;
							}
							else
							{
								num = 11;
								num2 = num;
							}
							continue;
						case 9:
							num = 1;
							if (num != 0)
							{
							}
							result = a2.a;
							num = 27;
							num2 = num;
							continue;
						case 2:
							if (a.a == null)
							{
								a = a.d;
								num = 24;
								num2 = num;
							}
							else
							{
								num = 30;
								num2 = num;
							}
							continue;
						case 7:
							num3 = 0;
							num = 6;
							num2 = num;
							continue;
						case 22:
							a = a.c[A_[num3]];
							num3++;
							num = 15;
							num2 = num;
							continue;
						case 11:
							num = 33;
							num2 = num;
							continue;
						case 30:
							num = 31;
							num2 = num;
							continue;
						case 31:
							if (a.b != ApplicabilityScope.Subtree)
							{
								num = 20;
								num2 = num;
							}
							else
							{
								num = 16;
								num2 = num;
							}
							continue;
						case 23:
							num = 18;
							num2 = num;
							continue;
						case 18:
							if (a2 == null)
							{
								num = 2;
								num2 = num;
							}
							else
							{
								num = 26;
								num2 = num;
							}
							continue;
						case 16:
							a2 = a;
							num = 28;
							num2 = num;
							continue;
						case 13:
							if (a.c != null)
							{
								num = 25;
								num2 = num;
								continue;
							}
							goto case 29;
						case 5:
							a2 = a;
							num = 14;
							num2 = num;
							continue;
						case 12:
						case 27:
						case 32:
							{
								return result;
							}
							IL_067e:
							a = a.d;
							num = 3;
							num2 = num;
							continue;
						}
						break;
					}
					goto case 0;
				case 0:
					A_2 = false;
					num3 = 0;
					a = this.a;
					a2 = null;
					result = null;
					this.a(path, out A_2, out A_);
					num = 10;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public T FindExactPath(string path)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				bool A_2 = default(bool);
				T result = default(T);
				int num3 = default(int);
				string[] A_ = default(string[]);
				a a = default(a);
				switch (0)
				{
				default:
					while (true)
					{
						switch (num2)
						{
						case 7:
							if (A_2)
							{
								num = 0;
								_ = num;
								result = this.a.a;
								num = 2;
								num2 = num;
								continue;
							}
							goto IL_00bf;
						case 12:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (num3 == A_.Length)
							{
								num = 13;
								num2 = num;
								continue;
							}
							goto case 2;
						case 9:
							if (a.c != null)
							{
								num = -4038;
								short num4 = num;
								num = -4038;
								switch (num4 == num)
								{
								case false:
								case true:
									break;
								default:
									num = 0;
									if (num != 0)
									{
									}
									num = 8;
									num2 = num;
									continue;
								}
								goto IL_00bf;
							}
							goto IL_0140;
						case 13:
							result = a.a;
							num = 3;
							num2 = num;
							continue;
						case 14:
							A_ = path.Split(new char[1] { '/' });
							num3 = 0;
							num = 6;
							num2 = num;
							continue;
						case 0:
						case 6:
							num = 4;
							num2 = num;
							continue;
						case 4:
							if (num3 < A_.Length)
							{
								num = 9;
								num2 = num;
							}
							else
							{
								num = 12;
								num2 = num;
							}
							continue;
						case 10:
							a = a.c[A_[num3]];
							num = 5;
							num2 = num;
							continue;
						case 5:
							num = 1;
							if (num == 0)
							{
							}
							goto IL_0140;
						case 8:
							num = 11;
							num2 = num;
							continue;
						case 11:
							if (a.c.ContainsKey(A_[num3]))
							{
								num = 10;
								num2 = num;
								continue;
							}
							goto IL_0140;
						case 2:
						case 3:
							{
								return result;
							}
							IL_00bf:
							num = 14;
							num2 = num;
							continue;
							IL_0140:
							num3++;
							num = 0;
							num2 = num;
							continue;
						}
						break;
					}
					goto case 0;
				case 0:
					A_2 = false;
					num3 = 0;
					a = this.a;
					result = null;
					this.a(path, out A_2, out A_);
					num = 7;
					num2 = num;
					goto default;
				}
			}
			}
		}

		private void a(string A_0, out bool A_1, out string[] A_2)
		{
			int a_ = 5;
			string a_2 = "8XREYK10DIW4TP46OSMAFRM1N8HP1EYVVUZDGZZRJKH2J6MCT8LBOZP2NG";
			switch (0)
			{
			case 0:
				goto IL_0052;
			}
			goto IL_001c;
			IL_001c:
			List<string> list = default(List<string>);
			List<string>.Enumerator enumerator = default(List<string>.Enumerator);
			int num = default(int);
			int num3 = default(int);
			short num2;
			while (true)
			{
				switch (num)
				{
				case 5:
					if (A_0 == null)
					{
						num2 = 10;
						num = num2;
					}
					else
					{
						num2 = 11;
						num = num2;
					}
					continue;
				case 4:
					num2 = 6;
					num = num2;
					continue;
				case 6:
					if (A_0[A_0.Length - 1] != '/')
					{
						A_1 = false;
						list.AddRange(A_0.Split(new char[1] { '/' }));
						list.RemoveAt(0);
						A_2 = list.ToArray();
						enumerator = list.GetEnumerator();
						num2 = 7;
						num = num2;
					}
					else
					{
						num2 = 2;
						num = num2;
					}
					continue;
				case 8:
					throw new ArgumentException("path must have a value", "path");
				case 11:
					if (A_0.Length == 0)
					{
						num2 = 8;
						num = num2;
					}
					else
					{
						num2 = 3;
						num = num2;
					}
					continue;
				case 2:
					throw new ArgumentException("Final path component is empty.  No path components may be empty.", "path");
				case 0:
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = 0;
					_ = num2;
					return;
				case 1:
					throw new ArgumentException("path must start in /", "path");
				case 10:
					throw new ArgumentNullException("path");
				case 7:
					try
					{
						num2 = 3;
						num = num2;
						while (true)
						{
							switch (num)
							{
							case 3:
								switch (0)
								{
								default:
									goto end_IL_0309;
								case 0:
									break;
								}
								goto default;
							case 0:
							{
								num2 = -15159;
								short num4 = num2;
								num2 = -15159;
								switch (num4 == num2)
								{
								default:
									num2 = 0;
									if (num2 != 0)
									{
									}
									if (string.IsNullOrWhiteSpace(enumerator.Current))
									{
										num2 = 6;
										num = num2;
										goto end_IL_0309;
									}
									num3++;
									break;
								case false:
								case true:
									break;
								}
								num2 = 2;
								num = num2;
								break;
							}
							case 6:
								throw new ArgumentException(string.Format("path component {0} is empty or whitespace.  No path component can be empty/whitespace.", num3));
							default:
								num2 = 4;
								num = num2;
								break;
							case 4:
								if (enumerator.MoveNext())
								{
									num2 = 0;
									num = num2;
								}
								else
								{
									num2 = 1;
									num = num2;
								}
								break;
							case 1:
								num2 = 5;
								num = num2;
								break;
							case 5:
								return;
								end_IL_0309:
								break;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				case 9:
					if (!(A_0 != "/"))
					{
						A_1 = true;
						A_2 = null;
						num2 = 0;
						num = num2;
					}
					else
					{
						num2 = 4;
						num = num2;
					}
					continue;
				case 3:
					if (A_0[0] == '/')
					{
						num2 = 9;
						num = num2;
					}
					else
					{
						num2 = 1;
						num = num2;
					}
					continue;
				}
				break;
			}
			goto IL_0052;
			IL_0052:
			list = new List<string>();
			num3 = 0;
			num2 = 5;
			num = num2;
			goto IL_001c;
		}
	}

	public static class ActiveDirectory
	{
		private enum a
		{
			a,
			b,
			c,
			d
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private class b
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string a;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string b;

			public uint c;

			public Guid d;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string e;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string f;

			public uint g;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string h;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string i;
		}

		private enum c : uint
		{
			a = 1u,
			b = 0x10u,
			c = 0x20u,
			d = 0x40u,
			e = 0x80u,
			f = 0x100u,
			g = 0x200u,
			h = 0x400u,
			i = 0x800u,
			j = 0x1000u,
			k = 0x2000u,
			l = 0x4000u,
			m = 0x8000u,
			n = 0x10000u,
			o = 0x20000u,
			p = 0x40000u,
			q = 0x80000u,
			r = 0x100000u,
			s = 0x200000u,
			t = 0x40000000u,
			u = 0x80000000u
		}

		public static bool IsInDomain()
		{
			int num = default(int);
			IntPtr A_ = default(IntPtr);
			a A_2 = default(a);
			short num2;
			switch (0)
			{
			default:
				while (true)
				{
					switch (num)
					{
					case 2:
						if (A_ != IntPtr.Zero)
						{
							goto IL_005b;
						}
						goto IL_017a;
					case 0:
						NetApiBufferFree(A_);
						A_ = IntPtr.Zero;
						num2 = 1;
						num = num2;
						continue;
					case 1:
						{
							num2 = 0;
							_ = num2;
							goto IL_017a;
						}
						IL_017a:
						return A_2 == a.d;
					}
					break;
					IL_005b:
					num2 = -24331;
					short num3 = num2;
					num2 = -24331;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 0;
						num = num2;
						continue;
					}
					goto IL_002f;
				}
				goto case 0;
			case 0:
				{
					A_ = IntPtr.Zero;
					A_2 = a.a;
					NetGetJoinInformation(null, out A_, out A_2);
					goto IL_002f;
				}
				IL_002f:
				num2 = 2;
				num = num2;
				goto default;
			}
		}

		public static string GetMachineDomain()
		{
			short num = -19167;
			short num2 = num;
			num = -19167;
			IntPtr A_ = default(IntPtr);
			string result = default(string);
			a A_2 = default(a);
			int num3 = default(int);
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				switch (0)
				{
				case 0:
					goto IL_0095;
				}
				goto IL_006f;
			case false:
			case true:
				goto IL_00ce;
				IL_0095:
				A_ = IntPtr.Zero;
				result = null;
				A_2 = a.a;
				NetGetJoinInformation(null, out A_, out A_2);
				num = 2;
				num3 = num;
				goto IL_006f;
				IL_006f:
				while (true)
				{
					switch (num3)
					{
					case 2:
						if (A_2 == a.d)
						{
							goto IL_00ce;
						}
						goto case 6;
					case 6:
						num = 7;
						num3 = num;
						continue;
					case 7:
						if (A_ != IntPtr.Zero)
						{
							num = 1;
							num3 = num;
							continue;
						}
						goto case 5;
					case 0:
						result = Marshal.PtrToStringUni(A_);
						num = 6;
						num3 = num;
						continue;
					case 4:
						num = 3;
						num3 = num;
						continue;
					case 3:
						num = 0;
						_ = num;
						if (A_ != IntPtr.Zero)
						{
							num = 0;
							num3 = num;
							continue;
						}
						goto case 6;
					case 1:
						NetApiBufferFree(A_);
						A_ = IntPtr.Zero;
						num = 5;
						num3 = num;
						continue;
					case 5:
						return result;
					}
					break;
				}
				goto IL_0095;
				IL_00ce:
				num = 1;
				if (num != 0)
				{
				}
				num = 4;
				num3 = num;
				goto IL_006f;
			}
		}

		public static string GetDomainName_Netbios(string domainName)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				b b = default(b);
				string result = default(string);
				switch (0)
				{
				default:
				{
					IntPtr A_ = default(IntPtr);
					while (true)
					{
						switch (num2)
						{
						case 1:
						{
							if (DsGetDcNameW(null, domainName, out var _, null, c.u, out A_) == 0)
							{
								num = 2;
								num2 = num;
								continue;
							}
							goto IL_01b3;
						}
						case 2:
						{
							Marshal.PtrToStructure(A_, b);
							result = b.e;
							NetApiBufferFree(A_);
							A_ = IntPtr.Zero;
							num = -20818;
							short num3 = num;
							num = -20818;
							switch (num3 == num)
							{
							case false:
							case true:
								break;
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 0;
								num2 = num;
								continue;
							}
							goto case 1;
						}
						case 0:
							{
								num = 0;
								_ = num;
								num = 1;
								if (num == 0)
								{
								}
								goto IL_01b3;
							}
							IL_01b3:
							return result;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					b = new b();
					result = null;
					_ = COM.E_FAIL;
					num = 1;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static string GetDomainName_Dns(string domainName)
		{
			IntPtr A_ = default(IntPtr);
			b b = default(b);
			string result = default(string);
			while (true)
			{
				short num = 0;
				int num2 = num;
				switch (num2)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				switch (0)
				{
				default:
					while (true)
					{
						switch (num2)
						{
						case 2:
						{
							if (DsGetDcNameW(null, domainName, out var _, null, c.t, out A_) == 0)
							{
								goto IL_00bf;
							}
							goto case 0;
						}
						case 1:
							num = 0;
							_ = num;
							Marshal.PtrToStructure(A_, b);
							result = b.e;
							NetApiBufferFree(A_);
							A_ = IntPtr.Zero;
							num = 0;
							num2 = num;
							continue;
						case 0:
							return result;
						}
						break;
						IL_00bf:
						num = 14788;
						short num3 = num;
						num = 14788;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 1;
							num2 = num;
							continue;
						}
						goto end_IL_0064;
					}
					goto case 0;
				case 0:
					{
						b = new b();
						result = null;
						_ = COM.E_FAIL;
						num = 2;
						num2 = num;
						goto default;
					}
					end_IL_0064:
					break;
				}
			}
		}

		[DllImport("netapi32.dll")]
		private static extern int NetGetJoinInformation([MarshalAs(UnmanagedType.LPWStr)] string A_0, out IntPtr A_1, out a A_2);

		[DllImport("netapi32.dll")]
		private static extern int NetApiBufferFree(IntPtr A_0);

		[DllImport("netapi32.dll")]
		private static extern int DsGetDcNameW([MarshalAs(UnmanagedType.LPWStr)] string A_0, [MarshalAs(UnmanagedType.LPWStr)] string A_1, out Guid A_2, [MarshalAs(UnmanagedType.LPWStr)] string A_3, c A_4, out IntPtr A_5);
	}

	public struct OSVERSIONINFOEX
	{
		public uint dwOSVersionInfoSize;

		public uint dwMajorVersion;

		public uint dwMinorVersion;

		public uint dwBuildNumber;

		public uint dwPlatformId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szCSDVersion;

		public ushort wServicePackMajor;

		public ushort wServicePackMinor;

		public WindowsProductSuite wSuiteMask;

		public WindowsProductType wProductType;

		public byte wReserved;
	}

	[DataContract]
	public enum WindowsProductType : byte
	{
		[EnumMember]
		[Description("Workstation")]
		VER_NT_WORKSTATION = 1,
		[Description("DomainController")]
		[EnumMember]
		VER_NT_DOMAIN_CONTROLLER,
		[EnumMember]
		[Description("Server")]
		VER_NT_SERVER
	}

	[Flags]
	[DataContract]
	public enum WindowsProductSuite : ushort
	{
		[Description("BackOffice")]
		[EnumMember]
		VER_SUITE_BACKOFFICE = 4,
		[EnumMember]
		[Description("Blade")]
		VER_SUITE_BLADE = 0x400,
		[Description("Compute")]
		[EnumMember]
		VER_SUITE_COMPUTE_SERVER = 0x4000,
		[Description("DataCenter")]
		[EnumMember]
		VER_SUITE_DATACENTER = 0x80,
		[Description("Enterprise")]
		[EnumMember]
		VER_SUITE_ENTERPRISE = 2,
		[EnumMember]
		[Description("Embedded")]
		VER_SUITE_EMBEDDEDNT = 0x40,
		[EnumMember]
		[Description("Home")]
		VER_SUITE_PERSONAL = 0x200,
		[Description("RdSingle")]
		[EnumMember]
		VER_SUITE_SINGLEUSERTS = 0x100,
		[EnumMember]
		[Description("SmallBusiness")]
		VER_SUITE_SMALLBUSINESS = 1,
		[Description("SmallBusinessRestrictive")]
		[EnumMember]
		VER_SUITE_SMALLBUSINESS_RESTRICTED = 0x20,
		[EnumMember]
		[Description("Storage")]
		VER_SUITE_STORAGE_SERVER = 0x2000,
		[Description("Terminal")]
		[EnumMember]
		VER_SUITE_TERMINAL = 0x10,
		[Description("HomeServer")]
		[EnumMember]
		VER_SUITE_WH_SERVER = 0x8000
	}

	[Flags]
	public enum SetErrorMode_Mode
	{
		SEM_FAILCRITICALERRORS = 1,
		SEM_NOALIGNMENTFAULTEXCEPT = 4,
		SEM_NOGPFAULTERRORBOX = 2,
		SEM_NOOPENFILEERRORBOX = 0x8000,
		All = 0x8007
	}

	private enum a
	{
		a = 1,
		b,
		c,
		d,
		e,
		f,
		g,
		h,
		i,
		j,
		k,
		l,
		m,
		n,
		o,
		p,
		q,
		r,
		s,
		t,
		u,
		v,
		w,
		x,
		y,
		z,
		aa,
		ab,
		ac
	}

	private enum b
	{
		a = 1,
		b,
		c
	}

	public enum PortableExecutableHHeader_MachineType
	{
		Native = 0,
		I386 = 332,
		Itanium = 512,
		AmdX64 = 34404
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class <>c
	{
		public static readonly <>c <>9;

		public static Func<NetworkInterface, bool> <>9__22_1;

		public static Action <>9__68_0;

		static <>c()
		{
			short num = 14272;
			short num2 = num;
			num = 14272;
			switch (num2 == num)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			<>9 = new <>c();
		}

		internal bool a(NetworkInterface A_0)
		{
			if (A_0.OperationalStatus == OperationalStatus.Up)
			{
				short num = 0;
				_ = num;
				num = 13775;
				short num2 = num;
				num = 13775;
				switch (num2 == num)
				{
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num == 0)
					{
					}
					break;
				case false:
				case true:
					break;
				}
				return !A_0.IsReceiveOnly;
			}
			return false;
		}

		internal void a()
		{
			int a_ = 15;
			string a_2 = "SK2KCKUPE2P9ZI9MSS5QDPSD5800NZBTKH5XDGWEOWZTG521EHUAZV82H";
			short num = -12586;
			short num2 = num;
			num = -12586;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				throw new IOException("Format is not 64-bit");
			}
		}
	}

	[CompilerGenerated]
	private sealed class c
	{
		public bool a;

		public IPAddress b;

		internal void c()
		{
			int num = default(int);
			IPHostEntry hostEntry = default(IPHostEntry);
			d d = default(d);
			switch (0)
			{
			default:
				while (true)
				{
					short num3;
					short num2;
					switch (num)
					{
					case 7:
						if (!a)
						{
							num2 = 1;
							num = num2;
							continue;
						}
						goto IL_00e2;
					case 0:
						num2 = 2;
						num = num2;
						continue;
					case 2:
						if (hostEntry.AddressList.Length != 0)
						{
							num2 = 3;
							num = num2;
							continue;
						}
						return;
					case 5:
						if (b == null)
						{
							num2 = 0;
							num = num2;
							continue;
						}
						return;
					case 1:
						d.a = AddressFamily.InterNetwork;
						num2 = 6;
						num = num2;
						continue;
					case 6:
						num2 = 1;
						if (num2 == 0)
						{
						}
						goto IL_00e2;
					case 3:
						b = hostEntry.AddressList[0];
						num2 = 4;
						num = num2;
						continue;
					case 4:
						{
							num2 = 0;
							_ = num2;
							return;
						}
						IL_0122:
						num2 = 0;
						if (num2 != 0)
						{
						}
						b = hostEntry.AddressList.FirstOrDefault(d.b);
						num2 = 5;
						num = num2;
						continue;
						IL_00e2:
						num2 = -29269;
						num3 = num2;
						num2 = -29269;
						switch (num3 == num2)
						{
						case false:
						case true:
							break;
						default:
							goto IL_0122;
						}
						goto case 7;
					}
					break;
				}
				goto case 0;
			case 0:
			{
				d = new d();
				hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				d.a = AddressFamily.InterNetworkV6;
				short num2 = 7;
				num = num2;
				goto default;
			}
			}
		}
	}

	[CompilerGenerated]
	private sealed class d
	{
		public AddressFamily a;

		internal bool b(IPAddress A_0)
		{
			short num = -9667;
			short num2 = num;
			num = -9667;
			switch (num2 == num)
			{
			default:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				return A_0.AddressFamily == a;
			}
		}
	}

	[CompilerGenerated]
	private sealed class e
	{
		public string a;

		public IpType b;

		public HttpWebResponse c;

		public HttpWebResponse d;

		public HttpWebResponse e;

		public IPAddress f;

		public BindIPEndPoint g;

		public Action h;

		internal void j()
		{
			int a_ = 19;
			string a_2 = "TG68ZC6IBGGK00BGTSWHW7K9Q0R14TKSNUGJT3KORCNP0ZTXOZFNO6FN";
			short num = 0;
			switch (num)
			{
			default:
			{
				num = 16;
				int num2 = num;
				Action action = default(Action);
				Exception ex = default(Exception);
				HttpWebResponse httpWebResponse = default(HttpWebResponse);
				while (true)
				{
					Action action2;
					switch (num2)
					{
					case 16:
						switch (0)
						{
						default:
							goto end_IL_005e;
						case 0:
							break;
						}
						goto default;
					default:
						action2 = h;
						if (action2 == null)
						{
							action = (h = delegate
							{
								int a_3 = 7;
								string a_4 = "QUIAHK72DOSNA3YZ8H6H0Z43A8321KPOMNHDN0XY7TRLX6SA410CET";
								int num4 = default(int);
								HttpWebRequest httpWebRequest = default(HttpWebRequest);
								while (true)
								{
									switch (0)
									{
									default:
										while (true)
										{
											short num5;
											switch (num4)
											{
											case 6:
												if (!Net.HasProxyServer(httpWebRequest))
												{
													num5 = 2;
													num4 = num5;
													continue;
												}
												goto IL_0172;
											case 2:
												num5 = 0;
												_ = num5;
												httpWebRequest.Proxy = Net.GetWinhttpProxy();
												num5 = 4;
												num4 = num5;
												continue;
											case 4:
												num5 = 1;
												if (num5 == 0)
												{
												}
												goto IL_0172;
											case 0:
												goto IL_01ad;
											case 3:
												num5 = 5;
												num4 = num5;
												continue;
											case 5:
												httpWebRequest.ServicePoint.BindIPEndPointDelegate = delegate(ServicePoint A_0, IPEndPoint A_1, int A_2)
												{
													short num7 = 2;
													int num8 = num7;
													while (true)
													{
														num7 = 0;
														_ = num7;
														switch (num8)
														{
														case 2:
															switch (0)
															{
															default:
																goto end_IL_005b;
															case 0:
																break;
															}
															goto default;
														default:
															if (A_2 >= 1)
															{
																return (IPEndPoint)null;
															}
															num7 = 1;
															if (num7 != 0)
															{
															}
															num7 = 1;
															num8 = num7;
															break;
														case 3:
														{
															num7 = 12464;
															short num9 = num7;
															num7 = 12464;
															switch (num9 == num7)
															{
															case false:
															case true:
																break;
															default:
																num7 = 0;
																if (num7 != 0)
																{
																}
																return new IPEndPoint(IPAddress.Any, A_1.Port);
															}
															goto default;
														}
														case 1:
															num7 = 0;
															num8 = num7;
															break;
														case 0:
															{
																if (b != IpType.Ipv4)
																{
																	return new IPEndPoint(IPAddress.IPv6Any, A_1.Port);
																}
																num7 = 3;
																num8 = num7;
																break;
															}
															end_IL_005b:
															break;
														}
													}
												};
												num5 = 1;
												num4 = num5;
												continue;
											case 1:
												goto IL_02a9;
												IL_0172:
												httpWebRequest.ReadWriteTimeout = 2000;
												httpWebRequest.Timeout = httpWebRequest.ReadWriteTimeout;
												num5 = 0;
												num4 = num5;
												continue;
											}
											break;
											IL_01ad:
											num5 = -17213;
											short num6 = num5;
											num5 = -17213;
											switch (num6 == num5)
											{
											case false:
											case true:
												break;
											default:
												goto IL_01f3;
											}
											goto end_IL_0013;
											IL_01f3:
											num5 = 0;
											if (num5 != 0)
											{
											}
											if (b != 0)
											{
												num5 = 3;
												num4 = num5;
												continue;
											}
											goto IL_02a9;
											IL_02a9:
											c = (HttpWebResponse)httpWebRequest.GetResponse();
											return;
										}
										goto case 0;
									case 0:
										{
											httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://{0}", a));
											httpWebRequest.AllowAutoRedirect = false;
											httpWebRequest.Method = "HEAD";
											ServicePointManager.Expect100Continue = false;
											short num5 = 6;
											num4 = num5;
											goto default;
										}
										end_IL_0013:
										break;
									}
								}
							});
							num = 26;
							num2 = num;
							break;
						}
						goto IL_0571;
					case 17:
						num = 0;
						num2 = num;
						break;
					case 0:
						if (f != null)
						{
							num = 2;
							num2 = num;
							break;
						}
						return;
					case 19:
						if (f == null)
						{
							num = 21;
							num2 = num;
							break;
						}
						goto case 22;
					case 15:
						num = 27;
						num2 = num;
						break;
					case 27:
						if (c != null)
						{
							num = 24;
							num2 = num;
						}
						else
						{
							e = d;
							num = 8;
							num2 = num;
						}
						break;
					case 14:
						if (f.AddressFamily == AddressFamily.InterNetwork)
						{
							num = 9;
							num2 = num;
							break;
						}
						goto case 13;
					case 12:
					{
						WebException ex2 = ex as WebException;
						d = (HttpWebResponse)ex2.Response;
						num = 15;
						num2 = num;
						break;
					}
					case 22:
						num = 7;
						num2 = num;
						break;
					case 7:
						if (b != 0)
						{
							num = 17;
							num2 = num;
							break;
						}
						return;
					case 3:
						num = 0;
						_ = num;
						num = 20;
						num2 = num;
						break;
					case 20:
						if (f.AddressFamily != AddressFamily.InterNetworkV6)
						{
							num = 13;
							num2 = num;
							break;
						}
						return;
					case 24:
						e = c;
						num = 5;
						num2 = num;
						break;
					case 9:
					{
						num = -11498;
						short num3 = num;
						num = -11498;
						switch (num3 == num)
						{
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 10;
							num2 = num;
							goto end_IL_005e;
						case false:
						case true:
							break;
						}
						goto IL_06a0;
					}
					case 10:
						if (b == IpType.Ipv6)
						{
							num = 3;
							num2 = num;
							break;
						}
						return;
					case 13:
						f = null;
						Logger.globalLogger.WriteLine("IP {0} found but wrong family {1} != {2}", f, f.AddressFamily, b);
						num = 18;
						num2 = num;
						break;
					case 18:
						return;
					case 5:
					case 8:
						num = 4;
						num2 = num;
						break;
					case 4:
						if (e != null)
						{
							num = 6;
							num2 = num;
							break;
						}
						goto IL_0138;
					case 26:
						action2 = action;
						goto IL_0571;
					case 25:
						if (ex is WebException)
						{
							num = 12;
							num2 = num;
							break;
						}
						goto case 15;
					case 6:
						httpWebResponse = e;
						num = 11;
						num2 = num;
						break;
					case 2:
						num = 23;
						num2 = num;
						break;
					case 23:
						if (b == IpType.Ipv4)
						{
							num = 1;
							num2 = num;
							break;
						}
						goto case 9;
					case 1:
						num = 1;
						if (num != 0)
						{
						}
						num = 14;
						num2 = num;
						break;
					case 21:
						Logger.globalLogger.WriteLine("Using backup IP determination");
						f = GetInternetIpAddress();
						goto IL_06a0;
					case 11:
						{
							try
							{
								f = httpWebResponse.GetLocalIpAddress();
							}
							finally
							{
								num = 1;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 1:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (httpWebResponse != null)
										{
											num = 2;
											num2 = num;
											continue;
										}
										break;
									case 2:
										((IDisposable)httpWebResponse).Dispose();
										num = 0;
										num2 = num;
										continue;
									case 0:
										break;
									}
									break;
								}
							}
							goto IL_0138;
						}
						IL_0138:
						num = 19;
						num2 = num;
						break;
						IL_06a0:
						num = 22;
						num2 = num;
						break;
						IL_0571:
						ex = DoNothrow(action2);
						num = 25;
						num2 = num;
						break;
						end_IL_005e:
						break;
					}
				}
			}
			}
		}

		internal void i()
		{
			int a_ = 7;
			string a_2 = "QUIAHK72DOSNA3YZ8H6H0Z43A8321KPOMNHDN0XY7TRLX6SA410CET";
			int num = default(int);
			HttpWebRequest httpWebRequest = default(HttpWebRequest);
			while (true)
			{
				switch (0)
				{
				default:
					while (true)
					{
						short num2;
						switch (num)
						{
						case 6:
							if (!Net.HasProxyServer(httpWebRequest))
							{
								num2 = 2;
								num = num2;
								continue;
							}
							goto IL_0172;
						case 2:
							num2 = 0;
							_ = num2;
							httpWebRequest.Proxy = Net.GetWinhttpProxy();
							num2 = 4;
							num = num2;
							continue;
						case 4:
							num2 = 1;
							if (num2 == 0)
							{
							}
							goto IL_0172;
						case 0:
							goto IL_01ad;
						case 3:
							num2 = 5;
							num = num2;
							continue;
						case 5:
							httpWebRequest.ServicePoint.BindIPEndPointDelegate = delegate(ServicePoint A_0, IPEndPoint A_1, int A_2)
							{
								short num4 = 2;
								int num5 = num4;
								while (true)
								{
									num4 = 0;
									_ = num4;
									switch (num5)
									{
									case 2:
										switch (0)
										{
										default:
											goto end_IL_005b;
										case 0:
											break;
										}
										goto default;
									default:
										if (A_2 >= 1)
										{
											return (IPEndPoint)null;
										}
										num4 = 1;
										if (num4 != 0)
										{
										}
										num4 = 1;
										num5 = num4;
										break;
									case 3:
									{
										num4 = 12464;
										short num6 = num4;
										num4 = 12464;
										switch (num6 == num4)
										{
										case false:
										case true:
											break;
										default:
											num4 = 0;
											if (num4 != 0)
											{
											}
											return new IPEndPoint(IPAddress.Any, A_1.Port);
										}
										goto default;
									}
									case 1:
										num4 = 0;
										num5 = num4;
										break;
									case 0:
										{
											if (b != IpType.Ipv4)
											{
												return new IPEndPoint(IPAddress.IPv6Any, A_1.Port);
											}
											num4 = 3;
											num5 = num4;
											break;
										}
										end_IL_005b:
										break;
									}
								}
							};
							num2 = 1;
							num = num2;
							continue;
						case 1:
							goto IL_02a9;
							IL_0172:
							httpWebRequest.ReadWriteTimeout = 2000;
							httpWebRequest.Timeout = httpWebRequest.ReadWriteTimeout;
							num2 = 0;
							num = num2;
							continue;
						}
						break;
						IL_01ad:
						num2 = -17213;
						short num3 = num2;
						num2 = -17213;
						switch (num3 == num2)
						{
						case false:
						case true:
							break;
						default:
							goto IL_01f3;
						}
						goto end_IL_0013;
						IL_01f3:
						num2 = 0;
						if (num2 != 0)
						{
						}
						if (b != 0)
						{
							num2 = 3;
							num = num2;
							continue;
						}
						goto IL_02a9;
						IL_02a9:
						c = (HttpWebResponse)httpWebRequest.GetResponse();
						return;
					}
					goto case 0;
				case 0:
					{
						httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://{0}", a));
						httpWebRequest.AllowAutoRedirect = false;
						httpWebRequest.Method = "HEAD";
						ServicePointManager.Expect100Continue = false;
						short num2 = 6;
						num = num2;
						goto default;
					}
					end_IL_0013:
					break;
				}
			}
		}

		internal IPEndPoint i(ServicePoint A_0, IPEndPoint A_1, int A_2)
		{
			short num = 2;
			int num2 = num;
			while (true)
			{
				num = 0;
				_ = num;
				switch (num2)
				{
				case 2:
					switch (0)
					{
					default:
						goto end_IL_005b;
					case 0:
						break;
					}
					goto default;
				default:
					if (A_2 < 1)
					{
						num = 1;
						if (num != 0)
						{
						}
						num = 1;
						num2 = num;
						break;
					}
					return null;
				case 3:
				{
					num = 12464;
					short num3 = num;
					num = 12464;
					switch (num3 == num)
					{
					case false:
					case true:
						break;
					default:
						num = 0;
						if (num != 0)
						{
						}
						return new IPEndPoint(IPAddress.Any, A_1.Port);
					}
					goto default;
				}
				case 1:
					num = 0;
					num2 = num;
					break;
				case 0:
					{
						if (b != IpType.Ipv4)
						{
							return new IPEndPoint(IPAddress.IPv6Any, A_1.Port);
						}
						num = 3;
						num2 = num;
						break;
					}
					end_IL_005b:
					break;
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class f
	{
		public List<IPAddress> a;

		internal void b()
		{
			short num = 0;
			switch (num)
			{
			default:
			{
				num = 0;
				int num2 = num;
				IEnumerable<NetworkInterface> enumerable = default(IEnumerable<NetworkInterface>);
				IEnumerator<NetworkInterface> enumerator2 = default(IEnumerator<NetworkInterface>);
				NetworkInterface current = default(NetworkInterface);
				IEnumerator<UnicastIPAddressInformation> enumerator = default(IEnumerator<UnicastIPAddressInformation>);
				UnicastIPAddressInformation current2 = default(UnicastIPAddressInformation);
				while (true)
				{
					short num3;
					switch (num2)
					{
					case 0:
						switch (0)
						{
						default:
							goto end_IL_004b;
						case 0:
							break;
						}
						goto default;
					default:
						enumerable = NetworkInterface.GetAllNetworkInterfaces().Where(delegate(NetworkInterface A_0)
						{
							if (A_0.OperationalStatus == OperationalStatus.Up)
							{
								short num4 = 0;
								_ = num4;
								num4 = 13775;
								short num5 = num4;
								num4 = 13775;
								switch (num5 == num4)
								{
								default:
									num4 = 1;
									if (num4 != 0)
									{
									}
									num4 = 0;
									if (num4 == 0)
									{
									}
									break;
								case false:
								case true:
									break;
								}
								return !A_0.IsReceiveOnly;
							}
							return false;
						});
						num = 1;
						num2 = num;
						break;
					case 3:
						try
						{
							num = 3;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 3:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									num = 1;
									num2 = num;
									continue;
								case 1:
									if (!enumerator2.MoveNext())
									{
										num = 0;
										num2 = num;
										continue;
									}
									current = enumerator2.Current;
									enumerator = current.GetIPProperties().UnicastAddresses.GetEnumerator();
									num = 4;
									num2 = num;
									continue;
								case 4:
									try
									{
										num = 4;
										num2 = num;
										while (true)
										{
											switch (num2)
											{
											case 4:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											default:
												num = 3;
												num2 = num;
												continue;
											case 3:
												if (!enumerator.MoveNext())
												{
													num = 8;
													num2 = num;
												}
												else
												{
													current2 = enumerator.Current;
													num = 11;
													num2 = num;
												}
												continue;
											case 7:
												a.Add(current2.Address);
												num = 9;
												num2 = num;
												continue;
											case 10:
												a.Add(current2.Address);
												num = 6;
												num2 = num;
												continue;
											case 11:
												if (current2.Address.IsIPv6LinkLocal)
												{
													num = 10;
													num2 = num;
												}
												else
												{
													num = 2;
													num2 = num;
												}
												continue;
											case 2:
												if (current2.Address.AddressFamily == AddressFamily.InterNetwork)
												{
													num = 0;
													num2 = num;
													continue;
												}
												goto default;
											case 0:
												num = 5;
												num2 = num;
												continue;
											case 5:
												if (current.NetworkInterfaceType == NetworkInterfaceType.Loopback)
												{
													num = 7;
													num2 = num;
													continue;
												}
												goto default;
											case 8:
												num = 1;
												num2 = num;
												continue;
											case 1:
												break;
											}
											break;
										}
									}
									finally
									{
										num = 2;
										num2 = num;
										while (true)
										{
											switch (num2)
											{
											case 2:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											default:
												if (enumerator != null)
												{
													num = 0;
													num2 = num;
													continue;
												}
												break;
											case 0:
												enumerator.Dispose();
												num = 1;
												num2 = num;
												continue;
											case 1:
												break;
											}
											break;
										}
									}
									goto default;
								case 0:
									num = 2;
									num2 = num;
									continue;
								case 2:
									break;
								}
								break;
							}
						}
						finally
						{
							num = 0;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 0:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (enumerator2 != null)
									{
										num = 2;
										num2 = num;
										continue;
									}
									break;
								case 2:
									enumerator2.Dispose();
									num = 1;
									num2 = num;
									continue;
								case 1:
									num = 1;
									if (num == 0)
									{
									}
									break;
								}
								break;
							}
						}
						goto IL_0583;
					case 2:
						enumerator2 = enumerable.GetEnumerator();
						num = 3;
						num2 = num;
						break;
					case 1:
						{
							if (enumerable.Count() > 0)
							{
								num = 2;
								num2 = num;
								break;
							}
							goto IL_0583;
						}
						IL_0583:
						num = 31454;
						num3 = num;
						num = 31454;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							_ = num;
							num = 0;
							if (num == 0)
							{
							}
							return;
						}
						goto case 2;
						end_IL_004b:
						break;
					}
				}
			}
			}
		}
	}

	[CompilerGenerated]
	private sealed class g
	{
		public List<IPAddress> a;

		internal void b()
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				int num4 = default(int);
				NetworkInterface[] allNetworkInterfaces = default(NetworkInterface[]);
				switch (0)
				{
				default:
				{
					NetworkInterface networkInterface = default(NetworkInterface);
					IEnumerator<UnicastIPAddressInformation> enumerator = default(IEnumerator<UnicastIPAddressInformation>);
					UnicastIPAddressInformation current = default(UnicastIPAddressInformation);
					while (true)
					{
						switch (num2)
						{
						case 1:
							num = 0;
							_ = num;
							goto case 4;
						case 0:
							if (networkInterface.OperationalStatus == OperationalStatus.Up)
							{
								num = 3;
								num2 = num;
								continue;
							}
							goto IL_0085;
						case 4:
							num = 6;
							num2 = num;
							continue;
						case 6:
							if (num4 < allNetworkInterfaces.Length)
							{
								networkInterface = allNetworkInterfaces[num4];
								num = 0;
								num2 = num;
							}
							else
							{
								num = 5;
								num2 = num;
							}
							continue;
						case 5:
							return;
						case 3:
							enumerator = networkInterface.GetIPProperties().UnicastAddresses.GetEnumerator();
							num = 2;
							num2 = num;
							continue;
						case 2:
							{
								num = 1;
								if (num != 0)
								{
								}
								try
								{
									num = 7;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 7:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											num = 6;
											num2 = num;
											continue;
										case 6:
											if (!enumerator.MoveNext())
											{
												num = 3;
												num2 = num;
											}
											else
											{
												current = enumerator.Current;
												num = 8;
												num2 = num;
											}
											continue;
										case 4:
											num = 1;
											num2 = num;
											continue;
										case 1:
											if (current.Address.AddressFamily == AddressFamily.InterNetworkV6)
											{
												num = 0;
												num2 = num;
												continue;
											}
											goto default;
										case 0:
										{
											num = -10714;
											short num3 = num;
											num = -10714;
											switch (num3 == num)
											{
											default:
												num = 0;
												if (num == 0)
												{
												}
												break;
											case false:
											case true:
												goto end_IL_0117;
											}
											goto IL_02ba;
										}
										case 8:
											if (current.Address.AddressFamily != AddressFamily.InterNetwork)
											{
												num = 4;
												num2 = num;
												continue;
											}
											goto IL_02ba;
										case 3:
											num = 2;
											num2 = num;
											continue;
										case 2:
											break;
											IL_02ba:
											a.Add(current.Address);
											num = 5;
											num2 = num;
											continue;
											end_IL_0117:
											break;
										}
										break;
									}
								}
								finally
								{
									num = 1;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 1:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											if (enumerator != null)
											{
												num = 2;
												num2 = num;
												continue;
											}
											break;
										case 2:
											enumerator.Dispose();
											num = 0;
											num2 = num;
											continue;
										case 0:
											break;
										}
										break;
									}
								}
								goto IL_0085;
							}
							IL_0085:
							num4++;
							num = 1;
							num2 = num;
							continue;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					num4 = 0;
					num = 4;
					num2 = num;
					goto default;
				}
			}
			}
		}
	}

	[CompilerGenerated]
	private sealed class h
	{
		public RegistryKey a;

		public string b;

		public int c;

		internal int d()
		{
			short num = -7074;
			short num2 = num;
			num = -7074;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return (int)a.GetValue(b, c);
			}
		}
	}

	[CompilerGenerated]
	private sealed class i
	{
		public RegistryKey a;

		public string b;

		public uint c;

		internal uint d()
		{
			short num = 7588;
			short num2 = num;
			num = 7588;
			switch (num2 == num)
			{
			default:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 0;
				if (num != 0)
				{
				}
				return (uint)(int)a.GetValue(b, c);
			}
		}
	}

	[CompilerGenerated]
	private sealed class j
	{
		public RegistryKey a;

		public string b;

		public byte c;

		internal byte d()
		{
			short num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 9210;
			short num2 = num;
			num = 9210;
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				return (byte)a.GetValue(b, c);
			}
		}
	}

	[CompilerGenerated]
	private sealed class k<a>
	{
		public RegistryKey a;

		public string b;

		public a c;

		internal a d()
		{
			while (true)
			{
				Type underlyingType = Enum.GetUnderlyingType(typeof(a));
				object value = Convert.ChangeType(this.a.GetValue(b, c), underlyingType);
				if (!Enum.IsDefined(typeof(a), value))
				{
					break;
				}
				short num = 0;
				_ = num;
				num = -26233;
				short num2 = num;
				num = -26233;
				switch (num2 == num)
				{
				case false:
				case true:
					continue;
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return (a)Enum.ToObject(typeof(a), value);
			}
			return c;
		}
	}

	[CompilerGenerated]
	private sealed class l
	{
		public RegistryKey a;

		public string b;

		public string c;

		internal string d()
		{
			short num = 1;
			if (num != 0)
			{
			}
			num = -3711;
			short num2 = num;
			num = -3711;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				break;
			case true:
				break;
			}
			num = 0;
			if (num != 0)
			{
			}
			return (string)a.GetValue(b, c);
		}
	}

	[CompilerGenerated]
	private sealed class m<a>
	{
		public string a;

		public a b;

		internal void c()
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			short num = 0;
			switch (num)
			{
			}
			num = 10771;
			short num2 = num;
			num = 10771;
			switch (num2 == num)
			{
			case false:
			case true:
				return;
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				UnicodeEncoding encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
				StreamWriter streamWriter = new StreamWriter(memoryStream, encoding);
				try
				{
					streamWriter.Write(this.a);
					streamWriter.Flush();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					XmlReader xmlReader = XmlReader.Create(new StreamReader(memoryStream, encoding));
					try
					{
						DataContractSerializer val = new DataContractSerializer(typeof(a));
						b = (a)((XmlObjectSerializer)val).ReadObject(xmlReader);
					}
					finally
					{
						num = 2;
						int num3 = num;
						while (true)
						{
							switch (num3)
							{
							case 2:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (xmlReader != null)
								{
									num = 1;
									num3 = num;
									continue;
								}
								break;
							case 1:
								((IDisposable)xmlReader).Dispose();
								num = 0;
								num3 = num;
								continue;
							case 0:
								break;
							}
							break;
						}
					}
				}
				finally
				{
					num = 0;
					int num3 = num;
					while (true)
					{
						switch (num3)
						{
						case 0:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (streamWriter != null)
							{
								num = 2;
								num3 = num;
								continue;
							}
							break;
						case 2:
							((IDisposable)streamWriter).Dispose();
							num = 1;
							num3 = num;
							continue;
						case 1:
							break;
						}
						break;
					}
				}
			}
			finally
			{
				num = 2;
				int num3 = num;
				while (true)
				{
					switch (num3)
					{
					case 2:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (memoryStream != null)
						{
							num = 1;
							num3 = num;
							continue;
						}
						break;
					case 1:
						((IDisposable)memoryStream).Dispose();
						num = 0;
						num3 = num;
						continue;
					case 0:
						break;
					}
					break;
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class n<a>
	{
		public string a;

		public a b;

		internal void c()
		{
			short num = 25473;
			short num2 = num;
			num = 25473;
			switch (num2 == num)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			this.a = DataContractSerializerSerialize(b);
		}
	}

	[CompilerGenerated]
	private sealed class o
	{
		public string a;

		public byte[] b;

		public PortableExecutableHHeader_MachineType c;

		internal void d()
		{
			while (true)
			{
				FileStream fileStream = new FileStream(a, FileMode.Open, FileAccess.Read);
				short num;
				try
				{
					fileStream.Read(b, 0, 4096);
				}
				finally
				{
					num = 2;
					int num2 = num;
					while (true)
					{
						switch (num2)
						{
						case 2:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (fileStream != null)
							{
								num = 1;
								num2 = num;
								continue;
							}
							break;
						case 1:
							((IDisposable)fileStream).Dispose();
							num = 0;
							num2 = num;
							continue;
						case 0:
							break;
						}
						break;
					}
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				num = 18265;
				short num3 = num;
				num = 18265;
				switch (num3 == num)
				{
				case false:
				case true:
					continue;
				}
				num = 0;
				if (num != 0)
				{
				}
				int num4 = BitConverter.ToInt32(b, 60);
				int num5 = BitConverter.ToUInt16(b, num4 + 4);
				c = (PortableExecutableHHeader_MachineType)num5;
				return;
			}
		}
	}

	public static readonly Exception NotYetInitialized;

	private static readonly Dictionary<string, RegistryHive> m_a;

	private const string m_b = "Eastern Standard Time";

	public static readonly Version ClrVersion_450;

	public static readonly Version ClrVersion_451;

	public static readonly Version ClrVersion_452;

	public static readonly Version ClrVersion_460;

	private static IntPtr m_c;

	public static string GetAssemblyQualifiedName(string namespaceQualifiedTypeName, string assemblyFilename)
	{
		int a_ = 0;
		string a_2 = "R3OXGO2C22U75BM8IHV6CNPTP196FVYAOKP3UNUISQ3DGDHMD53EN2ZJ685";
		short num = 8458;
		short num2 = num;
		num = 8458;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			string text = null;
			Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFilename);
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assemblyFilename);
			text = BitConverter.ToString(assembly.GetName().GetPublicKeyToken()).Replace("-", "");
			return GetAssemblyQualifiedName(namespaceQualifiedTypeName, assembly.GetName().Name, versionInfo.FileVersion, "neutral", text);
		}
		}
	}

	public static string GetAssemblyQualifiedName(string namespaceQualifiedTypeName, string assemblyTitle, string assemblyVersion, string assemblyCulture, string publicKeyToken)
	{
		int a_ = 14;
		string a_2 = "3WGX6NLIKS9L17N4S0JBV5T34IW9NLFDQUUPSVN6P1NJ7DV9Y6HC";
		switch (0)
		{
		case 0:
			goto IL_004e;
		}
		goto IL_001c;
		IL_001c:
		int num = default(int);
		string text = default(string);
		short num2;
		while (true)
		{
			switch (num)
			{
			case 1:
				if (string.IsNullOrWhiteSpace(namespaceQualifiedTypeName))
				{
					num2 = 2;
					num = num2;
				}
				else
				{
					num2 = 4;
					num = num2;
				}
				continue;
			case 6:
				throw new ArgumentNullException("publicKeyToken");
			case 5:
				throw new ArgumentNullException("assemblyVersion");
			case 4:
				num2 = 1;
				if (num2 != 0)
				{
				}
				if (string.IsNullOrWhiteSpace(assemblyTitle))
				{
					num2 = 0;
					_ = num2;
					num2 = 8;
					num = num2;
				}
				else
				{
					num2 = 7;
					num = num2;
				}
				continue;
			case 3:
				if (assemblyCulture != null)
				{
					goto IL_01c3;
				}
				goto IL_0339;
			case 2:
				throw new ArgumentNullException("namespaceQualifiedTypeName");
			case 0:
				if (!string.IsNullOrWhiteSpace(publicKeyToken))
				{
					num2 = 3;
					num = num2;
				}
				else
				{
					num2 = 6;
					num = num2;
				}
				continue;
			case 10:
				text = assemblyCulture;
				num2 = 9;
				num = num2;
				continue;
			case 9:
			{
				num2 = -6055;
				short num3 = num2;
				num2 = -6055;
				switch (num3 == num2)
				{
				case false:
				case true:
					break;
				default:
					goto IL_02b0;
				}
				goto IL_01c3;
			}
			case 7:
				if (!string.IsNullOrWhiteSpace(assemblyVersion))
				{
					num2 = 0;
					num = num2;
				}
				else
				{
					num2 = 5;
					num = num2;
				}
				continue;
			case 8:
				{
					throw new ArgumentNullException("assemblyTitle");
				}
				IL_02b0:
				num2 = 0;
				if (num2 == 0)
				{
				}
				goto IL_0339;
				IL_01c3:
				num2 = 10;
				num = num2;
				continue;
				IL_0339:
				return string.Format("{0}, {1}, Version={2}, Culture={3}, PublicKeyToken={4}", namespaceQualifiedTypeName, assemblyTitle, assemblyVersion, text, publicKeyToken);
			}
			break;
		}
		goto IL_004e;
		IL_004e:
		text = "neutral";
		num2 = 1;
		num = num2;
		goto IL_001c;
	}

	public static bool IsWindowsServerFeatureInstalled(WindowsServerFeatures serverFeatureId)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		int a_ = 18;
		string a_2 = "9RCMA1TD9JUXLGV06ET9MXMMPPEPPIY3I8RD2DJZA801DTLWPCQIGMY9YDBHR2S";
		bool result2 = default(bool);
		short num;
		try
		{
			ManagementObjectEnumerator enumerator = new ManagementClass("Win32_ServerFeature").GetInstances().GetEnumerator();
			try
			{
				num = 6;
				int num2 = num;
				int result = default(int);
				while (true)
				{
					switch (num2)
					{
					case 6:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					case 8:
						if (int.TryParse(((ManagementBaseObject)(ManagementObject)enumerator.Current)["ID"].ToString(), out result))
						{
							num = 4;
							num2 = num;
							continue;
						}
						goto default;
					default:
						num = 7;
						num2 = num;
						continue;
					case 7:
						if (enumerator.MoveNext())
						{
							num = 8;
							num2 = num;
						}
						else
						{
							num = 1;
							num2 = num;
						}
						continue;
					case 2:
						result2 = true;
						num = 3;
						num2 = num;
						continue;
					case 3:
						break;
					case 4:
						num = 5;
						num2 = num;
						continue;
					case 5:
						if (result == (int)serverFeatureId)
						{
							num = 2;
							num2 = num;
							continue;
						}
						goto default;
					case 1:
						num = 0;
						num2 = num;
						continue;
					case 0:
						goto IL_025d;
					}
					break;
				}
			}
			finally
			{
				num = 0;
				int num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 0:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (enumerator != null)
						{
							num = 1;
							num2 = num;
							continue;
						}
						break;
					case 1:
						((IDisposable)enumerator).Dispose();
						num = 2;
						num2 = num;
						continue;
					case 2:
						break;
					}
					break;
				}
			}
		}
		catch (Exception)
		{
			goto IL_025d;
		}
		num = 10024;
		short num3 = num;
		num = 10024;
		switch (num3 == num)
		{
		case false:
		case true:
			break;
		default:
			num = 0;
			_ = num;
			goto case true;
		case true:
			num = 0;
			if (num != 0)
			{
			}
			return result2;
		}
		goto IL_025d;
		IL_025d:
		num = 1;
		if (num != 0)
		{
		}
		return false;
	}

	public static IPAddress GetInternetIpAddress(bool preferIpV6 = false)
	{
		short num = 32377;
		short num2 = num;
		num = 32377;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			IPAddress b = null;
			DoNothrow(delegate
			{
				int num3 = default(int);
				IPHostEntry hostEntry = default(IPHostEntry);
				d d = default(d);
				switch (0)
				{
				default:
					while (true)
					{
						short num5;
						short num4;
						switch (num3)
						{
						case 7:
							if (!preferIpV6)
							{
								num4 = 1;
								num3 = num4;
								continue;
							}
							goto IL_00e2;
						case 0:
							num4 = 2;
							num3 = num4;
							continue;
						case 2:
							if (hostEntry.AddressList.Length != 0)
							{
								num4 = 3;
								num3 = num4;
								continue;
							}
							return;
						case 5:
							if (b == null)
							{
								num4 = 0;
								num3 = num4;
								continue;
							}
							return;
						case 1:
							d.a = AddressFamily.InterNetwork;
							num4 = 6;
							num3 = num4;
							continue;
						case 6:
							num4 = 1;
							if (num4 == 0)
							{
							}
							goto IL_00e2;
						case 3:
							b = hostEntry.AddressList[0];
							num4 = 4;
							num3 = num4;
							continue;
						case 4:
							{
								num4 = 0;
								_ = num4;
								return;
							}
							IL_0122:
							num4 = 0;
							if (num4 != 0)
							{
							}
							b = hostEntry.AddressList.FirstOrDefault(d.b);
							num4 = 5;
							num3 = num4;
							continue;
							IL_00e2:
							num4 = -29269;
							num5 = num4;
							num4 = -29269;
							switch (num5 == num4)
							{
							case false:
							case true:
								break;
							default:
								goto IL_0122;
							}
							goto case 7;
						}
						break;
					}
					goto case 0;
				case 0:
				{
					d = new d();
					hostEntry = Dns.GetHostEntry(Dns.GetHostName());
					d.a = AddressFamily.InterNetworkV6;
					short num4 = 7;
					num3 = num4;
					goto default;
				}
				}
			});
			return b;
		}
		}
	}

	public static IPAddress GetInternetIpAddress(string testHttpHostname)
	{
		short num = 12886;
		short num2 = num;
		num = 12886;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		IPAddress f = null;
		IpType b = IpType.Ipv4;
		HttpWebResponse c = null;
		HttpWebResponse d = null;
		HttpWebResponse e = null;
		Action h = default(Action);
		DoNothrow(delegate
		{
			int a_ = 19;
			string a_2 = "TG68ZC6IBGGK00BGTSWHW7K9Q0R14TKSNUGJT3KORCNP0ZTXOZFNO6FN";
			short num3 = 0;
			switch (num3)
			{
			default:
			{
				num3 = 16;
				int num4 = num3;
				Action action = default(Action);
				Exception ex = default(Exception);
				HttpWebResponse httpWebResponse = default(HttpWebResponse);
				while (true)
				{
					Action action2;
					switch (num4)
					{
					case 16:
						switch (0)
						{
						default:
							goto end_IL_005e;
						case 0:
							break;
						}
						goto default;
					default:
						action2 = h;
						if (action2 == null)
						{
							action = (h = delegate
							{
								int a_3 = 7;
								string a_4 = "QUIAHK72DOSNA3YZ8H6H0Z43A8321KPOMNHDN0XY7TRLX6SA410CET";
								int num6 = default(int);
								HttpWebRequest httpWebRequest = default(HttpWebRequest);
								while (true)
								{
									switch (0)
									{
									default:
										while (true)
										{
											short num7;
											switch (num6)
											{
											case 6:
												if (!Net.HasProxyServer(httpWebRequest))
												{
													num7 = 2;
													num6 = num7;
													continue;
												}
												goto IL_0172;
											case 2:
												num7 = 0;
												_ = num7;
												httpWebRequest.Proxy = Net.GetWinhttpProxy();
												num7 = 4;
												num6 = num7;
												continue;
											case 4:
												num7 = 1;
												if (num7 == 0)
												{
												}
												goto IL_0172;
											case 0:
												goto IL_01ad;
											case 3:
												num7 = 5;
												num6 = num7;
												continue;
											case 5:
												httpWebRequest.ServicePoint.BindIPEndPointDelegate = delegate(ServicePoint A_0, IPEndPoint A_1, int A_2)
												{
													short num9 = 2;
													int num10 = num9;
													while (true)
													{
														num9 = 0;
														_ = num9;
														switch (num10)
														{
														case 2:
															switch (0)
															{
															default:
																goto end_IL_005b;
															case 0:
																break;
															}
															goto default;
														default:
															if (A_2 >= 1)
															{
																return (IPEndPoint)null;
															}
															num9 = 1;
															if (num9 != 0)
															{
															}
															num9 = 1;
															num10 = num9;
															break;
														case 3:
														{
															num9 = 12464;
															short num11 = num9;
															num9 = 12464;
															switch (num11 == num9)
															{
															case false:
															case true:
																break;
															default:
																num9 = 0;
																if (num9 != 0)
																{
																}
																return new IPEndPoint(IPAddress.Any, A_1.Port);
															}
															goto default;
														}
														case 1:
															num9 = 0;
															num10 = num9;
															break;
														case 0:
															{
																if (b != IpType.Ipv4)
																{
																	return new IPEndPoint(IPAddress.IPv6Any, A_1.Port);
																}
																num9 = 3;
																num10 = num9;
																break;
															}
															end_IL_005b:
															break;
														}
													}
												};
												num7 = 1;
												num6 = num7;
												continue;
											case 1:
												goto IL_02a9;
												IL_0172:
												httpWebRequest.ReadWriteTimeout = 2000;
												httpWebRequest.Timeout = httpWebRequest.ReadWriteTimeout;
												num7 = 0;
												num6 = num7;
												continue;
											}
											break;
											IL_01ad:
											num7 = -17213;
											short num8 = num7;
											num7 = -17213;
											switch (num8 == num7)
											{
											case false:
											case true:
												break;
											default:
												goto IL_01f3;
											}
											goto end_IL_0013;
											IL_01f3:
											num7 = 0;
											if (num7 != 0)
											{
											}
											if (b != 0)
											{
												num7 = 3;
												num6 = num7;
												continue;
											}
											goto IL_02a9;
											IL_02a9:
											c = (HttpWebResponse)httpWebRequest.GetResponse();
											return;
										}
										goto case 0;
									case 0:
										{
											httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://{0}", testHttpHostname));
											httpWebRequest.AllowAutoRedirect = false;
											httpWebRequest.Method = "HEAD";
											ServicePointManager.Expect100Continue = false;
											short num7 = 6;
											num6 = num7;
											goto default;
										}
										end_IL_0013:
										break;
									}
								}
							});
							num3 = 26;
							num4 = num3;
							break;
						}
						goto IL_0571;
					case 17:
						num3 = 0;
						num4 = num3;
						break;
					case 0:
						if (f == null)
						{
							return;
						}
						num3 = 2;
						num4 = num3;
						break;
					case 19:
						if (f == null)
						{
							num3 = 21;
							num4 = num3;
							break;
						}
						goto case 22;
					case 15:
						num3 = 27;
						num4 = num3;
						break;
					case 27:
						if (c != null)
						{
							num3 = 24;
							num4 = num3;
						}
						else
						{
							e = d;
							num3 = 8;
							num4 = num3;
						}
						break;
					case 14:
						if (f.AddressFamily == AddressFamily.InterNetwork)
						{
							num3 = 9;
							num4 = num3;
							break;
						}
						goto case 13;
					case 12:
					{
						WebException ex2 = ex as WebException;
						d = (HttpWebResponse)ex2.Response;
						num3 = 15;
						num4 = num3;
						break;
					}
					case 22:
						num3 = 7;
						num4 = num3;
						break;
					case 7:
						if (b == IpType.Any)
						{
							return;
						}
						num3 = 17;
						num4 = num3;
						break;
					case 3:
						num3 = 0;
						_ = num3;
						num3 = 20;
						num4 = num3;
						break;
					case 20:
						if (f.AddressFamily == AddressFamily.InterNetworkV6)
						{
							return;
						}
						num3 = 13;
						num4 = num3;
						break;
					case 24:
						e = c;
						num3 = 5;
						num4 = num3;
						break;
					case 9:
					{
						num3 = -11498;
						short num5 = num3;
						num3 = -11498;
						switch (num5 == num3)
						{
						default:
							num3 = 0;
							if (num3 != 0)
							{
							}
							num3 = 10;
							num4 = num3;
							goto end_IL_005e;
						case false:
						case true:
							break;
						}
						goto IL_06a0;
					}
					case 10:
						if (b != IpType.Ipv6)
						{
							return;
						}
						num3 = 3;
						num4 = num3;
						break;
					case 13:
						f = null;
						Logger.globalLogger.WriteLine("IP {0} found but wrong family {1} != {2}", f, f.AddressFamily, b);
						num3 = 18;
						num4 = num3;
						break;
					case 18:
						return;
					case 5:
					case 8:
						num3 = 4;
						num4 = num3;
						break;
					case 4:
						if (e != null)
						{
							num3 = 6;
							num4 = num3;
							break;
						}
						goto IL_0138;
					case 26:
						action2 = action;
						goto IL_0571;
					case 25:
						if (ex is WebException)
						{
							num3 = 12;
							num4 = num3;
							break;
						}
						goto case 15;
					case 6:
						httpWebResponse = e;
						num3 = 11;
						num4 = num3;
						break;
					case 2:
						num3 = 23;
						num4 = num3;
						break;
					case 23:
						if (b == IpType.Ipv4)
						{
							num3 = 1;
							num4 = num3;
							break;
						}
						goto case 9;
					case 1:
						num3 = 1;
						if (num3 != 0)
						{
						}
						num3 = 14;
						num4 = num3;
						break;
					case 21:
						Logger.globalLogger.WriteLine("Using backup IP determination");
						f = GetInternetIpAddress();
						goto IL_06a0;
					case 11:
						{
							try
							{
								f = httpWebResponse.GetLocalIpAddress();
							}
							finally
							{
								num3 = 1;
								num4 = num3;
								while (true)
								{
									switch (num4)
									{
									case 1:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (httpWebResponse != null)
										{
											num3 = 2;
											num4 = num3;
											continue;
										}
										break;
									case 2:
										((IDisposable)httpWebResponse).Dispose();
										num3 = 0;
										num4 = num3;
										continue;
									case 0:
										break;
									}
									break;
								}
							}
							goto IL_0138;
						}
						IL_0138:
						num3 = 19;
						num4 = num3;
						break;
						IL_06a0:
						num3 = 22;
						num4 = num3;
						break;
						IL_0571:
						ex = DoNothrow(action2);
						num3 = 25;
						num4 = num3;
						break;
						end_IL_005e:
						break;
					}
				}
			}
			}
		});
		return f;
	}

	public static IPAddress[] GetAllLinkLocalIps()
	{
		short num = 0;
		_ = num;
		num = 3584;
		short num2 = num;
		num = 3584;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			List<IPAddress> a = new List<IPAddress>();
			DoNothrow(delegate
			{
				short num3 = 0;
				switch (num3)
				{
				default:
				{
					num3 = 0;
					int num4 = num3;
					IEnumerable<NetworkInterface> enumerable = default(IEnumerable<NetworkInterface>);
					IEnumerator<NetworkInterface> enumerator2 = default(IEnumerator<NetworkInterface>);
					NetworkInterface current = default(NetworkInterface);
					IEnumerator<UnicastIPAddressInformation> enumerator = default(IEnumerator<UnicastIPAddressInformation>);
					UnicastIPAddressInformation current2 = default(UnicastIPAddressInformation);
					while (true)
					{
						short num5;
						switch (num4)
						{
						case 0:
							switch (0)
							{
							default:
								goto end_IL_004b;
							case 0:
								break;
							}
							goto default;
						default:
							enumerable = NetworkInterface.GetAllNetworkInterfaces().Where(delegate(NetworkInterface A_0)
							{
								if (A_0.OperationalStatus == OperationalStatus.Up)
								{
									short num6 = 0;
									_ = num6;
									num6 = 13775;
									short num7 = num6;
									num6 = 13775;
									switch (num7 == num6)
									{
									default:
										num6 = 1;
										if (num6 != 0)
										{
										}
										num6 = 0;
										if (num6 == 0)
										{
										}
										break;
									case false:
									case true:
										break;
									}
									return !A_0.IsReceiveOnly;
								}
								return false;
							});
							num3 = 1;
							num4 = num3;
							break;
						case 3:
							try
							{
								num3 = 3;
								num4 = num3;
								while (true)
								{
									switch (num4)
									{
									case 3:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										num3 = 1;
										num4 = num3;
										continue;
									case 1:
										if (!enumerator2.MoveNext())
										{
											num3 = 0;
											num4 = num3;
										}
										else
										{
											current = enumerator2.Current;
											enumerator = current.GetIPProperties().UnicastAddresses.GetEnumerator();
											num3 = 4;
											num4 = num3;
										}
										continue;
									case 4:
										try
										{
											num3 = 4;
											num4 = num3;
											while (true)
											{
												switch (num4)
												{
												case 4:
													switch (0)
													{
													default:
														continue;
													case 0:
														break;
													}
													goto default;
												default:
													num3 = 3;
													num4 = num3;
													continue;
												case 3:
													if (!enumerator.MoveNext())
													{
														num3 = 8;
														num4 = num3;
													}
													else
													{
														current2 = enumerator.Current;
														num3 = 11;
														num4 = num3;
													}
													continue;
												case 7:
													a.Add(current2.Address);
													num3 = 9;
													num4 = num3;
													continue;
												case 10:
													a.Add(current2.Address);
													num3 = 6;
													num4 = num3;
													continue;
												case 11:
													if (current2.Address.IsIPv6LinkLocal)
													{
														num3 = 10;
														num4 = num3;
													}
													else
													{
														num3 = 2;
														num4 = num3;
													}
													continue;
												case 2:
													if (current2.Address.AddressFamily == AddressFamily.InterNetwork)
													{
														num3 = 0;
														num4 = num3;
														continue;
													}
													goto default;
												case 0:
													num3 = 5;
													num4 = num3;
													continue;
												case 5:
													if (current.NetworkInterfaceType == NetworkInterfaceType.Loopback)
													{
														num3 = 7;
														num4 = num3;
														continue;
													}
													goto default;
												case 8:
													num3 = 1;
													num4 = num3;
													continue;
												case 1:
													break;
												}
												break;
											}
										}
										finally
										{
											num3 = 2;
											num4 = num3;
											while (true)
											{
												switch (num4)
												{
												case 2:
													switch (0)
													{
													default:
														continue;
													case 0:
														break;
													}
													goto default;
												default:
													if (enumerator != null)
													{
														num3 = 0;
														num4 = num3;
														continue;
													}
													break;
												case 0:
													enumerator.Dispose();
													num3 = 1;
													num4 = num3;
													continue;
												case 1:
													break;
												}
												break;
											}
										}
										goto default;
									case 0:
										num3 = 2;
										num4 = num3;
										continue;
									case 2:
										break;
									}
									break;
								}
							}
							finally
							{
								num3 = 0;
								num4 = num3;
								while (true)
								{
									switch (num4)
									{
									case 0:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (enumerator2 != null)
										{
											num3 = 2;
											num4 = num3;
											continue;
										}
										break;
									case 2:
										enumerator2.Dispose();
										num3 = 1;
										num4 = num3;
										continue;
									case 1:
										num3 = 1;
										if (num3 == 0)
										{
										}
										break;
									}
									break;
								}
							}
							goto IL_0583;
						case 2:
							enumerator2 = enumerable.GetEnumerator();
							num3 = 3;
							num4 = num3;
							break;
						case 1:
							{
								if (enumerable.Count() > 0)
								{
									num3 = 2;
									num4 = num3;
									break;
								}
								goto IL_0583;
							}
							IL_0583:
							num3 = 31454;
							num5 = num3;
							num3 = 31454;
							switch (num5 == num3)
							{
							case false:
							case true:
								break;
							default:
								num3 = 0;
								_ = num3;
								num3 = 0;
								if (num3 == 0)
								{
								}
								return;
							}
							goto case 2;
							end_IL_004b:
							break;
						}
					}
				}
				}
			});
			return a.ToArray();
		}
		}
	}

	public static IPAddress[] GetAllIps()
	{
		short num = 0;
		_ = num;
		num = -28016;
		short num2 = num;
		num = -28016;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			List<IPAddress> a = new List<IPAddress>();
			DoNothrow(delegate
			{
				short num3 = 0;
				int num4 = num3;
				switch (num4)
				{
				default:
				{
					int num6 = default(int);
					NetworkInterface[] allNetworkInterfaces = default(NetworkInterface[]);
					switch (0)
					{
					default:
					{
						NetworkInterface networkInterface = default(NetworkInterface);
						IEnumerator<UnicastIPAddressInformation> enumerator = default(IEnumerator<UnicastIPAddressInformation>);
						UnicastIPAddressInformation current = default(UnicastIPAddressInformation);
						while (true)
						{
							switch (num4)
							{
							case 1:
								num3 = 0;
								_ = num3;
								goto case 4;
							case 0:
								if (networkInterface.OperationalStatus == OperationalStatus.Up)
								{
									num3 = 3;
									num4 = num3;
									continue;
								}
								goto IL_0085;
							case 4:
								num3 = 6;
								num4 = num3;
								continue;
							case 6:
								if (num6 < allNetworkInterfaces.Length)
								{
									networkInterface = allNetworkInterfaces[num6];
									num3 = 0;
									num4 = num3;
								}
								else
								{
									num3 = 5;
									num4 = num3;
								}
								continue;
							case 5:
								return;
							case 3:
								enumerator = networkInterface.GetIPProperties().UnicastAddresses.GetEnumerator();
								num3 = 2;
								num4 = num3;
								continue;
							case 2:
								{
									num3 = 1;
									if (num3 != 0)
									{
									}
									try
									{
										num3 = 7;
										num4 = num3;
										while (true)
										{
											switch (num4)
											{
											case 7:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											default:
												num3 = 6;
												num4 = num3;
												continue;
											case 6:
												if (!enumerator.MoveNext())
												{
													num3 = 3;
													num4 = num3;
												}
												else
												{
													current = enumerator.Current;
													num3 = 8;
													num4 = num3;
												}
												continue;
											case 4:
												num3 = 1;
												num4 = num3;
												continue;
											case 1:
												if (current.Address.AddressFamily == AddressFamily.InterNetworkV6)
												{
													num3 = 0;
													num4 = num3;
													continue;
												}
												goto default;
											case 0:
											{
												num3 = -10714;
												short num5 = num3;
												num3 = -10714;
												switch (num5 == num3)
												{
												default:
													num3 = 0;
													if (num3 == 0)
													{
													}
													break;
												case false:
												case true:
													goto end_IL_0117;
												}
												goto IL_02ba;
											}
											case 8:
												if (current.Address.AddressFamily != AddressFamily.InterNetwork)
												{
													num3 = 4;
													num4 = num3;
													continue;
												}
												goto IL_02ba;
											case 3:
												num3 = 2;
												num4 = num3;
												continue;
											case 2:
												break;
												IL_02ba:
												a.Add(current.Address);
												num3 = 5;
												num4 = num3;
												continue;
												end_IL_0117:
												break;
											}
											break;
										}
									}
									finally
									{
										num3 = 1;
										num4 = num3;
										while (true)
										{
											switch (num4)
											{
											case 1:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											default:
												if (enumerator != null)
												{
													num3 = 2;
													num4 = num3;
													continue;
												}
												break;
											case 2:
												enumerator.Dispose();
												num3 = 0;
												num4 = num3;
												continue;
											case 0:
												break;
											}
											break;
										}
									}
									goto IL_0085;
								}
								IL_0085:
								num6++;
								num3 = 1;
								num4 = num3;
								continue;
							}
							break;
						}
						goto case 0;
					}
					case 0:
						allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
						num6 = 0;
						num3 = 4;
						num4 = num3;
						goto default;
					}
				}
				}
			});
			return a.ToArray();
		}
		}
	}

	public static string GetFullyQualifiedDomainName()
	{
		int a_ = 14;
		string a_2 = "58HVI1JI8J0ABCQB2O9C9BWWTGVMSWEP6DY8BR9SWN7QJ9NR0I2";
		int num = default(int);
		string result = default(string);
		string hostName = default(string);
		string domainName = default(string);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 3:
					goto IL_0101;
				case 0:
				{
					short num2 = 1;
					if (num2 != 0)
					{
					}
					result = hostName;
					num2 = 2;
					num = num2;
					continue;
				}
				case 1:
				case 2:
					goto end_IL_0013;
				}
				break;
				IL_0101:
				if (hostName.EndsWith(domainName))
				{
					short num2 = 0;
					num = num2;
				}
				else
				{
					result = string.Format("{0}.{1}", hostName, domainName);
					short num2 = 1;
					num = num2;
				}
			}
			goto case 0;
		case 0:
			{
				short num2 = 31579;
				short num3 = num2;
				num2 = 31579;
				switch (num3 == num2)
				{
				case false:
				case true:
					goto end_IL_0013;
				}
				num2 = 0;
				if (num2 != 0)
				{
				}
				num2 = 0;
				_ = num2;
				domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
				hostName = Dns.GetHostName();
				result = null;
				num2 = 3;
				num = num2;
				goto default;
			}
			end_IL_0013:
			break;
		}
		return result;
	}

	public static Exception CloneException(Exception sourceException)
	{
		int num2 = default(int);
		while (true)
		{
			short num = 1;
			if (num != 0)
			{
			}
			ICloneable cloneable = null;
			Exception ex = null;
			try
			{
				switch (0)
				{
				default:
					while (true)
					{
						switch (num2)
						{
						case 6:
							if (cloneable != null)
							{
								num = 1;
								num2 = num;
								continue;
							}
							goto case 2;
						case 1:
							ex = (Exception)cloneable.Clone();
							num = 2;
							num2 = num;
							continue;
						case 4:
						{
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							MemoryStream memoryStream = new MemoryStream();
							binaryFormatter.Serialize(memoryStream, sourceException);
							memoryStream.Seek(0L, SeekOrigin.Begin);
							ex = (Exception)binaryFormatter.Deserialize(memoryStream);
							num = 0;
							num2 = num;
							continue;
						}
						case 2:
							num = 5;
							num2 = num;
							continue;
						case 5:
							if (ex == null)
							{
								num = 4;
								num2 = num;
								continue;
							}
							goto case 0;
						case 0:
							num = 3;
							num2 = num;
							continue;
						case 3:
							goto end_IL_0048;
						}
						goto case 0;
						continue;
						end_IL_0048:
						break;
					}
					break;
				case 0:
					cloneable = sourceException as ICloneable;
					num = 6;
					num2 = num;
					goto default;
				}
			}
			catch
			{
			}
			num = -12630;
			short num3 = num;
			num = -12630;
			switch (num3 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			return ex;
		}
	}

	public static void RethrowExceptionSameStackTrace(Exception exceptionToRetrow)
	{
		int a_ = 0;
		string a_2 = "012J3CI96ZP6IPHDHCYWWFDZOCBMPM06ITHEUKFQB0D9TPFAA67";
		short num = 13054;
		short num2 = num;
		num = 13054;
		switch (num2 == num)
		{
		default:
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(exceptionToRetrow, null);
			throw exceptionToRetrow;
		}
	}

	public static bool IsWebExceptionWithStatus(Exception exceptionToCheck, WebExceptionStatus targetWebExceptionStatus)
	{
		int num = default(int);
		WebException ex = default(WebException);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 1:
					num2 = 1;
					if (num2 != 0)
					{
					}
					if (ex != null)
					{
						num2 = 2;
						num = num2;
						continue;
					}
					goto IL_019f;
				case 4:
					result = true;
					num2 = 0;
					num = num2;
					continue;
				case 0:
				{
					num2 = -14488;
					short num3 = num2;
					num2 = -14488;
					switch (num3 == num2)
					{
					case false:
					case true:
						goto IL_0161;
					}
					num2 = 0;
					if (num2 == 0)
					{
					}
					goto IL_019f;
				}
				case 2:
					num2 = 3;
					num = num2;
					continue;
				case 3:
					{
						if (ex.Status == targetWebExceptionStatus)
						{
							goto IL_0161;
						}
						goto IL_019f;
					}
					IL_0161:
					num2 = 4;
					num = num2;
					continue;
					IL_019f:
					num2 = 0;
					_ = num2;
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			ex = exceptionToCheck as WebException;
			result = false;
			short num2 = 1;
			num = num2;
			goto default;
		}
		}
	}

	public static DateTime DuoUnixTimeToLocalDateTime(int duoUnixTime)
	{
		short num = -28047;
		short num2 = num;
		num = -28047;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(duoUnixTime).ToLocalTime();
		}
	}

	public static DateTime DuoUnixTimeToUtcDateTime(int duoUnixTime)
	{
		short num = 27930;
		short num2 = num;
		num = 27930;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(duoUnixTime).ToUniversalTime();
	}

	public static Exception DoNothrow<T>(Func<T> unsafeLogic, out T resultValue)
	{
		while (true)
		{
			short num = 0;
			_ = num;
			Exception notYetInitialized = NotYetInitialized;
			resultValue = default(T);
			try
			{
				num = 1;
				if (num != 0)
				{
				}
				resultValue = unsafeLogic();
				notYetInitialized = null;
			}
			catch (Exception ex)
			{
				notYetInitialized = ex;
			}
			num = -9759;
			short num2 = num;
			num = -9759;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			if (num != 0)
			{
			}
			return notYetInitialized;
		}
	}

	public static T DoNothrow<T>(Func<T> unsafeLogic, T defaultValue)
	{
		while (true)
		{
			T result = defaultValue;
			try
			{
				result = unsafeLogic();
			}
			catch (Exception)
			{
			}
			short num = 1;
			if (num != 0)
			{
			}
			num = -17930;
			short num2 = num;
			num = -17930;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			return result;
		}
	}

	public static Exception DoNothrow(Action unsafeLogic)
	{
		while (true)
		{
			Exception notYetInitialized = NotYetInitialized;
			try
			{
				unsafeLogic();
				notYetInitialized = null;
			}
			catch (Exception ex)
			{
				notYetInitialized = ex;
			}
			short num = -14880;
			short num2 = num;
			num = -14880;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			return notYetInitialized;
		}
	}

	public static bool ContainsNocase(this string sourceString, string stringToFind)
	{
		short num = 0;
		_ = num;
		num = -6570;
		short num2 = num;
		num = -6570;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			string value = stringToFind.ToUpper();
			return sourceString.ToUpper().Contains(value);
		}
		}
	}

	public static void SetBool(this RegistryKey sourceKey, string valueName, bool valueToStore, bool forceWrite = false)
	{
		while (true)
		{
			int num = (valueToStore ? 1 : 0);
			if (!(valueToStore || forceWrite))
			{
				break;
			}
			short num2 = 18835;
			short num3 = num2;
			num2 = 18835;
			switch (num3 == num2)
			{
			case false:
			case true:
				continue;
			default:
				num2 = 0;
				_ = num2;
				break;
			case true:
				break;
			}
			num2 = 1;
			if (num2 != 0)
			{
			}
			num2 = 0;
			if (num2 != 0)
			{
			}
			sourceKey.SetValue(valueName, num, RegistryValueKind.DWord);
			return;
		}
		sourceKey.DeleteValue(valueName, throwOnMissingValue: false);
	}

	public static void SetValueAnyString(this RegistryKey sourceKey, string valueName, string valueToStore)
	{
		while (string.IsNullOrEmpty(valueToStore))
		{
			short num = 0;
			_ = num;
			num = 9260;
			short num2 = num;
			num = 9260;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			sourceKey.DeleteValue(valueName, throwOnMissingValue: false);
			return;
		}
		sourceKey.SetValue(valueName, valueToStore, RegistryValueKind.String);
	}

	public static void SetValueUri(this RegistryKey sourceKey, string valueName, Uri valueToStore)
	{
		int num = default(int);
		string valueToStore2 = default(string);
		short num2;
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 0:
					goto IL_0111;
				case 2:
					valueToStore2 = valueToStore.ToString();
					num2 = 1;
					num = num2;
					continue;
				case 1:
					goto end_IL_0001;
				}
				break;
			}
			goto case 0;
		case 0:
			{
				num2 = 16808;
				short num3 = num2;
				num2 = 16808;
				switch (num3 == num2)
				{
				case false:
				case true:
					goto IL_0111;
				}
				num2 = 1;
				if (num2 != 0)
				{
				}
				num2 = 0;
				if (num2 != 0)
				{
				}
				num2 = 0;
				_ = num2;
				valueToStore2 = null;
				num2 = 0;
				num = num2;
				goto default;
			}
			IL_0111:
			if (!(valueToStore != null))
			{
				break;
			}
			num2 = 2;
			num = num2;
			goto default;
			end_IL_0001:
			break;
		}
		sourceKey.SetValueAnyString(valueName, valueToStore2);
	}

	public static int SafeReadDword(this RegistryKey sourceKey, string valueName, int defaultValue = 0)
	{
		short num = -6501;
		short num2 = num;
		num = -6501;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		return DoNothrow(delegate
		{
			short num3 = -7074;
			short num4 = num3;
			num3 = -7074;
			switch (num4 == num3)
			{
			default:
				num3 = 0;
				_ = num3;
				num3 = 1;
				if (num3 != 0)
				{
				}
				num3 = 0;
				if (num3 != 0)
				{
				}
				return (int)sourceKey.GetValue(valueName, defaultValue);
			}
		}, defaultValue);
	}

	public static uint SafeReadDword(this RegistryKey sourceKey, string valueName, uint defaultValue = 0u)
	{
		short num = -26901;
		short num2 = num;
		num = -26901;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		return DoNothrow(delegate
		{
			short num3 = 7588;
			short num4 = num3;
			num3 = 7588;
			switch (num4 == num3)
			{
			default:
				num3 = 1;
				if (num3 != 0)
				{
				}
				num3 = 0;
				_ = num3;
				num3 = 0;
				if (num3 != 0)
				{
				}
				return (uint)(int)sourceKey.GetValue(valueName, defaultValue);
			}
		}, defaultValue);
	}

	public static byte SafeReadDword(this RegistryKey sourceKey, string valueName, byte defaultValue = 0)
	{
		short num = 18225;
		short num2 = num;
		num = 18225;
		switch (num2 == num)
		{
		default:
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return DoNothrow(delegate
			{
				short num3 = 1;
				if (num3 != 0)
				{
				}
				num3 = 0;
				_ = num3;
				num3 = 9210;
				short num4 = num3;
				num3 = 9210;
				switch (num4 == num3)
				{
				default:
					num3 = 0;
					if (num3 != 0)
					{
					}
					return (byte)sourceKey.GetValue(valueName, defaultValue);
				}
			}, defaultValue);
		}
	}

	public static uint SafeReadDword(this RegistryKey sourceKey, string valueName, out bool valueExists)
	{
		short num = 1;
		if (num != 0)
		{
		}
		num = 0;
		_ = num;
		valueExists = false;
		uint result = 0u;
		try
		{
			int num2 = default(int);
			object value = default(object);
			switch (0)
			{
			default:
				while (true)
				{
					switch (num2)
					{
					case 0:
					{
						num = 10712;
						short num3 = num;
						num = 10712;
						switch (num3 == num)
						{
						case false:
						case true:
							goto IL_013d;
						}
						num = 0;
						if (num != 0)
						{
						}
						if (value != null)
						{
							num = 2;
							num2 = num;
							continue;
						}
						goto case 1;
					}
					case 2:
						goto IL_013d;
					case 1:
						num = 3;
						num2 = num;
						continue;
					case 3:
						goto end_IL_0079;
						IL_013d:
						result = (uint)(int)value;
						valueExists = true;
						num = 1;
						num2 = num;
						continue;
					}
					goto case 0;
					continue;
					end_IL_0079:
					break;
				}
				break;
			case 0:
				value = sourceKey.GetValue(valueName);
				num = 0;
				num2 = num;
				goto default;
			}
		}
		catch
		{
		}
		return result;
	}

	public static EnumType SafeReadEnum<EnumType>(this RegistryKey sourceKey, string valueName, EnumType defaultValue)
	{
		short num = 4460;
		short num2 = num;
		num = 4460;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			return DoNothrow(delegate
			{
				while (true)
				{
					Type underlyingType = Enum.GetUnderlyingType(typeof(EnumType));
					object value = Convert.ChangeType(sourceKey.GetValue(valueName, defaultValue), underlyingType);
					if (!Enum.IsDefined(typeof(EnumType), value))
					{
						break;
					}
					short num3 = 0;
					_ = num3;
					num3 = -26233;
					short num4 = num3;
					num3 = -26233;
					switch (num4 == num3)
					{
					case false:
					case true:
						break;
					default:
						num3 = 1;
						if (num3 != 0)
						{
						}
						num3 = 0;
						if (num3 != 0)
						{
						}
						return (EnumType)Enum.ToObject(typeof(EnumType), value);
					}
				}
				return defaultValue;
			}, defaultValue);
		}
	}

	public static string SafeReadString(this RegistryKey sourceKey, string valueName, string defaultValue = null)
	{
		short num = -30330;
		short num2 = num;
		num = -30330;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return DoNothrow(delegate
			{
				short num3 = 1;
				if (num3 != 0)
				{
				}
				num3 = -3711;
				short num4 = num3;
				num3 = -3711;
				switch (num4 == num3)
				{
				default:
					num3 = 0;
					_ = num3;
					break;
				case true:
					break;
				}
				num3 = 0;
				if (num3 != 0)
				{
				}
				return (string)sourceKey.GetValue(valueName, defaultValue);
			}, defaultValue);
		}
	}

	public static string SafeReadString(this RegistryKey sourceKey, string valueName, out bool valueExists, string defaultValue = null)
	{
		string result = defaultValue;
		valueExists = false;
		short num;
		try
		{
			num = 0;
			int num2 = num;
			while (true)
			{
				switch (num2)
				{
				case 0:
					switch (0)
					{
					default:
						continue;
					case 0:
						break;
					}
					goto default;
				default:
					if (sourceKey.GetValueKind(valueName) == RegistryValueKind.String)
					{
						num = 3;
						num2 = num;
						continue;
					}
					goto IL_00fb;
				case 3:
					result = (string)sourceKey.GetValue(valueName, defaultValue);
					valueExists = true;
					num = 2;
					num2 = num;
					continue;
				case 2:
				{
					num = 30279;
					short num3 = num;
					num = 30279;
					switch (num3 == num)
					{
					case false:
					case true:
						break;
					default:
						goto IL_00dc;
					}
					goto case 3;
				}
				case 1:
					{
						num = 1;
						if (num == 0)
						{
						}
						break;
					}
					IL_00dc:
					num = 0;
					if (num == 0)
					{
					}
					goto IL_00fb;
					IL_00fb:
					num = 1;
					num2 = num;
					continue;
				}
				break;
			}
		}
		catch
		{
		}
		num = 0;
		_ = num;
		return result;
	}

	public static bool SafeReadBoolean(this RegistryKey sourceKey, string valueName, bool defaultValue = false)
	{
		short num = 0;
		_ = num;
		num = 17796;
		short num2 = num;
		num = 17796;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			int defaultValue2 = (defaultValue ? 1 : 0);
			defaultValue2 = sourceKey.SafeReadDword(valueName, defaultValue2);
			return defaultValue2 != 0;
		}
		}
	}

	public static bool SafeReadBoolean(this RegistryKey sourceKey, string valueName, out bool valueExists, bool defaultValue = false)
	{
		short num = 1;
		if (num != 0)
		{
		}
		num = 0;
		_ = num;
		num = -8556;
		short num2 = num;
		num = -8556;
		bool result = default(bool);
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			result = defaultValue;
			valueExists = false;
			try
			{
				int num3 = default(int);
				object value = default(object);
				switch (0)
				{
				default:
					while (true)
					{
						switch (num3)
						{
						case 3:
							if (value is int)
							{
								num = 2;
								num3 = num;
								continue;
							}
							goto case 1;
						case 2:
							result = (int)value != 0;
							valueExists = true;
							num = 1;
							num3 = num;
							continue;
						case 1:
							num = 0;
							num3 = num;
							continue;
						case 0:
							goto end_IL_00f4;
						}
						goto case 0;
						continue;
						end_IL_00f4:
						break;
					}
					break;
				case 0:
					value = sourceKey.GetValue(valueName);
					num = 3;
					num3 = num;
					goto default;
				}
			}
			catch
			{
			}
			break;
		case false:
		case true:
			break;
		}
		return result;
	}

	public static Uri SafeReadUri(this RegistryKey sourceKey, string valueName, Uri defaultValue = null)
	{
		short num = -19350;
		short num2 = num;
		num = -19350;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		bool valueExists = false;
		return sourceKey.SafeReadUri(valueName, out valueExists, defaultValue);
	}

	public static Uri SafeReadUri(this RegistryKey sourceKey, string valueName, out bool valueExists, Uri defaultValue = null)
	{
		int num = default(int);
		Uri result = default(Uri);
		string defaultValue2 = default(string);
		switch (0)
		{
		default:
		{
			string text = default(string);
			while (true)
			{
				switch (num)
				{
				case 4:
					if (defaultValue != null)
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					goto case 0;
				case 1:
				{
					result = new Uri(text);
					short num2 = 5;
					num = num2;
					continue;
				}
				case 5:
				{
					short num2 = 0;
					_ = num2;
					goto IL_01e8;
				}
				case 0:
				{
					text = sourceKey.SafeReadString(valueName, out valueExists, defaultValue2);
					short num2 = 3;
					num = num2;
					continue;
				}
				case 3:
					if (text != null)
					{
						short num2 = 1;
						num = num2;
						continue;
					}
					goto IL_01e8;
				case 2:
					{
						defaultValue2 = defaultValue.ToString();
						short num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = -16527;
						short num3 = num2;
						num2 = -16527;
						switch (num3 == num2)
						{
						case false:
						case true:
							break;
						default:
							num2 = 0;
							if (num2 != 0)
							{
							}
							num2 = 0;
							num = num2;
							continue;
						}
						goto case 1;
					}
					IL_01e8:
					return result;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			defaultValue2 = null;
			result = null;
			valueExists = false;
			short num2 = 4;
			num = num2;
			goto default;
		}
		}
	}

	public static void SaveFlagsEnumAsSeparateValues(Enum valueToSave, string targetKeyPathname, string baseRegistryValueName, Enum specialAll, Enum specialNone)
	{
		int a_ = 12;
		string a_2 = "2V5PHY5UQ93WE2MHQ24BDPSD0ZDZTU6KMQMPHL4ZQY0UZUOT7ERCXM";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			List<Enum> list = default(List<Enum>);
			switch (0)
			{
			default:
			{
				RegistryKey registryKey = default(RegistryKey);
				RegistryKey sourceKey = default(RegistryKey);
				string valueName = default(string);
				Enum @enum = default(Enum);
				IEnumerator enumerator = default(IEnumerator);
				string arg = default(string);
				EnumMemberAttribute[] array = default(EnumMemberAttribute[]);
				IDisposable disposable = default(IDisposable);
				while (true)
				{
					switch (num2)
					{
					case 2:
						if (Attribute.IsDefined(valueToSave.GetType(), typeof(FlagsAttribute)))
						{
							list.Add(specialAll);
							list.Add(specialNone);
							registryKey = (sourceKey = OpenRegistryKey(targetKeyPathname, openForWrite: true));
							num = 1;
							num2 = num;
							continue;
						}
						goto IL_00cd;
					case 1:
					{
						try
						{
							num = 2;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 2:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (valueToSave != specialAll)
									{
										num = 4;
										num2 = num;
										continue;
									}
									break;
								case 4:
									num = 3;
									num2 = num;
									continue;
								case 3:
									if (valueToSave != specialNone)
									{
										num = 1;
										num2 = num;
										continue;
									}
									break;
								case 5:
									goto end_IL_0157;
								case 0:
									try
									{
										num = 5;
										num2 = num;
										while (true)
										{
											switch (num2)
											{
											case 5:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											case 12:
											case 15:
												sourceKey.SetBool(valueName, valueToSave.HasFlag(@enum));
												num = 13;
												num2 = num;
												continue;
											default:
												num = 6;
												num2 = num;
												continue;
											case 6:
												if (!enumerator.MoveNext())
												{
													num = 16;
													num2 = num;
												}
												else
												{
													@enum = (Enum)enumerator.Current;
													num = 14;
													num2 = num;
												}
												continue;
											case 7:
												arg = array[0].Value;
												num = 10;
												num2 = num;
												continue;
											case 14:
												if (!list.Contains(@enum))
												{
													num = 11;
													num2 = num;
													continue;
												}
												goto default;
											case 4:
												num = 9;
												num2 = num;
												continue;
											case 9:
												if (array.Length != 0)
												{
													num = 7;
													num2 = num;
													continue;
												}
												goto IL_046c;
											case 11:
												array = (EnumMemberAttribute[])valueToSave.GetType().GetField(@enum.ToString()).GetCustomAttributes(typeof(EnumMemberAttribute), inherit: true);
												num = 2;
												num2 = num;
												continue;
											case 2:
												if (array != null)
												{
													num = 4;
													num2 = num;
													continue;
												}
												goto IL_046c;
											case 8:
												valueName = string.Format("{0}_{1}", baseRegistryValueName, arg);
												num = 15;
												num2 = num;
												continue;
											case 1:
											case 10:
												num = 3;
												num2 = num;
												continue;
											case 3:
												if (baseRegistryValueName == null)
												{
													valueName = baseRegistryValueName;
													num = 12;
													num2 = num;
												}
												else
												{
													num = 8;
													num2 = num;
												}
												continue;
											case 16:
												num = 0;
												num2 = num;
												continue;
											case 0:
												break;
												IL_046c:
												arg = valueToSave.ToString();
												num = 1;
												num2 = num;
												continue;
											}
											break;
										}
									}
									finally
									{
										switch (0)
										{
										default:
											while (true)
											{
												switch (num2)
												{
												case 1:
													if (disposable != null)
													{
														num = 2;
														num2 = num;
														continue;
													}
													goto end_IL_0574;
												case 2:
													disposable.Dispose();
													num = 0;
													num2 = num;
													continue;
												case 0:
													goto end_IL_0574;
												}
												goto case 0;
												continue;
												end_IL_0574:
												break;
											}
											break;
										case 0:
											disposable = enumerator as IDisposable;
											num = 1;
											num2 = num;
											goto default;
										}
									}
									sourceKey.SetBool(baseRegistryValueName, valueToStore: false);
									num = 5;
									num2 = num;
									continue;
								case 1:
									enumerator = Enum.GetValues(valueToSave.GetType()).GetEnumerator();
									num = 0;
									num2 = num;
									continue;
								case 6:
									goto end_IL_0157;
								}
								sourceKey.SetBool(baseRegistryValueName, valueToSave == specialAll);
								num = 6;
								num2 = num;
								continue;
								end_IL_0157:
								break;
							}
						}
						finally
						{
							num = 1;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 1:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (registryKey != null)
									{
										num = 2;
										num2 = num;
										continue;
									}
									break;
								case 2:
									((IDisposable)registryKey).Dispose();
									num = 0;
									num2 = num;
									continue;
								case 0:
									break;
								}
								break;
							}
						}
						num = 20176;
						short num3 = num;
						num = 20176;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num == 0)
							{
							}
							return;
						}
						goto IL_00cd;
					}
					case 0:
						{
							throw new ArgumentException("T must be a Flags enumeration", "T");
						}
						IL_00cd:
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						num2 = num;
						continue;
					}
					break;
				}
				goto case 0;
			}
			case 0:
				num = 0;
				_ = num;
				list = new List<Enum>();
				num = 2;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static T ReadFlagsEnumFromSeparateValues<T>(string targetKeyPathname, string baseRegistryValueName, T specialAll, T specialNone, T defaultValue)
	{
		short num = 0;
		_ = num;
		T result = defaultValue;
		try
		{
			num = -330;
			short num2 = num;
			num = -330;
			switch (num2 == num)
			{
			default:
			{
				num = 0;
				if (num != 0)
				{
				}
				RegistryKey registryKey = OpenRegistryKey(targetKeyPathname, openForWrite: false);
				try
				{
					result = ReadFlagsEnumFromSeparateValues(registryKey, baseRegistryValueName, specialAll, specialNone, defaultValue);
				}
				finally
				{
					num = 1;
					int num3 = num;
					while (true)
					{
						switch (num3)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey != null)
							{
								num = 0;
								num3 = num;
								continue;
							}
							break;
						case 0:
							((IDisposable)registryKey).Dispose();
							num = 2;
							num3 = num;
							continue;
						case 2:
							break;
						}
						break;
					}
				}
				break;
			}
			case false:
			case true:
				break;
			}
		}
		catch
		{
		}
		num = 1;
		if (num != 0)
		{
		}
		return result;
	}

	public static T ReadFlagsEnumFromSeparateValues<T>(RegistryKey targetKey, string baseRegistryValueName, T specialAll, T specialNone, T defaultValue)
	{
		int a_ = 16;
		string a_2 = "MEOZYQ4PBPSHDOXB7Q844DXWS12MQ9AC8MFU9XEE47DEJ59Q";
		short num = 0;
		int num2 = num;
		bool valueExists = default(bool);
		IEnumerator enumerator = default(IEnumerator);
		Array values = default(Array);
		T result = default(T);
		ulong num4 = default(ulong);
		EnumMemberAttribute[] array = default(EnumMemberAttribute[]);
		Enum @enum = default(Enum);
		string arg = default(string);
		string valueName = default(string);
		List<Enum> list = default(List<Enum>);
		IDisposable disposable = default(IDisposable);
		bool flag = default(bool);
		switch (num2)
		{
		default:
			{
				switch (0)
				{
				case 0:
					goto IL_005b;
				}
				goto IL_0040;
			}
			IL_0040:
			while (true)
			{
				switch (num2)
				{
				case 3:
					if (!typeof(T).IsEnum)
					{
						num = 2;
						num2 = num;
					}
					else
					{
						num = 1;
						num2 = num;
					}
					continue;
				case 4:
					try
					{
						switch (0)
						{
						default:
							while (true)
							{
								switch (num2)
								{
								case 5:
									if (valueExists)
									{
										num = 10;
										num2 = num;
									}
									else
									{
										enumerator = values.GetEnumerator();
										num = 7;
										num2 = num;
									}
									continue;
								case 8:
									result = (T)Convert.ChangeType(num4, typeof(T).GetEnumUnderlyingType());
									num = 1;
									num2 = num;
									continue;
								case 7:
									try
									{
										num = 17;
										num2 = num;
										while (true)
										{
											switch (num2)
											{
											case 17:
												switch (0)
												{
												default:
													continue;
												case 0:
													break;
												}
												goto default;
											case 1:
												array = (EnumMemberAttribute[])typeof(T).GetField(@enum.ToString()).GetCustomAttributes(typeof(EnumMemberAttribute), inherit: true);
												num = 5;
												num2 = num;
												continue;
											case 5:
												if (array != null)
												{
													num = 13;
													num2 = num;
													continue;
												}
												goto IL_0363;
											case 0:
												arg = array[0].Value;
												num = 8;
												num2 = num;
												continue;
											default:
												num = 4;
												num2 = num;
												continue;
											case 4:
												if (!enumerator.MoveNext())
												{
													num = 10;
													num2 = num;
												}
												else
												{
													@enum = (Enum)enumerator.Current;
													num = 3;
													num2 = num;
												}
												continue;
											case 9:
											case 18:
												num = 6;
												num2 = num;
												continue;
											case 6:
												if (targetKey.SafeReadBoolean(valueName))
												{
													num = 2;
													num2 = num;
													continue;
												}
												goto default;
											case 8:
											case 14:
												num = 12;
												num2 = num;
												continue;
											case 12:
												if (baseRegistryValueName != null)
												{
													num = 15;
													num2 = num;
												}
												else
												{
													valueName = baseRegistryValueName;
													num = 18;
													num2 = num;
												}
												continue;
											case 13:
												num = 11;
												num2 = num;
												continue;
											case 11:
												if (array.Length != 0)
												{
													num = 0;
													num2 = num;
													continue;
												}
												goto IL_0363;
											case 3:
												if (!list.Contains(@enum))
												{
													num = 1;
													num2 = num;
													continue;
												}
												goto default;
											case 2:
												num4 |= Convert.ToUInt64(@enum);
												num = 16;
												num2 = num;
												continue;
											case 15:
												valueName = string.Format("{0}_{1}", baseRegistryValueName, arg);
												num = 9;
												num2 = num;
												continue;
											case 10:
												num = 7;
												num2 = num;
												continue;
											case 7:
												break;
												IL_0363:
												arg = result.ToString();
												num = 14;
												num2 = num;
												continue;
											}
											break;
										}
									}
									finally
									{
										switch (0)
										{
										default:
											while (true)
											{
												switch (num2)
												{
												case 2:
													if (disposable != null)
													{
														num = 0;
														num2 = num;
														continue;
													}
													goto end_IL_0558;
												case 0:
													disposable.Dispose();
													num = 1;
													num2 = num;
													continue;
												case 1:
													goto end_IL_0558;
												}
												goto case 0;
												continue;
												end_IL_0558:
												break;
											}
											break;
										case 0:
											disposable = enumerator as IDisposable;
											num = 2;
											num2 = num;
											goto default;
										}
									}
									num = 4;
									num2 = num;
									continue;
								case 3:
									result = specialAll;
									num = 0;
									num2 = num;
									continue;
								case 4:
									if (num4 != 0L)
									{
										num = 8;
										num2 = num;
										continue;
									}
									goto case 0;
								case 10:
									num = 9;
									num2 = num;
									continue;
								case 9:
									if (!flag)
									{
										result = specialNone;
										num = 6;
										num2 = num;
									}
									else
									{
										num = 3;
										num2 = num;
									}
									continue;
								case 0:
								case 1:
								case 6:
									num = 2;
									num2 = num;
									continue;
								case 2:
									goto end_IL_00d7;
								}
								goto case 0;
								continue;
								end_IL_00d7:
								break;
							}
							break;
						case 0:
							flag = targetKey.SafeReadBoolean(baseRegistryValueName, out valueExists);
							num = 5;
							num2 = num;
							goto default;
						}
					}
					catch
					{
					}
					return result;
				case 0:
					throw new ArgumentException("T must be a Flags enumeration", "T");
				case 2:
					throw new ArgumentException("T must be an enumeration", "T");
				case 1:
				{
					if (!Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
					{
						num = 0;
						num2 = num;
						continue;
					}
					list.Add((Enum)(object)specialAll);
					list.Add((Enum)(object)specialNone);
					values = Enum.GetValues(typeof(T));
					num = -24324;
					short num3 = num;
					num = -24324;
					switch (num3 == num)
					{
					case false:
					case true:
						continue;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 4;
					num2 = num;
					continue;
				}
				}
				break;
			}
			goto IL_005b;
			IL_005b:
			result = defaultValue;
			list = new List<Enum>();
			num4 = 0uL;
			valueExists = false;
			flag = false;
			_ = NotYetInitialized;
			num = 3;
			num2 = num;
			goto IL_0040;
		}
	}

	public static void DeleteRegistryKey(string keyPathname)
	{
		int a_ = 4;
		string a_2 = "HZPKS65MPOCTL5QE2X2U4500Y7QZZYBF3PZO6S1BOBCO1EG1NT16BAGH6Z";
		RegistryKey registryKey = default(RegistryKey);
		string subkeyPath = default(string);
		RegistryHive rootHive = default(RegistryHive);
		RegistryView registryView = default(RegistryView);
		while (true)
		{
			IL_0012:
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			switch (0)
			{
			case 0:
				goto IL_0084;
			}
			goto IL_0071;
			IL_0071:
			while (true)
			{
				switch (num2)
				{
				case 2:
					goto IL_00ac;
				case 0:
					goto IL_00d4;
				case 1:
					try
					{
						registryKey.DeleteSubKeyTree(subkeyPath, throwOnMissingSubKey: false);
					}
					finally
					{
						num = 2;
						num2 = num;
						while (true)
						{
							switch (num2)
							{
							case 2:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (registryKey != null)
								{
									num = 1;
									num2 = num;
									continue;
								}
								break;
							case 1:
								((IDisposable)registryKey).Dispose();
								num = 0;
								num2 = num;
								continue;
							case 0:
								break;
							}
							break;
						}
					}
					num = 0;
					_ = num;
					return;
				}
				break;
				IL_00d4:
				num = 26843;
				short num3 = num;
				num = 26843;
				switch (num3 == num)
				{
				case false:
				case true:
					break;
				default:
					num = 0;
					if (num != 0)
					{
					}
					registryKey = RegistryKey.OpenBaseKey(rootHive, registryView);
					num = 1;
					num2 = num;
					continue;
				}
				goto IL_0012;
				IL_00ac:
				if (subkeyPath != null)
				{
					num = 0;
					num2 = num;
					continue;
				}
				throw new ArgumentException("keyPathname must specify a subkey of a root hive");
			}
			goto IL_0084;
			IL_0084:
			ParseRegistryPath(keyPathname, recognizeViewSpecifier: true, out rootHive, out subkeyPath, out registryView);
			num = 2;
			num2 = num;
			goto IL_0071;
		}
	}

	public static RegistryKey OpenRegistryKey(string keyPathname, bool openForWrite, RegistryView view)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			string subkeyPath = default(string);
			RegistryKey registryKey2 = default(RegistryKey);
			RegistryHive rootHive = default(RegistryHive);
			switch (0)
			{
			default:
			{
				RegistryKey registryKey = default(RegistryKey);
				while (true)
				{
					switch (num2)
					{
					case 1:
						if (subkeyPath != null)
						{
							num = 2;
							num2 = num;
						}
						else
						{
							registryKey2 = RegistryKey.OpenBaseKey(rootHive, view);
							num = 0;
							num2 = num;
						}
						continue;
					case 3:
						try
						{
							switch (0)
							{
							default:
								while (true)
								{
									switch (num2)
									{
									case 3:
									{
										num = 27912;
										short num3 = num;
										num = 27912;
										switch (num3 == num)
										{
										case false:
										case true:
											continue;
										}
										num = 0;
										if (num != 0)
										{
										}
										if (openForWrite)
										{
											num = 2;
											num2 = num;
											continue;
										}
										goto case 5;
									}
									case 2:
										num = 1;
										num2 = num;
										continue;
									case 1:
										if (registryKey2 == null)
										{
											num = 4;
											num2 = num;
											continue;
										}
										goto case 5;
									case 4:
										registryKey.CreateSubKey(keyPathname);
										num = 5;
										num2 = num;
										continue;
									case 5:
										num = 0;
										num2 = num;
										continue;
									case 0:
										goto end_IL_00dc;
									}
									goto case 0;
									continue;
									end_IL_00dc:
									break;
								}
								break;
							case 0:
								registryKey2 = registryKey.OpenSubKey(keyPathname, openForWrite);
								num = 3;
								num2 = num;
								goto default;
							}
						}
						finally
						{
							num = 1;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 1:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (registryKey != null)
									{
										num = 0;
										num2 = num;
										continue;
									}
									break;
								case 0:
									((IDisposable)registryKey).Dispose();
									num = 2;
									num2 = num;
									continue;
								case 2:
									break;
								}
								break;
							}
						}
						goto IL_036c;
					case 2:
						registryKey = RegistryKey.OpenBaseKey(rootHive, view);
						num = 3;
						num2 = num;
						continue;
					case 0:
						{
							num = 1;
							if (num == 0)
							{
							}
							goto IL_036c;
						}
						IL_036c:
						return registryKey2;
					}
					break;
				}
				goto case 0;
			}
			case 0:
			{
				num = 0;
				_ = num;
				registryKey2 = null;
				ParseRegistryPath(keyPathname, recognizeViewSpecifier: false, out rootHive, out subkeyPath, out var _);
				num = 1;
				num2 = num;
				goto default;
			}
			}
		}
		}
	}

	public static RegistryKey OpenRegistryKey(string keyPathname, bool openForWrite)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			string subkeyPath = default(string);
			RegistryKey registryKey2 = default(RegistryKey);
			RegistryHive rootHive = default(RegistryHive);
			RegistryView registryView = default(RegistryView);
			switch (0)
			{
			default:
			{
				RegistryKey registryKey = default(RegistryKey);
				while (true)
				{
					switch (num2)
					{
					case 3:
						if (subkeyPath != null)
						{
							num = 0;
							num2 = num;
							continue;
						}
						registryKey2 = RegistryKey.OpenBaseKey(rootHive, registryView);
						num = 0;
						_ = num;
						num = 1;
						num2 = num;
						continue;
					case 2:
						try
						{
							switch (0)
							{
							default:
								while (true)
								{
									switch (num2)
									{
									case 5:
										if (openForWrite)
										{
											num = 1;
											num2 = num;
											continue;
										}
										goto case 3;
									case 1:
										num = 0;
										num2 = num;
										continue;
									case 0:
										if (registryKey2 == null)
										{
											num = 2;
											num2 = num;
											continue;
										}
										goto case 3;
									case 2:
										registryKey2 = registryKey.CreateSubKey(subkeyPath);
										num = 3;
										num2 = num;
										continue;
									case 3:
									{
										num = -29988;
										short num3 = num;
										num = -29988;
										switch (num3 == num)
										{
										default:
											num = 0;
											if (num != 0)
											{
											}
											num = 4;
											num2 = num;
											continue;
										case false:
										case true:
											break;
										}
										goto end_IL_00d8;
									}
									case 4:
										goto end_IL_00d8;
									}
									goto case 0;
									continue;
									end_IL_00d8:
									break;
								}
								break;
							case 0:
								registryKey2 = registryKey.OpenSubKey(subkeyPath, openForWrite);
								num = 5;
								num2 = num;
								goto default;
							}
						}
						finally
						{
							num = 2;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 2:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (registryKey != null)
									{
										num = 1;
										num2 = num;
										continue;
									}
									break;
								case 1:
									((IDisposable)registryKey).Dispose();
									num = 0;
									num2 = num;
									continue;
								case 0:
									break;
								}
								break;
							}
						}
						goto case 1;
					case 0:
						registryKey = RegistryKey.OpenBaseKey(rootHive, registryView);
						num = 2;
						num2 = num;
						continue;
					case 1:
						return registryKey2;
					}
					break;
				}
				goto case 0;
			}
			case 0:
				registryKey2 = null;
				ParseRegistryPath(keyPathname, recognizeViewSpecifier: true, out rootHive, out subkeyPath, out registryView);
				num = 3;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static void ParseRegistryPath(string keyPathname, bool recognizeViewSpecifier, out RegistryHive rootHive, out string subkeyPath, out RegistryView registryView)
	{
		int a_ = 5;
		string a_2 = "KNSP78OJ2SRL4VM4H50L7S7YST3CSNUNCVNB6Z1O6Z95X8TCNV7INYNQJAPHAY";
		short num = 0;
		int num2 = num;
		int num3 = default(int);
		string text3 = default(string);
		string text2 = default(string);
		string text = default(string);
		switch (num2)
		{
		default:
			{
				switch (0)
				{
				case 0:
					goto IL_009f;
				}
				goto IL_0040;
			}
			IL_0040:
			while (true)
			{
				switch (num2)
				{
				case 14:
					if (keyPathname == null)
					{
						num = 5;
						num2 = num;
						continue;
					}
					subkeyPath = null;
					num3 = keyPathname.IndexOf('\\');
					num = 3;
					num2 = num;
					continue;
				case 8:
					text3 = text2.Substring(0, text2.Length - 2);
					registryView = RegistryView.Registry64;
					num = 1;
					num2 = num;
					continue;
				case 21:
					text3 = text2.Substring(0, text2.Length - 2);
					registryView = RegistryView.Registry32;
					num = 19;
					num2 = num;
					continue;
				case 10:
					text2 = keyPathname.Substring(0, num3);
					num = 15;
					num2 = num;
					continue;
				case 15:
					if (num3 < keyPathname.Length - 1)
					{
						num = 7;
						num2 = num;
						continue;
					}
					goto case 18;
				case 3:
					if (num3 != -1)
					{
						num = 10;
						num2 = num;
					}
					else
					{
						text2 = keyPathname;
						num = 18;
						num2 = num;
					}
					continue;
				case 9:
					if (text.Equals("64"))
					{
						num = 8;
						num2 = num;
					}
					else
					{
						text3 = text2;
						num = 12;
						num2 = num;
					}
					continue;
				case 16:
					rootHive = DuoUtil.m_a[text3];
					return;
				case 6:
					num = 2;
					num2 = num;
					continue;
				case 2:
					if (!recognizeViewSpecifier)
					{
						text3 = text2;
						num = -1335;
						short num4 = num;
						num = -1335;
						switch (num4 == num)
						{
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 11;
							num2 = num;
							continue;
						case false:
						case true:
							break;
						}
						goto case 16;
					}
					num = 1;
					if (num != 0)
					{
					}
					num = 17;
					num2 = num;
					continue;
				case 7:
					subkeyPath = keyPathname.Substring(num3 + 1);
					num = 4;
					num2 = num;
					continue;
				case 4:
					num = 0;
					_ = num;
					goto case 18;
				case 1:
				case 11:
				case 12:
				case 19:
					num = 0;
					num2 = num;
					continue;
				case 0:
					if (DuoUtil.m_a.ContainsKey(text3))
					{
						num = 16;
						num2 = num;
						continue;
					}
					throw new ArgumentException(string.Format("Unrecognized root hive {0}", text3), "keyPathname");
				case 5:
					throw new ArgumentNullException("keyPathname");
				case 18:
					registryView = RegistryView.Default;
					num = 13;
					num2 = num;
					continue;
				case 13:
					if (text2.Length > 1)
					{
						num = 6;
						num2 = num;
						continue;
					}
					goto case 1;
				case 17:
					text = text2.Substring(text2.Length - 2);
					num = 20;
					num2 = num;
					continue;
				case 20:
					if (!text.Equals("32"))
					{
						num = 9;
						num2 = num;
					}
					else
					{
						num = 21;
						num2 = num;
					}
					continue;
				}
				break;
			}
			goto IL_009f;
			IL_009f:
			num3 = -1;
			text2 = null;
			text = null;
			text3 = null;
			num = 14;
			num2 = num;
			goto IL_0040;
		}
	}

	public static Exception SafeDataContractSerializerDeserialize<T>(string sourceXml, out T deserializedObject)
	{
		short num = -19594;
		short num2 = num;
		num = -19594;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		T b = default(T);
		_ = NotYetInitialized;
		Exception result = DoNothrow(delegate
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			short num3 = 0;
			switch (num3)
			{
			default:
			{
				num3 = 10771;
				short num4 = num3;
				num3 = 10771;
				switch (num4 == num3)
				{
				default:
				{
					num3 = 1;
					if (num3 != 0)
					{
					}
					num3 = 0;
					_ = num3;
					num3 = 0;
					if (num3 != 0)
					{
					}
					MemoryStream memoryStream = new MemoryStream();
					try
					{
						UnicodeEncoding encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
						StreamWriter streamWriter = new StreamWriter(memoryStream, encoding);
						try
						{
							streamWriter.Write(sourceXml);
							streamWriter.Flush();
							memoryStream.Seek(0L, SeekOrigin.Begin);
							XmlReader xmlReader = XmlReader.Create(new StreamReader(memoryStream, encoding));
							try
							{
								DataContractSerializer val = new DataContractSerializer(typeof(T));
								b = (T)((XmlObjectSerializer)val).ReadObject(xmlReader);
								break;
							}
							finally
							{
								num3 = 2;
								int num5 = num3;
								while (true)
								{
									switch (num5)
									{
									case 2:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (xmlReader != null)
										{
											num3 = 1;
											num5 = num3;
											continue;
										}
										break;
									case 1:
										((IDisposable)xmlReader).Dispose();
										num3 = 0;
										num5 = num3;
										continue;
									case 0:
										break;
									}
									break;
								}
							}
						}
						finally
						{
							num3 = 0;
							int num5 = num3;
							while (true)
							{
								switch (num5)
								{
								case 0:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (streamWriter != null)
									{
										num3 = 2;
										num5 = num3;
										continue;
									}
									break;
								case 2:
									((IDisposable)streamWriter).Dispose();
									num3 = 1;
									num5 = num3;
									continue;
								case 1:
									break;
								}
								break;
							}
						}
					}
					finally
					{
						num3 = 2;
						int num5 = num3;
						while (true)
						{
							switch (num5)
							{
							case 2:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (memoryStream != null)
								{
									num3 = 1;
									num5 = num3;
									continue;
								}
								break;
							case 1:
								((IDisposable)memoryStream).Dispose();
								num3 = 0;
								num5 = num3;
								continue;
							case 0:
								break;
							}
							break;
						}
					}
				}
				case false:
				case true:
					break;
				}
				break;
			}
			}
		});
		deserializedObject = b;
		return result;
	}

	public static Exception SafeDataContractSerializerSerialize<T>(T sourceObject, out string serializedXml)
	{
		short num = -2841;
		short num2 = num;
		num = -2841;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			string a = null;
			_ = NotYetInitialized;
			Exception result = DoNothrow(delegate
			{
				short num3 = 25473;
				short num4 = num3;
				num3 = 25473;
				switch (num4 == num3)
				{
				default:
					num3 = 0;
					_ = num3;
					num3 = 1;
					if (num3 != 0)
					{
					}
					num3 = 0;
					if (num3 != 0)
					{
					}
					a = DataContractSerializerSerialize(sourceObject);
					break;
				}
			});
			serializedXml = a;
			return result;
		}
		}
	}

	public static string DataContractSerializerSerialize<T>(T sourceObject, Encoding targetEncoding = null)
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		XmlWriter xmlWriter = default(XmlWriter);
		while (true)
		{
			short num = 0;
			switch (num)
			{
			}
			num = -12637;
			short num2 = num;
			num = -12637;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			Encoding encoding = targetEncoding;
			string result = null;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				num = 2;
				int num3 = num;
				while (true)
				{
					switch (num3)
					{
					case 2:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (targetEncoding == null)
						{
							num = 1;
							num3 = num;
							continue;
						}
						break;
					case 1:
						encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
						num = 0;
						num3 = num;
						continue;
					case 3:
						try
						{
							((XmlObjectSerializer)new DataContractSerializer(typeof(T))).WriteObject(xmlWriter, (object)sourceObject);
							xmlWriter.Flush();
							memoryStream.Seek(0L, SeekOrigin.Begin);
							StreamReader streamReader = new StreamReader(memoryStream, encoding);
							try
							{
								result = streamReader.ReadToEnd();
							}
							finally
							{
								num = 0;
								num3 = num;
								while (true)
								{
									switch (num3)
									{
									case 0:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (streamReader != null)
										{
											num = 1;
											num3 = num;
											continue;
										}
										break;
									case 1:
										((IDisposable)streamReader).Dispose();
										num = 2;
										num3 = num;
										continue;
									case 2:
										break;
									}
									break;
								}
							}
						}
						finally
						{
							num = 2;
							num3 = num;
							while (true)
							{
								switch (num3)
								{
								case 2:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (xmlWriter != null)
									{
										num = 1;
										num3 = num;
										continue;
									}
									break;
								case 1:
									((IDisposable)xmlWriter).Dispose();
									num = 0;
									num3 = num;
									continue;
								case 0:
									break;
								}
								break;
							}
						}
						goto end_IL_00ee;
					case 0:
						break;
					}
					xmlWriter = XmlWriter.Create(new StreamWriter(memoryStream, encoding));
					num = 3;
					num3 = num;
					continue;
					end_IL_00ee:
					break;
				}
			}
			finally
			{
				num = 1;
				int num3 = num;
				while (true)
				{
					switch (num3)
					{
					case 1:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (memoryStream != null)
						{
							num = 0;
							num3 = num;
							continue;
						}
						break;
					case 0:
						((IDisposable)memoryStream).Dispose();
						num = 2;
						num3 = num;
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			num = 1;
			if (num != 0)
			{
			}
			return result;
		}
	}

	public static string ReadRootJsonKey(string filePathname, string keyName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		JavaScriptSerializer val = new JavaScriptSerializer();
		string text = File.ReadAllText(Path.Combine(filePathname));
		Dictionary<string, object> dictionary = (Dictionary<string, object>)val.DeserializeObject(text);
		if (dictionary.ContainsKey(keyName))
		{
			short num = -5260;
			short num2 = num;
			num = -5260;
			switch (num2 == num)
			{
			default:
				num = 0;
				_ = num;
				goto case true;
			case true:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return dictionary[keyName].ToString();
			case false:
			case true:
				break;
			}
		}
		return null;
	}

	public static void SetRootJsonKey(string targetFilename, string rootKeyName, string newValue)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		short num = 20066;
		short num2 = num;
		num = 20066;
		switch (num2 == num)
		{
		default:
			num = 0;
			_ = num;
			break;
		case true:
			break;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		JavaScriptSerializer val = new JavaScriptSerializer();
		string text = File.ReadAllText(Path.Combine(targetFilename));
		Dictionary<string, object> dictionary = (Dictionary<string, object>)val.DeserializeObject(text);
		dictionary[rootKeyName] = newValue;
		string contents = val.Serialize((object)dictionary);
		File.WriteAllText(targetFilename, contents);
	}

	public static void SetJsonKey(string targetFilename, string[] jsonKeyNames, string newValue, bool createParentKeysIfNotFound = true, bool createTargetKeyIfNotFound = true)
	{
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		int a_ = 9;
		string a_2 = "5MU415TZ162JTOKF3LJNP8CRBO4ZCXB4K12CBDFKGD51FSNZQ4I03RUY1D";
		short num = 0;
		switch (num)
		{
		default:
		{
			num = 30;
			int num2 = num;
			int num3 = default(int);
			Dictionary<string, object> dictionary2 = default(Dictionary<string, object>);
			StringBuilder stringBuilder2 = default(StringBuilder);
			StringBuilder stringBuilder = default(StringBuilder);
			int num5 = default(int);
			bool flag = default(bool);
			JavaScriptSerializer val = default(JavaScriptSerializer);
			Dictionary<string, object> dictionary = default(Dictionary<string, object>);
			string text2 = default(string);
			string[] array = default(string[]);
			while (true)
			{
				switch (num2)
				{
				case 30:
					switch (0)
					{
					default:
						goto end_IL_0063;
					case 0:
						break;
					}
					goto default;
				default:
					if (jsonKeyNames == null)
					{
						num = 25;
						num2 = num;
					}
					else
					{
						num = 9;
						num2 = num;
					}
					break;
				case 33:
				case 36:
					num3++;
					num = 4;
					num2 = num;
					break;
				case 16:
					dictionary2 = (Dictionary<string, object>)dictionary2[jsonKeyNames[num3]];
					num = 36;
					num2 = num;
					break;
				case 22:
					num = 5;
					num2 = num;
					break;
				case 5:
					if (dictionary2[jsonKeyNames[num3]] is Dictionary<string, object>)
					{
						num = 16;
						num2 = num;
						break;
					}
					throw new KeyNotFoundException(string.Format("Key {0} exists but is not an object in file {1}, target key {2}!", stringBuilder2, targetFilename, stringBuilder));
				case 11:
				case 37:
					num5++;
					num = 32;
					num2 = num;
					break;
				case 24:
					flag = true;
					num = 29;
					num2 = num;
					break;
				case 3:
				{
					dictionary2[jsonKeyNames[^1]] = newValue;
					string contents = val.Serialize((object)dictionary);
					File.WriteAllText(targetFilename, contents);
					return;
				}
				case 0:
					if (stringBuilder == null)
					{
						num = 6;
						num2 = num;
					}
					else
					{
						stringBuilder2.AppendFormat("\\{0}", jsonKeyNames[num3]);
						num = 1;
						num2 = num;
					}
					break;
				case 25:
					throw new ArgumentNullException("jsonKeyNames");
				case 34:
					stringBuilder.Append(text2);
					num = 37;
					num2 = num;
					break;
				case 14:
					if (stringBuilder != null)
					{
						stringBuilder.AppendFormat("\\{0}", text2);
						num = 0;
						_ = num;
						num = 11;
						num2 = num;
					}
					else
					{
						num = 34;
						num2 = num;
					}
					break;
				case 8:
					num = 10;
					num2 = num;
					break;
				case 10:
					if (jsonKeyNames.Length <= 1)
					{
						dictionary2 = dictionary;
						num = 2;
						num2 = num;
					}
					else
					{
						num = 27;
						num2 = num;
					}
					break;
				case 2:
				case 15:
					num = 7;
					num2 = num;
					break;
				case 7:
					if (!dictionary2.ContainsKey(jsonKeyNames[^1]))
					{
						flag = createTargetKeyIfNotFound;
						num = 17;
						num2 = num;
					}
					else
					{
						num = 24;
						num2 = num;
					}
					break;
				case 4:
				case 31:
					num = 28;
					num2 = num;
					break;
				case 28:
					if (num3 < jsonKeyNames.Length - 1)
					{
						num = 0;
						num2 = num;
					}
					else
					{
						num = 35;
						num2 = num;
					}
					break;
				case 6:
					stringBuilder2.Append(jsonKeyNames[num3]);
					num = 21;
					num2 = num;
					break;
				case 26:
					num = 1;
					if (num != 0)
					{
					}
					if (!createParentKeysIfNotFound)
					{
						throw new KeyNotFoundException(string.Format("Key {0} not found in file {1}, target key {2}!", stringBuilder2, targetFilename, stringBuilder));
					}
					num = 18;
					num2 = num;
					break;
				case 13:
				case 32:
					num = 23;
					num2 = num;
					break;
				case 23:
					if (num5 < array.Length)
					{
						text2 = array[num5];
						num = 14;
						num2 = num;
					}
					else
					{
						num = 8;
						num2 = num;
					}
					break;
				case 19:
					throw new ArgumentException("Key names array cannot be empty", "jsonKeyNames");
				case 35:
					num = 15;
					num2 = num;
					break;
				case 27:
					dictionary2 = dictionary;
					num3 = 0;
					num = 31;
					num2 = num;
					break;
				case 1:
				case 21:
					num = 20;
					num2 = num;
					break;
				case 20:
					if (!dictionary2.ContainsKey(jsonKeyNames[num3]))
					{
						num = 26;
						num2 = num;
					}
					else
					{
						num = 22;
						num2 = num;
					}
					break;
				case 9:
					if (jsonKeyNames.Length != 0)
					{
						val = new JavaScriptSerializer();
						string text = File.ReadAllText(Path.Combine(targetFilename));
						dictionary = (Dictionary<string, object>)val.DeserializeObject(text);
						dictionary2 = null;
						stringBuilder2 = new StringBuilder();
						flag = false;
						stringBuilder = new StringBuilder();
						array = jsonKeyNames;
						num5 = 0;
						num = 13;
						num2 = num;
					}
					else
					{
						num = 19;
						num2 = num;
					}
					break;
				case 18:
				{
					num = 12028;
					short num4 = num;
					num = 12028;
					switch (num4 == num)
					{
					case false:
					case true:
						break;
					default:
						num = 0;
						if (num != 0)
						{
						}
						dictionary2[jsonKeyNames[num3]] = new Dictionary<string, object>();
						num = 33;
						num2 = num;
						goto end_IL_0063;
					}
					goto case 35;
				}
				case 17:
				case 29:
					num = 12;
					num2 = num;
					break;
				case 12:
					{
						if (flag)
						{
							num = 3;
							num2 = num;
							break;
						}
						throw new KeyNotFoundException(string.Format("Target key {0} not found in file {1}!", stringBuilder.ToString(), targetFilename));
					}
					end_IL_0063:
					break;
				}
			}
		}
		}
	}

	public static string GetCaptureGroup(string filePathname, string regularExpression, int captureGroupNumber)
	{
		short num = -12625;
		short num2 = num;
		num = -12625;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			string input = File.ReadAllText(filePathname);
			Match match = new Regex(regularExpression, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace).Match(input);
			if (match != null)
			{
				return match.Groups[1 + captureGroupNumber].Value;
			}
			break;
		}
		case false:
		case true:
			break;
		}
		return null;
	}

	public static bool IsCurrentUserElevatedAdmin()
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		}
		WindowsIdentity current = default(WindowsIdentity);
		int A_ = default(int);
		bool result2 = default(bool);
		while (true)
		{
			bool result = false;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				switch (0)
				{
				default:
					while (true)
					{
						switch (num2)
						{
						case 2:
							goto end_IL_00d5;
						case 13:
							if (current == null)
							{
								num = 0;
								num2 = num;
							}
							else
							{
								num = 5;
								num2 = num;
							}
							continue;
						case 6:
							if (GetTokenInformation(current.Token, DuoUtil.a.r, intPtr, A_, out A_))
							{
								num = 1;
								num2 = num;
								continue;
							}
							goto case 11;
						case 0:
							result2 = false;
							num = 14;
							num2 = num;
							continue;
						case 14:
							goto end_IL_00cb;
						case 10:
							result2 = false;
							num = 3;
							num2 = num;
							continue;
						case 3:
							goto end_IL_00cb;
						case 1:
							result = Marshal.ReadInt32(intPtr) == 2;
							num = 11;
							num2 = num;
							continue;
						case 9:
							if (Environment.OSVersion.Platform == PlatformID.Win32NT)
							{
								num = 4;
								num2 = num;
								continue;
							}
							goto case 10;
						case 5:
							if (!new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator))
							{
								num = 9;
								num2 = num;
							}
							else
							{
								num = 8;
								num2 = num;
							}
							continue;
						case 4:
							num = 12;
							num2 = num;
							continue;
						case 12:
							if (Environment.OSVersion.Version.Major >= 6)
							{
								A_ = Marshal.SizeOf(typeof(int));
								intPtr = Marshal.AllocCoTaskMem(A_);
								num = 6;
								num2 = num;
							}
							else
							{
								num = 10;
								num2 = num;
							}
							continue;
						case 8:
							result2 = true;
							num = 7;
							num2 = num;
							continue;
						case 7:
							goto end_IL_00cb;
						case 11:
							num = 2;
							num2 = num;
							continue;
						}
						goto case 0;
						continue;
						end_IL_00d5:
						break;
					}
					break;
				case 0:
					current = WindowsIdentity.GetCurrent();
					num = 13;
					num2 = num;
					goto default;
				}
				goto IL_0031;
				end_IL_00cb:;
			}
			catch
			{
				goto IL_0031;
			}
			finally
			{
				num = 0;
				num2 = num;
				while (true)
				{
					switch (num2)
					{
					case 0:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (intPtr != IntPtr.Zero)
						{
							num = 1;
							num2 = num;
							continue;
						}
						break;
					case 1:
						Marshal.FreeCoTaskMem(intPtr);
						num = 2;
						num2 = num;
						continue;
					case 2:
						break;
					}
					break;
				}
			}
			break;
			IL_0031:
			num = -4563;
			short num3 = num;
			num = -4563;
			switch (num3 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return result;
		}
		num = 1;
		if (num != 0)
		{
		}
		return result2;
	}

	public static string GetModulePathname(string moduleFileNodeName)
	{
		int num = default(int);
		IntPtr moduleHandleW = default(IntPtr);
		string result = default(string);
		StringBuilder stringBuilder = default(StringBuilder);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 1:
					if (moduleHandleW != IntPtr.Zero)
					{
						short num2 = 4;
						num = num2;
						continue;
					}
					goto case 3;
				case 0:
				{
					result = stringBuilder.ToString();
					short num2 = 3;
					num = num2;
					continue;
				}
				case 4:
				{
					short num2 = 20642;
					short num3 = num2;
					num2 = 20642;
					switch (num3 == num2)
					{
					default:
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 2;
						num = num2;
						continue;
					case false:
					case true:
						break;
					}
					goto case 2;
				}
				case 2:
					if (GetModuleFileNameExW(DuoUtil.m_c, moduleHandleW, stringBuilder, stringBuilder.Capacity) != 0)
					{
						short num2 = 0;
						num = num2;
						continue;
					}
					goto case 3;
				case 3:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			moduleHandleW = GetModuleHandleW(moduleFileNodeName);
			stringBuilder = new StringBuilder(512);
			result = null;
			short num2 = 0;
			_ = num2;
			num2 = 1;
			num = num2;
			goto default;
		}
		}
	}

	public static Version GetClrVersion()
	{
		int a_ = 11;
		string a_2 = "MI2Z471P4BOZOSSBV5OFI3NCFQV5F5LZXWBBBZSBH0W6HTFT2GBI36IGK7RM6";
		int num = default(int);
		Version version = default(Version);
		Version result = default(Version);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 6:
					if (version.CompareTo(ClrVersion_450) > 0)
					{
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 8;
						num = num2;
						continue;
					}
					goto case 2;
				case 5:
					if (version.CompareTo(ClrVersion_460) > 0)
					{
						result = version;
						num2 = 4;
						num = num2;
						continue;
					}
					goto IL_0137;
				case 9:
				{
					num2 = -32539;
					short num3 = num2;
					num2 = -32539;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						result = new Version("4.5.1");
						num2 = 1;
						num = num2;
						continue;
					}
					goto IL_0137;
				}
				case 1:
					num2 = 0;
					_ = num2;
					goto case 2;
				case 7:
					result = new Version("4.5.0");
					num2 = 2;
					num = num2;
					continue;
				case 8:
					num2 = 11;
					num = num2;
					continue;
				case 11:
					if (version.CompareTo(ClrVersion_451) <= 0)
					{
						num2 = 7;
						num = num2;
					}
					else
					{
						num2 = 10;
						num = num2;
					}
					continue;
				case 0:
					result = new Version("4.5.2");
					num2 = 3;
					num = num2;
					continue;
				case 10:
					if (version.CompareTo(ClrVersion_452) > 0)
					{
						num2 = 5;
						num = num2;
					}
					else
					{
						num2 = 9;
						num = num2;
					}
					continue;
				case 2:
				case 3:
				case 4:
					{
						return result;
					}
					IL_0137:
					num2 = 0;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			version = Version.Parse(FileVersionInfo.GetVersionInfo(GetModulePathname("clr.dll")).ProductVersion);
			result = null;
			short num2 = 6;
			num = num2;
			goto default;
		}
		}
	}

	public static OSVERSIONINFOEX GetOsVersion()
	{
		short num = 31529;
		short num2 = num;
		num = 31529;
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			OSVERSIONINFOEX A_ = default(OSVERSIONINFOEX);
			A_.dwOSVersionInfoSize = (uint)Marshal.SizeOf(A_);
			GetVersionEx(ref A_);
			return A_;
		}
		}
	}

	public static string GetFlagsDescription(Enum enumValue)
	{
		int a_ = 17;
		string a_2 = "D2R0CPV59UVBG99NH4V6OMCY4KNT2IOPB34JKZXV30I85FG0ZY156I";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			num = -21001;
			short num3 = num;
			num = -21001;
			StringBuilder stringBuilder = default(StringBuilder);
			switch (num3 == num)
			{
			default:
			{
				num = 0;
				if (num != 0)
				{
				}
				num = 1;
				if (num != 0)
				{
				}
				stringBuilder = new StringBuilder();
				IEnumerator enumerator = enumValue.GetType().GetEnumValues().GetEnumerator();
				try
				{
					num = 14;
					num2 = num;
					MemberInfo[] member = default(MemberInfo[]);
					Enum @enum = default(Enum);
					DescriptionAttribute descriptionAttribute = default(DescriptionAttribute);
					object[] customAttributes = default(object[]);
					while (true)
					{
						switch (num2)
						{
						case 14:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						case 6:
							member = enumValue.GetType().GetMember(@enum.ToString());
							num = 0;
							num2 = num;
							continue;
						case 0:
							if (member != null)
							{
								num = 11;
								num2 = num;
								continue;
							}
							goto default;
						case 1:
							if (enumValue.HasFlag(@enum))
							{
								num = 6;
								num2 = num;
								continue;
							}
							goto default;
						case 5:
							stringBuilder.Append(descriptionAttribute.Description);
							num = 9;
							num2 = num;
							continue;
						default:
							num = 12;
							num2 = num;
							continue;
						case 12:
							if (enumerator.MoveNext())
							{
								@enum = (Enum)enumerator.Current;
								num = 1;
								num2 = num;
							}
							else
							{
								num = 16;
								num2 = num;
							}
							continue;
						case 3:
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (customAttributes.Length >= 1)
							{
								num = 17;
								num2 = num;
								continue;
							}
							goto default;
						case 11:
							num = 4;
							num2 = num;
							continue;
						case 4:
							if (member.Length == 1)
							{
								num = 10;
								num2 = num;
								continue;
							}
							goto default;
						case 17:
							descriptionAttribute = customAttributes[0] as DescriptionAttribute;
							num = 7;
							num2 = num;
							continue;
						case 7:
							if (stringBuilder.Length != 0)
							{
								stringBuilder.Append(" ");
								stringBuilder.Append(descriptionAttribute.Description);
								num = 13;
								num2 = num;
							}
							else
							{
								num = 5;
								num2 = num;
							}
							continue;
						case 10:
							customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
							num = 2;
							num2 = num;
							continue;
						case 2:
							if (customAttributes != null)
							{
								num = 3;
								num2 = num;
								continue;
							}
							goto default;
						case 16:
							num = 15;
							num2 = num;
							continue;
						case 15:
							break;
						}
						break;
					}
				}
				finally
				{
					IDisposable disposable = default(IDisposable);
					switch (0)
					{
					default:
						while (true)
						{
							switch (num2)
							{
							case 1:
								if (disposable != null)
								{
									num = 0;
									num2 = num;
									continue;
								}
								goto end_IL_046f;
							case 0:
								disposable.Dispose();
								num = 2;
								num2 = num;
								continue;
							case 2:
								goto end_IL_046f;
							}
							goto case 0;
							continue;
							end_IL_046f:
							break;
						}
						break;
					case 0:
						disposable = enumerator as IDisposable;
						num = 1;
						num2 = num;
						goto default;
					}
				}
				break;
			}
			case false:
			case true:
				break;
			}
			num = 0;
			_ = num;
			return stringBuilder.ToString();
		}
		}
	}

	public static Exception GetMachineType(string fileName, out PortableExecutableHHeader_MachineType moduleType)
	{
		short num = 18524;
		short num2 = num;
		num = 18524;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			byte[] b = new byte[4096];
			PortableExecutableHHeader_MachineType c = PortableExecutableHHeader_MachineType.Native;
			_ = NotYetInitialized;
			Exception result = DoNothrow(delegate
			{
				while (true)
				{
					FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
					short num3;
					try
					{
						fileStream.Read(b, 0, 4096);
					}
					finally
					{
						num3 = 2;
						int num4 = num3;
						while (true)
						{
							switch (num4)
							{
							case 2:
								switch (0)
								{
								default:
									continue;
								case 0:
									break;
								}
								goto default;
							default:
								if (fileStream != null)
								{
									num3 = 1;
									num4 = num3;
									continue;
								}
								break;
							case 1:
								((IDisposable)fileStream).Dispose();
								num3 = 0;
								num4 = num3;
								continue;
							case 0:
								break;
							}
							break;
						}
					}
					num3 = 1;
					if (num3 != 0)
					{
					}
					num3 = 0;
					_ = num3;
					num3 = 18265;
					short num5 = num3;
					num3 = 18265;
					switch (num5 == num3)
					{
					case false:
					case true:
						break;
					default:
					{
						num3 = 0;
						if (num3 != 0)
						{
						}
						int num6 = BitConverter.ToInt32(b, 60);
						int num7 = BitConverter.ToUInt16(b, num6 + 4);
						c = (PortableExecutableHHeader_MachineType)num7;
						return;
					}
					}
				}
			});
			moduleType = c;
			return result;
		}
		}
	}

	public static Exception IsExecuteable64Bit(string filePathname)
	{
		int num = default(int);
		PortableExecutableHHeader_MachineType moduleType = default(PortableExecutableHHeader_MachineType);
		switch (0)
		{
		default:
			while (true)
			{
				Action action;
				switch (num)
				{
				case 1:
				{
					short num2 = -16008;
					short num3 = num2;
					num2 = -16008;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						if (moduleType == PortableExecutableHHeader_MachineType.AmdX64)
						{
							num2 = 0;
							_ = num2;
							num2 = 3;
							num = num2;
						}
						else
						{
							num2 = 0;
							num = num2;
						}
						continue;
					case false:
					case true:
						break;
					}
					goto case 3;
				}
				case 3:
					return null;
				case 0:
					action = <>c.<>9__68_0;
					if (action == null)
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					goto IL_01af;
				case 2:
					{
						short num2 = 1;
						if (num2 != 0)
						{
						}
						action = (<>c.<>9__68_0 = delegate
						{
							int a_ = 15;
							string a_2 = "SK2KCKUPE2P9ZI9MSS5QDPSD5800NZBTKH5XDGWEOWZTG521EHUAZV82H";
							short num4 = -12586;
							short num5 = num4;
							num4 = -12586;
							switch (num5 == num4)
							{
							default:
								num4 = 0;
								_ = num4;
								num4 = 1;
								if (num4 != 0)
								{
								}
								num4 = 0;
								if (num4 != 0)
								{
								}
								throw new IOException("Format is not 64-bit");
							}
						});
						goto IL_01af;
					}
					IL_01af:
					return DoNothrow(action);
				}
				break;
			}
			goto case 0;
		case 0:
		{
			moduleType = PortableExecutableHHeader_MachineType.Native;
			Exception notYetInitialized = NotYetInitialized;
			GetMachineType(filePathname, out moduleType);
			short num2 = 1;
			num = num2;
			goto default;
		}
		}
	}

	public static bool CreateTempDirectory(out string pathToCreatedDirectory)
	{
		int a_ = 2;
		string a_2 = "BE6HEG6BVPSSJ71L7PXVKLKMZ2IW8UBDXY4PBNJ2T0G3QRYG";
		int num = default(int);
		int num4 = default(int);
		bool flag = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 2:
					num2 = 9;
					num = num2;
					continue;
				case 9:
				{
					num2 = -16591;
					short num3 = num2;
					num2 = -16591;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						if (num4 >= 5)
						{
							num2 = 4;
							num = num2;
						}
						else
						{
							pathToCreatedDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
							num2 = 7;
							num = num2;
						}
						continue;
					case false:
					case true:
						break;
					}
					goto IL_02c0;
				}
				case 6:
				case 8:
					num2 = 0;
					num = num2;
					continue;
				case 0:
					num2 = 0;
					_ = num2;
					if (!flag)
					{
						num2 = 2;
						num = num2;
						continue;
					}
					goto case 4;
				case 1:
					try
					{
						Directory.CreateDirectory(pathToCreatedDirectory);
						flag = true;
					}
					catch (Exception)
					{
						flag = false;
					}
					goto IL_0266;
				case 5:
					return true;
				case 7:
					if (!Directory.Exists(pathToCreatedDirectory))
					{
						num2 = 1;
						num = num2;
						continue;
					}
					goto IL_0266;
				case 4:
					num2 = 3;
					num = num2;
					continue;
				case 3:
					{
						if (flag)
						{
							num2 = 5;
							num = num2;
							continue;
						}
						goto IL_02c0;
					}
					IL_0266:
					num4++;
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = 6;
					num = num2;
					continue;
					IL_02c0:
					Console.WriteLine("Failed to create a temporary directory");
					pathToCreatedDirectory = string.Empty;
					return false;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			pathToCreatedDirectory = string.Empty;
			flag = false;
			num4 = 0;
			short num2 = 8;
			num = num2;
			goto default;
		}
		}
	}

	public static void AttemptCleanupOfDirectoryInTempSpace(string directoryToDelete)
	{
		int a_ = 9;
		string a_2 = "6MIHESRYTBJHYF8V7G5RXYYY3QELZOV99Q73XF4G05NYHWMG0Q2X";
		short num;
		try
		{
			num = 29172;
			short num2 = num;
			num = 29172;
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				Directory.Delete(directoryToDelete, recursive: true);
				break;
			}
		}
		catch (Exception)
		{
			Console.WriteLine("Cleanup of temp space directory {0} failed.", directoryToDelete);
		}
		num = 0;
		_ = num;
		num = 1;
		if (num == 0)
		{
		}
	}

	public static bool FileExistsWithGivenType(string pathToSourceFile, string expectedFileExtension)
	{
		short num = 3;
		int num2 = num;
		while (true)
		{
			switch (num2)
			{
			case 3:
				switch (0)
				{
				default:
					continue;
				case 0:
					break;
				}
				goto default;
			default:
				if (!File.Exists(pathToSourceFile))
				{
					break;
				}
				goto IL_0045;
			case 2:
			{
				num = 7161;
				short num3 = num;
				num = 7161;
				switch (num3 == num)
				{
				case false:
				case true:
					break;
				default:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					return true;
				}
				goto IL_0045;
			}
			case 1:
				num = 0;
				_ = num;
				num = 0;
				num2 = num;
				continue;
			case 0:
				{
					if (expectedFileExtension.EqualsNoCase(Path.GetExtension(pathToSourceFile)))
					{
						num = 2;
						num2 = num;
						continue;
					}
					break;
				}
				IL_0045:
				num = 1;
				num2 = num;
				continue;
			}
			break;
		}
		return false;
	}

	public static bool MoveFileToBackupLocationIfExists(string filepath, string backupPath)
	{
		int a_ = 1;
		string a_2 = "9HJZ5A98MLG1UULL2P1ZNTDAELPL1PLVH143O08NP6HN35BJMDBG6C2ETVJ372";
		if (File.Exists(filepath))
		{
			while (true)
			{
				bool result;
				try
				{
					File.Move(filepath, backupPath);
					FileSecurity.PreserveInheritanceRules(backupPath);
					Console.WriteLine("Moved {0} to backup location {1}", filepath, backupPath);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					result = false;
					goto IL_0070;
				}
				break;
				IL_0070:
				short num = 0;
				_ = num;
				num = 611;
				short num2 = num;
				num = 611;
				switch (num2 == num)
				{
				case false:
				case true:
					continue;
				}
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return result;
			}
		}
		return true;
	}

	public static void ParseExactUsernameDomain(string logonuserUsername, string logonuserDomain, out string exactUsername, out string exactDomain)
	{
		int num = default(int);
		string text = default(string);
		switch (0)
		{
		default:
		{
			int num2 = default(int);
			int num4 = default(int);
			while (true)
			{
				short num3;
				switch (num)
				{
				case 17:
					if (logonuserDomain != null)
					{
						num3 = 7;
						num = num3;
						continue;
					}
					goto case 9;
				case 7:
				{
					num3 = -17900;
					short num5 = num3;
					num3 = -17900;
					switch (num5 == num3)
					{
					default:
						num3 = 0;
						if (num3 != 0)
						{
						}
						exactDomain = logonuserDomain;
						num3 = 9;
						num = num3;
						continue;
					case false:
					case true:
						break;
					}
					goto case 8;
				}
				case 8:
					exactUsername = logonuserUsername.Substring(num2 + 1);
					text = logonuserUsername.Substring(0, num2);
					num3 = 26;
					num = num3;
					continue;
				case 18:
					num3 = 1;
					if (num3 != 0)
					{
					}
					if (num4 != -1)
					{
						num3 = 10;
						num = num3;
						continue;
					}
					goto IL_0276;
				case 13:
					num3 = 0;
					_ = num3;
					num3 = 20;
					num = num3;
					continue;
				case 20:
					if (num2 != -1)
					{
						num3 = 1;
						num = num3;
						continue;
					}
					goto IL_047c;
				case 9:
					num3 = 16;
					num = num3;
					continue;
				case 16:
					if (logonuserUsername != null)
					{
						num3 = 15;
						num = num3;
						continue;
					}
					goto case 3;
				case 2:
					num3 = 25;
					num = num3;
					continue;
				case 25:
					if (num2 < num4)
					{
						num3 = 8;
						num = num3;
						continue;
					}
					exactUsername = logonuserUsername.Substring(0, num4);
					text = logonuserUsername.Substring(num4 + 1);
					num3 = 11;
					num = num3;
					continue;
				case 5:
					exactDomain = text;
					num3 = 0;
					num = num3;
					continue;
				case 0:
					return;
				case 21:
					exactUsername = logonuserUsername.Substring(0, num4);
					text = logonuserUsername.Substring(num4 + 1);
					num3 = 4;
					num = num3;
					continue;
				case 15:
					num2 = -1;
					num4 = -1;
					num2 = logonuserUsername.IndexOf('\\');
					num4 = logonuserUsername.IndexOf('@');
					num3 = 19;
					num = num3;
					continue;
				case 19:
					if (num4 == -1)
					{
						num3 = 13;
						num = num3;
						continue;
					}
					goto IL_047c;
				case 3:
				case 4:
				case 11:
				case 24:
				case 26:
					num3 = 22;
					num = num3;
					continue;
				case 22:
					if (exactDomain.Length == 0)
					{
						num3 = 5;
						num = num3;
						continue;
					}
					return;
				case 6:
					if (num4 != -1)
					{
						num3 = 14;
						num = num3;
						continue;
					}
					goto IL_0175;
				case 1:
					exactUsername = logonuserUsername.Substring(num2 + 1);
					text = logonuserUsername.Substring(0, num2);
					num3 = 24;
					num = num3;
					continue;
				case 14:
					num3 = 12;
					num = num3;
					continue;
				case 12:
					if (num2 == -1)
					{
						num3 = 21;
						num = num3;
						continue;
					}
					goto IL_0175;
				case 10:
					num3 = 23;
					num = num3;
					continue;
				case 23:
					{
						if (num2 != -1)
						{
							num3 = 2;
							num = num3;
							continue;
						}
						goto IL_0276;
					}
					IL_047c:
					num3 = 6;
					num = num3;
					continue;
					IL_0175:
					num3 = 18;
					num = num3;
					continue;
					IL_0276:
					exactUsername = logonuserUsername;
					num3 = 3;
					num = num3;
					continue;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			text = "";
			exactUsername = "";
			exactDomain = "";
			short num3 = 17;
			num = num3;
			goto default;
		}
		}
	}

	public static string GetFdqnFromDomain(string friendlyDomainName, IDuoLogger log)
	{
		//IL_00d1: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		int a_ = 14;
		string a_2 = "QA4XSVNERGTWUQPM7ZZTLXEGV02MJGOITS8NI2JAN539ZSXWF68A194TGXBXY";
		string result;
		short num;
		try
		{
			Domain val;
			try
			{
				num = 28470;
				short num2 = num;
				num = 28470;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					val = Domain.GetDomain(new DirectoryContext((DirectoryContextType)0, friendlyDomainName));
					break;
				}
			}
			catch (ActiveDirectoryObjectNotFoundException)
			{
				log.AppendDebugLine("GetFdqnFromDomain (handle exception ActiveDirectoryObjectNotFoundException).  Using GetCurrentDomain()");
				val = Domain.GetCurrentDomain();
			}
			log.AppendDebugLine(string.Format("Translated FriendlyDomain using: {0} to FQDN: {1}", friendlyDomainName, ((ActiveDirectoryPartition)val).Name));
			result = ((ActiveDirectoryPartition)val).Name;
		}
		catch (DirectoryServicesCOMException val3)
		{
			DirectoryServicesCOMException val4 = val3;
			log.WriteLine(string.Format("Failed to determine FQDN from FriendlyDomain {0}\nError:\n{1}", friendlyDomainName, ((Exception)(object)val4).Message.ToString()));
			result = null;
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		_ = num;
		return result;
	}

	public static string GetUpnUsername(string origUsername, IDuoLogger log)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		int a_ = 8;
		string a_2 = "TU9AMAI6X76IRM9ZI9MGEXQQ5OBE0J18O5XP452LON9H775K66OC9DWQJCTF";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			string exactDomain = default(string);
			string exactUsername = default(string);
			switch (0)
			{
			default:
			{
				string fdqnFromDomain = default(string);
				string text = default(string);
				string userPrincipalName = default(string);
				string result = default(string);
				while (true)
				{
					switch (num2)
					{
					case 2:
						num = 1;
						if (num != 0)
						{
						}
						if (string.IsNullOrEmpty(exactDomain))
						{
							num = 0;
							_ = num;
							num = 1;
							num2 = num;
						}
						else
						{
							fdqnFromDomain = GetFdqnFromDomain(exactDomain, log);
							text = a(fdqnFromDomain, log);
							num = 0;
							num2 = num;
						}
						continue;
					case 0:
						try
						{
							PrincipalContext val = new PrincipalContext((ContextType)1, fdqnFromDomain, text);
							try
							{
								UserPrincipal val2 = UserPrincipal.FindByIdentity(val, origUsername);
								try
								{
									num = 3;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 3:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											if (val2 == null)
											{
												num = 4;
												num2 = num;
											}
											else
											{
												userPrincipalName = ((Principal)val2).UserPrincipalName;
												num = 1;
												num2 = num;
											}
											continue;
										case 4:
										{
											string text3 = "Failed to lookup UPN tied using original username: " + origUsername;
											text3 += "\nFalling back to SamAccountName";
											log.AppendDebugLine(string.Format(text3));
											result = origUsername;
											num = 2;
											num2 = num;
											continue;
										}
										case 2:
											break;
										case 6:
										{
											string text2 = exactUsername + "@" + fdqnFromDomain;
											log.AppendDebugLine("GetUpnUsername userPrincipalName null.  Returning " + text2);
											result = text2;
											num = 0;
											num2 = num;
											continue;
										}
										case 0:
											break;
										case 1:
										{
											if (string.IsNullOrEmpty(userPrincipalName))
											{
												num = 6;
												num2 = num;
												continue;
											}
											result = userPrincipalName;
											num = 19892;
											short num3 = num;
											num = 19892;
											switch (num3 == num)
											{
											case false:
											case true:
												break;
											default:
												num = 0;
												if (num != 0)
												{
												}
												num = 5;
												num2 = num;
												continue;
											}
											break;
										}
										case 5:
											break;
										}
										break;
									}
								}
								finally
								{
									num = 1;
									num2 = num;
									while (true)
									{
										switch (num2)
										{
										case 1:
											switch (0)
											{
											default:
												continue;
											case 0:
												break;
											}
											goto default;
										default:
											if (val2 != null)
											{
												num = 2;
												num2 = num;
												continue;
											}
											break;
										case 2:
											((IDisposable)val2).Dispose();
											num = 0;
											num2 = num;
											continue;
										case 0:
											break;
										}
										break;
									}
								}
							}
							finally
							{
								num = 1;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 1:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (val != null)
										{
											num = 2;
											num2 = num;
											continue;
										}
										break;
									case 2:
										((IDisposable)val).Dispose();
										num = 0;
										num2 = num;
										continue;
									case 0:
										break;
									}
									break;
								}
							}
						}
						catch (Exception arg)
						{
							log.AppendDebugLine(string.Format("An exception occurred while resolving user.  Deriving UPN from the username and domain.  {0}", arg));
							result = exactUsername + "@" + fdqnFromDomain;
						}
						return result;
					case 1:
						return exactUsername;
					}
					break;
				}
				goto case 0;
			}
			case 0:
				log.AppendDebugLine(string.Format("Duo has been configured for UPN username format. Translating from: " + origUsername));
				exactUsername = "";
				exactDomain = "";
				ParseExactUsernameDomain(origUsername, "", out exactUsername, out exactDomain);
				log.AppendDebugLine(string.Format("Split into user = {0}; domain = {1}", exactUsername, exactDomain));
				num = 2;
				num2 = num;
				goto default;
			}
		}
		}
	}

	private static string a(string A_0, IDuoLogger A_1)
	{
		int a_ = 14;
		string a_2 = "Q8TNTSDPWD1HMITZ64T7XSH1O4F0OCADD3RG4V1UYFCMKRGOU1LVD7YXBZM3H";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			int num4 = default(int);
			StringBuilder stringBuilder = default(StringBuilder);
			string[] array = default(string[]);
			switch (0)
			{
			default:
				while (true)
				{
					num = 1;
					if (num != 0)
					{
					}
					switch (num2)
					{
					case 2:
						num4++;
						num = 6;
						num2 = num;
						continue;
					case 3:
						stringBuilder.Append(",");
						num = 2;
						num2 = num;
						continue;
					case 0:
						if (num4 < array.Length - 1)
						{
							num = 3;
							num2 = num;
							continue;
						}
						goto case 2;
					case 1:
					case 6:
						num = 4;
						num2 = num;
						continue;
					case 4:
						if (num4 < array.Length)
						{
							string arg = array[num4];
							stringBuilder.Append(string.Format("DC={0}", arg));
							num = 0;
							num2 = num;
						}
						else
						{
							num = 0;
							_ = num;
							num = 5;
							num2 = num;
						}
						continue;
					case 5:
					{
						num = 28674;
						short num3 = num;
						num = 28674;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							A_1.AppendDebugLine(string.Format("Parsed FQDN: {0} into Base DN: {1}", A_0, stringBuilder.ToString()));
							return stringBuilder.ToString();
						}
						goto case 4;
					}
					}
					break;
				}
				goto case 0;
			case 0:
				stringBuilder = new StringBuilder();
				array = A_0.Split(new char[1] { '.' });
				num4 = 0;
				num = 1;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static string ProtectRegistryValue(string registryValue, string entropyString)
	{
		short num = 7986;
		short num2 = num;
		num = 7986;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			byte[] bytes = Encoding.UTF8.GetBytes(registryValue);
			byte[] bytes2 = Encoding.UTF8.GetBytes(entropyString);
			return Convert.ToBase64String(ProtectedData.Protect(bytes, bytes2, (DataProtectionScope)1));
		}
		}
	}

	public static string ProtectRegistryValue(string registryValue, IDuoSecretProtection secretProtection)
	{
		short num = -29427;
		short num2 = num;
		num = -29427;
		switch (num2 == num)
		{
		default:
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 0;
			if (num != 0)
			{
			}
			return ProtectRegistryValue(registryValue, secretProtection.GetEntropyString());
		}
	}

	public static string UnProtectRegistryValue(string protectedRegistryValue, string entropyString)
	{
		short num = -13591;
		short num2 = num;
		num = -13591;
		switch (num2 == num)
		{
		default:
		{
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			byte[] array = Convert.FromBase64String(protectedRegistryValue);
			byte[] bytes = Encoding.UTF8.GetBytes(entropyString);
			byte[] bytes2 = ProtectedData.Unprotect(array, bytes, (DataProtectionScope)1);
			return Encoding.UTF8.GetString(bytes2);
		}
		}
	}

	public static string UnProtectRegistryValue(string protectedRegistryValue, IDuoSecretProtection secretProtection)
	{
		short num = 0;
		_ = num;
		num = 29476;
		short num2 = num;
		num = 29476;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			return UnProtectRegistryValue(protectedRegistryValue, secretProtection.GetEntropyString());
		}
	}

	public static string ObfuscateEntropy(string entropyString)
	{
		int a_ = 17;
		string a_2 = "GI0TG3KDB1FB63K33Y7OCD1BOGXTTI9WP3U6BDEHN8M5THB4NJMNLLD7";
		int num = default(int);
		int num4 = default(int);
		byte[] bytes = default(byte[]);
		byte[] array = default(byte[]);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 0:
				case 1:
					num2 = 2;
					num = num2;
					continue;
				case 2:
					if (num4 >= bytes.Length)
					{
						num2 = 3;
						num = num2;
						continue;
					}
					goto IL_011c;
				case 3:
					{
						num2 = 25740;
						short num3 = num2;
						num2 = 25740;
						switch (num3 == num2)
						{
						default:
							num2 = 0;
							if (num2 != 0)
							{
							}
							return Convert.ToBase64String(array);
						case false:
						case true:
							break;
						}
						goto IL_011c;
					}
					IL_011c:
					array[num4] = (byte)(bytes[num4] ^ "LwtdbfYssbgJMRghvfTpUPUUi8AMDj"[num4 % "LwtdbfYssbgJMRghvfTpUPUUi8AMDj".Length]);
					num4++;
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = 0;
					_ = num2;
					num2 = 1;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			bytes = Encoding.UTF8.GetBytes(entropyString);
			array = new byte[bytes.Length];
			num4 = 0;
			short num2 = 0;
			num = num2;
			goto default;
		}
		}
	}

	public static bool CompareSignatures(string callingFile)
	{
		int a_ = 18;
		string a_2 = "FUWFDPSZ7GSVVDZWWCM4S2LEPWUDWDSTI2ZBKKDVXG72AKOXFSN81E43N01X9H";
		int num3 = default(int);
		X509Certificate2 x509Certificate = default(X509Certificate2);
		X509Certificate2 x509Certificate2 = default(X509Certificate2);
		switch (0)
		{
		default:
			while (true)
			{
				short num = 22077;
				short num2 = num;
				num = 22077;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					switch (num3)
					{
					case 1:
						goto IL_00cc;
					case 0:
						goto IL_0103;
					case 3:
						goto IL_0143;
					case 2:
						num = 1;
						if (num != 0)
						{
						}
						return x509Certificate.Thumbprint == x509Certificate2.Thumbprint;
					}
					break;
				case false:
				case true:
					{
						Console.WriteLine("Thumbprints are not a match. Terminating");
						return false;
					}
					IL_00cc:
					if (x509Certificate != null)
					{
						num = 0;
						num3 = num;
						continue;
					}
					goto case false;
					IL_0143:
					num = 0;
					_ = num;
					if (x509Certificate2 != null)
					{
						num = 2;
						num3 = num;
						continue;
					}
					goto case false;
					IL_0103:
					num = 3;
					num3 = num;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			x509Certificate = a(Assembly.GetExecutingAssembly().Location);
			x509Certificate2 = a(callingFile);
			short num = 1;
			num3 = num;
			goto default;
		}
		}
	}

	private static X509Certificate2 a(string A_0)
	{
		int a_ = 1;
		string a_2 = "BEMBFTOHB8AU8G5UJHDBJ5LILC6Z073UR1V9EQFOJO2J7TA0ILZZ12D5";
		X509Certificate2 result;
		short num;
		try
		{
			num = -5706;
			short num2 = num;
			num = -5706;
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				result = new X509Certificate2(X509Certificate.CreateFromSignedFile(A_0));
				break;
			}
		}
		catch
		{
			Console.WriteLine("Error retrieving the signature for " + A_0);
			result = null;
		}
		num = 0;
		_ = num;
		num = 1;
		if (num != 0)
		{
		}
		return result;
	}

	public static bool ValidateKeys(string skey, string apiHost, string iKey)
	{
		int a_ = 5;
		string a_2 = "QV3LQX4S51V05JPEK7WG8FXIPXRZQZEW1ZAH8UBWFBMKGROURB3O";
		short num = 0;
		switch (num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 4;
			int num2 = num;
			while (true)
			{
				switch (num2)
				{
				case 4:
					switch (0)
					{
					default:
						goto end_IL_009b;
					case 0:
						break;
					}
					goto default;
				default:
					if (!string.IsNullOrEmpty(skey))
					{
						goto IL_00c7;
					}
					goto case 2;
				case 0:
					try
					{
						DuoApi duoApi = new DuoApi(iKey, skey, apiHost, "DuoUtil");
						Dictionary<string, string> parameters = new Dictionary<string, string>();
						long num4 = Convert.ToInt64(duoApi.JSONApiCall<Dictionary<string, object>>("GET", "/auth/v2/ping", parameters)["time"]);
						duoApi.JSONApiCall<Dictionary<string, object>>(date: new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num4), method: "GET", path: "/auth/v2/check", parameters: parameters, timeout: 0);
						return true;
					}
					catch
					{
						return false;
					}
				case 2:
					return false;
				case 3:
					num = 1;
					num2 = num;
					break;
				case 1:
					num = 0;
					_ = num;
					if (!string.IsNullOrEmpty(apiHost))
					{
						num = 32629;
						short num3 = num;
						num = 32629;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 6;
							num2 = num;
							goto end_IL_009b;
						}
						goto IL_00c7;
					}
					goto case 2;
				case 6:
					num = 5;
					num2 = num;
					break;
				case 5:
					{
						if (string.IsNullOrEmpty(iKey))
						{
							num = 2;
							num2 = num;
						}
						else
						{
							num = 0;
							num2 = num;
						}
						break;
					}
					IL_00c7:
					num = 3;
					num2 = num;
					break;
					end_IL_009b:
					break;
				}
			}
		}
		}
	}

	public static string GetRegistryValue(string keyPath, string valueName, RegistryView registryView = RegistryView.Registry64)
	{
		int a_ = 4;
		string a_2 = "C7PVGQ9SZ75P5Q92WERQIC5IRA3KSAG5OQW4VK6Q514B9ZUHTT4VC";
		string result = default(string);
		short num;
		try
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 13637;
			short num2 = num;
			num = 13637;
			switch (num2 == num)
			{
			default:
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				switch (num)
				{
				}
				break;
			case false:
			case true:
				break;
			}
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
			try
			{
				RegistryKey registryKey2 = registryKey.OpenSubKey(keyPath);
				try
				{
					num = 0;
					int num3 = num;
					while (true)
					{
						object obj;
						switch (num3)
						{
						case 0:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey2 == null)
							{
								num = 4;
								num3 = num;
							}
							else
							{
								num = 3;
								num3 = num;
							}
							continue;
						case 3:
						{
							object? value = registryKey2.GetValue(valueName, null);
							if (value == null)
							{
								num = 6;
								num3 = num;
								continue;
							}
							obj = value.ToString();
							goto IL_01e8;
						}
						case 6:
							obj = null;
							goto IL_01e8;
						case 4:
							num = 1;
							num3 = num;
							continue;
						case 1:
							throw new Exception("Failed to open the registry key: " + keyPath);
						case 5:
							obj = string.Empty;
							break;
						case 2:
							goto end_IL_0113;
							IL_01e8:
							if (obj == null)
							{
								num = 5;
								num3 = num;
								continue;
							}
							break;
						}
						result = (string)obj;
						num = 2;
						num3 = num;
						continue;
						end_IL_0113:
						break;
					}
				}
				finally
				{
					num = 0;
					int num3 = num;
					while (true)
					{
						switch (num3)
						{
						case 0:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey2 != null)
							{
								num = 1;
								num3 = num;
								continue;
							}
							break;
						case 1:
							((IDisposable)registryKey2).Dispose();
							num = 2;
							num3 = num;
							continue;
						case 2:
							break;
						}
						break;
					}
				}
			}
			finally
			{
				num = 1;
				int num3 = num;
				while (true)
				{
					switch (num3)
					{
					case 1:
						switch (0)
						{
						default:
							continue;
						case 0:
							break;
						}
						goto default;
					default:
						if (registryKey != null)
						{
							num = 0;
							num3 = num;
							continue;
						}
						break;
					case 0:
						((IDisposable)registryKey).Dispose();
						num = 2;
						num3 = num;
						continue;
					case 2:
						break;
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to read from registry: " + ex.Message, ex);
		}
		num = 0;
		_ = num;
		return result;
	}

	public static void WriteToRegistry(string keyPath, string valueName, string value, RegistryView registryView = RegistryView.Registry64)
	{
		int a_ = 9;
		string a_2 = "ZZJJ0B20FBR0PBIX8MVP1UQULROBYP2B0P9L0X8CWWX1A0E64WVCS765W";
		short num;
		try
		{
			num = 486;
			short num2 = num;
			num = 486;
			switch (num2 == num)
			{
			default:
			{
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);
				try
				{
					num = 1;
					int num3 = num;
					RegistryKey registryKey2 = default(RegistryKey);
					while (true)
					{
						switch (num3)
						{
						case 1:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey == null)
							{
								num = 0;
								num3 = num;
							}
							else
							{
								registryKey2 = registryKey.CreateSubKey(keyPath, writable: true);
								num = 2;
								num3 = num;
							}
							continue;
						case 0:
							throw new Exception(string.Format("Failed to open the base registry key for view: {0}", registryView));
						case 2:
							try
							{
								num = 2;
								num3 = num;
								while (true)
								{
									switch (num3)
									{
									case 2:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (registryKey2 == null)
										{
											num = 0;
											num3 = num;
										}
										else
										{
											registryKey2.SetValue(valueName, value, RegistryValueKind.String);
											num = 1;
											num3 = num;
										}
										continue;
									case 0:
										throw new Exception("Failed to open or create the registry key: " + keyPath);
									case 1:
										break;
									}
									break;
								}
							}
							finally
							{
								num = 2;
								num3 = num;
								while (true)
								{
									switch (num3)
									{
									case 2:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										if (registryKey2 != null)
										{
											num = 0;
											num3 = num;
											continue;
										}
										break;
									case 0:
										((IDisposable)registryKey2).Dispose();
										num = 1;
										num3 = num;
										continue;
									case 1:
										break;
									}
									break;
								}
							}
							num = 3;
							num3 = num;
							continue;
						case 3:
							break;
						}
						break;
					}
				}
				finally
				{
					num = 2;
					int num3 = num;
					while (true)
					{
						switch (num3)
						{
						case 2:
							switch (0)
							{
							default:
								continue;
							case 0:
								break;
							}
							goto default;
						default:
							if (registryKey != null)
							{
								num = 0;
								num3 = num;
								continue;
							}
							break;
						case 0:
							((IDisposable)registryKey).Dispose();
							num = 1;
							num3 = num;
							continue;
						case 1:
							break;
						}
						break;
					}
				}
				break;
			}
			case false:
			case true:
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(string.Format("keyPath={0}   valueName={1}   value={2}   registryView={3}", keyPath, valueName, value, registryView));
			throw new Exception("Failed to write to registry: " + ex.Message, ex);
		}
		num = 0;
		_ = num;
	}

	[DllImport("kernel32.dll")]
	public static extern SetErrorMode_Mode SetErrorMode(SetErrorMode_Mode newMode);

	[DllImport("advapi32.dll", SetLastError = true)]
	private static extern bool GetTokenInformation(IntPtr A_0, a A_1, IntPtr A_2, int A_3, out int A_4);

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr GetModuleHandleW(string A_0);

	[DllImport("psapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern uint GetModuleFileNameExW(IntPtr A_0, IntPtr A_1, [Out] StringBuilder A_2, int A_3);

	[DllImport("kernel32", SetLastError = true)]
	private static extern bool GetVersionEx(ref OSVERSIONINFOEX A_0);

	static DuoUtil()
	{
		int a_ = 19;
		string a_2 = "69IN1DJ6JSX2OIBYDO0R21FG01F263P9FMYOVEM70H39D933JTSX";
		short num = 23227;
		short num2 = num;
		num = 23227;
		switch (num2 == num)
		{
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		_ = num;
		num = 0;
		if (num != 0)
		{
		}
		NotYetInitialized = new Exception("Not Yet Initialized");
		DuoUtil.m_a = new Dictionary<string, RegistryHive>(StringComparer.InvariantCultureIgnoreCase)
		{
			{
				"HKEY_CLASSES_ROOT",
				RegistryHive.ClassesRoot
			},
			{
				"HKCR",
				RegistryHive.ClassesRoot
			},
			{
				"HKEY_CURRENT_CONFIG",
				RegistryHive.CurrentConfig
			},
			{
				"HKCC",
				RegistryHive.CurrentConfig
			},
			{
				"HKEY_CURRENT_USER",
				RegistryHive.CurrentUser
			},
			{
				"HKCU",
				RegistryHive.CurrentUser
			},
			{
				"HKEY_DYNAMIC_DATA",
				(RegistryHive)(-2147483642)
			},
			{
				"HKDD",
				(RegistryHive)(-2147483642)
			},
			{
				"HKEY_LOCAL_MACHINE",
				RegistryHive.LocalMachine
			},
			{
				"HKLM",
				RegistryHive.LocalMachine
			},
			{
				"HKEY_PERFORMANCE_DATA",
				RegistryHive.PerformanceData
			},
			{
				"HKPD",
				RegistryHive.PerformanceData
			},
			{
				"HKEY_USERS",
				RegistryHive.Users
			},
			{
				"HKU",
				RegistryHive.Users
			}
		};
		ClrVersion_450 = new Version("4.0.30319.0");
		ClrVersion_451 = new Version("4.0.30319.17001");
		ClrVersion_452 = new Version("4.0.30319.18401");
		ClrVersion_460 = new Version("4.6.79.0");
		DuoUtil.m_c = IntPtr.Subtract(IntPtr.Zero, 1);
	}
}
public static class StringExtensions
{
	public static bool EqualsNoCase(this string baseString, string comparerString)
	{
		short num = 0;
		_ = num;
		num = 12860;
		short num2 = num;
		num = 12860;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			return string.Equals(baseString, comparerString, StringComparison.InvariantCultureIgnoreCase);
		}
	}

	public static bool StaticEqualsNoCase(string baseString, string comparerString)
	{
		short num = 1;
		if (num != 0)
		{
		}
		num = -25763;
		short num2 = num;
		num = -25763;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return string.Equals(baseString, comparerString, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
public static class CertExtensions
{
	[CompilerGenerated]
	private sealed class a
	{
		public Oid a;

		internal bool b(Oid A_0)
		{
			short num = 0;
			_ = num;
			num = -12645;
			short num2 = num;
			num = -12645;
			switch (num2 == num)
			{
			default:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				return A_0.Value.Equals(a.Value);
			}
		}
	}

	[CompilerGenerated]
	private sealed class b
	{
		public Oid a;

		internal bool b(Oid A_0)
		{
			short num = -8093;
			short num2 = num;
			num = -8093;
			switch (num2 == num)
			{
			default:
				num = 1;
				if (num != 0)
				{
				}
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				return A_0.Value.Equals(a.Value);
			}
		}
	}

	public static readonly Oid KeyExtendedUsage_ServerAuthentication;

	public static readonly Oid KeyExtendedUsage_ClientAuthentication;

	public static readonly Oid KeyExtendedUsage_CodeSigning;

	public static readonly Oid KeyExtendedUsage_Email;

	public static readonly Oid KeyExtendedUsage_Timestamping;

	public static readonly Oid CertificateExtensionId_SubjectAlternateName;

	public static bool KeyBasicConstraintIsCertificateAuthority(this X509Certificate2 certificateToCheck)
	{
		short num = 0;
		_ = num;
		num = -29836;
		short num2 = num;
		num = -29836;
		bool result = default(bool);
		switch (num2 == num)
		{
		default:
		{
			num = 1;
			if (num != 0)
			{
			}
			num = 0;
			if (num != 0)
			{
			}
			int num3 = default(int);
			X509BasicConstraintsExtension x509BasicConstraintsExtension = default(X509BasicConstraintsExtension);
			switch (0)
			{
			default:
				while (true)
				{
					switch (num3)
					{
					case 2:
						if (x509BasicConstraintsExtension != null)
						{
							num = 0;
							num3 = num;
							continue;
						}
						goto end_IL_00d1;
					case 0:
						result = x509BasicConstraintsExtension.CertificateAuthority;
						num = 1;
						num3 = num;
						continue;
					case 1:
						goto end_IL_00d1;
					}
					break;
				}
				goto case 0;
			case 0:
				{
					result = true;
					x509BasicConstraintsExtension = certificateToCheck.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault();
					num = 2;
					num3 = num;
					goto default;
				}
				end_IL_00d1:
				break;
			}
			break;
		}
		case false:
		case true:
			break;
		}
		return result;
	}

	public static bool KeyEnhancedUsageValidForExact(this X509Certificate2 certificateToCheck, Oid[] requiredExactMatchUsageOids)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		}
		X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = default(X509EnhancedKeyUsageExtension);
		a a = default(a);
		IEnumerator<Oid> enumerator = default(IEnumerator<Oid>);
		bool result = default(bool);
		while (true)
		{
			switch (0)
			{
			default:
				while (true)
				{
					num = -30199;
					short num3 = num;
					num = -30199;
					switch (num3 == num)
					{
					case false:
					case true:
						break;
					default:
						goto IL_0074;
					}
					break;
					IL_0074:
					num = 1;
					if (num != 0)
					{
					}
					num = 0;
					if (num != 0)
					{
					}
					switch (num2)
					{
					case 0:
						if (x509EnhancedKeyUsageExtension != null)
						{
							num = 1;
							num2 = num;
							continue;
						}
						goto IL_03d0;
					case 4:
						try
						{
							num = 0;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 0:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								case 6:
									if (requiredExactMatchUsageOids.FirstOrDefault(a.b) != null)
									{
										num = 3;
										num2 = num;
										continue;
									}
									break;
								default:
									num = 4;
									num2 = num;
									continue;
								case 4:
									if (enumerator.MoveNext())
									{
										a = new a();
										a.a = enumerator.Current;
										result = false;
										num = 6;
										num2 = num;
									}
									else
									{
										num = 5;
										num2 = num;
									}
									continue;
								case 3:
									result = true;
									num = 1;
									num2 = num;
									continue;
								case 5:
									break;
								case 2:
									goto end_IL_0170;
								}
								num = 2;
								num2 = num;
								continue;
								end_IL_0170:
								break;
							}
						}
						finally
						{
							num = 0;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 0:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (enumerator != null)
									{
										num = 2;
										num2 = num;
										continue;
									}
									break;
								case 2:
									enumerator.Dispose();
									num = 1;
									num2 = num;
									continue;
								case 1:
									break;
								}
								break;
							}
						}
						goto IL_03d0;
					case 2:
						enumerator = x509EnhancedKeyUsageExtension.EnhancedKeyUsages.OfType<Oid>().GetEnumerator();
						num = 4;
						num2 = num;
						continue;
					case 1:
						num = 3;
						num2 = num;
						continue;
					case 3:
						{
							num = 0;
							_ = num;
							if (requiredExactMatchUsageOids.Length == x509EnhancedKeyUsageExtension.EnhancedKeyUsages.Count)
							{
								num = 2;
								num2 = num;
								continue;
							}
							goto IL_03d0;
						}
						IL_03d0:
						return result;
					}
					goto case 0;
				}
				break;
			case 0:
				result = false;
				x509EnhancedKeyUsageExtension = certificateToCheck.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
				num = 0;
				num2 = num;
				goto default;
			}
		}
	}

	public static bool KeyEnhancedUsageValidFor(this X509Certificate2 certificateToCheck, Oid oidToCheck)
	{
		int num = default(int);
		X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = default(X509EnhancedKeyUsageExtension);
		b b = default(b);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 3:
					if (x509EnhancedKeyUsageExtension != null)
					{
						short num2 = 0;
						num = num2;
						continue;
					}
					goto case 2;
				case 0:
				{
					short num2 = 1;
					num = num2;
					continue;
				}
				case 1:
					if (x509EnhancedKeyUsageExtension.EnhancedKeyUsages.OfType<Oid>().FirstOrDefault(b.b) != null)
					{
						short num2 = -7298;
						short num3 = num2;
						num2 = -7298;
						switch (num3 == num2)
						{
						case false:
						case true:
							break;
						default:
							num2 = 0;
							if (num2 != 0)
							{
							}
							num2 = 1;
							if (num2 != 0)
							{
							}
							num2 = 4;
							num = num2;
							continue;
						}
						goto case 0;
					}
					goto case 2;
				case 4:
				{
					short num2 = 0;
					_ = num2;
					result = true;
					num2 = 2;
					num = num2;
					continue;
				}
				case 2:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			b = new b();
			b.a = oidToCheck;
			result = false;
			x509EnhancedKeyUsageExtension = certificateToCheck.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
			short num2 = 3;
			num = num2;
			goto default;
		}
		}
	}

	public static bool KeyValidForUseExact(this X509Certificate2 certificateToCheck, X509KeyUsageFlags acceptableUsages)
	{
		int num = default(int);
		X509KeyUsageExtension x509KeyUsageExtension = default(X509KeyUsageExtension);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 0:
				{
					short num2 = 1;
					if (num2 != 0)
					{
					}
					if (x509KeyUsageExtension != null)
					{
						num2 = 1;
						num = num2;
						continue;
					}
					goto case 2;
				}
				case 1:
				{
					short num2 = 22618;
					short num3 = num2;
					num2 = 22618;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 == 0)
						{
						}
						break;
					case false:
					case true:
						break;
					}
					num2 = 4;
					num = num2;
					continue;
				}
				case 3:
				{
					result = true;
					short num2 = 2;
					num = num2;
					continue;
				}
				case 4:
					if (x509KeyUsageExtension.KeyUsages == acceptableUsages)
					{
						short num2 = 3;
						num = num2;
						continue;
					}
					goto case 2;
				case 2:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = false;
			x509KeyUsageExtension = certificateToCheck.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
			short num2 = 0;
			_ = num2;
			num2 = 0;
			num = num2;
			goto default;
		}
		}
	}

	public static bool KeyValidForUse(this X509Certificate2 certificateToCheck, X509KeyUsageFlags flagToCheck)
	{
		int num = default(int);
		X509KeyUsageExtension x509KeyUsageExtension = default(X509KeyUsageExtension);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 1:
					if (x509KeyUsageExtension != null)
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					goto case 0;
				case 2:
				{
					short num2;
					while (true)
					{
						result = x509KeyUsageExtension.KeyUsages.HasFlag(flagToCheck);
						num2 = -5280;
						short num3 = num2;
						num2 = -5280;
						switch (num3 == num2)
						{
						case false:
						case true:
							continue;
						}
						break;
					}
					num2 = 0;
					_ = num2;
					num2 = 0;
					if (num2 != 0)
					{
					}
					num2 = 0;
					num = num2;
					continue;
				}
				case 0:
				{
					short num2 = 1;
					if (num2 != 0)
					{
					}
					return result;
				}
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = false;
			x509KeyUsageExtension = certificateToCheck.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
			short num2 = 1;
			num = num2;
			goto default;
		}
		}
	}

	public static X509KeyUsageFlags KeyValidUsages(this X509Certificate2 certificateToCheck)
	{
		int num = default(int);
		X509KeyUsageExtension x509KeyUsageExtension = default(X509KeyUsageExtension);
		X509KeyUsageFlags result = default(X509KeyUsageFlags);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 0:
					if (x509KeyUsageExtension != null)
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					goto IL_0171;
				case 2:
				{
					result = x509KeyUsageExtension.KeyUsages;
					short num2 = 9706;
					short num3 = num2;
					num2 = 9706;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 1;
						num = num2;
						continue;
					}
					goto case 0;
				}
				case 1:
					{
						short num2 = 0;
						_ = num2;
						num2 = 1;
						if (num2 == 0)
						{
						}
						goto IL_0171;
					}
					IL_0171:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = X509KeyUsageFlags.None;
			x509KeyUsageExtension = certificateToCheck.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault();
			short num2 = 0;
			num = num2;
			goto default;
		}
		}
	}

	public static Oid[] KeyEnhancedUsages(this X509Certificate2 certificateToCheck)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = default(X509EnhancedKeyUsageExtension);
			List<Oid> list = default(List<Oid>);
			switch (0)
			{
			default:
			{
				OidEnumerator enumerator = default(OidEnumerator);
				while (true)
				{
					switch (num2)
					{
					case 5:
						num = 1;
						if (num != 0)
						{
						}
						if (x509EnhancedKeyUsageExtension != null)
						{
							num = 0;
							num2 = num;
							continue;
						}
						goto case 4;
					case 1:
					case 3:
						num = 2;
						num2 = num;
						continue;
					case 2:
						goto IL_0114;
					case 0:
						enumerator = x509EnhancedKeyUsageExtension.EnhancedKeyUsages.GetEnumerator();
						num = 1;
						num2 = num;
						continue;
					case 4:
						return list.ToArray();
					}
					break;
					IL_0114:
					if (!enumerator.MoveNext())
					{
						num = 1429;
						short num3 = num;
						num = 1429;
						switch (num3 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 4;
							num2 = num;
							continue;
						}
						goto IL_0069;
					}
					num = 0;
					_ = num;
					Oid current = enumerator.Current;
					list.Add(current);
					num = 3;
					num2 = num;
				}
				goto case 0;
			}
			case 0:
				{
					list = new List<Oid>();
					x509EnhancedKeyUsageExtension = certificateToCheck.Extensions.OfType<X509EnhancedKeyUsageExtension>().FirstOrDefault();
					goto IL_0069;
				}
				IL_0069:
				num = 5;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static X509Extension GetExtension(this X509Certificate2 sourceCertificate, Oid extensionIdToGet)
	{
		int num = default(int);
		X509Extension result = default(X509Extension);
		switch (0)
		{
		default:
		{
			X509Extension current = default(X509Extension);
			X509ExtensionEnumerator enumerator = default(X509ExtensionEnumerator);
			while (true)
			{
				short num2;
				switch (num)
				{
				case 1:
					if (sourceCertificate.Extensions != null)
					{
						num2 = 5;
						num = num2;
						continue;
					}
					goto case 3;
				case 6:
					result = current;
					num2 = 4;
					num = num2;
					continue;
				case 0:
					if (current.Oid.Value.Equals(extensionIdToGet.Value, StringComparison.InvariantCultureIgnoreCase))
					{
						num2 = 6;
						num = num2;
						continue;
					}
					goto case 2;
				case 5:
				{
					enumerator = sourceCertificate.Extensions.GetEnumerator();
					num2 = 18874;
					short num3 = num2;
					num2 = 18874;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 1;
						if (num2 != 0)
						{
						}
						num2 = 2;
						num = num2;
						continue;
					case false:
					case true:
						break;
					}
					goto IL_021e;
				}
				case 2:
					num2 = 7;
					num = num2;
					continue;
				case 7:
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						num2 = 0;
						num = num2;
						continue;
					}
					goto IL_021e;
				case 3:
				case 4:
					{
						return result;
					}
					IL_021e:
					num2 = 0;
					_ = num2;
					num2 = 3;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			result = null;
			short num2 = 1;
			num = num2;
			goto default;
		}
		}
	}

	public static string[] GetSubjectAlternateNames(this X509Certificate2 certificateToCheck)
	{
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Invalid comparison between Unknown and I4
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Expected O, but got Unknown
		int a_ = 14;
		string a_2 = "DQCSGTMEU79YS8CK335RKI3UZL47YHFF7ZJ3TE35J0J3ZH9J4QSWCX5SJM";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			X509Extension extension = default(X509Extension);
			List<string> list = default(List<string>);
			switch (0)
			{
			default:
			{
				IEnumerator enumerator = default(IEnumerator);
				IAlternativeName val = default(IAlternativeName);
				IDisposable disposable = default(IDisposable);
				while (true)
				{
					switch (num2)
					{
					case 1:
					{
						num = 0;
						_ = num;
						num = 17971;
						short num3 = num;
						num = 17971;
						switch (num3 == num)
						{
						case false:
						case true:
							goto IL_01a8;
						}
						num = 0;
						if (num != 0)
						{
						}
						if (extension != null)
						{
							num = 0;
							num2 = num;
							continue;
						}
						goto IL_017e;
					}
					case 0:
					{
						num = 1;
						if (num != 0)
						{
						}
						CX509ExtensionAlternativeNamesClass val2 = new CX509ExtensionAlternativeNamesClass();
						string text = BitConverter.ToString(extension.RawData).Replace("-", "");
						((IX509ExtensionAlternativeNames)val2).InitializeDecode((EncodingType)4, text);
						enumerator = ((IAlternativeNames)((IX509ExtensionAlternativeNames)val2).AlternativeNames).GetEnumerator();
						num = 2;
						num2 = num;
						continue;
					}
					case 3:
						if (list.Count <= 0)
						{
							return null;
						}
						goto IL_01a8;
					case 4:
						return list.ToArray();
					case 2:
						{
							try
							{
								num = 4;
								num2 = num;
								while (true)
								{
									switch (num2)
									{
									case 4:
										switch (0)
										{
										default:
											continue;
										case 0:
											break;
										}
										goto default;
									default:
										num = 2;
										num2 = num;
										continue;
									case 2:
										if (!enumerator.MoveNext())
										{
											num = 3;
											num2 = num;
										}
										else
										{
											val = (IAlternativeName)enumerator.Current;
											num = 5;
											num2 = num;
										}
										continue;
									case 1:
										list.Add(val.strValue);
										num = 6;
										num2 = num;
										continue;
									case 5:
										if ((int)val.Type == 3)
										{
											num = 1;
											num2 = num;
											continue;
										}
										goto default;
									case 3:
										num = 0;
										num2 = num;
										continue;
									case 0:
										break;
									}
									break;
								}
							}
							finally
							{
								switch (0)
								{
								default:
									while (true)
									{
										switch (num2)
										{
										case 1:
											if (disposable != null)
											{
												num = 0;
												num2 = num;
												continue;
											}
											goto end_IL_0383;
										case 0:
											disposable.Dispose();
											num = 2;
											num2 = num;
											continue;
										case 2:
											goto end_IL_0383;
										}
										goto case 0;
										continue;
										end_IL_0383:
										break;
									}
									break;
								case 0:
									disposable = enumerator as IDisposable;
									num = 1;
									num2 = num;
									goto default;
								}
							}
							goto IL_017e;
						}
						IL_01a8:
						num = 4;
						num2 = num;
						continue;
						IL_017e:
						num = 3;
						num2 = num;
						continue;
					}
					break;
				}
				goto case 0;
			}
			case 0:
				extension = certificateToCheck.GetExtension(CertificateExtensionId_SubjectAlternateName);
				list = new List<string>();
				num = 1;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static bool HasSubjectAlternateNames(this X509Certificate2 certificateToCheck)
	{
		string[] subjectAlternateNames;
		short num;
		while (true)
		{
			subjectAlternateNames = certificateToCheck.GetSubjectAlternateNames();
			if (subjectAlternateNames != null)
			{
				break;
			}
			num = -1476;
			short num2 = num;
			num = -1476;
			switch (num2 == num)
			{
			case false:
			case true:
				continue;
			}
			num = 0;
			if (num != 0)
			{
			}
			num = 1;
			if (num != 0)
			{
			}
			return false;
		}
		num = 0;
		_ = num;
		return subjectAlternateNames.Length != 0;
	}

	public static string[] GetSubjectAlternateNames_Wildcard(this X509Certificate2 certificateToCheck)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			num = 0;
			_ = num;
			string[] subjectAlternateNames = default(string[]);
			List<string> list = default(List<string>);
			switch (0)
			{
			default:
			{
				int num3 = default(int);
				string text = default(string);
				string[] array = default(string[]);
				while (true)
				{
					switch (num2)
					{
					case 10:
						if (subjectAlternateNames != null)
						{
							num = 3;
							num2 = num;
							continue;
						}
						goto case 6;
					case 8:
						num3++;
						num = 2;
						num2 = num;
						continue;
					case 5:
						num = 4;
						num2 = num;
						continue;
					case 4:
						if (text.IndexOf('*') >= 0)
						{
							num = 1;
							num2 = num;
							continue;
						}
						goto case 8;
					case 3:
						array = subjectAlternateNames;
						num3 = 0;
						num = 1;
						if (num != 0)
						{
						}
						num = 0;
						num2 = num;
						continue;
					case 0:
					case 2:
						num = 7;
						num2 = num;
						continue;
					case 7:
					{
						if (num3 >= array.Length)
						{
							num = 6;
							num2 = num;
							continue;
						}
						num = -5765;
						short num4 = num;
						num = -5765;
						switch (num4 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							text = array[num3];
							num = 9;
							num2 = num;
							continue;
						}
						goto case 3;
					}
					case 9:
						if (text != null)
						{
							num = 5;
							num2 = num;
							continue;
						}
						goto case 8;
					case 1:
						list.Add(text);
						num = 8;
						num2 = num;
						continue;
					case 6:
						return list.ToArray();
					}
					break;
				}
				goto case 0;
			}
			case 0:
				subjectAlternateNames = certificateToCheck.GetSubjectAlternateNames();
				list = new List<string>();
				num = 10;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static string[] GetSubjectAlternateNames_Hostname(this X509Certificate2 certificateToCheck)
	{
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			string[] subjectAlternateNames = default(string[]);
			List<string> list = default(List<string>);
			switch (0)
			{
			default:
			{
				int num3 = default(int);
				string text = default(string);
				string[] array = default(string[]);
				while (true)
				{
					switch (num2)
					{
					case 10:
						if (subjectAlternateNames != null)
						{
							num = 5;
							num2 = num;
							continue;
						}
						goto case 9;
					case 1:
						num3++;
						num = 1;
						if (num != 0)
						{
						}
						num = 4;
						num2 = num;
						continue;
					case 2:
						num = 0;
						num2 = num;
						continue;
					case 0:
						if (text.IndexOf('*') == -1)
						{
							num = 8;
							num2 = num;
							continue;
						}
						goto case 1;
					case 5:
						array = subjectAlternateNames;
						num3 = 0;
						num = 6;
						num2 = num;
						continue;
					case 4:
					case 6:
					{
						num = -218;
						short num4 = num;
						num = -218;
						switch (num4 == num)
						{
						case false:
						case true:
							break;
						default:
							num = 0;
							if (num != 0)
							{
							}
							num = 3;
							num2 = num;
							continue;
						}
						goto case 5;
					}
					case 3:
						if (num3 >= array.Length)
						{
							num = 9;
							num2 = num;
						}
						else
						{
							text = array[num3];
							num = 7;
							num2 = num;
						}
						continue;
					case 7:
						if (text != null)
						{
							num = 0;
							_ = num;
							num = 2;
							num2 = num;
							continue;
						}
						goto case 1;
					case 8:
						list.Add(text);
						num = 1;
						num2 = num;
						continue;
					case 9:
						return list.ToArray();
					}
					break;
				}
				goto case 0;
			}
			case 0:
				subjectAlternateNames = certificateToCheck.GetSubjectAlternateNames();
				list = new List<string>();
				num = 10;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static bool HostnameMatchesAnySubjectAlternateName(this X509Certificate2 certificateToCheck, string hostnameToCheck)
	{
		int a_ = 12;
		string a_2 = "VFY3E0JJRZ557D1T62OZ690P7KSLZQON21XMLHWV411YV8DRW2T";
		short num = 0;
		int num2 = num;
		switch (num2)
		{
		default:
		{
			string[] subjectAlternateNames = default(string[]);
			bool flag = default(bool);
			switch (0)
			{
			default:
			{
				string text = default(string);
				string pattern = default(string);
				int num3 = default(int);
				string[] array = default(string[]);
				while (true)
				{
					switch (num2)
					{
					case 2:
						if (subjectAlternateNames != null)
						{
							num = 3;
							num2 = num;
							continue;
						}
						goto IL_0543;
					case 3:
						goto IL_00af;
					case 5:
						try
						{
							num = 7;
							num2 = num;
							while (true)
							{
								switch (num2)
								{
								case 13:
									break;
								case 7:
									switch (0)
									{
									default:
										continue;
									case 0:
										break;
									}
									goto default;
								default:
									if (!string.IsNullOrWhiteSpace(text))
									{
										num = 10;
										num2 = num;
										continue;
									}
									goto case 6;
								case 12:
									num = 2;
									num2 = num;
									continue;
								case 2:
									goto end_IL_0144;
								case 3:
									flag = text.Equals(hostnameToCheck, StringComparison.InvariantCultureIgnoreCase);
									num = 6;
									num2 = num;
									continue;
								case 6:
								case 9:
									num = 11;
									num2 = num;
									continue;
								case 11:
									if (flag)
									{
										num = 12;
										num2 = num;
									}
									else
									{
										num = 13;
										num2 = num;
									}
									continue;
								case 0:
								case 8:
									flag = new Regex(pattern).IsMatch(hostnameToCheck);
									num = 9;
									num2 = num;
									continue;
								case 1:
									if (!text.Equals("*"))
									{
										string text2 = Regex.Escape(text);
										pattern = string.Format("\\A{0}\\Z", text2.Replace("\\*", "[^.]*"));
										num = 0;
										num2 = num;
									}
									else
									{
										num = 4;
										num2 = num;
									}
									continue;
								case 10:
									num = 5;
									num2 = num;
									continue;
								case 5:
									if (text.IndexOf('*') != -1)
									{
										num = 1;
										num2 = num;
									}
									else
									{
										num = 3;
										num2 = num;
									}
									continue;
								case 4:
									pattern = "\\A[^.]+\\Z";
									num = 8;
									num2 = num;
									continue;
								}
								break;
							}
							goto IL_011d;
							end_IL_0144:;
						}
						catch
						{
							goto IL_011d;
						}
						goto IL_0543;
					case 1:
					case 6:
						num = 0;
						num2 = num;
						continue;
					case 0:
						goto IL_0480;
					case 4:
						{
							num = 0;
							_ = num;
							goto IL_0543;
						}
						IL_011d:
						num3++;
						num = 1;
						num2 = num;
						continue;
						IL_0543:
						return flag;
					}
					break;
					IL_0480:
					num = 1;
					if (num != 0)
					{
					}
					if (num3 >= array.Length)
					{
						num = 4;
						num2 = num;
					}
					else
					{
						text = array[num3];
						num = 5;
						num2 = num;
					}
					continue;
					IL_00af:
					num = 29877;
					short num4 = num;
					num = 29877;
					switch (num4 == num)
					{
					case false:
					case true:
						break;
					default:
						num = 0;
						if (num != 0)
						{
						}
						array = subjectAlternateNames;
						num3 = 0;
						num = 6;
						num2 = num;
						continue;
					}
					goto IL_0072;
				}
				goto case 0;
			}
			case 0:
				{
					subjectAlternateNames = certificateToCheck.GetSubjectAlternateNames();
					flag = false;
					goto IL_0072;
				}
				IL_0072:
				num = 2;
				num2 = num;
				goto default;
			}
		}
		}
	}

	public static bool HasSubjectAlternateName_Wildcard(this X509Certificate2 certificateToCheck)
	{
		int num = default(int);
		string[] subjectAlternateNames_Wildcard = default(string[]);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 2:
				{
					num2 = 28358;
					short num3 = num2;
					num2 = 28358;
					switch (num3 == num2)
					{
					case false:
					case true:
						goto IL_00e0;
					}
					num2 = 0;
					if (num2 != 0)
					{
					}
					if (subjectAlternateNames_Wildcard != null)
					{
						num2 = 1;
						num = num2;
						continue;
					}
					goto IL_01bb;
				}
				case 3:
					goto IL_00e0;
				case 0:
					num2 = 1;
					if (num2 == 0)
					{
					}
					goto IL_01bb;
				case 1:
					num2 = 4;
					num = num2;
					continue;
				case 4:
					{
						if (subjectAlternateNames_Wildcard.Length != 0)
						{
							num2 = 3;
							num = num2;
							continue;
						}
						goto IL_01bb;
					}
					IL_01bb:
					return result;
					IL_00e0:
					result = true;
					num2 = 0;
					_ = num2;
					num2 = 0;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			subjectAlternateNames_Wildcard = certificateToCheck.GetSubjectAlternateNames_Wildcard();
			result = false;
			short num2 = 2;
			num = num2;
			goto default;
		}
		}
	}

	static CertExtensions()
	{
		int a_ = 2;
		string a_2 = "AQTO51JKKJS9NQUUUVYNIYOJKGKHB29EMVJAS4K6NH1XM08OD7VYPLH2VIXUSCR";
		short num = 17309;
		short num2 = num;
		num = 17309;
		switch (num2 == num)
		{
		}
		num = 0;
		if (num != 0)
		{
		}
		num = 1;
		if (num != 0)
		{
		}
		num = 0;
		_ = num;
		KeyExtendedUsage_ServerAuthentication = new Oid("1.3.6.1.5.5.7.3.1");
		KeyExtendedUsage_ClientAuthentication = new Oid("1.3.6.1.5.5.7.3.2");
		KeyExtendedUsage_CodeSigning = new Oid("1.3.6.1.5.5.7.3.3");
		KeyExtendedUsage_Email = new Oid("1.3.6.1.5.5.7.3.4");
		KeyExtendedUsage_Timestamping = new Oid("1.3.6.1.5.5.7.3.8");
		CertificateExtensionId_SubjectAlternateName = new Oid("2.5.29.17");
	}
}
public static class VersionExtensions
{
	public static bool EqualsMajorMinorInline(this Version sourceVersion, Version compareVersion)
	{
		short num = 1;
		int num2 = num;
		while (true)
		{
			switch (num2)
			{
			case 1:
				switch (0)
				{
				default:
					continue;
				case 0:
					break;
				}
				goto default;
			default:
			{
				if (sourceVersion.Major != compareVersion.Major)
				{
					break;
				}
				num = 30599;
				short num3 = num;
				num = 30599;
				switch (num3 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					num = 0;
					_ = num;
					num = 3;
					num2 = num;
					continue;
				case false:
				case true:
					break;
				}
				goto case 2;
			}
			case 0:
				return sourceVersion.Build == compareVersion.Build;
			case 3:
				num = 1;
				if (num != 0)
				{
				}
				num = 2;
				num2 = num;
				continue;
			case 2:
				if (sourceVersion.Minor == compareVersion.Minor)
				{
					num = 0;
					num2 = num;
					continue;
				}
				break;
			}
			break;
		}
		return false;
	}

	public static bool GreaterThanOrEquals_MajorMinorInline(this Version sourceVersion, Version compareVersion)
	{
		int num = default(int);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 8:
					if (sourceVersion.Major > compareVersion.Major)
					{
						short num2 = 0;
						_ = num2;
						num2 = 1;
						num = num2;
					}
					else
					{
						short num2 = 11;
						num = num2;
					}
					continue;
				case 2:
					if (sourceVersion.Minor == compareVersion.Minor)
					{
						short num2 = 10;
						num = num2;
						continue;
					}
					goto case 9;
				case 3:
				{
					result = true;
					short num2 = -7065;
					short num3 = num2;
					num2 = -7065;
					switch (num3 == num2)
					{
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 6;
						num = num2;
						continue;
					case false:
					case true:
						break;
					}
					goto case 9;
				}
				case 6:
				{
					short num2 = 1;
					if (num2 == 0)
					{
					}
					goto case 9;
				}
				case 11:
					if (sourceVersion.Major == compareVersion.Major)
					{
						short num2 = 4;
						num = num2;
						continue;
					}
					goto case 9;
				case 7:
				{
					result = true;
					short num2 = 12;
					num = num2;
					continue;
				}
				case 1:
				{
					result = true;
					short num2 = 9;
					num = num2;
					continue;
				}
				case 10:
				{
					short num2 = 0;
					num = num2;
					continue;
				}
				case 0:
					if (sourceVersion.Build >= compareVersion.Build)
					{
						short num2 = 7;
						num = num2;
						continue;
					}
					goto case 9;
				case 4:
				{
					short num2 = 5;
					num = num2;
					continue;
				}
				case 5:
					if (sourceVersion.Minor <= compareVersion.Minor)
					{
						short num2 = 2;
						num = num2;
					}
					else
					{
						short num2 = 3;
						num = num2;
					}
					continue;
				case 9:
				case 12:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = false;
			short num2 = 8;
			num = num2;
			goto default;
		}
		}
	}

	public static bool LessThanOrEquals_MajorMinorInline(this Version sourceVersion, Version compareVersion)
	{
		int num = default(int);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 8:
					if (sourceVersion.Major < compareVersion.Major)
					{
						num2 = 10;
						num = num2;
					}
					else
					{
						num2 = 12;
						num = num2;
					}
					continue;
				case 7:
					if (sourceVersion.Minor == compareVersion.Minor)
					{
						num2 = 9;
						num = num2;
						continue;
					}
					goto case 1;
				case 2:
					result = true;
					num2 = 0;
					num = num2;
					continue;
				case 0:
					num2 = 0;
					_ = num2;
					goto case 1;
				case 12:
					if (sourceVersion.Major == compareVersion.Major)
					{
						goto IL_021c;
					}
					goto case 1;
				case 4:
					result = true;
					num2 = 1;
					num = num2;
					continue;
				case 10:
					result = true;
					num2 = 5;
					num = num2;
					continue;
				case 9:
					num2 = 6;
					num = num2;
					continue;
				case 6:
					if (sourceVersion.Build <= compareVersion.Build)
					{
						num2 = 4;
						num = num2;
						continue;
					}
					goto case 1;
				case 3:
					num2 = 11;
					num = num2;
					continue;
				case 11:
					if (sourceVersion.Minor >= compareVersion.Minor)
					{
						num2 = 2906;
						short num3 = num2;
						num2 = 2906;
						switch (num3 == num2)
						{
						default:
							num2 = 1;
							if (num2 != 0)
							{
							}
							num2 = 0;
							if (num2 != 0)
							{
							}
							num2 = 7;
							num = num2;
							continue;
						case false:
						case true:
							break;
						}
						goto IL_021c;
					}
					num2 = 2;
					num = num2;
					continue;
				case 1:
				case 5:
					{
						return result;
					}
					IL_021c:
					num2 = 3;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = false;
			short num2 = 8;
			num = num2;
			goto default;
		}
		}
	}

	public static bool LessThan_MajorMinorInline(this Version sourceVersion, Version compareVersion)
	{
		int num = default(int);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				short num2;
				switch (num)
				{
				case 4:
					if (sourceVersion.Major >= compareVersion.Major)
					{
						num2 = 8;
						num = num2;
						continue;
					}
					goto IL_00b0;
				case 10:
					if (sourceVersion.Minor == compareVersion.Minor)
					{
						num2 = 2;
						num = num2;
						continue;
					}
					goto case 3;
				case 0:
				{
					result = true;
					num2 = -9131;
					short num3 = num2;
					num2 = -9131;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 3;
						num = num2;
						continue;
					}
					goto IL_00b0;
				}
				case 8:
					if (sourceVersion.Major == compareVersion.Major)
					{
						num2 = 5;
						num = num2;
						continue;
					}
					goto case 3;
				case 9:
					num2 = 1;
					if (num2 != 0)
					{
					}
					result = true;
					num2 = 11;
					num = num2;
					continue;
				case 1:
					result = true;
					num2 = 7;
					num = num2;
					continue;
				case 2:
					num2 = 12;
					num = num2;
					continue;
				case 12:
					if (sourceVersion.Build < compareVersion.Build)
					{
						num2 = 9;
						num = num2;
						continue;
					}
					goto case 3;
				case 5:
					num2 = 6;
					num = num2;
					continue;
				case 6:
					if (sourceVersion.Minor >= compareVersion.Minor)
					{
						num2 = 10;
						num = num2;
					}
					else
					{
						num2 = 0;
						num = num2;
					}
					continue;
				case 3:
				case 7:
				case 11:
					{
						return result;
					}
					IL_00b0:
					num2 = 1;
					num = num2;
					continue;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			short num2 = 0;
			_ = num2;
			result = false;
			num2 = 4;
			num = num2;
			goto default;
		}
		}
	}

	public static bool LessThanEquals_MajorMinor(this Version sourceVersion, Version compareVersion)
	{
		int num = default(int);
		bool result = default(bool);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 5:
					if (sourceVersion.Major < compareVersion.Major)
					{
						short num2 = 2;
						num = num2;
					}
					else
					{
						short num2 = 1;
						num = num2;
					}
					continue;
				case 4:
				{
					result = sourceVersion.Minor <= compareVersion.Minor;
					short num2 = 3;
					num = num2;
					continue;
				}
				case 1:
					if (sourceVersion.Major == compareVersion.Major)
					{
						short num2 = 11127;
						short num3 = num2;
						num2 = 11127;
						switch (num3 == num2)
						{
						default:
							num2 = 1;
							if (num2 != 0)
							{
							}
							num2 = 0;
							if (num2 != 0)
							{
							}
							num2 = 4;
							num = num2;
							continue;
						case false:
						case true:
							break;
						}
					}
					goto case 0;
				case 2:
				{
					result = true;
					short num2 = 0;
					_ = num2;
					num2 = 0;
					num = num2;
					continue;
				}
				case 0:
				case 3:
					return result;
				}
				break;
			}
			goto case 0;
		case 0:
		{
			result = false;
			short num2 = 5;
			num = num2;
			goto default;
		}
		}
	}
}
public static class ReflectionExtensions
{
	public const string InvalidEnumArgumentExceptionKey_AttemptedNewValue = "AttemptedNewValue";

	public const string InvalidEnumArgumentExceptionKey_AttemptedNewValueInt = "AttemptedNewValueInt";

	public static object GetPropertyValue(this object sourceObject, string propertyName)
	{
		short num = 0;
		_ = num;
		int num3 = default(int);
		object result = default(object);
		switch (0)
		{
		default:
		{
			PropertyInfo property = default(PropertyInfo);
			while (true)
			{
				num = -5446;
				short num2 = num;
				num = -5446;
				switch (num2 == num)
				{
				default:
					num = 0;
					if (num != 0)
					{
					}
					switch (num3)
					{
					case 1:
						if (sourceObject != null)
						{
							goto IL_00e4;
						}
						goto case 3;
					case 4:
						goto IL_0104;
					case 2:
						goto IL_012c;
					case 0:
						if (property != null)
						{
							goto IL_0169;
						}
						goto case 3;
					case 3:
						num = 1;
						if (num != 0)
						{
						}
						return result;
					}
					break;
				case false:
				case true:
					goto IL_012c;
					IL_00e4:
					num = 2;
					num3 = num;
					continue;
					IL_0169:
					num = 4;
					num3 = num;
					continue;
					IL_012c:
					property = sourceObject.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					num = 0;
					num3 = num;
					continue;
					IL_0104:
					result = property.GetValue(sourceObject, null);
					num = 3;
					num3 = num;
					continue;
				}
				break;
			}
			goto case 0;
		}
		case 0:
			result = null;
			num = 1;
			num3 = num;
			goto default;
		}
	}

	public static object GetFieldValue(this object sourceObject, string fieldName)
	{
		int num = default(int);
		object result = default(object);
		switch (0)
		{
		default:
		{
			FieldInfo field = default(FieldInfo);
			while (true)
			{
				switch (num)
				{
				case 3:
					if (sourceObject != null)
					{
						short num2 = 14690;
						short num3 = num2;
						num2 = 14690;
						switch (num3 == num2)
						{
						case false:
						case true:
							continue;
						}
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 2;
						num = num2;
						continue;
					}
					goto case 0;
				case 1:
				{
					result = field.GetValue(sourceObject);
					short num2 = 0;
					num = num2;
					continue;
				}
				case 2:
				{
					field = sourceObject.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					short num2 = 0;
					_ = num2;
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = 4;
					num = num2;
					continue;
				}
				case 4:
					if (field != null)
					{
						short num2 = 1;
						num = num2;
						continue;
					}
					goto case 0;
				case 0:
					return result;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			result = null;
			short num2 = 3;
			num = num2;
			goto default;
		}
		}
	}

	public static void SetValueFromInt(this PropertyInfo targetProperty, object targetObject, int newValue)
	{
		int a_ = 5;
		string a_2 = "2G4XMXJVIWXHT0OZA27JW7DAVBP9QMPX9CS7NBA5ID3YTSG5WMYPZABUUU";
		switch (0)
		{
		case 0:
			goto IL_0082;
		}
		goto IL_001c;
		IL_001c:
		int num = default(int);
		Type type = default(Type);
		PropertyInfo propertyInfo = default(PropertyInfo);
		short num2;
		while (true)
		{
			switch (num)
			{
			case 5:
				if (targetProperty.PropertyType.IsEnum)
				{
					num2 = 11;
					num = num2;
					continue;
				}
				goto case 21;
			case 2:
				if (type == typeof(uint))
				{
					num2 = 19;
					num = num2;
				}
				else
				{
					num2 = 23;
					num = num2;
				}
				continue;
			case 16:
				if (type == typeof(byte))
				{
					num2 = 14;
					num = num2;
				}
				else
				{
					num2 = 22;
					num = num2;
				}
				continue;
			case 9:
				propertyInfo = targetProperty.DeclaringType.GetProperty(targetProperty.Name, targetProperty.PropertyType);
				num2 = 4;
				num = num2;
				continue;
			case 23:
				if (type == typeof(ushort))
				{
					num2 = 18;
					num = num2;
				}
				else
				{
					num2 = 12;
					num = num2;
				}
				continue;
			case 20:
				num2 = 0;
				num = num2;
				continue;
			case 0:
				if (!targetProperty.CanWrite)
				{
					num2 = 9;
					num = num2;
					continue;
				}
				goto case 4;
			case 1:
				propertyInfo.SetValue(targetObject, (ulong)newValue, null);
				return;
			case 12:
				if (type == typeof(sbyte))
				{
					num2 = 15;
					num = num2;
					continue;
				}
				throw new InvalidCastException(string.Format("Property {0}.{1} is not an integer type", targetObject.GetType().Name, targetProperty.Name));
			case 10:
				propertyInfo.SetValue(targetObject, (short)newValue, null);
				return;
			case 4:
				CheckValidTypeAndValue(targetProperty, targetObject, newValue);
				num2 = 6;
				num = num2;
				continue;
			case 6:
				if (type == typeof(long))
				{
					num2 = 0;
					_ = num2;
					num2 = 8;
					num = num2;
				}
				else
				{
					num2 = 13;
					num = num2;
				}
				continue;
			case 18:
				propertyInfo.SetValue(targetObject, (ushort)newValue, null);
				return;
			case 15:
				propertyInfo.SetValue(targetObject, (sbyte)newValue, null);
				return;
			case 17:
				propertyInfo.SetValue(targetObject, newValue, null);
				return;
			case 11:
				type = type.GetEnumUnderlyingType();
				num2 = 21;
				num = num2;
				continue;
			case 8:
				propertyInfo.SetValue(targetObject, (long)newValue, null);
				return;
			case 21:
				propertyInfo = targetProperty;
				num2 = 7;
				num = num2;
				continue;
			case 7:
				if (targetProperty.ReflectedType != targetProperty.DeclaringType)
				{
					num2 = 20;
					num = num2;
					continue;
				}
				goto case 4;
			case 3:
				if (!(type == typeof(short)))
				{
					num2 = 16;
					num = num2;
				}
				else
				{
					num2 = 10;
					num = num2;
				}
				continue;
			case 19:
				propertyInfo.SetValue(targetObject, (uint)newValue, null);
				return;
			case 22:
				if (!(type == typeof(ulong)))
				{
					num2 = 2;
					num = num2;
				}
				else
				{
					num2 = 1;
					num = num2;
				}
				continue;
			case 13:
				if (!(type == typeof(int)))
				{
					num2 = 3;
					num = num2;
				}
				else
				{
					num2 = 17;
					num = num2;
				}
				continue;
			case 14:
				while (true)
				{
					num2 = -6724;
					short num3 = num2;
					num2 = -6724;
					switch (num3 == num2)
					{
					case false:
					case true:
						continue;
					}
					num2 = 0;
					if (num2 != 0)
					{
					}
					num2 = 1;
					if (num2 != 0)
					{
					}
					propertyInfo.SetValue(targetObject, (byte)newValue, null);
					return;
				}
			}
			break;
		}
		goto IL_0082;
		IL_0082:
		type = targetProperty.PropertyType;
		propertyInfo = null;
		num2 = 5;
		num = num2;
		goto IL_001c;
	}

	public static int GetValueAsInt(this PropertyInfo sourceProperty, object sourceObject)
	{
		int a_ = 18;
		string a_2 = "CBTWHWILXUW61PSM82J3DJ4PTWSVITTCM76NBHO7ZUEGWQ265DUX";
		int num2 = default(int);
		Type type = default(Type);
		switch (0)
		{
		default:
		{
			int result = default(int);
			object value = default(object);
			while (true)
			{
				short num = 1;
				if (num != 0)
				{
				}
				switch (num2)
				{
				case 13:
					if (type.IsEnum)
					{
						num = 22;
						num2 = num;
						continue;
					}
					goto case 16;
				case 26:
					result = (int)(uint)value;
					num = 15;
					num2 = num;
					continue;
				case 2:
					if (type == typeof(ulong))
					{
						num = 17;
						num2 = num;
					}
					else
					{
						num = 19;
						num2 = num;
					}
					continue;
				case 5:
					if (type == typeof(int))
					{
						num = 1;
						num2 = num;
					}
					else
					{
						num = 0;
						num2 = num;
					}
					continue;
				case 25:
					if (type == typeof(sbyte))
					{
						num = 7;
						num2 = num;
						continue;
					}
					throw new InvalidCastException(string.Format("Member {0}.{1} is not an integer type", sourceObject.GetType().Name, sourceProperty.Name));
				case 16:
					value = sourceProperty.GetValue(sourceObject, null);
					num = 3;
					num2 = num;
					continue;
				case 3:
					if (!(type == typeof(long)))
					{
						num = 5;
						num2 = num;
					}
					else
					{
						num = 4;
						num2 = num;
					}
					continue;
				case 19:
					if (type == typeof(uint))
					{
						num = 26;
						num2 = num;
					}
					else
					{
						num = 12;
						num2 = num;
					}
					continue;
				case 7:
					result = (sbyte)value;
					num = 14;
					num2 = num;
					continue;
				case 9:
					result = (byte)value;
					num = 11;
					num2 = num;
					continue;
				case 1:
				{
					num = -4528;
					short num3 = num;
					num = -4528;
					switch (num3 == num)
					{
					default:
						num = 0;
						if (num != 0)
						{
						}
						result = (int)value;
						num = 24;
						num2 = num;
						continue;
					case false:
					case true:
						break;
					}
					goto IL_0467;
				}
				case 6:
					result = (ushort)value;
					num = 8;
					num2 = num;
					continue;
				case 20:
					result = (short)value;
					goto IL_0467;
				case 22:
					type = type.GetEnumUnderlyingType();
					num = 16;
					num2 = num;
					continue;
				case 4:
					result = (int)(long)value;
					num = 0;
					_ = num;
					num = 18;
					num2 = num;
					continue;
				case 0:
					if (type == typeof(short))
					{
						num = 20;
						num2 = num;
					}
					else
					{
						num = 21;
						num2 = num;
					}
					continue;
				case 21:
					if (!(type == typeof(byte)))
					{
						num = 2;
						num2 = num;
					}
					else
					{
						num = 9;
						num2 = num;
					}
					continue;
				case 12:
					if (!(type == typeof(ushort)))
					{
						num = 25;
						num2 = num;
					}
					else
					{
						num = 6;
						num2 = num;
					}
					continue;
				case 17:
					result = (int)(ulong)value;
					num = 10;
					num2 = num;
					continue;
				case 8:
				case 10:
				case 11:
				case 14:
				case 15:
				case 18:
				case 23:
				case 24:
					{
						return result;
					}
					IL_0467:
					num = 23;
					num2 = num;
					continue;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			type = sourceProperty.PropertyType;
			short num = 13;
			num2 = num;
			goto default;
		}
		}
	}

	public static void SetValueFromInt(this FieldInfo targetField, object targetObject, int newValue)
	{
		int a_ = 2;
		string a_2 = "RTSKWCPLU1JDG6KUCPNZT45P2ZRG7O0O9ONM1PEK8YM8W3LFVL6A49TW";
		switch (0)
		{
		case 0:
			goto IL_006e;
		}
		goto IL_001c;
		IL_001c:
		int num = default(int);
		Type type = default(Type);
		short num2;
		while (true)
		{
			switch (num)
			{
			case 7:
				if (type.IsEnum)
				{
					num2 = 18;
					num = num2;
					continue;
				}
				goto case 13;
			case 8:
				if (type == typeof(uint))
				{
					num2 = 2;
					num = num2;
				}
				else
				{
					num2 = 10;
					num = num2;
				}
				continue;
			case 0:
				if (!(type == typeof(ulong)))
				{
					num2 = 8;
					num = num2;
				}
				else
				{
					num2 = 17;
					num = num2;
				}
				continue;
			case 3:
				if (type == typeof(int))
				{
					num2 = 16;
					num = num2;
				}
				else
				{
					num2 = 1;
					num = num2;
				}
				continue;
			case 5:
				targetField.SetValue(targetObject, (ushort)newValue);
				return;
			case 13:
				CheckValidTypeAndValue(targetField, targetObject, newValue);
				num2 = 11;
				num = num2;
				continue;
			case 11:
				num2 = 1;
				if (num2 != 0)
				{
				}
				if (!(type == typeof(long)))
				{
					num2 = 3;
					num = num2;
				}
				else
				{
					num2 = 6;
					num = num2;
				}
				continue;
			case 2:
				targetField.SetValue(targetObject, (uint)newValue);
				return;
			case 14:
				if (type == typeof(sbyte))
				{
					num2 = 9;
					num = num2;
					continue;
				}
				throw new InvalidCastException(string.Format("Field {0}.{1} is not an integer type", targetObject.GetType().Name, targetField.Name));
			case 4:
				if (!(type == typeof(byte)))
				{
					num2 = 0;
					num = num2;
					continue;
				}
				goto IL_0319;
			case 16:
				targetField.SetValue(targetObject, newValue);
				return;
			case 10:
				if (!(type == typeof(ushort)))
				{
					num2 = 14;
					num = num2;
				}
				else
				{
					num2 = 5;
					num = num2;
				}
				continue;
			case 9:
				targetField.SetValue(targetObject, (sbyte)newValue);
				return;
			case 1:
			{
				num2 = -29907;
				short num3 = num2;
				num2 = -29907;
				switch (num3 == num2)
				{
				case false:
				case true:
					break;
				default:
					num2 = 0;
					if (num2 != 0)
					{
					}
					if (!(type == typeof(short)))
					{
						num2 = 4;
						num = num2;
					}
					else
					{
						num2 = 12;
						num = num2;
					}
					continue;
				}
				goto IL_0319;
			}
			case 18:
				type = type.GetEnumUnderlyingType();
				num2 = 13;
				num = num2;
				continue;
			case 6:
				num2 = 0;
				_ = num2;
				targetField.SetValue(targetObject, (long)newValue);
				return;
			case 12:
				targetField.SetValue(targetObject, (short)newValue);
				return;
			case 15:
				targetField.SetValue(targetObject, (byte)newValue);
				return;
			case 17:
				{
					targetField.SetValue(targetObject, (ulong)newValue);
					return;
				}
				IL_0319:
				num2 = 15;
				num = num2;
				continue;
			}
			break;
		}
		goto IL_006e;
		IL_006e:
		type = targetField.FieldType;
		num2 = 7;
		num = num2;
		goto IL_001c;
	}

	public static int GetValueAsInt(this FieldInfo sourceField, object sourceObject)
	{
		int a_ = 14;
		string a_2 = "06FK462UZ5EW4INE1YZWBTRO6RW12ABPVY1RMJ0NAQRYOFNAN";
		int num = default(int);
		Type type = default(Type);
		switch (0)
		{
		default:
		{
			int result = default(int);
			object value = default(object);
			while (true)
			{
				switch (num)
				{
				case 24:
					if (type.IsEnum)
					{
						short num2 = 5;
						num = num2;
						continue;
					}
					goto case 26;
				case 20:
				{
					result = (int)(uint)value;
					short num2 = 7;
					num = num2;
					continue;
				}
				case 12:
					if (type == typeof(ulong))
					{
						short num2 = 16;
						num = num2;
					}
					else
					{
						short num2 = 1;
						num = num2;
					}
					continue;
				case 13:
					if (type == typeof(int))
					{
						short num2 = 10;
						num = num2;
					}
					else
					{
						short num2 = 2;
						num = num2;
					}
					continue;
				case 0:
					if (type == typeof(sbyte))
					{
						short num2 = 21;
						num = num2;
						continue;
					}
					throw new InvalidCastException(string.Format("Member {0}.{1} is not an integer type", sourceObject.GetType().Name, sourceField.Name));
				case 26:
				{
					short num2 = 1;
					if (num2 != 0)
					{
					}
					value = sourceField.GetValue(sourceObject);
					num2 = 14;
					num = num2;
					continue;
				}
				case 14:
					if (!(type == typeof(long)))
					{
						short num2 = 13;
						num = num2;
					}
					else
					{
						short num2 = 3;
						num = num2;
					}
					continue;
				case 3:
				{
					short num2 = 0;
					_ = num2;
					result = (int)(long)value;
					num2 = 19;
					num = num2;
					continue;
				}
				case 1:
					if (type == typeof(uint))
					{
						short num2 = 20;
						num = num2;
					}
					else
					{
						short num2 = 25;
						num = num2;
					}
					continue;
				case 21:
				{
					result = (sbyte)value;
					short num2 = 17;
					num = num2;
					continue;
				}
				case 6:
				{
					result = (byte)value;
					short num2 = 15;
					num = num2;
					continue;
				}
				case 10:
				{
					result = (int)value;
					short num2 = 18;
					num = num2;
					continue;
				}
				case 11:
				{
					result = (ushort)value;
					short num2 = 22;
					num = num2;
					continue;
				}
				case 4:
				{
					result = (short)value;
					short num2 = 23;
					num = num2;
					continue;
				}
				case 5:
				{
					type = type.GetEnumUnderlyingType();
					short num2 = -2538;
					short num3 = num2;
					num2 = -2538;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 26;
						num = num2;
						continue;
					}
					goto case 13;
				}
				case 2:
					if (type == typeof(short))
					{
						short num2 = 4;
						num = num2;
					}
					else
					{
						short num2 = 9;
						num = num2;
					}
					continue;
				case 9:
					if (!(type == typeof(byte)))
					{
						short num2 = 12;
						num = num2;
					}
					else
					{
						short num2 = 6;
						num = num2;
					}
					continue;
				case 25:
					if (!(type == typeof(ushort)))
					{
						short num2 = 0;
						num = num2;
					}
					else
					{
						short num2 = 11;
						num = num2;
					}
					continue;
				case 16:
				{
					result = (int)(ulong)value;
					short num2 = 8;
					num = num2;
					continue;
				}
				case 7:
				case 8:
				case 15:
				case 17:
				case 18:
				case 19:
				case 22:
				case 23:
					return result;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			type = sourceField.FieldType;
			short num2 = 24;
			num = num2;
			goto default;
		}
		}
	}

	public static void CheckValidTypeAndValue(MemberInfo targetMember, object targetObject, int newValue)
	{
		int a_ = 1;
		string a_2 = "5PUUQ2FEL1VXAILKYJXK4C45ADZVSOYB2V48V6B4RYMXYQO1QXQFXJGDXZ7TF6I";
		int num = default(int);
		Type type2 = default(Type);
		Type type = default(Type);
		switch (0)
		{
		default:
			while (true)
			{
				switch (num)
				{
				case 40:
					if (targetMember == null)
					{
						short num2 = 0;
						num = num2;
					}
					else
					{
						short num2 = 50;
						num = num2;
					}
					continue;
				case 19:
					if (type2 == typeof(uint))
					{
						short num2 = 54;
						num = num2;
					}
					else
					{
						short num2 = 52;
						num = num2;
					}
					continue;
				case 41:
				{
					short num2 = 35;
					num = num2;
					continue;
				}
				case 35:
					if (!Enum.IsDefined(type, (byte)newValue))
					{
						short num2 = 56;
						num = num2;
						continue;
					}
					return;
				case 18:
				{
					short num2 = 34;
					num = num2;
					continue;
				}
				case 34:
					if (type.IsEnum)
					{
						short num2 = 25;
						num = num2;
						continue;
					}
					return;
				case 43:
				{
					InvalidEnumArgumentException ex5 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex5.Data["AttemptedNewValue"] = (ulong)newValue;
					ex5.Data["AttemptedNewValueInt"] = newValue;
					throw ex5;
				}
				case 47:
					if (type.IsEnum)
					{
						short num2 = 6;
						num = num2;
						continue;
					}
					return;
				case 39:
				{
					short num2 = 22;
					num = num2;
					continue;
				}
				case 22:
					if (type.IsEnum)
					{
						short num2 = 41;
						num = num2;
						continue;
					}
					return;
				case 21:
				{
					short num2 = 3;
					num = num2;
					continue;
				}
				case 3:
					if (type.IsEnum)
					{
						short num2 = 1;
						num = num2;
						continue;
					}
					return;
				case 12:
				{
					InvalidEnumArgumentException ex8 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex8.Data["AttemptedNewValue"] = (ushort)newValue;
					ex8.Data["AttemptedNewValueInt"] = newValue;
					throw ex8;
				}
				case 20:
					if (type2 == typeof(short))
					{
						short num2 = 11;
						num = num2;
					}
					else
					{
						short num2 = 48;
						num = num2;
					}
					continue;
				case 0:
					throw new ArgumentNullException("sourceMember");
				case 23:
				{
					type2 = type;
					short num2 = 44;
					num = num2;
					continue;
				}
				case 15:
				case 29:
				{
					short num2 = 7;
					num = num2;
					continue;
				}
				case 7:
					if (type.IsEnum)
					{
						type2 = type.GetEnumUnderlyingType();
						short num2 = 45;
						num = num2;
					}
					else
					{
						short num2 = 23;
						num = num2;
					}
					continue;
				case 1:
				{
					short num2 = 31;
					num = num2;
					continue;
				}
				case 31:
					if (!Enum.IsDefined(type, (long)newValue))
					{
						short num2 = 58;
						num = num2;
						continue;
					}
					return;
				case 33:
					if (type2 == typeof(sbyte))
					{
						short num2 = 17;
						num = num2;
						continue;
					}
					goto IL_048a;
				case 17:
				{
					short num2 = 0;
					_ = num2;
					num2 = 47;
					num = num2;
					continue;
				}
				case 36:
				{
					InvalidEnumArgumentException ex2 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex2.Data["AttemptedNewValue"] = (uint)newValue;
					ex2.Data["AttemptedNewValueInt"] = newValue;
					throw ex2;
				}
				case 37:
				{
					InvalidEnumArgumentException ex = new InvalidEnumArgumentException("newValue", newValue, type);
					ex.Data["AttemptedNewValue"] = newValue;
					ex.Data["AttemptedNewValueInt"] = newValue;
					throw ex;
				}
				case 11:
				{
					short num2 = 13;
					num = num2;
					continue;
				}
				case 13:
					if (type.IsEnum)
					{
						short num2 = 4;
						num = num2;
						continue;
					}
					return;
				case 10:
				{
					short num2 = 24;
					num = num2;
					continue;
				}
				case 24:
					if (type.IsEnum)
					{
						short num2 = 51;
						num = num2;
						continue;
					}
					return;
				case 51:
				{
					short num2 = 4083;
					short num3 = num2;
					num2 = 4083;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						num2 = 0;
						if (num2 != 0)
						{
						}
						num2 = 57;
						num = num2;
						continue;
					}
					goto IL_048a;
				}
				case 27:
					if (!(type2 == typeof(int)))
					{
						short num2 = 20;
						num = num2;
					}
					else
					{
						short num2 = 32;
						num = num2;
					}
					continue;
				case 44:
				case 45:
				{
					short num2 = 16;
					num = num2;
					continue;
				}
				case 16:
					if (!(type2 == typeof(long)))
					{
						short num2 = 27;
						num = num2;
					}
					else
					{
						short num2 = 21;
						num = num2;
					}
					continue;
				case 52:
					if (!(type2 == typeof(ushort)))
					{
						short num2 = 33;
						num = num2;
					}
					else
					{
						short num2 = 18;
						num = num2;
					}
					continue;
				case 32:
				{
					short num2 = 49;
					num = num2;
					continue;
				}
				case 49:
					if (type.IsEnum)
					{
						short num2 = 28;
						num = num2;
						continue;
					}
					return;
				case 58:
				{
					InvalidEnumArgumentException ex7 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex7.Data["AttemptedNewValue"] = (long)newValue;
					ex7.Data["AttemptedNewValueInt"] = newValue;
					throw ex7;
				}
				case 56:
				{
					InvalidEnumArgumentException ex6 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex6.Data["AttemptedNewValue"] = (byte)newValue;
					ex6.Data["AttemptedNewValueInt"] = newValue;
					throw ex6;
				}
				case 50:
					if (targetObject == null)
					{
						short num2 = 5;
						num = num2;
					}
					else
					{
						short num2 = 38;
						num = num2;
					}
					continue;
				case 48:
					if (type2 == typeof(byte))
					{
						short num2 = 39;
						num = num2;
					}
					else
					{
						short num2 = 9;
						num = num2;
					}
					continue;
				case 4:
				{
					short num2 = 46;
					num = num2;
					continue;
				}
				case 46:
					if (!Enum.IsDefined(type, (short)newValue))
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					return;
				case 25:
				{
					short num2 = 55;
					num = num2;
					continue;
				}
				case 55:
					if (!Enum.IsDefined(type, (ushort)newValue))
					{
						short num2 = 12;
						num = num2;
						continue;
					}
					return;
				case 2:
				{
					InvalidEnumArgumentException ex4 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex4.Data["AttemptedNewValue"] = (short)newValue;
					ex4.Data["AttemptedNewValueInt"] = newValue;
					throw ex4;
				}
				case 6:
				{
					short num2 = 14;
					num = num2;
					continue;
				}
				case 14:
					if (!Enum.IsDefined(type, (sbyte)newValue))
					{
						short num2 = 53;
						num = num2;
						continue;
					}
					return;
				case 38:
					if (!(targetMember is PropertyInfo))
					{
						type = ((FieldInfo)targetMember).FieldType;
						short num2 = 15;
						num = num2;
					}
					else
					{
						short num2 = 42;
						num = num2;
					}
					continue;
				case 42:
				{
					short num2 = 1;
					if (num2 != 0)
					{
					}
					type = ((PropertyInfo)targetMember).PropertyType;
					num2 = 29;
					num = num2;
					continue;
				}
				case 5:
					throw new ArgumentNullException("targetObject");
				case 53:
				{
					InvalidEnumArgumentException ex3 = new InvalidEnumArgumentException("newValue", newValue, type);
					ex3.Data["AttemptedNewValue"] = (sbyte)newValue;
					ex3.Data["AttemptedNewValueInt"] = newValue;
					throw ex3;
				}
				case 9:
					if (!(type2 == typeof(ulong)))
					{
						short num2 = 19;
						num = num2;
					}
					else
					{
						short num2 = 10;
						num = num2;
					}
					continue;
				case 28:
				{
					short num2 = 59;
					num = num2;
					continue;
				}
				case 59:
					if (!Enum.IsDefined(type, newValue))
					{
						short num2 = 37;
						num = num2;
						continue;
					}
					return;
				case 26:
				{
					short num2 = 8;
					num = num2;
					continue;
				}
				case 8:
					if (!Enum.IsDefined(type, (uint)newValue))
					{
						short num2 = 36;
						num = num2;
						continue;
					}
					return;
				case 57:
					if (!Enum.IsDefined(type, (ulong)newValue))
					{
						short num2 = 43;
						num = num2;
						continue;
					}
					return;
				case 54:
				{
					short num2 = 30;
					num = num2;
					continue;
				}
				case 30:
					{
						if (type.IsEnum)
						{
							short num2 = 26;
							num = num2;
							continue;
						}
						return;
					}
					IL_048a:
					throw new InvalidCastException(string.Format("Property {0}.{1} is not an integer type", targetObject.GetType().Name, targetMember.Name));
				}
				break;
			}
			goto case 0;
		case 0:
		{
			type = null;
			type2 = null;
			short num2 = 40;
			num = num2;
			goto default;
		}
		}
	}
}
public static class ByteArrayExtensions
{
	public static bool EqualsContents(this byte[] sourceArray, byte[] checkArray)
	{
		int num = default(int);
		bool flag = default(bool);
		switch (0)
		{
		default:
		{
			int num2 = default(int);
			while (true)
			{
				switch (num)
				{
				case 5:
					if (sourceArray != null)
					{
						short num3 = 8;
						num = num3;
						continue;
					}
					goto IL_02ee;
				case 0:
					if (sourceArray.Length == checkArray.Length)
					{
						short num3 = 4;
						num = num3;
						continue;
					}
					goto IL_02ee;
				case 8:
				{
					short num3 = 10;
					num = num3;
					continue;
				}
				case 10:
					if (checkArray != null)
					{
						short num3 = 2;
						num = num3;
						continue;
					}
					goto IL_02ee;
				case 6:
				case 7:
				{
					short num3 = 1;
					num = num3;
					continue;
				}
				case 1:
					if (num2 < sourceArray.Length && flag)
					{
						flag = sourceArray[num2] == checkArray[num2];
						num2++;
						short num3 = 1;
						if (num3 != 0)
						{
						}
						num3 = 6;
						num = num3;
					}
					else
					{
						short num3 = 3;
						num = num3;
					}
					continue;
				case 3:
				{
					short num3 = 0;
					_ = num3;
					goto IL_02ee;
				}
				case 2:
				{
					short num3 = 11;
					num = num3;
					continue;
				}
				case 11:
					if (sourceArray != checkArray)
					{
						short num3 = 9;
						num = num3;
						continue;
					}
					goto IL_02ee;
				case 9:
				{
					short num3 = -30315;
					short num4 = num3;
					num3 = -30315;
					switch (num4 == num3)
					{
					case false:
					case true:
						continue;
					}
					num3 = 0;
					if (num3 != 0)
					{
					}
					num3 = 0;
					num = num3;
					continue;
				}
				case 4:
					{
						flag = true;
						num2 = 0;
						short num3 = 7;
						num = num3;
						continue;
					}
					IL_02ee:
					return flag;
				}
				break;
			}
			goto case 0;
		}
		case 0:
		{
			flag = false;
			short num3 = 5;
			num = num3;
			goto default;
		}
		}
	}
}
public static class HttpWebResponseExtensions
{
	public static SecurityProtocolType GetConnectionProtocol(this HttpWebResponse response)
	{
		int a_ = 18;
		string a_2 = "OBSG7GY2K6K4B63VFA0L4QN4TSZH3B9FRGBFBLAOT7K7R676S39C7";
		switch (0)
		{
		case 0:
			goto IL_0052;
		}
		goto IL_001c;
		IL_001c:
		int num = default(int);
		int num3 = default(int);
		short num2;
		while (true)
		{
			switch (num)
			{
			case 3:
				if (((uint)num3 & 0x30u) != 0)
				{
					num2 = 10;
					num = num2;
				}
				else
				{
					num2 = 7;
					num = num2;
				}
				continue;
			case 5:
			{
				num2 = 0;
				_ = num2;
				num2 = -11674;
				short num4 = num2;
				num2 = -11674;
				switch (num4 == num2)
				{
				case false:
				case true:
					goto IL_032a;
				}
				num2 = 0;
				if (num2 != 0)
				{
				}
				if (DuoUtil.Net.supportsTls12)
				{
					num2 = 2;
					num = num2;
					continue;
				}
				goto IL_0395;
			}
			case 1:
				num2 = 0;
				num = num2;
				continue;
			case 0:
				num2 = 1;
				if (num2 != 0)
				{
				}
				if (((uint)num3 & (uint)DuoUtil.Net.tls11Flag) != 0)
				{
					num2 = 8;
					num = num2;
					continue;
				}
				goto IL_0136;
			case 7:
				if (((uint)num3 & 0xC0u) != 0)
				{
					num2 = 9;
					num = num2;
				}
				else
				{
					num2 = 4;
					num = num2;
				}
				continue;
			case 2:
				num2 = 6;
				num = num2;
				continue;
			case 6:
				if (((uint)num3 & (uint)DuoUtil.Net.tls12Flag) != 0)
				{
					num2 = 11;
					num = num2;
					continue;
				}
				goto IL_0395;
			case 10:
				goto IL_032a;
			case 8:
				return DuoUtil.Net.tls11Flag;
			case 11:
				return DuoUtil.Net.tls12Flag;
			case 4:
				if (DuoUtil.Net.supportsTls11)
				{
					num2 = 1;
					num = num2;
					continue;
				}
				goto IL_0136;
			case 9:
				{
					return SecurityProtocolType.Tls;
				}
				IL_0136:
				num2 = 5;
				num = num2;
				continue;
				IL_032a:
				return SecurityProtocolType.Ssl3;
				IL_0395:
				return (SecurityProtocolType)num3;
			}
			break;
		}
		goto IL_0052;
		IL_0052:
		num3 = (int)response.GetResponseStream().GetPropertyValue("Connection").GetPropertyValue("NetworkStream")
			.GetFieldValue("m_Worker")
			.GetPropertyValue("Context")
			.GetPropertyValue("ConnectionInfo")
			.GetFieldValue("Protocol");
		num2 = 3;
		num = num2;
		goto IL_001c;
	}

	public static IPAddress GetLocalIpAddress(this HttpWebResponse response)
	{
		int a_ = 15;
		string a_2 = "Y9KKXKCK9FVO4JGQZ6GEWG8FN3FZPNAW88V1SM02C2D2D218E2JYO";
		short num = 1;
		if (num != 0)
		{
		}
		num = 6429;
		short num2 = num;
		num = 6429;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			return ((response.GetResponseStream().GetPropertyValue("Connection").GetPropertyValue("Socket") as Socket).LocalEndPoint as IPEndPoint).Address;
		}
	}
}
public static class AssemblyExtensions
{
	public static string GetCodebasePathname(this Assembly sourceAssembly)
	{
		short num = -4123;
		short num2 = num;
		num = -4123;
		switch (num2 == num)
		{
		default:
			num = 0;
			if (num != 0)
			{
			}
			num = 0;
			_ = num;
			num = 1;
			if (num != 0)
			{
			}
			return new Uri(sourceAssembly.CodeBase).LocalPath;
		}
	}

	internal static string b(string A_0, int A_1, string A_2)
	{
		char[] array = A_0.ToCharArray();
		int num = (int)((nint)(156344638 + A_1) + (nint)45 + 77 + 32 + 14 + 84);
		int num2 = 0;
		if (num2 >= 1)
		{
			goto IL_0037;
		}
		goto IL_006a;
		IL_0037:
		int num3 = num2;
		char num4 = array[num3];
		byte b = (byte)((num4 & 0xFFu) ^ (uint)num++);
		byte b2 = (byte)(((int)num4 >> 8) ^ num++);
		byte num5 = b2;
		b2 = b;
		b = num5;
		array[num3] = (char)((b2 << 8) | b);
		num2++;
		goto IL_006a;
		IL_006a:
		if (num2 >= array.Length)
		{
			return string.Intern(new string(array));
		}
		goto IL_0037;
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '8.2.0.7535-95108c96')
