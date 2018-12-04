using System;
using System.Collections.Generic;
using System.Text;

namespace BotLibrary {
    public class HelpUtility {

        private readonly IDictionary<string, string> cmdHelpStore;

        public HelpUtility() {
            cmdHelpStore = new Dictionary<string, string>();
            cmdHelpStore.Add(Consts.Command.ADD_PAGE, AddPageCommandText);
            cmdHelpStore.Add(Consts.Command.DELETE_PAGE, DeletePageCommandText);
            cmdHelpStore.Add(Consts.Command.ADD_BUTTON, AddButtonCommandText);
            cmdHelpStore.Add(Consts.Command.DELETE_BUTTON, DeleteButtonCommandText);
            cmdHelpStore.Add(Consts.Command.BUTTON_LIST, ButtonListCommandText);
            cmdHelpStore.Add(Consts.Command.HELP, HelpCommandText);
        }

        public string GetCommandHelp(string cmd) {
            if (cmdHelpStore.TryGetValue(cmd, out string result)) {
                return result;
            } else {
                return "";
            }
        }

        private readonly string AddPageCommandText = $@"Неверный формат команды! Справка по команде:

{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.ADD_PAGE}
PAGE_NAME
PAGE_TEXT

где 
PAGE_NAME - Название страницы, которую нужно добавить
PAGE_TEXT - Содержание страницы. Может быть многострочным (берется весь текст до конца сообщения)";

        private readonly string ButtonListCommandText = $@"Неверный формат команды! Справка по команде:

{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.BUTTON_LIST}
PAGE_NAME

где 
PAGE_NAME - Название страницы, список кнопок которой нужно получить";

        private readonly string DeletePageCommandText = $@"Неверный формат команды! Справка по команде:

{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.DELETE_PAGE}
PAGE_NAME

где 
PAGE_NAME - Название страницы, которую нужно удалить";

        private readonly string AddButtonCommandText = $@"Неверный формат команды! Справка по команде:

{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.ADD_BUTTON}
PAGE_NAME
BUTTON_NAME
NEXT_PAGE_NAME
COLOR

где 
PAGE_NAME - Название страницы, на которую нужно добавить кнопку
BUTTON_NAME - Название кнопки (этот текст будет на кнопке)
NEXT_PAGE_NAME - Название страницы, на которую нужно перейти при нажатии на кнопку
COLOR - Цвет кнопки. Возможные значения: {Consts.Color.WHITE}, {Consts.Color.RED}, {Consts.Color.GREEN}, {Consts.Color.BLUE}. Если не указать, будет использовано значение по умолчанию - {Consts.Color.WHITE}";

        private readonly string DeleteButtonCommandText = $@"Неверный формат команды! Справка по команде:

{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.DELETE_BUTTON}
PAGE_NAME
BUTTON_NAME

где 
PAGE_NAME - Название страницы, с которой нужно удалить кнопку
BUTTON_NAME - Название кнопки, которую нужно удалить";

        private readonly string HelpCommandText = $@"Список доступных команд:
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.PAGE_LIST} - Получить список страниц
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.ADD_PAGE} - Добавить страницу
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.DELETE_PAGE} - Удалить страницу
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.BUTTON_LIST} - Получить список кнопок на странице
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.ADD_BUTTON} - Добавить кнопку на страницу
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.DELETE_BUTTON} - Удалить кнопку со страницы
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.VERIFY} - Проверка конфигурации
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.SAVE} - Сохранить изменения
{Consts.Command.ADMIN_CMD_PREFIX} {Consts.Command.RESET} - Отменить несохраненные изменения";

    }
}
