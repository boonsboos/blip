namespace Hardware;

// the BLIP processor is based on the instruction set and behaviour of the MOS 6502.
// it is also simplified because we don't need every single instruction.

using Register = Byte;

public class Processor
{
    public Processor() { }

    private static Register A = 0;
    private static Register B = 0;
    private static Register C = 0;

    private static byte  StackPointer       = 0; // 0x1FF - 0x0100
    private static byte  Flag               = 0;
    private static short InstructionPointer = 0; // also known as the program counter

    public void RTI()
    {
        this.Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte highIP = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte lowIP  = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        this.InstructionPointer = ((short)(highIP) << 8 + (short)(lowIP));
    }

    public void PHA()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF-StackPointer), this.A);
    }

    public void PHP()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF - StackPointer), this.Flag);
    }

    public void PLA()
    {
        this.A = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }

    public void PLP()
    {
        this.Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }
}