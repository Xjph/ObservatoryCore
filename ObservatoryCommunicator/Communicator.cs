using System.Collections.ObjectModel;
using Observatory.Framework;
using Observatory.Framework.Files.Journal;
using Observatory.Framework.Interfaces;
using static Observatory.Framework.PluginActionUI;

namespace Observatory.Communicator
{
    public class Communicator : IObservatoryWorker
    {
        public Communicator()
        {
            var uiObject = new CommunicatorGridItem();
            _resultsGrid.Add(uiObject);
            _ui = new PluginActionUI(_resultsGrid, ActionDock.Bottom);
        }

        public string Version => "0.0.1";

        public PluginUI PluginUI => _ui;

        public object Settings
        {
            get => _settings;
            set { _settings = (CommunicatorSettings)value; }
        }

        public AboutInfo AboutInfo =>
            new()
            {
                FullName = "Observatory Communicator",
                ShortName = "Communicator",
                Description =
                    "Communicator is a core plugin for Observatory, designed to display sent and received messages in a grid.",
                AuthorName = "Vithigar",
                Links = [],
            };

        public void JournalEvent<TJournal>(TJournal journal)
            where TJournal : JournalBase
        {
            switch (journal)
            {
                case LoadGame loadGame:
                    _currentCommanderName = loadGame.Commander;
                    break;
                case SendText sendText:
                    var sendRow = new CommunicatorGridItem(
                        sendText.TimestampDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        GetChannelDisplayName(sendText.To_Localised ?? sendText.To),
                        _currentCommanderName ?? "You",
                        sendText.Message
                    );
                    AddGridItem(sendRow);
                    break;
                case ReceiveText receiveText:
                    var receiveRow = new CommunicatorGridItem(
                        receiveText.TimestampDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        GetChannelDisplayName(receiveText.Channel.ToString()),
                        receiveText.From_Localised ?? receiveText.From,
                        receiveText.Message_Localised ?? receiveText.Message
                    );
                    AddGridItem(receiveRow);
                    Notify(receiveRow);
                    break;
            }
        }

        private void Notify(CommunicatorGridItem item)
        {
            if (ChannelNotify(item.Channel))
            {
                NotificationArgs args = new() { Title = item.From, Detail = item.Message };
                if (_settings.SuppressVoice)
                {
                    args.Rendering = NotificationRendering.All ^ NotificationRendering.NativeVocal;
                }
                _core?.SendNotification(args);
            }
        }

        public void Load(IObservatoryCore observatoryCore)
        {
            _core = observatoryCore;
            _ui.Actions.Add(
                new(
                    "Local",
                    (chk) => CheckBoxChanged(LOCAL, chk),
                    ActionType.Checkbox,
                    _settings.ShowLocal
                )
            );
            _ui.Actions.Add(
                new(
                    "System",
                    (chk) => CheckBoxChanged(SYSTEM, chk),
                    ActionType.Checkbox,
                    _settings.ShowSystem
                )
            );
            _ui.Actions.Add(
                new(
                    "Private",
                    (chk) => CheckBoxChanged(PRIVATE, chk),
                    ActionType.Checkbox,
                    _settings.ShowPrivate
                )
            );
            _ui.Actions.Add(
                new(
                    "Wing",
                    (chk) => CheckBoxChanged(WING, chk),
                    ActionType.Checkbox,
                    _settings.ShowWing
                )
            );
            _ui.Actions.Add(
                new(
                    "Squadron",
                    (chk) => CheckBoxChanged(SQUADRON, chk),
                    ActionType.Checkbox,
                    _settings.ShowSquadron
                )
            );
            _ui.Actions.Add(
                new(
                    "NPC",
                    (chk) => CheckBoxChanged(NPC, chk),
                    ActionType.Checkbox,
                    _settings.ShowNPC
                )
            );
        }

        private void AddGridItem(CommunicatorGridItem item)
        {
            _gridItems.Add(item);
            if (ChannelShown(item.Channel))
                _core?.AddGridItem(this, item);
        }

        private void CheckBoxChanged(string channel, bool isChecked)
        {
            switch (channel)
            {
                case PRIVATE:
                    _settings.ShowPrivate = isChecked;
                    break;
                case LOCAL:
                    _settings.ShowLocal = isChecked;
                    break;
                case SYSTEM:
                    _settings.ShowSystem = isChecked;
                    break;
                case SQUADRON:
                    _settings.ShowSquadron = isChecked;
                    break;
                case NPC:
                    _settings.ShowNPC = isChecked;
                    break;
                case WING:
                    _settings.ShowWing = isChecked;
                    break;
            }

            _core?.SaveSettings(this);

            var filteredItems = _gridItems.Where(item => ChannelShown(item.Channel));
            _core?.ClearGrid(this, new CommunicatorGridItem());
            _core?.AddGridItems(this, filteredItems, false);
        }

        private bool ChannelShown(string? channel)
        {
            return channel?.ToLower() switch
            {
                LOCAL => _settings.ShowLocal,
                SYSTEM => _settings.ShowSystem,
                "system" => _settings.ShowSystem,
                SQUADRON => _settings.ShowSquadron,
                NPC => _settings.ShowNPC,
                WING => _settings.ShowWing,
                _ => _settings.ShowPrivate,
            };
        }

        private bool ChannelNotify(string? channel)
        {
            return channel?.ToLower() switch
            {
                LOCAL => _settings.NotifyLocal,
                SYSTEM => _settings.NotifySystem,
                "system" => _settings.NotifySystem,
                SQUADRON => _settings.NotifySquadron,
                NPC => _settings.NotifyNPC,
                WING => _settings.NotifyWing,
                _ => _settings.NotifyPrivate,
            };
        }

        private static string GetChannelDisplayName(string channel)
        {
            return channel.ToLower() switch
            {
                WING => "Wing",
                SQUADRON => "Squadron",
                NPC => "NPC",
                SYSTEM => "System",
                LOCAL => "Local",
                _ => channel,
            };
        }

        private string? _currentCommanderName;

        private readonly PluginActionUI _ui;

        private readonly ObservableCollection<object> _resultsGrid = [];

        private readonly List<CommunicatorGridItem> _gridItems = [];

        private CommunicatorSettings _settings = new();

        private IObservatoryCore? _core;

        private const string PRIVATE = "private";
        private const string LOCAL = "local";
        private const string SYSTEM = "starsystem";
        private const string SQUADRON = "squadron";
        private const string NPC = "npc";
        private const string WING = "wing";
    }
}
