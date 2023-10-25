using AiNotes.Managers;
using AiNotes.Pages;
using System.Globalization;

namespace AiNotes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AiNotesPage), typeof(AiNotesPage));
            Routing.RegisterRoute(nameof(AiNotesSummaryPage), typeof(AiNotesSummaryPage));
            Routing.RegisterRoute(nameof(SelectNoteTypePage), typeof(SelectNoteTypePage));
            Routing.RegisterRoute(nameof(SummaryPreviewPage), typeof(SummaryPreviewPage));

            SetApplanguge();
        }

        private void SetApplanguge()
        {
            string currentCulture = Preferences.Get("currentCulture", "sv-SE");

            LocalizationResourceManager.Instance.SetCulture(new CultureInfo(currentCulture));

            Preferences.Set("currentCulture", currentCulture);
        }
    }
}