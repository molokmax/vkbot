using BotLibrary;
using System;

namespace ConsoleStarter {
    class Program {
        static void Main(string[] args) {
            ChatBot engine = new ChatBot();
            engine.Init();
            string userId = "1";

            while (true) {
                Console.Write("You: ");
                string userMessage = Console.ReadLine();
                if (String.Equals(userMessage, "QUIT", StringComparison.InvariantCultureIgnoreCase)) {
                    break;
                }
                UserMessage msg = new UserMessage() {
                    UserId = userId,
                    Message = userMessage
                };
                BotMessage answer = engine.UserInput(msg);
                Console.WriteLine($"Bot: {answer.Message}");
            }
        }
    }
}
