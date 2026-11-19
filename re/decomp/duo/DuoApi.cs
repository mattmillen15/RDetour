using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

[assembly: TargetFramework(".NETFramework,Version=v4.7.1", FrameworkDisplayName = ".NET Framework 4.7.1")]
[assembly: Dotfuscator("464981:1:0:7.5.0+b3e290d373", 0)]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyDescription("")]
[assembly: ComVisible(false)]
[assembly: Guid("0d68d6dc-08d4-4c7e-9ace-f23acbf541f4")]
[assembly: AssemblyTrademark("")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyTitle("DuoApi")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Duo Security")]
[assembly: AssemblyProduct("DuoApi.Properties")]
[assembly: AssemblyCopyright("2018")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyVersion("1.0.0.0")]
[AttributeUsage(AttributeTargets.Assembly)]
[ComVisible(false)]
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
namespace Duo
{
	public class DuoApi
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

		public string DEFAULT_AGENT = CertificatePinnerFactory.c("䬎搐簒吔䜖倘堚东眞䀠儢唤\u0826ᠨԪᴬ", 3, "YG0Z9IH08KU3A9KU4JTA9SMPE0EUUWKLVTHYM8DKXOBGLNWM8C950YFIQM");

		[CompilerGenerated]
		private bool m_a = true;

		private string m_b;

		private string c;

		private string d;

		private string e;

		private string f;

		private bool g;

		public bool AllowProxyDetection
		{
			[CompilerGenerated]
			get
			{
				short num = 18154;
				short num2 = num;
				num = 18154;
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
					return this.m_a;
				}
			}
			[CompilerGenerated]
			set
			{
				short num = 25868;
				short num2 = num;
				num = 25868;
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
				this.m_a = value;
			}
		}

		public DuoApi(string ikey, string skey, string host, bool enable_cert_pinning = false)
			: this(ikey, skey, host, null, enable_cert_pinning)
		{
		}

		public DuoApi(string ikey, string skey, string host, string user_agent, bool enable_cert_pinning = false)
			: this(ikey, skey, host, user_agent, CertificatePinnerFactory.c("眞唠圢唤否", 19, "PZT9NAZJG0C0B08EQD2L3UNVYSGG8Z9KRZXDET70M1S2MVB309NF712DR"), enable_cert_pinning)
		{
		}

		protected DuoApi(string ikey, string skey, string host, string user_agent, string url_scheme, bool enable_cert_pinning = false)
		{
			this.m_b = ikey;
			c = skey;
			d = host;
			e = url_scheme;
			g = enable_cert_pinning;
			if (string.IsNullOrEmpty(user_agent))
			{
				f = FormatUserAgent(DEFAULT_AGENT);
			}
			else
			{
				f = user_agent;
			}
		}

		public static string CanonicalizeParams(Dictionary<string, string> parameters)
		{
			int a_ = 15;
			string a_2 = "D0LKXWJK9ECUHPMMJAYHS19MAZPR7G67CJA7KMF1UOCL04B2QXNTXHNL20C0HO";
			short num = 0;
			_ = num;
			num = -14786;
			short num2 = num;
			num = -14786;
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
			List<string> list = new List<string>();
			using (Dictionary<string, string>.Enumerator enumerator = parameters.GetEnumerator())
			{
				num = 4;
				int num3 = num;
				string text = default(string);
				while (true)
				{
					switch (num3)
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
					case 5:
						text = Regex.Replace(text, CertificatePinnerFactory.c("㌚㠜䐞ᄠย\u1c24昦Ш洪䰬Ȯ地渲渴\u0736ᐸȺ簼ሾ\u0740≂桄ⅆᑈ扊", a_, a_2), delegate(Match A_0)
						{
							short num6 = 0;
							_ = num6;
							num6 = 13595;
							short num7 = num6;
							num6 = 13595;
							switch (num7 == num6)
							{
							default:
								num6 = 0;
								if (num6 != 0)
								{
								}
								num6 = 1;
								if (num6 != 0)
								{
								}
								return A_0.Value.ToUpperInvariant();
							}
						});
						num = 6;
						num3 = num;
						continue;
					case 6:
						text = Regex.Replace(text, CertificatePinnerFactory.c("㌚䘜㸞ؠଢతദ琨Ȫ", a_, a_2), delegate(Match A_0)
						{
							int a_3 = 9;
							string a_4 = "1O709SNX0DT3OBRB203F3PWIZ23LQM82P1WEYZXFBIJYPXCH";
							short num4 = 1;
							if (num4 != 0)
							{
							}
							num4 = -25359;
							short num5 = num4;
							num4 = -25359;
							switch (num5 == num4)
							{
							default:
								num4 = 0;
								_ = num4;
								break;
							case true:
								break;
							}
							num4 = 0;
							if (num4 != 0)
							{
							}
							return CertificatePinnerFactory.c("〔", a_3, a_4) + Convert.ToByte(A_0.Value[0]).ToString(CertificatePinnerFactory.c("䴔", a_3, a_4));
						});
						text = text.Replace(CertificatePinnerFactory.c("㸚⨜娞", a_, a_2), CertificatePinnerFactory.c("攚", a_, a_2));
						text = text.Replace(CertificatePinnerFactory.c("〚", a_, a_2), CertificatePinnerFactory.c("㸚⼜⼞", a_, a_2));
						list.Add(text);
						num = 1;
						num3 = num;
						continue;
					default:
						num = 0;
						num3 = num;
						continue;
					case 0:
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, string> current = enumerator.Current;
							text = string.Format(CertificatePinnerFactory.c("怚ⴜ戞ᰠ堢ᐤ娦", a_, a_2), HttpUtility.UrlEncode(current.Key), HttpUtility.UrlEncode(current.Value));
							num = 5;
							num3 = num;
						}
						else
						{
							num = 2;
							num3 = num;
						}
						continue;
					case 2:
						num = 1;
						if (num != 0)
						{
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
			list.Sort(StringComparer.Ordinal);
			return string.Join(CertificatePinnerFactory.c("㴚", a_, a_2), list.ToArray());
		}

		protected string CanonicalizeRequest(string method, string path, string canon_params, string date)
		{
			int a_ = 8;
			string a_2 = "FVH426SE6OICS3D0TF9B1BSCCAX63GKYKYM2QHTRBBH7JXAKEROEZHL3DQAMBEI";
			short num = 0;
			_ = num;
			num = 5149;
			short num2 = num;
			num = 5149;
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
				string[] value = new string[5]
				{
					date,
					method.ToUpperInvariant(),
					d.ToLower(),
					path,
					canon_params
				};
				return string.Join(CertificatePinnerFactory.c("ḓ", a_, a_2), value);
			}
			}
		}

		public string Sign(string method, string path, string canon_params, string date)
		{
			int a_ = 19;
			string a_2 = "P2Q305493YKLRUL5G3YD3P6N6TQG9ZU4BT6G6WC4KNSBHOI149IGOUO";
			short num = 4135;
			short num2 = num;
			num = 4135;
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
				string a_3 = CanonicalizeRequest(method, path, canon_params, date);
				string text = b(a_3);
				string a_4 = this.m_b + CertificatePinnerFactory.c("┞", a_, a_2) + text;
				return CertificatePinnerFactory.c("崞䀠倢䰤䐦न", a_, a_2) + a(a_4);
			}
			}
		}

		public string ApiCall(string method, string path, Dictionary<string, string> parameters)
		{
			short num = 27692;
			short num2 = num;
			num = 27692;
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
				HttpStatusCode statusCode;
				return ApiCall(method, path, parameters, 0, DateTime.UtcNow, out statusCode);
			}
			}
		}

		public string ApiCall(string method, string path, Dictionary<string, string> parameters, int timeout, out HttpStatusCode statusCode)
		{
			short num = -26326;
			short num2 = num;
			num = -26326;
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
				return ApiCall(method, path, parameters, 0, DateTime.UtcNow, out statusCode);
			}
		}

		public string ApiCall(string method, string path, Dictionary<string, string> parameters, int timeout, DateTime date, out HttpStatusCode statusCode)
		{
			int a_ = 11;
			string a_2 = "K68UB8M4LCBG7BZQREH2EIIDN4UESEX4VMMIZWXDPN1RYWKD5RISJUEF";
			short num = 0;
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
				{
					HttpWebRequest httpWebRequest = default(HttpWebRequest);
					byte[] bytes = default(byte[]);
					Stream requestStream = default(Stream);
					while (true)
					{
						short num3;
						switch (num2)
						{
						case 19:
							if (!method.Equals(CertificatePinnerFactory.c("䜖嘘䠚䤜", a_, a_2)))
							{
								num = 23;
								num2 = num;
								continue;
							}
							goto IL_0240;
						case 23:
							num = 8;
							num2 = num;
							continue;
						case 8:
							if (!method.Equals(CertificatePinnerFactory.c("䜖䰘会", a_, a_2)))
							{
								num = 17;
								num2 = num;
								continue;
							}
							goto IL_0240;
						case 17:
							num = 6;
							num2 = num;
							continue;
						case 6:
							if (parameters.Count > 0)
							{
								num = 14;
								num2 = num;
								continue;
							}
							goto IL_0240;
						case 14:
							text2 = CertificatePinnerFactory.c("⠖", a_, a_2) + text;
							num = 10;
							num2 = num;
							continue;
						case 10:
							num = 1;
							if (num == 0)
							{
							}
							goto IL_0240;
						case 13:
						{
							string requestUriString = string.Format(CertificatePinnerFactory.c("氖⤘昚✜〞ภ堢ᐤ娦刨\u192a倬吮Ȱ串", a_, a_2), e, d, path, text2);
							string text3 = a(date);
							string value = Sign(method, path, text, text3);
							if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Ssl3))
							{
								ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
							}
							if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls))
							{
								ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls;
							}
							if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls11))
							{
								ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls11;
							}
							ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
							httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
							httpWebRequest.Method = method;
							httpWebRequest.Accept = CertificatePinnerFactory.c("瘖椘欚焜瘞䈠䈢儤並䘨䔪Ȭ䔮䈰尲嬴", a_, a_2);
							httpWebRequest.Headers.Add(CertificatePinnerFactory.c("嘖氘漚甜瀞匠䨢弤䘦崨䈪䈬䄮", a_, a_2), value);
							httpWebRequest.Headers.Add(CertificatePinnerFactory.c("伖㐘弚栜瀞ఠ朢䐤匦䰨", a_, a_2), text3);
							httpWebRequest.UserAgent = f;
							num = 15;
							num2 = num;
							continue;
						}
						case 15:
							if (g)
							{
								num = 20;
								num2 = num;
								continue;
							}
							goto IL_0430;
						case 20:
							try
							{
								httpWebRequest.ServerCertificateValidationCallback = CertificatePinnerFactory.GetDuoCertificatePinner();
							}
							catch (Exception inner)
							{
								throw new DuoException(0, CertificatePinnerFactory.c("儖砘爚焜稞䔠\u0322儤䠦न个䌬丮匰弲倴᜶娸帺似䬾⡀╂ⱄ⑆⡈㽊⡌潎⅐㩒㭔㥖じ㕚㩜", a_, a_2), inner);
							}
							goto IL_0430;
						case 5:
							if (AllowProxyDetection)
							{
								num = 9;
								num2 = num;
								continue;
							}
							goto case 0;
						case 9:
							num = 22;
							num2 = num;
							continue;
						case 22:
							if (!HasProxyServer(httpWebRequest))
							{
								num = 3;
								num2 = num;
								continue;
							}
							goto case 0;
						case 3:
							httpWebRequest.Proxy = GetWinhttpProxy();
							num = 0;
							num2 = num;
							continue;
						case 0:
							num = 11;
							num2 = num;
							continue;
						case 11:
							if (!method.Equals(CertificatePinnerFactory.c("䜖嘘䠚䤜", a_, a_2)))
							{
								num = 12;
								num2 = num;
								continue;
							}
							goto case 21;
						case 12:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (method.Equals(CertificatePinnerFactory.c("䜖䰘会", a_, a_2)))
							{
								num = 21;
								num2 = num;
								continue;
							}
							goto IL_071b;
						case 21:
							bytes = Encoding.UTF8.GetBytes(text);
							httpWebRequest.ContentType = CertificatePinnerFactory.c("瘖椘欚焜瘞䈠䈢儤並䘨䔪Ȭ圮\u1c30䐲䈴䀶ᐸ崺刼䴾ⱀ湂い㕆╈⹊⍌ⱎ㹐㝒ご㍖", a_, a_2);
							httpWebRequest.ContentLength = bytes.Length;
							requestStream = httpWebRequest.GetRequestStream();
							num = 16;
							num2 = num;
							continue;
						case 16:
							try
							{
								requestStream.Write(bytes, 0, bytes.Length);
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
										if (requestStream != null)
										{
											num = 0;
											num2 = num;
											continue;
										}
										break;
									case 0:
										((IDisposable)requestStream).Dispose();
										num = 1;
										num2 = num;
										continue;
									case 1:
										break;
									}
									break;
								}
							}
							goto IL_071b;
						case 7:
							if (timeout > 0)
							{
								num = 2;
								num2 = num;
								continue;
							}
							goto case 4;
						case 2:
							httpWebRequest.Timeout = timeout;
							num = 4;
							num2 = num;
							continue;
						case 4:
							num = 0;
							_ = num;
							num = 18;
							num2 = num;
							continue;
						case 18:
							{
								try
								{
									HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
									try
									{
										StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
										try
										{
											statusCode = httpWebResponse.StatusCode;
											return streamReader.ReadToEnd();
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
													if (streamReader != null)
													{
														num = 2;
														num2 = num;
														continue;
													}
													break;
												case 2:
													((IDisposable)streamReader).Dispose();
													num = 1;
													num2 = num;
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
												if (httpWebResponse != null)
												{
													num = 1;
													num2 = num;
													continue;
												}
												break;
											case 1:
												((IDisposable)httpWebResponse).Dispose();
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
								catch (WebException ex)
								{
									if (ex.Response != null)
									{
										HttpWebResponse httpWebResponse2 = (HttpWebResponse)ex.Response;
										try
										{
											StreamReader streamReader2 = new StreamReader(httpWebResponse2.GetResponseStream());
											try
											{
												statusCode = httpWebResponse2.StatusCode;
												return streamReader2.ReadToEnd();
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
														if (streamReader2 != null)
														{
															num = 1;
															num2 = num;
															continue;
														}
														break;
													case 1:
														((IDisposable)streamReader2).Dispose();
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
													if (httpWebResponse2 != null)
													{
														num = 0;
														num2 = num;
														continue;
													}
													break;
												case 0:
													((IDisposable)httpWebResponse2).Dispose();
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
									throw;
								}
							}
							IL_071b:
							num = 7;
							num2 = num;
							continue;
							IL_0240:
							num = 13;
							num2 = num;
							continue;
							IL_0430:
							num = 14595;
							num3 = num;
							num = 14595;
							switch (num3 == num)
							{
							default:
								num = 0;
								if (num != 0)
								{
								}
								num = 5;
								num2 = num;
								continue;
							case false:
							case true:
								break;
							}
							goto case 5;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					text = CanonicalizeParams(parameters);
					text2 = "";
					num = 19;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public T JSONApiCall<T>(string method, string path, Dictionary<string, string> parameters) where T : class
		{
			short num = 0;
			_ = num;
			num = -29728;
			short num2 = num;
			num = -29728;
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
				return JSONApiCall<T>(method, path, parameters, 0, DateTime.UtcNow);
			}
		}

		public T JSONApiCall<T>(string method, string path, Dictionary<string, string> parameters, int timeout) where T : class
		{
			short num = 1;
			if (num != 0)
			{
			}
			num = -32043;
			short num2 = num;
			num = -32043;
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
			return JSONApiCall<T>(method, path, parameters, timeout, DateTime.UtcNow);
		}

		public T JSONApiCall<T>(string method, string path, Dictionary<string, string> parameters, int timeout, DateTime date) where T : class
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Expected O, but got Unknown
			int a_ = 9;
			string a_2 = "MQ4Y5UPIO2Y38NS01PGCRT9ZFJ8SF8XQU5AO7DJSXZAXKJRMEZU86MCJ9FS";
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
				HttpStatusCode statusCode;
				string text = ApiCall(method, path, parameters, timeout, date, out statusCode);
				JavaScriptSerializer val = new JavaScriptSerializer();
				try
				{
					Dictionary<string, object> dictionary = default(Dictionary<string, object>);
					switch (0)
					{
					default:
					{
						string api_message_detail = default(string);
						T result = default(T);
						int code = default(int);
						int? num4 = default(int?);
						while (true)
						{
							switch (num2)
							{
							case 5:
								if (dictionary[CertificatePinnerFactory.c("昔挖砘漚", a_, a_2)] as string == CertificatePinnerFactory.c("娔尖", a_, a_2))
								{
									num = 8;
									num2 = num;
									continue;
								}
								goto IL_02cd;
							case 3:
							case 4:
								api_message_detail = "";
								num = 2;
								num2 = num;
								continue;
							case 2:
								if (dictionary.ContainsKey(CertificatePinnerFactory.c("研爖樘栚簜砞䐠簢䄤䈦崨䨪䐬䌮", a_, a_2)))
								{
									num = 7;
									num2 = num;
									continue;
								}
								goto case 6;
							case 8:
								result = dictionary[CertificatePinnerFactory.c("朔爖樘欚爜焞删䘢", a_, a_2)] as T;
								num = 9;
								num2 = num;
								continue;
							case 7:
								api_message_detail = dictionary[CertificatePinnerFactory.c("研爖樘栚簜砞䐠簢䄤䈦崨䨪䐬䌮", a_, a_2)] as string;
								num = 6;
								num2 = num;
								continue;
							case 1:
								code = num4.Value;
								num = 4;
								num2 = num;
								continue;
							case 0:
							{
								num = 10779;
								short num3 = num;
								num = 10779;
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
									if (!num4.HasValue)
									{
										code = 0;
										num = 3;
										num2 = num;
									}
									else
									{
										num = 1;
										num2 = num;
									}
									continue;
								}
								goto IL_02cd;
							}
							case 6:
								throw new ApiException(code, (int)statusCode, dictionary[CertificatePinnerFactory.c("研爖樘栚簜砞䐠", a_, a_2)] as string, api_message_detail);
							case 9:
								{
									return result;
								}
								IL_02cd:
								num4 = dictionary[CertificatePinnerFactory.c("瘔砖紘縚", a_, a_2)] as int?;
								num = 0;
								num2 = num;
								continue;
							}
							break;
						}
						goto case 0;
					}
					case 0:
						dictionary = val.Deserialize<Dictionary<string, object>>(text);
						num = 5;
						num2 = num;
						goto default;
					}
				}
				catch (ApiException)
				{
					throw;
				}
				catch (Exception inner)
				{
					throw new BadResponseException((int)statusCode, inner);
				}
			}
			}
		}

		public static string FormatUserAgent(string product_name, Dictionary<string, string> uaAdditions = null)
		{
			int a_ = 17;
			string a_2 = "JQT4KQAWH25P236O12D5QP2UG0HHCQBQEXZ5CZBN7BMWQQQEAA";
			int num = default(int);
			string text = default(string);
			switch (0)
			{
			default:
			{
				Dictionary<string, string>.Enumerator enumerator = default(Dictionary<string, string>.Enumerator);
				while (true)
				{
					switch (num)
					{
					case 3:
					{
						short num2 = 1;
						if (num2 != 0)
						{
						}
						if (uaAdditions != null)
						{
							num2 = 0;
							num = num2;
							continue;
						}
						goto IL_0326;
					}
					case 4:
						try
						{
							short num2 = 3;
							num = num2;
							while (true)
							{
								switch (num)
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
									num2 = 1;
									num = num2;
									continue;
								case 1:
								{
									if (enumerator.MoveNext())
									{
										KeyValuePair<string, string> current = enumerator.Current;
										text = text + CertificatePinnerFactory.c("☜㼞", a_, a_2) + current.Key + CertificatePinnerFactory.c("✜㼞", a_, a_2) + current.Value;
										num2 = 2;
										num = num2;
										continue;
									}
									num2 = 20338;
									short num3 = num2;
									num2 = 20338;
									switch (num3 == num2)
									{
									default:
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
									break;
								}
								case 4:
									break;
								case 0:
									goto end_IL_0103;
								}
								num2 = 0;
								num = num2;
								continue;
								end_IL_0103:
								break;
							}
						}
						finally
						{
							((IDisposable)enumerator).Dispose();
						}
						goto IL_0326;
					case 1:
					{
						enumerator = uaAdditions.GetEnumerator();
						short num2 = 4;
						num = num2;
						continue;
					}
					case 0:
					{
						short num2 = 2;
						num = num2;
						continue;
					}
					case 2:
						{
							if (uaAdditions.Count > 0)
							{
								short num2 = 0;
								_ = num2;
								num2 = 1;
								num = num2;
								continue;
							}
							goto IL_0326;
						}
						IL_0326:
						return text + CertificatePinnerFactory.c("㐜", a_, a_2);
					}
					break;
				}
				goto case 0;
			}
			case 0:
			{
				text = string.Format(CertificatePinnerFactory.c("昜⼞尠\u0322ത尦ᠨ嘪ᘬ༮ἰ紲瀴挶\u1938䀺༼䈾", a_, a_2), product_name, Environment.OSVersion, Environment.Version);
				short num2 = 3;
				num = num2;
				goto default;
			}
			}
		}

		private string b(string A_0)
		{
			int a_ = 5;
			string a_2 = "JZPGCHDHFKUL88LL27CBTXFRGTEONBDAYGF1OIR51EE6VGAI9X0U9EUG";
			short num = -11380;
			short num2 = num;
			num = -11380;
			string result = default(string);
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
				HMACSHA1 hMACSHA = new HMACSHA1(Encoding.ASCII.GetBytes(c));
				try
				{
					byte[] bytes = Encoding.ASCII.GetBytes(A_0);
					hMACSHA.ComputeHash(bytes);
					result = BitConverter.ToString(hMACSHA.Hash).Replace(CertificatePinnerFactory.c("㰐", a_, a_2), "").ToLower();
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
							if (hMACSHA != null)
							{
								num = 1;
								num3 = num;
								continue;
							}
							break;
						case 1:
							((IDisposable)hMACSHA).Dispose();
							num = 0;
							num3 = num;
							continue;
						case 0:
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
			num = 0;
			_ = num;
			return result;
		}

		private static string a(string A_0)
		{
			short num = -29521;
			short num2 = num;
			num = -29521;
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
				return Convert.ToBase64String(Encoding.ASCII.GetBytes(A_0));
			}
		}

		private static string a(DateTime A_0)
		{
			int a_ = 6;
			string a_2 = "Z9L9PLWG2NP11LHXE1BAV73GW5DH8E50094VDG79BUORX3FLVFS357YM";
			short num = 1;
			if (num != 0)
			{
			}
			num = -23451;
			short num2 = num;
			num = -23451;
			switch (num2 == num)
			{
			default:
			{
				num = 0;
				if (num != 0)
				{
				}
				num = 0;
				_ = num;
				string text = A_0.ToString(CertificatePinnerFactory.c("瘑瀓爕㐗㨙砛稝\u001f漡椣欥\u0827匩唫圭䤯ሱ簳縵ȷ圹儻н㌿ㅁ", a_, a_2), CultureInfo.InvariantCulture);
				int num3 = TimeZone.CurrentTimeZone.GetUtcOffset(A_0).Hours;
				string text2;
				if (num3 >= 0)
				{
					text2 = CertificatePinnerFactory.c("㤑", a_, a_2);
				}
				else
				{
					num3 *= -1;
					text2 = CertificatePinnerFactory.c("㼑", a_, a_2);
				}
				return string.Concat(str2: (text2 + num3.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')).PadRight(5, '0'), str0: text, str1: CertificatePinnerFactory.c("㈑", a_, a_2));
			}
			}
		}

		public static WebProxy GetWinhttpProxy()
		{
			int a_ = 5;
			string a_2 = "QMLRBA6C4CLBDTMPMSUK424WLBZD976OBGX1MPOGRXYOEPH22F936FX82B2GQC";
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				int num3 = default(int);
				string text = default(string);
				string[] A_ = default(string[]);
				string[] array = default(string[]);
				WebProxy result = default(WebProxy);
				bool bypassOnLocal = default(bool);
				switch (0)
				{
				default:
				{
					b b = default(b);
					while (true)
					{
						switch (num2)
						{
						case 6:
							if (num3 != 0)
							{
								num = 10;
								num2 = num;
								continue;
							}
							throw new Exception(string.Format(CertificatePinnerFactory.c("䘐稒笔弖洘漚洜㼞䠠䴢䰤匦न䴪䰬䘮崰嘲儴᜶䈸\u0b3a䀼", a_, a_2), Marshal.GetLastWin32Error()));
						case 1:
							num = 8;
							num2 = num;
							continue;
						case 8:
							num = 0;
							_ = num;
							if (text != null)
							{
								num = 0;
								num2 = num;
								continue;
							}
							goto case 18;
						case 22:
							A_ = new string[1] { b.c };
							num = 15;
							num2 = num;
							continue;
						case 11:
							num = 14;
							num2 = num;
							continue;
						case 14:
							if (array.Length == 0)
							{
								num = 4;
								num2 = num;
							}
							else
							{
								text = array[0];
								num = 3;
								num2 = num;
							}
							continue;
						case 20:
							result = new WebProxy(text, bypassOnLocal, A_);
							num = 18;
							num2 = num;
							continue;
						case 16:
							num = 9;
							num2 = num;
							continue;
						case 9:
							if (b.b != null)
							{
								num = 24;
								num2 = num;
								continue;
							}
							goto case 13;
						case 2:
							bypassOnLocal = A_.Contains<string>(CertificatePinnerFactory.c("紐簒瘔瘖甘", a_, a_2), StringComparer.InvariantCultureIgnoreCase);
							a(ref A_);
							num = 1;
							num2 = num;
							continue;
						case 18:
							WinHttpCloseHandle(num3);
							num3 = 0;
							num = 23;
							num2 = num;
							continue;
						case 19:
							num = 12;
							num2 = num;
							continue;
						case 12:
							if (A_.Length == 0)
							{
								num = 22;
								num2 = num;
								continue;
							}
							goto case 15;
						case 4:
							text = b.b;
							num = 13;
							num2 = num;
							continue;
						case 17:
							text = text.Split(new char[1] { '=' }).Last();
							num = 20;
							num2 = num;
							continue;
						case 13:
						{
							num = 4104;
							short num4 = num;
							num = 4104;
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
							goto case 15;
						}
						case 7:
							if (b.c != null)
							{
								num = 5;
								num2 = num;
								continue;
							}
							goto case 1;
						case 10:
							b = new b();
							WinHttpGetDefaultProxyConfiguration(b);
							num = 27;
							num2 = num;
							continue;
						case 27:
							if (b.b != null)
							{
								num = 16;
								num2 = num;
								continue;
							}
							goto case 18;
						case 24:
							array = b.b.Split(' ', '\t', ';');
							num = 25;
							num2 = num;
							continue;
						case 25:
							if (array != null)
							{
								num = 11;
								num2 = num;
								continue;
							}
							goto case 4;
						case 3:
							num = 1;
							if (num == 0)
							{
							}
							goto case 13;
						case 5:
							A_ = b.c.Split(' ', '\t', ';');
							num = 28;
							num2 = num;
							continue;
						case 28:
							if (A_ != null)
							{
								num = 19;
								num2 = num;
								continue;
							}
							goto case 22;
						case 0:
							num = 21;
							num2 = num;
							continue;
						case 21:
							if (text.Contains(CertificatePinnerFactory.c("Ⱀ", a_, a_2)))
							{
								num = 17;
								num2 = num;
								continue;
							}
							goto case 20;
						case 15:
							num = 26;
							num2 = num;
							continue;
						case 26:
							if (A_ != null)
							{
								num = 2;
								num2 = num;
								continue;
							}
							goto case 1;
						case 23:
							return result;
						}
						break;
					}
					goto case 0;
				}
				case 0:
					array = null;
					text = null;
					A_ = null;
					bypassOnLocal = false;
					result = null;
					num3 = WinHttpOpen(CertificatePinnerFactory.c("唐昒稔䌖簘栚検", a_, a_2), DuoApi.a.a, null, null, 0);
					num = 6;
					num2 = num;
					goto default;
				}
			}
			}
		}

		public static bool HasProxyServer(HttpWebRequest requestObject)
		{
			int a_ = 5;
			string a_2 = "F36LATB1ZKVAGW7JNCEC7GV254J9S6BPA0ZRYJPT3MHK84GVE7NRFDD";
			int num = default(int);
			bool result = default(bool);
			WebProxy webProxy = default(WebProxy);
			FieldInfo field = default(FieldInfo);
			while (true)
			{
				short num2;
				switch (0)
				{
				default:
					while (true)
					{
						int num3;
						switch (num)
						{
						case 9:
							if (requestObject.Proxy != null)
							{
								num2 = 12;
								num = num2;
							}
							else
							{
								result = false;
								num2 = 13;
								num = num2;
							}
							continue;
						case 7:
							num3 = 0;
							goto IL_0272;
						case 1:
						case 10:
							num2 = 11;
							num = num2;
							continue;
						case 11:
							num2 = 1;
							if (num2 != 0)
							{
							}
							if (webProxy == null)
							{
								num2 = 7;
								num = num2;
							}
							else
							{
								num2 = 8;
								num = num2;
							}
							continue;
						case 2:
							field = requestObject.Proxy.GetType().GetField(CertificatePinnerFactory.c("昐瘒眔䜖欘琚攜昞", a_, a_2), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							num2 = 6;
							num = num2;
							continue;
						case 6:
							if (field != null)
							{
								num2 = 5;
								num = num2;
								continue;
							}
							goto case 1;
						case 12:
							num2 = 4;
							num = num2;
							continue;
						case 4:
							if (!(requestObject.Proxy is WebProxy))
							{
								num2 = 2;
								num = num2;
							}
							else
							{
								webProxy = requestObject.Proxy as WebProxy;
								num2 = 10;
								num = num2;
							}
							continue;
						case 8:
							num2 = 0;
							num = num2;
							continue;
						case 0:
							num3 = ((webProxy.Address != null) ? 1 : 0);
							goto IL_0272;
						case 5:
							webProxy = field.GetValue(requestObject.Proxy) as WebProxy;
							num2 = 1;
							num = num2;
							continue;
						case 3:
						case 13:
							{
								return result;
							}
							IL_0272:
							result = (byte)num3 != 0;
							num2 = 3;
							num = num2;
							continue;
						}
						break;
					}
					goto case 0;
				case 0:
					{
						num2 = 4308;
						short num4 = num2;
						num2 = 4308;
						switch (num4 == num2)
						{
						case false:
						case true:
							break;
						default:
							goto IL_009c;
						}
						break;
					}
					IL_009c:
					num2 = 0;
					if (num2 != 0)
					{
					}
					webProxy = null;
					result = false;
					num2 = 0;
					_ = num2;
					num2 = 9;
					num = num2;
					goto default;
				}
			}
		}

		public static string WildcardToRegex(string pattern)
		{
			int a_ = 5;
			string a_2 = "YYVICQ63DIZMWNOC857CB32L3JG3PSXTLSUCKRMDSEPYFIHKWCEWSBYUQIQ";
			short num = 12611;
			short num2 = num;
			num = 12611;
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
			return Regex.Escape(pattern).Replace(CertificatePinnerFactory.c("䴐㤒", a_, a_2), CertificatePinnerFactory.c("㼐㤒", a_, a_2));
		}

		private static void a(ref string[] A_0)
		{
			int num2 = default(int);
			int num4 = default(int);
			switch (0)
			{
			default:
				while (true)
				{
					short num = 0;
					_ = num;
					switch (num2)
					{
					case 0:
					case 2:
						num = 1;
						num2 = num;
						continue;
					case 1:
					{
						num = -18098;
						short num3 = num;
						num = -18098;
						switch (num3 == num)
						{
						case false:
						case true:
							return;
						}
						num = 0;
						if (num != 0)
						{
						}
						if (num4 >= A_0.Length)
						{
							num = 3;
							num2 = num;
							continue;
						}
						A_0[num4] = WildcardToRegex(A_0[num4]);
						num4++;
						num = 0;
						num2 = num;
						continue;
					}
					case 3:
						return;
					}
					break;
				}
				goto case 0;
			case 0:
			{
				short num = 1;
				if (num != 0)
				{
				}
				num4 = 0;
				num = 2;
				num2 = num;
				goto default;
			}
			}
		}

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern bool WinHttpGetDefaultProxyConfiguration([In][Out] b A_0);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern int WinHttpOpen([MarshalAs(UnmanagedType.LPWStr)] string A_0, a A_1, [MarshalAs(UnmanagedType.LPWStr)] string A_2, [MarshalAs(UnmanagedType.LPWStr)] string A_3, int A_4);

		[DllImport("winhttp.dll", CharSet = CharSet.Unicode)]
		private static extern bool WinHttpCloseHandle(int A_0);
	}
}
[CompilerGenerated]
internal sealed class a
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 6)]
	internal struct a
	{
	}

	internal static readonly a a/* Not supported: data(20 00 09 00 3B 00) */;
}
namespace Duo
{
	public interface IDuoLogger
	{
		void WriteLine(string logLineText, LogLevel logLevel = LogLevel.Information);

