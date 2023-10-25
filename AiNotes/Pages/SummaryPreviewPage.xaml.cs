using AiNotes.Models;
using AiNotes.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AiNotes.Pages;

public partial class SummaryPreviewPage : ContentPage
{
    private readonly SummaryPreviewViewModel viewModel;
    public SummaryPreviewPage(string encodedMarkdownText)
    {
        InitializeComponent();

        viewModel = new SummaryPreviewViewModel(encodedMarkdownText);

        BindingContext = viewModel;

        RegisterEvents();
    }

    void RegisterEvents()
    {
        WeakReferenceMessenger.Default.Register<CloseModalMessage>(this, async (r, m) =>
        {
            await Navigation.PopModalAsync();
        });
    }

    protected void OnDisappearing(object sender, EventArgs e)
    {
        base.OnDisappearing();
        WeakReferenceMessenger.Default.Unregister<CloseModalMessage>(this);
    }
}