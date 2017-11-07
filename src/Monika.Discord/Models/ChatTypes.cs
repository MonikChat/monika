using System;

namespace Monika.Models
{
    [Flags]
    public enum ChatTypes
    {
        Guild = 0b01,
        DM    = 0b10,
        All   = ChatTypes.Guild | ChatTypes.DM,
    }
}