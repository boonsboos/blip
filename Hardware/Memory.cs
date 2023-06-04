namespace Hardware;

// the BLIP machine has a 16 bit address bus and 64K of addressable memory.
// addresses are derived big endian: high byte first, low byte second.

public static class Memory
{
    private static byte[] mem = new byte[65536];

    // gets a byte from an address
    public static byte GetByteAtAddress(byte hi, byte lo)
    {
        return mem[((short) hi << 8) + ((short) lo)];
    }

    public static byte GetByteAtAddress(short addr)
    {
        return mem[addr];
    }

    // sets byte at address.
    public static void SetByteAtAddress(byte hi, byte lo, byte data)
    {
        mem[((short) hi << 8) + ((short) lo)] = data;
    }

    public static void SetByteAtAddress(short addr, byte data)
    {
        mem[addr] = data;
    }

    // the 0 address should always be 0;
    public static byte GetNull()
    {
        return mem[0];
    }

    public static short GetNMI()
    {
        short a = (short) mem[0xFFFA];
        short b = (short) mem[0xFFFB]; 
        return (short)(a << 8 + b); 
    }

    public static short GetReset()
    {
        short a = (short) mem[0xFFFC];
        short b = (short) mem[0xFFFD]; 
        return (short)(a << 8 + b); 
    }

    public static short GetIRQ()
    {
        short a = (short) mem[0xFFFE];
        short b = (short) mem[0xFFFF]; 
        return (short)(a << 8 + b); 
    }
}