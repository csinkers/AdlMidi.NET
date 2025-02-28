using System;

namespace ADLMidi.NET;

/// <summary>
/// Represents a dynamic bank in the ADLMIDI library.
/// </summary>
public class Bank
{
    readonly IntPtr _device;
    AdlMidiImports.Bank _bank;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bank"/> class.
    /// </summary>
    /// <param name="device">The device pointer.</param>
    /// <param name="bank">The bank structure.</param>
    internal Bank(IntPtr device, AdlMidiImports.Bank bank)
    {
        _device = device;
        _bank = bank;
    }

    /// <summary>
    /// Gets the identifier of the bank.
    /// </summary>
    /// <returns>The <see cref="BankId"/> of the bank.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation fails.</exception>
    public BankId GetBankId()
    {
        var result = AdlMidiImports.adl_getBankId(_device, ref _bank, out var id);
        if (result < 0)
            throw new InvalidOperationException();

        return id;
    }

    /// <summary>
    /// Removes the bank.
    /// </summary>
    /// <returns>An integer indicating the result of the operation.</returns>
    public int RemoveBank()
    {
        return AdlMidiImports.adl_removeBank(_device, ref _bank);
    }

    /// <summary>
    /// Gets the instrument at the specified index.
    /// </summary>
    /// <param name="index">The index of the instrument.</param>
    /// <returns>The <see cref="Instrument"/> at the specified index.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation fails.</exception>
    public Instrument GetInstrument(uint index)
    {
        var result = AdlMidiImports.adl_getInstrument(_device, ref _bank, index, out var instrument);
        if (result < 0)
            throw new InvalidOperationException();

        return instrument;
    }

    /// <summary>
    /// Sets the instrument at the specified index.
    /// </summary>
    /// <param name="index">The index of the instrument.</param>
    /// <param name="ins">The instrument to set.</param>
    /// <returns>An integer indicating the result of the operation.</returns>
    public int SetInstrument(uint index, ref Instrument ins)
    {
        return AdlMidiImports.adl_setInstrument(_device, ref _bank, index, ref ins);
    }

    /// <summary>
    /// Loads an embedded bank.
    /// </summary>
    /// <param name="num">The number of the embedded bank to load.</param>
    /// <returns>An integer indicating the result of the operation.</returns>
    public int LoadEmbeddedBank(int num)
    {
        return AdlMidiImports.adl_loadEmbeddedBank(_device, ref _bank, num);
    }
}