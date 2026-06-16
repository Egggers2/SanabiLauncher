using SS14.Launcher.Api;
using SS14.Launcher.Localization;
using SS14.Launcher.Models.Data;

namespace SS14.Launcher.ViewModels.Login;

public class AuthErrorsOverlayViewModel : ViewModelBase
{
    public IErrorOverlayOwner ParentVm { get; }
    public string Title { get; }
    public string[] Errors { get; }

    public AuthErrorsOverlayViewModel(IErrorOverlayOwner parentVM, string title, string[] errors)
    {
        ParentVm = parentVM;
        Title = title;
        Errors = errors;
    }

    public static string[] AuthCodeToErrors(string[] errors, AuthApi.AuthenticateDenyResponseCode code, AuthServerInfo? serverInfo = null)
    {
        if (code == AuthApi.AuthenticateDenyResponseCode.UnknownError)
            return errors;

        var loc = LocalizationManager.Instance;
        var err = code switch
        {
            AuthApi.AuthenticateDenyResponseCode.InvalidCredentials => "login-error-invalid-credentials",
            AuthApi.AuthenticateDenyResponseCode.AccountUnconfirmed => "login-error-account-unconfirmed",

            // Never shown I hope.
            AuthApi.AuthenticateDenyResponseCode.TfaRequired => "login-error-account-2fa-required",
            AuthApi.AuthenticateDenyResponseCode.TfaInvalid => "login-error-account-2fa-invalid",
            AuthApi.AuthenticateDenyResponseCode.AccountLocked => "login-error-account-account-locked",
            _ => "login-error-unknown"
        };

        return [loc.GetString(err) + $"\n\n Problematic auth server: {serverInfo?.UrlSet.GetMostSuccessfulUrl() ?? "N/A [none specified]"}"];
    }

    public void Ok()
    {
        ParentVm.OverlayOk();
    }
}
