using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;
#if ANDROID
using Android.Content;
#endif
#if IOS
using Foundation;
using UIKit;
# endif
namespace AiNotes.Services
{

    public interface IUserPersmissionsService
    {
        Task<bool> GetPermissionsFromUser(CancellationToken cancellationToken);
    }
    public class UserPersmissionsService : IUserPersmissionsService
    {
        private async Task<bool> CheckForPermissionForMicrophone()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> CheckForPermissionForNetworkState()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.NetworkState>();
            }

            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            return true;
        }
#if ANDROID
        private static void OpenSettings()
        {
            var packageName = Platform.CurrentActivity.PackageName;
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            var uri = Android.Net.Uri.Parse($"package:{packageName}");
            intent.SetData(uri);
            Platform.CurrentActivity.StartActivity(intent);
        }
#endif

#if IOS
        private void OpenSettings()
        {
            var url = new NSUrl(UIApplication.OpenSettingsUrlString);
            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions() { });
            }
        }
#endif

        public async Task<bool> GetPermissionsFromUser(CancellationToken cancellationToken)
        {
            var isMicrophoneGranted = await CheckForPermissionForMicrophone();
            var isNetworkStateGranted = await CheckForPermissionForMicrophone();

            if (isMicrophoneGranted && isNetworkStateGranted)
            {
                return true;
            }

            string message;
            if (!isMicrophoneGranted)
            {
                message = "The app wont work without microphone permisson. Do you want to change this i the settings.";

            }
            else if (!isNetworkStateGranted)
            {
                message = "The app wont work without network permisson. Do you want to change this i the settings.";

            }
            else
            {
                message = "The app wont work without microphone and network permissons. Do you want to change this i the settings.";
            }


            var snackbar = GetSnackbar(message);
            await snackbar.Show(cancellationToken);

            return false;
        }

        private Snackbar GetSnackbar(string message)
        {
            object bgColor = Colors.Red;
            object actionBtnTextColor = Colors.Yellow;

            var resources = Application.Current.Resources;
            resources.TryGetValue("PrimaryAccent", out bgColor);
            resources.TryGetValue("Secondary", out actionBtnTextColor);

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = (Color)bgColor,
                TextColor = Colors.Black,
                ActionButtonTextColor = (Color)actionBtnTextColor,
                CornerRadius = new CornerRadius(10),
                Font = Font.SystemFontOfSize(14),
                ActionButtonFont = Font.SystemFontOfSize(16),
                CharacterSpacing = 0.1
            };

            var snackbar = new Snackbar
            {
                Text = message,
                Duration = TimeSpan.FromSeconds(5),
                ActionButtonText = "Open",
                Action = () =>
                {
#if ANDROID || IOS
                    OpenSettings();
#endif
                },
                VisualOptions = snackbarOptions
            };

            return snackbar;
        }
    }
}