		void WriteLine(string formatString, params object[] arguments);

		void AppendDebugLine(string debugLine);
	}
	public enum LogLevel
	{
		Trace,
		Debug,
		Information,
		Warning,
		Error,
		Critical,
		None
	}
	public interface IDuoSecretProtection
	{
		void SaveSecret(string secret);

		string GetEntropyString();

		bool ValidateSecret(string secret);
	}
	[Serializable]
	public class DuoException : Exception
	{
		[CompilerGenerated]
		private int <HttpStatus>k__BackingField;

		public int HttpStatus
		{
			[CompilerGenerated]
			get
			{
				short num = 12625;
				short num2 = num;
				num = 12625;
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
					return <HttpStatus>k__BackingField;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = -8369;
				short num2 = num;
				num = -8369;
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
				<HttpStatus>k__BackingField = value;
			}
		}

		public DuoException(int http_status, string message, Exception inner)
			: base(message, inner)
		{
			HttpStatus = http_status;
		}

		protected DuoException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{
		}
	}
	[Serializable]
	public class ApiException : DuoException
	{
		[CompilerGenerated]
		private int <Code>k__BackingField;

		[CompilerGenerated]
		private string <ApiMessage>k__BackingField;

		[CompilerGenerated]
		private string <ApiMessageDetail>k__BackingField;

