using AiNotes.ViewModels;
using Newtonsoft.Json;
namespace AiNotes.Pages;

public partial class AiNotesPage : ContentPage
{
    private readonly AiNotesViewModel viewModel;

    public AiNotesPage()
    {
        InitializeComponent();

        viewModel = Application.Current.Handler.MauiContext.Services.GetService<AiNotesViewModel>();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await viewModel.Initiate();
        }
        catch (Exception ex)
        {

            var spokenText = JsonConvert.SerializeObject(ex.StackTrace);
            string destination = $"{nameof(AiNotesSummaryPage)}?spokenText={Uri.EscapeDataString(spokenText)}";

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync(destination);
            });
        }

    
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        viewModel.DisposeOfResources();
    }
}