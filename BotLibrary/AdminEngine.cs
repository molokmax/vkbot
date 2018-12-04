using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotLibrary {
    public class AdminEngine {

        public AdminEngine() {
            mapCommands.Add(Consts.Command.HELP, HelpCommand);
            mapCommands.Add(Consts.Command.VERIFY, VerifyCommand);
            mapCommands.Add(Consts.Command.PAGE_LIST, PageListCommand);
            mapCommands.Add(Consts.Command.ADD_PAGE, AddPageCommand);
            mapCommands.Add(Consts.Command.DELETE_PAGE, DeletePageCommand); 
            mapCommands.Add(Consts.Command.BUTTON_LIST, ButtonListCommand);
            mapCommands.Add(Consts.Command.ADD_BUTTON, AddButtonCommand);
            mapCommands.Add(Consts.Command.DELETE_BUTTON, DeleteButtonCommand);
        }

        private readonly HelpUtility helpUtility = new HelpUtility();

        private IDictionary<string, Func<UserMessage, string>> mapCommands = new Dictionary<string, Func<UserMessage, string>>();

        private IList<IState> store;

        public void ResetStore(IEnumerable<IState> srcStore) {
            store = new List<IState>();
            foreach (IState s in srcStore) {
                IState state = (IState) s.Clone();
                store.Add(state);
            }
        }

        public string ExecuteCommand(UserMessage msg) {
            string cmd = GetAdminCommandText(msg);
            if (mapCommands.TryGetValue(cmd.ToLowerInvariant(), out Func<UserMessage, string> act)) {
                return act(msg);
            } else {
                return HelpCommand(msg);
            }
        }

        public IList<IState> GetStore() {
            return store;
        }

        public bool IsResetCommand(UserMessage msg) {
            string cmd = GetAdminCommandText(msg);
            return String.Equals(cmd, Consts.Command.RESET, StringComparison.InvariantCultureIgnoreCase);
        }
        public bool IsSaveCommand(UserMessage msg) {
            string cmd = GetAdminCommandText(msg);
            return String.Equals(cmd, Consts.Command.SAVE, StringComparison.InvariantCultureIgnoreCase);
        }
        public bool IsReloadCommand(UserMessage msg) {
            string cmd = GetAdminCommandText(msg);
            return String.Equals(cmd, Consts.Command.RELOAD, StringComparison.InvariantCultureIgnoreCase);
        }

        private string[] GetCommandLines(UserMessage msg) {
            return msg.Message.Split('\n');
        }

        public string GetAdminCommandText(UserMessage msg) {
            string cmd = GetCommandLines(msg).First();
            string[] cmdArr = cmd.Split(' ');
            if (cmdArr.Length < 2) {
                return "";
            } else {
                return cmdArr[1].Trim();
            }
        }

        public bool IsAdminCommand(UserMessage msg) {
            return msg.IsAdmin && msg.Message.StartsWith(Consts.Command.ADMIN_CMD_PREFIX, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HelpCommand(UserMessage msg) {
            return helpUtility.GetCommandHelp(Consts.Command.HELP);
        }

        private string VerifyCommand(UserMessage msg) {
            IList<string> result = new List<string>();
            ISet<string> pages = new HashSet<string>(store.Select(s => s.GetName().ToLowerInvariant()));
            foreach (IState s in store) {
                foreach (BotTag tag in s.GetTags()) {
                    if (!pages.Contains(tag.NextState.ToLowerInvariant())) {
                        result.Add($"Кнопка '{tag.Name}' на странице '{s.GetName()}' ссылается на неизвестную страницу '{tag.NextState}'");
                    }
                }
            }
            if (result.Any()) {
                return $"Обнаружены следующие проблемы: \n{ String.Join('\n', result) }";
            } else {
                return "OK";
            }
        }

        private string PageListCommand(UserMessage msg) {
            IList<string> result = store.Select(s => s.GetName()).ToList();
            return String.Join('\n', result);
        }

        private string ButtonListCommand(UserMessage msg) {
            string[] lines = GetCommandLines(msg);
            if (lines.Length < 2) {
                return helpUtility.GetCommandHelp(Consts.Command.BUTTON_LIST);
            } else {
                string pageName = lines[1].Trim();
                IState state = store.FirstOrDefault(s => String.Equals(s.GetName(), pageName, StringComparison.InvariantCultureIgnoreCase));
                if (state == null) {
                    return $"Страница '{pageName}' не найдена";
                } else {
                    IEnumerable<string> result = state.GetTags()
                        .Select(t => $"Кнопка: '{t.Name}', Переход на страницу: '{t.NextState}'");
                    return String.Join('\n', result);
                }
            }
        }

        private string AddPageCommand(UserMessage msg) {
            string[] lines = GetCommandLines(msg);
            if (lines.Length < 3) {
                return helpUtility.GetCommandHelp(Consts.Command.ADD_PAGE);
            } else {
                string pageName = lines[1].Trim();
                string text = String.Join('\n', lines.Skip(2));
                if (store.Any(s => String.Equals(s.GetName(), pageName))) {
                    return $"Страница '{pageName}' уже существует";
                } else {
                    store.Add(new State(pageName, text));
                    return "Готово";
                }
            }
        }

        private string DeletePageCommand(UserMessage msg) {
            string[] lines = GetCommandLines(msg);
            if (lines.Length < 2) {
                return helpUtility.GetCommandHelp(Consts.Command.DELETE_PAGE);
            } else {
                string pageName = lines[1].Trim();
                IState state = store.FirstOrDefault(s => String.Equals(s.GetName(), pageName, StringComparison.InvariantCultureIgnoreCase));
                if (state == null) {
                    return $"Страница '{pageName}' не найдена";
                } else {
                    store.Remove(state);
                    return "Готово";
                }
            }
        }

        private string AddButtonCommand(UserMessage msg) {
            string[] lines = GetCommandLines(msg);
            if (lines.Length < 4) {
                return helpUtility.GetCommandHelp(Consts.Command.ADD_BUTTON);
            } else {
                string pageName = lines[1].Trim();
                string btnName = lines[2].Trim();
                string nextPage = lines[3].Trim();
                string color = lines.Length >= 5 ? lines[4].Trim() : null;
                string hiddenText = lines.Length >= 6 ? lines[5].Trim() : null;
                IState state = store.FirstOrDefault(s => String.Equals(s.GetName(), pageName, StringComparison.InvariantCultureIgnoreCase));
                if (state == null) {
                    return $"Страница '{pageName}' не найдена";
                } else {
                    if (state.GetTags().Any(t => String.Equals(t.Name, btnName))) {
                        return $"Кнопка '{btnName}' на странице '{pageName}' уже существует";
                    } else {
                        bool hidden = String.Equals(hiddenText, "YES", StringComparison.InvariantCultureIgnoreCase) || String.Equals(hiddenText, "TRUE", StringComparison.InvariantCultureIgnoreCase);
                        state.AddTag(btnName, nextPage, color, hidden);
                        return "Готово";
                    }
                }
            }
        }

        private string DeleteButtonCommand(UserMessage msg) {
            string[] lines = GetCommandLines(msg);
            if (lines.Length < 3) {
                return helpUtility.GetCommandHelp(Consts.Command.DELETE_BUTTON);
            } else {
                string pageName = lines[1].Trim();
                string btnName = lines[2].Trim();
                IState state = store.FirstOrDefault(s => String.Equals(s.GetName(), pageName, StringComparison.InvariantCultureIgnoreCase));
                if (state.DeleteTag(btnName)) {
                    return "Готово";
                } else {
                    return $"Кнопка '{btnName}' на странице '{pageName}' не найдена";
                }
            }
        }

    }
}
