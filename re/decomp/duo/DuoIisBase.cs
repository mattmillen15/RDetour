using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[assembly: AssemblyProduct("DuoIisBase")]
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: AssemblyTitle("DuoIisBase")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: Guid("af93af82-16fc-4714-83cb-2a838b97bdf2")]
[assembly: Dotfuscator("464981:1:0:7.5.0+b3e290d373", 0)]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: AssemblyCopyright("Copyright ©  2025")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: TargetFramework(".NETFramework,Version=v4.7.1", FrameworkDisplayName = ".NET Framework 4.7.1")]
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
namespace Duo;

public class DuoSecretProtection : IDuoSecretProtection
{
	private string a;

	public DuoSecretProtection()
	{
		int a_ = 16;
		string a_2 = "TYJHB9ER8CMS0QPIBYJY3SGBCTXJXK37YE4TXJBXCZOXD9LN8MIU4XF539VM";
		base..ctor();
		a = Assembly.GetCallingAssembly()?.Location;
		if (string.IsNullOrEmpty(a))
		{
			throw new Exception(b("簾\u2040⽂⥄⹆❈ⱊ浌⹎≐⁒ご㩖㭘㝚\u245c罞ᅠɢᅤས䥨ࡪɬ\u1a6eᵰ\u1772啴\u1976ᙸ\u0f7a嵼ᵾ\ue480ꎂ\ue184\ue286ﶈ\uee8aﾌ\ue28e\uf890ﶒ\uf094\uf396래", a_, a_2));
		}
	}

	public void SaveSecret(string secret)
	{
		int a_ = 8;
		string a_2 = "LRKZ71KGAKMHR0HZ7ET5ASN42IGYJAKWOKAU20QUP6TKKT647";
		string value = DuoUtil.ProtectRegistryValue(secret, this);
		short num;
		if (!string.IsNullOrEmpty(value))
		{
			num = -21896;
			short num2 = num;
			num = -21896;
			switch (num2 == num)
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
				DuoUtil.WriteToRegistry(b("搶瘸紺椼栾@ᅂDᭆ\u0d48㹊≌潎ɐ㙒㙔≖⭘㉚⥜♞㵠❢\u1064ࡦ㭨ཪ\u1a6c੮፰", a_, a_2), b("琶唸刺堼儾㕀᱂ᙄ≆⩈㥊⡌㭎", a_, a_2), value);
				return;
			}
		}
		num = 0;
		_ = num;
		throw new Exception(b("然堸刺儼娾╀捂ㅄ⡆楈⹊⍌ⱎ⍐⩒╔⍖祘⽚㕜㩞䅠\u1062dѦ᭨\u0e6aᥬ䅮兰ᙲ᭴ᑶ\u0b78ɺർ\u0b7e\ue480\ue782횄\ue286\uea88力\ue88cﮎ놐\ue492\uf494\ue496릘\uf59a\ue89c\uf39e춠", a_, a_2));
	}

	public virtual string GetEntropyString()
	{
		int a_ = 8;
		string a_2 = "MC3TKKUZG25T2DKNAWTD4OVOE79WRU0AOV0CYJBE2FIGPQB078EMOEBOD1H3B7";
		short num = -20394;
		short num2 = num;
		num = -20394;
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
			if (DuoUtil.CompareSignatures(a))
			{
				goto case false;
			}
			throw new Exception(b("搶倸尺匼帾㕀㙂㝄≆㩈歊⥌⁎煐㵒㩔⍖祘㙚㱜⭞ɠ\u0b62䕤Ŧ٨ᥪ䵬", a_, a_2) + a + b("᜶堸唺夼ἾՀ㙂⩄ቆ㵈≊⅌慎", a_, a_2));
		case false:
		case true:
			Console.WriteLine(b("搶倸尺匼帾㕀㙂㝄≆㩈歊⁌⹎═げ㵔祖祘ᱚ㡜ㅞѠᅢѤ፦hժ੬佮ᑰᵲŴնᙸ\u0b7aѼ彾\uf280\uf782\uf784\uee86\ue788\uec8aꎌ", a_, a_2));
			return DuoUtil.ObfuscateEntropy(b("戶縸挺焼唾ㅀ㩂Dፆ㡈Պ界ݎ\u2450ɒ㭔ݖ⡘\u175a㕜᭞ᅠ╢⍤⑦౨ɪᡬѮ\u1a70㉲ⱴṶ㵸ぺ᭼㕾뚀캂쮄", a_, a_2));
		}
	}

	public bool ValidateSecret(string sKey)
	{
		int a_ = 4;
		string a_2 = "HC5EJW5QKOIXVAZHZPYGY753EI5RFRW6BK4NY6MNBXR0RG4E2WDFI8EOF5T1W";
		bool result;
		short num;
		try
		{
			num = 17515;
			short num2 = num;
			num = 17515;
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
				string registryValue = DuoUtil.GetRegistryValue(b("怲稴然洸氺簼派рὂń㉆♈歊Ṍ⩎㉐♒❔㹖ⵘ≚Ŝ᭞ᑠ\u0c62㝤\u0366Ṩ\u0e6aཬ", a_, a_2), b("笲娴䐶䴸", a_, a_2));
				string registryValue2 = DuoUtil.GetRegistryValue(b("怲稴然洸氺簼派рὂń㉆♈歊Ṍ⩎㉐♒❔㹖ⵘ≚Ŝ᭞ᑠ\u0c62㝤\u0366Ṩ\u0e6aཬ", a_, a_2), b("瀲头帶尸唺䤼怾ࡀ❂", a_, a_2));
				result = DuoUtil.ValidateKeys(sKey, registryValue, registryValue2);
				break;
			}
			}
		}
		catch (Exception)
		{
			result = false;
		}
		num = 0;
		_ = num;
		return result;
	}

	internal static string b(string A_0, int A_1, string A_2)
	{
		char[] array = A_0.ToCharArray();
		int num = (int)((nint)(1346828101 + A_1) + (nint)50 + 89 + 94);
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
public static class DuoConstants
{
	public const string REGISTRY_PATH = "SOFTWARE\\Duo Security\\DuoRdweb";

	public const string FAIL_OPEN = "failOpen";

	public const string REGISTRY_SECRET_NAME = "Client_Secret";
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '8.2.0.7535-95108c96')
