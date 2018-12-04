using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotLibrary {
    public class ChatBot {

        private IStateMachine machine;
        private AdminEngine adminEngine;
        private IStateMachineInitializer initializer;

        private const string configPath = "./config.xml";

        public void Init() {
            initializer = new StateMachineInitializer(configPath);
            LoadMachine();

            adminEngine = new AdminEngine();
            adminEngine.ResetStore(machine.GetStates());
        }

        private void LoadMachine() {
            machine = new StateMachine();
            initializer.Initialize(machine);
        }

        public BotMessage UserInput(UserMessage msg) {
            if (machine == null) {
                throw new ApplicationException("Engine is not initialized");
            }
            if (adminEngine.IsAdminCommand(msg)) {
                return AdminInput(msg);
            } else {
                IState state = machine.GetState(msg.UserId, msg.Message);
                BotMessage result = new BotMessage() {
                    UserId = msg.UserId,
                    Message = state.GetMessage(),
                    Tags = state.GetTags()
                };
                return result;
            }
        }


        private BotMessage AdminInput(UserMessage msg) {
            string resultText;
            if (adminEngine.IsResetCommand(msg)) {
                adminEngine.ResetStore(machine.GetStates());
                resultText = "Готово";
            } else if (adminEngine.IsSaveCommand(msg)) {
                IList<IState> store = adminEngine.GetStore();
                initializer.Save(store);
                LoadMachine();
                resultText = "Готово";
            } else if (adminEngine.IsReloadCommand(msg)) {
                LoadMachine();
                resultText = "Готово";
            } else {
                resultText = adminEngine.ExecuteCommand(msg);
            }
            return new BotMessage() {
                UserId = msg.UserId,
                Message = resultText,
                Tags = new List<BotTag>()
            };
        }


    }
}
