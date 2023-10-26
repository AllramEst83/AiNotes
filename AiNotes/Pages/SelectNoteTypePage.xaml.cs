using AiNotes.Services;
using AiNotes.ViewModels;
using CommunityToolkit.Maui.Alerts;

namespace AiNotes.Pages;

public partial class SelectNoteTypePage : ContentPage
{
    private SelectNoteTypeViewModel viewModel;
    private readonly IConnectivityService connectivityService;
    private readonly IUserPersmissionsService userPersmissionsService;

    public SelectNoteTypePage(IConnectivityService connectivityService, IUserPersmissionsService userPersmissionsService)
    {
        InitializeComponent();

        viewModel = new SelectNoteTypeViewModel();
        BindingContext = viewModel;

        this.connectivityService = connectivityService;
        this.userPersmissionsService = userPersmissionsService;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var cancellactionTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellactionTokenSource.Token;

        await userPersmissionsService.GetPermissionsFromUser(cancellationToken);

        bool isConnected = connectivityService.IsConnected();
        if (!isConnected)
        {
            await Toast.Make("No internet connection! Please connect to the internet.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show(CancellationToken.None);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        viewModel.Reset();
    }

    private void MeetingTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        viewModel.OnPickerChange();
    }
}