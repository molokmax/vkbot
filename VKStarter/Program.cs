using BotLibrary;
using System.Collections.Generic;
using NLog;
using System;
using System.Linq;

namespace VKStarter {
    class Program {

        private static ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args) {

            try {
                logger.Log(LogLevel.Info, "VkBot start");
                VKWrapper vk = new VKWrapper();
                vk.Init("./vk_token.config");
                logger.Log(LogLevel.Info, "VKWrapper initialized");

                ChatBot engine = new ChatBot();
                engine.Init();
                logger.Log(LogLevel.Info, "ChatBot initialized");

                while (true) {
                    try {
                        logger.Log(LogLevel.Trace, "New loop");
                        IEnumerable<UserMessage> messages = vk.GetMessages();
                        logger.Log(LogLevel.Debug, "messages received");
                        foreach (UserMessage msg in messages) {
                            try {
                                logger.Log(LogLevel.Debug, $"UserMessage: UserId '{msg.UserId}', IsAdmin '{msg.IsAdmin}', Message '{msg.Message}'");
                                BotMessage answer = engine.UserInput(msg);
                                string tags = String.Join(", ", answer.Tags.Select(t => $"['{t.Name}'->'{t.NextState}' ({t.Color})]"));
                                logger.Log(LogLevel.Debug, $"BotMessage: UserId '{answer.UserId}', Message '{answer.Message}'\n tags: {tags}");
                                vk.SendMessage(answer);
                                logger.Log(LogLevel.Debug, "BotMessage sent");
                            } catch (Exception e1) {
                                logger.Log(LogLevel.Error, e1);
                            }
                        }
                    } catch (Exception e2) {
                        logger.Log(LogLevel.Error, e2);
                    }
                }
            } catch (Exception e3) {
                logger.Log(LogLevel.Error, e3);
            } finally {
                logger.Log(LogLevel.Info, "VkBot stop");
            }

        }
    }
}
