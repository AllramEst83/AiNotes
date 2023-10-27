using AiNotes.Models;
using AiNotes.Pages;
using AiNotes.Services;
using AiNotes.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace AiNotes
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var assembly = typeof(MauiProgram).Assembly;
            string configFileName;

#if DEBUG
            configFileName = "AiNotes.appsettings.Local.json";
#else
configFileName = "AiNotes.appsettings.json";
#endif

            using (Stream stream = assembly.GetManifestResourceStream(configFileName))
            {
                var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();

                builder.Configuration.AddConfiguration(config);
            }

            builder.Services.AddOptions<AppSettings>()
                    .Bind(builder.Configuration.GetSection("ApplicationSettings"));


            builder.Services

           //Services
           .AddSingleton<IUserPersmissionsService, UserPersmissionsService>()
           .AddSingleton<IChatGptService, ChatGptService>()
           .AddSingleton<ITranscriptionManager, TranscriptionManager>()
           .AddSingleton<IConnectivityService, ConnectivityService>()

#if ANDROID
           .AddSingleton<IAndroidAudioRecordService, AndroidAudioRecordService>()
#endif
          //Pages
          .AddSingleton<AiNotesPage>()
          .AddSingleton<AiNotesSummaryPage>()
          .AddSingleton<SelectNoteTypePage>()
          .AddSingleton<SummaryPreviewPage>()

           //ViewModels
           .AddSingleton<AiNotesViewModel>()
           .AddSingleton<AiNotesSummaryViewModel>()
           .AddSingleton<SelectNoteTypeViewModel>()
           .AddSingleton<SummaryPreviewViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}