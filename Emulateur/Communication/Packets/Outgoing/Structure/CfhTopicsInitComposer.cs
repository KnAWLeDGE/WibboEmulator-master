﻿using Butterfly.HabboHotel.Support;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing
{
    class CfhTopicsInitComposer : ServerPacket
    {
        public CfhTopicsInitComposer(Dictionary<string, List<ModerationPresetActions>> UserActionPresets)
            : base(ServerPacketHeader.CfhTopicsInitMessageComposer)
        {

            WriteInteger(UserActionPresets.Count);
            foreach (KeyValuePair<string, List<ModerationPresetActions>> Cat in UserActionPresets.ToList())
            {
                WriteString(Cat.Key);
                WriteInteger(Cat.Value.Count);
                foreach (ModerationPresetActions Preset in Cat.Value.ToList())
                {
                    WriteString(Preset.Caption);
                    WriteInteger(Preset.Id);
                    WriteString(Preset.Type);
                }
            }
        }
    }
}
