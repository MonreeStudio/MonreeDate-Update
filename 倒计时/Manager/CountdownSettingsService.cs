using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace 倒计时.Manager
{
    /// <summary>
    /// Stores per-schedule settings using the schedule GUID as the identity.
    /// Older releases used the display name (and sometimes the name + date),
    /// so reads lazily copy those values to the new keys. Keeping the legacy
    /// values during the transition makes upgrades safe for duplicate names.
    /// </summary>
    internal sealed class CountdownSettingsService
    {
        private const string NotePrefix = "Schedule.Note.";
        private const string AlertPrefix = "Schedule.Alert.";
        private const string DesktopIdPrefix = "DesktopScheduleId";
        private const string MainTileIdKey = "MainTileScheduleId";
        private const string LegacyTileOwnerPrefix = "Schedule.LegacyTileOwner.";
        private const string LegacyTileOwnerIndexKey = "Schedule.LegacyTileOwners";

        private readonly Func<string, bool> contains;
        private readonly Func<string, object> read;
        private readonly Action<string, object> write;
        private readonly Action<string> remove;
        internal CountdownSettingsService()
            : this(ApplicationData.Current.LocalSettings)
        {
        }
        internal CountdownSettingsService(ApplicationDataContainer settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            var propertySet = settings.Values;
            contains = key => propertySet.ContainsKey(key);
            read = key => propertySet.ContainsKey(key) ? propertySet[key] : null;
            write = (key, value) => propertySet[key] = value;
            remove = key => propertySet.Remove(key);
        }

        internal CountdownSettingsService(IDictionary<string, object> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var map = values;
            contains = map.ContainsKey;
            read = key => map.ContainsKey(key) ? map[key] : null;
            write = (key, value) => map[key] = value;
            remove = key => map.Remove(key);
        }
        internal string GetNote(string id, string legacyName, string legacyDate)
        {
            return ReadMigrating(NoteKey(id ?? string.Empty), LegacyNoteKey(legacyName ?? string.Empty, legacyDate ?? string.Empty), string.Empty);
        }
        internal void SetNote(string id, string legacyName, string legacyDate, string value)
        {
            string note = value ?? string.Empty;
            Set(NoteKey(id), note);
        }
        internal string GetAlert(string id, string legacyName, string suffix)
        {
            return ReadMigrating(AlertKey(id, suffix), LegacyAlertKey(legacyName, suffix), "0");
        }
        internal void SetAlert(string id, string legacyName, string suffix, string value)
        {
            string alertValue = string.IsNullOrWhiteSpace(value) ? "0" : value;
            Set(AlertKey(id, suffix), alertValue);
        }

        /// <summary>Copies all name-based note and alert values to an existing ID.</summary>
        internal void MigrateSchedule(string id, string oldName, string oldDate,
            string newName, string newDate)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            string note = GetNote(id, oldName, oldDate);
            SetNote(id, newName, newDate, note);
            foreach (string suffix in new[] { "", "1", "3", "Personal" })
            {
                string value = GetAlert(id, oldName, suffix);
                if (Has(LegacyAlertKey(oldName, suffix)) || Has(AlertKey(id, suffix)))
                    SetAlert(id, newName, suffix, value);
            }
        }

        /// <summary>
        /// Assigns legacy name-based values to one deterministic GUID and then
        /// removes the old key. This prevents two schedules with the same name
        /// from continuing to share settings after the first upgrade run.
        /// </summary>
        internal void MigrateLegacySettings(IEnumerable<ScheduleIdentity> identities)
        {
            foreach (var identity in (identities ?? Enumerable.Empty<ScheduleIdentity>())
                .Where(item => !string.IsNullOrWhiteSpace(NormalizeId(item.Id))))
            {
                string id = NormalizeId(identity.Id);
                string noteKey = LegacyNoteKey(identity.Name, identity.Date);
                if (!Has(NoteKey(id)) && Has(noteKey))
                {
                    Set(NoteKey(id), Get(noteKey));
                    remove(noteKey);
                }
                foreach (string suffix in new[] { "", "1", "3", "Personal" })
                {
                    string oldKey = LegacyAlertKey(identity.Name, suffix);
                    string newKey = AlertKey(id, suffix);
                    if (!Has(newKey) && Has(oldKey))
                    {
                        Set(newKey, Get(oldKey));
                        remove(oldKey);
                    }
                }
                if (!Has(TileStateKey(id)) && Has(identity.Name))
                {
                    Set(TileStateKey(id), Get(identity.Name));
                    remove(identity.Name);
                }
            }
        }
        internal string GetTileState(string id, string legacyName)
        {
            string current = Get(TileStateKey(id));
            if (current != null) return current;
            // Secondary tile state was historically stored under the raw name.
            string legacyKey = string.IsNullOrWhiteSpace(legacyName) ? null : legacyName;
            string legacy = Get(legacyKey);
            if (legacy != null)
            {
                Set(TileStateKey(id), legacy);
                remove(legacyKey);
            }
            return legacy ?? "unpined";
        }
        internal void SetTileState(string id, string legacyName, bool pinned)
        {
            string value = pinned ? "pined" : "unpined";
            Set(TileStateKey(id), value);
        }

        /// <summary>Returns whether this schedule is selected for the desktop tool.</summary>
        internal bool IsDesktopSelected(string id, string legacyName)
        {
            string value = Get(DesktopSelectionKey(id));
            if (value == null && !string.IsNullOrWhiteSpace(legacyName))
            {
                value = Get("DesktopFlag" + legacyName);
                if (value != null) Set(DesktopSelectionKey(id), value);
            }
            return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
        internal void SetDesktopSelected(string id, string legacyName, bool selected)
        {
            string value = selected ? "1" : "0";
            Set(DesktopSelectionKey(id), value);
        }
        internal string GetMainTileId(IEnumerable<ScheduleIdentity> schedules)
        {
            string id = Get(MainTileIdKey);
            if (!string.IsNullOrWhiteSpace(NormalizeId(id))) return NormalizeId(id);

            string legacyName = Get("mainTile");
            if (string.IsNullOrWhiteSpace(legacyName)) return string.Empty;
            var match = (schedules ?? Enumerable.Empty<ScheduleIdentity>())
                .FirstOrDefault(item => string.Equals(item.Name, legacyName, StringComparison.Ordinal));
            if (match != null)
            {
                string normalized = NormalizeId(match.Id);
                if (normalized.Length == 0) return string.Empty;
                Set(MainTileIdKey, normalized);
                remove("mainTile");
                return normalized;
            }
            return string.Empty;
        }
        internal void SetMainTileId(string id)
        {
            Set(MainTileIdKey, NormalizeId(id));
        }
        internal void SetMainTileId(string id, string legacyName)
        {
            SetMainTileId(id);
        }

        // Explicit aliases make the intent clear at call sites that deal with
        // the tile rather than the historical "main" tile name.
        internal string GetTileScheduleId(IEnumerable<ScheduleIdentity> schedules)
        {
            return GetMainTileId(schedules);
        }
        internal void SetTileScheduleId(string id, string legacyName)
        {
            SetMainTileId(id, legacyName);
        }

        /// <summary>
        /// Reads the selected desktop schedule IDs and upgrades DesktopKeyN
        /// values that were previously stored as names.
        /// </summary>
        internal List<string> GetDesktopScheduleIds(IEnumerable<ScheduleIdentity> schedules)
        {
            var known = (schedules ?? Enumerable.Empty<ScheduleIdentity>()).ToList();
            var result = new List<string>();
            int count = ReadInt("ItemCount", 3);
            // Read all slots when ItemCount was not persisted by older builds.
            for (int i = 0; i < Math.Min(3, Math.Max(0, count)); i++)
            {
                string id = Get(DesktopIdPrefix + i);
                if (string.IsNullOrWhiteSpace(id))
                {
                    string oldValue = Get("DesktopKey" + i);
                    var match = known.FirstOrDefault(item => string.Equals(item.Id, oldValue, StringComparison.Ordinal)
                        || string.Equals(item.Name, oldValue, StringComparison.Ordinal));
                    id = match == null ? null : NormalizeId(match.Id);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        Set(DesktopIdPrefix + i, id);
                        remove("DesktopKey" + i);
                    }
                }
                id = NormalizeId(id);
                if (!string.IsNullOrWhiteSpace(id) && known.Any(item => NormalizeId(item.Id) == id))
                    result.Add(id);
            }
            var distinct = result.Distinct(StringComparer.Ordinal).Take(3).ToList();
            // A few older builds only persisted DesktopFlag<name>. Rebuild the
            // slots from those flags when DesktopKeyN was empty.
            bool hasGuidSlots = Enumerable.Range(0, 3).Any(i => Has(DesktopIdPrefix + i));
            if (distinct.Count == 0 && !hasGuidSlots && !Has("ItemCount"))
            {
                foreach (var item in known)
                {
                    string normalized = NormalizeId(item.Id);
                    if (!IsDesktopSelected(item.Id, item.Name) || string.IsNullOrWhiteSpace(normalized) || distinct.Contains(normalized)) continue;
                    distinct.Add(normalized);
                    Set(DesktopIdPrefix + (distinct.Count - 1), normalized);
                    remove("DesktopFlag" + item.Name);
                    if (distinct.Count == 3) break;
                }
            }
            return distinct;
        }
        internal void SetDesktopScheduleIds(IEnumerable<string> ids)
        {
            var values = (ids ?? Enumerable.Empty<string>())
                .Select(NormalizeId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal).Take(3).ToList();
            for (int i = 0; i < 3; i++)
            {
                string previousId = NormalizeId(Get(DesktopIdPrefix + i));
                if (previousId.Length > 0 && !values.Contains(previousId))
                    Set(DesktopSelectionKey(previousId), "0");
            }
            Set("ItemCount", values.Count);
            for (int i = 0; i < 3; i++)
            {
                string key = DesktopIdPrefix + i;
                if (i < values.Count)
                {
                    Set(key, values[i]);
                    Set(DesktopSelectionKey(values[i]), "1");
                }
                else remove(key);
                remove("DesktopKey" + i);
            }
        }

        /// <summary>Starts a new GUID namespace without inheriting old keys.</summary>
        internal void InitializeSchedule(string id)
        {
            string normalized = NormalizeId(id);
            if (normalized.Length == 0) throw new ArgumentException("日程 ID 无效。", nameof(id));
            foreach (string key in new[]
            {
                NoteKey(normalized), AlertKey(normalized, ""), AlertKey(normalized, "1"),
                AlertKey(normalized, "3"), AlertKey(normalized, "Personal"), TileStateKey(normalized),
                DesktopSelectionKey(normalized)
            })
            {
                remove(key);
            }
        }

        /// <summary>Removes only settings owned by this GUID.</summary>
        internal void RemoveSchedule(string id)
        {
            string normalized = NormalizeId(id);
            if (normalized.Length == 0) return;
            foreach (string key in new[]
            {
                NoteKey(normalized), AlertKey(normalized, ""), AlertKey(normalized, "1"),
                AlertKey(normalized, "3"), AlertKey(normalized, "Personal"), TileStateKey(normalized),
                DesktopSelectionKey(normalized)
            })
            {
                remove(key);
            }
            for (int i = 0; i < 3; i++)
            {
                if (NormalizeId(Get(DesktopIdPrefix + i)) == normalized)
                    remove(DesktopIdPrefix + i);
            }
            var remainingTileIds = GetLegacyTileIds();
            foreach (string tileId in remainingTileIds.Where(tileId => NormalizeId(Get(LegacyTileOwnerPrefix + tileId)) == normalized).ToList())
            {
                remove(LegacyTileOwnerPrefix + tileId);
                remainingTileIds.Remove(tileId);
            }
            Set(LegacyTileOwnerIndexKey, string.Join("\n", remainingTileIds));
            if (NormalizeId(Get(MainTileIdKey)) == normalized)
                SetMainTileId(string.Empty);
        }

        /// <summary>Returns the stable D-format GUID used in all new keys.</summary>
        internal static string NormalizeId(string id)
        {
            Guid parsed;
            return Guid.TryParse(id, out parsed) ? parsed.ToString("D") : string.Empty;
        }
        internal static string NoteKey(string id) { return NotePrefix + NormalizeId(id); }
        internal static string AlertKey(string id, string suffix) { return AlertPrefix + NormalizeId(id) + "." + (suffix ?? string.Empty); }
        internal static string TileStateKey(string id) { return "Schedule.Tile." + NormalizeId(id); }
        internal static string DesktopSelectionKey(string id) { return "Schedule.Desktop." + NormalizeId(id); }
        internal static string GetSecondaryTileId(string id) { return "ScheduleTile-" + NormalizeId(id); }
        internal string[] GetSecondaryTileIds(string id)
        {
            string normalized = NormalizeId(id);
            if (normalized.Length == 0) return new string[0];
            var result = new List<string> { GetSecondaryTileId(normalized) };
            foreach (string legacyTileId in GetLegacyTileIds())
            {
                string ownerId = Get(LegacyTileOwnerPrefix + legacyTileId);
                if (NormalizeId(ownerId) == normalized)
                {
                    if (!string.IsNullOrWhiteSpace(legacyTileId)) result.Add(legacyTileId);
                }
            }
            return result.Distinct(StringComparer.Ordinal).ToArray();
        }

        internal void MigrateLegacyTiles(IEnumerable<ScheduleIdentity> identities, IEnumerable<string> tileIds)
        {
            var known = (identities ?? Enumerable.Empty<ScheduleIdentity>())
                .Where(item => !string.IsNullOrWhiteSpace(NormalizeId(item.Id)))
                .OrderBy(item => NormalizeId(item.Id), StringComparer.Ordinal).ToList();
            var canonical = new HashSet<string>(known.Select(item => GetSecondaryTileId(item.Id)), StringComparer.Ordinal);
            foreach (string tileId in (tileIds ?? Enumerable.Empty<string>()).Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                if (canonical.Contains(tileId) || tileId.StartsWith("Schedule.LegacyTileOwner.", StringComparison.Ordinal)) continue;
                var owner = known.FirstOrDefault(item => string.Equals(item.Name, tileId, StringComparison.Ordinal));
                if (owner != null)
                {
                    Set(LegacyTileOwnerPrefix + tileId, NormalizeId(owner.Id));
                    var legacyIds = GetLegacyTileIds();
                    if (!legacyIds.Contains(tileId, StringComparer.Ordinal))
                    {
                        legacyIds.Add(tileId);
                        Set(LegacyTileOwnerIndexKey, string.Join("\n", legacyIds));
                    }
                }
            }
        }

        private List<string> GetLegacyTileIds()
        {
            string raw = Get(LegacyTileOwnerIndexKey);
            return string.IsNullOrWhiteSpace(raw)
                ? new List<string>()
                : raw.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.Ordinal).ToList();
        }
        private static string LegacyNoteKey(string name, string date) { return (name ?? string.Empty) + (date ?? string.Empty); }
        private static string LegacyAlertKey(string name, string suffix) { return "Alert" + (name ?? string.Empty) + (suffix ?? string.Empty); }

        private bool Has(string key) { return !string.IsNullOrWhiteSpace(key) && contains(key); }
        private string Get(string key) { return Has(key) ? read(key)?.ToString() : null; }
        private void Set(string key, object value) { write(key, value); }

        private string ReadMigrating(string currentKey, string legacyKey, string fallback = "")
        {
            string current = Get(currentKey);
            if (current != null) return current;
            string legacy = Get(legacyKey);
            if (legacy != null)
            {
                Set(currentKey, legacy);
                remove(legacyKey);
                return legacy;
            }
            return fallback;
        }

        private int ReadInt(string key, int fallback)
        {
            object value = Has(key) ? read(key) : null;
            if (value is int) return (int)value;
            int parsed;
            return int.TryParse(value?.ToString(), out parsed) ? parsed : fallback;
        }
    }

    internal sealed class ScheduleIdentity
    {
        internal ScheduleIdentity(string id, string name, string date = null)
        {
            Id = id ?? string.Empty;
            Name = name ?? string.Empty;
            Date = date ?? string.Empty;
        }
        internal string Id { get; private set; }
        internal string Name { get; private set; }
        internal string Date { get; private set; }
    }
}
