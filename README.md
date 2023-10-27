# AiNotes

AiNotes is a .NET MAUI application designed to be a note-taking app powered by Microsoft Azure Cognitive Service Speech recognition and ChatGpt for summarization. The application is targeted only for Android.

## Features

- **Android Support**: Fully functional on Android devices.
- **iOS Support**: Not supported.
- **Translation Service**: Utilizes MS Azure Cognitive Service Speech and ChatGpt.

## Development Environment

This project was developed using the following tools and technologies:

- **Visual Studio 2022**: The primary development environment used for coding, debugging, and testing.
- **.NET 7**: The project is built on the .NET 7 framework, utilizing its features and capabilities to create a robust and efficient application.

Ensure that you have these versions installed if you plan to contribute to or build the project locally.

## Development Setup

To develop locally, you will need to create an `appsettings.Local.json` file and replace the placeholders with the appropriate values.

### `appsettings.Local.json`

```json
{
  "ApplicationSettings": {
    "AzureKeys": {
      "AzureSubscriptionKey": "AZURESUBSCRIPTIONKEY",
      "AzureRegion": "AZUREREGION"
    },
    "AppCenterSecret": "APPCENTERSECRET",
    "DeveloperEmail": "DEVELOPEREMAIL",
    "ChatGptSettings": {
      "MaxTokens": "CHATGPT_MAXTOKENS",
      "Temperature": "CHATGPT_TEMPERATURE",
      "TopP": "CHATGPT_TOPP",
      "ApiKey": "CHATGPT_APIKEY",
      "Model": "CHATGPT_MODEL",
      "EndPoint": "CHATGPT_ENDPOINT"
    }
  }
}
```

## ToDo

#### Life Cycle in `App.xaml.cs` (and then in all components)
- [ ] `OnSleep`
- [ ] `OnResume`
- [ ] `OnConnectivityChanged`

#### Component Level
- [ ] `OnAppearing`
- [ ] `OnDisappearing`
- [ ] `OnConnectivityChanged`

#### Additional Features
- [ ] Add multiple language support (English)

