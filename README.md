# AdlMidi.NET
AdlMidi.NET is a .NET wrapper for the libADLMIDI library, a free Software MIDI synthesizer library with OPL3 emulation

Original ADLMIDI code: Copyright (c) 2010-2014 Joel Yliluoma <bisqwit@iki.fi>

ADLMIDI Library API:   Copyright (c) 2015-2020 Vitaly Novichkov <admin@wohlnet.ru>

AdlMidi.NET:           Copyright (c) 2020 Cam Sinclair <csinkers@gmail.com>

[https://github.com/Wohlstand/libADLMIDI](https://github.com/Wohlstand/libADLMIDI)

# License

The .NET wrapper code is licensed as LGPL v2.1+, and the libADLMIDI library and its parts are licensed as follows (from the libADLMIDI readme):

The library is licensed under in it's parts LGPL 2.1+, GPL v2+, GPL v3+, and MIT.
* Nuked OPL3 emulators are licensed under LGPL v2.1+.
* DosBox OPL3 emulator is licensed under GPL v2+.
* Chip interfaces are licensed under LGPL v2.1+.
* File Reader class and MIDI Sequencer is licensed under MIT.
* WOPL reader and writer module is licensed under MIT.
* Other parts of library are licensed under GPLv3+.

# libADLMIDI build settings

The libADLMIDI.so / .dll files incuded in the NuGet package were generated via the following commands:

## Linux:
`
mkdir build
cd build
cmake -DCMAKE_BUILD_TYPE=Release \
    -DlibADLMIDI_STATIC=OFF \
    -DlibADLMIDI_SHARED=ON \
    -DWITH_EMBEDDED_BANKS=OFF \
    -DWITH_MUS_SUPPORT=OFF \
    -DWITH_XMI_SUPPORT=ON \
    -DUSE_DOSBOX_EMULATOR=OFF \
    -DUSE_NUKED_EMULATOR=ON \
    -DUSE_OPAL_EMULATOR=OFF \
    -DUSE_JAVA_EMULATOR=OFF \
    ..
make
[ -d ../../ADLMidi.NET/runtimes/linux-x64/native ] || mkdir -p ../../ADLMidi.NET/runtimes/linux-x64/native
cp -L libADLMIDI.so ../../ADLMidi.NET/runtimes/linux-x64/native
`

## Windows:
* Run CMake setup program
* Pick libADLMIDI directory
* Pick release build
* Disable all emulators other than Nuked
* Disable static, enable shared
* Ensure XMI support and MIDI sequencer are enabled
* Disable MUS support and embedded banks (not required)
* Build generated solution file using VS2019
* Copy resultant dll to ADLMidi.NET runtimes/win-x64/native directory

