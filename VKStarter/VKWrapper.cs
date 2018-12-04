using BotLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace VKStarter {
    public class VKWrapper {
        //private ulong appId;
        //private string login;
        //private string password;

        private VkApi api;

        private ulong groupId;
        private IEnumerable<string> adminIds;

        private string longPollKey;
        private string longPollServer;
        private string longPollTs;
        private const int longPollWait = 25;

        public void Init(string configPath = "./vk.config") {
            if (!File.Exists(configPath)) {
                throw new ApplicationException($"Configuration file '{configPath}' don't exists");
            } else {
                string[] content = File.ReadAllLines(configPath);
                if (content.Length < 2) {
                    throw new ApplicationException("Incorrect Configuration file '{configPath}'. First line - access_token, second line - group_id");
                }

                api = new VkApi();

                api.Authorize(GetAuthParams(content));

                groupId = Convert.ToUInt64(content[1]);
                adminIds = (content[2] ?? "").Split(new char[] { ';', ',', ' ' }).Select(x => x.Trim()).Distinct().ToList();

                LongPollServerResponse serverInfo = api.Groups.GetLongPollServer(groupId);
                longPollKey = serverInfo.Key;
                longPollServer = serverInfo.Server;
                longPollTs = serverInfo.Ts;
            }
        }

        private ApiAuthParams GetAuthParams(string[] content) {
            // if (content.Length < 30) {
                return new ApiAuthParams() {
                    AccessToken = content[0],
                    Settings = Settings.All
                };
            /*} else {
                ulong appId = Convert.ToUInt64(content[0].Trim());
                string login = content[1].Trim();
                string password = content[2].Trim();

                return new ApiAuthParams {
                    ApplicationId = appId,
                    Login = login,
                    Password = password,
                    Settings = Settings.All
                };
            }*/
        }

        public IEnumerable<UserMessage> GetMessages() {
            if (api == null) {
                throw new ApplicationException("VKWrapper is not initialized");
            }

            BotsLongPollHistoryResponse response = api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams() {
                Key = longPollKey,
                Server = longPollServer,
                Ts = longPollTs,
                Wait = longPollWait
            });
            longPollTs = response.Ts;
            
            IList<UserMessage> result = new List<UserMessage>();
            foreach (var upd in response.Updates) {
                if (upd.Type == GroupUpdateType.MessageNew) {
                    string userId = upd.Message.FromId.ToString();
                    result.Add(new UserMessage() {
                        IsAdmin = adminIds.Contains(userId),
                        UserId = userId,
                        Message = upd.Message.Text
                    });
                }
            }

            return result;
        }

        private KeyboardButtonColor GetButtonType(string color) {
            switch (color.ToUpper()) {
                case Consts.Color.WHITE:
                    return KeyboardButtonColor.Default;
                case Consts.Color.RED:
                    return KeyboardButtonColor.Negative;
                case Consts.Color.GREEN:
                    return KeyboardButtonColor.Positive;
                case Consts.Color.BLUE:
                    return KeyboardButtonColor.Primary;
                default:
                    throw new NotImplementedException($"Color: {color}");
            }
        }

        public void SendMessage(BotMessage msg) {
            if (api == null) {
                throw new ApplicationException("VKWrapper is not initialized");
            }
            List<List<MessageKeyboardButton>> buttons = new List<List<MessageKeyboardButton>>();
            foreach (BotTag tag in msg.Tags.Where(t => !t.Hidden)) {
                MessageKeyboardButton btn = new MessageKeyboardButton() {
                    Color = GetButtonType(tag.Color),
                    Action = new MessageKeyboardButtonAction() {
                        Label = tag.Name,
                        Type = KeyboardButtonActionType.Text
                    }
                };
                buttons.Add(new List<MessageKeyboardButton>() {
                    btn
                });
            }

            api.Messages.Send(new MessagesSendParams() {
                UserId = Convert.ToInt64(msg.UserId),
                Message = msg.Message,
                Keyboard = new MessageKeyboard() {
                    OneTime = true,
                    Buttons = buttons
                }
            });
        }
    }
}
