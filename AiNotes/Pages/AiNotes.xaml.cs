using AiNotes.ViewModels;
using CommunityToolkit.Maui.Alerts;

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

            await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show(CancellationToken.None);
        }

    
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        viewModel.DisposeOfResources();
    }
}