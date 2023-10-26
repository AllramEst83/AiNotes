using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AiNotes.Exstensions;
using AiNotes.Models;
using AiNotes.Pages;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;

namespace AiNotes.ViewModels
{
    public partial class SelectNoteTypeViewModel : BaseViewModel
    {
        [ObservableProperty]
        string selectedItem = null;
        [ObservableProperty]
        bool isEnabled = false;
        public ObservableCollection<string> MeetingTypes { get; } = new ObservableCollection<string>
        {
            NoteType.FrittAntecknande.GetDescription(),
            NoteType.Samtal2Personer.GetDescription(),
            NoteType.MoteFleraPersoner.GetDescription(),
            NoteType.Lista.GetDescription()
        };

        public SelectNoteTypeViewModel() { }

        [RelayCommand]
        async Task SelectMeetingType()
        {
            if (!string.IsNullOrEmpty(SelectedItem))
            {

                try
                {
                    var description = JsonConvert.SerializeObject(SelectedItem);
                    string destination = $"{nameof(AiNotesPage)}?noteDescription={Uri.EscapeDataString(description)}";

                    await Shell.Current.GoToAsync(destination);
                }
                catch (Exception ex)
                {

                   await Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show(CancellationToken.None);
                }

    
            }
        }

        public void OnPickerChange()
        {
            if (!string.IsNullOrEmpty(SelectedItem))
            {
                IsEnabled = true;
            }
        }

        public void Reset()
        {
            SelectedItem = null;
        }
    }
}
