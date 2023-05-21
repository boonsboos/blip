namespace Hardware;

// the BLIP processor is based on the instruction set and behaviour of the MOS 6502.
// it is also simplified because we don't need every single instruction.

using Register = Byte;

public class Processor
{
    Memory mem;

    public Processor() {
        mem = new Memory();
    }

    private static Register A = 0;
    private static Register B = 0;
    private static Register C = 0;

    private static byte  StackPointer       = 0; // 0x1FF - 0x0100
    private static byte  Flag               = 0;
    private static short InstructionPointer = 0; // also known as the program counter

    public void RTI() {
        Flag = mem.getByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte highIP = mem.getByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte lowIP  = mem.getByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        InstructionPointer = (short)(highIP << 8 + lowIP);
    }
}