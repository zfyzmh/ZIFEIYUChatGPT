using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIFEIYU.Services
{
    /// <summary>
    /// 语音服务类
    /// </summary>
    public class SpeechServices
    {
        public SpeechServices(UserServices userServices)
        {
            this.userServices = userServices;
        }

        private SpeechConfig? speechConfig;
        private readonly UserServices userServices;

        public SpeechConfig SpeechConfig
        {
            get
            {
                var config = userServices.GetConfig().GetAwaiter().GetResult();
                speechConfig ??= SpeechConfig.FromSubscription(config.AzureKey, config.AzureRegion);
                return speechConfig;
            }
        }

        public async Task<string> FromDefaultMicrophoneInput()
        {
            await CheckAndRequestLocationPermission();
            SpeechConfig.SpeechRecognitionLanguage = "zh-CN";
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

            return speechRecognitionResult.Text;
        }

        private bool isPlayVoice;

        public async Task PlayVoice(string Test)
        {
            if (!isPlayVoice)
            {
                isPlayVoice = true;
                SpeechConfig.SpeechRecognitionLanguage = "zh-CN";
                SpeechConfig.SpeechSynthesisVoiceName = "zh-CN-XiaochenNeural";

                using var synthesizer = new SpeechSynthesizer(SpeechConfig);

                await synthesizer.SpeakTextAsync(Test);
                isPlayVoice = false;
            }
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Microphone>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Microphone>();

            return status;
        }
    }
}