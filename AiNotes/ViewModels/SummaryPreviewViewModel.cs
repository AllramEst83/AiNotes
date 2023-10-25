using AiNotes.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Markdig;

namespace AiNotes.ViewModels
{
    public partial class SummaryPreviewViewModel : BaseViewModel
    {

        [ObservableProperty]
        string markdownPreview;
        public SummaryPreviewViewModel(string markdownText)
        {
            var rawMarkdownText = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(Uri.UnescapeDataString(markdownText));
            UpdateMarkdownPreview(rawMarkdownText);
        }

        private void UpdateMarkdownPreview(string rawMarkdownText)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            MarkdownPreview = Markdown.ToHtml(rawMarkdownText ?? string.Empty, pipeline);
        }

        [RelayCommand]
        public void GoBack()
        {
            WeakReferenceMessenger.Default.Send(new CloseModalMessage());
        }
    }
}
