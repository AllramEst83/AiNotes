using AiNotes.Models;
using AiNotes.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AiNotes.Pages;

public partial class AiNotesSummaryPage : ContentPage
{
    private AiNotesSummaryViewModel viewModel;

    public AiNotesSummaryPage()
    {
        InitializeComponent();
        viewModel = new AiNotesSummaryViewModel();
        BindingContext = viewModel;

        RegisterEvents();
    }

    void RegisterEvents()
    {
        WeakReferenceMessenger.Default.Register<OpenModalMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushModalAsync(new SummaryPreviewPage(m.MarkdownText));
            });
        });
    }

    void UnregisterEvents()
    {
        WeakReferenceMessenger.Default.Unregister<OpenModalMessage>(this);
    }

    protected void OnDisappearing(object sender, EventArgs e)
    {
        base.OnDisappearing();

        UnregisterEvents();
    }
}