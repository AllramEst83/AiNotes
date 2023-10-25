namespace AiNotes.Services
{
    public interface ITranscriptionManager
    {
        public void AddTranscription(string transcription);
        public string GetTranscriptions();
        public int GetTokenCount();
        public void Clear();

    }
    public class TranscriptionManager : ITranscriptionManager
    {
        private List<string> transcriptions = new List<string>();
        private int tokenCount = 0;

        // Method to add a new transcription
        public void AddTranscription(string transcription)
        {
            transcriptions.Add(transcription);

            // Update the token count based on the total string length
            UpdateTokenCount();
        }

        // Method to update the token count based on the total string length
        private void UpdateTokenCount()
        {
            // Concatenate all transcriptions into a single string
            var allTranscriptions = string.Join(" ", transcriptions);

            // Estimate the token count based on the total string length
            tokenCount = (int)Math.Ceiling(allTranscriptions.Length / 2.85);
        }

        // Method to clear the transcriptions list
        public void Clear()
        {
            transcriptions.Clear();
            tokenCount = 0;  // Reset the token count as well
        }

        // Method to get the current token count
        public int GetTokenCount()
        {
            return tokenCount;
        }

        // Method to get the transcriptions as a single string
        public string GetTranscriptions()
        {
            return string.Join(" ", transcriptions);
        }
    }
}
