using System;
using System.Collections.Generic;
using ReactiveUI;
using SS14.Launcher.Models.Data;

namespace SS14.Launcher.Models.Logins;

public abstract class LoggedInAccount : ReactiveObject
{
    public string Username => LoginInfo.Username;
    public Guid UserId => LoginInfo.UserId;

    protected LoggedInAccount(LoginInfo loginInfo)
    {
        LoginInfo = loginInfo;
    }

    public LoginInfo LoginInfo { get; }

    public abstract AccountLoginStatus Status { get; }

    /// <summary>
    ///     List is populated only when this data instantiated.
    /// </summary>
    public List<AuthServerInfo>? SupportedAuthServers = null;

    public bool ShouldShowLoginUponSwitching() => Status == AccountLoginStatus.Expired;
}