		public int Code
		{
			[CompilerGenerated]
			get
			{
				short num = 18914;
				short num2 = num;
				num = 18914;
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
					return <Code>k__BackingField;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = 27160;
				short num2 = num;
				num = 27160;
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
				<Code>k__BackingField = value;
			}
		}

		public string ApiMessage
		{
			[CompilerGenerated]
			get
			{
				short num = -31676;
				short num2 = num;
				num = -31676;
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
					return <ApiMessage>k__BackingField;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = 18292;
				short num2 = num;
				num = 18292;
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
				<ApiMessage>k__BackingField = value;
			}
		}

		public string ApiMessageDetail
		{
			[CompilerGenerated]
			get
			{
				short num = 21527;
				short num2 = num;
				num = 21527;
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
					return <ApiMessageDetail>k__BackingField;
				}
			}
			[CompilerGenerated]
			private set
			{
				short num = 1;
				if (num != 0)
				{
				}
				num = 27068;
				short num2 = num;
				num = 27068;
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
				<ApiMessageDetail>k__BackingField = value;
			}
		}

		public ApiException(int code, int http_status, string api_message, string api_message_detail)
			: base(http_status, a(code, api_message, api_message_detail), null)
		{
			Code = code;
			ApiMessage = api_message;
			ApiMessageDetail = api_message_detail;
		}

