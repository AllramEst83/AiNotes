#if ANDROID
using Android.Media;

namespace AiNotes.Services;
public interface IAndroidAudioRecordService : IDisposable
{
    void StartRecording();
    void StopRecording();
    public RecordState GetRecordState { get; }
    Task<(byte[] buffer, int bytesRead)> GetAudioStream();
    bool IsRecording { get; }
}

public class AndroidAudioRecordService : IAndroidAudioRecordService
{
    private AudioRecord audioRecord;
    private readonly int sampleRate = 44100;
    private readonly int bufferSize = 1024;
    private readonly ChannelIn channelConfig = ChannelIn.Mono;
    private readonly Encoding audioFormat = Encoding.Pcm16bit;
    private readonly byte[] audioBuffer;
    public RecordState GetRecordState
    {
        get
        {
            return audioRecord == null ? RecordState.Stopped : audioRecord.RecordingState;
        }
    }

    public bool IsRecording { get; private set; }

    public AndroidAudioRecordService()
    {
        audioBuffer = new byte[bufferSize];
    }

    public void StartRecording()
    {
        if (audioRecord == null)
        {
            audioRecord = new AudioRecord(AudioSource.Mic, sampleRate, channelConfig, audioFormat, bufferSize);
        }

        if (audioRecord.State == State.Initialized && !IsRecording)
        {
            audioRecord.StartRecording();
            IsRecording = true;
        }
    }

    public void StopRecording()
    {
        if (audioRecord != null && audioRecord.RecordingState == RecordState.Recording)
        {
            audioRecord.Stop();
            IsRecording = false;
        }
    }

    public async Task<(byte[] buffer, int bytesRead)> GetAudioStream()
    {
        if (audioRecord == null)
        {
            throw new InvalidOperationException("AudioRecord is not initialized or not in recording state.");
        }

        int bytesRead = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
        return (audioBuffer, bytesRead);
    }


    public void Dispose()
    {
        if (audioRecord != null)
        {
            if (IsRecording)
            {
                audioRecord.Stop();
            }

            audioRecord.Release();
            audioRecord.Dispose();
            audioRecord = null;
            IsRecording = false;
        }
    }
}
#endif
