using AiNotes.Models;
using AiNotes.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace AiNotes.ViewModels
{
    public partial class SelectNoteTypeViewModel : BaseViewModel
    {
        [ObservableProperty]
        NoteType selectedItem;
        [ObservableProperty]
        bool isEnabled = false;
        public ObservableCollection<NoteType> MeetingTypes { get; } = new ObservableCollection<NoteType>
        {
            NoteType.FrittAntecknande,
            NoteType.Samtal2Personer,
            NoteType.MoteFleraPersoner,
            NoteType.Lista
        };

        public SelectNoteTypeViewModel() { }

        [RelayCommand]
        async Task SelectMeetingType()
        {
            if (SelectedItem != NoteType.None)
            {
                var description = JsonConvert.SerializeObject(SelectedItem);
                string destination = $"{nameof(AiNotesPage)}?noteDescription={Uri.EscapeDataString(description)}";

                await Shell.Current.GoToAsync(destination);
            }
        }

        public void OnPickerChange()
        {
            if (SelectedItem != NoteType.None)
            {
                IsEnabled = true;
            }
        }

        public void Reset()
        {
            SelectedItem = NoteType.None;
        }
    }
}
