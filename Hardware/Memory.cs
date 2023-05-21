namespace Hardware;

// the BLIP machine has a 16 bit address bus and 64K of addressable memory.
// addresses are derived big endian: high byte first, low byte second.

public class Memory
{
    private byte[] mem = new byte[65536];

    // gets a byte from an address
    public byte getByteAtAddress(byte one, byte two) {
        return mem[((short) one << 8) + ((short) two)];
    }

    public byte getByteAtAddress(short one) {
        return mem[one];
    }

    // the 0 address should always be 0;
    public byte getNull() {
        return mem[0];
    }

    public short GetNMI() {
        short a = (short) mem[0xFFFA];
        short b = (short) mem[0xFFFB]; 
        return (short)(a << 8 + b); 
    }

    public short GetReset() {
        short a = (short) mem[0xFFFC];
        short b = (short) mem[0xFFFD]; 
        return (short)(a << 8 + b); 
    }

    public short GetIRQ() {
        short a = (short) mem[0xFFFE];
        short b = (short) mem[0xFFFF]; 
        return (short)(a << 8 + b); 
    }
}