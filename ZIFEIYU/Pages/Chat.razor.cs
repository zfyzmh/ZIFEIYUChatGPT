﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using MudBlazor;
using ZFY.ChatGpt.Dto;
using ZFY.ChatGpt.Dto.InputDto;
using ZIFEIYU.Dto;
using ZIFEIYU.Entity;
using ZIFEIYU.Services;
using JsonHelper = ZIFEIYU.util.JsonHelper;

namespace ZIFEIYU.Pages
{
    public partial class Chat : IDisposable
    {
        private bool _processing;

        private bool _isDispose = false;
        [Inject] public ChatServices? ChatGPTServices { get; set; }

        [Inject] public SpeechServices? SpeechServices { get; set; }

        [Inject] public UserServices? UserServices { get; set; }

        [Inject] public IJSRuntime? jSRuntime { get; set; }

        [Inject] public IMemoryCache? MemoryCache { get; set; }

        public MudTextField<string>? TextField { get; set; }
        public MudSelect<Templates>? SelectTemplate { get; set; }

        public string? ChatTheme { get; set; }

        public string? HelperText { get; set; }

        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        private bool IsMarkdown
        {
            get { return MemoryCache.TryGetValue("IsMarkdown", out _); }
            set
            {
                if (value)
                {
                    MemoryCache.Set("IsMarkdown", 0);
                }
                else
                {
                    MemoryCache.Remove("IsMarkdown");
                }

                StateHasChanged();
            }
        }

        public List<ChatEntity>? chatEntities { get; set; }

        public List<Templates>? Templates { get; set; }

        public List<Cognitiveservices>? Cognitiveservices { get; set; } = new List<Cognitiveservices>();

        public List<Cognitiveservices>? Spokesmans { get; set; } = new List<Cognitiveservices>();

        private Cognitiveservices? _spokesman;
        public Cognitiveservices? Spokesman
        { get => _spokesman; set { _spokesman = value; SpeechServices.SwitchLanguage(value.ShortName); } }

        public Templates AddTemplates { get; set; } = new Templates();

        public long ChatId { get; set; }

        public bool PrepositiveVisible { get; set; }

        private string _language;

        public string Language
        {
            get => _language; set
            {
                _language = value;
                Spokesmans = Cognitiveservices!.Where(m => m.Locale == value).ToList();
                Spokesman = Spokesmans.First();
                StateHasChanged();
            }
        }

        public bool ADDPrepositiveVisible { get; set; }

        public bool SettingVisible { get; set; }

        public double Temperature { get; set; } = 1;

        public double PresencePenalty { get; set; } = 0;
        public double FrequencyPenalty { get; set; } = 0;

        public bool IsVoice { get; set; }

        public string? ChatPrepositive { get; set; }

        private DialogOptions PrepositiveDialogOptions = new() { FullWidth = true };

        protected override async Task OnInitializedAsync()
        {
            await InitTemplates();

            await InitHistory();

            await InitCognitiveservices();

            ChatEntity chat = await ChatGPTServices!.GetChatCurrent();
            if (chat != null)
            {
                ChatId = chat.Id;
                ChatTheme = chat.Theme!;
                Messages = await JsonHelper.DeserializeJsonToListAsync<ChatMessage>(chat.DialogJson);
            }

            await base.OnInitializedAsync();
        }

        private async Task InitHistory()
        {
            chatEntities = await ChatGPTServices.InitHistory();
        }

        private async Task InitCognitiveservices()
        {
            if (MemoryCache.TryGetValue("Cognitiveservices", out List<Cognitiveservices> temps))
            {
                Cognitiveservices.AddRange(temps);
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("cognitiveservices-voices.json");
                using var reader = new StreamReader(stream);
                string contents = reader.ReadToEnd();
                var cognitives = JsonHelper.DeserializeJsonToList<Cognitiveservices>(contents);
                Cognitiveservices.AddRange(cognitives);
                MemoryCache.Set("Cognitiveservices", contents);
            }
            var VoiceName = (await UserServices!.GetConfig()).SpeechSynthesisVoiceName;
            await SpeechServices!.SwitchLanguage(VoiceName);

            Language = SpeechServices.GetRegionPrefix(VoiceName);
            Spokesman = Spokesmans!.First(m => m.ShortName == VoiceName);
        }

