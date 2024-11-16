using System;

namespace ADLMidi.NET;

[Flags]
public enum GlobalBankFlags : byte
{
    DeepTremolo = 1,
    DeepVibrato = 2,
}