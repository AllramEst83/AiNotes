
using AiNotes.Managers;
using AiNotes.Models;
using AiNotes.Pages;
using AiNotes.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AiNotes.ViewModels
{
    [QueryProperty(nameof(NoteDescription), "noteDescription")]
    public partial class AiNotesViewModel : BaseViewModel
    {
        public string NoteDescription
        {
            set
            {
                var noteDescription = JsonConvert.DeserializeObject<NoteType>(Uri.UnescapeDataString(value));

                chatGptService.SetNoteTypeInstructions(noteDescription);
            }
        }

        private readonly IChatGptService chatGptService;
        private readonly ITranscriptionManager transcriptionManager;
        private readonly AppSettings appSettings;

#if ANDROID
        public readonly IAndroidAudioRecordService androidAudioRecordService;
#endif

        private SpeechRecognizer speechRecognizer;
        private PushAudioInputStream _pushStream;
        private bool _isRecognizerActive = false;
        private bool _continueProcessing = true;
        private SemaphoreSlim _semaphore = new(1, 1);

        [ObservableProperty]
        bool animationIsActive = false;
        [ObservableProperty]
        bool isBusy;
        [ObservableProperty]
        string buttonText = "Start";


        private double _currentProgress;
        public double CurrentProgress
        {
            get => _currentProgress;
            set => SetProperty(ref _currentProgress, value);
        }

        public AiNotesViewModel(
            IChatGptService chatGptService,
            IOptions<AppSettings> appSettings,
            ITranscriptionManager transcriptionManager
#if ANDROID
            , IAndroidAudioRecordService androidAudioRecordService
#endif
            )
        {
            this.appSettings = appSettings.Value;
            this.chatGptService = chatGptService;
            this.transcriptionManager = transcriptionManager;
#if ANDROID
            this.androidAudioRecordService = androidAudioRecordService;
#endif
        }

        public void Initiate()
        {
            SetupRecognizer("sv-SE");
        }

        [RelayCommand]
        async Task ToggleTranscription()
        {
            if (_isRecognizerActive)
            {
                await StopTranscription();
            }
            else
            {
                await StartTranscription();
            }
        }

        async Task StartTranscription()
        {
            AnimationIsActive = true;
            StartToProcessAudio();
            await StartRecognizer();
            StartRecording();
            _isRecognizerActive = true;
            ButtonText = LocalizationResourceManager.Instance["Stop"] as string;
        }

        async Task StopTranscription()
        {
            AnimationIsActive = false;
            ButtonText = LocalizationResourceManager.Instance["Summarizing"] as string;

            IsBusy = true;
            StopRecording();
            await StopRecognizers();
            var text = await Summarize();
            IsBusy = false;

            await Navigate(text);
        }

        async Task<string> Summarize()
        {

            var text = await SummerizeConversation();
            if (string.IsNullOrEmpty(text))
            {
                text = LocalizationResourceManager.Instance["NoTextFromResponse"] as string;
            }

            return text;
        }

        async Task Navigate(string text)
        {
            var spokenText = JsonConvert.SerializeObject(text);
            string destination = $"{nameof(AiNotesSummaryPage)}?spokenText={Uri.EscapeDataString(spokenText)}";

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync(destination);
            });
        }

        async Task Stop()
        {
            try
            {
                StopRecording();
                await StopRecognizers();
            }
            catch (Exception ex)
            {
                Console.Write("Error thrown when trying to Stop", ex);
                throw;
            }
        }

        private async Task<string> SummerizeConversation()
        {
            string response = string.Empty;
            string transcription = transcriptionManager.GetTranscriptions();

            await chatGptService.GetResponseAsync(transcription)
            .ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    response = task.Result;
                }
                else
                {
                    var message = LocalizationResourceManager.Instance["Gpt3ErrorOccured"] as string;
                    ShowToastMessage(message);

                    Console.WriteLine("Error occured while trying to get response from GPT-3", task.Exception);
                }
            });

            return response;
        }

        private void StartRecording()
        {
#if ANDROID
            if (!androidAudioRecordService.IsRecording)
            {
                androidAudioRecordService.StartRecording();
            }
#endif
        }

        private void StopRecording()
        {
#if ANDROID
            if (androidAudioRecordService.IsRecording)
            {
                androidAudioRecordService.StopRecording();
            }
#endif
        }

        private void StartToProcessAudio()
        {
            //https://www.syncfusion.com/blogs/post/building-an-audio-recorder-and-player-app-in-net-maui.aspx
            //Audio recorder for android, IOS and

            if (!_continueProcessing)
            {
                _continueProcessing = !_continueProcessing;
            }

            Task.Run(async () =>
            {
                while (_continueProcessing)
                {
                    await _semaphore.WaitAsync();

                    try
                    {
#if ANDROID
                        if (androidAudioRecordService != null && androidAudioRecordService.IsRecording)
                        {
                            var (audioBuffer, bytesRead) = await androidAudioRecordService.GetAudioStream();
                            if (bytesRead > 0)
                            {
                                if (_isRecognizerActive)
                                {
                                    _pushStream.Write(audioBuffer, bytesRead);
                                }
                            }
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occured while reading from the audio stream.", ex);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }

                }
            });
        }

        public void DisposeOfResources()
        {
            _continueProcessing = false;
            _isRecognizerActive = false;
            ButtonText = LocalizationResourceManager.Instance["Start"] as string;
            transcriptionManager.Clear();
            CurrentProgress = 0;

            try
            {
                speechRecognizer.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured wile trying to dispse of _translationRecognizer", ex);
            }

            try
            {
                _pushStream.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured wile trying to dispse of _pushStream", ex);
            }


#if ANDROID
            try
            {
                if (androidAudioRecordService.IsRecording) { androidAudioRecordService.StopRecording(); };
                androidAudioRecordService.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured wile trying to dispse of androidAudioRecordService", ex);
            }
#endif
        }

        async Task StopRecognizers()
        {
            if (_isRecognizerActive)
            {
                await speechRecognizer.StopContinuousRecognitionAsync();
                _isRecognizerActive = false;
            }
        }

        private async Task StartRecognizer()
        {
            if (_isRecognizerActive)
            {
                await speechRecognizer.StopContinuousRecognitionAsync();
                _isRecognizerActive = false;
            }

            if (!_isRecognizerActive)
            {
                await speechRecognizer.StartContinuousRecognitionAsync();
                _isRecognizerActive = true;
            }
        }

        private void SetupRecognizer(string languageCode)
        {
            var speechConfig = SetupSpeechRecognizer(languageCode);

            (var audioConfig, _pushStream) = ConfigureAudioStream();

            speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            RegisterRecognizers(speechRecognizer);
        }

        private void RegisterRecognizers(SpeechRecognizer speechRecognizer)
        {
            speechRecognizer.SpeechStartDetected += (sender, args) =>
            {
                Console.WriteLine("SpeechStartDetected");
            };

            speechRecognizer.Recognized += async (sender, args) =>
            {
                switch (args.Result.Reason)
                {
                    case ResultReason.RecognizedSpeech:

                        if (!string.IsNullOrEmpty(args.Result.Text))
                        {
                            var spokenText = args.Result.Text.Trim();

                            Console.WriteLine("Recognized: " + spokenText);

                            transcriptionManager.AddTranscription(spokenText);

                            var tokenCount = transcriptionManager.GetTokenCount();

                            await HasTokenCountBeenExceeded();
                        }

                        break;

                    default:
                        var message = $"{args.Result.Reason} - {args.Result.Text}";
                        Console.WriteLine(message);
                        break;
                }
            };

            speechRecognizer.Canceled += (sender, args) =>
            {
                string message;
                switch (args.Result.Reason)
                {
                    case ResultReason.Canceled:
                        message = "Translation has stopped. Please go back and select languages again.";
                        break;
                    case ResultReason.NoMatch:
                        message = "Unable to translate. Please try again. Please go back and select languages again.";
                        break;

                    default:
                        message = "An unexpected error occured. Please restart the application";
                        break;

                }

                Console.WriteLine(message);
            };
        }

        private async Task HasTokenCountBeenExceeded()
        {
            var tokenCount = transcriptionManager.GetTokenCount();
            var maxTokens = appSettings.ChatGptSettings.MaxTokens;

            var targetProgress = CalculateUsedPercentage(tokenCount, maxTokens);

            await SmoothProgressUpdate(targetProgress);

            var seventyFivePercentThreshold = maxTokens * 0.75;
            if (tokenCount >= maxTokens)
            {
                await ToggleTranscription();
            }
            else if (tokenCount >= seventyFivePercentThreshold)
            {
                var message = LocalizationResourceManager.Instance["75ProcentTokenLimitMessage"] as string;
                ShowToastMessage(message);
            }
        }

        private async Task SmoothProgressUpdate(double targetProgress)
        {
            const double incrementThreshold = 0.005;
            while (_currentProgress < targetProgress - incrementThreshold)
            {
                double increment = (targetProgress - _currentProgress) / 5;
                double nextProgress = _currentProgress + increment;

                _currentProgress = nextProgress;
                OnPropertyChanged(nameof(CurrentProgress));
                await Task.Delay(20);
            }

            _currentProgress = Math.Round(targetProgress, 2);
            OnPropertyChanged(nameof(CurrentProgress));
        }

        private static double CalculateUsedPercentage(int tokenCount, int maxTokens)
        {
            // Calculate the used percentage
            double usedPercentage = (double)tokenCount / maxTokens;

            // Ensure the value is within the progress bar's scale of 0.01 to 1 (or 0.99)
            double progressBarValue = Math.Max(0.01, Math.Min(1, usedPercentage));

            // If you want to round to nearest 0.01 for granularity
            progressBarValue = Math.Round(progressBarValue, 2);

            return progressBarValue;
        }

        private void ShowToastMessage(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show(CancellationToken.None);
            });
        }

        private static (AudioConfig, PushAudioInputStream) ConfigureAudioStream()
        {
            var pushStream = AudioInputStream.CreatePushStream(AudioStreamFormat.GetWaveFormatPCM(44100, 16, 1));
            var audioConfig = AudioConfig.FromStreamInput(pushStream);
            return (audioConfig, pushStream);
        }

        private SpeechConfig SetupSpeechRecognizer(string languageCode)
        {
            var config = SpeechConfig.FromSubscription(appSettings.AzureKeys.AzureSubscriptionKey, appSettings.AzureKeys.AzureRegion);
            config.SpeechRecognitionLanguage = languageCode;

            return config;
        }

        [RelayCommand]
        public async Task Reset()
        {
            IsBusy = true;
            AnimationIsActive = false;

            await Stop();
            DisposeOfResources();

            Initiate();
            ButtonText = LocalizationResourceManager.Instance["Start"] as string;
            IsBusy = false;
        }
    }
}
