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
        Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte highIP = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        byte lowIP  = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
        InstructionPointer = (short)((short)(highIP << 8) + (short)(lowIP));
    }

    public void PHA()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF-StackPointer), A);
    }

    public void PHP()
    {
        StackPointer--;
        Memory.SetByteAtAddress((short)(0x01FF - StackPointer), Flag);
    }

    public void PLA()
    {
        A = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }

    public void PLP()
    {
        Flag = Memory.GetByteAtAddress((short)(0x01FF - StackPointer));
        StackPointer++;
    }

    // 0xAA
    public void TAB()
    {
        B = A;
    }

    // 0xA8
    public void TAC()
    {
        C = A;
    }

    // 0xBA
    public void TSB()
    {
        B = StackPointer;
    }

    // 0x8A
    public void TBA()
    {
        A = B;
    }

    //0x9A
    public void TBS()
    {
        StackPointer = B;
    }

    // 0x98
    public void TCA()
    {
        A = C;
    }

    // 0x38
    public void SEC()
    {
        Flag |= (1 << 0);
    }

    // 0xF8
    public void SED()
    {
        Flag |= (1 << 3);
    }

    // 0x78
    public void SEI()
    {
        Flag |= (1 << 2);
    }
}