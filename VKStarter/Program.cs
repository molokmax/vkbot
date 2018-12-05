using BotLibrary;
using System.Collections.Generic;
using NLog;
using System;
using System.Linq;
using System.IO;

namespace VKStarter {
    class Program {

        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private static void WriteLog(Exception e) {
            logger.Log(LogLevel.Error, e);
            Console.Error.WriteLine("ERROR: " + e.ToString());
        }

        private static void WriteLog(LogLevel level, string msg) {
            logger.Log(level, msg);
            if (level.Ordinal >= LogLevel.Debug.Ordinal) {
                Console.WriteLine("{0}: {1}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
            }
        }

        static void Main(string[] args) {

            try {
                WriteLog(LogLevel.Info, "VkBot start");
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                VKWrapper vk = new VKWrapper();
                vk.Init("./vk_token.config");
                WriteLog(LogLevel.Info, "VKWrapper initialized");

                ChatBot engine = new ChatBot();
                engine.Init();
                WriteLog(LogLevel.Info, "ChatBot initialized");

                while (true) {
                    try {
                        WriteLog(LogLevel.Trace, "New loop");
                        IEnumerable<UserMessage> messages = vk.GetMessages();
                        WriteLog(LogLevel.Trace, "Messages received");
                        foreach (UserMessage msg in messages) {
                            try {
                                WriteLog(LogLevel.Debug, $"UserMessage: UserId '{msg.UserId}', IsAdmin '{msg.IsAdmin}', Message '{msg.Message}'");
                                BotMessage answer = engine.UserInput(msg);
                                string tags = String.Join(", ", answer.Tags.Select(t => $"['{t.Name}'->'{t.NextState}' ({t.Color})]"));
                                WriteLog(LogLevel.Debug, $"BotMessage: UserId '{answer.UserId}', Message '{answer.Message}', Tags: {tags}");
                                vk.SendMessage(answer);
                                WriteLog(LogLevel.Debug, "BotMessage sent");
                            } catch (Exception e1) {
                                WriteLog(e1);
                            }
                        }
                    } catch (Exception e2) {
                        WriteLog(e2);
                    }
                }
            } catch (Exception e3) {
                WriteLog(e3);
            } finally {
                WriteLog(LogLevel.Info, "VkBot stop");
            }

        }
    }
}
