#define TRACE
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using Microsoft.JScript;
using Microsoft.TerminalServices.Publishing.Portal.FormAuthentication;
using Microsoft.TerminalServices.Publishing.Portal.Resources;
using Microsoft.TerminalServices.Publishing.Portal.WebFeedConfigHandler;
using Microsoft.TerminalServices.Publishing.Portal.neutralresources;
using Microsoft.Win32.SafeHandles;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.5")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: ComVisible(false)]
[assembly: AssemblyProduct("Microsoft (R) Windows (R) Operating System")]
[assembly: AssemblyCopyright("Copyright (c) Microsoft Corporation. All rights reserved.")]
[assembly: AssemblyCompany("Microsoft Corporation")]
[assembly: AssemblyFileVersion("10.0.10011.16384")]
[assembly: AssemblyKeyFile("d:\\os\\public\\amd64fre\\internal\\strongnamekeys\\fake\\windows.snk")]
[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyVersion("10.0.0.0")]
namespace Microsoft.TerminalServices.Publishing.Portal
{
	public class RapWebService
	{
		private static StoreAccessorFactory gm_StoreAccessorFactory = null;

		private static readonly object gm_StoreAccessorFactoryLock = new object();

		private bool m_PollAppFreshness = true;

		public RapWebService(bool PollAppFreshness)
		{
			m_PollAppFreshness = PollAppFreshness;
		}

		public static void StaticInitialize(StoreAccessorFactory storeAccessorFactory)
		{
			lock (gm_StoreAccessorFactoryLock)
			{
				gm_StoreAccessorFactory = storeAccessorFactory;
			}
		}

		internal async Task<IEnumerable<AppInfo>> GetRemoteAppsAsync(string strUserIdentity, bool onlyShowAvailableByDefaultResources, bool includeRDPFileContents)
		{
			try
			{
				IStoreAccessor storeAccessor = GetStoreAccessor();
				IEnumerable<AppInfo> apps;
				IEnumerable<AppInfo> desktops;
				try
				{
					Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>> tuple = await storeAccessor.GetApplicationsAsync(strUserIdentity, onlyShowAvailableByDefaultResources, includeRDPFileContents);
					apps = tuple.Item1;
					desktops = tuple.Item2;
				}
				catch (UnsupportedSourceServerException)
				{
					TraceWrite.TraceErrorNoContext("Store.GetApplications threw an UnsupportedSourceServerException, returning 0 resources");
					return new AppInfo[0];
				}
				List<AppInfo> list = new List<AppInfo>();
				List<AppInfo> list2 = new List<AppInfo>();
				if (apps != null)
				{
					list.AddRange(apps);
				}
				if (desktops != null)
				{
					list2.AddRange(desktops);
				}
				IComparer<IAppInfo> comparer = new AppInfoComparer();
				list.Sort(comparer);
				list2.Sort(comparer);
				list.AddRange(list2);
				return list;
			}
			finally
			{
				_ = 0;
			}
		}

		internal WorkspaceInfo GetWorkspaceInfo()
		{
			WorkspaceInfo workspaceInfo = null;
			try
			{
				IStoreAccessor storeAccessor = GetStoreAccessor();
				try
				{
					workspaceInfo = storeAccessor.GetWorkspaceInfo();
				}
				catch (UnsupportedSourceServerException)
				{
					TraceWrite.TraceErrorNoContext("Store.GetWorkspaceInfo threw an UnsupportedSourceServerException");
				}
				if (workspaceInfo == null)
				{
					TraceWrite.TraceWarningNoContext("Could not retrieve workspace info from the store, falling back to Web.config");
					workspaceInfo = new WorkspaceInfo();
					workspaceInfo.RedirectorName = string.Empty;
					workspaceInfo.WorkspaceId = RDWebAccessConfig.WorkspaceId;
					workspaceInfo.WorkspaceName = RDWebAccessConfig.WorkspaceName;
					workspaceInfo.WorkspaceDescription = RDWebAccessConfig.WorkspaceDescription;
					workspaceInfo.EventLogUploadAddress = string.Empty;
				}
				TraceWrite.TraceVerboseNoContext("Workspace name before removing invalid chars: [" + workspaceInfo.WorkspaceName + "]");
				workspaceInfo.WorkspaceName = MakeLegalPathName(workspaceInfo.WorkspaceName);
				TraceWrite.TraceVerboseNoContext("Workspace name after removing invalid chars: [" + workspaceInfo.WorkspaceName + "]");
				return workspaceInfo;
			}
			finally
			{
			}
		}

		private string MakeLegalPathName(string workspaceName)
		{
			if (string.IsNullOrEmpty(workspaceName))
			{
				return workspaceName;
			}
			string text = Regex.Replace(workspaceName, "[<>:\"/\\\\|?*\\x01-\\x1F]+", "", RegexOptions.CultureInvariant);
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int num = -1;
			for (int num2 = text.Length - 1; num2 >= 0; num2--)
			{
				if ('.' != text[num2] && !char.IsWhiteSpace(text[num2]))
				{
					num = num2;
					break;
				}
			}
			int num3 = num + 1;
			if (num3 >= text.Length)
			{
				return text;
			}
			return text.Remove(num3);
		}

		internal RdpSigningCertificate GetRdpSigningCertificate()
		{
			RdpSigningCertificate result = null;
			try
			{
				result = GetStoreAccessor().GetRdpSigningCertificate();
			}
			catch (UnsupportedSourceServerException)
			{
				TraceWrite.TraceErrorNoContext("Store.GetRdpSigningCertificate threw an UnsupportedSourceServerException, returning no certs");
			}
			finally
			{
			}
			return result;
		}

		internal string GetClaimsHint()
		{
			string result = "";
			try
			{
				result = GetStoreAccessor().GetClaimsHint();
			}
			catch (UnsupportedSourceServerException)
			{
				TraceWrite.TraceErrorNoContext("Store.GetClaimsHint threw an UnsupportedSourceServerException, returning an empty hint string");
			}
			finally
			{
			}
			return result;
		}

		internal async Task<string> GetPrivacyLinkAsync()
		{
			string privacyLink = string.Empty;
			try
			{
				privacyLink = await GetStoreAccessor().GetPrivacyLinkAsync();
			}
			catch (UnsupportedSourceServerException)
			{
				TraceWrite.TraceErrorNoContext("Store.GetPrivacyLink threw an UnsupportedSourceServerException, returning an empty privacy link");
			}
			finally
			{
				_ = 0;
			}
			return privacyLink;
		}

		public async Task<List<AccountDetailAndConsentStatus>> GetTenantAccountsAndUserConsentStatusAsync(object securityPrincipal)
		{
			return await GetStoreAccessor().GetTenantAccountsAndUserConsentStatusAsync(securityPrincipal);
		}

		internal bool SupportsWorkspaceReconnect()
		{
			bool flag = false;
			try
			{
				return GetStoreAccessor().SupportsWorkspaceReconnect;
			}
			finally
			{
			}
		}

		private IStoreAccessor GetStoreAccessor()
		{
			lock (gm_StoreAccessorFactoryLock)
			{
				if (gm_StoreAccessorFactory != null)
				{
					return gm_StoreAccessorFactory(m_PollAppFreshness);
				}
				if (RDWebAccessConfig.CloudMode)
				{
					TraceWrite.TraceErrorNoContext("GetStoreAccessor fell through to the on-prem accessor, but CloudMode is enabled in the Web.config. The cloud accessor should have been used instead.");
					throw new ApplicationException("Attempted to use the on-premise IStoreAccessor when in cloud mode");
				}
				return new RWSCPubAndTsAccessor(m_PollAppFreshness);
			}
		}
	}
	public delegate IStoreAccessor StoreAccessorFactory(bool pollAppFreshness);
	public class AccountDetailAndConsentStatus
	{
		public string ConsentStatus;

		public string TenantAccountId;

		public string TenantDisplayName;

		public string TenantDefaultDomain;

		public string WorkspaceName;

		public string ContactEmail;
	}
	public interface IStoreAccessor
	{
		bool SupportsWorkspaceReconnect { get; }

		Task<Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>> GetApplicationsAsync(string strUserIdentity, bool onlyShowAvailableByDefaultResources, bool includeRDPFileContents);

		WorkspaceInfo GetWorkspaceInfo();

		RdpSigningCertificate GetRdpSigningCertificate();

		string GetClaimsHint();

		Task<string> GetPrivacyLinkAsync();

		Task<List<AccountDetailAndConsentStatus>> GetTenantAccountsAndUserConsentStatusAsync(object securityPrincipal);
	}
	public class AppInfoComparer : IComparer<IAppInfo>
	{
		int IComparer<IAppInfo>.Compare(IAppInfo objAppInfo1, IAppInfo objAppInfo2)
		{
			if (objAppInfo1 != null && objAppInfo2 != null && ((objAppInfo1.Name != null) & (objAppInfo2.Name != null)))
			{
				return string.Compare(objAppInfo1.Name, objAppInfo2.Name, StringComparison.OrdinalIgnoreCase);
			}
			return 0;
		}
	}
	public class AppInfo : IAppInfo
	{
		private RemoteAppXml m_remoteAppXml;

		private string m_name;

		private FileExtensionInfo m_FileExtension;

		private DateTime m_timestamp;

		private bool m_isRemoteDesktop;

		private string m_tsName;

		public string TSName => m_tsName;

		public string Name => m_name;

		public DateTime Timestamp => m_timestamp;

		public bool IsRemoteDesktop => m_isRemoteDesktop;

		public RemoteAppXml RemoteAppXml => m_remoteAppXml;

		public bool AvailableToUsersByDefault { get; private set; }

		public string FileExtension => m_FileExtension.Name;

		public AppInfo(string name, string tsName, RemoteAppXml xml, bool isRemoteDesktop, DateTime timestamp, string fileExtension, bool availableToUsersByDefault)
		{
			m_name = name;
			m_remoteAppXml = xml;
			m_isRemoteDesktop = isRemoteDesktop;
			m_timestamp = timestamp;
			m_FileExtension.Name = fileExtension;
			m_tsName = tsName;
			AvailableToUsersByDefault = availableToUsersByDefault;
		}
	}
	public interface IAppInfo
	{
		string Name { get; }

		RemoteAppXml RemoteAppXml { get; }
	}
	public struct FileExtensionInfo
	{
		public string Name;

		public bool PrimaryHandler;

		public IconInfo AssocIcon;

		public FileExtensionInfo(string name, bool primaryHandler)
		{
			Name = name;
			PrimaryHandler = primaryHandler;
			AssocIcon = null;
		}
	}
	public struct RDPFileInfo
	{
		public string RdpFileName;

		public string RdpFileContents;

		public string RdpETag;

		public RDPFileInfo(string rdpFileName, string rdpFileContents, string rdpETag)
		{
			RdpFileName = rdpFileName;
			RdpFileContents = rdpFileContents;
			RdpETag = rdpETag;
		}
	}
	public enum IconInfoType
	{
		Contents,
		URI
	}
	public class IconInfo
	{
		public string name;

		public string type;

		private byte[] value;

		private List<ImageInfo> images;

		public IconInfoType IconInfoType { get; set; }

		public string IconUri { get; set; }

		public Dictionary<int, string> IconPngUris { get; }

		public string IconETag { get; set; }

		public Dictionary<int, string> IconPngETags { get; }

		public IconInfo(byte[] data, string iconName, int length, ImageFormat imageFormat, int[] imageDimensions)
		{
			IconInfoType = IconInfoType.Contents;
			name = iconName;
			type = "binary";
			if (length > 0 && data != null)
			{
				value = new byte[length];
				Array.Copy(data, value, length);
			}
			else
			{
				value = null;
			}
			images = new List<ImageInfo>();
			try
			{
				MakeImages(imageFormat, imageDimensions);
			}
			catch (InvalidAppIconException ex)
			{
				TraceWrite.TraceErrorNoContext("Failed to generate icon images for " + iconName + ", error: " + ex);
			}
		}

		public IconInfo(byte[] data, string iconName, int length)
		{
			IconInfoType = IconInfoType.Contents;
			name = iconName;
			type = "binary";
			if (length > 0 && data != null)
			{
				value = new byte[length];
				Array.Copy(data, value, length);
			}
			else
			{
				value = null;
			}
		}

		public IconInfo(Icon icon, string iconName, ImageFormat imageFormat, int[] imageDimensions)
		{
			IconInfoType = IconInfoType.Contents;
			MemoryStream memoryStream = new MemoryStream();
			if (icon != null)
			{
				icon.Save((Stream)memoryStream);
			}
			name = iconName;
			type = "binary";
			value = memoryStream.GetBuffer();
			memoryStream.Close();
			images = new List<ImageInfo>();
			try
			{
				MakeImages(imageFormat, imageDimensions);
			}
			catch (InvalidAppIconException ex)
			{
				TraceWrite.TraceErrorNoContext("Failed to generate icon images for " + iconName + ", error: " + ex);
			}
		}

		private void MakeImages(ImageFormat imageFormat, int[] imageDimensions)
		{
			ImageInfo imageInfo = null;
			foreach (int imageDimension in imageDimensions)
			{
				imageInfo = MakeImage(imageFormat, imageDimension);
				if (imageInfo != null)
				{
					images.Add(imageInfo);
				}
			}
		}

		private ImageInfo MakeImage(ImageFormat imageFormat, int imageDimension)
		{
			ImageInfo imageInfo = null;
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = MakeIconImage(value, imageDimension, imageDimension, imageFormat);
				if (memoryStream != null)
				{
					imageInfo = new ImageInfo();
					imageInfo.format = imageFormat;
					imageInfo.dimension = imageDimension;
					imageInfo.value = memoryStream.ToArray();
				}
			}
			finally
			{
				memoryStream?.Close();
			}
			return imageInfo;
		}