        private async Task InitTemplates()
        {
            Templates = await ChatGPTServices.GetAllPrepositive();

            if (MemoryCache.TryGetValue("FileTemplates", out List<Templates> temps))
            {
                Templates.AddRange(temps);
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("Templates/Prompts.zh-an.json");
                using var reader = new StreamReader(stream);
                var contents = reader.ReadToEnd();
                var Filetemps = JsonHelper.DeserializeJsonToList<Templates>(contents);
                Templates.AddRange(Filetemps);
                MemoryCache.Set("FileTemplates", Filetemps);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task Send()
        {
            if (!string.IsNullOrWhiteSpace(HelperText))
            {
                ChatGPTServices.IsManualCancellation = false;
                Messages.Add(new ChatMessage() { Role = "user", Content = HelperText });
                await TextField.Clear();
                await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                _processing = true;
                StateHasChanged();

                InChat inChat = new(Messages)
                {
                    Temperature = Temperature,
                    PresencePenalty = PresencePenalty,
                    FrequencyPenalty = FrequencyPenalty
                };

                await ChatGPTServices.SendChat(inChat, DialogEvent);
                _processing = false;
                if (!_isDispose) await jSRuntime.InvokeAsync<Task>("UpdateScroll", "IndexBody");
                StateHasChanged();

                if (ChatGPTServices.IsManualCancellation) return;

                await ChatGPTServices.SaveChat(Messages, ChatId);
                if (ChatId == 0)
                {
                    ChatEntity chat = await ChatGPTServices.GetChatCurrent();
                    if (chat != null)
                    {
                        ChatId = chat.Id;
                        ChatTheme = chat.Theme!;
                        Messages = await JsonHelper.DeserializeJsonToListAsync<ChatMessage>(chat.DialogJson);
                    }
                }
            }
        }

        public async Task Reset()
        {
            HelperText = string.Empty;
            ChatPrepositive = string.Empty;
            ChatId = 0;
            Messages = new List<ChatMessage>();
            ChatPrepositive = string.Empty;
            StateHasChanged();
        }

        private void DialogEvent(object sender, List<ChatMessage> e)
        {
            Messages = e;
            StateHasChanged();
        }

        public async Task KeyDown(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Code == "Enter" && !_processing && !string.IsNullOrWhiteSpace(TextField.Value))
            {
                await Send();
            }
        }

        public async Task Voice()
        {
            IsVoice = true;
            StateHasChanged();
            HelperText = await SpeechServices.FromDefaultMicrophoneInput();
            IsVoice = false;
            StateHasChanged();
        }

        public async Task SwitchChat(long chatId)
        {
            if (chatId != ChatId)
            {
                ChatId = chatId;
                var chat = chatEntities.First(m => m.Id == ChatId);
                Messages = JsonHelper.DeserializeJsonToObject<List<ChatMessage>>(chat.DialogJson);
                await ChatGPTServices.SaveChat(Messages, chat.Id);
                StateHasChanged();
            }
        }

        public async Task Stop()
        {
            await Task.Delay(50);
            var lastMsg = Messages.Last();
            if (lastMsg.Role == "user")
            {
                ChatGPTServices.IsManualCancellation = true;
                await ChatGPTServices.Stop();
                HelperText = lastMsg.Content;
                Messages.Remove(lastMsg);
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            _isDispose = true;
        }

        public async Task ClearHistoryChat(int num)
        {
            await ChatGPTServices.ClearHistoryChat(num);
            chatEntities = await ChatGPTServices.InitHistory();
            StateHasChanged();
        }

        public async Task DeleteChat()
        {
            await ChatGPTServices.DeleteChat(ChatId);
            ChatId = 0;
            Messages = new List<ChatMessage>();
            chatEntities = await ChatGPTServices.InitHistory();
            StateHasChanged();
        }

        public void SavePrepositive()
        {
            if (Messages.Count > 0)
            {
                if (Messages[0].Role != "system")
                {
                    Messages.Insert(0, new ChatMessage() { Role = "system", Content = ChatPrepositive });
                }
                else
                {
                    Messages[0].Content = ChatPrepositive;
                }
            }
            else
            {
                Messages.Insert(0, new ChatMessage() { Role = "system", Content = ChatPrepositive });
            }

            PrepositiveVisible = false;
        }

        public void Retry()
        {
            if (Messages.Count >= 2)
            {
                HelperText = Messages[Messages.Count - 2].Content;
                Messages.RemoveAt(Messages.Count - 1);
                Messages.RemoveAt(Messages.Count - 1);
            }
        }

        public async Task PrepositiveChanged(Templates template)
        {
            if (template != null) ChatPrepositive = template.Prompt;
        }

        /// <summary>
        /// 添加对话模板
        /// </summary>
        /// <returns></returns>
        public async Task AddPrepositive()
        {
            if (!string.IsNullOrWhiteSpace(AddTemplates.Act) && !string.IsNullOrWhiteSpace(AddTemplates.Prompt))
            {
                await ChatGPTServices.AddPrepositive(AddTemplates);
                AddTemplates = new Templates();
                StateHasChanged();
                await InitTemplates();
                ADDPrepositiveVisible = false;
            }
        }

        /// <summary>
        /// 删除对话模板
        /// </summary>
        /// <returns></returns>
        public async Task DelPrepositive()
        {
            if (SelectTemplate.Value == null) return;

            if (SelectTemplate.Value.Id > 0)
            {
                await ChatGPTServices.DeletePrepositive(SelectTemplate.Value.Id);
                ChatPrepositive = string.Empty;
                await SelectTemplate.Clear();
                StateHasChanged();
                await InitTemplates();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public async Task ClearPrepositive(MouseEventArgs eventArgs)
        {
            ChatPrepositive = string.Empty;
            StateHasChanged();
        }
    }
}