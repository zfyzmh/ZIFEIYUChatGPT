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
            SwitchLanguage("zh-CN", "zh-CN-XiaochenNeural");
            if ((await CheckAndRequestMicrophonePermission()) == PermissionStatus.Granted)
            {
                using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                return speechRecognitionResult.Text;
            }
            return "";
        }

        private bool isPlayVoice;

        /// <summary>
        /// 切换语言,及发言人
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="Spokesman">发言人</param>
        /// <returns></returns>
        public void SwitchLanguage(string language, string? Spokesman = null)
        {
            SpeechConfig.SpeechRecognitionLanguage = language;
            if (Spokesman != null)
            {
                SpeechConfig.SpeechSynthesisVoiceName = Spokesman;
            }
        }

        public async Task PlayVoice(string Test)
        {
            if (!isPlayVoice)
            {
                SwitchLanguage("zh-CN", "zh-CN-XiaochenNeural");
                isPlayVoice = true;

                using var synthesizer = new SpeechSynthesizer(SpeechConfig);

                await synthesizer.SpeakTextAsync(Test);
                isPlayVoice = false;
            }
        }

        public async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
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