		protected ApiException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{
		}

		private static string a(int A_0, string A_1, string A_2)
		{
			int a_ = 12;
			string a_2 = "DTDWY8UH62S3AIE91CRGALWT5XQT5GZS9GE1GOH80DIW214XB";
			short num = 15306;
			short num2 = num;
			num = 15306;
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
			return string.Format(CertificatePinnerFactory.c("尗漙猛㸝感爡洣إ洧堩師䄭䈯ሱ伳ص䔷9᰻\u193d㬿獁㥃慅桇扉歋㕍扏⽑獓罕", a_, a_2), A_0, A_1, A_2);
		}
	}
	[Serializable]
	public class BadResponseException : DuoException
	{
		public BadResponseException(int http_status, Exception inner)
			: base(http_status, a(http_status, inner), inner)
		{
		}

		protected BadResponseException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{
		}

		private static string a(int A_0, Exception A_1)
		{
			int a_ = 15;
			string a_2 = "PDMFNSUMT0RL1W55KWUE5SVBF7GE5DPO0SIGZS81YGTB4O9RRW3Y8OVY6T9";
			int num = default(int);
			string arg = default(string);
			short num2;
			switch (0)
			{
			default:
				while (true)
				{
					switch (num)
					{
					case 2:
						goto IL_0097;
					case 1:
						arg = string.Format(CertificatePinnerFactory.c("㰚昜⼞尠Т", a_, a_2), A_1.Message);
						num2 = 0;
						num = num2;
						continue;
					case 0:
						goto IL_01a0;
					}
					break;
					IL_0097:
					num2 = 1;
					if (num2 != 0)
					{
					}
					num2 = -7749;
					short num3 = num2;
					num2 = -7749;
					switch (num3 == num2)
					{
					case false:
					case true:
						break;
					default:
						goto IL_0111;
					}
					goto IL_005d;
					IL_01a0:
					return string.Format(CertificatePinnerFactory.c("尚爜欞Ġ䘢圤唦䘨太ബ吮İ串ᔴ䀶倸伺唼Ἶ\u0940ᝂᅄᝆ楈ᡊ㥌⹎═♒♔睖≘橚⁜", a_, a_2), arg, A_0);
					IL_0111:
					num2 = 0;
					if (num2 != 0)
					{
					}
					if (A_1 != null)
					{
						num2 = 1;
						num = num2;
						continue;
					}
					goto IL_01a0;
				}
				goto case 0;
			case 0:
				{
					arg = CertificatePinnerFactory.c("㌚猜樞䴠伢త", a_, a_2);
					goto IL_005d;
				}
				IL_005d:
				num2 = 0;
				_ = num2;
				num2 = 2;
				num = num2;
				goto default;
			}
		}
	}
	public class CertificatePinnerFactory
	{
		private readonly HashSet<string> m_a;

		public CertificatePinnerFactory(HashSet<string> pinnedSpkiHashes)
		{
			this.m_a = pinnedSpkiHashes;
		}

		public static RemoteCertificateValidationCallback GetDuoCertificatePinner()
		{
			short num = -10360;
			short num2 = num;
			num = -10360;
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
				return new CertificatePinnerFactory(a()).a;
			}
		}

		public static RemoteCertificateValidationCallback GetCertificateDisabler()
		{
			short num = 23869;
			short num2 = num;
			num = 23869;
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
				return delegate
				{
					short num3 = -32600;
					short num4 = num3;
					num3 = -32600;
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
					return true;
				};
			}
		}

		internal bool a(object A_0, X509Certificate A_1, X509Chain A_2, SslPolicyErrors A_3)
		{
			short num = 7;
			int num2 = num;
			X509ChainElementEnumerator enumerator = default(X509ChainElementEnumerator);
			string item = default(string);
			while (true)
			{
				switch (num2)
				{
				case 7:
					switch (0)
					{
					default:
						goto end_IL_0025;
					case 0:
						break;
					}
					goto default;
				default:
					if (A_1 != null)
					{
						num = 8;
						num2 = num;
						break;
					}
					goto case 6;
				case 8:
					num = 12;
					num2 = num;
					break;
				case 12:
					num = 1;
					if (num != 0)
					{
					}
					if (A_2 == null)
					{
						num = 0;
						_ = num;
						num = 6;
						num2 = num;
					}
					else
					{
						num = 11;
						num2 = num;
					}
					break;
				case 2:
					return false;
				case 11:
					if (A_3 != 0)
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
				case 4:
					if (A_2.ChainStatus.Any(delegate(X509ChainStatus A_0)
					{
						short num4 = 1211;
						short num5 = num4;
						num4 = 1211;
						switch (num5 == num4)
						{
						default:
							num4 = 1;
							if (num4 != 0)
							{
							}
							num4 = 0;
							_ = num4;
							num4 = 0;
							if (num4 != 0)
							{
							}
							return A_0.Status != X509ChainStatusFlags.NoError;
						}
					}))
					{
						num = 1;
						num2 = num;
					}
					else
					{
						enumerator = A_2.ChainElements.GetEnumerator();
						num = 9;
						num2 = num;
					}
					break;
				case 9:
					num = 5;
					num2 = num;
					break;
				case 5:
					if (!enumerator.MoveNext())
					{
						num = 3;
						num2 = num;
					}
					else
					{
						item = a(new X509Certificate2(enumerator.Current.Certificate));
						num = 10;
						num2 = num;
					}
					break;
				case 10:
				{
					if (!this.m_a.Contains(item))
					{
						goto case 9;
					}
					num = -1858;
					short num3 = num;
					num = -1858;
					switch (num3 == num)
					{
					default:
						num = 0;
						if (num != 0)
						{
						}
						num = 0;
						num2 = num;
						goto end_IL_0025;
					case false:
					case true:
						break;
					}
					goto case 6;
				}
				case 6:
					return false;
				case 1:
					return false;
				case 0:
					return true;
				case 3:
					{
						return false;
					}
					end_IL_0025:
					break;
				}
			}
		}

		internal static HashSet<string> a()
		{
			int a_ = 15;
			string a_2 = "M6G6JMP1F6BZ91WSRWEWY3OWSFC01P9FWKQBRN3NCM200SE3XOF";
			short num = -16717;
			short num2 = num;
			num = -16717;
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
				return new HashSet<string>
				{
					c("〚㘜刞挠䐢愤漦\u1c28簪樬央細ਲ眴吶圸\u0e3a缼娾牀獂♄ᕆ⩈\u074a経⥎摐᱒織ᥖ⁘㑚՜⩞㙠ᝢ㑤\u0366ㅨ婪౬♮䱰", a_, a_2),
					c("級ⴜ吞瘠ఢ挤匦堨缪䜬尮0\u0332ഴ礶䤸戺圼\u0b3e獀၂㝄F㽈ъཌ絎Ő⍒ⵔṖམᙚ敜ㅞ㙠᭢ཤ㝦ᡨⅪ⩬⩮䱰", a_, a_2),
					c("唚氜椞攠椢䤤䘦娨Ъ樬紮到樲圴吶游縺Լ氾湀\u0a42♄≆ň牊⹌㹎晐摒㹔ざ楘ㅚଡ଼㝞㭠٢⑤㝦ㅨ\u1a6a啬Ѯ䱰", a_, a_2),
					c("√㘜攞䐠ሢ䘤紦丨礪ᐬ搮縰Ȳ帴洶䬸洺礼䜾@睂\u0d44ᙆ罈㵊≌ݎ\u0350ၒ\u0654Ŗ\u1758⅚楜൞ՠ㝢♤ὦ嵨㹪啬㩮䱰", a_, a_2),
					c("倚樜簞䈠琢䐤搦丨太䌬丮䘰Բ䄴䐶䬸䤺渼瀾着牂\u0344⁆Ո⩊⹌Ŏ㙐ᑒ杔\u1a56ᑘ\u175aⱜ杞♠♢卤䱦٨㭪塬♮䱰", a_, a_2),
					c("䰚爜瘞瘠焢尤渦昨紪挬丮࠰娲崴嘶笸堺吼派ቀB牄\u1f46ň⅊⅌♎ࡐR汔Ŗ\u2e58๚\u1a5c\u105e⡠ᙢŤ卦㥨⥪屬坮䱰", a_, a_2),
					c("琚帜㐞圠䰢缤欦怨刪\u192c朮細瘲Դ然漸漺࠼䠾\u0740㝂㵄㵆ɈJ≌\u244eᵐ\u1752ݔ᱖X橚㉜ᅞ\u0a60բ⽤㹦౨䁪呬坮䱰", a_, a_2),
					c("稚洜稞ဠ欢氤渦猨ᴪ礬ᨮ唰в爴搶༸\u0a3a搼紾㉀灂㝄\u0346絈Պ\u1b4c㥎㩐㕒㭔Ŗ\u2e58Ṛᅜ㱞≠ㅢ㉤卦⭨\u1a6a\u1b6c彮䱰", a_, a_2),
					c("椚猜㐞瘠漢椤䤦䐨嬪ᐬ央Ȱ䘲焴朶\u0e38簺洼举⍀⁂⑄⹆ᭈ⽊⥌摎ѐ㵒ᙔ\u1a56⭘㩚ⵜ桞剠\u1a62ὤ呦\u1068Ṫ䉬ᡮ䱰", a_, a_2),
					c("⼚堜瀞戠漢樤樦弨缪怬ᜮ䈰唲\u0734甶縸瀺甼䨾ɀ⩂⽄\u0c46㥈ࡊ⭌ᝎ㽐\u0652TՖ癘㱚牜潞በb\u0364զ偨౪㕬≮䱰", a_, a_2),
					c("䴚笜笞ᠠᘢ朤倦洨个縬縮帰ᠲ笴戶怸䌺欼稾р\u0a42⥄ㅆ≈ъ⅌ᡎࡐ慒\u0654㙖㕘ၚᙜ湞ൠ㍢\u0d64ᵦ♨፪婬坮䱰", a_, a_2),
					c("瘚堜礞䴠礢焤ሦ䰨䔪䈬紮0甲䀴漶甸尺搼显ـ㉂⭄ᅆ\u0c48⑊ᝌ㥎㱐㕒汔㑖歘㥚ଡ଼ᵞᅠ\u0a62⩤൦と㩪嵬౮䱰", a_, a_2),
					c("多㘜眞琠ᬢ䠤圦ᄨ崪礬䘮欰礲\u0334堶簸簺ሼ࠾㥀㝂㙄睆ⅈ硊ὌṎ敐ᑒṔ敖ౘ㵚㹜՞㝠ቢdてⅨ䑪ɬ\u086e䱰", a_, a_2),
					c("渚栜⨞焠愢ฤ樦稨ሪ愬\u1c2eḰ唲匴甶ᘸ欺䠼款ـ畂ф汆Ṉ⅊㹌\u1b4e═ݒ㑔ᅖ汘楚ⱜ\u2e5eୠᅢ٤⽦⽨㍪㽬㩮䱰", a_, a_2),
					c("簚吜⸞丠倢ਤ嘦\u1928䈪栬弮地弲䴴䔶瘸崺漼紾ᝀ\u0742ᵄ㙆Ὀ⑊ᩌŎ扐ݒ⽔恖ᵘ㩚⭜灞噠⩢ㅤ䱦䉨㽪╬㹮䱰", a_, a_2)
				};
			}
		}

		internal static string a(X509Certificate2 A_0)
		{
			while (true)
			{
				short num = 1;
				if (num != 0)
				{
				}
				byte[] buffer = a(A_0.RawData);
				SHA256 sHA = SHA256.Create();
				string result;
				try
				{
					result = Convert.ToBase64String(sHA.ComputeHash(buffer));
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
							if (sHA != null)
							{
								num = 2;
								num2 = num;
								continue;
							}
							break;
						case 2:
							((IDisposable)sHA).Dispose();
							num = 1;
							num2 = num;
							continue;
						case 1:
							break;
						}
						break;
					}
				}
				num = 13191;
				short num3 = num;
				num = 13191;
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
		}

		internal static byte[] a(byte[] A_0)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				num = -4515;
				short num3 = num;
				num = -4515;
				int A_ = default(int);
				switch (num3 == num)
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
					switch (0)
					{
					case 0:
						goto IL_0119;
					}
					goto IL_0106;
				case false:
				case true:
					goto IL_0166;
					IL_0166:
					num = 1;
					num2 = num;
					goto IL_0106;
					IL_0119:
					A_ = 0;
					a(A_0, ref A_, 48);
					b(A_0, ref A_);
					a(A_0, ref A_, 48);
					b(A_0, ref A_);
					num = 0;
					num2 = num;
					goto IL_0106;
					IL_0106:
					while (true)
					{
						switch (num2)
						{
						case 0:
							if (A_0[A_] == 160)
							{
								goto IL_0166;
							}
							goto case 2;
						case 1:
							a(A_0, ref A_);
							num = 2;
							num2 = num;
							continue;
						case 2:
						{
							a(A_0, ref A_);
							a(A_0, ref A_);
							a(A_0, ref A_);
							a(A_0, ref A_);
							a(A_0, ref A_);
							int num4 = A_;
							a(A_0, ref A_, 48);
							int num5 = b(A_0, ref A_);
							int num6 = A_ - num4 + num5;
							byte[] array = new byte[num6];
							Array.Copy(A_0, num4, array, 0, num6);
							return array;
						}
						}
						break;
					}
					goto IL_0119;
				}
			}
			}
		}

		private static void a(byte[] A_0, ref int A_1, byte A_2)
		{
			int a_ = 12;
			string a_2 = "KXY9N1JEP6P1CZNO46CVPZM4NRB59Z9F2GJH3Z93MIVA0Y68EYU15V73OSDOKTS";
			short num = 0;
			_ = num;
			if (A_0[A_1] != A_2)
			{
				while (true)
				{
					num = 7738;
					short num2 = num;
					num = 7738;
					switch (num2 == num)
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
						if (num != 0)
						{
						}
						throw new FormatException(string.Format(c("尗弙丛㸝借䌡嘣唥䴧\u0a29䤫尭䈯崱䘳వᠷ弹䐻丽┿⅁ぃ⍅ⱇ橉㡋⽍㝏牑摓\u2e55⍗橙晛\u065d剟ὡ䑣ݥᱧ䩩\u036b\u086dᙯűᅳɵ塷Ź䵻ͽ걿ꊁ\ue383\ue985ﲇꪉ벋\uf68d\ueb8fꂑ꺓캕ꪗ\ue799", a_, a_2), A_2, A_1, A_0[A_1]));
					}
				}
			}
			A_1++;
		}

		private static int b(byte[] A_0, ref int A_1)
		{
			short num = 0;
			int num2 = num;
			switch (num2)
			{
			default:
			{
				int num6 = default(int);
				switch (0)
				{
				default:
				{
					int num5 = default(int);
					int num3 = default(int);
					int num4 = default(int);
					while (true)
					{
						switch (num2)
						{
						case 3:
							if (num6 < 128)
							{
								num = 2;
								num2 = num;
								continue;
							}
							num5 = num6 & 0x7F;
							num3 = 0;
							num4 = 0;
							num = 0;
							num2 = num;
							continue;
						case 2:
						{
							num = -30623;
							short num7 = num;
							num = -30623;
							switch (num7 == num)
							{
							default:
								num = 0;
								if (num != 0)
								{
								}
								return num6;
							case false:
							case true:
								break;
							}
							goto IL_0203;
						}
						case 0:
						case 5:
							num = 1;
							num2 = num;
							continue;
						case 1:
							if (num4 < num5)
							{
								num3 = (num3 << 8) | A_0[A_1++];
								num4++;
								num = 5;
								num2 = num;
								continue;
							}
							goto IL_0203;
						case 4:
							{
								return num3;
							}
							IL_0203:
							num = 0;
							_ = num;
							num = 4;
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
					num6 = A_0[A_1++];
					num = 3;
					num2 = num;
					goto default;
				}
			}
			}
		}

		private static void a(byte[] A_0, ref int A_1)
		{
			short num = 20488;
			short num2 = num;
			num = 20488;
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
			A_1++;
			int num3 = b(A_0, ref A_1);
			A_1 += num3;
		}

		internal static string c(string A_0, int A_1, string A_2)
		{
			char[] array = A_0.ToCharArray();
			int num = (int)((nint)(2032411492 + A_1) + (nint)95 + 21 + 51);
			int num2 = 0;
			if (num2 >= 1)
			{
				goto IL_0029;
			}
			goto IL_005c;
			IL_0029:
			int num3 = num2;
			char num4 = array[num3];
			byte b = (byte)((num4 & 0xFFu) ^ (uint)num++);
			byte b2 = (byte)(((int)num4 >> 8) ^ num++);
			byte num5 = b2;
			b2 = b;
			b = num5;
			array[num3] = (char)((b2 << 8) | b);
			num2++;
			goto IL_005c;
			IL_005c:
			if (num2 >= array.Length)
			{
				return string.Intern(new string(array));
			}
			goto IL_0029;
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '8.2.0.7535-95108c96')
