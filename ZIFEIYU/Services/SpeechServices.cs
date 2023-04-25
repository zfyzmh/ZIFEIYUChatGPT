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
                Entity.UserConfig config = userServices.GetConfig().GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(config.AzureKey) && !string.IsNullOrEmpty(config.AzureRegion))
                {
                    speechConfig ??= SpeechConfig.FromSubscription(config.AzureKey, config.AzureRegion);
                }
                return speechConfig;
            }
        }

        public async Task<string> FromDefaultMicrophoneInput()
        {
            if (SpeechConfig == null) return "key未设置!";

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
        /// 切换语言
        /// </summary>
        /// <param name="Spokesman">发言人(前缀为语言)</param>
        /// <returns></returns>
        public async Task SwitchLanguage(string Spokesman)
        {
            if (speechConfig == null) return;
            var user = await userServices.GetConfig();
            user.SpeechSynthesisVoiceName = Spokesman;
            await userServices.UpdateConfig(user);

            SpeechConfig.SpeechRecognitionLanguage = GetRegionPrefix(Spokesman);

            SpeechConfig.SpeechSynthesisVoiceName = Spokesman;
        }

        /// <summary>
        /// 从发言人获取地区前缀
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetRegionPrefix(string input)
        {
            string[] parts = input.Split('-');
            if (parts.Length >= 2)
            {
                return parts[0] + "-" + parts[1];
            }
            else
            {
                return "未找到地区前缀";
            }
        }

        public async Task PlayVoice(string Test)
        {
            if (SpeechConfig == null) return;
            if (!isPlayVoice)
            {
                isPlayVoice = true;

                using var synthesizer = new SpeechSynthesizer(SpeechConfig);

                await synthesizer.SpeakTextAsync(Test);
                isPlayVoice = false;
            }
        }

        /// <summary>
        /// 请求麦克风权限
        /// </summary>
        /// <returns></returns>
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