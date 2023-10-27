using AiNotes.ViewModels;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        viewModel.Initiate();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        viewModel.DisposeOfResources();
    }
}