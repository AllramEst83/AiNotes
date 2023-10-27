using AiNotes.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;

namespace AiNotes.ViewModels
{
    [QueryProperty(nameof(SpokenText), "spokenText")]
    public partial class AiNotesSummaryViewModel : BaseViewModel
    {
        string spokenText;
        public string SpokenText
        {
            get => spokenText;
            set
            {
                if (spokenText != value)
                {
                    spokenText = value;
                    OnPropertyChanged();

                    RawMarkdownText = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(Uri.UnescapeDataString(value));
                }
            }
        }

        string rawMarkdownText;
        public string RawMarkdownText
        {
            get => rawMarkdownText;
            set
            {
                if (rawMarkdownText != value)
                {
                    rawMarkdownText = value;
                    OnPropertyChanged();
                }
            }
        }

        public AiNotesSummaryViewModel() { }

        [RelayCommand]
        private void ShowPreview()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var markdownText = JsonConvert.SerializeObject(RawMarkdownText);
                string encodedMarkdownText = Uri.EscapeDataString(markdownText);

                WeakReferenceMessenger.Default.Send(new OpenModalMessage(encodedMarkdownText));
            });
        }

        [RelayCommand]
        private async Task CopyToClipboard()
        {
            await Clipboard.SetTextAsync(RawMarkdownText);

            await Toast.Make("Text has been copied to clipboard.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(CancellationToken.None); ;
        }
    }
}
