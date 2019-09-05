using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Storage;
using Uno.Threading;

namespace Uno.AzureDevOps.Business.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private const string StorageKey = "Uado.OAuthKey";

		private static readonly object _lock = new object();

		private readonly IOAuthClient _oAuthClient;
		private readonly ISecureStorage _secureStorage;

		private bool _isInvalidToken;
		private AsyncLock _refreshTokenLock = new AsyncLock();

		public AuthenticationService(IOAuthClient oAuthClient, ISecureStorage secureStorage)
		{
			_oAuthClient = oAuthClient ?? throw new ArgumentNullException(nameof(oAuthClient));
			_secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
		}

		public event LoggedOutEventHandler LoggedOut;

		public Task<T> AuthenticatedExecution<T>(Func<CancellationToken, OAuthData, Task<T>> func, CancellationToken ct = default)
		{
			return InternalExecute(func, ct);
		}

		public async Task AuthenticatedExecution(Func<CancellationToken, OAuthData, Task> func, CancellationToken ct = default)
		{
			await InternalExecute(
				async (token, authenticationData) =>
				{
					await func(ct, authenticationData);

					return 0;
				},
				ct);
		}

		public bool IsAuthenticated()
		{
			lock (_lock)
			{
				return _secureStorage.GetValue<OAuthData>(StorageKey) != null;
			}
		}

		public async Task Login(string authenticationCode, CancellationToken ct = default)
		{
			if (string.IsNullOrWhiteSpace(authenticationCode))
			{
				throw new ArgumentException("Please provide a valid, non-empty, authentication code from Azure DevOps", nameof(authenticationCode));
			}

			var authenticationResponse = await _oAuthClient.GetAuthData(authenticationCode, ct);

			lock (_lock)
			{
				_secureStorage.SetValue(StorageKey, authenticationResponse.Auth);
			}
		}

		public void Logout()
		{
			InternalLogout();
		}

		private void InternalLogout(Exception exception = null)
		{
			lock (_lock)
			{
				_secureStorage.DeleteAll();
				LoggedOut?.Invoke(new LoggedOutEventArgs() { Exception = exception });
			}
		}

		private async Task<T> InternalExecute<T>(Func<CancellationToken, OAuthData, Task<T>> func, CancellationToken ct)
		{
			if (!_secureStorage.HasKey(StorageKey))
			{
				throw new InvalidOperationException(message: "Please login with an user account before executing authorized actions");
			}

			var authenticationData = await GetToken(ct);

			try
			{
				return await func(ct, authenticationData);
			}
			catch (UnauthorizedAccessException)
			{
				try
				{
					_isInvalidToken = true;
					authenticationData = await GetToken(ct);
					return await func(ct, authenticationData);
				}
				catch (UnauthorizedAccessException ex)
				{
					InternalLogout(ex);
				}

				return default;
			}
		}

		private async Task<OAuthData> GetToken(CancellationToken ct)
		{
			// This operation is not cancellable
			ct = CancellationToken.None;
			using (await _refreshTokenLock.LockAsync(ct))
			{
				var authenticationData = _secureStorage.GetValue<OAuthData>(StorageKey);

				if (_isInvalidToken)
				{
					var refreshedAuthData = await _oAuthClient.RefreshToken(authenticationData.RefreshToken, ct);

					authenticationData = refreshedAuthData.Auth;

					_secureStorage.SetValue(StorageKey, refreshedAuthData.Auth);
				}

				return authenticationData;
			}
		}
	}
}
