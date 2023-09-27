using System.Collections.Generic;

namespace SysBot.Pokemon
{
    /// <summary>
    /// Pokémon Scarlet/Violet RAM offsets
    /// </summary>
    public class PokeDataOffsetsSV
    {
        public const string SVGameVersion = "2.0.1";
        public const string ScarletID = "0100A3D008C5C000";
        public const string VioletID  = "01008F6008C5E000";
        public IReadOnlyList<long> BoxStartPokemonPointer { get; } = new long[] { 0x0000000, 0x000, 0x00, 0x000, 0x00 };
        public IReadOnlyList<long> LinkTradePartnerPokemonPointer { get; } = new long[] { 0x0000000, 0x00, 0x00, 0x00, 0x000 };
        public IReadOnlyList<long> LinkTradePartnerNIDPointer { get; } = new long[] { 0x0000000, 0x00, 0x00 };
        public IReadOnlyList<long> MyStatusPointer { get; } = new long[] { 0x0000000, 0x00, 0x00 };
        public IReadOnlyList<long> Trader1MyStatusPointer { get; } = new long[] { 0x0000000, 0x00, 0x00, 0x0 }; // The trade partner status uses a compact struct that looks like MyStatus.
        public IReadOnlyList<long> Trader2MyStatusPointer { get; } = new long[] { 0x0000000, 0x00, 0x00, 0x0 };
        public IReadOnlyList<long> ConfigPointer { get; } = new long[] {0x0000000, 0x00, 0x00 };
        public IReadOnlyList<long> CurrentBoxPointer { get; } = new long[] {0x0000000, 0x0000, 0x000 };
        public IReadOnlyList<long> PortalBoxStatusPointer { get; } = new long[] { 0x0000000, 0x000, 0x000, 0x00, 0x000, 0x00 };  // 9-A in portal, 4-6 in box.
        public IReadOnlyList<long> IsConnectedPointer { get; } = new long[] { 0x0000000, 0x00 };
        public IReadOnlyList<long> OverworldPointer { get; } = new long[] { 0x0000000, 0x000, 0x00, 0x00, 0x00 };

        public const int BoxFormatSlotSize = 0x158;
        public const ulong LibAppletWeID = 0x010000000000100a; // One of the process IDs for the news.
    }
}