		private static MemoryStream MakeIconImage(byte[] icon_contents, int height, int width, ImageFormat format)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			MemoryStream memoryStream = null;
			MemoryStream memoryStream2 = null;
			Icon val = null;
			Bitmap val2 = null;
			try
			{
				memoryStream = new MemoryStream(icon_contents);
				try
				{
					val = new Icon((Stream)memoryStream, width, height);
				}
				catch (ArgumentException inner)
				{
					throw new InvalidAppIconException("Failed to create the Icon object, does the passed-in byte[] contain a valid icon?", inner);
				}
				val2 = val.ToBitmap();
				memoryStream2 = new MemoryStream();
				((Image)val2).Save((Stream)memoryStream2, format);
				return memoryStream2;
			}
			finally
			{
				if (val2 != null)
				{
					((Image)val2).Dispose();
				}
				if (val != null)
				{
					val.Dispose();
				}
				memoryStream?.Close();
			}
		}

		public byte[] iconBytes()
		{
			return value;
		}

		public byte[] GetImageBytes(int dimension, ImageFormat format)
		{
			byte[] array = null;
			ImageInfo imageInfo = null;
			if (images != null)
			{
				foreach (ImageInfo image in images)
				{
					if (image.dimension == dimension && image.format == format)
					{
						array = image.value;
					}
				}
			}
			if (array == null)
			{
				TraceWrite.TraceInfoNoContext("Icon name=" + name + ", size=" + dimension.ToString(CultureInfo.InvariantCulture) + " not found in cache, creating it now.");
				try
				{
					imageInfo = MakeImage(format, dimension);
				}
				catch (InvalidAppIconException ex)
				{
					TraceWrite.TraceErrorNoContext("Failed to generate icon image for " + name + ", error: " + ex);
					imageInfo = null;
				}
				if (imageInfo != null)
				{
					array = imageInfo.value;
				}
			}
			return array;
		}
	}
	internal class ImageInfo
	{
		public int dimension;

		public byte[] value;

		public ImageFormat format;
	}
	public class WorkspaceInfo
	{
		private string wsName;

		private string wsId;

		private string wsDesc;

		private string wsRedirectorName;

		private string wsEventLogUploadAddress;

		public string WorkspaceName
		{
			get
			{
				return wsName;
			}
			set
			{
				wsName = value;
			}
		}

		public string WorkspaceId
		{
			get
			{
				return wsId;
			}
			set
			{
				wsId = value;
			}
		}

		public string WorkspaceDescription
		{
			get
			{
				return wsDesc;
			}
			set
			{
				wsDesc = value;
			}
		}

		public string RedirectorName
		{
			get
			{
				return wsRedirectorName;
			}
			set
			{
				wsRedirectorName = value;
			}
		}

		public string EventLogUploadAddress
		{
			get
			{
				return wsEventLogUploadAddress;
			}
			set
			{
				wsEventLogUploadAddress = value;
			}
		}
	}
	public class RdpSigningCertificate
	{
		public string wsHash;

		private static char[] hexDigits = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};

		public static string GetHashStringFromByteArray(byte[] objByteArray)
		{
			string result = null;
			if (objByteArray != null)
			{
				int num = objByteArray.Length;
				if (num > 0)
				{
					char[] array = new char[num * 2];
					for (int i = 0; i < num; i++)
					{
						int num2 = objByteArray[i];
						array[i * 2] = hexDigits[num2 >> 4];
						array[i * 2 + 1] = hexDigits[num2 & 0xF];
					}
					result = new string(array);
				}
			}
			return result;
		}
	}
	internal class RdpSigningCertificateCacheItem
	{
		internal ulong lastUpdateTime;

		internal RdpSigningCertificate objRdpSigningCertificate;
	}
	internal class ConnectionSettings
	{
		internal bool m_useSmartCard = true;

		internal int m_Port = 3389;

		internal string m_FarmName;

		internal int m_GatewayUsage;

		internal bool m_RequireServerAuth;

		internal string m_GatewayName;
	}
	internal class TsCacheItem
	{
		internal AppInfo[] appInfoListRemote;

		internal AppInfo[] appInfoListDesktop;
	}
	internal class RWSTerminalServerAccessor : IStoreAccessor
	{
		internal class UserFilteredAppCacheItem
		{
			internal AppInfo[] appsListFiltered;

			internal AppInfo[] desktopsListFiltered;

			internal string[] tsNames;
		}

		private static int gm_stateLockTimeout;

		private static ReaderWriterLock gm_stateLock;

		private static string[] gm_TSNames;

		private static string gm_TSList;

		private static string gm_workspaceID;

		private const string RDPWorkspaceIDField = "Workspace Id:s:";

		private static bool gm_filteringDisabled;

		private AppFilterer m_filterer;

		private bool m_PollAppFreshness = true;

		private static ImageFormat gm_imageFormat;

		private static int[] gm_imageDimensions;

		private static bool gm_rdshVersionCheckDisabled;

		private TraceWrite m_TraceWrite = new TraceWrite();

		private static long gm_CacheExpirationInMinutes;

		private const long gm_DefaultCacheExpirationInMinutes = 120L;

		private const string gm_CacheConnectionSettingsPrefix = "CS:";

		private const string gm_CacheAppsPrefix = "TSApps:";

		private const string gm_RdpSignCertificateCachePrefix = "TsRdpSignCertificateCache";

		private const string gm_UserFilteredAppCachePrefix = "UserFilteredTSApps:";

		private static SortedList<string, RDSHSemiSyncEventWatcher> gm_watchers;

		private const string gm_RdpFilesDirectory = "rdp";

		private const string gm_RdpFileExtension = ".rdp";

		private const uint gm_UnsupportedRdshMinVersion = 8u;

		public bool SupportsWorkspaceReconnect => false;

		static RWSTerminalServerAccessor()
		{
			gm_stateLockTimeout = -1;
			gm_stateLock = new ReaderWriterLock();
			gm_TSNames = null;
			gm_TSList = null;
			gm_workspaceID = "";
			gm_filteringDisabled = false;
			gm_imageFormat = ImageFormat.Png;
			gm_imageDimensions = new int[1] { 32 };
			gm_rdshVersionCheckDisabled = false;
			gm_CacheExpirationInMinutes = 120L;
			gm_watchers = null;
			TraceWrite.TraceVerboseNoContext("Calling static constructor");
			TraceWrite.TraceVerboseNoContext("Setting up gm_watchers.");
			gm_watchers = new SortedList<string, RDSHSemiSyncEventWatcher>();
		}

		public RWSTerminalServerAccessor(string tsName, bool PollAppFreshness)
		{
			try
			{
				gm_stateLock.AcquireWriterLock(gm_stateLockTimeout);
				TraceWrite.TraceVerboseNoContext("Calling constructor");
				if (string.IsNullOrEmpty(tsName))
				{
					TraceWrite.TraceErrorNoContext("No TS Name specified in constructor.");
					tsName = string.Empty;
				}
				gm_TSList = tsName;
				gm_TSNames = tsName.Split(new char[1] { ';' });
				m_PollAppFreshness = PollAppFreshness;
				int num = 0;
				TraceWrite.TraceVerboseNoContext("Total of " + gm_TSNames.Length.ToString(CultureInfo.InvariantCulture) + " specified Terminal Servers.");
				string[] array = gm_TSNames;
				foreach (string strA in array)
				{
					if (string.Compare(strA, "localhost", StringComparison.OrdinalIgnoreCase) == 0)
					{
						gm_TSNames[num] = Environment.MachineName;
					}
					TraceWrite.TraceVerboseNoContext(num.ToString(CultureInfo.InvariantCulture) + ". Terminal Server: " + gm_TSNames[num] + ".");
					num++;
				}
				m_TraceWrite.Context = "Terminal Server: " + gm_TSNames[0];
				gm_workspaceID = RDWebAccessConfig.WorkspaceId;
				string text = "";
				text = ConfigurationManager.AppSettings["PointsToTSDisableAppFiltering"];
				if (!string.IsNullOrEmpty(text) && text == "true")
				{
					gm_filteringDisabled = true;
				}
				string text2 = "";
				text2 = ConfigurationManager.AppSettings["DebugFlag839"];
				if (!string.IsNullOrEmpty(text2) && text2 == "true")
				{
					gm_rdshVersionCheckDisabled = true;
				}
				string text3 = "";
				text3 = ConfigurationManager.AppSettings["CacheTimeToLiveInMinutes"];
				if (string.IsNullOrEmpty(text3))
				{
					gm_CacheExpirationInMinutes = 120L;
				}
				else
				{
					try
					{
						gm_CacheExpirationInMinutes = Convert.ToInt32(text3, NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException ex)
					{
						TraceWrite.TraceVerboseNoContext("Cannot convert Cache TTL \"" + text3 + "\" to an int, Using default. " + ex.Message);
						gm_CacheExpirationInMinutes = 120L;
					}
					catch (OverflowException ex2)
					{
						TraceWrite.TraceVerboseNoContext("Cannot convert Cache TTL \"" + text3 + "\" to an int, Using default. " + ex2.Message);
						gm_CacheExpirationInMinutes = 120L;
					}
				}
				TraceWrite.TraceVerboseNoContext("Cache TTL is " + gm_CacheExpirationInMinutes.ToString(CultureInfo.InvariantCulture) + " minutes");
			}
			finally
			{
				gm_stateLock.ReleaseWriterLock();
			}
			if (m_filterer == null)
			{
				m_filterer = new AppFilterer();
			}
			RemoveAppsForServersNotInListFromCache();
			TraceWrite.TraceVerboseNoContext("Exiting constructor");
		}

		public string GetClaimsHint()
		{
			return "";
		}

		public Task<string> GetPrivacyLinkAsync()
		{
			return Task.FromResult(string.Empty);
		}

		public Task<List<AccountDetailAndConsentStatus>> GetTenantAccountsAndUserConsentStatusAsync(object securityPrincipal)
		{
			return Task.FromResult(new List<AccountDetailAndConsentStatus>());
		}

		private static void PutRdpSigningCertificateInfoInCache(ref RdpSigningCertificate objRdpSigningCertificate)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected O, but got Unknown
			if (objRdpSigningCertificate != null)
			{
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				string text = gm_TSNames[0];
				gm_stateLock.ReleaseReaderLock();
				TraceWrite.TraceVerboseNoContext("Putting Rdp Signing Certificate into the Cache");
				RdpSigningCertificateCacheItem rdpSigningCertificateCacheItem = new RdpSigningCertificateCacheItem();
				rdpSigningCertificateCacheItem.objRdpSigningCertificate = objRdpSigningCertificate;
				rdpSigningCertificateCacheItem.lastUpdateTime = 0uL;
				HttpRuntime.Cache.Insert("TsRdpSignCertificateCache" + text, (object)rdpSigningCertificateCacheItem, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
			}
		}

		private static RdpSigningCertificate GetFreshRdpSigningCertificate()
		{
			//IL_01c5: Expected O, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_00b7: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected O, but got Unknown
			TraceWrite.TraceVerboseNoContext("Getting Fresh RDP Signing Certificate Hash");
			RdpSigningCertificate objRdpSigningCertificate = new RdpSigningCertificate();
			if (objRdpSigningCertificate != null)
			{
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				string text = gm_TSNames[0];
				gm_stateLock.ReleaseReaderLock();
				objRdpSigningCertificate.wsHash = "";
				try
				{
					if (!IsSupportedRdsh(text))
					{
						TSEventLog.Event(args: new object[1] { text }, message: LangSpecific.TSPORTAL_ERROR_14, id: 14, etype: (EventLogEntryType)1);
						TraceWrite.TraceWarningNoContext("RDSH server [" + text + "] is not a supported RDSH server, so not getting fresh RDP signing certificate");
						throw new UnsupportedSourceServerException();
					}
					ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(text);
					ManagementScope val = new ManagementScope(WmiUtility.GetWmiPathRoot(text), connectionOptions);
					val.Connect();
					ObjectQuery val2 = new ObjectQuery("SELECT * FROM Win32_TSDeploymentSettings");
					ManagementObjectCollection val3 = new ManagementObjectSearcher(val, val2).Get();
					if (val3 != null)
					{
						ManagementObjectEnumerator enumerator = val3.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								ManagementObject val4 = (ManagementObject)enumerator.Current;
								if (((ManagementBaseObject)val4)["HasCertificate"] == null)
								{
									continue;
								}
								bool flag = false;
								try
								{
									flag = bool.Parse(((ManagementBaseObject)val4)["HasCertificate"].ToString());
								}
								catch (ArgumentNullException ex)
								{
									TraceWrite.TraceWarningNoContext("Exception Message : {0}, Param : {1}.", ex.Message, ex.ParamName);
								}
								catch (FormatException ex2)
								{
									TraceWrite.TraceWarningNoContext("Exception Message : {0}.", ex2.Message);
								}
								if (flag && ((ManagementBaseObject)val4)["CertificateHash"] != null)
								{
									TraceWrite.TraceVerboseNoContext("Found Certificate which has a Certificate Hash");
									if (((ManagementBaseObject)val4)["CertificateHash"] is byte[] objByteArray)
									{
										objRdpSigningCertificate.wsHash = RdpSigningCertificate.GetHashStringFromByteArray(objByteArray);
									}
									break;
								}
							}
						}
						finally
						{
							((IDisposable)enumerator)?.Dispose();
						}
					}
					PutRdpSigningCertificateInfoInCache(ref objRdpSigningCertificate);
				}
				catch (ManagementException val5)
				{
					ManagementException val6 = val5;
					TSEventLog.Event(args: new object[1] { text }, message: LangSpecific.TSPORTAL_ERROR_5, id: 5, etype: (EventLogEntryType)2);
					TraceWrite.TraceErrorNoContext("WMI error accessing Terminal Server: " + ((Exception)(object)val6).Message);
				}
				catch (UnauthorizedAccessException ex3)
				{
					TSEventLog.Event(args: new object[1] { text }, message: LangSpecific.TSPORTAL_ERROR_8, id: 8, etype: (EventLogEntryType)1);
					TraceWrite.TraceErrorNoContext("<GetFreshRdpSigningCertificate> WMI access denied accessing Terminal Server: " + ex3.Message);
				}
				catch (COMException ex4)
				{
					TSEventLog.Event(args: new object[1] { text }, message: LangSpecific.TSPORTAL_ERROR_5, id: 5, etype: (EventLogEntryType)2);
					TraceWrite.TraceErrorNoContext("WMI COM error accessing Terminal Server: " + ex4.Message);
				}
				catch (NullReferenceException)
				{
					(new object[1])[0] = text;
				}
			}
			return objRdpSigningCertificate;
		}

		private object GetRdpSigningCertificateInfoFromCache()
		{
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string text = gm_TSNames[0];
			gm_stateLock.ReleaseReaderLock();
			UpdateCacheFromWMIEvents(text);
			return HttpRuntime.Cache["TsRdpSignCertificateCache" + text];
		}

		public RdpSigningCertificate GetRdpSigningCertificate()
		{
			RdpSigningCertificate result = null;
			if (!HttpContext.Current.Response.IsClientConnected)
			{
				TraceWrite.TraceInfoNoContext("Returning early, client not connected");
			}
			else
			{
				object rdpSigningCertificateInfoFromCache = GetRdpSigningCertificateInfoFromCache();
				if (rdpSigningCertificateInfoFromCache != null)
				{
					TraceWrite.TraceVerboseNoContext("Got Rdp Certificate from cache");
					result = (rdpSigningCertificateInfoFromCache as RdpSigningCertificateCacheItem).objRdpSigningCertificate;
				}
				else
				{
					m_TraceWrite.TraceInfo("Cache has expired, downloading new Rdp Certificate.");
					result = GetFreshRdpSigningCertificate();
				}
			}
			return result;
		}

		public WorkspaceInfo GetWorkspaceInfo()
		{
			return null;
		}

		public Task<Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>> GetApplicationsAsync(string strSid, bool onlyShowAvailableByDefaultResources, bool includeRDPFileContents)
		{
			AppInfo[] array = null;
			AppInfo[] array2 = null;
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string[] array3 = gm_TSNames;
			bool flag = gm_filteringDisabled;
			gm_stateLock.ReleaseReaderLock();
			if (flag)
			{
				m_TraceWrite.TraceInfo("App Filtering is disabled.");
			}
			List<AppInfo> list = new List<AppInfo>();
			List<AppInfo> list2 = new List<AppInfo>();
			DateTime utcNow = DateTime.UtcNow;
			string[] array4 = array3;
			foreach (string text in array4)
			{
				object appsFromCache = GetAppsFromCache(text);
				TsCacheItem tsCacheItem;
				if (appsFromCache == null)
				{
					tsCacheItem = GetFreshApplications(text, utcNow);
				}
				else
				{
					tsCacheItem = appsFromCache as TsCacheItem;
					m_TraceWrite.TraceVerbose("Apps found in cache for server: " + text);
				}
				if (tsCacheItem.appInfoListRemote != null)
				{
					list.AddRange(tsCacheItem.appInfoListRemote);
				}
				if (tsCacheItem.appInfoListDesktop != null)
				{
					list2.AddRange(tsCacheItem.appInfoListDesktop);
				}
			}
			array = list.ToArray();
			array2 = list2.ToArray();
			if (!flag)
			{
				AppInfo[] filteredApps = null;
				AppInfo[] filteredDesktops = null;
				bool flag2 = false;
				if (!m_PollAppFreshness)
				{
					flag2 = GetFilteredAppsFromCache(strSid, out filteredApps, out filteredDesktops);
				}
				if (!flag2)
				{
					m_filterer.FilterApplications(array, array2, strSid, out filteredApps, out filteredDesktops);
					PutFilteredAppsInCache(strSid, ref filteredApps, ref filteredDesktops);
					array = filteredApps;
					array2 = filteredDesktops;
				}
			}
			return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(array, array2));
		}

		internal static TsCacheItem GetFreshApplications(string tsName, DateTime requestTime)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_02dc: Expected O, but got Unknown
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Expected O, but got Unknown
			ArrayList apps = new ArrayList();
			ArrayList apps2 = new ArrayList();
			TsCacheItem tsciData = new TsCacheItem();
			EnumerationOptions val = new EnumerationOptions();
			TimeSpan timeout = new TimeSpan(0, 1, 0);
			((ManagementOptions)val).Timeout = timeout;
			try
			{
				if (!IsSupportedRdsh(tsName))
				{
					TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_14, id: 14, etype: (EventLogEntryType)1);
					TraceWrite.TraceWarningNoContext("RDSH server [" + tsName + "] is not a supported RDSH server, so not getting fresh applications");
					throw new UnsupportedSourceServerException();
				}
				RDSHSemiSyncEventWatcher rDSHSemiSyncEventWatcher;
				lock (gm_watchers)
				{
					if (!gm_watchers.ContainsKey(tsName) || (rDSHSemiSyncEventWatcher = gm_watchers[tsName]) == null)
					{
						TraceWrite.TraceVerboseNoContext("Adding event watcher for server " + tsName);
						rDSHSemiSyncEventWatcher = (gm_watchers[tsName] = new RDSHSemiSyncEventWatcher(tsName));
					}
				}
				lock (rDSHSemiSyncEventWatcher)
				{
					if (!rDSHSemiSyncEventWatcher.Connected)
					{
						rDSHSemiSyncEventWatcher.Connect();
					}
				}
				string text = WmiUtility.GetWmiPathRoot(tsName) + ":Win32_TSPublishedApplication";
				ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(tsName);
				ManagementScope mgmtScope = new ManagementScope(new ManagementPath(text), connectionOptions);
				TSPublishedApplication.TSPublishedApplicationCollection instances = TSPublishedApplication.GetInstances(mgmtScope, val);
				TraceWrite.TraceInfoNoContext("Received " + instances.Count.ToString(CultureInfo.InvariantCulture) + " apps from " + tsName);
				TSPublishedDesktop.TSPublishedDesktopCollection instances2 = TSPublishedDesktop.GetInstances(mgmtScope, val);
				TraceWrite.TraceInfoNoContext("Received " + instances2.Count.ToString(CultureInfo.InvariantCulture) + " desktops from " + tsName);
				TraceWrite.TraceInfoNoContext("Checking Apps...");
				foreach (TSPublishedApplication item in instances)
				{
					TraceWrite.TraceInfoNoContext("Checking Program: " + item.Name);
					if (item.ShowInPortal)
					{
						ProcessPublishedApp(item, tsName, ref apps, requestTime);
						TraceWrite.TraceInfoNoContext("Processed " + item.Name);
					}
					else
					{
						TraceWrite.TraceInfoNoContext("Program is not meant to be shown in TSWA");
					}
				}
				TraceWrite.TraceInfoNoContext("Checking Desktops...");
				foreach (TSPublishedDesktop item2 in instances2)
				{
					TraceWrite.TraceInfoNoContext("Checking Desktop: " + item2.Name);
					if (item2.ShowInPortal && !item2.IsVmFarm)
					{
						ProcessPublishedDesktop(item2, tsName, ref apps2, requestTime);
					}
					else
					{
						TraceWrite.TraceInfoNoContext("Desktop is not meant to be shown in TSWA");
					}
				}
			}
			catch (ManagementException val2)
			{
				ManagementException val3 = val2;
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_5, id: 5, etype: (EventLogEntryType)2);
				TraceWrite.TraceErrorNoContext("WMI error accessing Terminal Server: " + ((Exception)(object)val3).Message);
				return tsciData;
			}
			catch (UnauthorizedAccessException ex)
			{
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_8, id: 8, etype: (EventLogEntryType)1);
				TraceWrite.TraceErrorNoContext("<GetFreshApplications> WMI access denied accessing Terminal Server: " + ex.Message + ", STACK: " + ex.StackTrace);
				return tsciData;
			}
			catch (COMException ex2)
			{
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_5, id: 5, etype: (EventLogEntryType)2);
				TraceWrite.TraceErrorNoContext("WMI COM error accessing Terminal Server: " + ex2.Message);
				return tsciData;
			}
			catch (FileNotFoundException ex3)
			{
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_8, id: 8, etype: (EventLogEntryType)1);
				TraceWrite.TraceErrorNoContext("<GetFreshApplications> WMI error accessing Terminal Server:  " + ex3.Message);
				return tsciData;
			}
			catch (Win32Exception ex4)
			{
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_8, id: 8, etype: (EventLogEntryType)1);
				TraceWrite.TraceErrorNoContext("<GetFreshApplications> WMI error accessing Terminal Server:  " + ex4.Message);
				return tsciData;
			}
			if (apps.Count + apps2.Count <= 0)
			{
				TSEventLog.Event(args: new object[1] { tsName }, message: LangSpecific.TSPORTAL_ERROR_4, id: 4, etype: (EventLogEntryType)2);
			}
			TraceWrite.TraceInfoNoContext("TS: " + tsName + " Apps: " + apps.Count.ToString(CultureInfo.InvariantCulture) + " Desktops: " + apps2.Count.ToString(CultureInfo.InvariantCulture));
			tsciData.appInfoListRemote = (AppInfo[])apps.ToArray(typeof(AppInfo));
			tsciData.appInfoListDesktop = (AppInfo[])apps2.ToArray(typeof(AppInfo));
			PutAppsInCache(tsName, ref tsciData);
			return tsciData;
		}

		public static ConnectionSettings GetConnectionSettings(string ServerName)
		{
			object connectionSettingsFromCache = GetConnectionSettingsFromCache(ServerName);
			if (connectionSettingsFromCache != null)
			{
				TraceWrite.TraceVerboseNoContext("Got connections settings from cache");
				return connectionSettingsFromCache as ConnectionSettings;
			}
			return GetFreshConnectionSettings(ServerName);
		}

		internal static ConnectionSettings GetFreshConnectionSettings(string ServerName)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected O, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			ConnectionSettings settings = new ConnectionSettings();
			ObjectGetOptions val = new ObjectGetOptions();
			if (ServerName != null)
			{
				char[] trimChars = new char[3] { '\\', ' ', '\t' };
				ServerName = ServerName.TrimStart(trimChars);
			}
			ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(ServerName);
			ManagementScope val2 = ((ServerName != null) ? new ManagementScope("\\\\" + ServerName + "\\root\\cimv2\\TerminalServices", connectionOptions) : new ManagementScope("\\\\.\\root\\cimv2\\TerminalServices", connectionOptions));
			ManagementPath val3 = new ManagementPath("Win32_TSDeploymentSettings");
			ManagementClass val4 = new ManagementClass(val2, val3, val);
			TraceWrite.TraceInfoNoContext("Getting connection settings from Terminal Server: " + ServerName);
			ManagementObjectEnumerator enumerator = val4.GetInstances().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ManagementObject val5 = (ManagementObject)enumerator.Current;
					val5.Get();
					settings.m_Port = (int)((ManagementBaseObject)val5)["Port"];
					settings.m_FarmName = (string)((ManagementBaseObject)val5)["FarmName"];
					settings.m_GatewayUsage = (int)((ManagementBaseObject)val5)["GatewayUsage"];
					settings.m_GatewayName = (string)((ManagementBaseObject)val5)["GatewayName"];
					if ((int)((ManagementBaseObject)val5)["GatewayAuthMode"] == 1)
					{
						settings.m_useSmartCard = true;
					}
					else
					{
						settings.m_useSmartCard = false;
					}
					settings.m_RequireServerAuth = (bool)((ManagementBaseObject)val5)["RequireServerAuth"];
					PutConnectionSettingsInCache(ServerName, ref settings);
					TraceWrite.TraceVerboseNoContext("Succeeded putting connection settings in cache");
				}
				return settings;
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
		}

		private static object GetConnectionSettingsFromCache(string serverName)
		{
			UpdateCacheFromWMIEvents(serverName);
			return HttpRuntime.Cache["CS:" + serverName];
		}

		private static void PutConnectionSettingsInCache(string serverName, ref ConnectionSettings settings)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			if (settings != null)
			{
				HttpRuntime.Cache.Insert("CS:" + serverName, (object)settings, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
			}
		}

		private object GetAppsFromCache(string serverName)
		{
			UpdateCacheFromWMIEvents(serverName);
			return HttpRuntime.Cache["TSApps:" + serverName];
		}

		private static void PutAppsInCache(string tsName, ref TsCacheItem tsciData)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			if (tsciData != null)
			{
				HttpRuntime.Cache.Insert("TSApps:" + tsName, (object)tsciData, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
			}
		}

		private static bool GetFilteredAppsFromCache(string userSid, out AppInfo[] filteredApps, out AppInfo[] filteredDesktops)
		{
			filteredApps = null;
			filteredDesktops = null;
			bool result = false;
			bool flag = false;
			string text = "UserFilteredTSApps:" + userSid;
			if (HttpRuntime.Cache[text] is UserFilteredAppCacheItem userFilteredAppCacheItem)
			{
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				string[] array = gm_TSNames;
				gm_stateLock.ReleaseReaderLock();
				flag = userFilteredAppCacheItem.tsNames.Length != array.Length;
				int num = 0;
				while (!flag && num < array.Length)
				{
					flag = string.CompareOrdinal(userFilteredAppCacheItem.tsNames[num], array[num]) != 0;
					num++;
				}
				if (flag)
				{
					TraceWrite.TraceVerboseNoContext("Filtered apps removed from the cache for " + userSid);
					HttpRuntime.Cache.Remove(text);
				}
				else
				{
					TraceWrite.TraceVerboseNoContext("Filtered apps found in the cache for " + userSid);
					filteredApps = userFilteredAppCacheItem.appsListFiltered;
					filteredDesktops = userFilteredAppCacheItem.desktopsListFiltered;
					result = true;
				}
			}
			return result;
		}

		private static void PutFilteredAppsInCache(string userSid, ref AppInfo[] filteredApps, ref AppInfo[] filteredDesktops)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			if (filteredApps != null && filteredDesktops != null)
			{
				UserFilteredAppCacheItem userFilteredAppCacheItem = new UserFilteredAppCacheItem();
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				userFilteredAppCacheItem.tsNames = (string[])gm_TSNames.Clone();
				gm_stateLock.ReleaseReaderLock();
				userFilteredAppCacheItem.appsListFiltered = filteredApps;
				userFilteredAppCacheItem.desktopsListFiltered = filteredDesktops;
				string[] array = new string[userFilteredAppCacheItem.tsNames.Length];
				for (int i = 0; i < userFilteredAppCacheItem.tsNames.Length; i++)
				{
					array[i] = "TSApps:" + userFilteredAppCacheItem.tsNames[i];
				}
				CacheDependency val = new CacheDependency((string[])null, array);
				HttpRuntime.Cache.Insert("UserFilteredTSApps:" + userSid, (object)userFilteredAppCacheItem, val);
				TraceWrite.TraceVerboseNoContext("Filtered apps stored in the cache for " + userSid);
			}
		}

		private void RemoveAppsForServersNotInListFromCache()
		{
			IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string[] array = gm_TSNames;
			gm_stateLock.ReleaseReaderLock();
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key.ToString();
				if (string.CompareOrdinal(text, 0, "TSApps:", 0, "TSApps:".Length) != 0)
				{
					continue;
				}
				bool flag = false;
				string[] array2 = array;
				foreach (string strB in array2)
				{
					if (string.CompareOrdinal(text.Substring("TSApps:".Length), strB) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					TraceWrite.TraceVerboseNoContext("Deleting key " + text + " from cache, since the TSs it refers to are not being pointed-to by TSWA");
					_ = enumerator.Value;
					HttpRuntime.Cache.Remove(text);
				}
			}
			lock (gm_watchers)
			{
				for (int num = gm_watchers.Count - 1; num >= 0; num--)
				{
					string text2 = gm_watchers.Keys[num];
					bool flag = false;
					string[] array2 = array;
					foreach (string strB2 in array2)
					{
						if (string.CompareOrdinal(text2, strB2) == 0)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						TraceWrite.TraceVerboseNoContext("Deleting key " + text2 + " from Watchers, since the TSs it refers to are not being pointed-to by TSWA");
						RDSHSemiSyncEventWatcher rDSHSemiSyncEventWatcher = gm_watchers.Values[num];
						if (rDSHSemiSyncEventWatcher != null)
						{
							TraceWrite.TraceVerboseNoContext("Removing event watcher for server " + text2);
							try
							{
								lock (rDSHSemiSyncEventWatcher)
								{
									rDSHSemiSyncEventWatcher.Disconnect();
								}
							}
							catch (COMException ex)
							{
								TraceWrite.TraceErrorNoContext("WMI COM error stopping WMI watcher: " + ex.Message);
							}
						}
						gm_watchers.RemoveAt(num);
					}
				}
			}
		}

		private static void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason removedReason)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			TraceWrite.TraceVerboseNoContext("Cache item " + key + " removed.");
			if ((int)removedReason != 2)
			{
				return;
			}
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string[] array = gm_TSNames;
			gm_stateLock.ReleaseReaderLock();
			if (string.CompareOrdinal(key, 0, "CS:", 0, "CS:".Length) == 0)
			{
				GetFreshConnectionSettings(key.Substring("CS:".Length));
			}
			else if (string.CompareOrdinal(key, 0, "TSApps:", 0, "TSApps:".Length) == 0)
			{
				DateTime utcNow = DateTime.UtcNow;
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (string.CompareOrdinal(key.Substring("TSApps:".Length), text) == 0)
					{
						GetFreshApplications(text, utcNow);
						break;
					}
				}
			}
			else if (string.CompareOrdinal(key, 0, "TsRdpSignCertificateCache", 0, "TsRdpSignCertificateCache".Length) == 0 && string.CompareOrdinal(key.Substring("TsRdpSignCertificateCache".Length), array[0]) == 0)
			{
				GetFreshRdpSigningCertificate();
			}
		}

		private static void ProcessPublishedApp(TSPublishedApplication tsPublishedApp, string tsName, ref ArrayList apps, DateTime requestTime)
		{
			RemoteAppXml remoteAppXml = new RemoteAppXml();
			string text = tsPublishedApp.RDPFileContents;
			if (string.IsNullOrEmpty(text))
			{
				text = GetRdpProperties(tsPublishedApp, tsName);
			}
			text = addworkspaceInfoToRDPFile(text);
			string rdp_file_name = "";
			string text2 = tsName + "-" + tsPublishedApp.Alias;
			IconInfo icon = GetIcon(tsPublishedApp.IconContents, text2);
			PrepareRdpFile(ref text, text2, out rdp_file_name, requestTime);
			text = GlobalObject.escape((object)text);
			remoteAppXml.Initialize(rdpFileInfo: new RDPFileInfo(rdp_file_name, text, string.Empty), applicationName: tsPublishedApp.Name, extensions: new FileExtensionInfo[0], folderNames: new string[0], icon: icon, applicationAlias: text2, requiredCommandLine: tsPublishedApp.RequiredCommandLine, applicationPath: tsPublishedApp.Path0, SecurityDescriptor: tsPublishedApp.SecurityDescriptor);
			AppInfo value = new AppInfo(tsPublishedApp.Name, tsName, remoteAppXml, isRemoteDesktop: false, requestTime, null, availableToUsersByDefault: true);
			apps.Insert(0, value);
		}

		private static void ProcessPublishedDesktop(TSPublishedDesktop tsPublishedDesktop, string tsName, ref ArrayList apps, DateTime requestTime)
		{
			RemoteAppXml remoteAppXml = new RemoteAppXml();
			string text = tsPublishedDesktop.RDPFileContents;
			if (string.IsNullOrEmpty(text))
			{
				text = GetRdpProperties(tsPublishedDesktop, tsName);
			}
			text = addworkspaceInfoToRDPFile(text);
			string rdp_file_name = "";
			string text2 = tsName + "-" + tsPublishedDesktop.Alias;
			IconInfo icon = GetIcon(tsPublishedDesktop.IconContents, text2);
			PrepareRdpFile(ref text, text2, out rdp_file_name, requestTime);
			text = GlobalObject.escape((object)text);
			remoteAppXml.Initialize(rdpFileInfo: new RDPFileInfo(rdp_file_name, text, string.Empty), applicationName: tsPublishedDesktop.Name, extensions: new FileExtensionInfo[0], folderNames: new string[0], icon: icon, applicationAlias: text2, requiredCommandLine: "", applicationPath: "", SecurityDescriptor: tsPublishedDesktop.SecurityDescriptor);
			AppInfo value = new AppInfo(tsPublishedDesktop.Name, tsName, remoteAppXml, isRemoteDesktop: true, requestTime, null, availableToUsersByDefault: true);
			apps.Add(value);
		}

		private static IconInfo GetIcon(byte[] iconContents, string iconPath)
		{
			IconInfo iconInfo = null;
			if (iconContents != null)
			{
				iconInfo = new IconInfo(iconContents, iconPath, iconContents.Length, gm_imageFormat, gm_imageDimensions);
			}
			else
			{
				iconInfo = CreateDefaultIcon(iconPath);
				TraceWrite.TraceWarningNoContext("Program's icon file not available - using default.");
			}
			return iconInfo;
		}

		private static string addworkspaceInfoToRDPFile(string rdpProperties)
		{
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string text = gm_workspaceID;
			gm_stateLock.ReleaseReaderLock();
			rdpProperties = removeWorkspaceIDFromRDPFile(rdpProperties);
			TraceWrite.TraceVerboseNoContext("Adding workspace ID: Workspace Id:s:" + text + " to the RDP file.");
			rdpProperties = rdpProperties + "Workspace Id:s:" + text + "\r\n";
			return rdpProperties;
		}

		private static string removeWorkspaceIDFromRDPFile(string rdpProperties)
		{
			int num = rdpProperties.IndexOf("Workspace Id:s:", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return rdpProperties;
			}
			int num2 = rdpProperties.IndexOf('\n', num);
			if (num2 == -1)
			{
				num2 = rdpProperties.Length - 1;
			}
			else
			{
				try
				{
					if (rdpProperties.Substring(num2, 1).Equals("\r"))
					{
						num2++;
					}
				}
				catch (ArgumentOutOfRangeException)
				{
				}
			}
			int count = num2 - num;
			try
			{
				rdpProperties = rdpProperties.Remove(num, count);
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				TraceWrite.TraceWarningNoContext("Unable to succesfully remove RDPWorkspace Field: " + ex2.ToString());
			}
			return rdpProperties;
		}

		private static void PrepareRdpFile(ref string rdp_contents, string alias, out string rdp_file_name, DateTime requestTime)
		{
			TraceWrite.TraceVerboseNoContext("Preparing RDP file for " + alias);
			rdp_file_name = "ts-" + alias + ".rdp";
			bool flag = false;
			try
			{
				flag = Convert.ToBoolean(ConfigurationManager.AppSettings["OptimizeExperienceState"], CultureInfo.InvariantCulture);
			}
			catch (ConfigurationErrorsException)
			{
				TraceWrite.TraceInfoNoContext("OptimizeExperienceState not present in config file, not optimizing for LAN");
			}
			catch (FormatException)
			{
				TraceWrite.TraceInfoNoContext("OptimizeExperienceState is not in the correct format, not optimizing for LAN");
			}
			if (!flag)
			{
				return;
			}
			int num = rdp_contents.IndexOf("connection type:i:", StringComparison.OrdinalIgnoreCase);
			if (num != -1)
			{
				TraceWrite.TraceInfoNoContext("Connection type detected, removing the string.");
				int num2 = rdp_contents.IndexOf("\n", num, StringComparison.OrdinalIgnoreCase);
				if (num2 != -1)
				{
					rdp_contents = rdp_contents.Remove(num, num2 - num + 1);
				}
				else
				{
					rdp_contents = rdp_contents.Remove(num);
				}
			}
			if (rdp_contents.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
			{
				rdp_contents += "connection type:i:6\r\n";
			}
			else
			{
				rdp_contents += "\r\nconnection type:i:6\r\n";
			}
			TraceWrite.TraceInfoNoContext("Optimizing RDP for LAN.");
		}

		private static StringBuilder GetCommonRdpProperties(string ts_Name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ConnectionSettings connectionSettings = GetConnectionSettings(ts_Name);
			stringBuilder.Append("full address:s:" + ts_Name + "\r\n");
			if (connectionSettings.m_Port != 0)
			{
				stringBuilder.Append("Server Port:i:" + connectionSettings.m_Port.ToString(CultureInfo.InvariantCulture) + "\r\n");
			}
			if (connectionSettings.m_RequireServerAuth)
			{
				stringBuilder.Append("authentication level:i:1\r\n");
			}
			else
			{
				stringBuilder.Append("authentication level:i:2\r\n");
			}
			if (connectionSettings.m_GatewayUsage == 0)
			{
				stringBuilder.Append("gatewayusagemethod:i:0\r\n");
				stringBuilder.Append("gatewayhostname:s:\r\n");
				stringBuilder.Append("gatewayprofileusagemethod:i:1\r\n");
			}
			else if (connectionSettings.m_GatewayUsage == 1)
			{
				stringBuilder.Append("gatewayusagemethod:i:2\r\n");
				stringBuilder.Append("gatewayhostname:s:" + connectionSettings.m_GatewayName + "\r\n");
				stringBuilder.Append("gatewayprofileusagemethod:i:1\r\n");
			}
			else if (connectionSettings.m_GatewayUsage == 2)
			{
				stringBuilder.Append("gatewayusagemethod:i:1\r\n");
				stringBuilder.Append("gatewayhostname:s:" + connectionSettings.m_GatewayName + "\r\n");
				stringBuilder.Append("gatewayprofileusagemethod:i:1\r\n");
			}
			else if (connectionSettings.m_GatewayUsage == 3)
			{
				stringBuilder.Append("gatewayusagemethod:i:1\r\n");
				stringBuilder.Append("gatewayhostname:s:" + connectionSettings.m_GatewayName + "\r\n");
				stringBuilder.Append("gatewayprofileusagemethod:i:0\r\n");
			}
			if (connectionSettings.m_useSmartCard)
			{
				stringBuilder.Append("gatewaycredentialssource:i:1\r\n");
			}
			else
			{
				stringBuilder.Append("gatewaycredentialssource:i:0\r\n");
			}
			return stringBuilder;
		}

		private static string GetRdpProperties(TSPublishedApplication tsPublishedApp, string tsName)
		{
			StringBuilder commonRdpProperties = GetCommonRdpProperties(tsName);
			if (!string.IsNullOrEmpty(tsPublishedApp.Alias))
			{
				commonRdpProperties.Append("alternate shell:s:||" + tsPublishedApp.Alias + "\r\n");
			}
			else
			{
				commonRdpProperties.Append("alternate shell:s:" + tsPublishedApp.Path0 + "\r\n");
			}
			commonRdpProperties.Append("remoteapplicationname:s:" + tsPublishedApp.Name + "\r\n");
			if (tsPublishedApp.CommandLineSetting == TSPublishedApplication.CommandLineSettingValues.Require)
			{
				commonRdpProperties.Append("remoteapplicationcmdline:s:" + tsPublishedApp.RequiredCommandLine + "\r\n");
			}
			return commonRdpProperties.ToString();
		}

		private static string GetRdpProperties(TSPublishedDesktop tsPublishedDesktop, string tsName)
		{
			return GetCommonRdpProperties(tsName).ToString();
		}

		private static IconInfo CreateDefaultIcon(string name)
		{
			return new IconInfo(NeutralResources.mstscIcon, name, gm_imageFormat, gm_imageDimensions);
		}

		private static void UpdateCacheFromWMIEvents(string serverName)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			ManagementBaseObject val = null;
			RDSHSemiSyncEventWatcher rDSHSemiSyncEventWatcher = null;
			bool flag = false;
			lock (gm_watchers)
			{
				if (gm_watchers.ContainsKey(serverName))
				{
					rDSHSemiSyncEventWatcher = gm_watchers[serverName];
				}
			}
			if (rDSHSemiSyncEventWatcher != null)
			{
				try
				{
					lock (rDSHSemiSyncEventWatcher)
					{
						while ((val = rDSHSemiSyncEventWatcher.GetNextEvent()) != null)
						{
							TraceWrite.TraceVerboseNoContext("Processing a WMI event from server " + serverName);
							if (!flag)
							{
								string text = null;
								try
								{
									text = (string)((ManagementBaseObject)val["TargetInstance"]).SystemProperties["__Class"].Value;
								}
								catch (ManagementException)
								{
								}
								catch (COMException)
								{
								}
								if (text == null || string.Compare(text, "Win32_TSPublishedApplication", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text, "Win32_TSRemoteDesktop", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text, "Win32_TSDeploymentSettings", StringComparison.OrdinalIgnoreCase) == 0)
								{
									flag = true;
									TraceWrite.TraceVerboseNoContext("Clearing the cache for server " + serverName + " in response to a WMI event");
								}
							}
						}
					}
				}
				catch (RDSHSemiSyncEventWatcher.WatcherNotReadyException ex2)
				{
					TraceWrite.TraceWarningNoContext("The event watcher is not ready for event processing: " + ex2.Message);
					flag = true;
				}
				catch
				{
					flag = true;
				}
			}
			if (flag)
			{
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				string strB = gm_TSNames[0];
				gm_stateLock.ReleaseReaderLock();
				HttpRuntime.Cache.Remove("TSApps:" + serverName);
				HttpRuntime.Cache.Remove("CS:" + serverName);
				if (string.CompareOrdinal(serverName, strB) == 0)
				{
					HttpRuntime.Cache.Remove("TsRdpSignCertificateCache" + serverName);
				}
			}
		}

		internal static bool IsSupportedRdsh(string tsName)
		{
			//IL_009f: Expected O, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Invalid comparison between I4 and Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_0065: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			bool result = false;
			TraceWrite.TraceVerboseNoContext("Checking whether [" + tsName + "] is a supported RDSH server");
			if (gm_rdshVersionCheckDisabled)
			{
				TraceWrite.TraceWarningNoContext("Debug Flag 839 was set");
				result = true;
			}
			else
			{
				ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(tsName);
				ManagementScope val = new ManagementScope(WmiUtility.GetWmiPathRoot(tsName), connectionOptions);
				val.Connect();
				ObjectQuery val2 = new ObjectQuery("SELECT * FROM Win32_TSSystemInfo");
				ManagementObjectCollection val3 = new ManagementObjectSearcher(val, val2).Get();
				if (val3 != null)
				{
					ManagementObjectEnumerator enumerator = val3.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							ManagementObject val4 = (ManagementObject)enumerator.Current;
							try
							{
								result = (uint)((ManagementBaseObject)val4)["ProviderVersion"] < 8;
							}
							catch (ManagementException val5)
							{
								ManagementException val6 = val5;
								if (-2147217406 != (int)val6.ErrorCode)
								{
									throw;
								}
								TraceWrite.TraceInfoNoContext("The WMI provider does not support the ProviderVersion WMI property, it must be Windows Server 2008 R2 or earlier");
								result = true;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator)?.Dispose();
					}
				}
			}
			return result;
		}
	}
	internal static class WmiUtility
	{
		internal static ConnectionOptions GetConnectionOptions(string tsName)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			return new ConnectionOptions
			{
				Authentication = (AuthenticationLevel)6
			};
		}

		internal static string GetWmiPathRoot(string tsName)
		{
			return "\\\\" + tsName + "\\root\\cimv2\\TerminalServices";
		}
	}
	public class RemoteAppXml
	{
		private string m_applicationName;

		private FileExtensionInfo[] m_extensions;

		private HashSet<string> m_folderNames;

		private RDPFileInfo m_rdpFileInfo;

		private IconInfo m_icon;

		private string m_applicationAlias;

		private string m_requiredCommandLine;

		private string m_applicationPath;

		private string m_securityDescriptor;

		public string ApplicationName => m_applicationName;

		public FileExtensionInfo[] Extensions => m_extensions;

		public HashSet<string> FolderNames => m_folderNames;

		public RDPFileInfo RdpFileInfo => m_rdpFileInfo;

		public IconInfo Icon
		{
			get
			{
				return m_icon;
			}
			set
			{
				m_icon = value;
			}
		}

		public string ApplicationAlias => m_applicationAlias;

		public string RequiredCommandLine => m_requiredCommandLine;

		public string ApplicationPath => m_applicationPath;

		public string SecurityDescriptor => m_securityDescriptor;

		public void Initialize(string applicationName, FileExtensionInfo[] extensions, string[] folderNames, RDPFileInfo rdpFileInfo, IconInfo icon, string applicationAlias, string requiredCommandLine, string applicationPath, string SecurityDescriptor)
		{
			m_applicationName = applicationName;
			m_extensions = extensions;
			m_rdpFileInfo = rdpFileInfo;
			m_icon = icon;
			m_applicationAlias = applicationAlias;
			m_requiredCommandLine = requiredCommandLine;
			m_applicationPath = applicationPath;
			if (SecurityDescriptor == null)
			{
				m_securityDescriptor = "";
			}
			else
			{
				m_securityDescriptor = SecurityDescriptor;
			}
			m_folderNames = new HashSet<string>(folderNames, StringComparer.OrdinalIgnoreCase);
			m_folderNames.RemoveWhere(IllegalPathName);
		}

		public static string CleanupRdpFilePath(string path)
		{
			string text = path;
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			foreach (char oldChar in invalidFileNameChars)
			{
				text = text.Replace(oldChar, '_');
			}
			text = text.Replace(' ', '_');
			text = text.Replace('(', '_');
			text = text.Replace(')', '_');
			text = text.Replace('\'', '_');
			return text.Replace('"', '_');
		}

		private static bool IllegalPathName(string folderName)
		{
			bool flag = true;
			if (!string.IsNullOrEmpty(folderName))
			{
				char c = folderName[folderName.Length - 1];
				flag = !Regex.Match(folderName, "^/([^<>:\"/\\\\|?*\\x01-\\x1F]+)?$", RegexOptions.CultureInvariant).Success || char.IsWhiteSpace(c) || c.Equals('.');
			}
			if (flag)
			{
				TraceWrite.TraceErrorNoContext("Illegal folder name detected, removing from the folder list.  Folder name = [" + folderName + "]");
			}
			return flag;
		}
	}
	internal class TSPublishedApplication : Component
	{
		public enum CommandLineSettingValues
		{
			DoNotAllow,
			Allow,
			Require,
			NULL_ENUM_VALUE
		}

		public enum ProxyUsageValues
		{
			NeverUseProxy,
			AlwaysUseProxy,
			UseProxyIfDirectFails,
			NULL_ENUM_VALUE
		}

		public class TSPublishedApplicationCollection : ICollection, IEnumerable
		{
			public class TSPublishedApplicationEnumerator : IEnumerator
			{
				private ManagementObjectEnumerator privObjEnum;

				public virtual object Current => new TSPublishedApplication((ManagementObject)privObjEnum.Current);

				public TSPublishedApplicationEnumerator(ManagementObjectEnumerator objEnum)
				{
					privObjEnum = objEnum;
				}

				public virtual bool MoveNext()
				{
					return privObjEnum.MoveNext();
				}

				public virtual void Reset()
				{
					privObjEnum.Reset();
				}
			}

			private ManagementObjectCollection privColObj;

			public virtual int Count => privColObj.Count;

			public virtual bool IsSynchronized => privColObj.IsSynchronized;

			public virtual object SyncRoot => this;

			public TSPublishedApplicationCollection(ManagementObjectCollection objCollection)
			{
				privColObj = objCollection;
			}

			public virtual void CopyTo(Array array, int index)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Expected O, but got Unknown
				if (array != null)
				{
					privColObj.CopyTo(array, index);
					for (int i = 0; i < array.Length; i++)
					{
						array.SetValue(new TSPublishedApplication((ManagementObject)array.GetValue(i)), i);
					}
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new TSPublishedApplicationEnumerator(privColObj.GetEnumerator());
			}
		}

		private static string CreatedClassName = "Win32_TSPublishedApplication";

		private ManagementObject PrivateLateBoundObject;

		private bool AutoCommitProp;

		private ManagementBaseObject curObj;

		private bool isEmbedded;

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ManagementClassName
		{
			get
			{
				string text = CreatedClassName;
				if (curObj != null && curObj.ClassPath != null)
				{
					text = (string)curObj["__CLASS"];
					if (string.IsNullOrEmpty(text))
					{
						text = CreatedClassName;
					}
				}
				return text;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Contents of the icon corresponding to the application")]
		public byte[] IconContents => (byte[])curObj["IconContents"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Path to the application icon")]
		public string IconPath => (string)curObj["IconPath"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Index of the icon")]
		public int IconIndex => (int)curObj["IconIndex"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Contents of the RDP File corresponding to the application")]
		public string RDPFileContents => (string)curObj["RDPFileContents"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Command Line Arguments setting")]
		public CommandLineSettingValues CommandLineSetting
		{
			get
			{
				if (curObj["CommandLineSetting"] == null)
				{
					return (CommandLineSettingValues)Convert.ToInt32(3);
				}
				return (CommandLineSettingValues)Convert.ToInt32(curObj["CommandLineSetting"], NumberFormatInfo.InvariantInfo);
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Name of the application")]
		public string Name => (string)curObj["Name"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Alias of the application")]
		public string Alias => (string)curObj["Alias"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Path to the application")]
		public string Path0 => (string)curObj["Path"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Whether this application should be shown in the TS Portal")]
		public bool ShowInPortal
		{
			get
			{
				if (curObj["ShowInPortal"] == null)
				{
					return Convert.ToBoolean(0);
				}
				return (bool)curObj["ShowInPortal"];
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Security Descriptor controlling access to the application, in SDDL Format. Empty string implies allow all access.")]
		public string SecurityDescriptor
		{
			get
			{
				//IL_0027: Expected O, but got Unknown
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Invalid comparison between Unknown and I4
				try
				{
					string text = (string)curObj["SecurityDescriptor"];
					if (string.IsNullOrEmpty(text))
					{
						return null;
					}
					return text;
				}
				catch (ManagementException val)
				{
					ManagementException val2 = val;
					if ((int)val2.ErrorCode == -2147217406)
					{
						return null;
					}
					throw val2;
				}
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Command Line Arguments required for this application")]
		public string RequiredCommandLine => (string)curObj["RequiredCommandLine"];

		public TSPublishedApplication(ManagementObject theObject)
		{
			Initialize();
			if (CheckIfProperClass((ManagementBaseObject)(object)theObject))
			{
				PrivateLateBoundObject = theObject;
				curObj = (ManagementBaseObject)(object)PrivateLateBoundObject;
				return;
			}
			throw new ArgumentException(LangSpecific.TSPublishExceptionMessage);
		}

		private bool CheckIfProperClass(ManagementBaseObject theObj)
		{
			if (theObj != null && string.Compare((string)theObj["__CLASS"], ManagementClassName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			Array array = (Array)theObj["__DERIVATION"];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Compare((string)array.GetValue(i), ManagementClassName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ResetIconPath()
		{
			curObj["IconPath"] = null;
			if (!isEmbedded && AutoCommitProp)
			{
				PrivateLateBoundObject.Put();
			}
		}

		private void ResetPath0()
		{
			curObj["Path"] = null;
			if (!isEmbedded && AutoCommitProp)
			{
				PrivateLateBoundObject.Put();
			}
		}

		private void Initialize()
		{
			AutoCommitProp = true;
			isEmbedded = false;
		}

		public static TSPublishedApplicationCollection GetInstances(ManagementScope mgmtScope, EnumerationOptions enumOptions)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			if (mgmtScope == null)
			{
				ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(null);
				mgmtScope = new ManagementScope("root\\cimv2\\TerminalServices", connectionOptions);
			}
			ManagementPath val = new ManagementPath();
			val.ClassName = "Win32_TSPublishedApplication";
			val.NamespacePath = "root\\cimv2\\TerminalServices";
			ManagementClass val2 = new ManagementClass(mgmtScope, val, (ObjectGetOptions)null);
			if (enumOptions == null)
			{
				enumOptions = new EnumerationOptions();
				enumOptions.EnsureLocatable = true;
			}
			return new TSPublishedApplicationCollection(val2.GetInstances(enumOptions));
		}
	}
	internal class TSPublishedDesktop : Component
	{
		public class TSPublishedDesktopCollection : ICollection, IEnumerable
		{
			public class TSPublishedDesktopEnumerator : IEnumerator
			{
				private ManagementObjectEnumerator privObjEnum;

				public virtual object Current => new TSPublishedDesktop((ManagementObject)privObjEnum.Current);

				public TSPublishedDesktopEnumerator(ManagementObjectEnumerator objEnum)
				{
					privObjEnum = objEnum;
				}

				public virtual bool MoveNext()
				{
					return privObjEnum.MoveNext();
				}

				public virtual void Reset()
				{
					privObjEnum.Reset();
				}
			}

			private ManagementObjectCollection privColObj;

			public virtual int Count => privColObj.Count;

			public virtual bool IsSynchronized => privColObj.IsSynchronized;

			public virtual object SyncRoot => this;

			public TSPublishedDesktopCollection(ManagementObjectCollection objCollection)
			{
				privColObj = objCollection;
			}

			public virtual void CopyTo(Array array, int index)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Expected O, but got Unknown
				if (array != null)
				{
					privColObj.CopyTo(array, index);
					for (int i = 0; i < array.Length; i++)
					{
						array.SetValue(new TSPublishedDesktop((ManagementObject)array.GetValue(i)), i);
					}
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				return new TSPublishedDesktopEnumerator(privColObj.GetEnumerator());
			}
		}

		private static string CreatedClassName = "Win32_TSRemoteDesktop";

		private ManagementObject PrivateLateBoundObject;

		private bool AutoCommitProp;

		private ManagementBaseObject curObj;

		private bool isEmbedded;

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ManagementClassName
		{
			get
			{
				string text = CreatedClassName;
				if (curObj != null && curObj.ClassPath != null)
				{
					text = (string)curObj["__CLASS"];
					if (string.IsNullOrEmpty(text))
					{
						text = CreatedClassName;
					}
				}
				return text;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Contents of the icon corresponding to the desktop")]
		public byte[] IconContents => (byte[])curObj["IconContents"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Path to the Desktop icon")]
		public string IconPath => (string)curObj["IconPath"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Contents of the RDP File corresponding to the desktop")]
		public string RDPFileContents => (string)curObj["RDPFileContents"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Name of the Desktop")]
		public string Name => (string)curObj["Name"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Alias of the Desktop")]
		public string Alias => (string)curObj["Alias"];

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Whether this Desktop should be shown in the TS Portal")]
		public bool ShowInPortal
		{
			get
			{
				if (curObj["ShowInPortal"] == null)
				{
					return Convert.ToBoolean(0);
				}
				return (bool)curObj["ShowInPortal"];
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Whether this Desktop is actually a VM Farm")]
		public bool IsVmFarm
		{
			get
			{
				object obj = null;
				try
				{
					obj = curObj["IsVmFarm"];
				}
				catch (ManagementException)
				{
				}
				if (obj == null)
				{
					return false;
				}
				return (bool)obj;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Security Descriptor controlling access to the application, in SDDL Format. Empty string implies allow all access.")]
		public string SecurityDescriptor
		{
			get
			{
				//IL_0019: Expected O, but got Unknown
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Invalid comparison between Unknown and I4
				string text;
				try
				{
					text = (string)curObj["SecurityDescriptor"];
				}
				catch (ManagementException val)
				{
					ManagementException val2 = val;
					if ((int)val2.ErrorCode == -2147217406)
					{
						return null;
					}
					throw val2;
				}
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				return text;
			}
		}

		public TSPublishedDesktop(ManagementObject theObject)
		{
			Initialize();
			if (CheckIfProperClass((ManagementBaseObject)(object)theObject))
			{
				PrivateLateBoundObject = theObject;
				curObj = (ManagementBaseObject)(object)PrivateLateBoundObject;
				return;
			}
			throw new ArgumentException(LangSpecific.TSPublishExceptionMessage);
		}

		private bool CheckIfProperClass(ManagementBaseObject theObj)
		{
			if (theObj == null)
			{
				return false;
			}
			if (string.Compare((string)theObj["__CLASS"], ManagementClassName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			Array array = (Array)theObj["__DERIVATION"];
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Compare((string)array.GetValue(i), ManagementClassName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ResetIconPath()
		{
			curObj["IconPath"] = null;
			if (!isEmbedded && AutoCommitProp)
			{
				PrivateLateBoundObject.Put();
			}
		}

		private void Initialize()
		{
			AutoCommitProp = true;
			isEmbedded = false;
		}

		public static TSPublishedDesktopCollection GetInstances(ManagementScope mgmtScope, EnumerationOptions enumOptions)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			if (mgmtScope == null)
			{
				ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(null);
				mgmtScope = new ManagementScope("root\\cimv2\\TerminalServices", connectionOptions);
			}
			ManagementPath val = new ManagementPath();
			val.ClassName = "Win32_TSRemoteDesktop";
			val.NamespacePath = "root\\cimv2\\TerminalServices";
			ManagementClass val2 = new ManagementClass(mgmtScope, val, (ObjectGetOptions)null);
			if (enumOptions == null)
			{
				enumOptions = new EnumerationOptions();
				enumOptions.EnsureLocatable = true;
			}
			return new TSPublishedDesktopCollection(val2.GetInstances(enumOptions));
		}
	}
	internal static class TSEventLog
	{
		private static string strEventLogSourceName = "RDWebAccess";

		private static EventLogTraceListener logListener;

		private static EventLog eventLog;

		internal static string EventLogSourceName
		{
			get
			{
				return strEventLogSourceName;
			}
			set
			{
				strEventLogSourceName = value;
			}
		}

		internal static void Event(string message, int id, EventLogEntryType etype)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (logListener == null || eventLog == null)
				{
					eventLog = new EventLog(EventLog.LogNameFromSourceName(EventLogSourceName, "."), ".", EventLogSourceName);
					if (eventLog == null)
					{
						return;
					}
					logListener = new EventLogTraceListener(eventLog);
				}
				if (logListener != null)
				{
					eventLog.WriteEntry(message, etype, id);
				}
			}
			catch (ArgumentException ex)
			{
				TraceWrite.TraceErrorNoContext("Unable to write {0} event to event log. Exception: {1} Event Message: {2}", etype, ex.Message, message);
			}
			catch (InvalidOperationException ex2)
			{
				TraceWrite.TraceErrorNoContext("Unable to write {0} event to event log. Exception: {1} Event Message: {2}", etype, ex2.Message, message);
			}
			catch (Win32Exception ex3)
			{
				TraceWrite.TraceErrorNoContext("Unable to write {0} event to event log. Exception: {1} Event Message: {2}", etype, ex3.Message, message);
			}
		}

		internal static void Event(string message, int id, EventLogEntryType etype, params object[] args)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (args != null)
			{
				Event(string.Format(CultureInfo.CurrentCulture, message, args), id, etype);
			}
			else
			{
				Event(message, id, etype);
			}
		}
	}
	public delegate void DoTraceMessage(TraceLevel level, string fmt, params object[] args);
	public delegate string DoTracePIIMessage(string inputString);
	public class TraceWrite
	{
		private static DoTraceMessage gm_DoTraceMessage;

		private static DoTracePIIMessage gm_DoTracePIIMessage;

		private static TraceLevel m_currentLevel;

		private string m_context;

		public string Context
		{
			get
			{
				return m_context;
			}
			set
			{
				m_context = value;
			}
		}

		static TraceWrite()
		{
			gm_DoTraceMessage = null;
			gm_DoTracePIIMessage = null;
			m_currentLevel = new TraceSwitch("TraceTSWA", "").Level;
			gm_DoTraceMessage = DefaultTraceMessageImpl;
			gm_DoTracePIIMessage = DefaultClearTextTraceMessageImpl;
		}

		public static void StaticInitialize(DoTraceMessage doTraceMessageDelegate, DoTracePIIMessage doTracePIIMessageDelegate)
		{
			gm_DoTraceMessage = doTraceMessageDelegate;
			gm_DoTracePIIMessage = doTracePIIMessageDelegate;
		}

		public void TraceError(string message, params object[] args)
		{
			TraceErrorNoContext(m_context + ":" + message, args);
		}

		public void TraceWarning(string message, params object[] args)
		{
			TraceWarningNoContext(m_context + ":" + message, args);
		}

		public void TraceInfo(string message, params object[] args)
		{
			TraceInfoNoContext(m_context + ":" + message, args);
		}

		public void TraceVerbose(string message, params object[] args)
		{
			TraceVerboseNoContext(m_context + ":" + message, args);
		}

		public static void TraceErrorNoContext(string message, params object[] args)
		{
			TraceMessage(TraceLevel.Error, message, args);
		}

		public static void TraceInfoNoContext(string message, params object[] args)
		{
			TraceMessage(TraceLevel.Info, message, args);
		}

		public static void TraceVerboseNoContext(string message, params object[] args)
		{
			TraceMessage(TraceLevel.Verbose, message, args);
		}

		public static void TraceWarningNoContext(string message, params object[] args)
		{
			TraceMessage(TraceLevel.Warning, message, args);
		}

		private static void TraceMessage(TraceLevel level, string fmt, params object[] args)
		{
			gm_DoTraceMessage(level, fmt, args);
		}

		public static string TracePII(string inputString)
		{
			return gm_DoTracePIIMessage(inputString);
		}

		private static void DefaultTraceMessageImpl(TraceLevel level, string fmt, params object[] args)
		{
			if (level <= m_currentLevel)
			{
				switch (level)
				{
				case TraceLevel.Error:
					Trace.TraceError(FormatMessageText(level, fmt, args));
					break;
				case TraceLevel.Warning:
					Trace.TraceWarning(FormatMessageText(level, fmt, args));
					break;
				default:
					Trace.TraceInformation(FormatMessageText(level, fmt, args));
					break;
				}
			}
		}

		private static string DefaultClearTextTraceMessageImpl(string inputString)
		{
			return inputString;
		}

		private static string FormatMessageText(TraceLevel level, string fmt, params object[] args)
		{
			string text = ((args.Length == 0) ? fmt : string.Format(CultureInfo.InvariantCulture, fmt, args));
			return string.Format(CultureInfo.InvariantCulture, "{0,1:yyyy/MM/dd HH:mm:ss} [{1}] {2} {3}", DateTime.Now, level.ToString(), Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture), text);
		}
	}
	public enum RdpType
	{
		Link,
		Inline,
		Both
	}
	public class WebFeed
	{
		private class FeedData
		{
			public Dictionary<string, ResourceXml> resource_list;

			public Dictionary<string, TerminalServerXml> ts_list;

			public List<string> folders;

			public bool supportsReconnect;

			public DateTime most_recent_update;
		}

		private string schema_name = "http://schemas.microsoft.com/ts/2007/05/tswf";

		private string Win7_schema_version = "1.1";

		private string Win8_schema_version = "2.1";

		internal const int default_icon_dim = 32;

		private WebFeedConfig feed_config;

		private string storage_location;

		private WorkspaceInfo m_wsInfo;

		private RdpType rdpRenderType;

		private bool m_onlyShowAvailableByDefaultResources;

		internal WebFeedConfig FeedConfig => feed_config;

		private string StorageLocation
		{
			get
			{
				if (storage_location == null)
				{
					storage_location = RequestHelper.GetRequestDirectoryPath(HttpContext.Current.Request);
					TraceWrite.TraceVerboseNoContext("ASP.NET location:" + storage_location);
				}
				return storage_location;
			}
		}

		public WebFeed(RdpType rdpRenderTypeArg, bool onlyShowAvailableByDefaultResources)
		{
			rdpRenderType = rdpRenderTypeArg;
			m_onlyShowAvailableByDefaultResources = onlyShowAvailableByDefaultResources;
			feed_config = new WebFeedConfig();
			if (feed_config == null)
			{
				TraceWrite.TraceErrorNoContext("Fail to create WebFeedConfig object.");
			}
		}

		public WorkspaceInfo GetFetchedWorkspaceInfo()
		{
			return m_wsInfo;
		}

		public async Task<Tuple<string, int>> GenerateFeedAsync(string userIdentity, FeedXmlVersion xmlVersion, string folderPath, bool writeXmlDecl)
		{
			TraceWrite.TraceInfoNoContext("Generating XML Feed compatible with schema version: " + xmlVersion);
			TraceWrite.TraceInfoNoContext("Calling GetDataForFeedAsync for " + userIdentity + " with folder " + folderPath);
			FeedData obj = await GetDataForFeedAsync(userIdentity, folderPath);
			Dictionary<string, ResourceXml> resource_list = obj.resource_list;
			Dictionary<string, TerminalServerXml> ts_list = obj.ts_list;
			List<string> folders = obj.folders;
			bool supportsReconnect = obj.supportsReconnect;
			DateTime most_recent_update = obj.most_recent_update;
			if (folderPath != null && folderPath.Length > 0 && !folderPath.Equals("/") && resource_list.Count == 0)
			{
				TraceWrite.TraceErrorNoContext("Requested folder does not exist: " + folderPath);
				throw new WorkspaceUnknownFolderException();
			}
			MemoryStream memoryStream = new MemoryStream();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.ConformanceLevel = ((!writeXmlDecl) ? ConformanceLevel.Fragment : ConformanceLevel.Document);
			xmlWriterSettings.OmitXmlDeclaration = !writeXmlDecl;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineOnAttributes = true;
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
			if (writeXmlDecl)
			{
				xmlWriter.WriteStartDocument();
			}
			xmlWriter.WriteStartElement("ResourceCollection", schema_name);
			xmlWriter.WriteStartAttribute("PubDate");
			xmlWriter.WriteValue(DateTime.UtcNow);
			xmlWriter.WriteEndAttribute();
			xmlWriter.WriteAttributeString("SchemaVersion", (xmlVersion == FeedXmlVersion.Win7) ? Win7_schema_version : Win8_schema_version);
			xmlWriter.WriteStartElement("Publisher");
			xmlWriter.WriteStartAttribute("LastUpdated");
			xmlWriter.WriteValue(most_recent_update.ToUniversalTime());
			xmlWriter.WriteEndAttribute();
			string value = (supportsReconnect ? "true" : "false");
			if (m_wsInfo != null)
			{
				string workspaceName = m_wsInfo.WorkspaceName;
				string workspaceId = m_wsInfo.WorkspaceId;
				string workspaceDescription = m_wsInfo.WorkspaceDescription;
				xmlWriter.WriteAttributeString("Name", workspaceName);
				xmlWriter.WriteAttributeString("ID", workspaceId);
				xmlWriter.WriteAttributeString("Description", workspaceDescription);
				if (xmlVersion != 0)
				{
					xmlWriter.WriteAttributeString("SupportsReconnect", value);
				}
				if (xmlVersion != 0 && folderPath != null && folderPath.Length > 0)
				{
					xmlWriter.WriteAttributeString("DisplayFolder", folderPath);
				}
				if (xmlVersion != 0 && folderPath != null && folderPath.Equals("/") && folders.Count > 0)
				{
					xmlWriter.WriteStartElement("SubFolders");
					foreach (string item in folders)
					{
						if (!item.Equals(folderPath))
						{
							new FolderXml(item).ToXML(xmlWriter, xmlVersion);
						}
					}
					xmlWriter.WriteEndElement();
				}
				xmlWriter.WriteStartElement("Resources");
				foreach (ResourceXml value2 in resource_list.Values)
				{
					value2.ToXML(xmlWriter, xmlVersion);
				}
				xmlWriter.WriteEndElement();
				int count = resource_list.Count;
				xmlWriter.WriteStartElement("TerminalServers");
				foreach (TerminalServerXml value3 in ts_list.Values)
				{
					value3.ToXML(xmlWriter, xmlVersion);
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				if (writeXmlDecl)
				{
					xmlWriter.WriteEndDocument();
				}
				xmlWriter.Flush();
				xmlWriter.Close();
				return new Tuple<string, int>(Encoding.UTF8.GetString(memoryStream.ToArray()), count);
			}
			TraceWrite.TraceErrorNoContext("Could not retrieve workspace info from any source");
			throw new WorkspaceUnavailableException(LangSpecific.TSPORTAL_FEED_NO_WORKSPACE_INFO_ERROR_TEXT);
		}

		private async Task<FeedData> GetDataForFeedAsync(string userIdentity, string folderName)
		{
			FeedData feedData = new FeedData
			{
				resource_list = new Dictionary<string, ResourceXml>(),
				ts_list = new Dictionary<string, TerminalServerXml>(1),
				folders = new List<string>(),
				supportsReconnect = false
			};
			bool pollAppFreshness = true;
			if (feed_config == null)
			{
				feedData.most_recent_update = DateTime.UtcNow;
				return feedData;
			}
			_ = DateTime.MinValue;
			RapWebService rap_ws = new RapWebService(pollAppFreshness);
			m_wsInfo = rap_ws.GetWorkspaceInfo();
			IEnumerable<AppInfo> enumerable = await rap_ws.GetRemoteAppsAsync(userIdentity, m_onlyShowAvailableByDefaultResources, (rdpRenderType != 0) ? true : false);
			foreach (AppInfo item in enumerable)
			{
				TraceWrite.TraceInfoNoContext("Received app: {0}", item.Name);
			}
			DateTime dateTime = ProcessApps(enumerable, folderName, ref feedData.resource_list, ref feedData.ts_list, ref feedData.folders);
			if (dateTime == DateTime.MinValue)
			{
				dateTime = DateTime.UtcNow;
			}
			feedData.supportsReconnect = rap_ws.SupportsWorkspaceReconnect();
			feedData.most_recent_update = dateTime;
			return feedData;
		}

		private static TerminalServerXml BuildTerminalServer(string ts_name)
		{
			string text = ts_name;
			if (string.Compare(ts_name, "localhost", StringComparison.OrdinalIgnoreCase) == 0)
			{
				try
				{
					text = Environment.MachineName;
				}
				catch (InvalidOperationException ex)
				{
					TraceWrite.TraceErrorNoContext("Could not determine actual machine name for localhost: " + ex.Message);
				}
			}
			return new TerminalServerXml(text, text);
		}

		internal DateTime ProcessApps(IEnumerable<AppInfo> apps, string folderName, ref Dictionary<string, ResourceXml> resource_list, ref Dictionary<string, TerminalServerXml> ts_list, ref List<string> folders)
		{
			DateTime dateTime = DateTime.MinValue;
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (AppInfo app in apps)
			{
				string text = GenerateResourceID(app);
				ResourceXml value;
				if (resource_list.ContainsKey(text))
				{
					resource_list.TryGetValue(text, out value);
					if (value.Alias != app.RemoteAppXml.ApplicationAlias)
					{
						TraceWrite.TraceErrorNoContext("Resources with ID " + text + " had different aliases: " + value.Alias + ", " + app.RemoteAppXml.ApplicationAlias + ".");
					}
					if (value.Title != app.RemoteAppXml.ApplicationName)
					{
						TraceWrite.TraceErrorNoContext("Resources with ID " + text + " had different titles: '" + value.Title + "', '" + app.RemoteAppXml.ApplicationName + "'.");
					}
					if (value.CommandLine != app.RemoteAppXml.RequiredCommandLine)
					{
						TraceWrite.TraceErrorNoContext("Resources with ID " + text + " had different command lines: '" + value.CommandLine + "', '" + app.RemoteAppXml.RequiredCommandLine + "'.");
					}
					if (value.ExecutableName != GetExecutableName(app.RemoteAppXml.ApplicationPath))
					{
						TraceWrite.TraceErrorNoContext("Resources with ID " + text + " had different executable names: '" + value.ExecutableName + "', '" + GetExecutableName(app.RemoteAppXml.ApplicationPath) + "'.");
					}
				}
				else
				{
					value = new ResourceXml(text, app.RemoteAppXml.ApplicationAlias, app.RemoteAppXml.ApplicationName, ComputeResourceType(app), app.RemoteAppXml.RequiredCommandLine, GetExecutableName(app.RemoteAppXml.ApplicationPath), app.AvailableToUsersByDefault);
					GetIconData(app, ref value);
					GetFileExtensionData(app, ref value);
					GetFolderData(app, ref value);
					if (folderName == null || folderName.Length == 0 || (folderName.Equals("/") && app.RemoteAppXml.FolderNames.Count == 0) || app.RemoteAppXml.FolderNames.Contains(folderName))
					{
						resource_list.Add(text, value);
					}
					hashSet.UnionWith(app.RemoteAppXml.FolderNames);
				}
				TerminalServerXml terminalServerXml = null;
				if (ts_list.ContainsKey(app.TSName))
				{
					terminalServerXml = ts_list[app.TSName];
				}
				else
				{
					terminalServerXml = BuildTerminalServer(app.TSName);
					ts_list.Add(app.TSName, terminalServerXml);
				}
				if (value.LastUpdated < app.Timestamp)
				{
					value.LastUpdated = app.Timestamp;
				}
				if (terminalServerXml.LastUpdated < app.Timestamp)
				{
					terminalServerXml.LastUpdated = app.Timestamp;
				}
				if (dateTime < app.Timestamp)
				{
					dateTime = app.Timestamp;
				}
				HostingTerminalServerXml ts = new HostingTerminalServerXml(terminalServerXml, GetRdpFileData(app));
				value.AddHostingTerminalServer(ts);
			}
			if (dateTime == DateTime.MinValue)
			{
				dateTime = DateTime.UtcNow;
			}
			folders.AddRange(hashSet);
			folders.Sort();
			return dateTime;
		}

		internal static string GenerateResourceID(AppInfo app)
		{
			string s = app.RemoteAppXml.ApplicationAlias + app.RemoteAppXml.ApplicationName + app.RemoteAppXml.RequiredCommandLine;
			HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider();
			byte[] array = null;
			try
			{
				array = hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(s));
			}
			catch (TargetInvocationException ex)
			{
				TraceWrite.TraceErrorNoContext("MD5 hash couldn't be created: " + ex.Message);
			}
			catch (ArgumentNullException ex2)
			{
				TraceWrite.TraceErrorNoContext("MD5 hash couldn't be created: " + ex2.Message);
			}
			catch (ObjectDisposedException ex3)
			{
				TraceWrite.TraceErrorNoContext("MD5 hash couldn't be created: " + ex3.Message);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static string GetExecutableName(string path)
		{
			return Path.GetFileName(path);
		}

		private static string ComputeResourceType(AppInfo app)
		{
			if (app.IsRemoteDesktop)
			{
				return "Desktop";
			}
			return "RemoteApp";
		}

		private void GetIconData(AppInfo app, ref ResourceXml resource)
		{
			ImageFormat formatObject = feed_config.IconSettings.FormatObject;
			string filetype_in = feed_config.IconSettings.Format.ToString();
			IconInfo iconInfo = null;
			int[] array;
			if (((ConfigurationElementCollection)feed_config.IconSettings.IconDimensions).Count == 0)
			{
				array = new int[1] { 32 };
			}
			else
			{
				array = new int[((ConfigurationElementCollection)feed_config.IconSettings.IconDimensions).Count];
				for (int i = 0; i < ((ConfigurationElementCollection)feed_config.IconSettings.IconDimensions).Count; i++)
				{
					array[i] = feed_config.IconSettings.IconDimensions[i].Dim;
				}
			}
			iconInfo = app.RemoteAppXml.Icon;
			switch (iconInfo.IconInfoType)
			{
			case IconInfoType.Contents:
				if (iconInfo.type == "binary")
				{
					string iconFile = GetIconFile(iconInfo.iconBytes(), iconInfo.name, 32, 32, ImageFormat.Icon, StorageLocation, app.Timestamp, null);
					resource.AddIcon(new IconXml(iconFile, 32, "Ico", is_raw_in: true, iconInfo.IconETag));
					int[] array2 = array;
					foreach (int num2 in array2)
					{
						string eTag_in2 = ((iconInfo.IconPngETags != null && iconInfo.IconPngETags.ContainsKey(num2)) ? iconInfo.IconPngETags[num2] : string.Empty);
						iconFile = GetIconFile(iconInfo.iconBytes(), iconInfo.name, num2, num2, formatObject, StorageLocation, app.Timestamp, null);
						resource.AddIcon(new IconXml(iconFile, num2, filetype_in, is_raw_in: false, eTag_in2));
					}
				}
				else
				{
					TraceWrite.TraceErrorNoContext("Error processing Icon information for " + resource.Title);
				}
				break;
			case IconInfoType.URI:
			{
				resource.AddIcon(new IconXml(iconInfo.IconUri, 32, "Ico", is_raw_in: true, iconInfo.IconETag));
				int[] array2 = array;
				foreach (int num in array2)
				{
					if (iconInfo.IconPngUris.ContainsKey(num))
					{
						string eTag_in = ((iconInfo.IconPngETags != null && iconInfo.IconPngETags.ContainsKey(num)) ? iconInfo.IconPngETags[num] : string.Empty);
						resource.AddIcon(new IconXml(iconInfo.IconPngUris[num], num, filetype_in, is_raw_in: false, eTag_in));
					}
				}
				break;
			}
			}
		}

		private void GetFileExtensionData(AppInfo app, ref ResourceXml resource)
		{
			FileExtensionInfo[] extensions = app.RemoteAppXml.Extensions;
			for (int i = 0; i < extensions.Length; i++)
			{
				FileExtensionInfo fileExtensionInfo = extensions[i];
				FileExtensionXml fileExtensionXml = new FileExtensionXml(fileExtensionInfo.Name, fileExtensionInfo.PrimaryHandler);
				if (fileExtensionInfo.AssocIcon != null)
				{
					if (fileExtensionInfo.AssocIcon.type != "binary")
					{
						TraceWrite.TraceErrorNoContext("Error processing Icon information for " + resource.Title + " file association with ext " + fileExtensionInfo.Name + ". Skipping icon...");
						continue;
					}
					string iconFile = GetIconFile(fileExtensionInfo.AssocIcon.iconBytes(), app.RemoteAppXml.ApplicationAlias, 32, 32, ImageFormat.Icon, StorageLocation, app.Timestamp, fileExtensionInfo.Name);
					fileExtensionXml.AddIcon(new IconXml(iconFile, 32, "Ico", is_raw_in: true, string.Empty));
				}
				resource.AddFileExtension(fileExtensionXml);
			}
		}

		private void GetFolderData(AppInfo app, ref ResourceXml resource)
		{
			foreach (string folderName in app.RemoteAppXml.FolderNames)
			{
				resource.AddFolder(new FolderXml(folderName));
			}
		}

		private RdpFileXml GetRdpFileData(AppInfo app)
		{
			if (rdpRenderType != RdpType.Inline && string.IsNullOrEmpty(app.RemoteAppXml.RdpFileInfo.RdpFileName))
			{
				TraceWrite.TraceErrorNoContext("The linked rdp files option was selected, but no rdp file name was created");
			}
			string empty = string.Empty;
			return new RdpFileXml(url_in: (!Uri.IsWellFormedUriString(app.RemoteAppXml.RdpFileInfo.RdpFileName, UriKind.Absolute)) ? (StorageLocation + "rdp/" + Uri.EscapeDataString(app.RemoteAppXml.RdpFileInfo.RdpFileName)) : app.RemoteAppXml.RdpFileInfo.RdpFileName, contents_in: app.RemoteAppXml.RdpFileInfo.RdpFileContents, rdp_type_in: rdpRenderType, file_Extension: app.FileExtension, eTag_in: app.RemoteAppXml.RdpFileInfo.RdpETag);
		}

		private static string JScriptEncodeRdp(string rdpContents)
		{
			return rdpContents.Replace("\\", "\\\\").Replace("\r\n", "\\n\" + \r\n\t\t\t\"");
		}

		public static string GetIconFile(byte[] icon, string appName, int height, int width, ImageFormat format, string storageLocation, DateTime appTimeStamp, string fileAssocExt)
		{
			string text = "";
			bool flag = true;
			string text2 = "";
			string text3;
			if (format == ImageFormat.Png)
			{
				text3 = ".png";
			}
			else if (format == ImageFormat.Icon)
			{
				text3 = ".ico";
			}
			else
			{
				TraceWrite.TraceErrorNoContext("Unsupported icon format: " + ((object)format).ToString() + "  Defaulting to .png");
				text3 = ".png";
				format = ImageFormat.Png;
			}
			if (format == ImageFormat.Png)
			{
				text2 = "_" + height.ToString(CultureInfo.InvariantCulture) + "x" + width.ToString(CultureInfo.InvariantCulture);
			}
			if (icon != null && icon.Length != 0 && !string.IsNullOrEmpty(appName))
			{
				text = ((fileAssocExt != null) ? (storageLocation + "rdp/fileassoc/" + fileAssocExt + "/" + Uri.EscapeDataString(appName) + text2 + text3) : (storageLocation + "rdp/" + Uri.EscapeDataString(appName) + text2 + text3));
				flag = false;
			}
			if (flag)
			{
				TraceWrite.TraceInfoNoContext("Could not generate icon file, creating default instead");
				text = storageLocation + "rdp/mstsc256" + text2 + text3;
			}
			if (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
			{
				text += HttpContext.Current.Request.Url.Query;
			}
			return text;
		}
	}
	internal class ResourceXml
	{
		private string id;

		private string alias;

		private string title;

		private string resource_type;

		private string cmd_line;

		private string exe_name;

		private bool show_by_default;

		private DateTime last_updated;

		private List<IconXml> icons;

		private List<FileExtensionXml> file_extensions;

		private List<FolderXml> folders;

		private List<HostingTerminalServerXml> servers;

		internal string ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		internal string Alias => alias;

		internal string Title => title;

		internal string CommandLine => cmd_line;

		internal string ExecutableName => exe_name;

		internal DateTime LastUpdated
		{
			get
			{
				return last_updated;
			}
			set
			{
				last_updated = value;
			}
		}

		internal List<HostingTerminalServerXml> HostingTerminalServers => servers;

		internal ResourceXml(string id_in, string alias_in, string title_in, string resource_type_in, string cmd_line_in, string executable_name_in, bool show_bydefault_in)
		{
			id = id_in;
			alias = alias_in;
			title = title_in;
			resource_type = resource_type_in;
			cmd_line = cmd_line_in;
			exe_name = executable_name_in;
			show_by_default = show_bydefault_in;
			icons = new List<IconXml>();
			file_extensions = new List<FileExtensionXml>();
			folders = new List<FolderXml>();
			servers = new List<HostingTerminalServerXml>();
			last_updated = DateTime.MinValue;
		}

		internal void AddIcon(IconXml icon)
		{
			icons.Add(icon);
		}

		internal void AddFileExtension(FileExtensionXml ext)
		{
			file_extensions.Add(ext);
		}

		internal void AddFolder(FolderXml folder)
		{
			folders.Add(folder);
		}

		internal void AddHostingTerminalServer(HostingTerminalServerXml ts)
		{
			servers.Add(ts);
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("Resource");
			writer.WriteAttributeString("ID", id);
			writer.WriteAttributeString("Alias", alias);
			writer.WriteAttributeString("Title", title);
			writer.WriteStartAttribute("LastUpdated");
			writer.WriteValue(last_updated.ToUniversalTime());
			writer.WriteEndAttribute();
			writer.WriteAttributeString("Type", resource_type);
			if (!string.IsNullOrEmpty(cmd_line))
			{
				writer.WriteAttributeString("RequiredCommandLine", cmd_line);
			}
			if (!string.IsNullOrEmpty(exe_name))
			{
				writer.WriteAttributeString("ExecutableName", exe_name);
			}
			if (xmlVersion != 0)
			{
				writer.WriteAttributeString("ShowByDefault", show_by_default.ToString().ToLowerInvariant());
			}
			writer.WriteStartElement("Icons");
			foreach (IconXml icon in icons)
			{
				icon.ToXML(writer, xmlVersion);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("FileExtensions");
			foreach (FileExtensionXml file_extension in file_extensions)
			{
				file_extension.ToXML(writer, xmlVersion);
			}
			writer.WriteEndElement();
			if (xmlVersion != 0 && folders.Count > 0)
			{
				writer.WriteStartElement("Folders");
				foreach (FolderXml folder in folders)
				{
					folder.ToXML(writer, xmlVersion);
				}
				writer.WriteEndElement();
			}
			writer.WriteStartElement("HostingTerminalServers");
			foreach (HostingTerminalServerXml server in servers)
			{
				server.ToXML(writer, xmlVersion);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
	internal class IconXml
	{
		private string data;

		private int dim;

		private string filetype;

		private bool is_raw;

		private string eTag;

		internal IconXml(string data_in, int dim_in, string filetype_in)
			: this(data_in, dim_in, filetype_in, is_raw_in: false, string.Empty)
		{
		}

		internal IconXml(string data_in, int dim_in, string filetype_in, bool is_raw_in, string eTag_in)
		{
			data = data_in;
			dim = dim_in;
			filetype = filetype_in;
			is_raw = is_raw_in;
			eTag = eTag_in;
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			string text = dim.ToString(CultureInfo.InvariantCulture);
			if (is_raw)
			{
				writer.WriteStartElement("IconRaw");
			}
			else
			{
				writer.WriteStartElement("Icon" + text);
				writer.WriteAttributeString("Dimensions", text + "x" + text);
			}
			writer.WriteAttributeString("FileType", filetype);
			writer.WriteAttributeString("FileURL", data);
			if (!string.IsNullOrEmpty(eTag))
			{
				writer.WriteAttributeString("ETag", eTag);
			}
			writer.WriteEndElement();
		}
	}
	internal class HostingTerminalServerXml
	{
		private TerminalServerXml ts;

		private RdpFileXml rdp;

		internal TerminalServerXml LinkedTerminalServer => ts;

		internal HostingTerminalServerXml(TerminalServerXml linked_ts, RdpFileXml rdp_in)
		{
			ts = linked_ts;
			rdp = rdp_in;
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("HostingTerminalServer");
			rdp.ToXML(writer, xmlVersion);
			writer.WriteStartElement("TerminalServerRef");
			writer.WriteAttributeString("Ref", ts.ID);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
	internal class RdpFileXml
	{
		private string contents;

		private string url;

		private RdpType rdp_type;

		private string fileExtension;

		private string eTag;

		internal RdpFileXml(string contents_in, string url_in, RdpType rdp_type_in, string file_Extension, string eTag_in)
		{
			contents = contents_in;
			url = url_in;
			rdp_type = rdp_type_in;
			if (string.IsNullOrEmpty(file_Extension))
			{
				fileExtension = ".rdp";
			}
			else
			{
				fileExtension = file_Extension;
			}
			eTag = eTag_in;
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("ResourceFile");
			writer.WriteAttributeString("FileExtension", fileExtension);
			if (rdp_type != RdpType.Inline)
			{
				writer.WriteAttributeString("URL", url);
			}
			if (rdp_type != 0)
			{
				writer.WriteStartElement("Content");
				try
				{
					writer.WriteCData(contents);
				}
				catch (ArgumentException ex)
				{
					TraceWrite.TraceErrorNoContext("Cannot write inline rdp file to xml (would result in invalid xml): " + ex.Message);
				}
				writer.WriteEndElement();
			}
			if (!string.IsNullOrEmpty(eTag))
			{
				writer.WriteAttributeString("ETag", eTag);
			}
			writer.WriteEndElement();
		}
	}
	internal class FileExtensionXml
	{
		private string extName;

		private bool primaryHandler;

		private List<IconXml> icons;

		internal FileExtensionXml(string extName_in, bool primaryHandler_in)
		{
			extName = extName_in;
			primaryHandler = primaryHandler_in;
			icons = new List<IconXml>();
		}

		internal void AddIcon(IconXml icon)
		{
			icons.Add(icon);
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("FileExtension");
			writer.WriteAttributeString("Name", extName);
			if (xmlVersion != 0)
			{
				writer.WriteAttributeString("PrimaryHandler", primaryHandler.ToString());
				if (icons.Count > 0)
				{
					writer.WriteStartElement("FileAssociationIcons");
					foreach (IconXml icon in icons)
					{
						icon.ToXML(writer, xmlVersion);
					}
					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement();
		}
	}
	internal class FolderXml
	{
		private string folderName;

		internal FolderXml(string folderName_in)
		{
			folderName = folderName_in;
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("Folder");
			writer.WriteAttributeString("Name", folderName);
			writer.WriteEndElement();
		}
	}
	internal class TerminalServerXml
	{
		private string id;

		private string name;

		private DateTime last_updated;

		internal string ID => id;

		internal DateTime LastUpdated
		{
			get
			{
				return last_updated;
			}
			set
			{
				last_updated = value;
			}
		}

		internal TerminalServerXml(string id_in, string name_in)
		{
			id = id_in;
			name = name_in;
			last_updated = DateTime.MinValue;
		}

		internal void ToXML(XmlWriter writer, FeedXmlVersion xmlVersion)
		{
			writer.WriteStartElement("TerminalServer");
			writer.WriteAttributeString("ID", id);
			writer.WriteAttributeString("Name", name);
			writer.WriteStartAttribute("LastUpdated");
			writer.WriteValue(last_updated.ToUniversalTime());
			writer.WriteEndAttribute();
			writer.WriteEndElement();
		}
	}
	internal class cacheItem
	{
		internal ulong lastUpdateTime;

		internal AppInfo[] apps;

		internal AppInfo[] desktops;
	}
	internal class UserFilteredAppCacheItem
	{
		internal AppInfo[] appsListFiltered;

		internal AppInfo[] desktopsListFiltered;

		internal string cpubName;
	}
	internal class WorkspaceCacheItem
	{
		internal ulong lastUpdateTime;

		internal WorkspaceInfo Info;
	}
	internal class RWSCPUBAccessor : IStoreAccessor
	{
		private class NativeMethods
		{
			internal struct CERTIFICATE_HASH
			{
				public IntPtr CertificateHashPtr;

				public int CertificateHashSize;
			}

			private NativeMethods()
			{
			}

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getLastCacheUpdateTime(ref ulong lastUpdateTime, string pszNetworkAddress, string pszEndpoint);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint initializeResources(ref IntPtr context, string userID, string pszNetworkAddress, string pszEndpoint, int flags, int filterFlags, string providerFilter, string szFilter, ref int numResources, ref ulong lastUpdateTime);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getResource(IntPtr context, uint idx, StringBuilder alias, int ceAlias, StringBuilder name, int ceName, StringBuilder resourceFile, int ceResourceFile, StringBuilder fileExtension, int ceFileExtension, StringBuilder pluginType, int cePluginType, [MarshalAs(UnmanagedType.Bool)] ref bool isDiscoverable, ref uint pceIconSize, byte[] iconContents, ref uint pcePluginBlobsize, byte[] blobContents, ref int resourceType, ref uint numFileAssociations, ref uint numFolders);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getResourceFileAssociation(IntPtr context, uint resourceIdx, uint assocIdx, StringBuilder extName, int ceExtName, [MarshalAs(UnmanagedType.Bool)] ref bool primaryHandler, ref uint pceIconSize, byte[] iconContents);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getResourceFolder(IntPtr context, uint resourceIdx, uint folderIdx, StringBuilder folderName, int ceFolderName);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getResourceSecurityDescriptor(IntPtr context, uint resourceIdx, int ceSecurityDescriptor, StringBuilder securityDescriptor);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern void freeResources(ref IntPtr context);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getWorkspaceInfo(StringBuilder workspaceID, int ceWorkspaceID, StringBuilder workspaceName, int ceWorkspaceName, StringBuilder redirectorName, int ceRedirectorName, string pszNetworkAddress, string pszEndpoint);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern uint getRDPSigningCertificates(ref IntPtr CertificateHashes, ref uint NumberOfCertificateHashes, string pszNetworkAddress, string pszEndpoint);

			[DllImport("tscpubStub.dll", CharSet = CharSet.Unicode)]
			internal static extern void freeRDPSigningCertificates(IntPtr CertificateHashes, uint NumberOfCertificateHashes);
		}

		private static int gm_stateLockTimeout = -1;

		private static ReaderWriterLock gm_stateLock = new ReaderWriterLock();

		private const uint m_AccessDeniedHResult = 2147942405u;

		private const uint m_InsufficientBufferHResult = 2147942522u;

		private const uint m_BadPathHResult = 2147942561u;

		private const uint m_RpcUnknownIfHResult = 2147944117u;

		private static DateTime gm_RdpCertificateLastQueryTime = default(DateTime);

		private static DateTime gm_WorkspaceLastQueryTime = default(DateTime);

		private static string gm_HexStringFormat = "x";

		private string m_TSName = "";

		private bool m_PollAppFreshness = true;

		private static bool gm_filteringDisabled = false;

		private AppFilterer m_filterer;

		private IconFactory m_iconFactory;

		private const int MAX_ALIAS = 193;

		private const int MAX_NAME = 80;

		private const int MAX_SMALL = 40;

		private const int INIT_SECURITYDESCRIPTOR_LENGTH = 1024;

		private const int MAX_RESOURCE = 131072;

		private const int MAX_ICON = 3072000;

		private const int MAX_FOLDER_NAME = 256;

		private const int MAX_EXTENSION = 256;

		private const int RESOURCE_TYPE_APPLICATION = 1;

		private const int RESOURCE_TYPE_DESKTOP = 2;

		private const string m_RdpFilesDirectory = "rdp";

		private const string m_CacheAppsPrefix = "CPubApps:";

		private static long gm_WksRdpCertLastUpdateFetchInMinutes = 3L;

		private const string m_WorkspaceCache = "TsWorkspaceCache";

		private const string m_RdpSignCertificateCache = "TsRdpSignCertificateCache";

		private const string m_UserFilteredAppCachePrefix = "UserFilteredCPubApps:";

		private const string m_CacheIconsPrefix = "CPubIcons:";

		private TraceWrite m_TraceWrite = new TraceWrite();

		private static long gm_CacheExpirationInMinutes = 120L;

		private const long m_DefaultCacheExpirationInMinutes = 120L;

		private string m_cpub_rpcport = string.Empty;

		public bool SupportsWorkspaceReconnect => true;

		public RWSCPUBAccessor(string tsName, bool PollAppFreshness)
		{
			m_TSName = tsName;
			m_PollAppFreshness = PollAppFreshness;
			try
			{
				gm_stateLock.AcquireWriterLock(gm_stateLockTimeout);
				if (string.Compare(m_TSName, "localhost", StringComparison.OrdinalIgnoreCase) == 0)
				{
					m_TSName = Environment.MachineName;
				}
				TraceWrite.TraceInfoNoContext("Connecting to Centralized Publishing Server: " + m_TSName);
				m_cpub_rpcport = ConfigurationManager.AppSettings["DefaultCentralPublishingPort"];
				if (string.IsNullOrEmpty(m_cpub_rpcport))
				{
					m_cpub_rpcport = "5504";
				}
				string text = "";
				text = ConfigurationManager.AppSettings["PointsToCPubDisableAppFiltering"];
				if (!string.IsNullOrEmpty(text) && text == "true")
				{
					gm_filteringDisabled = true;
				}
				string text2 = "";
				text2 = ConfigurationManager.AppSettings["CacheTimeToLiveInMinutes"];
				if (string.IsNullOrEmpty(text2))
				{
					gm_CacheExpirationInMinutes = 120L;
				}
				else
				{
					try
					{
						gm_CacheExpirationInMinutes = Convert.ToInt32(text2, NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException ex)
					{
						TraceWrite.TraceVerboseNoContext("Cannot convert Cache TTL \"" + text2 + "\" to an int, Using default. " + ex.Message);
						gm_CacheExpirationInMinutes = 120L;
					}
					catch (OverflowException ex2)
					{
						TraceWrite.TraceVerboseNoContext("Cannot convert Cache TTL \"" + text2 + "\" to an int, Using default. " + ex2.Message);
						gm_CacheExpirationInMinutes = 120L;
					}
				}
				TraceWrite.TraceVerboseNoContext("Cache TTL is " + gm_CacheExpirationInMinutes.ToString(CultureInfo.InvariantCulture) + " minutes");
				m_iconFactory = new IconFactory("CPubIcons:", gm_CacheExpirationInMinutes);
			}
			finally
			{
				gm_stateLock.ReleaseWriterLock();
			}
			if (m_filterer == null)
			{
				m_filterer = new AppFilterer();
			}
			RemoveAppsForServersNotInListFromCache();
		}

		public string GetClaimsHint()
		{
			return "";
		}

		public Task<string> GetPrivacyLinkAsync()
		{
			return Task.FromResult(string.Empty);
		}

		public Task<List<AccountDetailAndConsentStatus>> GetTenantAccountsAndUserConsentStatusAsync(object securityPrincipal)
		{
			return Task.FromResult(new List<AccountDetailAndConsentStatus>());
		}

		private ulong GetLastUpdateTime()
		{
			ulong lastUpdateTime = 0uL;
			uint lastCacheUpdateTime = NativeMethods.getLastCacheUpdateTime(ref lastUpdateTime, m_TSName, m_cpub_rpcport);
			if (-2147024891 == (int)lastCacheUpdateTime)
			{
				TSEventLog.Event(args: new object[1] { m_TSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get last cache update time: Access Denied.");
			}
			else
			{
				if (-2147023179 == (int)lastCacheUpdateTime)
				{
					TSEventLog.Event(args: new object[2] { m_TSName, lastCacheUpdateTime }, message: LangSpecific.TSPORTAL_ERROR_15, id: 15, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get last cache update time: Server " + m_TSName + " is not running a supported CPub service (RPC interface mismatch). 0x" + lastCacheUpdateTime.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
					throw new UnsupportedSourceServerException();
				}
				if (lastCacheUpdateTime != 0)
				{
					TSEventLog.Event(args: new object[2] { m_TSName, lastCacheUpdateTime }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get last cache update time: 0x" + lastCacheUpdateTime.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				}
			}
			return lastUpdateTime;
		}

		private void PutRdpSigningCertificateInfoInCache(ref RdpSigningCertificate objRdpSigningCertificate, ulong lastUpdateTime)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			m_TraceWrite.TraceVerbose("Putting Rdp Signing Certificate into the Cache");
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			RdpSigningCertificateCacheItem rdpSigningCertificateCacheItem = new RdpSigningCertificateCacheItem();
			rdpSigningCertificateCacheItem.objRdpSigningCertificate = objRdpSigningCertificate;
			rdpSigningCertificateCacheItem.lastUpdateTime = lastUpdateTime;
			HttpRuntime.Cache.Insert("TsRdpSignCertificateCache" + tSName, (object)rdpSigningCertificateCacheItem, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
		}

		private RdpSigningCertificate GetFreshRdpSigningCertificate()
		{
			RdpSigningCertificate objRdpSigningCertificate = new RdpSigningCertificate();
			objRdpSigningCertificate.wsHash = "";
			IntPtr CertificateHashes = IntPtr.Zero;
			uint NumberOfCertificateHashes = 0u;
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			uint rDPSigningCertificates = NativeMethods.getRDPSigningCertificates(ref CertificateHashes, ref NumberOfCertificateHashes, tSName, m_cpub_rpcport);
			if (-2147024891 == (int)rDPSigningCertificates)
			{
				TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get RDP Signing Certificates: Access Denied.");
			}
			else
			{
				if (-2147023179 == (int)rDPSigningCertificates)
				{
					TSEventLog.Event(args: new object[2] { tSName, rDPSigningCertificates }, message: LangSpecific.TSPORTAL_ERROR_15, id: 15, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get RDP Signing Certificates: Server " + tSName + " is not running a supported CPub service (RPC interface mismatch). 0x" + rDPSigningCertificates.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
					throw new UnsupportedSourceServerException();
				}
				if (rDPSigningCertificates != 0)
				{
					TSEventLog.Event(args: new object[2] { tSName, rDPSigningCertificates }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get RDP Signing Certificates: 0x" + rDPSigningCertificates.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				}
				else
				{
					if (NumberOfCertificateHashes != 0)
					{
						IntPtr intPtr = CertificateHashes;
						for (uint num = 0u; num < NumberOfCertificateHashes; num++)
						{
							NativeMethods.CERTIFICATE_HASH cERTIFICATE_HASH = (NativeMethods.CERTIFICATE_HASH)Marshal.PtrToStructure(intPtr, typeof(NativeMethods.CERTIFICATE_HASH));
							if (cERTIFICATE_HASH.CertificateHashSize > 0)
							{
								byte[] array = new byte[cERTIFICATE_HASH.CertificateHashSize];
								Marshal.Copy(cERTIFICATE_HASH.CertificateHashPtr, array, 0, cERTIFICATE_HASH.CertificateHashSize);
								if (objRdpSigningCertificate.wsHash == "")
								{
									objRdpSigningCertificate.wsHash = RdpSigningCertificate.GetHashStringFromByteArray(array);
								}
								else
								{
									RdpSigningCertificate rdpSigningCertificate = objRdpSigningCertificate;
									rdpSigningCertificate.wsHash = rdpSigningCertificate.wsHash + ";" + RdpSigningCertificate.GetHashStringFromByteArray(array);
								}
							}
							intPtr = IntPtr.Add(intPtr, Marshal.SizeOf((object)cERTIFICATE_HASH));
						}
						ulong lastUpdateTime = GetLastUpdateTime();
						gm_RdpCertificateLastQueryTime = DateTime.Now;
						PutRdpSigningCertificateInfoInCache(ref objRdpSigningCertificate, lastUpdateTime);
					}
					NativeMethods.freeRDPSigningCertificates(CertificateHashes, NumberOfCertificateHashes);
				}
			}
			return objRdpSigningCertificate;
		}

		private object GetRdpSigningCertificateInfoFromCache()
		{
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			return HttpRuntime.Cache["TsRdpSignCertificateCache" + tSName];
		}

		public RdpSigningCertificate GetRdpSigningCertificate()
		{
			RdpSigningCertificate result = null;
			if (!HttpContext.Current.Response.IsClientConnected)
			{
				TraceWrite.TraceInfoNoContext("Returning early, client not connected");
			}
			else
			{
				object rdpSigningCertificateInfoFromCache = GetRdpSigningCertificateInfoFromCache();
				if (rdpSigningCertificateInfoFromCache != null)
				{
					RdpSigningCertificateCacheItem rdpSigningCertificateCacheItem = rdpSigningCertificateInfoFromCache as RdpSigningCertificateCacheItem;
					DateTime dateTime = new DateTime((long)rdpSigningCertificateCacheItem.lastUpdateTime);
					m_TraceWrite.TraceVerbose("GetRdpSigningCertificate : CachedTime: " + dateTime.ToString() + " RdpCertificateLastQueryTime: " + gm_RdpCertificateLastQueryTime.ToString() + " CurrentDateTime: " + DateTime.Now);
					if ((DateTime.Now - gm_RdpCertificateLastQueryTime).Minutes < gm_WksRdpCertLastUpdateFetchInMinutes)
					{
						m_TraceWrite.TraceVerbose("Rdp Signing Certificate Info found in cache, not re-querying, it may be stale!");
						result = rdpSigningCertificateCacheItem.objRdpSigningCertificate;
					}
					else if (!isCacheExpired(ref rdpSigningCertificateCacheItem.lastUpdateTime))
					{
						gm_RdpCertificateLastQueryTime = DateTime.Now;
						result = rdpSigningCertificateCacheItem.objRdpSigningCertificate;
						m_TraceWrite.TraceVerbose("Rdp Signing Certificate Info found in cache!");
					}
					else
					{
						m_TraceWrite.TraceInfo("Cache has expired, downloading new information.");
						result = GetFreshRdpSigningCertificate();
					}
				}
				else
				{
					m_TraceWrite.TraceInfo("Cache is not available, downloading information.");
					result = GetFreshRdpSigningCertificate();
				}
			}
			return result;
		}

		public WorkspaceInfo GetWorkspaceInfo()
		{
			if (!HttpContext.Current.Response.IsClientConnected)
			{
				TraceWrite.TraceInfoNoContext("Returning early, client not connected");
				return null;
			}
			object workspaceInfoFromCache = GetWorkspaceInfoFromCache();
			if (workspaceInfoFromCache != null)
			{
				WorkspaceCacheItem workspaceCacheItem = workspaceInfoFromCache as WorkspaceCacheItem;
				DateTime dateTime = new DateTime((long)workspaceCacheItem.lastUpdateTime);
				m_TraceWrite.TraceVerbose("GetWorkspaceInfo : CachedTime: " + dateTime.ToString() + " WorkspaceLastQueryTime: " + gm_WorkspaceLastQueryTime.ToString() + " CurrentDateTime: " + DateTime.Now);
				if ((DateTime.Now - gm_WorkspaceLastQueryTime).Minutes < gm_WksRdpCertLastUpdateFetchInMinutes)
				{
					WorkspaceInfo info = workspaceCacheItem.Info;
					m_TraceWrite.TraceVerbose("Workspace Info found in cache, not re-querying, it may be stale!!!");
					return info;
				}
				if (!isCacheExpired(ref workspaceCacheItem.lastUpdateTime))
				{
					WorkspaceInfo info2 = workspaceCacheItem.Info;
					gm_WorkspaceLastQueryTime = DateTime.Now;
					m_TraceWrite.TraceVerbose("Workspace Info found in cache!!!");
					return info2;
				}
				m_TraceWrite.TraceInfo("Cache has expired, downloading new information.");
			}
			return GetFreshWorkspaceInfo();
		}

		internal WorkspaceInfo GetFreshWorkspaceInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			StringBuilder stringBuilder2 = new StringBuilder(80);
			StringBuilder stringBuilder3 = new StringBuilder(80);
			WorkspaceInfo workspaceInfo = null;
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			uint workspaceInfo2 = NativeMethods.getWorkspaceInfo(stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity, tSName, m_cpub_rpcport);
			if (-2147024891 == (int)workspaceInfo2)
			{
				TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get workspace information: access denied.");
				return null;
			}
			if (-2147023179 == (int)workspaceInfo2)
			{
				TSEventLog.Event(args: new object[2] { tSName, workspaceInfo2 }, message: LangSpecific.TSPORTAL_ERROR_15, id: 15, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get workspace information: Server " + tSName + " is not running a supported CPub service (RPC interface mismatch). 0x" + workspaceInfo2.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				throw new UnsupportedSourceServerException();
			}
			if (workspaceInfo2 != 0)
			{
				TSEventLog.Event(args: new object[2] { tSName, workspaceInfo2 }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get workspace information: 0x" + workspaceInfo2.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				return null;
			}
			workspaceInfo = new WorkspaceInfo();
			workspaceInfo.WorkspaceName = stringBuilder2.ToString();
			workspaceInfo.WorkspaceId = stringBuilder.ToString();
			workspaceInfo.WorkspaceDescription = string.Empty;
			workspaceInfo.RedirectorName = stringBuilder3.ToString();
			workspaceInfo.EventLogUploadAddress = string.Empty;
			ulong lastUpdateTime = GetLastUpdateTime();
			gm_WorkspaceLastQueryTime = DateTime.Now;
			PutWorkspaceInfoInCache(ref workspaceInfo, lastUpdateTime);
			return workspaceInfo;
		}

		private object GetWorkspaceInfoFromCache()
		{
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			return HttpRuntime.Cache["TsWorkspaceCache" + tSName];
		}

		private void PutWorkspaceInfoInCache(ref WorkspaceInfo wsInfo, ulong lastUpdateTime)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			m_TraceWrite.TraceVerbose("Putting workspace Info into the Cache");
			WorkspaceCacheItem workspaceCacheItem = new WorkspaceCacheItem();
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			workspaceCacheItem.Info = wsInfo;
			workspaceCacheItem.lastUpdateTime = lastUpdateTime;
			HttpRuntime.Cache.Insert("TsWorkspaceCache" + tSName, (object)workspaceCacheItem, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
		}

		public Task<Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>> GetApplicationsAsync(string strSid, bool onlyShowAvailableByDefaultResources, bool includeRDPFileContents)
		{
			AppInfo[] array = null;
			AppInfo[] array2 = null;
			if (!HttpContext.Current.Response.IsClientConnected)
			{
				TraceWrite.TraceInfoNoContext("Returning early, client not connected");
				return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(array, array2));
			}
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			if (gm_filteringDisabled)
			{
				m_TraceWrite.TraceInfo("App Filtering is disabled.");
			}
			DateTime utcNow = DateTime.UtcNow;
			TraceWrite.TraceInfoNoContext("User name for current Windows Identity: " + strSid);
			m_TraceWrite.TraceInfo("Attempting to get Applications from the cache");
			cacheItem appsFromCache = GetAppsFromCache(tSName);
			if (appsFromCache != null)
			{
				if (!m_PollAppFreshness || !isCacheExpired(ref appsFromCache.lastUpdateTime))
				{
					array = appsFromCache.apps;
					array2 = appsFromCache.desktops;
					m_TraceWrite.TraceVerbose("Apps found in cache!!!");
					if (array == null && array2 == null)
					{
						m_TraceWrite.TraceError("No applications were listed in the cache, returning an empty set.");
						return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(array, array2));
					}
				}
				else
				{
					m_TraceWrite.TraceInfo("Cache has expired, downloading new information.");
					HttpRuntime.Cache.Remove("CPubApps:" + tSName);
				}
			}
			else
			{
				m_TraceWrite.TraceInfo("No apps found in the cache");
			}
			if (array == null && array2 == null)
			{
				appsFromCache = GetFreshApps(utcNow);
				array = appsFromCache.apps;
				array2 = appsFromCache.desktops;
			}
			if (!gm_filteringDisabled)
			{
				AppInfo[] filteredApps = null;
				AppInfo[] filteredDesktops = null;
				bool flag = false;
				if (!m_PollAppFreshness)
				{
					flag = GetFilteredAppsFromCache(strSid, out filteredApps, out filteredDesktops);
				}
				if (!flag)
				{
					m_filterer.FilterApplications(array, array2, strSid, out filteredApps, out filteredDesktops);
					PutFilteredAppsInCache(strSid, ref filteredApps, ref filteredDesktops);
					array = filteredApps;
					array2 = filteredDesktops;
				}
			}
			return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(array, array2));
		}

		private cacheItem GetAppsFromCache(string key)
		{
			object obj = HttpRuntime.Cache["CPubApps:" + key];
			if (obj == null)
			{
				return null;
			}
			return obj as cacheItem;
		}

		private void PutRAPAppsInCache(ref cacheItem dataToCache, string key)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			m_TraceWrite.TraceVerbose("Putting applications into the Cache");
			HttpRuntime.Cache.Insert("CPubApps:" + key, (object)dataToCache, (CacheDependency)null, DateTime.UtcNow.AddMinutes(gm_CacheExpirationInMinutes), Cache.NoSlidingExpiration, (CacheItemPriority)3, new CacheItemRemovedCallback(OnCacheItemRemoved));
		}

		private bool GetFilteredAppsFromCache(string userSid, out AppInfo[] filteredApps, out AppInfo[] filteredDesktops)
		{
			filteredApps = null;
			filteredDesktops = null;
			bool result = false;
			string text = "UserFilteredCPubApps:" + userSid;
			if (HttpRuntime.Cache[text] is UserFilteredAppCacheItem userFilteredAppCacheItem)
			{
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				string tSName = m_TSName;
				gm_stateLock.ReleaseReaderLock();
				if (string.CompareOrdinal(userFilteredAppCacheItem.cpubName, tSName) != 0)
				{
					TraceWrite.TraceVerboseNoContext("Filtered apps removed from the cache for " + userSid);
					HttpRuntime.Cache.Remove(text);
				}
				else
				{
					TraceWrite.TraceVerboseNoContext("Filtered apps found in the cache for " + userSid);
					filteredApps = userFilteredAppCacheItem.appsListFiltered;
					filteredDesktops = userFilteredAppCacheItem.desktopsListFiltered;
					result = true;
				}
			}
			return result;
		}

		private void PutFilteredAppsInCache(string userSid, ref AppInfo[] filteredApps, ref AppInfo[] filteredDesktops)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			if (filteredApps != null && filteredDesktops != null)
			{
				UserFilteredAppCacheItem userFilteredAppCacheItem = new UserFilteredAppCacheItem();
				gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
				userFilteredAppCacheItem.cpubName = m_TSName;
				gm_stateLock.ReleaseReaderLock();
				userFilteredAppCacheItem.appsListFiltered = filteredApps;
				userFilteredAppCacheItem.desktopsListFiltered = filteredDesktops;
				CacheDependency val = new CacheDependency((string[])null, new string[1] { "CPubApps:" + userFilteredAppCacheItem.cpubName });
				HttpRuntime.Cache.Insert("UserFilteredCPubApps:" + userSid, (object)userFilteredAppCacheItem, val);
				TraceWrite.TraceVerboseNoContext("Filtered apps stored in the cache for " + userSid);
			}
		}

		private void RemoveAppsForServersNotInListFromCache()
		{
			IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key.ToString();
				if (string.CompareOrdinal(text, 0, "CPubApps:", 0, "CPubApps:".Length) == 0 && string.CompareOrdinal(text.Substring("CPubApps:".Length), tSName) != 0)
				{
					TraceWrite.TraceVerboseNoContext("Deleting key " + text + " from cache, since the Connection broker it refers to are not being pointed-to by TSWA");
					_ = enumerator.Value;
					HttpRuntime.Cache.Remove(text);
				}
			}
		}

		private void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason removedReason)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			m_TraceWrite.TraceVerbose("Received OnCacheItemRemoved key: {0}, reason: {1}", key, removedReason);
			if ((int)removedReason != 1)
			{
				if ((int)removedReason != 2)
				{
					return;
				}
				if (string.CompareOrdinal(key, 0, "CPubApps:", 0, "CPubApps:".Length) == 0)
				{
					m_TraceWrite.TraceInfo("Apps cache item expired....");
					cacheItem cacheItem2 = value as cacheItem;
					AppInfo[] array = null;
					if (cacheItem2 != null)
					{
						array = cacheItem2.apps;
					}
					if (array != null)
					{
						m_TraceWrite.TraceInfo("apps cache entry is not null");
						DateTime utcNow = DateTime.UtcNow;
						if (GetFreshApps(utcNow) == null)
						{
							m_TraceWrite.TraceInfo("No fresh apps found");
						}
					}
					else
					{
						m_TraceWrite.TraceInfo("apps cache entry is null");
					}
				}
				if (string.CompareOrdinal(key, 0, "TsWorkspaceCache", 0, "TsWorkspaceCache".Length) == 0 && GetFreshWorkspaceInfo() == null)
				{
					m_TraceWrite.TraceInfo("Unable to get workspace information.");
				}
				if (string.CompareOrdinal(key, 0, "TsRdpSignCertificateCache", 0, "TsRdpSignCertificateCache".Length) == 0 && GetFreshRdpSigningCertificate() == null)
				{
					m_TraceWrite.TraceInfo("Unable to get Rdp Certificate information.");
				}
			}
			else if (string.CompareOrdinal(key, 0, "CPubApps:", 0, "CPubApps:".Length) == 0)
			{
				m_TraceWrite.TraceInfo("Apps cache item removed....");
			}
		}

		internal cacheItem GetFreshApps(DateTime requestTime)
		{
			cacheItem dataToCache = new cacheItem();
			string providerFilter = "";
			string szFilter = "";
			int numResources = 0;
			ulong lastUpdateTime = 0uL;
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			IntPtr context = IntPtr.Zero;
			uint num = NativeMethods.initializeResources(ref context, null, tSName, m_cpub_rpcport, 0, 0, providerFilter, szFilter, ref numResources, ref lastUpdateTime);
			if (-2147024891 == (int)num)
			{
				TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to initialize resources: access denied.");
			}
			else
			{
				if (-2147023179 == (int)num)
				{
					TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_15, id: 15, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to initialize resources: Server " + tSName + " is not running a supported CPub service (RPC interface mismatch). 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
					throw new UnsupportedSourceServerException();
				}
				if (num != 0)
				{
					TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to initialize resources: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				}
			}
			if (num != 0)
			{
				return dataToCache;
			}
			StringBuilder stringBuilder = new StringBuilder(193);
			StringBuilder stringBuilder2 = new StringBuilder(80);
			StringBuilder stringBuilder3 = new StringBuilder(131072);
			StringBuilder stringBuilder4 = new StringBuilder(256);
			StringBuilder stringBuilder5 = new StringBuilder(40);
			StringBuilder stringBuilder6 = new StringBuilder(256, 256);
			bool isDiscoverable = true;
			byte[] blobContents = new byte[3072000];
			byte[] array = new byte[3072000];
			byte[] iconContents = new byte[3072000];
			uint num2 = 3072000u;
			uint num3 = 3072000u;
			uint num4 = 3072000u;
			uint numFileAssociations = 0u;
			uint numFolders = 0u;
			int resourceType = 0;
			FileExtensionInfo[] array2 = null;
			ArrayList apps = new ArrayList();
			ArrayList apps2 = new ArrayList();
			for (uint num5 = 0u; num5 < numResources; num5++)
			{
				num2 = 3072000u;
				num3 = 3072000u;
				num = NativeMethods.getResource(context, num5, stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity, stringBuilder4, stringBuilder4.Capacity, stringBuilder5, stringBuilder5.Capacity, ref isDiscoverable, ref num2, array, ref num3, blobContents, ref resourceType, ref numFileAssociations, ref numFolders);
				if (-2147024891 == (int)num)
				{
					TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get resource: access denied.");
				}
				else if (num != 0)
				{
					TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get resource: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				}
				if (num != 0)
				{
					continue;
				}
				array2 = new FileExtensionInfo[numFileAssociations];
				for (uint num6 = 0u; num6 < numFileAssociations; num6++)
				{
					StringBuilder stringBuilder7 = new StringBuilder(256);
					bool primaryHandler = false;
					num4 = 3072000u;
					num = NativeMethods.getResourceFileAssociation(context, num5, num6, stringBuilder7, stringBuilder7.Capacity, ref primaryHandler, ref num4, iconContents);
					if (-2147024891 == (int)num)
					{
						TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
						m_TraceWrite.TraceError("Unable to get resource file association info: access denied.");
					}
					else if (num != 0)
					{
						TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
						m_TraceWrite.TraceError("Unable to get resource file association info: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
					}
					if (num != 0)
					{
						break;
					}
					array2[num6] = new FileExtensionInfo(stringBuilder7.ToString(), primaryHandler);
					if (num4 != 0)
					{
						string iconName = stringBuilder.ToString() + "-" + stringBuilder7.ToString().Replace(".", "");
						array2[num6].AssocIcon = m_iconFactory.GetOrCreateIconInfo(iconContents, iconName, (int)num4, lastUpdateTime, null, null);
					}
				}
				string[] array3 = new string[numFolders];
				for (uint num7 = 0u; num7 < numFolders; num7++)
				{
					num = NativeMethods.getResourceFolder(context, num5, num7, stringBuilder6, stringBuilder6.Capacity);
					if (-2147024891 == (int)num)
					{
						TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
						m_TraceWrite.TraceError("Unable to get resource folder name info: access denied.");
					}
					else if (num != 0)
					{
						TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
						m_TraceWrite.TraceError("Unable to get resource folder name info: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
					}
					if (num != 0)
					{
						break;
					}
					if (stringBuilder6[0] != '/')
					{
						m_TraceWrite.TraceVerbose("Folder name is missing a leading slash, adding one. Folder name = " + stringBuilder6.ToString());
						try
						{
							stringBuilder6.Insert(0, '/');
						}
						catch (ArgumentOutOfRangeException)
						{
							num = 2147942561u;
							TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
							m_TraceWrite.TraceError("The resource folder name exceeded the allowed size: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
							continue;
						}
					}
					array3[num7] = stringBuilder6.ToString();
				}
				StringBuilder stringBuilder8 = new StringBuilder(1024);
				do
				{
					num = NativeMethods.getResourceSecurityDescriptor(context, num5, stringBuilder8.Capacity, stringBuilder8);
					if (-2147024774 == (int)num)
					{
						stringBuilder8.EnsureCapacity(stringBuilder8.Capacity * 2);
					}
				}
				while (-2147024774 == (int)num);
				if (-2147024891 == (int)num)
				{
					TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get resource security descriptor: access denied.");
				}
				else if (num != 0)
				{
					TSEventLog.Event(args: new object[2] { tSName, num }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
					m_TraceWrite.TraceError("Unable to get resource security descriptor: 0x" + num.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture));
				}
				if (isDiscoverable)
				{
					if (resourceType == 2)
					{
						ProcessPublishedApp(stringBuilder.ToString(), stringBuilder2.ToString(), stringBuilder3.ToString(), stringBuilder4.ToString(), (num2 == 0) ? null : array, num2, resourceType, array2, array3, stringBuilder8.ToString(), ref apps2, requestTime, lastUpdateTime);
					}
					else
					{
						ProcessPublishedApp(stringBuilder.ToString(), stringBuilder2.ToString(), stringBuilder3.ToString(), stringBuilder4.ToString(), (num2 == 0) ? null : array, num2, resourceType, array2, array3, stringBuilder8.ToString(), ref apps, requestTime, lastUpdateTime);
					}
				}
			}
			NativeMethods.freeResources(ref context);
			cacheItem appsFromCache = GetAppsFromCache(tSName);
			if (appsFromCache != null && appsFromCache.lastUpdateTime > lastUpdateTime)
			{
				m_TraceWrite.TraceVerbose("Cache has newer information, aborting update");
				return appsFromCache;
			}
			dataToCache.lastUpdateTime = lastUpdateTime;
			dataToCache.apps = (AppInfo[])apps.ToArray(typeof(AppInfo));
			dataToCache.desktops = (AppInfo[])apps2.ToArray(typeof(AppInfo));
			PutRAPAppsInCache(ref dataToCache, tSName);
			return dataToCache;
		}

		private bool isCacheExpired(ref ulong lastUpdateTime)
		{
			ulong lastUpdateTime2 = 0uL;
			bool result = false;
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			uint lastCacheUpdateTime = NativeMethods.getLastCacheUpdateTime(ref lastUpdateTime2, tSName, m_cpub_rpcport);
			if (-2147024891 == (int)lastCacheUpdateTime)
			{
				TSEventLog.Event(args: new object[1] { tSName }, message: LangSpecific.TSPORTAL_ERROR_10, id: 10, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get last cache update time: access denied. Assuming cache is NOT expired.");
			}
			else if (lastCacheUpdateTime != 0)
			{
				TSEventLog.Event(args: new object[2] { tSName, lastCacheUpdateTime }, message: LangSpecific.TSPORTAL_ERROR_11, id: 11, etype: (EventLogEntryType)1);
				m_TraceWrite.TraceError("Unable to get last cache update time: 0x" + lastCacheUpdateTime.ToString(gm_HexStringFormat, CultureInfo.InvariantCulture) + ". Assuming cache is NOT expired.");
			}
			else
			{
				m_TraceWrite.TraceInfo("Checking if cache is expired:  cacheUpdate: " + lastUpdateTime.ToString(CultureInfo.CurrentCulture) + " RPC Server cacheUpdate: " + lastUpdateTime2.ToString(CultureInfo.CurrentCulture));
				if (lastUpdateTime2 > lastUpdateTime)
				{
					m_TraceWrite.TraceInfo("Cache is Expired.");
					result = true;
					lastUpdateTime = lastUpdateTime2;
				}
				else
				{
					m_TraceWrite.TraceInfo("The cache is NOT expired.");
				}
			}
			return result;
		}

		private void ProcessPublishedApp(string alias, string name, string resourceFile, string fileExtension, byte[] iconContents, uint pceIconSize, int resourceType, FileExtensionInfo[] fileAssociations, string[] folderNames, string securityDescriptor, ref ArrayList apps, DateTime requestTime, ulong lastUpdateTime)
		{
			RemoteAppXml remoteAppXml = new RemoteAppXml();
			gm_stateLock.AcquireReaderLock(gm_stateLockTimeout);
			string tSName = m_TSName;
			gm_stateLock.ReleaseReaderLock();
			IconInfo icon;
			if (iconContents == null)
			{
				m_TraceWrite.TraceWarning("Program's icon file not available - using default.");
				icon = m_iconFactory.GetDefaultIcon();
			}
			else
			{
				icon = m_iconFactory.GetOrCreateIconInfo(iconContents, alias, (int)pceIconSize, lastUpdateTime, IconFactory.gm_defaultImageFormat, IconFactory.gm_defaultImageDimensions);
			}
			string rdp_file_name = "";
			PrepareRdpFile(ref resourceFile, fileExtension, tSName, alias, out rdp_file_name, requestTime);
			resourceFile = GlobalObject.escape((object)resourceFile);
			RDPFileInfo rdpFileInfo = new RDPFileInfo(rdp_file_name, resourceFile, string.Empty);
			remoteAppXml.Initialize(name, fileAssociations, folderNames, rdpFileInfo, icon, alias, "", "", securityDescriptor);
			bool flag = resourceType == 2;
			AppInfo value = new AppInfo(name, tSName, remoteAppXml, flag, requestTime, fileExtension, availableToUsersByDefault: true);
			if (flag)
			{
				apps.Add(value);
			}
			else
			{
				apps.Insert(0, value);
			}
			m_TraceWrite.TraceInfo("App: {0} added, FileExtension: {1}", name, fileExtension);
		}

		private void PrepareRdpFile(ref string rdp_contents, string fileExtension, string ts_name, string alias, out string rdp_file_name, DateTime requestTime)
		{
			string text = "cpub-" + alias;
			bool flag = false;
			try
			{
				flag = Convert.ToBoolean(ConfigurationManager.AppSettings["OptimizeExperienceState"], CultureInfo.InvariantCulture);
			}
			catch (ConfigurationErrorsException)
			{
				TraceWrite.TraceInfoNoContext("OptimizeExperienceState not present in config file, not optimizing for LAN");
			}
			catch (FormatException)
			{
				TraceWrite.TraceInfoNoContext("OptimizeExperienceState is not in the correct format, not optimizing for LAN");
			}
			if (flag)
			{
				int num = rdp_contents.IndexOf("connection type:i:", StringComparison.OrdinalIgnoreCase);
				if (num != -1)
				{
					TraceWrite.TraceInfoNoContext("Connection type detected, removing the string.");
					int num2 = rdp_contents.IndexOf("\n", num, StringComparison.OrdinalIgnoreCase);
					if (num2 != -1)
					{
						rdp_contents = rdp_contents.Remove(num, num2 - num + 1);
					}
					else
					{
						rdp_contents = rdp_contents.Remove(num);
					}
				}
				if (rdp_contents.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
				{
					rdp_contents += "connection type:i:6\r\n";
				}
				else
				{
					rdp_contents += "\r\nconnection type:i:6\r\n";
				}
				TraceWrite.TraceInfoNoContext("Optimizing RDP for LAN.");
			}
			text = ((!string.IsNullOrEmpty(fileExtension)) ? (text + fileExtension) : (text + ".rdp"));
			text = RemoteAppXml.CleanupRdpFilePath(text);
			rdp_file_name = text;
		}
	}
	[Serializable]
	public class WorkspaceUnavailableException : Exception, ISerializable
	{
		public WorkspaceUnavailableException()
		{
		}

		public WorkspaceUnavailableException(string message)
			: base(message)
		{
		}

		public WorkspaceUnavailableException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected WorkspaceUnavailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class WorkspaceUnknownFolderException : Exception, ISerializable
	{
		public WorkspaceUnknownFolderException()
		{
		}

		public WorkspaceUnknownFolderException(string message)
			: base(message)
		{
		}

		public WorkspaceUnknownFolderException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected WorkspaceUnknownFolderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class UnsupportedSourceServerException : Exception, ISerializable
	{
		public UnsupportedSourceServerException()
		{
		}

		public UnsupportedSourceServerException(string message)
			: base(message)
		{
		}

		public UnsupportedSourceServerException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UnsupportedSourceServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class InvalidTenantException : Exception, ISerializable
	{
		public InvalidTenantException()
		{
		}

		public InvalidTenantException(string message)
			: base(message)
		{
		}

		public InvalidTenantException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidTenantException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class InvalidAppIconException : Exception, ISerializable
	{
		public InvalidAppIconException()
		{
		}

		public InvalidAppIconException(string message)
			: base(message)
		{
		}

		public InvalidAppIconException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidAppIconException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	[Serializable]
	public class ResourceNotModifiedException : Exception, ISerializable
	{
		public ResourceNotModifiedException()
		{
		}

		public ResourceNotModifiedException(string message)
			: base(message)
		{
		}

		public ResourceNotModifiedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ResourceNotModifiedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	public class ResourceFileHandler : HttpTaskAsyncHandler
	{
		private enum FileType
		{
			Resource,
			ICO,
			PNG,
			RDP
		}

		private struct ResourceFileHandlerResult
		{
			public byte[] contents;

			public string contentType;

			public int statusCode;

			public DateTime lastModifiedTime;
		}

		private RapWebService rapWebService;

		private WebFeedConfig feedConfig;

		public override bool IsReusable => false;

		public ResourceFileHandler()
		{
			bool pollAppFreshness = false;
			feedConfig = new WebFeedConfig();
			rapWebService = new RapWebService(pollAppFreshness);
		}

		public override async Task ProcessRequestAsync(HttpContext context)
		{
			ResourceFileHandlerResult result = default(ResourceFileHandlerResult);
			result.contents = null;
			result.contentType = null;
			result.lastModifiedTime = DateTime.MinValue;
			result.statusCode = 404;
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (rapWebService == null)
			{
				throw new WorkspaceUnavailableException(LangSpecific.TSPORTAL_NO_WORKSPACE_CONFIG_TEXT);
			}
			bool headReq = context.Request.HttpMethod.Equals("HEAD");
			try
			{
				Match topLevelPathMatch = Regex.Match(context.Request.PathInfo, "^/([^/]+)$", RegexOptions.CultureInvariant);
				Match match = Regex.Match(context.Request.PathInfo, "^/fileassoc/([^/]+)/([^/]+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
				if (!headReq && !context.Request.HttpMethod.Equals("GET"))
				{
					result.statusCode = 405;
					context.Response.Headers.Add("Allow", "GET, HEAD");
				}
				else if (match.Success)
				{
					string value = match.Groups[1].Value;
					string desiredFile = match.Groups[2].Value;
					string extension = Path.GetExtension(desiredFile);
					string fileStem = Path.GetFileNameWithoutExtension(desiredFile);
					TraceWrite.TraceVerboseNoContext("ResourceFileHandler serving icon file '" + desiredFile + "' for file extension '" + value + "'");
					if (extension.Equals(".ico", StringComparison.OrdinalIgnoreCase))
					{
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						result = await ServeFileAssociationIconAsync(fileStem, value);
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
					}
				}
				else if (topLevelPathMatch.Success)
				{
					string desiredFile = topLevelPathMatch.Groups[1].Value;
					string extension = Path.GetExtension(desiredFile);
					string fileStem = Path.GetFileNameWithoutExtension(desiredFile);
					TraceWrite.TraceVerboseNoContext("ResourceFileHandler serving file:" + desiredFile);
					FileType fileType = FileType.Resource;
					if (extension.Equals(".ico", StringComparison.OrdinalIgnoreCase))
					{
						fileType = FileType.ICO;
					}
					else if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
					{
						fileType = FileType.PNG;
					}
					else if (extension.Equals(".rdp", StringComparison.OrdinalIgnoreCase))
					{
						fileType = FileType.RDP;
					}
					switch (fileType)
					{
					case FileType.ICO:
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						result = await ServeAppIconAsync(fileStem);
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						break;
					case FileType.PNG:
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						result = await ServeImageAsync(fileType, fileStem);
						if (!context.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						break;
					default:
						if (extension.Length != 0)
						{
							if (!context.Response.IsClientConnected)
							{
								TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
								return;
							}
							result = await ServeResourceAsync(fileType, desiredFile);
							if (!context.Response.IsClientConnected)
							{
								TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
								return;
							}
						}
						break;
					}
				}
				else
				{
					result.statusCode = 404;
				}
			}
			catch (InvalidTenantException ex)
			{
				TraceWrite.TraceWarningNoContext("Error mapping the request to a tenant: " + ex.Message);
				result.statusCode = 404;
			}
			catch (ArgumentException ex2)
			{
				TraceWrite.TraceWarningNoContext("Error in the format of the requested file name: " + ex2.Message);
				result.statusCode = 404;
			}
			catch (Exception ex3)
			{
				TraceWrite.TraceErrorNoContext("ResourceFileHandler: Internal error:" + ex3.Message);
				result.statusCode = 500;
			}
			if (result.contents != null && result.statusCode == 200 && result.contentType != null)
			{
				context.Response.ContentType = result.contentType;
				if (!headReq)
				{
					context.Response.BinaryWrite(result.contents);
				}
				if (result.lastModifiedTime != DateTime.MinValue && result.lastModifiedTime.CompareTo(DateTime.Now) < 0)
				{
					context.Response.Cache.SetLastModified(result.lastModifiedTime);
				}
			}
			context.Response.StatusCode = result.statusCode;
			try
			{
				context.Response.Flush();
			}
			catch (HttpException val)
			{
				HttpException val2 = val;
				TraceWrite.TraceWarningNoContext("Error flushing the response to the client: " + ((object)val2).ToString());
			}
		}

		private string GetUserIdentity()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			AuthenticationMode val = (AuthenticationMode)0;
			string result = "";
			object section = ConfigurationManager.GetSection("system.web/authentication");
			AuthenticationSection val2 = (AuthenticationSection)((section is AuthenticationSection) ? section : null);
			if (val2 != null)
			{
				val = val2.Mode;
			}
			if ((int)val != 1)
			{
				if (HttpContext.Current.Request.Cookies["TSWAAuthHttpOnlyCookie"] != null)
				{
					result = new TSFormAuthTicketInfo(HttpContext.Current).UserIdentity;
				}
				else
				{
					string text = "";
					HttpCookie val3 = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
					if (val3 != null)
					{
						text = val3.Value;
					}
					if (!string.IsNullOrEmpty(text))
					{
						FormsAuthenticationTicket val4 = null;
						try
						{
							val4 = FormsAuthentication.Decrypt(text);
						}
						catch (Exception ex)
						{
							if (!(ex is ArgumentException) && !(ex is FormatException) && !(ex is HttpException))
							{
								throw;
							}
							TraceWrite.TraceWarningNoContext("In ResourceFileHandler, .ASPXAUTH cookie contains an invalid forms auth ticket. Exception: {0}", ex.ToString());
						}
						if (val4 != null)
						{
							result = val4.Name;
						}
					}
				}
			}
			else
			{
				result = ((WindowsIdentity)HttpContext.Current.User.Identity).User.ToString();
			}
			return result;
		}

		private async Task<ResourceFileHandlerResult> ServeFileAssociationIconAsync(string fileStem, string fileAssocExt)
		{
			ResourceFileHandlerResult result = default(ResourceFileHandlerResult);
			result.contents = null;
			result.statusCode = 404;
			result.contentType = null;
			result.lastModifiedTime = DateTime.MinValue;
			AppInfo appInfo = Array.Find(new List<AppInfo>(await rapWebService.GetRemoteAppsAsync(GetUserIdentity(), onlyShowAvailableByDefaultResources: false, includeRDPFileContents: false)).ToArray(), (AppInfo currentApp) => currentApp.RemoteAppXml.ApplicationAlias.Equals(fileStem, StringComparison.OrdinalIgnoreCase));
			if (appInfo != null)
			{
				IconInfo assocIcon = Array.Find(appInfo.RemoteAppXml.Extensions, (FileExtensionInfo fileAssoc) => fileAssoc.Name.Equals(fileAssocExt, StringComparison.OrdinalIgnoreCase) && fileAssoc.AssocIcon.type.Equals("binary")).AssocIcon;
				if (assocIcon != null)
				{
					result.contents = assocIcon.iconBytes();
					if (result.contents != null)
					{
						result.statusCode = 200;
						result.contentType = "image/x-icon";
						result.lastModifiedTime = appInfo.Timestamp;
					}
				}
			}
			return result;
		}

		private async Task<ResourceFileHandlerResult> ServeAppIconAsync(string fileStem)
		{
			ResourceFileHandlerResult result = default(ResourceFileHandlerResult);
			result.contents = null;
			result.statusCode = 404;
			result.contentType = null;
			result.lastModifiedTime = DateTime.MinValue;
			if (fileStem.Equals("mstsc256", StringComparison.OrdinalIgnoreCase))
			{
				MemoryStream memoryStream = new MemoryStream();
				NeutralResources.mstscIcon.Save((Stream)memoryStream);
				if (memoryStream.Length > 0)
				{
					result.contents = memoryStream.ToArray();
					result.statusCode = 200;
					result.contentType = "image/x-icon";
					result.lastModifiedTime = DateTime.MinValue;
				}
			}
			else
			{
				AppInfo appInfo = Array.Find(new List<AppInfo>(await rapWebService.GetRemoteAppsAsync(GetUserIdentity(), onlyShowAvailableByDefaultResources: false, includeRDPFileContents: false)).ToArray(), (AppInfo currentApp) => currentApp.RemoteAppXml.ApplicationAlias.Equals(fileStem, StringComparison.OrdinalIgnoreCase) && currentApp.RemoteAppXml.Icon.type.Equals("binary"));
				if (appInfo != null)
				{
					result.contents = appInfo.RemoteAppXml.Icon.iconBytes();
					if (result.contents != null)
					{
						result.statusCode = 200;
						result.contentType = "image/x-icon";
						result.lastModifiedTime = appInfo.Timestamp;
					}
				}
			}
			return result;
		}

		private async Task<ResourceFileHandlerResult> ServeImageAsync(FileType fileType, string fileStem)
		{
			ResourceFileHandlerResult result = default(ResourceFileHandlerResult);
			result.contents = null;
			result.statusCode = 404;
			result.contentType = null;
			result.lastModifiedTime = DateTime.MinValue;
			Match match = Regex.Match(fileStem, "^(.+)_(\\d+)x(\\d+)$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
			if (match.Success)
			{
				string iconStem = match.Groups[1].Value;
				int iconWidth = Convert.ToInt32(match.Groups[2].Value, CultureInfo.InvariantCulture);
				int num = Convert.ToInt32(match.Groups[3].Value, CultureInfo.InvariantCulture);
				if (ValidIconType(iconWidth, fileType) && iconWidth == num)
				{
					if (iconStem.Equals("mstsc256", StringComparison.OrdinalIgnoreCase) && fileType == FileType.PNG)
					{
						MemoryStream memoryStream = new MemoryStream();
						((Image)new Icon(NeutralResources.mstscIcon, iconWidth, num).ToBitmap()).Save((Stream)memoryStream, ImageFormat.Png);
						if (memoryStream.Length > 0)
						{
							result.contentType = "image/png";
							result.contents = memoryStream.ToArray();
							result.statusCode = 200;
							result.lastModifiedTime = DateTime.MinValue;
						}
					}
					else
					{
						AppInfo appInfo = Array.Find(new List<AppInfo>(await rapWebService.GetRemoteAppsAsync(GetUserIdentity(), onlyShowAvailableByDefaultResources: false, includeRDPFileContents: false)).ToArray(), (AppInfo currentApp) => currentApp.RemoteAppXml.ApplicationAlias.Equals(iconStem, StringComparison.OrdinalIgnoreCase) && currentApp.RemoteAppXml.Icon.type.Equals("binary"));
						if (appInfo != null && fileType == FileType.PNG)
						{
							result.contents = appInfo.RemoteAppXml.Icon.GetImageBytes(iconWidth, ImageFormat.Png);
							if (result.contents != null)
							{
								result.contentType = "image/png";
								result.statusCode = 200;
								result.lastModifiedTime = appInfo.Timestamp;
							}
						}
					}
				}
			}
			return result;
		}

		private async Task<ResourceFileHandlerResult> ServeResourceAsync(FileType fileType, string desiredFile)
		{
			ResourceFileHandlerResult result = default(ResourceFileHandlerResult);
			result.contents = null;
			result.statusCode = 404;
			result.contentType = null;
			result.lastModifiedTime = DateTime.MinValue;
			AppInfo appInfo = Array.Find(new List<AppInfo>(await rapWebService.GetRemoteAppsAsync(GetUserIdentity(), onlyShowAvailableByDefaultResources: false, includeRDPFileContents: true)).ToArray(), (AppInfo currentApp) => currentApp.RemoteAppXml.RdpFileInfo.RdpFileName.Equals(desiredFile, StringComparison.OrdinalIgnoreCase));
			if (appInfo != null)
			{
				if (fileType == FileType.RDP)
				{
					result.contentType = "application/x-rdp";
				}
				else
				{
					result.contentType = "application/octet-stream";
					HttpContext.Current.Response.Headers.Add("Content-Disposition", "attachment");
				}
				result.contents = Encoding.UTF8.GetBytes(GlobalObject.unescape((object)appInfo.RemoteAppXml.RdpFileInfo.RdpFileContents));
				if (result.contents != null)
				{
					result.statusCode = 200;
					result.lastModifiedTime = appInfo.Timestamp;
				}
			}
			return result;
		}

		private bool ValidIconType(int iconWidth, FileType fileType)
		{
			bool flag = false;
			if (feedConfig != null)
			{
				IconDimensionsElementCollection iconDimensions = feedConfig.IconSettings.IconDimensions;
				if (((ConfigurationElementCollection)iconDimensions).Count == 0)
				{
					flag = iconWidth == 32;
				}
				else
				{
					foreach (IconDimensionsElement item in iconDimensions)
					{
						if (item.Dim == iconWidth)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				return fileType == FileType.PNG;
			}
			return false;
		}
	}
	internal class IconCacheItem
	{
		internal ulong lastUpdateTime;

		internal IconInfo icon;
	}
	public class IconFactory
	{
		private static IconInfo gm_defaultIcon;

		public static ImageFormat gm_defaultImageFormat;

		public static int[] gm_defaultImageDimensions;

		private object m_defaultIconLock;

		private object m_cacheIconLock;

		private string m_cacheIconsPrefix;

		private long m_cacheExpirationInMinutes;

		static IconFactory()
		{
			gm_defaultIcon = null;
			gm_defaultImageFormat = ImageFormat.Png;
			gm_defaultImageDimensions = new int[1] { 32 };
			gm_defaultIcon = new IconInfo(NeutralResources.mstscIcon, "mstsc256", gm_defaultImageFormat, gm_defaultImageDimensions);
		}

		public IconFactory(string cachePrefix, long cacheExpirationTimeInMinutes)
		{
			m_cacheIconsPrefix = cachePrefix;
			m_cacheExpirationInMinutes = cacheExpirationTimeInMinutes;
			m_defaultIconLock = new object();
			m_cacheIconLock = new object();
		}

		public IconInfo GetDefaultIcon()
		{
			return gm_defaultIcon;
		}

		public IconInfo GetOrCreateIconInfo(byte[] iconContents, string iconName, int pceIconSize, ulong lastUpdateTime, ImageFormat imageFormat, int[] imageDimensions)
		{
			string text = m_cacheIconsPrefix + iconName;
			IconInfo iconInfo = GetCachedIconIfCurrent(text, lastUpdateTime);
			if (iconInfo == null)
			{
				lock (m_cacheIconLock)
				{
					iconInfo = GetCachedIconIfCurrent(text, lastUpdateTime);
					if (iconInfo == null)
					{
						IconCacheItem iconCacheItem = new IconCacheItem();
						iconCacheItem.lastUpdateTime = lastUpdateTime;
						if (imageFormat == null && imageDimensions == null)
						{
							iconCacheItem.icon = new IconInfo(iconContents, iconName, pceIconSize);
						}
						else
						{
							iconCacheItem.icon = new IconInfo(iconContents, iconName, pceIconSize, imageFormat, imageDimensions);
						}
						HttpRuntime.Cache.Insert(text, (object)iconCacheItem, (CacheDependency)null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(m_cacheExpirationInMinutes), (CacheItemPriority)3, (CacheItemRemovedCallback)null);
						TraceWrite.TraceVerboseNoContext("Wrote icon " + iconName + " into cache");
						iconInfo = iconCacheItem.icon;
					}
				}
			}
			return iconInfo;
		}

		private IconInfo GetCachedIconIfCurrent(string iconKey, ulong lastUpdateTime)
		{
			IconInfo result = null;
			object obj = null;
			if ((obj = HttpRuntime.Cache.Get(iconKey)) != null)
			{
				IconCacheItem iconCacheItem = obj as IconCacheItem;
				if (iconCacheItem.lastUpdateTime >= lastUpdateTime)
				{
					result = iconCacheItem.icon;
				}
			}
			return result;
		}
	}
	public enum FeedXmlVersion
	{
		Win7,
		Win8
	}
	public sealed class ContentNegotiationHelper
	{
		private const string gm_win8MediaType = "application/x-msts-radc+xml";

		private const string gm_win7MediaType = "text/xml";

		private const string gm_browserMediaType = "text/xml";

		private const string gm_versionParameterName = "radc_schema_version";

		private const string gm_win7VersionVal = "1.1";

		private const string gm_currentVersionVal = "2.0";

		private ContentNegotiationHelper()
		{
		}

		public static void GetPreferredXmlVersion(out FeedXmlVersion xmlVersion, out string contentType)
		{
			string text = HttpContext.Current.Request.Params.Get("radc_schema_version");
			if (!string.IsNullOrEmpty(text) && text.Trim().ToLower(CultureInfo.InvariantCulture).Equals("2.0"))
			{
				TraceWrite.TraceInfoNoContext("Detected URL parameter, serving Current version of XML Feed");
				contentType = "text/xml";
				xmlVersion = FeedXmlVersion.Win8;
				return;
			}
			if (!string.IsNullOrEmpty(text) && text.Trim().ToLower(CultureInfo.InvariantCulture).Equals("1.1"))
			{
				TraceWrite.TraceInfoNoContext("Detected URL parameter, serving Win7 version of XML Feed");
				contentType = "text/xml";
				xmlVersion = FeedXmlVersion.Win7;
				return;
			}
			try
			{
				GetVersionFromHttpHeaders(out xmlVersion, out contentType);
			}
			catch (Exception ex)
			{
				TraceWrite.TraceWarningNoContext("Error parsing HTTP Accept header, defaulting to Win7 version of XML feed: {0}", ex.Message);
				contentType = "text/xml";
				xmlVersion = FeedXmlVersion.Win7;
			}
		}

		private static void GetVersionFromHttpHeaders(out FeedXmlVersion xmlVersion, out string contentType)
		{
			xmlVersion = FeedXmlVersion.Win7;
			contentType = "text/xml";
			string[] acceptTypes = HttpContext.Current.Request.AcceptTypes;
			for (int i = 0; i < acceptTypes.Length; i++)
			{
				AcceptHeaderVal acceptHeaderVal = new AcceptHeaderVal(acceptTypes[i]);
				if (acceptHeaderVal.MediaRange.Equals("application/x-msts-radc+xml"))
				{
					acceptHeaderVal.Parameters.TryGetValue("radc_schema_version", out var value);
					if (!string.IsNullOrEmpty(value) && value.Equals("2.0"))
					{
						TraceWrite.TraceVerboseNoContext("Found accept-header requesting Win8 version of feed");
						xmlVersion = FeedXmlVersion.Win8;
						contentType = "application/x-msts-radc+xml";
						break;
					}
				}
			}
		}
	}
	internal class AcceptHeaderVal
	{
		private static Regex gm_mediaRangeSeparatorRegex;

		private string m_mediaRange;

		private Dictionary<string, string> m_parameters;

		internal string MediaRange => m_mediaRange;

		internal IDictionary<string, string> Parameters => m_parameters;

		static AcceptHeaderVal()
		{
			string text = "\r\n                                        \"            # opening quote\r\n                                        (             # begin group: quoted characters (0 or more times)\r\n                                            \\\\.       # escaped-char\r\n                                            |\r\n                                            [^\\\\\"]   # non-quote char\r\n                                        )*            # end group: quoted characters (0 or more times)\r\n                                        \"            # closing quote\r\n                                        ";
			string pattern = "\r\n                                              ^\r\n                                              (?<mediarange>   # begin capture group: media range + media parameters\r\n                                                  [^;]+       # media range (more permissive than actual spec). \r\n                                                  (           # begin group: media parameters (0 or more times)\r\n                                                      \\s*\r\n                                                      ;\r\n                                                      [^=]+   # media parameter name (more permissive than actual spec)\r\n                                                      =\r\n                                                      \\s*\r\n                                                      ( (" + text + ") | [^;]+ ) # media parameter value (token value more permissive than actual spec)\r\n                                                  )*          # end group: media parameters (0 or more times)\r\n                                              )               # end group: media range + media parameters\r\n                                              \\s*\r\n                                              ;\r\n                                              \\s*\r\n                                              q\r\n                                              \\s*\r\n                                              =[^;]+    # q-val (more permissive than actual spec)\r\n                                              .*          # accept-ext parameters (*way* more permissive than actual spec)\r\n                                              $";
			RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant;
			gm_mediaRangeSeparatorRegex = new Regex(pattern, options);
		}

		internal AcceptHeaderVal(string acceptValStr)
		{
			m_mediaRange = "";
			m_parameters = new Dictionary<string, string>();
			Parse(acceptValStr);
		}

		private void Parse(string acceptValStr)
		{
			if (acceptValStr == null)
			{
				throw new ArgumentNullException("acceptValStr");
			}
			Match match = gm_mediaRangeSeparatorRegex.Match(acceptValStr.Trim());
			string contentType = ((!match.Success) ? acceptValStr : match.Groups[1].Value);
			ContentType contentType2 = new ContentType(contentType);
			m_mediaRange = contentType2.MediaType;
			foreach (DictionaryEntry parameter in contentType2.Parameters)
			{
				m_parameters.Add(parameter.Key as string, parameter.Value as string);
			}
		}
	}
	internal class RDSHSemiSyncEventWatcher
	{
		[Serializable]
		public class WatcherNotReadyException : Exception
		{
			public WatcherNotReadyException()
			{
			}

			protected WatcherNotReadyException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		protected string m_RDSHName;

		protected bool m_connected;

		protected ManagementEventWatcher m_eventWatcher;

		public bool Connected => m_connected;

		public RDSHSemiSyncEventWatcher(string RDSHName)
		{
			m_RDSHName = RDSHName;
			m_connected = false;
			m_eventWatcher = null;
		}

		public void Connect()
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Invalid comparison between Unknown and I4
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			if (m_connected && m_eventWatcher != null)
			{
				return;
			}
			TraceWrite.TraceInfoNoContext("Starting event watcher for: " + m_RDSHName);
			try
			{
				string wmiPathRoot = WmiUtility.GetWmiPathRoot(m_RDSHName);
				ConnectionOptions connectionOptions = WmiUtility.GetConnectionOptions(m_RDSHName);
				ManagementScope val = new ManagementScope(new ManagementPath(wmiPathRoot), connectionOptions);
				val.Connect();
				m_eventWatcher = new ManagementEventWatcher(val, new EventQuery("SELECT * FROM Win32_RemoteAppChangeEvent"));
				((ManagementOptions)m_eventWatcher.Options).Timeout = new TimeSpan(0, 0, 0);
				try
				{
					m_eventWatcher.WaitForNextEvent();
				}
				catch (ManagementException val2)
				{
					if ((int)val2.ErrorCode != 262148)
					{
						throw;
					}
				}
				TraceWrite.TraceVerboseNoContext("Succeeded setting up event watcher for: " + m_RDSHName);
				m_connected = true;
			}
			catch
			{
				TraceWrite.TraceWarningNoContext("Event watcher setup failed for: " + m_RDSHName);
				throw;
			}
		}

		public void Disconnect()
		{
			m_connected = false;
			if (m_eventWatcher != null)
			{
				m_eventWatcher.Stop();
				m_eventWatcher = null;
			}
			TraceWrite.TraceVerboseNoContext("Succeeded stopping the event watcher for: " + m_RDSHName);
		}

		public ManagementBaseObject GetNextEvent()
		{
			//IL_0072: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Invalid comparison between Unknown and I4
			ManagementBaseObject result = null;
			if (!m_connected || m_eventWatcher == null)
			{
				throw new WatcherNotReadyException();
			}
			try
			{
				result = m_eventWatcher.WaitForNextEvent();
			}
			catch (UnauthorizedAccessException ex)
			{
				TraceWrite.TraceWarningNoContext("WMI access denied while waiting for an event: " + ex.Message);
				m_connected = false;
				throw;
			}
			catch (COMException ex2)
			{
				TraceWrite.TraceWarningNoContext("WMI COM error while waiting for an event: " + ex2.Message);
				m_connected = false;
				throw;
			}
			catch (ManagementException val)
			{
				ManagementException val2 = val;
				if ((int)val2.ErrorCode != 262148)
				{
					TraceWrite.TraceWarningNoContext("WMI error while waiting for an event: " + ((Exception)(object)val2).Message);
					m_connected = false;
					throw;
				}
			}
			return result;
		}
	}
	internal class RWSCPubAndTsAccessor : IStoreAccessor
	{
		private string m_CPubName = "";

		private string m_RdshNames = "";

		private bool m_PollAppFreshness = true;

		public bool SupportsWorkspaceReconnect
		{
			get
			{
				if (!string.IsNullOrEmpty(m_CPubName) && string.IsNullOrEmpty(m_RdshNames))
				{
					return true;
				}
				return false;
			}
		}

		public RWSCPubAndTsAccessor(bool PollAppFreshness)
		{
			m_CPubName = RDWebAccessConfig.RadcmServerName;
			m_RdshNames = RDWebAccessConfig.RdshServerNames;
			m_PollAppFreshness = PollAppFreshness;
		}

		public RdpSigningCertificate GetRdpSigningCertificate()
		{
			RdpSigningCertificate rdpSigningCertificate = null;
			if (!string.IsNullOrEmpty(m_CPubName))
			{
				rdpSigningCertificate = ((IStoreAccessor)new RWSCPUBAccessor(m_CPubName, m_PollAppFreshness)).GetRdpSigningCertificate();
			}
			if (rdpSigningCertificate == null)
			{
				TraceWrite.TraceVerboseNoContext("Could not retrieve RDP signing cert from CPub, or no CPub server specified");
				if (!string.IsNullOrEmpty(m_RdshNames))
				{
					rdpSigningCertificate = ((IStoreAccessor)new RWSTerminalServerAccessor(m_RdshNames, m_PollAppFreshness)).GetRdpSigningCertificate();
				}
			}
			if (rdpSigningCertificate == null)
			{
				TraceWrite.TraceWarningNoContext("Could not retrieve RDP signing cert from RDSH, or no CPub or RDSH servers specified");
			}
			return rdpSigningCertificate;
		}

		public WorkspaceInfo GetWorkspaceInfo()
		{
			WorkspaceInfo workspaceInfo = null;
			if (!string.IsNullOrEmpty(m_CPubName))
			{
				workspaceInfo = ((IStoreAccessor)new RWSCPUBAccessor(m_CPubName, m_PollAppFreshness)).GetWorkspaceInfo();
			}
			if (workspaceInfo == null)
			{
				TraceWrite.TraceVerboseNoContext("Could not retrieve workspace info from CPub, or no CPub server specified");
			}
			return workspaceInfo;
		}

		public Task<Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>> GetApplicationsAsync(string strSid, bool onlyShowAvailableByDefaultResources, bool includeRDPFileContents)
		{
			List<AppInfo> list = new List<AppInfo>();
			List<AppInfo> list2 = new List<AppInfo>();
			if (string.IsNullOrEmpty(strSid))
			{
				throw new ArgumentException("Must specify a SID string", "strSid");
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(strSid);
			try
			{
				securityIdentifier.Translate(typeof(NTAccount));
			}
			catch (IdentityNotMappedException)
			{
				TraceWrite.TraceErrorNoContext("Invalid SID - nothing returned");
				return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(new AppInfo[0], new AppInfo[0]));
			}
			if (!string.IsNullOrEmpty(m_CPubName))
			{
				Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>> result = ((IStoreAccessor)new RWSCPUBAccessor(m_CPubName, m_PollAppFreshness)).GetApplicationsAsync(strSid, onlyShowAvailableByDefaultResources, includeRDPFileContents: true).Result;
				IEnumerable<AppInfo> item = result.Item1;
				IEnumerable<AppInfo> item2 = result.Item2;
				if (item != null)
				{
					list.AddRange(item);
				}
				if (item2 != null)
				{
					list2.AddRange(item2);
				}
			}
			else
			{
				TraceWrite.TraceVerboseNoContext("No CPub server specified, skipping CPub app collection");
			}
			if (!string.IsNullOrEmpty(m_RdshNames))
			{
				Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>> result2 = ((IStoreAccessor)new RWSTerminalServerAccessor(m_RdshNames, m_PollAppFreshness)).GetApplicationsAsync(strSid, onlyShowAvailableByDefaultResources, includeRDPFileContents: true).Result;
				IEnumerable<AppInfo> item3 = result2.Item1;
				IEnumerable<AppInfo> item4 = result2.Item2;
				if (item3 != null)
				{
					list.AddRange(item3);
				}
				if (item4 != null)
				{
					list2.AddRange(item4);
				}
			}
			else
			{
				TraceWrite.TraceVerboseNoContext("No RDSH servers specified, skipping RDSH app collection");
			}
			return Task.FromResult(new Tuple<IEnumerable<AppInfo>, IEnumerable<AppInfo>>(list, list2));
		}

		public string GetClaimsHint()
		{
			return "";
		}

		public Task<string> GetPrivacyLinkAsync()
		{
			return Task.FromResult(string.Empty);
		}

		public Task<List<AccountDetailAndConsentStatus>> GetTenantAccountsAndUserConsentStatusAsync(object securityPrincipal)
		{
			return Task.FromResult(new List<AccountDetailAndConsentStatus>());
		}
	}
	public static class PageContentsHelper
	{
		public static WorkspaceInfo GetWorkspaceInfo()
		{
			WorkspaceInfo workspaceInfo = null;
			bool pollAppFreshness = true;
			try
			{
				RapWebService rapWebService = new RapWebService(pollAppFreshness);
				if (rapWebService != null)
				{
					workspaceInfo = rapWebService.GetWorkspaceInfo();
				}
			}
			catch (Exception ex)
			{
				TraceWrite.TraceErrorNoContext("Exception retrieving Workspace Info: " + ex.Message);
			}
			if (workspaceInfo != null)
			{
				TraceWrite.TraceVerboseNoContext("Workspace ID : " + workspaceInfo.WorkspaceId);
				TraceWrite.TraceVerboseNoContext("Workspace Name : " + workspaceInfo.WorkspaceName);
			}
			return workspaceInfo;
		}

		public static string GetRdpSigningCertificateHash()
		{
			string text = "";
			bool pollAppFreshness = true;
			try
			{
				RapWebService rapWebService = new RapWebService(pollAppFreshness);
				if (rapWebService != null)
				{
					RdpSigningCertificate rdpSigningCertificate = rapWebService.GetRdpSigningCertificate();
					if (rdpSigningCertificate != null)
					{
						text = rdpSigningCertificate.wsHash;
					}
				}
			}
			catch (Exception ex)
			{
				TraceWrite.TraceErrorNoContext("Exception retrieving RDP Signing Cert Hash: " + ex.Message);
			}
			TraceWrite.TraceVerboseNoContext("GetRdpSigningCertificateHash returning : " + text);
			return text;
		}

		public static string GetClaimsHint()
		{
			string text = "";
			bool pollAppFreshness = true;
			try
			{
				RapWebService rapWebService = new RapWebService(pollAppFreshness);
				if (rapWebService != null)
				{
					text = rapWebService.GetClaimsHint();
				}
			}
			catch (Exception ex)
			{
				TraceWrite.TraceErrorNoContext("Exception retrieving claims hint: " + ex.Message);
			}
			TraceWrite.TraceVerboseNoContext("GetClaimsHint returning : " + text);
			return text;
		}

		public static async Task<string> GetPrivacyLinkAsync()
		{
			string strPrivacyLink = string.Empty;
			bool pollAppFreshness = true;
			try
			{
				RapWebService rapWebService = new RapWebService(pollAppFreshness);
				if (rapWebService != null)
				{
					strPrivacyLink = await rapWebService.GetPrivacyLinkAsync();
				}
			}
			catch (Exception ex)
			{
				TraceWrite.TraceErrorNoContext("Exception retrieving privacy link: " + ex.Message);
			}
			TraceWrite.TraceVerboseNoContext("GetPrivacyLink returning : " + strPrivacyLink);
			return strPrivacyLink;
		}
	}
	internal class AppFilterer
	{
		internal class SafeFilteringAuthzHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public SafeFilteringAuthzHandle()
				: base(ownsHandle: true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (IsInvalid)
				{
					return true;
				}
				bool result = NativeMethods.TSFinishAccessCheck(handle) == 0;
				handle = IntPtr.Zero;
				return result;
			}
		}

		private static class NativeMethods
		{
			internal const uint TSPUBFILTER_E_FAILED_SID_CONVERSION = 2147746560u;

			internal const uint TSPUBFILTER_E_FAILED_SD_CONVERSION = 2147746561u;

			internal const uint TSPUBFILTER_E_FAILED_CONTEXT_INIT = 2147746562u;

			[DllImport("tspubfilter.dll", CharSet = CharSet.Unicode)]
			internal static extern uint TSSetupAccessCheck(ref SafeFilteringResourceManagerHandle resourceManager);

			[DllImport("tspubfilter.dll", CharSet = CharSet.Unicode)]
			internal static extern uint TSStartAccessCheck(SafeFilteringResourceManagerHandle resourceManager, string userSid, ref SafeFilteringAuthzHandle authzHandle, ref uint hrDetails);

			[DllImport("tspubfilter.dll", CharSet = CharSet.Unicode)]
			internal static extern uint TSCheckAccess(SafeFilteringAuthzHandle authzHandle, string securityDescriptor, ref int isAllowedAccess, ref uint hrDetails);

			[DllImport("tspubfilter.dll", CharSet = CharSet.Unicode)]
			internal static extern uint TSFinishAccessCheck(IntPtr authzHandle);

			[DllImport("tspubfilter.dll", CharSet = CharSet.Unicode)]
			internal static extern uint TSTeardownAccessCheck(IntPtr resourceManager);
		}

		internal class SafeFilteringResourceManagerHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public SafeFilteringResourceManagerHandle()
				: base(ownsHandle: true)
			{
			}

			protected override bool ReleaseHandle()
			{
				if (IsInvalid)
				{
					return true;
				}
				bool result = NativeMethods.TSTeardownAccessCheck(handle) == 0;
				handle = IntPtr.Zero;
				return result;
			}
		}

		private SafeFilteringResourceManagerHandle m_filteringHandle = new SafeFilteringResourceManagerHandle();

		private const string HexStringFormat = "x";

		internal AppFilterer()
		{
			TraceWrite.TraceVerboseNoContext("Setting up access check.");
			uint num = NativeMethods.TSSetupAccessCheck(ref m_filteringHandle);
			if (num != 0)
			{
				TraceWrite.TraceErrorNoContext("NativeMethod call to setup filtering failed. Error: 0x" + num.ToString("x", CultureInfo.InvariantCulture));
			}
		}

		private bool FilterApp(SafeFilteringAuthzHandle authzHandle, AppInfo app, ref Hashtable sdTable)
		{
			int isAllowedAccess = 1;
			uint num = 0u;
			uint hrDetails = 0u;
			TraceWrite.TraceVerboseNoContext(">>>>> " + app.Name + " Checking Access");
			if (sdTable.Contains(app.RemoteAppXml.SecurityDescriptor))
			{
				isAllowedAccess = (int)sdTable[app.RemoteAppXml.SecurityDescriptor];
				TraceWrite.TraceVerboseNoContext("Lookup returned allow=" + isAllowedAccess.ToString(CultureInfo.InvariantCulture) + ", SD=" + app.RemoteAppXml.SecurityDescriptor);
			}
			else if (string.IsNullOrEmpty(app.RemoteAppXml.SecurityDescriptor))
			{
				isAllowedAccess = 1;
				sdTable.Add(app.RemoteAppXml.SecurityDescriptor, isAllowedAccess);
				TraceWrite.TraceVerboseNoContext("NULL SD, allow=1");
			}
			else
			{
				num = NativeMethods.TSCheckAccess(authzHandle, app.RemoteAppXml.SecurityDescriptor, ref isAllowedAccess, ref hrDetails);
				if (-2147220735 == (int)num)
				{
					TraceWrite.TraceErrorNoContext("Error checking access: the resource string Security Descriptor did not convert to a valid Security Descriptor. SD: " + app.RemoteAppXml.SecurityDescriptor + ", Error: 0x" + hrDetails.ToString("x", CultureInfo.InvariantCulture));
					isAllowedAccess = 1;
				}
				else if (num != 0)
				{
					TraceWrite.TraceErrorNoContext("Error calling NativeMethod to check access: Error: 0x" + num.ToString("x", CultureInfo.InvariantCulture));
					isAllowedAccess = 1;
				}
				else
				{
					sdTable.Add(app.RemoteAppXml.SecurityDescriptor, isAllowedAccess);
					TraceWrite.TraceVerboseNoContext("TSCheckAccess returned allow=" + isAllowedAccess.ToString(CultureInfo.InvariantCulture) + ", SD=" + app.RemoteAppXml.SecurityDescriptor);
				}
			}
			return isAllowedAccess == 1;
		}

		internal void FilterApplications(AppInfo[] unfilteredAppsIn, AppInfo[] unfilteredDesktopsIn, string strSid, out AppInfo[] filteredApps, out AppInfo[] filteredDesktops)
		{
			AppInfo[] array = ((unfilteredAppsIn == null) ? new AppInfo[0] : unfilteredAppsIn);
			AppInfo[] array2 = ((unfilteredDesktopsIn == null) ? new AppInfo[0] : unfilteredDesktopsIn);
			List<AppInfo> list = new List<AppInfo>(array.Length);
			List<AppInfo> list2 = new List<AppInfo>(array2.Length);
			Hashtable sdTable = new Hashtable();
			uint num = 0u;
			uint hrDetails = 0u;
			SafeFilteringAuthzHandle authzHandle = new SafeFilteringAuthzHandle();
			TraceWrite.TraceVerboseNoContext("Filtering apps for SID: " + strSid);
			num = NativeMethods.TSStartAccessCheck(m_filteringHandle, strSid, ref authzHandle, ref hrDetails);
			if (-2147220736 == (int)num)
			{
				TraceWrite.TraceErrorNoContext("Error starting filtering: the user SID did not convert to binary SID. SID: " + strSid + ", Error: 0x" + hrDetails.ToString("x", CultureInfo.InvariantCulture));
			}
			else if (-2147220734 == (int)num)
			{
				TraceWrite.TraceErrorNoContext("Error starting filtering: Could not initialize context from SID. SID: " + strSid + ", Error: 0x" + hrDetails.ToString("x", CultureInfo.InvariantCulture));
			}
			if (num != 0)
			{
				TraceWrite.TraceErrorNoContext("Error calling NativeMethod to start filtering: Error: 0x" + num.ToString("x", CultureInfo.InvariantCulture));
				AppInfo[] array3 = array;
				foreach (AppInfo item in array3)
				{
					list.Add(item);
				}
				array3 = array2;
				foreach (AppInfo item2 in array3)
				{
					list2.Add(item2);
				}
			}
			else
			{
				AppInfo[] array3 = array;
				foreach (AppInfo appInfo in array3)
				{
					if (FilterApp(authzHandle, appInfo, ref sdTable))
					{
						list.Add(appInfo);
						TraceWrite.TraceVerboseNoContext("ALLOWED access to app " + appInfo.Name);
					}
					else
					{
						TraceWrite.TraceInfoNoContext("DENIED access to app " + appInfo.Name);
					}
				}
				array3 = array2;
				foreach (AppInfo appInfo2 in array3)
				{
					if (FilterApp(authzHandle, appInfo2, ref sdTable))
					{
						list2.Add(appInfo2);
						TraceWrite.TraceVerboseNoContext("ALLOWED access to desktop " + appInfo2.Name);
					}
					else
					{
						TraceWrite.TraceInfoNoContext("DENIED access to desktop " + appInfo2.Name);
					}
				}
			}
			sdTable.Clear();
			authzHandle.Close();
			filteredApps = list.ToArray();
			filteredDesktops = list2.ToArray();
		}
	}
	public static class RequestHelper
	{
		public static string GetRequestApplicationPath(HttpRequest request)
		{
			Uri originalRequestUri = GetOriginalRequestUri(request);
			StringBuilder stringBuilder = new StringBuilder();
			string[] segments = originalRequestUri.Segments;
			foreach (string text in segments)
			{
				stringBuilder.Append(text);
				if (text.ToLowerInvariant().Equals("rdweb/"))
				{
					break;
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetRequestDirectoryPath(HttpRequest request)
		{
			Uri originalRequestUri = GetOriginalRequestUri(request);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string[] segments = originalRequestUri.Segments;
			foreach (string text in segments)
			{
				stringBuilder.Append(text);
				if (flag)
				{
					break;
				}
				if (text.ToLowerInvariant().Equals("rdweb/"))
				{
					flag = true;
				}
			}
			return stringBuilder.ToString();
		}

		public static Uri GetOriginalRequestUri(HttpRequest request)
		{
			TraceWrite.TraceVerboseNoContext("Request.RawUrl: {0}", request.RawUrl);
			return new Uri(request.Url.Scheme + Uri.SchemeDelimiter + request.Headers["Host"] + request.RawUrl);
		}

		public static string GetRequestFilePath(HttpRequest request)
		{
			Uri originalRequestUri = GetOriginalRequestUri(request);
			string text;
			if (string.IsNullOrEmpty(request.PathInfo))
			{
				text = originalRequestUri.AbsolutePath;
			}
			else
			{
				string text2 = HttpUtility.UrlPathEncode(request.PathInfo);
				TraceWrite.TraceVerboseNoContext("pathInfo: {0}", text2);
				int num = originalRequestUri.AbsolutePath.Length - text2.Length;
				if (num <= 0)
				{
					TraceWrite.TraceErrorNoContext("Unexpected state: request.PathInfo is [{0}], but the original requested URL's absolute path [{1}] does not seem long enough to contain it", request.PathInfo, originalRequestUri.AbsolutePath);
					throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "Unexpected state: request.PathInfo is [{0}], but the original requested URL's absolute path [{1}] does not seem long enough to contain it", new object[2] { request.PathInfo, originalRequestUri.AbsolutePath }));
				}
				if (!originalRequestUri.AbsolutePath.Substring(num).Equals(text2, StringComparison.Ordinal))
				{
					throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "Unexpected state: the original requested URL's absolute path [{0}] does not end in PathInfo [{1}]", new object[2] { originalRequestUri.AbsolutePath, request.PathInfo }));
				}
				text = originalRequestUri.AbsolutePath.Substring(0, num);
			}
			TraceWrite.TraceVerboseNoContext("filepath: {0}", text);
			return text;
		}
	}
	public abstract class WebFeedHandler : HttpTaskAsyncHandler
	{
		private TraceWrite m_trace;

		public override bool IsReusable => false;

		public WebFeedHandler()
		{
			m_trace = new TraceWrite();
			m_trace.Context = "WebFeedHandler: ";
		}

		public override async Task ProcessRequestAsync(HttpContext context)
		{
			m_trace.TraceVerbose("Serving request for WebFeed.aspx");
			HttpStatusCode statusCode = HttpStatusCode.OK;
			if (context == null)
			{
				throw new ArgumentException("The HttpContext cannot be null", "context");
			}
			string userIdentity = GetUserIdentity(context);
			context.Response.AppendHeader("Vary", "Accept");
			ContentNegotiationHelper.GetPreferredXmlVersion(out var xmlVersion, out var contentType);
			if (xmlVersion == FeedXmlVersion.Win7 && context.Request.PathInfo.Length != 0)
			{
				statusCode = HttpStatusCode.NotFound;
			}
			else
			{
				context.Response.ContentType = contentType;
				string text = context.Request.Params.Get("rdpRender");
				bool flag = !string.IsNullOrEmpty(text) && text.Trim().ToLower(CultureInfo.InvariantCulture).Equals("inline");
				try
				{
					WebFeed webFeed = new WebFeed(flag ? RdpType.Both : RdpType.Link, onlyShowAvailableByDefaultResources: false);
					if (!context.Response.IsClientConnected)
					{
						m_trace.TraceInfo("Exiting early, we detected that the client has cancelled the request");
						return;
					}
					Tuple<string, int> obj = await webFeed.GenerateFeedAsync(userIdentity, xmlVersion, context.Request.PathInfo, writeXmlDecl: true);
					string item = obj.Item1;
					_ = obj.Item2;
					if (!context.Response.IsClientConnected)
					{
						m_trace.TraceInfo("Exiting early, we detected that the client has cancelled the request");
						return;
					}
					context.Response.ContentEncoding = new UTF8Encoding();
					context.Response.Write(item);
				}
				catch (ResourceNotModifiedException)
				{
					m_trace.TraceInfo("Sending HttpStatusCode.NotModified status back to client");
					statusCode = HttpStatusCode.NotModified;
				}
				catch (WorkspaceUnknownFolderException)
				{
					statusCode = HttpStatusCode.NotFound;
				}
				catch (InvalidTenantException)
				{
					statusCode = HttpStatusCode.NotFound;
				}
				catch (WorkspaceUnavailableException ex4)
				{
					statusCode = HttpStatusCode.ServiceUnavailable;
					context.Response.Write("<Error>" + ex4.Message + "</Error>");
				}
			}
			context.Response.StatusCode = (int)statusCode;
			try
			{
				context.Response.Flush();
			}
			catch (HttpException val)
			{
				HttpException val2 = val;
				m_trace.TraceWarning("Error flushing the response to the client: " + ((object)val2).ToString());
			}
		}

		protected abstract string GetUserIdentity(HttpContext context);
	}
	public class PagesWebFeedHandler : WebFeedHandler
	{
		private TraceWrite m_trace;

		private bool m_isFormsAuth;

		public PagesWebFeedHandler()
		{
			m_trace = new TraceWrite();
			m_trace.Context = "PagesWebFeedHandler: ";
		}

		public override async Task ProcessRequestAsync(HttpContext context)
		{
			bool flag = true;
			if (context == null)
			{
				throw new ArgumentException("The HttpContext cannot be null", "context");
			}
			AuthenticationMode val = (AuthenticationMode)0;
			object section = ConfigurationManager.GetSection("system.web/authentication");
			AuthenticationSection val2 = (AuthenticationSection)((section is AuthenticationSection) ? section : null);
			if (val2 != null)
			{
				val = val2.Mode;
			}
			if ((int)val == 3)
			{
				m_isFormsAuth = true;
				if (!context.User.Identity.IsAuthenticated)
				{
					m_trace.TraceVerbose("Detected that we are in the pages vdir & not authenticated, so redirecting to the forms login page");
					Uri uri = new Uri(RequestHelper.GetOriginalRequestUri(context.Request), new Uri(RequestHelper.GetRequestFilePath(context.Request) + "/..", UriKind.Relative));
					context.Response.Redirect(uri.ToString() + "default.aspx?ReturnUrl=../WebFeed.aspx", false);
					context.ApplicationInstance.CompleteRequest();
					flag = false;
				}
			}
			if (flag)
			{
				await base.ProcessRequestAsync(context);
			}
		}

		protected override string GetUserIdentity(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentException("The HttpContext cannot be null", "context");
			}
			if (m_isFormsAuth)
			{
				return new TSFormAuthTicketInfo(HttpContext.Current).UserIdentity;
			}
			if (HttpContext.Current.User.Identity is WindowsIdentity)
			{
				return ((WindowsIdentity)context.User.Identity).User.ToString();
			}
			m_trace.TraceError("Unable to determine user identity, it appears that neither forms auth or windows auth are being used");
			throw new ApplicationException("Unable to determine user identity");
		}
	}
	public class FeedWebFeedHandler : WebFeedHandler
	{
		private TraceWrite m_trace;

		public FeedWebFeedHandler()
		{
			m_trace = new TraceWrite();
			m_trace.Context = "FeedWebFeedHandler: ";
		}

		protected override string GetUserIdentity(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentException("The HttpContext cannot be null", "context");
			}
			return context.User.Identity.Name;
		}
	}
}
namespace Microsoft.TerminalServices.Publishing.Portal.FormAuthentication
{
	public abstract class TSFormsAuthentication : IHttpModule
	{
		private string _strDomainUserName = "";

		private string _strPassword = "";

		private string _strNewPassword = "";

		private string _strConfirmNewPassword = "";

		private string _strUserIdentity = "";

		private bool _bPrivateMode;

		private int _iSessionTimeoutInMinutes;

		private bool _bPostRequestType;

		private string _strWorkspaceId = "";

		private string strRequestPageName;

		public const string strDomainUserNameValue = "DomainUserName";

		protected const string strHttpMethodPost = "POST";

		protected const string strLoginFormPageName = "login.aspx";

		protected const string strLogoffPageName = "logoff.aspx";

		protected const string strDefaultPageName = "default.aspx";

		protected const string strPasswordChangePageName = "password.aspx";

		protected const string strTSWAAuthHttpOnlyCookie = "TSWAAuthHttpOnlyCookie";

		protected const string strTSWAAuthClientSideCookie = "TSWAAuthClientSideCookie";

		protected const string strTSWATestCookie = "TSWAFeatureCheckCookie";

		protected const int iTSWAAuthVersion = 2;

		protected readonly int iPublicModeSessionTimeoutInMinutes;

		protected readonly int iPrivateModeSessionTimeoutInMinutes;

		private FormsAuthenticationModule objDefaultFormsAuthenticationModule;

		private MethodInfo _OnAuthenticateRequest;

		private MethodInfo _OnEndRequest;

		public string DomainUserName
		{
			get
			{
				return _strDomainUserName;
			}
			set
			{
				_strDomainUserName = value;
			}
		}

		public string Password
		{
			get
			{
				return _strPassword;
			}
			set
			{
				_strPassword = value;
			}
		}

		public string NewPassword
		{
			get
			{
				return _strNewPassword;
			}
			set
			{
				_strNewPassword = value;
			}
		}

		public string ConfirmNewPassword
		{
			get
			{
				return _strConfirmNewPassword;
			}
			set
			{
				_strConfirmNewPassword = value;
			}
		}

		public string UserIdentity
		{
			get
			{
				return _strUserIdentity;
			}
			set
			{
				_strUserIdentity = value;
			}
		}

		public bool PrivateMode
		{
			get
			{
				return _bPrivateMode;
			}
			set
			{
				_bPrivateMode = value;
				if (_bPrivateMode)
				{
					_iSessionTimeoutInMinutes = iPrivateModeSessionTimeoutInMinutes;
				}
				else
				{
					_iSessionTimeoutInMinutes = iPublicModeSessionTimeoutInMinutes;
				}
			}
		}

		public int SessionTimeoutInMinutes => _iSessionTimeoutInMinutes;

		public bool PostRequestType
		{
			get
			{
				return _bPostRequestType;
			}
			set
			{
				_bPostRequestType = value;
			}
		}

		public string WorkspaceId
		{
			get
			{
				return _strWorkspaceId;
			}
			set
			{
				_strWorkspaceId = value;
			}
		}

		public string ClaimsToken { get; set; }

		public TSFormsAuthentication()
		{
			NumberFormatInfo provider = new NumberFormatInfo
			{
				NegativeSign = "-"
			};
			try
			{
				string s = ConfigurationManager.AppSettings["PublicModeSessionTimeoutInMinutes"].ToString();
				iPublicModeSessionTimeoutInMinutes = int.Parse(s, provider);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception Message : {0}.", ex.Message);
				iPublicModeSessionTimeoutInMinutes = 20;
			}
			try
			{
				string s2 = ConfigurationManager.AppSettings["PrivateModeSessionTimeoutInMinutes"].ToString();
				iPrivateModeSessionTimeoutInMinutes = int.Parse(s2, provider);
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Exception Message : {0}.", ex2.Message);
				iPrivateModeSessionTimeoutInMinutes = 240;
			}
		}

		public void Dispose()
		{
			if (objDefaultFormsAuthenticationModule != null)
			{
				objDefaultFormsAuthenticationModule.Dispose();
				objDefaultFormsAuthenticationModule = null;
			}
		}

		public void Init(HttpApplication objHttpApplication)
		{
			Init(objHttpApplication, registerForEvents: true);
		}

		public void Init(HttpApplication objHttpApplication, bool registerForEvents)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Expected O, but got Unknown
			if (objHttpApplication == null)
			{
				throw new ArgumentNullException("objHttpApplication");
			}
			objDefaultFormsAuthenticationModule = new FormsAuthenticationModule();
			HttpApplication val = new HttpApplication();
			try
			{
				objDefaultFormsAuthenticationModule.Init(val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			Type type = ((object)objDefaultFormsAuthenticationModule).GetType();
			_OnAuthenticateRequest = type.GetMethod("OnEnter", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(object),
				typeof(EventArgs)
			}, null);
			_OnEndRequest = type.GetMethod("OnLeave", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[2]
			{
				typeof(object),
				typeof(EventArgs)
			}, null);
			if (_OnAuthenticateRequest == null || _OnEndRequest == null)
			{
				throw new Exception("Unable to get all required FormsAuthenticationModule entrypoints using reflection.");
			}
			if (registerForEvents)
			{
				EventHandlerTaskAsyncHelper val2 = new EventHandlerTaskAsyncHelper(new TaskEventHandler(OnAuthenticateRequestAsync));
				objHttpApplication.AddOnAuthenticateRequestAsync(val2.BeginEventHandler, val2.EndEventHandler);
				objHttpApplication.EndRequest += OnEndRequest;
			}
		}

		private bool ForceCookiesExpiryInResponse(HttpContext objHttpContext)
		{
			if (!PrivateMode)
			{
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].Expires = DateTime.Now.AddDays(-1000.0);
			}
			else
			{
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["Name"] = GlobalObject.encodeURIComponent((object)DomainUserName);
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["MachineType"] = "private";
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].Expires = DateTime.Now.AddDays(1000.0);
			}
			if (objHttpContext.Response.Cookies["TSWAAuthHttpOnlyCookie"] != null)
			{
				objHttpContext.Response.Cookies["TSWAAuthHttpOnlyCookie"].Expires = DateTime.Now.AddDays(-1000.0);
			}
			FormsAuthentication.SignOut();
			return true;
		}

		protected void RedirectBackWithErrorValue(HttpContext objHttpContext, string strDestPage, string strErrorMessage)
		{
			string absoluteUri = new Uri(new Uri(RequestHelper.GetOriginalRequestUri(objHttpContext.Request), RequestHelper.GetRequestFilePath(objHttpContext.Request)), ".").AbsoluteUri;
			string text = ((!string.IsNullOrEmpty(objHttpContext.Request.Url.Query)) ? (absoluteUri + strDestPage + objHttpContext.Request.Url.Query + "&Error=" + strErrorMessage) : (absoluteUri + strDestPage + "?Error=" + strErrorMessage));
			objHttpContext.Response.Redirect(text);
		}

		private bool CreateClientSideCookieInResponse(HttpContext objHttpContext)
		{
			bool result = true;
			try
			{
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].HttpOnly = false;
				if (objHttpContext.Request.IsSecureConnection)
				{
					objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].Secure = true;
				}
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["Name"] = GlobalObject.encodeURIComponent((object)DomainUserName);
				if (PrivateMode)
				{
					objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["MachineType"] = "private";
					objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].Expires = DateTime.Now.AddDays(1000.0);
				}
				else
				{
					objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["MachineType"] = "public";
				}
				objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"]["WorkSpaceID"] = GlobalObject.encodeURIComponent((object)WorkspaceId);
				TraceWrite.TraceInfoNoContext("CreateClientSideCookieInResponse - DomainUserName : " + TraceWrite.TracePII(DomainUserName) + ".");
				TraceWrite.TraceInfoNoContext("CreateClientSideCookieInResponse - WorkspaceId : " + WorkspaceId + ".");
				TraceWrite.TraceInfoNoContext("CreateClientSideCookieInResponse - objHttpContext.Response.Cookies[strTSWAAuthClientSideCookie].Value : " + objHttpContext.Response.Cookies["TSWAAuthClientSideCookie"].Value + ".");
			}
			catch (Exception ex)
			{
				TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ex.Message);
				result = false;
			}
			return result;
		}

		private bool CreateAuthCookieInResponse(HttpContext objHttpContext, string strAuthCookieValue)
		{
			bool result = true;
			try
			{
				objHttpContext.Response.Cookies["TSWAAuthHttpOnlyCookie"].HttpOnly = true;
				if (objHttpContext.Request.IsSecureConnection)
				{
					objHttpContext.Response.Cookies["TSWAAuthHttpOnlyCookie"].Secure = true;
				}
				objHttpContext.Response.Cookies["TSWAAuthHttpOnlyCookie"].Value = strAuthCookieValue;
			}
			catch (Exception ex)
			{
				TraceWrite.TraceWarningNoContext("Exception Message : {0}.", ex.Message);
				result = false;
			}
			return result;
		}

		private void CreateTestCookieInResponse(HttpContext objHttpContext)
		{
			objHttpContext.Response.Cookies["TSWAFeatureCheckCookie"].Value = "true";
			objHttpContext.Response.Cookies["TSWAFeatureCheckCookie"].Path = RequestHelper.GetRequestApplicationPath(objHttpContext.Request);
		}

		protected bool ExtractInfoFromForm(HttpContext objHttpContext)
		{
			bool result = false;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			NameValueCollection form = objHttpContext.Request.Form;
			string[] allKeys = form.AllKeys;
			for (int i = 0; i < allKeys.Length; i++)
			{
				if (allKeys[i] == null)
				{
					continue;
				}
				if (allKeys[i].Equals("DomainUserName", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					string text = values[0];
					if (!string.IsNullOrEmpty(text))
					{
						DomainUserName = text;
						flag = true;
					}
				}
				else if (allKeys[i].Equals("UserPass", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					Password = values[0];
					if (!string.IsNullOrEmpty(Password))
					{
						flag2 = true;
					}
				}
				else if (allKeys[i].Equals("NewUserPass", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					NewPassword = values[0];
				}
				else if (allKeys[i].Equals("ConfirmNewUserPass", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					ConfirmNewPassword = values[0];
				}
				else if (allKeys[i].Equals("MachineType", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					string text2 = values[0];
					if (!string.IsNullOrEmpty(text2) && text2.Equals("private", StringComparison.OrdinalIgnoreCase))
					{
						PrivateMode = true;
					}
				}
				else if (allKeys[i].Equals("WorkSpaceID", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					WorkspaceId = values[0];
				}
				else if (allKeys[i].Equals("ClaimsToken", StringComparison.OrdinalIgnoreCase))
				{
					string[] values = form.GetValues(allKeys[i]);
					ClaimsToken = values[0];
					flag3 = !string.IsNullOrEmpty(ClaimsToken);
				}
			}
			if (flag && (flag2 || flag3))
			{
				result = true;
			}
			TraceWrite.TraceInfoNoContext("ExtractInfoFromForm returning : " + result + ".");
			return result;
		}

		protected virtual void AddExtraInfoToAuthCookieUserData(HttpContext context, Dictionary<string, string> userData)
		{
		}

		protected virtual bool IsAuthCookieUserDataValid(HttpContext context, Dictionary<string, string> userData)
		{
			return true;
		}

		private bool ExtractInfoFromCookies(HttpContext objHttpContext)
		{
			//IL_016e: Expected O, but got Unknown
			bool result = false;
			bool flag = false;
			if (objHttpContext.Request.Cookies.AllKeys.Length != 0)
			{
				HttpCookie val = objHttpContext.Request.Cookies["TSWAAuthHttpOnlyCookie"];
				if (val != null)
				{
					FormsAuthenticationTicket val2 = null;
					try
					{
						val2 = FormsAuthentication.Decrypt(val.Value);
					}
					catch (ArgumentException ex)
					{
						TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ex.Message);
					}
					catch (CryptographicException ex2)
					{
						TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ex2.Message);
						if (!strRequestPageName.Equals("login.aspx", StringComparison.OrdinalIgnoreCase))
						{
							Uri uri = new Uri(RequestHelper.GetOriginalRequestUri(objHttpContext.Request), RequestHelper.GetRequestFilePath(objHttpContext.Request));
							string text = uri.AbsoluteUri.Substring(0, uri.AbsoluteUri.LastIndexOf('/') + 1);
							string text2 = ((!string.IsNullOrEmpty(objHttpContext.Request.Url.Query)) ? (text + "login.aspx" + objHttpContext.Request.Url.Query + "&Error=ServerConfigChanged") : (text + "login.aspx?Error=ServerConfigChanged"));
							TraceWrite.TraceInfoNoContext("objHttpContext.Request.Url.AbsoluteUri : {0}.", objHttpContext.Request.Url.AbsoluteUri);
							TraceWrite.TraceInfoNoContext("objHttpContext.Response.RedirectLocation : {0}.", text2);
							objHttpContext.Response.RedirectLocation = text2;
						}
					}
					catch (HttpException val3)
					{
						HttpException val4 = val3;
						TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ((Exception)(object)val4).Message);
						if (!strRequestPageName.Equals("login.aspx", StringComparison.OrdinalIgnoreCase))
						{
							Uri uri2 = new Uri(RequestHelper.GetOriginalRequestUri(objHttpContext.Request), RequestHelper.GetRequestFilePath(objHttpContext.Request));
							string text3 = uri2.AbsoluteUri.Substring(0, uri2.AbsoluteUri.LastIndexOf('/') + 1);
							string text4 = ((!string.IsNullOrEmpty(objHttpContext.Request.Url.Query)) ? (text3 + "login.aspx" + objHttpContext.Request.Url.Query + "&Error=ServerConfigChanged") : (text3 + "login.aspx?Error=ServerConfigChanged"));
							TraceWrite.TraceInfoNoContext("objHttpContext.Request.Url.AbsoluteUri : {0}.", objHttpContext.Request.Url.AbsoluteUri);
							TraceWrite.TraceInfoNoContext("objHttpContext.Response.RedirectLocation : {0}.", text4);
							objHttpContext.Response.RedirectLocation = text4;
						}
					}
					if (val2 != null)
					{
						TraceWrite.TraceInfoNoContext("objFormsAuthenticationTicket.UserData : {0}.", val2.UserData);
						if (val2.Expiration >= DateTime.Now && !string.IsNullOrEmpty(val2.UserData))
						{
							Dictionary<string, string> dictionary = TSFormsAuthUserDataHelper.FromUserDataString(val2.UserData);
							if (IsAuthCookieUserDataValid(objHttpContext, dictionary))
							{
								DomainUserName = dictionary["DomainUserName"];
								UserIdentity = val2.Name;
								flag = true;
								result = true;
								TraceWrite.TraceInfoNoContext("DomainUserName : {0}.", TraceWrite.TracePII(DomainUserName));
								TraceWrite.TraceInfoNoContext("UserIdentity : {0}.", TraceWrite.TracePII(UserIdentity));
							}
							else
							{
								TraceWrite.TraceErrorNoContext("Not using the auth cookie because we detected it is invalid");
								ForceCookiesExpiryInResponse(objHttpContext);
								RedirectBackWithErrorValue(objHttpContext, "default.aspx", "UnauthorizedAccessOverride");
							}
						}
					}
				}
				val = objHttpContext.Request.Cookies["TSWAAuthClientSideCookie"];
				if (val != null)
				{
					if (!string.IsNullOrEmpty(val["WorkSpaceID"]))
					{
						WorkspaceId = val["WorkSpaceID"];
					}
					if (!string.IsNullOrEmpty(val["MachineType"]) && val["MachineType"].Equals("private", StringComparison.OrdinalIgnoreCase))
					{
						PrivateMode = true;
					}
					if (!flag && !string.IsNullOrEmpty(val["Name"]))
					{
						DomainUserName = val["Name"];
					}
				}
			}
			else
			{
				result = false;
			}
			TraceWrite.TraceInfoNoContext("ExtractInfoFromCookies returning : " + result + ".");
			return result;
		}

		protected abstract Task<bool> AuthenticateUserAsync(HttpContext objHttpContext, string strDomainUserName, string strPassword, string strClaimsToken);

		private async Task<bool> SetupHttpContextUserAsync(HttpContext objHttpContext)
		{
			bool flag = true;
			if (string.IsNullOrEmpty(DomainUserName))
			{
				flag = false;
			}
			else
			{
				if (PostRequestType)
				{
					flag = await AuthenticateUserAsync(objHttpContext, DomainUserName, Password, ClaimsToken);
				}
				if (flag)
				{
					string strAuthCookieValue = "";
					DateTime now = DateTime.Now;
					DateTime dateTime = now.AddMinutes(SessionTimeoutInMinutes);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("DomainUserName", DomainUserName);
					AddExtraInfoToAuthCookieUserData(objHttpContext, dictionary);
					FormsAuthenticationTicket val = new FormsAuthenticationTicket(2, UserIdentity, now, dateTime, false, TSFormsAuthUserDataHelper.ToUserDataString(dictionary));
					try
					{
						strAuthCookieValue = FormsAuthentication.Encrypt(val);
					}
					catch (ArgumentNullException ex)
					{
						flag = false;
						TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ex.Message);
					}
					if (flag)
					{
						flag = CreateAuthCookieInResponse(objHttpContext, strAuthCookieValue);
						if (flag)
						{
							GenericPrincipal user = new GenericPrincipal(new GenericIdentity(DomainUserName, "Forms"), null);
							objHttpContext.User = user;
						}
					}
				}
			}
			TraceWrite.TraceInfoNoContext("SetupHttpContextUser returning : " + flag + ".");
			return flag;
		}

		public abstract void DoPasswordChangeRequest(object source);

		public virtual async Task OnAuthenticateRequestAsync(object source, EventArgs e)
		{
			HttpApplication val = (HttpApplication)source;
			if (val == null)
			{
				TraceWrite.TraceErrorNoContext("HttpApplication is Null.");
				throw new ArgumentNullException("source");
			}
			HttpContext objHttpContext = val.Context;
			strRequestPageName = objHttpContext.Request.FilePath.Substring(objHttpContext.Request.FilePath.LastIndexOf('/') + 1);
			DomainUserName = "";
			Password = "";
			UserIdentity = "";
			PrivateMode = false;
			WorkspaceId = "";
			ClaimsToken = "";
			bool flag = objHttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase);
			if (flag && strRequestPageName.Equals("login.aspx", StringComparison.OrdinalIgnoreCase))
			{
				PostRequestType = true;
			}
			else
			{
				PostRequestType = false;
			}
			TraceWrite.TraceVerboseNoContext("Page Requested : {0}, Request Type : {1}.", strRequestPageName, objHttpContext.Request.HttpMethod);
			CreateTestCookieInResponse(objHttpContext);
			if (flag && strRequestPageName.Equals("password.aspx", StringComparison.OrdinalIgnoreCase))
			{
				string text = ConfigurationManager.AppSettings["PasswordChangeEnabled"];
				if (text != null && text.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					DoPasswordChangeRequest(source);
				}
				else
				{
					RedirectBackWithErrorValue(objHttpContext, "login.aspx", "PasswordExpired");
				}
				return;
			}
			if (objHttpContext.User == null)
			{
				bool flag2;
				if (PostRequestType)
				{
					flag2 = ExtractInfoFromForm(objHttpContext);
					CreateClientSideCookieInResponse(objHttpContext);
				}
				else
				{
					flag2 = ExtractInfoFromCookies(objHttpContext);
				}
				TraceWrite.TraceInfoNoContext("Info from Form or Auth Cookie extracted : " + flag2 + ".");
				if (flag2)
				{
					if (strRequestPageName.Equals("logoff.aspx", StringComparison.OrdinalIgnoreCase))
					{
						objHttpContext.Request.Cookies.Clear();
						ForceCookiesExpiryInResponse(objHttpContext);
					}
					else
					{
						if (!objHttpContext.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						bool flag3 = await SetupHttpContextUserAsync(objHttpContext);
						if (!objHttpContext.Response.IsClientConnected)
						{
							TraceWrite.TraceInfoNoContext("Exiting early, we detected that the client has cancelled the request");
							return;
						}
						if (!flag3)
						{
							ForceCookiesExpiryInResponse(objHttpContext);
						}
					}
				}
				else if (PostRequestType && !PrivateMode)
				{
					ForceCookiesExpiryInResponse(objHttpContext);
				}
			}
			_OnAuthenticateRequest.Invoke(objDefaultFormsAuthenticationModule, new object[2] { source, e });
		}

		public void OnEndRequest(object source, EventArgs e)
		{
			_OnEndRequest.Invoke(objDefaultFormsAuthenticationModule, new object[2] { source, e });
		}
	}
	public class TSFormAuthTicketInfo
	{
		private const string strTSWAAuthHttpOnlyCookie = "TSWAAuthHttpOnlyCookie";

		private const string strTSWAAuthClientSideCookie = "TSWAAuthClientSideCookie";

		private readonly FormsAuthenticationTicket _objFormsAuthenticationTicket;

		private readonly HttpCookie _objTSWAAuthClientSideCookie;

		public string UserIdentity
		{
			get
			{
				string result = "";
				if (_objFormsAuthenticationTicket != null)
				{
					result = _objFormsAuthenticationTicket.Name;
				}
				return result;
			}
		}

		public bool PrivateMode
		{
			get
			{
				bool result = false;
				if (_objTSWAAuthClientSideCookie != null && !string.IsNullOrEmpty(_objTSWAAuthClientSideCookie["MachineType"]) && _objTSWAAuthClientSideCookie["MachineType"].Equals("private", StringComparison.OrdinalIgnoreCase))
				{
					result = true;
				}
				return result;
			}
		}

		public string DomainUserName
		{
			get
			{
				string result = "";
				if (_objFormsAuthenticationTicket != null)
				{
					result = TSFormsAuthUserDataHelper.FromUserDataString(_objFormsAuthenticationTicket.UserData)["DomainUserName"];
				}
				return result;
			}
		}

		public TSFormAuthTicketInfo(HttpContext objHttpContext)
		{
			if (objHttpContext != null)
			{
				_objTSWAAuthClientSideCookie = objHttpContext.Request.Cookies["TSWAAuthClientSideCookie"];
				HttpCookie val = objHttpContext.Request.Cookies["TSWAAuthHttpOnlyCookie"];
				try
				{
					_objFormsAuthenticationTicket = FormsAuthentication.Decrypt(val.Value);
					return;
				}
				catch (Exception ex)
				{
					if (ex is ArgumentException || ex is FormatException || ex is HttpException)
					{
						TraceWrite.TraceWarningNoContext(".ASPXAUTH cookie contains an invalid forms auth ticket. Exception: {0}", ex.ToString());
						_objFormsAuthenticationTicket = null;
						return;
					}
					throw;
				}
			}
			_objFormsAuthenticationTicket = null;
			_objTSWAAuthClientSideCookie = null;
		}
	}
	internal static class NativeMethods
	{
		public enum DS_NAME_FLAGS
		{
			DS_NAME_NO_FLAGS = 0,
			DS_NAME_FLAG_SYNTACTICAL_ONLY = 1,
			DS_NAME_FLAG_EVAL_AT_DC = 2,
			DS_NAME_FLAG_GCVERIFY = 4,
			DS_NAME_FLAG_TRUST_REFERRAL = 8
		}

		public enum DS_NAME_FORMAT
		{
			DS_UNKNOWN_NAME = 0,
			DS_FQDN_1779_NAME = 1,
			DS_NT4_ACCOUNT_NAME = 2,
			DS_DISPLAY_NAME = 3,
			DS_UNIQUE_ID_NAME = 6,
			DS_CANONICAL_NAME = 7,
			DS_USER_PRINCIPAL_NAME = 8,
			DS_CANONICAL_NAME_EX = 9,
			DS_SERVICE_PRINCIPAL_NAME = 10,
			DS_SID_OR_SID_HISTORY_NAME = 11,
			DS_DNS_DOMAIN_NAME = 12
		}

		public enum DS_NAME_ERROR
		{
			DS_NAME_NO_ERROR,
			DS_NAME_ERROR_RESOLVING,
			DS_NAME_ERROR_NOT_FOUND,
			DS_NAME_ERROR_NOT_UNIQUE,
			DS_NAME_ERROR_NO_MAPPING,
			DS_NAME_ERROR_DOMAIN_ONLY,
			DS_NAME_ERROR_NO_SYNTACTICAL_MAPPING,
			DS_NAME_ERROR_TRUST_REFERRAL
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct DS_NAME_RESULT_ITEM
		{
			public DS_NAME_ERROR eStatus;

			public string pDomain;

			public string pName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct DS_NAME_RESULT
		{
			public int cItems;

			public IntPtr rItems;
		}

		internal struct GENERIC_MAPPING
		{
			public uint GenericRead;

			public uint GenericWrite;

			public uint GenericExecute;

			public uint GenericAll;
		}

		internal struct LUID
		{
			public uint LowPart;

			public uint HighPart;
		}

		internal struct LUID_AND_ATTRIBUTES
		{
			public LUID Luid;

			public uint Attributes;
		}

		internal struct PRIVILEGE_SET
		{
			public uint PrivilegeCount;

			public uint Control;

			public LUID_AND_ATTRIBUTES Privilege;
		}

		public const int LOGON32_LOGON_NETWORK = 3;

		public const int LOGON32_PROVIDER_DEFAULT = 0;

		public const int READ_CONTROL = 131072;

		public const int STANDARD_RIGHTS_READ = 131072;

		public const int STANDARD_RIGHTS_WRITE = 131072;

		public const int STANDARD_RIGHTS_EXECUTE = 131072;

		public const int SYNCHRONIZE = 1048576;

		public const int STANDARD_RIGHTS_REQUIRED = 983040;

		public const int FILE_READ_ATTRIBUTES = 128;

		public const int FILE_WRITE_ATTRIBUTES = 256;

		public const int FILE_READ_EA = 8;

		public const int FILE_WRITE_EA = 16;

		public const int FILE_READ_DATA = 1;

		public const int FILE_WRITE_DATA = 2;

		public const int FILE_APPEND_DATA = 4;

		public const int FILE_EXECUTE = 32;

		public const int FILE_ALL_ACCESS = 2032127;

		public const int FILE_GENERIC_READ = 1179785;

		public const int FILE_GENERIC_WRITE = 1179926;

		public const int FILE_GENERIC_EXECUTE = 1179808;

		public const int ERROR_PASSWORD_EXPIRED = 1330;

		public const int ERROR_PASSWORD_MUST_CHANGE = 1907;

		public const uint HRESULT_NET_PASSWORD_COMPLEXITY = 2147944645u;

		public const uint HRESULT_BAD_PASSWORD = 2147942486u;

		public const uint HRESULT_LDAP_CONSTRAINT_VIOLATION = 2147950639u;

		public const int ERROR_INVALID_PASSWORD = 86;

		public const int ERROR_WRONG_PASSWORD = 1323;

		public const int ERROR_ILL_FORMED_PASSWORD = 1324;

		public const int ERROR_PASSWORD_RESTRICTION = 1325;

		public const int OWNER_SECURITY_INFORMATION = 1;

		public const int GROUP_SECURITY_INFORMATION = 2;

		public const int DACL_SECURITY_INFORMATION = 4;

		public const int ERROR_INSUFFICIENT_BUFFER = 122;

		public const int PRIVILEGE_SET_SIZE = 20;

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
		public static extern int DsBind(string strDomainControllerName, string strDnsDomainName, out IntPtr piDSContext);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
		public static extern int DsCrackNames(IntPtr piDSContext, DS_NAME_FLAGS eFlags, DS_NAME_FORMAT eFormatOffered, DS_NAME_FORMAT eformatDesired, int iNames, string[] pstrNames, out IntPtr piResult);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
		public static extern void DsFreeNameResult(IntPtr piResult);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
		public static extern int DsUnBind(ref IntPtr piDSContext);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool GetFileSecurity(string FileName, uint SecurityInformation, [Out] byte[] SecurityDescriptorPtr, uint SecurityDescriptorLength, ref uint SecurityDescriptorLengthNeeded);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode)]
		public static extern void MapGenericMask(ref IntPtr AccessMask, ref GENERIC_MAPPING GenericMapping);

		[DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool AccessCheck(byte[] SecurityDescriptorPtr, IntPtr ClientToken, int DesiredAccess, ref GENERIC_MAPPING GenericMapping, out PRIVILEGE_SET PrivilegeSet, ref uint PrivilegeSetLength, out uint GrantedAccess, out bool AccessStatus);

		public static DS_NAME_RESULT_ITEM[] DsCrackNamesWrapper(string strBindingServer, DS_NAME_FLAGS eFlags, DS_NAME_FORMAT eFormatOffered, DS_NAME_FORMAT eFormatDesired, string[] pstrNames)
		{
			IntPtr piDSContext = (IntPtr)0;
			IntPtr piResult = (IntPtr)0;
			try
			{
				int num = DsBind(null, (strBindingServer.Length == 0) ? null : strBindingServer, out piDSContext);
				if (num != 0)
				{
					throw new Exception("DsBind error: " + num.ToString(CultureInfo.InvariantCulture));
				}
				num = DsCrackNames(piDSContext, eFlags, eFormatOffered, eFormatDesired, pstrNames.Length, pstrNames, out piResult);
				if (num != 0)
				{
					throw new Exception("DsCrackNames error: " + num.ToString(CultureInfo.InvariantCulture));
				}
				DS_NAME_RESULT dS_NAME_RESULT = default(DS_NAME_RESULT);
				dS_NAME_RESULT.cItems = Marshal.ReadInt32(piResult);
				dS_NAME_RESULT.rItems = Marshal.ReadIntPtr(piResult, Marshal.OffsetOf(typeof(DS_NAME_RESULT), "rItems").ToInt32());
				IntPtr rItems = dS_NAME_RESULT.rItems;
				DS_NAME_RESULT_ITEM[] array = new DS_NAME_RESULT_ITEM[dS_NAME_RESULT.cItems];
				for (int i = 0; i < dS_NAME_RESULT.cItems; i++)
				{
					array[i] = (DS_NAME_RESULT_ITEM)Marshal.PtrToStructure(rItems, typeof(DS_NAME_RESULT_ITEM));
					rItems += Marshal.SizeOf((object)array[i]);
				}
				return array;
			}
			finally
			{
				if (piResult != (IntPtr)0)
				{
					DsFreeNameResult(piResult);
				}
				if (piDSContext != (IntPtr)0)
				{
					int num = DsUnBind(ref piDSContext);
				}
			}
		}
	}
	public class TSDomainFormsAuthentication : TSFormsAuthentication, IDisposable
	{
		private const string strFailedLogonError = "FailedLogon";

		private const string strFailedGenericError = "FailedGeneric";

		private const string strFailedBlankPasswordError = "FailedBlankPassword";

		private const string strFailedPasswordMatchError = "FailedPasswordMatch";

		private const string strFailedComplexityError = "ComplexityFailed";

		private byte[] SecurityDescriptorPtr;

		private string EscapeLDAPSpecialCharacters(string strUnescapedDN)
		{
			return strUnescapedDN.Replace("/", "\\/");
		}

		private void GetSecurityDescriptor(string strVDirPath)
		{
			if (SecurityDescriptorPtr != null)
			{
				TraceWrite.TraceVerboseNoContext("SecurityDescriptorPtr is not NULL.");
				return;
			}
			if (!Directory.Exists(strVDirPath))
			{
				TraceWrite.TraceVerboseNoContext("File {0} does not exist.", strVDirPath);
				return;
			}
			string text = strVDirPath.Substring(0, strVDirPath.LastIndexOf('\\', strVDirPath.Length - 2) + 1);
			TraceWrite.TraceVerboseNoContext("Virtual Directory Path : {0}, Parent Virtual Directory Path : {1}.", strVDirPath, text);
			uint SecurityDescriptorLengthNeeded = 0u;
			uint securityInformation = 7u;
			NativeMethods.GetFileSecurity(text, securityInformation, null, 0u, ref SecurityDescriptorLengthNeeded);
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 122)
			{
				TraceWrite.TraceInfoNoContext("GetFileSecurity() on {0} failed with error code {1}.", text, lastWin32Error);
				return;
			}
			if (SecurityDescriptorLengthNeeded == 0)
			{
				TraceWrite.TraceInfoNoContext("GetFileSecurity() on {0} return Security Descriptor size == 0.", text);
				return;
			}
			uint num = SecurityDescriptorLengthNeeded;
			SecurityDescriptorPtr = new byte[num];
			if (SecurityDescriptorPtr == null)
			{
				TraceWrite.TraceInfoNoContext("Marshal.AllocHGlobal() failed to allocated {0} bytes for the security descriptor.", num);
			}
			else if (!NativeMethods.GetFileSecurity(text, securityInformation, SecurityDescriptorPtr, num, ref SecurityDescriptorLengthNeeded))
			{
				TraceWrite.TraceInfoNoContext("GetFileSecurity() on {0} failed with error code {1}.", text, Marshal.GetLastWin32Error());
				SecurityDescriptorPtr = null;
			}
		}

		private bool AccessCheck(HttpContext objHttpContext, IntPtr objUserToken, uint iDesiredAccess)
		{
			bool result = false;
			if (objUserToken == IntPtr.Zero)
			{
				TraceWrite.TraceInfoNoContext("objUserToken is Invalid.");
				return result;
			}
			string physicalApplicationPath = objHttpContext.Request.PhysicalApplicationPath;
			if (string.IsNullOrEmpty(physicalApplicationPath))
			{
				TraceWrite.TraceInfoNoContext("strFileName is NULL or Empty.");
				return result;
			}
			if (SecurityDescriptorPtr == null)
			{
				GetSecurityDescriptor(physicalApplicationPath);
			}
			if (SecurityDescriptorPtr == null)
			{
				TraceWrite.TraceInfoNoContext("SecurityDescriptorPtr is NULL.");
				return result;
			}
			NativeMethods.GENERIC_MAPPING GenericMapping = default(NativeMethods.GENERIC_MAPPING);
			GenericMapping.GenericRead = 1179785u;
			GenericMapping.GenericWrite = 1179926u;
			GenericMapping.GenericExecute = 1179808u;
			GenericMapping.GenericAll = 2032127u;
			IntPtr AccessMask = new IntPtr(iDesiredAccess);
			NativeMethods.MapGenericMask(ref AccessMask, ref GenericMapping);
			uint PrivilegeSetLength = 20u;
			NativeMethods.PRIVILEGE_SET PrivilegeSet = default(NativeMethods.PRIVILEGE_SET);
			PrivilegeSet.PrivilegeCount = 0u;
			PrivilegeSet.Control = 0u;
			PrivilegeSet.Privilege.Luid.LowPart = 0u;
			PrivilegeSet.Privilege.Luid.HighPart = 0u;
			PrivilegeSet.Privilege.Attributes = 0u;
			result = NativeMethods.AccessCheck(SecurityDescriptorPtr, objUserToken, AccessMask.ToInt32(), ref GenericMapping, out PrivilegeSet, ref PrivilegeSetLength, out var _, out var AccessStatus);
			if (!result)
			{
				TraceWrite.TraceInfoNoContext("AccessCheck() on failed with error code {0}.", Marshal.GetLastWin32Error());
				return result;
			}
			result = AccessStatus;
			if (!result)
			{
				RedirectBackWithErrorValue(objHttpContext, "login.aspx", "UnauthorizedAccess");
			}
			return result;
		}

		protected override async Task<bool> AuthenticateUserAsync(HttpContext objHttpContext, string strDomainUserName, string strPassword, string strClaimsToken)
		{
			IntPtr phToken = IntPtr.Zero;
			string text = null;
			int num = strDomainUserName.IndexOf('\\');
			string text2;
			if (num != -1 && strDomainUserName.Length != 1)
			{
				text = strDomainUserName.Substring(0, num);
				text2 = strDomainUserName.Substring(num + 1);
			}
			else
			{
				text2 = strDomainUserName;
			}
			bool flag = NativeMethods.LogonUser(text2, text, base.Password, 3, 0, ref phToken);
			int lastWin32Error = Marshal.GetLastWin32Error();
			TraceWrite.TraceInfoNoContext("User Name : {0}, DomainName : {1}, Password : {2}, Private logon : {3}, LogonUser returned : {4}.", string.IsNullOrEmpty(text2) ? "NullOrEmpty" : text2, string.IsNullOrEmpty(text) ? "NullOrEmpty" : text, string.IsNullOrEmpty(base.Password) ? "NullOrEmpty" : "Non-NullOrEmpty", base.PrivateMode, flag);
			if (!flag)
			{
				TraceWrite.TraceInfoNoContext("LogonUser() failed with error code {0}.", lastWin32Error);
				if (lastWin32Error == 1330 || lastWin32Error == 1907)
				{
					string strErrorMessage = "PasswordExpired&UserName=" + HttpUtility.UrlEncode(base.DomainUserName);
					RedirectBackWithErrorValue(objHttpContext, "login.aspx", strErrorMessage);
				}
			}
			else
			{
				flag = AccessCheck(objHttpContext, phToken, 1179785u);
				if (!flag)
				{
					TraceWrite.TraceErrorNoContext("AccessCheck() returned : {0}.", flag);
				}
				else
				{
					try
					{
						WindowsIdentity windowsIdentity = new WindowsIdentity(phToken, "Forms");
						base.UserIdentity = windowsIdentity.User.ToString();
					}
					catch (ArgumentException ex)
					{
						flag = false;
						TraceWrite.TraceErrorNoContext("Exception Message : {0}, Param : {1}.", ex.Message, ex.ParamName);
					}
					catch (SecurityException ex2)
					{
						flag = false;
						TraceWrite.TraceErrorNoContext("Exception Message : {0}.", ex2.Message);
					}
				}
				NativeMethods.CloseHandle(phToken);
			}
			return await Task.FromResult(flag);
		}

		public override void DoPasswordChangeRequest(object source)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Expected O, but got Unknown
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			string text = "";
			_ = new string[0];
			HttpApplication val = (HttpApplication)source;
			if (val == null)
			{
				TraceWrite.TraceErrorNoContext("HttpApplication is Null.");
				throw new ArgumentNullException("source");
			}
			HttpContext context = val.Context;
			ExtractInfoFromForm(context);
			if (string.IsNullOrEmpty(base.DomainUserName) || string.IsNullOrEmpty(base.Password))
			{
				RedirectBackWithErrorValue(context, "password.aspx", "FailedLogon");
				return;
			}
			if (string.IsNullOrEmpty(base.NewPassword))
			{
				RedirectBackWithErrorValue(context, "password.aspx", "FailedBlankPassword");
				return;
			}
			if (string.IsNullOrEmpty(base.ConfirmNewPassword) || string.CompareOrdinal(base.NewPassword, base.ConfirmNewPassword) != 0)
			{
				RedirectBackWithErrorValue(context, "password.aspx", "FailedPasswordMatch");
				return;
			}
			NativeMethods.DS_NAME_FORMAT dS_NAME_FORMAT = NativeMethods.DS_NAME_FORMAT.DS_UNKNOWN_NAME;
			if (base.DomainUserName.IndexOf('@') > 0)
			{
				dS_NAME_FORMAT = NativeMethods.DS_NAME_FORMAT.DS_USER_PRINCIPAL_NAME;
			}
			else
			{
				if (base.DomainUserName.IndexOf('\\') <= 0)
				{
					RedirectBackWithErrorValue(context, "password.aspx", "FailedLogon");
					return;
				}
				dS_NAME_FORMAT = NativeMethods.DS_NAME_FORMAT.DS_NT4_ACCOUNT_NAME;
			}
			NativeMethods.DS_NAME_RESULT_ITEM[] array = new NativeMethods.DS_NAME_RESULT_ITEM[0];
			try
			{
				string strBindingServer = "";
				do
				{
					array = NativeMethods.DsCrackNamesWrapper(strBindingServer, NativeMethods.DS_NAME_FLAGS.DS_NAME_FLAG_TRUST_REFERRAL, dS_NAME_FORMAT, NativeMethods.DS_NAME_FORMAT.DS_FQDN_1779_NAME, new string[1] { base.DomainUserName });
					if (array.Length == 0)
					{
						throw new Exception();
					}
					strBindingServer = array[0].pDomain;
				}
				while (array[0].eStatus == NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_TRUST_REFERRAL);
			}
			catch
			{
				RedirectBackWithErrorValue(context, "password.aspx", "FailedGeneric");
				return;
			}
			switch (array[0].eStatus)
			{
			case NativeMethods.DS_NAME_ERROR.DS_NAME_NO_ERROR:
				text = array[0].pName;
				text = EscapeLDAPSpecialCharacters(text);
				try
				{
					DirectoryEntry val2 = new DirectoryEntry("LDAP://" + text);
					val2.Invoke("ChangePassword", new object[2] { base.Password, base.NewPassword });
					val2.CommitChanges();
					TraceWrite.TraceInfoNoContext("Password change completed for " + base.DomainUserName);
				}
				catch (Exception ex)
				{
					uint num = 0u;
					string strErrorMessage = "FailedGeneric";
					if (ex.InnerException != null && ex.InnerException is COMException)
					{
						COMException ex2 = (COMException)ex.InnerException;
						num = (uint)ex2.ErrorCode;
						TraceWrite.TraceInfoNoContext("Error in changing password. DN: {0}, ErrorCode: {1}, Exception: {2}", text, num, ex2.ToString());
					}
					switch (num)
					{
					case 2147942486u:
						strErrorMessage = "FailedLogon";
						break;
					case 2147944645u:
						strErrorMessage = "ComplexityFailed";
						break;
					case 2147950639u:
						if (ex.InnerException is DirectoryServicesCOMException)
						{
							DirectoryServicesCOMException val3 = (DirectoryServicesCOMException)ex.InnerException;
							TraceWrite.TraceInfoNoContext("Error in changing password. DirectoryServicesCOMException extended error code: {0}, Extended error message: {1}, Exception: {2}", val3.ExtendedError, val3.ExtendedErrorMessage, ((object)val3).ToString());
							switch (val3.ExtendedError)
							{
							case 1324:
							case 1325:
								strErrorMessage = "ComplexityFailed";
								break;
							case 86:
							case 1323:
								strErrorMessage = "FailedLogon";
								break;
							default:
								TraceWrite.TraceInfoNoContext("Unhandled Win32 ExtErrorCode: {0} returned by LDAP. ", val3.ExtendedError);
								break;
							}
						}
						break;
					default:
						TraceWrite.TraceInfoNoContext("Unhandled error in changing password. DN: {0}, Exception: {1}", text, ex.ToString());
						strErrorMessage = "FailedGeneric";
						break;
					}
					RedirectBackWithErrorValue(context, "password.aspx", strErrorMessage);
				}
				RedirectBackWithErrorValue(context, "password.aspx", "PasswordSuccess");
				break;
			case NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_NOT_FOUND:
			case NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_NOT_UNIQUE:
			case NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_NO_MAPPING:
			case NativeMethods.DS_NAME_ERROR.DS_NAME_ERROR_DOMAIN_ONLY:
				RedirectBackWithErrorValue(context, "password.aspx", "FailedLogon");
				break;
			default:
				RedirectBackWithErrorValue(context, "password.aspx", "FailedGeneric");
				break;
			}
		}
	}
	public abstract class FeedAuthModule : IHttpModule
	{
		protected TraceWrite m_trace;

		public const int SERVER2012R2_COOKIE_VERSION = 1;

		public static int CurrentCookieVersion => 1;

		public FeedAuthModule()
		{
			m_trace = new TraceWrite();
			m_trace.Context = "FeedAuthModule: ";
		}

		public void Init(HttpApplication application)
		{
			Init(application, registerForEvents: true);
		}

		public void Init(HttpApplication application, bool registerForEvents)
		{
			if (application == null)
			{
				m_trace.TraceError("Received null application parameter in Init");
			}
			else if (registerForEvents)
			{
				application.AuthenticateRequest += OnAuthenticateRequest;
				application.EndRequest += OnEndRequest;
			}
		}

		public void Dispose()
		{
		}

		public virtual void OnAuthenticateRequest(object source, EventArgs e)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (source == null)
				{
					m_trace.TraceError("Received null source parameter in OnAuthenticateRequest");
					return;
				}
				HttpContext context = ((HttpApplication)source).Context;
				if (context.User != null)
				{
					m_trace.TraceError("In OnAuthenticateRequest, looks like the user is already authenticated as username: " + TraceWrite.TracePII(context.User.Identity.Name));
					return;
				}
				HttpCookie val = context.Request.Cookies[".ASPXAUTH"];
				if (val == null)
				{
					m_trace.TraceVerbose("In OnAuthenticateRequest, no .ASPXAUTH cookie supplied");
					return;
				}
				if (string.IsNullOrEmpty(val.Value))
				{
					m_trace.TraceWarning("In OnAuthenticateRequest, .ASPXAUTH cookie is empty");
					return;
				}
				FormsAuthenticationTicket val2 = null;
				try
				{
					val2 = FormsAuthentication.Decrypt(val.Value);
				}
				catch (Exception ex)
				{
					if (ex is ArgumentException || ex is FormatException || ex is HttpException)
					{
						m_trace.TraceWarning("In OnAuthenticateRequest, .ASPXAUTH cookie contains an invalid forms auth ticket. Exception: {0}", ex.ToString());
						return;
					}
					throw;
				}
				if (val2.Expired)
				{
					m_trace.TraceWarning("In OnAuthenticateRequest, detected that the forms auth ticket is expired");
					return;
				}
				if (string.IsNullOrEmpty(val2.Name))
				{
					m_trace.TraceError("In OnAuthenticateRequest, the forms auth ticket does not contain a name");
					return;
				}
				Dictionary<string, string> userData = TSFormsAuthUserDataHelper.FromUserDataString(val2.UserData);
				if (!IsAuthCookieUserDataValid(context, userData))
				{
					m_trace.TraceError("In OnAuthenticateRequest, the forms auth ticket's UserData is invalid");
					return;
				}
				SetUserDataInContext(context, userData);
				context.User = new GenericPrincipal(new GenericIdentity(val2.Name, "Forms"), null);
				m_trace.TraceVerbose("User successfully authenticated as {0}", TraceWrite.TracePII(val2.Name));
			}
			catch (Exception ex2)
			{
				m_trace.TraceError("Unhandled exception in OnAuthenticateRequest: " + ex2.ToString());
				throw;
			}
		}

		public void OnEndRequest(object source, EventArgs e)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (source == null)
				{
					m_trace.TraceError("Received null source parameter in OnEndRequest");
					return;
				}
				HttpContext context = ((HttpApplication)source).Context;
				if (context.Response.StatusCode == 401)
				{
					m_trace.TraceVerbose("Detected unauthorized access, redirecting to login page");
					string redirectUrl;
					try
					{
						redirectUrl = GetRedirectUrl(context);
					}
					catch (InvalidTenantException ex)
					{
						m_trace.TraceWarning("InvalidTenantException: " + ex.ToString());
						context.Response.StatusCode = 404;
						return;
					}
					context.Response.Redirect(redirectUrl, false);
				}
			}
			catch (Exception ex2)
			{
				m_trace.TraceError("OnEndRequest: Unhandled exception " + ex2.ToString());
				throw;
			}
		}

		protected abstract string GetRedirectUrl(HttpContext context);

		protected virtual bool IsAuthCookieUserDataValid(HttpContext context, Dictionary<string, string> userData)
		{
			return true;
		}

		protected virtual void SetUserDataInContext(HttpContext context, Dictionary<string, string> userData)
		{
		}
	}
	public class DomainFeedAuthModule : FeedAuthModule
	{
		protected override string GetRedirectUrl(HttpContext context)
		{
			return "/RDWeb/FeedLogin/WebFeedLogin.aspx";
		}
	}
	public class WebFeedLoginHandler : IHttpHandler
	{
		private TraceWrite m_trace;

		public bool IsReusable => false;

		public WebFeedLoginHandler()
		{
			m_trace = new TraceWrite();
			m_trace.Context = "WebFeedLoginHandler: ";
		}

		public void ProcessRequest(HttpContext context)
		{
			//IL_012a: Expected O, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			HttpStatusCode statusCode = HttpStatusCode.OK;
			context.Response.ContentType = GetContentType();
			if (context.Request.PathInfo.Length != 0)
			{
				m_trace.TraceVerbose("Denying request with additional path information");
				statusCode = HttpStatusCode.NotFound;
			}
			else
			{
				string text = ((!(HttpContext.Current.User.Identity is WindowsIdentity)) ? context.User.Identity.Name : ((WindowsIdentity)context.User.Identity).User.ToString());
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				GetUserData(context, dictionary);
				string text2 = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(FeedAuthModule.CurrentCookieVersion, text, DateTime.UtcNow, DateTime.MaxValue, true, TSFormsAuthUserDataHelper.ToUserDataString(dictionary), VirtualPathUtility.GetDirectory(context.Request.FilePath)));
				context.Response.Write(text2);
				AppendTicketAsCookieToResponse(context, text2);
				m_trace.TraceVerbose("Successfully created forms auth ticket for user {0}", TraceWrite.TracePII(text));
			}
			context.Response.StatusCode = (int)statusCode;
			try
			{
				context.Response.Flush();
			}
			catch (HttpException val)
			{
				HttpException val2 = val;
				m_trace.TraceWarning("Error flushing the response to the client: " + ((object)val2).ToString());
			}
		}

		protected virtual void GetUserData(HttpContext context, Dictionary<string, string> userData)
		{
		}

		protected virtual void AppendTicketAsCookieToResponse(HttpContext context, string encryptedTicket)
		{
		}

		protected virtual string GetContentType()
		{
			return "application/x-msts-webfeed-login";
		}
	}
	public static class TSFormsAuthUserDataHelper
	{
		public static string ToUserDataString(Dictionary<string, string> userDataDict)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (userDataDict == null)
			{
				return string.Empty;
			}
			using StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			((XmlObjectSerializer)new DataContractSerializer(typeof(Dictionary<string, string>))).WriteObject((XmlWriter)xmlTextWriter, (object)userDataDict);
			return stringWriter.GetStringBuilder().ToString();
		}

		public static Dictionary<string, string> FromUserDataString(string userDataString)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(userDataString))
			{
				return new Dictionary<string, string>();
			}
			using StringReader input = new StringReader(userDataString);
			XmlTextReader xmlTextReader = new XmlTextReader(input);
			xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
			return (Dictionary<string, string>)((XmlObjectSerializer)new DataContractSerializer(typeof(Dictionary<string, string>))).ReadObject((XmlReader)xmlTextReader);
		}
	}
}
namespace Microsoft.TerminalServices.Publishing.Portal.WebFeedConfigHandler
{
	public enum RDWebAuthnContext
	{
		PAGES,
		FEED,
		FEEDLOGIN,
		FEEDDISCOVERY,
		FEEDDISCOVERYLOGIN,
		OTHER
	}
	public static class RDWebAccessConfig
	{
		private const string strRDServer = "ws08r2rdserver";

		private const string strRADCMServer = "radcmserver";

		private const string strCloudMode = "cloudmode";

		public static string RadcmServerName
		{
			get
			{
				string text = GetAppSetting("radcmserver");
				if (text == null)
				{
					text = string.Empty;
				}
				TraceWrite.TraceVerboseNoContext("Radcm Server Name : {0}.", text);
				return text;
			}
		}

		public static string RdshServerNames
		{
			get
			{
				string text = GetAppSetting("ws08r2rdserver");
				if (text == null)
				{
					text = string.Empty;
				}
				TraceWrite.TraceVerboseNoContext("RDSH Server Names : {0}.", text);
				return text;
			}
		}

		public static bool CloudMode
		{
			get
			{
				if (!bool.TryParse(GetAppSetting("cloudmode"), out var result))
				{
					result = false;
				}
				TraceWrite.TraceVerboseNoContext("Cloud Mode: {0}.", result.ToString());
				return result;
			}
		}

		public static string WorkspaceId
		{
			get
			{
				string text = GetAppSetting("WorkspaceID");
				if (text == null)
				{
					text = string.Empty;
				}
				TraceWrite.TraceVerboseNoContext("Workspace ID : {0}.", text);
				return text;
			}
		}

		public static string WorkspaceName
		{
			get
			{
				string text = GetAppSetting("WorkspaceName");
				if (text == null)
				{
					text = LangSpecific.TSPORTAL_DEFAULT_WORKSPACE;
				}
				TraceWrite.TraceVerboseNoContext("Workspace Name : {0}.", text);
				return text;
			}
		}

		public static string WorkspaceDescription
		{
			get
			{
				string text = GetAppSetting("WorkspaceDescription");
				if (text == null)
				{
					text = string.Empty;
				}
				TraceWrite.TraceVerboseNoContext("Workspace Description : {0}.", text);
				return text;
			}
		}

		public static RDWebAuthnContext AuthenticationContext
		{
			get
			{
				RDWebAuthnContext rDWebAuthnContext = RDWebAuthnContext.OTHER;
				string appSetting = GetAppSetting("AuthenticationContext");
				if (!string.IsNullOrEmpty(appSetting))
				{
					if (appSetting.Equals("pages", StringComparison.OrdinalIgnoreCase))
					{
						rDWebAuthnContext = RDWebAuthnContext.PAGES;
					}
					else if (appSetting.Equals("feedlogin", StringComparison.OrdinalIgnoreCase))
					{
						rDWebAuthnContext = RDWebAuthnContext.FEEDLOGIN;
					}
					else if (appSetting.Equals("feed", StringComparison.OrdinalIgnoreCase))
					{
						rDWebAuthnContext = RDWebAuthnContext.FEED;
					}
					else if (appSetting.Equals("feeddiscoverylogin", StringComparison.OrdinalIgnoreCase))
					{
						rDWebAuthnContext = RDWebAuthnContext.FEEDDISCOVERYLOGIN;
					}
					else if (appSetting.Equals("feeddiscovery", StringComparison.OrdinalIgnoreCase))
					{
						rDWebAuthnContext = RDWebAuthnContext.FEEDDISCOVERY;
					}
				}
				TraceWrite.TraceVerboseNoContext("Authentication Context : {0}.", rDWebAuthnContext);
				return rDWebAuthnContext;
			}
		}

		private static string GetAppSetting(string name)
		{
			//IL_0011: Expected O, but got Unknown
			string result = null;
			try
			{
				result = ConfigurationManager.AppSettings[name];
			}
			catch (ConfigurationErrorsException val)
			{
				ConfigurationErrorsException val2 = val;
				TraceWrite.TraceErrorNoContext("Could not retrieve the {0} configuration: {1}", name, ((Exception)(object)val2).Message);
			}
			return result;
		}
	}
	public class WebFeedConfig : ConfigurationSection
	{
		[ConfigurationProperty("IconSettings")]
		public IconSettingsElement IconSettings => (IconSettingsElement)((ConfigurationElement)this)["IconSettings"];
	}
	public class IconSettingsElement : ConfigurationElement
	{
		public enum DisplayType
		{
			Inline,
			Link
		}

		public enum IconImageFormat
		{
			Png
		}

		[ConfigurationProperty("Display", DefaultValue = DisplayType.Link, IsRequired = false)]
		public DisplayType Display => (DisplayType)((ConfigurationElement)this)["Display"];

		[ConfigurationProperty("Format", DefaultValue = IconImageFormat.Png, IsRequired = false)]
		public IconImageFormat Format => (IconImageFormat)((ConfigurationElement)this)["Format"];

		public ImageFormat FormatObject
		{
			get
			{
				IconImageFormat format = Format;
				return ImageFormat.Png;
			}
		}

		[ConfigurationProperty("IconDimensions")]
		public IconDimensionsElementCollection IconDimensions => ((ConfigurationElement)this)["IconDimensions"] as IconDimensionsElementCollection;
	}
	public class IconDimensionsElementCollection : ConfigurationElementCollection, IEnumerable<IconDimensionsElement>, IEnumerable
	{
		public IconDimensionsElement this[int index] => ((ConfigurationElementCollection)this).BaseGet(index) as IconDimensionsElement;

		public IEnumerator<IconDimensionsElement> GetEnumerator()
		{
			using IEnumerator<IconDimensionsElement> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return (ConfigurationElement)(object)new IconDimensionsElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((IconDimensionsElement)(object)element).Dim.ToString(CultureInfo.InvariantCulture);
		}
	}
	public class IconDimensionsElement : ConfigurationElement
	{
		private static int[] allowed_dims = new int[6] { 16, 32, 48, 64, 100, 256 };

		[ConfigurationProperty("Dim", DefaultValue = 32, IsRequired = true)]
		[CallbackValidator(CallbackMethodName = "ValidateDimCallback", Type = typeof(IconDimensionsElement))]
		public int Dim => (int)((ConfigurationElement)this)["Dim"];

		public static void ValidateDimCallback(object value)
		{
			int num = (int)value;
			bool flag = false;
			int[] array = allowed_dims;
			foreach (int num2 in array)
			{
				if (num == num2)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException("The Dim must be one of: 16, 32, 48, 64, 100, or 256");
			}
		}
	}
	internal static class NativeMethods
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RevertToSelf();

		[DllImport("advapi32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
	}
}
namespace Microsoft.TerminalServices.Publishing.Portal.Resources
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class LangSpecific
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Microsoft.TerminalServices.Publishing.Portal.Resources", typeof(LangSpecific).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string ADUtilExceptionMessage => ResourceManager.GetString("ADUtilExceptionMessage", resourceCulture);

		internal static string TSPORTAL_ERROR_1 => ResourceManager.GetString("TSPORTAL_ERROR_1", resourceCulture);

		internal static string TSPORTAL_ERROR_2 => ResourceManager.GetString("TSPORTAL_ERROR_2", resourceCulture);

		internal static string TSPORTAL_ERROR_3 => ResourceManager.GetString("TSPORTAL_ERROR_3", resourceCulture);

		internal static string TSPORTAL_ERROR_4 => ResourceManager.GetString("TSPORTAL_ERROR_4", resourceCulture);

		internal static string TSPORTAL_ERROR_5 => ResourceManager.GetString("TSPORTAL_ERROR_5", resourceCulture);

		internal static string TSPORTAL_ERROR_6 => ResourceManager.GetString("TSPORTAL_ERROR_6", resourceCulture);

		internal static string TSPORTAL_ERROR_7 => ResourceManager.GetString("TSPORTAL_ERROR_7", resourceCulture);

		internal static string TSPORTAL_ERROR_8 => ResourceManager.GetString("TSPORTAL_ERROR_8", resourceCulture);

		internal static string TSPORTAL_ERROR_9 => ResourceManager.GetString("TSPORTAL_ERROR_9", resourceCulture);

		internal static string TSPORTAL_ERROR_10 => ResourceManager.GetString("TSPORTAL_ERROR_10", resourceCulture);

		internal static string TSPORTAL_ERROR_11 => ResourceManager.GetString("TSPORTAL_ERROR_11", resourceCulture);

		internal static string TSPORTAL_ERROR_12 => ResourceManager.GetString("TSPORTAL_ERROR_12", resourceCulture);

		internal static string TSPORTAL_ERROR_13 => ResourceManager.GetString("TSPORTAL_ERROR_13", resourceCulture);

		internal static string TSPORTAL_ERROR_14 => ResourceManager.GetString("TSPORTAL_ERROR_14", resourceCulture);

		internal static string TSPORTAL_ERROR_15 => ResourceManager.GetString("TSPORTAL_ERROR_15", resourceCulture);

		internal static string TSPORTAL_EDIT_LABEL => ResourceManager.GetString("TSPORTAL_EDIT_LABEL", resourceCulture);

		internal static string TSPORTAL_CPUB => ResourceManager.GetString("TSPORTAL_CPUB", resourceCulture);

		internal static string TSPORTAL_TS => ResourceManager.GetString("TSPORTAL_TS", resourceCulture);

		internal static string TSPORTAL_TS_NAME => ResourceManager.GetString("TSPORTAL_TS_NAME", resourceCulture);

		internal static string TSPORTAL_TS_NAME_EXPLAIN => ResourceManager.GetString("TSPORTAL_TS_NAME_EXPLAIN", resourceCulture);

		internal static string TSPORTAL_CP_NAME_EXPLAIN => ResourceManager.GetString("TSPORTAL_CP_NAME_EXPLAIN", resourceCulture);

		internal static string TSPublishExceptionMessage => ResourceManager.GetString("TSPublishExceptionMessage", resourceCulture);

		internal static string TSPORTAL_BAD_TS => ResourceManager.GetString("TSPORTAL_BAD_TS", resourceCulture);

		internal static string TSPORTAL_BAD_CP => ResourceManager.GetString("TSPORTAL_BAD_CP", resourceCulture);

		internal static string TSPORTAL_AUTH_FAILURE_TS => ResourceManager.GetString("TSPORTAL_AUTH_FAILURE_TS", resourceCulture);

		internal static string TSPORTAL_AUTH_FAILURE_CP => ResourceManager.GetString("TSPORTAL_AUTH_FAILURE_CP", resourceCulture);

		internal static string TSPORTAL_NO_SCRIPT_PARA_1 => ResourceManager.GetString("TSPORTAL_NO_SCRIPT_PARA_1", resourceCulture);

		internal static string TSPORTAL_NO_SCRIPT_PARA_2 => ResourceManager.GetString("TSPORTAL_NO_SCRIPT_PARA_2", resourceCulture);

		internal static string TSPORTAL_FRAME_FONT => ResourceManager.GetString("TSPORTAL_FRAME_FONT", resourceCulture);

		internal static string TSPORTAL_STRING_NO_CONTROL_XP_CAPTION => ResourceManager.GetString("TSPORTAL_STRING_NO_CONTROL_XP_CAPTION", resourceCulture);

		internal static string TSPORTAL_STRING_NO_CONTROL_XP_PARAGRAPH_1 => ResourceManager.GetString("TSPORTAL_STRING_NO_CONTROL_XP_PARAGRAPH_1", resourceCulture);

		internal static string TSPORTAL_FEED_NO_WORKSPACE_INFO_ERROR_TEXT => ResourceManager.GetString("TSPORTAL_FEED_NO_WORKSPACE_INFO_ERROR_TEXT", resourceCulture);

		internal static string TSPORTAL_NO_WORKSPACE_CONFIG_TEXT => ResourceManager.GetString("TSPORTAL_NO_WORKSPACE_CONFIG_TEXT", resourceCulture);

		internal static string TSPORTAL_DEFAULT_WORKSPACE => ResourceManager.GetString("TSPORTAL_DEFAULT_WORKSPACE", resourceCulture);

		internal LangSpecific()
		{
		}
	}
}
namespace Microsoft.TerminalServices.Publishing.Portal.neutralresources
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class NeutralResources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("Microsoft.TerminalServices.Publishing.Portal.neutralresources.NeutralResources", typeof(NeutralResources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Bitmap defaultIcon => (Bitmap)ResourceManager.GetObject("defaultIcon", resourceCulture);

		internal static Icon mstscIcon => (Icon)ResourceManager.GetObject("mstsc", resourceCulture);

		internal NeutralResources()
		{
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '8.2.0.7535-95108c96')
