namespace QuickGPT.Logic
{
    internal class Settings
    {
        public const string OPENAI_API_KEY = "your-token";
        public const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions";
        public const string OPENAI_MODEL = "gpt-3.5-turbo";

        public const string SYSTEM_MESSAGE = "You are a helpful assistant. Keep your answers short, simple and informative.";
        public const int UPDATE_INTERVAL = 30;
    }
}
